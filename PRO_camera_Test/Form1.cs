using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Video.DirectShow;
using AForge.Imaging;
using AForge.Vision.Motion;
using System.Drawing.Imaging;
using System.IO;
using AForge.Video;
using AForge.Imaging.Filters;
using AForge;
using AForge.Math;
using System.Threading;
using NKH.MindSqualls;
using NKH.MindSqualls.MotorControl;
using System.Diagnostics;
using AForge.Math.Geometry;
using System.Drawing.Drawing2D;
using AForge.Robotics.Lego;
/*
u kazdeho blobu ukazat hue a size
rozpoznat, o jakou kosticku se jedna pomoci velikosti
zkusit ukladat obr i s alfa kanalem
zaporne HUE
drak and drop - biggest ma pozadi
spocitat pixely, ktere obrazek tvori, ne S x V!!!
jednotlive kosticky jako objekty s dinamickou velikosti, odvyjejici se z rozliseni
fill holes pri pocitani velikosti (cerne pozedi; prahovani, pak fill holes, spocitat, puvodni obr)
pokud více nez 4 rohy, pak kosticka L
ocrana proti zaseknuti na prvnim pasu a kolama - jednou za cas kola roztocit na druhou stranu; 
kola jsou pripojeny na brickbezDisp pres Bluetooth
*/
namespace PRO_camera_Test{
    public partial class Form1 : Form{
        public Form1(){
            InitializeComponent();
            //D
            pictureBox2.AllowDrop = true;
            pictureBox2.DragEnter += pictureBox2_DragEnter;
            //pictureBox2.DragDrop += pictureBox2_DragDrop;
            //D
        }
        //-------konfigurace NXT blokuu -------
        //brick ma display a je pres USB
        //brick.MotorA = rampa do krabicek 
        //brick.MotorB =  pas 2 (ten nizsi, jednoduchy)
        //brick.MotorC =  pas 1 (nahore)

        public static NxtBrick brick = new NxtBrick(NxtCommLinkType.USB, 3);
        public static NXTBrick brickBezDisp = new NXTBrick();

        //-------/konfigurace NXT blokuu -------
        FilterInfoCollection filterInfColl;
        public VideoCaptureDevice videoDevice;
        MotionDetector motionDetector;
        float f;
        private static int thresholdBG, blobMinWidth = 40, blobMinHeight = 40;
        private Boolean automaticRun = false, croppingImagePressed = false;
        public int qqq;
        static int blobArea = 0;
        static int minBlobArea;
        private static int currentBox, requiredBox;
        private static bool rotateDone = false, stopStartVideo=true, newObjectDetect=false;
        Thread thread1;
        private string directoryPath = @".\";//ukladaji se sem sejmute obrazky
        private string biggestBlobInfo = "";//vykresli se do pictureBoxu2 info o objektu (hue a size)
        internal void setCropRectangle() {
            Console.WriteLine(qqq);
        }

        private void Form1_Load(object sender, EventArgs e){

            motionDetector = new MotionDetector(new TwoFramesDifferenceDetector(), new MotionAreaHighlighting());

            filterInfColl = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo item in filterInfColl){
                comboBoxDevice.Items.Add(item.Name);
            }
            comboBoxDevice.SelectedIndex = 0;

            thresholdBG = (int)numericUpDown1Bakground.Value;
            minBlobArea = (int)numUpDownMinBlobSize.Value;
            blobMinWidth = (int)numericUpDown2BlobSizeW.Value;
            blobMinHeight = (int)numericUpDown3BlobSizeH.Value;
/*
            for (int i = 0; i < videoDevice.VideoCapabilities.Length; i++) {

                string resolution = "Resolution Number " + Convert.ToString(i) + "\n";
                string resolution_size = videoDevice.VideoCapabilities[i].FrameSize.ToString();

                Console.WriteLine(resolution + resolution_size);
                comboBoxMode.Items.Add(resolution + resolution_size);

            }*/
            /*
                        videoDevice = new VideoCaptureDevice(filterInfColl[comboBoxDevice.SelectedIndex].MonikerString);
                        comboBoxMode.Items.Clear();
                        // videoDevice = filterInfColl.VideoCapabilities;
                        videoDevice as VideoCapabilities;
                        foreach (VideoCapabilities capability in videoDevice) {
                            string item = string.Format("{0} x {1}", capability.FrameSize.Width, capability.FrameSize.Height);
                            if (!comboBoxMode.Items.Contains(item)) {
                                comboBoxMode.Items.Add(item);
                            }
                        }
                        if (videoCapabilities_cam1.Length == 0) {
                            comboBoxMode.Items.Add("Not Supported");
                        }
                        comboBoxMode.SelectedIndex = 0;
               */

            getHueFromRGB(66, 153, 8);

            groupBox1.Enabled = false;
            groupBox2.Enabled = false;
            checkBoxEdgeCorners.Enabled = false;

            if (brickBezDisp.Connect("COM7")) {
                System.Diagnostics.Debug.WriteLine("Connected successfully");

                /*  
                // enable controls
                  resetMotorButton.Enabled = true;
                  setMotorStateButton.Enabled = true;
                  getMotorStateButton.Enabled = true;
                  getInputButton.Enabled = true;
                  setInputModeButton.Enabled = true;
                  */
                // get device information
                string deviceName;
                byte[] btAddress;
                int btSignalStrength;
                int freeUserFlesh;

                brickBezDisp.GetDeviceInformation(out deviceName, out btAddress, out btSignalStrength, out freeUserFlesh);
                Console.WriteLine("deviceName: "+deviceName + ", btAddress " + btAddress+ ", btSignalStrength " + btSignalStrength+ ", freeUserFlesh " + freeUserFlesh);
                int batteryLevel;
                brickBezDisp.GetBatteryPower(out batteryLevel);
                Console.WriteLine("batteryLevel: " + batteryLevel);
            }
        }

        private void btnStart_Click(object sender, EventArgs e) {
            //   videoDevice = new VideoCaptureDevice(filterInfColl[comboBoxDevice.SelectedIndex].MonikerString);
            videoDevice = new VideoCaptureDevice(filterInfColl[comboBoxDevice.SelectedIndex].MonikerString);
            videoDevice.DesiredFrameRate = 60;
            //videoDevice.DesiredFrameSize = new Size(320, 240);
            //videoDevice.SetCameraProperty();
            //  videoDevice.DisplayPropertyPage(IntPtr.Zero);
            //System.Threading.Thread.Sleep(5000);
            thread1 = new Thread(new ThreadStart(motoryPasuuZvedak));

            for (int i = 0; i < videoDevice.VideoCapabilities.Length; i++) {

                string resolution = "Resolution Number " + Convert.ToString(i)+"\n";
                string resolution_size = videoDevice.VideoCapabilities[i].FrameSize.ToString();
                comboBoxMode.Items.Add(resolution + resolution_size);//srpen
                //pokud bych chtel rozliseni zadavat rucne, pridam do vyberu zarizeni prazdne misto a po vzberu skutecneh kamerz, hned nactu moznosti rozliseni
                Console.WriteLine(resolution_size);
            }
            videoDevice.VideoResolution = videoDevice.VideoCapabilities[2];//14
            videoSourcePlayer1.VideoSource = videoDevice;
            
            stopStartVideo = true;
            /**
            Resolution Number 0
            {Width=640, Height=480}
            Resolution Number 1
            {Width=160, Height=90}
            Resolution Number 2
            {Width=160, Height=120}
            Resolution Number 3
            {Width=176, Height=144}
            Resolution Number 4
            {Width=320, Height=180}
            Resolution Number 5
            {Width=320, Height=240}
            Resolution Number 6
            {Width=352, Height=288}
            Resolution Number 7
            {Width=432, Height=240}
            Resolution Number 8
            {Width=640, Height=360}
            Resolution Number 9
            {Width=800, Height=448}
            Resolution Number 10
            {Width=800, Height=600}
            Resolution Number 11
            {Width=864, Height=480}
            Resolution Number 12
            {Width=960, Height=720}
            Resolution Number 13
            {Width=1024, Height=576}
            Resolution Number 14
            {Width=1280, Height=720}
            Resolution Number 15
            {Width=1600, Height=896}
            Resolution Number 16
            {Width=1920, Height=1080}
            Resolution Number 17
            {Width=2304, Height=1296}
            Resolution Number 18
            {Width=2304, Height=1536}
*/
           // videoSourcePlayer1.NewFrame +=  new AForge.Controls.VideoSourcePlayer.NewFrameHandler(videoSourcePlayer1_NewFrame);
            videoSourcePlayer1.Start();
            
            /*if (videoSourcePlayer1.IsRunning) {
                pictureBox2.Image = (Bitmap)videoSourcePlayer1.GetCurrentVideoFrame();
            }*/

            /*************************/
            ////// Pokus se program spousti pro testovaci ucely bez pripojenych NXT, je nutne zakomentovat spousteni threadu1 (//thread1.Start();). Predejde se tak padu
            //  thread1.Start();
            /*************************/

            //Form1 thread1 = new Form1();
            //new Thread(thread1.motoryPasuuZvedak).Start();

        }

        private void btnStop_Click(object sender, EventArgs e) {
            if (videoSourcePlayer1.IsRunning) {
                videoSourcePlayer1.Stop();
                stop = true;
            }
        }

        private static Bitmap removeBackground(Bitmap image) {
            // lock image
            /*  BitmapData bitmapData = image.LockBits(
                  new Rectangle(0, 0, image.Width, image.Height),
                  ImageLockMode.ReadWrite, image.PixelFormat);*/

            // odstraneni cerneho pozadi

            ColorFiltering colorFilter = new ColorFiltering();

            colorFilter.Red = new IntRange(0, thresholdBG);
            colorFilter.Green = new IntRange(0, thresholdBG);
            colorFilter.Blue = new IntRange(0, thresholdBG);
            colorFilter.FillOutsideRange = false;

            Bitmap processedImage = colorFilter.Apply(image);
            return processedImage;
           /* colorFilter.ApplyInPlace(bitmapData);

            // step 2 - locating objects
            BlobCounter blobCounter = new BlobCounter();

            blobCounter.FilterBlobs = true;
            blobCounter.MinHeight = 5;
            blobCounter.MinWidth = 5;

            blobCounter.ProcessImage(bitmapData);
            Blob[] blobs = blobCounter.GetObjectsInformation();
            image.UnlockBits(bitmapData);
            return image;*/
        }
        private Bitmap imageOfRamp = null;
        private void rampImage() {
            //ulozi do promenne obazek podlozky pod kamerou pro pozdejsi zjisteni, zda se tam objevila kostka lega
            imageOfRamp = (Bitmap)pictureBox2.Image;
        }


        //******************* \/\/\ROI******************************
        private System.Drawing.Point RectStartPoint;
        private Rectangle rectangleForCropping = new Rectangle();
        private Rectangle croppedRectangle = new Rectangle();
        Region regionOI = new Region();
        private Brush selectionBrush = new SolidBrush(Color.FromArgb(120, 70, 155, 230));
        
        // Start Rectangle
        //
        private void pictureBox2_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            // Determine the initial rectangle coordinates...
            RectStartPoint = e.Location;
            Invalidate();
        }

        // Draw Rectangle
        //
        private void pictureBox2_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e) {
            if (e.Button != MouseButtons.Left)
                return;
            System.Drawing.Point tempEndPoint = e.Location;
            rectangleForCropping.Location = new System.Drawing.Point(
                Math.Min(RectStartPoint.X, tempEndPoint.X),
                Math.Min(RectStartPoint.Y, tempEndPoint.Y));
            rectangleForCropping.Size = new Size(
                Math.Abs(RectStartPoint.X - tempEndPoint.X),
                Math.Abs(RectStartPoint.Y - tempEndPoint.Y));
            pictureBox2.Invalidate();
            
        }

        // Draw Area
        //
        private System.Windows.Forms.PaintEventArgs ee;
        private void pictureBox2_Paint(object sender, System.Windows.Forms.PaintEventArgs e){
            // Draw the rectangle...
            ee = e;
            if (pictureBox2.Image != null){
                if (rectangleForCropping != null && rectangleForCropping.Width > 0 && rectangleForCropping.Height > 0)
                {
                    // e.Graphics.FillEllipse(selectionBrush, rectangleForCropping);// (Color.Azure);// FillRectangle(selectionBrush, rectangleForCropping);
                    // oznaci se vse krome vybraneho obdelniku
                    regionOI.MakeInfinite();
                    regionOI.Exclude(rectangleForCropping);
                    ee.Graphics.FillRegion(selectionBrush, regionOI);
                }
            }
        }

        private void pictureBox2_MouseUp(object sender, MouseEventArgs e){
            if (e.Button == MouseButtons.Right)
            {
                if (rectangleForCropping.Contains(e.Location))
                {
                    Debug.WriteLine("Right click");
                }
            }
        }
        private void pictureBox2_MouseClick(object sender, MouseEventArgs e){

        }
        //////******************* \/\/\ROI******************************
        //*************************  Difference  ***************************
        private void btnGo_Click(Bitmap image1, Bitmap image2)
        {
            Bitmap bm1 = (Bitmap)pictureBox2.Image;

            // Make a difference image.
            int wid = Math.Min(bm1.Width, imageOfRamp.Width);
            int hgt = Math.Min(bm1.Height, imageOfRamp.Height);
            Bitmap image3 = new Bitmap(wid, hgt);

            // Create the difference image.
            bool are_identical = true;
            Color eq_color = Color.White;
            Color ne_color = Color.Red;
            for (int x = 0; x < wid; x++){
                for (int y = 0; y < hgt; y++) {
                    if (bm1.GetPixel(x, y).Equals(imageOfRamp.GetPixel(x, y)))
                        image3.SetPixel(x, y, eq_color);
                    else {
                        image3.SetPixel(x, y, ne_color);
                        are_identical = false;
                    }
                }
            }

            // Display the result.
            pictureBox2.Image = image3;

            if ((bm1.Width != imageOfRamp.Width) || (bm1.Height != imageOfRamp.Height)) are_identical = false;
            if (are_identical) richTextBox2.Text = "----The images are identical\n" + richTextBox2.Text + "\n";
            else richTextBox2.Text = "----The images are different\n" + richTextBox2.Text + "\n";
        }
        //***********************\\\\\\\ Difference  ***************************
        //NewFrameNewFrameNewFrameNewFrameNewFrameNewFrameNewFrameNewFrameNewFrameNewFrameNewFrameNewFrameNewFrameNewFrameNewFrame
        //NewFrameNewFrameNewFrameNewFrameNewFrameNewFrameNewFrameNewFrameNewFrameNewFrameNewFrameNewFrameNewFrameNewFrameNewFrame
        //NewFrameNewFrameNewFrameNewFrameNewFrameNewFrameNewFrameNewFrameNewFrameNewFrameNewFrameNewFrameNewFrameNewFrameNewFrame
        private void videoSourcePlayer1_NewFrame(object sender, ref Bitmap image) {
            //public void videoSourcePlayer1_NewFrame(object sender, NewFrameEventArgs eventArgs) {
            

            // pravraceni obrazu o 180 stupnu
            Mirror filter180 = new Mirror(true, true);
            filter180.ApplyInPlace(image);

            Bitmap bitmap2 = new Bitmap(image);
            Bitmap bitmap3 = new Bitmap(image);

            // create filter
            BrightnessCorrection filter = new BrightnessCorrection(-50);
            // apply the filter
            //filter.ApplyInPlace(bitmap2);

            // pozdeji treba switch


            // if (croppingImagePressed && rectangleForCropping != null && rectangleForCropping.Width > 0 && rectangleForCropping.Height > 0)
            if (croppingImagePressed) {
                bitmap2 = CropImage(bitmap2, croppedRectangle.X, croppedRectangle.Y, croppedRectangle.Width, croppedRectangle.Height);
            }

            /*Bitmap result = new Bitmap(bitmap2.Width, bitmap2.Height, PixelFormat.Format8bppIndexed);

            Bitmap newBitmap = new Bitmap(result.Width, result.Height);
            Graphics graphics = Graphics.FromImage(newBitmap);
            graphics.DrawImage(result,  0, 0);
            */
            /*using (Graphics g = Graphics.FromImage(result)) {
                g.DrawImage(bitmap2, 0, 0, image.Width, image.Height);
            }*/
            //  bitmap2 = newBitmap;
            /*if((Bitmap)pictureBox1.Image != null) { 
            Difference filter3 = new Difference((Bitmap)pictureBox1.Image);
            // apply the filter
            bitmap2 = filter3.Apply(bitmap2);
            }*/

            // create processing filters sequence
            if (imageOfBackground != null) { 
                FiltersSequence processingFilter = new FiltersSequence();
                processingFilter.Add(new Difference(imageOfBackground));
                processingFilter.Add(new Grayscale(0.2126, 0.7152, 0.0722));
                processingFilter.Add(new Threshold(155));
                processingFilter.Add(new Opening());
                processingFilter.Add(new Edges());
                processingFilter.Add(new Dilatation());
                processingFilter.Add(new Dilatation());
                // apply the 

                FillHoles filter2 = new FillHoles();
                filter2.MaxHoleHeight = 500;
                filter2.MaxHoleWidth = 500;
                filter2.CoupledSizeFiltering = false;
                // apply the filter

                Bitmap tmp1 = processingFilter.Apply(bitmap3);
                tmp1 = filter2.Apply(tmp1);
                Bitmap result = bitmap3;
                for (int i = 0; i < tmp1.Width; i++) {
                    for (int j = 0; j < tmp1.Height; j++) {
                        if (tmp1.GetPixel(i, j) == Color.White || tmp1.GetPixel(i, j).B == 255) {
                            result.SetPixel(i, j, bitmap3.GetPixel(i, j));
                        } else result.SetPixel(i, j, Color.Black);
                    }
                }
                bitmap2 = result;
                bitmap2 = ConvertTo8bpp(bitmap2);

                /*                
                // extract red channel from the original image
                IFilter extrachChannel = new ExtractChannel(RGB.R);
                Bitmap redChannel = extrachChannel.Apply(bitmap3);
                //  merge red channel with moving object borders
                Merge mergeFilter = new Merge();
                mergeFilter.OverlayImage = tmp1;
                Bitmap tmp2 = mergeFilter.Apply(redChannel);
                // replace red channel in the original image
                ReplaceChannel replaceChannel1 = new ReplaceChannel(RGB.R, tmp2);
                replaceChannel1.ChannelImage = tmp2;
                Bitmap tmp3 = replaceChannel1.Apply(bitmap3);
                
                bitmap2 = tmp3;*/
            }
            ////////////////////////////////////////////AForge.Imaging.Filters.Difference;


            int pom;
            for (int i = 0; i < bitmap2.Width; i++) {
                for (int j = 0; j < bitmap2.Height; j++) {
                    //bitmap2.SetPixel(i,j,(Color)(bitmap2.GetPixel(i,j).R * 7 / 255) << 5 + (bitmap2.GetPixel(i, j).G * 7 / 255) << 2 + (bitmap2.GetPixel(i, j).B * 3 / 255));
                }
            }
            
            if (checkBox1RemBack.Checked || automaticRun) { 
                bitmap2 = removeBackground(bitmap2);
            }

            if(checkBox2Blob.Checked || automaticRun) {

                bitmap2 = BlobDetection(bitmap2);
                /*Grayscale filter = new Grayscale(0.2125, 0.7154, 0.0721);
                // apply the filter
                bitmap2 = filter.Apply(bitmap2);
                */
            }


            // f = motionDetector.ProcessFrame(bitmap2);
            //RGB w = new RGB();
            //HSL p = FromRGB(w);

            //pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage; //roztazeni dle velikosti boxu
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            this.Invoke((MethodInvoker)delegate {
                  pictureBox2.Image = bitmap2;
                  pictureBox2.Refresh();
            });

        }
        //NewFrameNewFrameNewFrameNewFrameNewFrameNewFrameNewFrameNewFrameNewFrameNewFrameNewFrameNewFrameNewFrameNewFrameNewFrame
        //NewFrameNewFrameNewFrameNewFrameNewFrameNewFrameNewFrameNewFrameNewFrameNewFrameNewFrameNewFrameNewFrameNewFrameNewFrame
        //NewFrameNewFrameNewFrameNewFrameNewFrameNewFrameNewFrameNewFrameNewFrameNewFrameNewFrameNewFrameNewFrameNewFrameNewFrame


        public static Bitmap ConvertTo8bpp(System.Drawing.Image img) {
            var ms = new System.IO.MemoryStream();   // Don't use using!!!
            img.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
            ms.Position = 0;
            return new Bitmap(ms);
        }


        private void timer1_Tick(object sender, EventArgs e) {
            label3.Text = "Value: " + f.ToString();
        }

        // ================== Ovladani NXT ===================
        private void motoryPasuuZvedak() {
            /*               Console.WriteLine("Ahoj!");
                           Console.WriteLine("run motor A 16");
                           // brick.MotorC = new NxtMotor();
                           brick.MotorA = new NxtMotor();

                           // brick.MotorC.PollInterval = 90;
                           brick.MotorA.PollInterval = 90;
                           //  brick.Connect();
                           brick.Connect();
                           //while (true) { 
                           // if (brick2.IsConnected) { 
                           brick.MotorA.Run(30, 400);
                           brick.Disconnect();

                   */
            /*
            //,,,,,,,,,,,,,,,,,,,,,,,,,,
     Thread.Sleep(7000);
     this.Invoke((MethodInvoker)delegate {
         Bitmap bbb;
         if (videoSourcePlayer1.IsRunning) {
             videoSourcePlayer1.Stop();
             stop = true;
         }
         Thread.Sleep(500);
         videoDevice.VideoResolution = videoDevice.VideoCapabilities[6];//14 { Width = 1280, Height = 720}
         videoSourcePlayer1.VideoSource = videoDevice;
         videoSourcePlayer1.Start();
         Thread.Sleep(6000);
         bbb = (Bitmap)videoSourcePlayer1.GetCurrentVideoFrame().Clone();
         Bitmap current = bbb;
         string filepath = Environment.CurrentDirectory;
         string fileName = System.IO.Path.Combine(filepath, @"name.bmp");
         bbb.Save(fileName);
         bbb.Dispose();
     });
     //,,,,,,,,,,,,,,,,,,,,,,,,,,
     */

            // run motor A
            NXTBrick.MotorState motorWheels = new NXTBrick.MotorState();
            NXTBrick.MotorState motorElevator = new NXTBrick.MotorState();
            NXTBrick.MotorState motorWheelsStop = new NXTBrick.MotorState();
            NXTBrick.MotorState motorElevatorStop = new NXTBrick.MotorState();
            NXTBrick.MotorState motorBoxes = new NXTBrick.MotorState();

            motorWheels.Power = 62;
            motorWheels.TurnRatio = 10;
            motorWheels.Mode = NXTBrick.MotorMode.On;
            motorWheels.Regulation = NXTBrick.MotorRegulationMode.Idle;
            motorWheels.RunState = NXTBrick.MotorRunState.Running;
            motorWheels.TachoLimit = 0;

            motorElevator.Power = 30;
            motorElevator.TurnRatio = 10;
            motorElevator.Mode = NXTBrick.MotorMode.On;
            motorElevator.Regulation = NXTBrick.MotorRegulationMode.Idle;
            motorElevator.RunState = NXTBrick.MotorRunState.Running;
            motorElevator.TachoLimit = 0;

            motorWheelsStop.Mode = NXTBrick.MotorMode.Brake;
            motorElevatorStop.Mode = NXTBrick.MotorMode.Brake;

            brick.MotorA = new NxtMotor(); // rampa do krabicek
            brick.MotorB = new NxtMotor(); // pas 2 (ten nizsi, jednoduchy)
            brick.MotorC = new NxtMotor(); // pas 1 (nahore)

           // brickBezDisp.SetMotorState(NXTBrick.Motor.B, motorWheels); // kola na prvnim pasu nahore
           // brickBezDisp.SetMotorState(NXTBrick.Motor.C, motorElevator); // vytah ze zasobniku na pas
            // Poll it every 50 milliseconds.
            brick.MotorA.PollInterval = 10;
            brick.MotorB.PollInterval = 50;
            brick.MotorC.PollInterval = 50;
            //  brick.Connect();
            //brick.MotorA.ResetMotorPosition(true);

            brick.MotorA.ResetMotorPosition(false);
            brick.MotorA.ResetMotorPosition(true);
            brick.CommLink.ResetMotorPosition(NxtMotorPort.PortA, true);
            brick.CommLink.ResetMotorPosition(NxtMotorPort.PortA, false);
            brick.MotorA.TriggerTachoCount = 0;

            //brick.MotorA.

            brick.Connect();
            brick.MotorA.ResetMotorPosition(false);
            brick.MotorA.ResetMotorPosition(true);
            brick.CommLink.ResetMotorPosition(NxtMotorPort.PortA, true);
            brick.CommLink.ResetMotorPosition(NxtMotorPort.PortA, false);

            brick.MotorA.TriggerTachoCount = 0;

            //while (true) { 
            // if (brick2.IsConnected) {
            //brick.MotorA.Run(-15, 0);
            brick.MotorB.Run(-35, 0);//-15
            brick.MotorC.Run(-20, 0);
            Console.WriteLine("run1 blobArea: " + Form1.blobArea);
            int milliseconds = 200;
            while (true) {
                /* if (Form1.blobArea < minBlobArea) {
                     brick.MotorA.Run(-30, 2);
                     Thread.Sleep(milliseconds);
                 //     Console.WriteLine("TachC: " + brick.MotorA.TachoCount);
             } else {*/
                //brick.MotorA.Run(-30, 0);
                //=========================== Prisela kostka lega ================================================
                if (Form1.blobArea > minBlobArea || Form1.stop) {
                  //  brick.MotorA.Brake();//**************rampa do krabicek
                    brick.MotorB.Brake();// pas 2 (dole)
                    brick.MotorC.Brake();// pas 1 (nahore)
                    brickBezDisp.SetMotorState(NXTBrick.Motor.B, motorWheelsStop);
                    brickBezDisp.SetMotorState(NXTBrick.Motor.C, motorElevatorStop);
                    //**********************************  vyklapeni a zklapeni
                    // Thread.Sleep(milliseconds);
                    Thread.Sleep(400);

                    //-------------------------- Zajisteni kvalitnejsiho obrazu -----------------------

                    /*
                    this.Invoke((MethodInvoker)delegate {
                        Bitmap bbb;
                        if (videoSourcePlayer1.IsRunning) {
                            videoSourcePlayer1.Stop();
                            stop = true;
                        }
                        Thread.Sleep(500);
                        videoDevice.VideoResolution = videoDevice.VideoCapabilities[6];//14 { Width = 1280, Height = 720}
                        videoSourcePlayer1.VideoSource = videoDevice;
                        videoSourcePlayer1.Start();
                        bbb= (Bitmap)videoSourcePlayer1.GetCurrentVideoFrame().Clone();
                        Bitmap current = (Bitmap)bbb.Clone();
                        string filepath = Environment.CurrentDirectory;
                        string fileName = System.IO.Path.Combine(filepath, @"name.bmp");
                        current.Save(fileName);
                        current.Dispose();
                    });
                    */
                    
                    //-------------------------- //Zajisteni kvalitnejsiho obrazu -----------------------

                    // getAVG_Hue();   // musi se tu provest pocitani HUE a pak se vypne kamera a pripoji s dobrym rozlisenim

                    //btnStopEvent();

                    // je tu promennna 'rotateDone', ktera se  zmeni na true, kdyz dokonci otoceni buxu na ten spravny
                    Box();  //provede se otoceni krabicek na tu spravnou
                    while (rotateDone) {
                        Thread.Sleep(50);
                    }
                    Console.WriteLine("currentBox: "+ currentBox+ "; requiredBox: " + requiredBox);
                    currentBox = 0;
                   // requiredBox = 1;
                    if (checkBoxRed.Checked) requiredBox = 0;
                    else if (checkBoxGreen.Checked) requiredBox = 1;
                    else if (checkBoxBlue.Checked) requiredBox = 2;
                    else if (checkBoxYellow.Checked) requiredBox = -1;
                   // else requiredBox = 0;

                    int claimtState = currentBox - requiredBox;
                    if (claimtState == 3) claimtState = -1;
                    if (claimtState == -3) claimtState = 1;
                    Console.WriteLine("claimtState: "+claimtState);
                    switch (claimtState) {
                        case 0:
                            Console.WriteLine("case 0");
                            break;
                        case 1:
                            Console.WriteLine("case 1");
                            motorBoxes.Power = 80;
                            motorBoxes.TurnRatio = 60;
                            motorBoxes.Mode = NXTBrick.MotorMode.On;
                            motorBoxes.Regulation = NXTBrick.MotorRegulationMode.Idle;
                            motorBoxes.RunState = NXTBrick.MotorRunState.Running;
                            motorBoxes.TachoLimit = 640;
                            brickBezDisp.SetMotorState(NXTBrick.Motor.A, motorBoxes); // otoceni krabicek
                            currentBox = requiredBox;
                            Thread.Sleep(6000);
                            break;
                        case 2:
                            Console.WriteLine("case 2");
                            motorBoxes.Power = 80;
                            motorBoxes.TurnRatio = 60;
                            motorBoxes.Mode = NXTBrick.MotorMode.On;
                            motorBoxes.Regulation = NXTBrick.MotorRegulationMode.Idle;
                            motorBoxes.RunState = NXTBrick.MotorRunState.Running;
                            motorBoxes.TachoLimit = 1280;
                            brickBezDisp.SetMotorState(NXTBrick.Motor.A, motorBoxes); // otoceni krabicek
                            currentBox = requiredBox;
                            Thread.Sleep(12000);
                            break;
                        case -1:
                            Console.WriteLine("case 3");
                            motorBoxes.Power = -80;
                            motorBoxes.TurnRatio = 60;
                            motorBoxes.Mode = NXTBrick.MotorMode.On;
                            motorBoxes.Regulation = NXTBrick.MotorRegulationMode.Idle;
                            motorBoxes.RunState = NXTBrick.MotorRunState.Running;
                            motorBoxes.TachoLimit = 640;
                            brickBezDisp.SetMotorState(NXTBrick.Motor.A, motorBoxes); // otoceni krabicek
                            currentBox = requiredBox;
                            Thread.Sleep(6000);
                            break;
                        default:
                            Console.WriteLine("a is not set");
                            break;
                    }

                    //richTextBox2.Text = "average hue = " + getAVG_Hue();
                    brick.MotorA.Run(25, 45);
                   Thread.Sleep(1000);
                   brick.MotorA.Run(-25, 45);
                   Thread.Sleep(1000);
                   //**************************************/
                } else {
                    //   Thread.Sleep(milliseconds);
                    // brick.MotorA.Run(-30, 30);
                   // brick.MotorA.Run(-15, 0);//***********
                    brick.MotorB.Run(-35, 0);//-15
                    brick.MotorC.Run(-20, 0);
                    brickBezDisp.SetMotorState(NXTBrick.Motor.B, motorWheels);
                    brickBezDisp.SetMotorState(NXTBrick.Motor.C, motorElevator);
                }
                //=========================== //Prisela kostka lega ===================================================
                // }
            }


            //   brick.Disconnect();
        }

        private void Box() {
            int b=0,a=0;
            if (a != b) { 
                switch (a) {
                    case 0:
                        Console.WriteLine("case 0");
                        if(b == 1) {

                        }else if(b == 2) {

                        } else {

                        }
                        //goto case 1;
                        b = a;
                        break;
                    case 1:
                        Console.WriteLine("case 1");
                        break;
                    case 2:
                        Console.WriteLine("case 2");
                        break;
                    case 3:
                        Console.WriteLine("case 3");
                        break;
                    default:
                        Console.WriteLine("a is not set");
                        break;
                }
            }
        }

        // ================== \Ovladani NXT ===================

        private Bitmap BlobDetection (Bitmap bitmap) {
            // create an instance of blob counter algorithm
            BlobCounter blobCounter = new BlobCounter();
            blobCounter.MinWidth = blobMinWidth;
            blobCounter.MinHeight = blobMinHeight;
            blobCounter.FilterBlobs = true;
            // process binary image
            blobCounter.ProcessImage(bitmap);
                   Rectangle[] rects = blobCounter.GetObjectsRectangles();
            // process blobs
            Blob[] blobs = blobCounter.GetObjectsInformation();
            //    foreach (Rectangle recs in rects)
 //////////         Console.WriteLine("array blobs.Length = "+ blobs.Length);
                if ((checkBox3Biggest.Checked || automaticRun) && blobs.Length > 0) {

                /*if (!newObjectDetect) { 
                        videoSourcePlayer1.Invoke((MethodInvoker)(() => videoSourcePlayer1.Stop()));
                        videoDevice.VideoResolution = videoDevice.VideoCapabilities[6];//14
                        videoSourcePlayer1.Invoke((MethodInvoker)(() => videoSourcePlayer1.VideoSource = videoDevice));
                        videoSourcePlayer1.Invoke((MethodInvoker)(() => videoSourcePlayer1.Start()));
                        newObjectDetect = true;
                    }
                    */
                // extract the biggest blob
                //pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
                //pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;

                if (blobs.Length > 0) {
                    blobCounter.ExtractBlobsImage(bitmap, blobs[0], true);

                    //blobCounter.GetObjectsRectangles();
                    
                  /*      blobCounter.ProcessImage(bitmap);
                        Rectangle[] rects2 = blobCounter.GetObjectsRectangles();
                        // process blobs
                        Blob[] blobs2 = blobCounter.GetObjectsInformation();
                    if (blobs2.Length > 0) {
                        Console.WriteLine("blobs222[0].Area : " + blobs2[0].Area + "  " + (int)blobs2[0].ColorMean.GetHue());
                        //Console.WriteLine("blobs[0].Rectangle : " + blobs[0].Rectangle.Height + " * " + blobs[0].Rectangle.Width + " = " + blobs[0].Rectangle.Height * blobs[0].Rectangle.Width);
                        Console.WriteLine("blobs222[0].Rectangle : " + blobs2[0].Rectangle.Height + " * " + blobs2[0].Rectangle.Width + " = " + blobs2[0].Rectangle.X + "x " + blobs2[0].Rectangle.Y + "y");
                        //label4.Invoke((MethodInvoker)(() => label4.Text = "Hue: " + (int)blobs[0].ColorMean.GetHue()));
                        richTextBox2.Invoke((MethodInvoker)(() => richTextBox2.Text = "blobs222[0].Area: " + blobs2[0].Area + "\n" + richTextBox2.Text + " - "+(int)blobs2[0].ColorMean.GetHue()+"\n"));

                    }*/
                    //blobCounter.GetObjectsRectangles();
                    //-------- zkoumani velikost blobu -----------
                    /*
                     foreach (Blob blob in blobs) {
                         Console.WriteLine("blob.Area : " + blob.Area);
                         Console.WriteLine("blob.Rectangle : " + blob.Rectangle.Height +" * "+ blob.Rectangle.Width+" = "+ blob.Rectangle.Height * blob.Rectangle.Width);

                     }*/

   //////                 Console.WriteLine("blobs[0].Area : " + blobs[0].Area+"  "+(int)blobs[0].ColorMean.GetHue());
                    //Console.WriteLine("blobs[0].Rectangle : " + blobs[0].Rectangle.Height + " * " + blobs[0].Rectangle.Width + " = " + blobs[0].Rectangle.Height * blobs[0].Rectangle.Width);
   //////                 Console.WriteLine("blobs[0].Rectangle : " + blobs[0].Rectangle.Height + " * " + blobs[0].Rectangle.Width + " = " +blobs[0].Rectangle.X + "x "+blobs[0].Rectangle.Y+"y");

                    /*richTextBox2.Text = "blobs[0].Area: " + blobs[0].Area+ "\n" + richTextBox2.Text + "\n";
                    richTextBox2.Text = "blobs[0].Rectangle: " + blobs[0].Rectangle.Height + "*" + blobs[0].Rectangle.Width + "=" + blobs[0].Rectangle.Height * blobs[0].Rectangle.Width + "\n" + richTextBox2.Text + "\n";
                    richTextBox2.Text = "----------------\n" + richTextBox2.Text + "\n";
                */
                    // invoke > jinak error: nelze napric vlakny menit cosi...
                    label4.Invoke((MethodInvoker)(() => label4.Text = "Hue: " + (int)blobs[0].ColorMean.GetHue()));
                    richTextBox2.Invoke((MethodInvoker)(() => richTextBox2.Text = "blobs[0].Area: " + blobs[0].Area + "\n" + richTextBox2.Text + "\n"));
                    
                    //zpusteni threadu na zastaveni motoruu
                    blobArea = blobs[0].Area;
                    if (blobs[0].Area > minBlobArea) {
                       // thread1.Abort();
                    }

                    //-------- zkoumani velikost blobu -----------

                } else {
                    blobArea = 0;
                    richTextBox2.Invoke((MethodInvoker)(() => richTextBox2.Text = "blobArea: " + blobArea + "\n" + richTextBox2.Text + "\n"));
                }
                ExtractBiggestBlob filter = new ExtractBiggestBlob();
                // apply the filter
                try {
                    bitmap = filter.Apply(bitmap);
                } catch (ArgumentException) {
                    Console.WriteLine("ArgumentException!!!");
                    blobArea = 0;
                }

                // Graphics g = Graphics.FromImage(bitmap);
                //PointF drawPoin2 = new PointF(bitmap, bitmap. + objectRect.Height + 4);
                //biggestBlobInfo = "avg HUE = " + (int)blobs[0].ColorMean.GetHue();
                biggestBlobInfo = "avg HUE = " + getAVG_Hue(bitmap)+ "; Size=" + bitmap.Width + " x " + bitmap.Height+" = "+ bitmap.Width*bitmap.Height+"\n";
                /*g.DrawString(imageInfo, new Font("Arial", 12), new SolidBrush(Color.Black), new System.Drawing.Point(bitmap.Width- bitmap.Width/2, bitmap.Height- bitmap.Height/2)); //imageInfo.Length
                    g.Dispose();
                */
                int nPixels = 0;
                if (checkBoxComputePixels.Checked || automaticRun) {
                    nPixels = ComputePixels(bitmap);
                    biggestBlobInfo += "nPixels = "+nPixels;
                }
                /*
                BlobCounter blobCounter2 = new BlobCounter();
                SimpleShapeChecker shapeChecker = new SimpleShapeChecker();
                List<IntPoint> edgePoints = blobCounter.GetBlobsEdgePoints(blobs[0]);
                List<IntPoint> cornerPoints;

                // use the shape checker to extract the corner points
                if (shapeChecker.IsQuadrilateral(edgePoints, out cornerPoints)) {
                    // only do things if the corners form a rectangle
                    if (shapeChecker.CheckPolygonSubType(cornerPoints) == PolygonSubType.Rectangle) {
                        // here i use the graphics class to draw an overlay, but you
                        // could also just use the cornerPoints list to calculate your
                        // x, y, width, height values.
                        List<System.Drawing.Point> Points = new List<System.Drawing.Point>();
                        foreach (var point in cornerPoints) {
                            Points.Add(new System.Drawing.Point(point.X, point.Y));
                        }
                        Console.WriteLine("cornerPoints.Count = " + cornerPoints.Count);
                        float side1Length = (float)cornerPoints[0].DistanceTo(cornerPoints[1]);
                        float side2Length = (float)cornerPoints[0].DistanceTo(cornerPoints[3]);
                        //  float side2Length = (float)cornerPoints[0].DistanceTo(cornerPoints[3]);
                        // float side2Length = (float)cornerPoints[0].DistanceTo(cornerPoints[3]);

                        Console.WriteLine(side1Length + " side1Length");
                        Console.WriteLine(side2Length + " side2Length");
                        Graphics g3 = Graphics.FromImage(bitmap);
                        g3.DrawPolygon(new Pen(Color.Red, 5.0f), Points.ToArray());
                        foreach (IntPoint corner in cornerPoints) {
                            g3.DrawRectangle(new Pen(Color.Blue, 5.0f), corner.X - 3, corner.Y - 3, 4, 4);
                        }


                    }
                }*/
                if (checkBoxEdgeCorners.Checked || automaticRun) {
                    biggestBlobInfo = "avg HUE = " + getAVG_Hue(bitmap);

                    /*
                   // if (checkBoxFillHole.checked){
                        var filterCountPix = new FiltersSequence(Grayscale.CommonAlgorithms.BT709,
                                                        new Threshold(2), new FillHoles());
                        bitmap = filterCountPix.Apply(bitmap);
                    //    } 
                    */
                    bitmap = DetectEdgesCorners(bitmap);

                    

                }   
                /*
                                nPixels = 0;
                                for (int y = 0; y < bitmap.Height; y++) {
                                    for (int x = 0; x < bitmap.Width; x++) {
                                        Console.WriteLine(bitmap.GetPixel(x, y).R+" "+bitmap.GetPixel(x, y).G+" "+ bitmap.GetPixel(x, y).B+" "+ bitmap.GetPixel(x, y).A);
                                        if (bitmap.GetPixel(x, y) == Color.Black) nPixels++;
                                    }
                                }
                                Console.WriteLine("nPixels222 = " + nPixels);
                                */
                //return DetectBigBlobs(bitmap);
                //return DetectCorners(bitmap); 
                return bitmap;

            } else {
                biggestBlobInfo = "";
                    if (rects.Length > 0) {
                    foreach (Rectangle objectRect in rects) {
                        Bitmap bitmap222 = bitmap;
                        Graphics g = Graphics.FromImage(bitmap);

                        using (Pen pen = new Pen(Color.FromArgb(160, 255, 160), 3)) {
                            //g.DrawRectangle(pen, objectRect); //puvodni
                            g.DrawRectangle(pen, objectRect);
                            PointF drawPoin = new PointF(objectRect.X, objectRect.Y);
                            int objectX = objectRect.X + objectRect.Width / 2 - bitmap.Width / 2;
                            int objectY = bitmap.Height / 2 - (objectRect.Y + objectRect.Height / 2);
                            PointF drawPoin2 = new PointF(objectRect.X, objectRect.Y + objectRect.Height + 4);

  
                             Bitmap target = new Bitmap(objectRect.Width, objectRect.Height);
                             using (Graphics gr = Graphics.FromImage(target)) {
                                 gr.DrawImage(bitmap222, new Rectangle(0, 0, target.Width, target.Height),
                                                  objectRect, GraphicsUnit.Pixel);
                             }
/*
                            Bitmap target=null;
                            ExtractBiggestBlob filter = new ExtractBiggestBlob();
                            // apply the filter
                            try {
                                target = filter.Apply(bitmap);
                            } catch (ArgumentException) {
                                Console.WriteLine("ArgumentException!!!");
                                blobArea = 0;
                            }
                            */
                            // Bitmap bmpObjectRect = new Bitmap(objectRect.Width, objectRect.Height, objectRect.);//pro kazdy objekt spovitat HUE!
                            // String Blobinformation = "HUE="+getAVG_Hue() +"X= " + objectX.ToString() + "  Y= " + objectY.ToString() + "\nSize=" + objectRect.Width + "x" + objectRect.Height + "=" + objectRect.Width* objectRect.Height;
                            String Blobinformation = "HUE = "+getAVG_Hue(target) +" "+target.Width + "x" + target.Height + "=" + target.Width * target.Height+"\nSize=" + objectRect.Width + "x" + objectRect.Height + "=" + objectRect.Width* objectRect.Height;
                            g.DrawString(Blobinformation, new Font("Arial", 12), new SolidBrush(Color.LightSkyBlue), drawPoin2);

                        }
                        g.Dispose();
                          }
                    }
                    return bitmap;
                }
           // return bitmap;
            
        }

        // spočítá, kolik se v bitmapě vyskytuje nečerných pixelů (tedy pixelů objektu, pokud pozadí bude černé)
        private int ComputePixels(Bitmap bitmap) {
            var filterCountPix = new FiltersSequence(Grayscale.CommonAlgorithms.BT709,
                                 new Threshold(2), new FillHoles());
            // var newBitmap = filter.Apply(bitmap);
            Bitmap bitmapCountPix = filterCountPix.Apply(bitmap);

            int nPixels = 0;
            Console.WriteLine(bitmapCountPix.Height + " x " + bitmapCountPix.Width);
            for (int xx = 0; xx < bitmapCountPix.Width; xx++) {
                for (int yy = 0; yy < bitmapCountPix.Height; yy++) {
                    // Console.WriteLine(column+" "+row);
                    if (!bitmapCountPix.GetPixel(xx, yy).ToArgb().Equals(Color.Black.ToArgb())) nPixels++;
                }
            }
            Console.WriteLine("nPixels = " + nPixels);
            return nPixels;
        }

        // Vrátí batmapu, kde bude vyznačená hranice objektu spolu s rohy, pokud se bude jednat o obdelník
        private Bitmap DetectEdgesCorners(Bitmap bitmap) {
            // locating objects
            BlobCounter blobCounter2 = new BlobCounter();

            blobCounter2.FilterBlobs = true;
            blobCounter2.MinHeight = 5;
            blobCounter2.MinWidth = 5;

            blobCounter2.ProcessImage(bitmap);
            Blob[] blobs2 = blobCounter2.GetObjectsInformation();
            // check for rectangles
            SimpleShapeChecker shapeChecker = new SimpleShapeChecker();

            SolidBrush brush = new SolidBrush(Color.Blue);
            Pen pen = new Pen(brush);
            float side1Length=0, side2Length=0;
            foreach (var blob in blobs2) {
                List<IntPoint> edgePoints = blobCounter2.GetBlobsEdgePoints(blob);
                List<IntPoint> cornerPoints;

                // use the shape checker to extract the corner points
                if (shapeChecker.IsQuadrilateral(edgePoints, out cornerPoints)) {
                    // only do things if the corners form a rectangle
                    if (shapeChecker.CheckPolygonSubType(cornerPoints) == PolygonSubType.Rectangle) {
                        // here i use the graphics class to draw an overlay, but you
                        // could also just use the cornerPoints list to calculate your
                        // x, y, width, height values.
                        List<System.Drawing.Point> Points = new List<System.Drawing.Point>();
                        foreach (var point in cornerPoints) {
                            Points.Add(new System.Drawing.Point(point.X, point.Y));
                        }
                        Console.WriteLine("cornerPoints.Count = " + cornerPoints.Count);
                         side1Length = (float)cornerPoints[0].DistanceTo(cornerPoints[1]);
                         side2Length = (float)cornerPoints[0].DistanceTo(cornerPoints[3]);
                        //  float side2Length = (float)cornerPoints[0].DistanceTo(cornerPoints[3]);
                        // float side2Length = (float)cornerPoints[0].DistanceTo(cornerPoints[3]);
                        biggestBlobInfo += " side1=" + side1Length + " side2=" + side2Length;
                        Console.WriteLine(side1Length + " side1Length");
                        Console.WriteLine(side2Length + " side2Length");

                        Graphics g3 = Graphics.FromImage(bitmap);
                        g3.DrawPolygon(new Pen(Color.Red, 5.0f), Points.ToArray());
                        int t = 0;
                        foreach (IntPoint corner in cornerPoints) {
                            g3.DrawRectangle(new Pen(Color.Blue, 5.0f), corner.X - 1, corner.Y - 1, 4, 4);
                            g3.DrawString("c"+t, new Font("Arial", 12), new SolidBrush(Color.LightSkyBlue), corner.X - 3, corner.Y - 3);
                            t++;
                        }


                    }
                }
            }
            
            return bitmap;
        }

        public Bitmap DetectBigBlobs(Bitmap bitmapDetectBigBlobs) {
            BlobCounter blobCounter = new BlobCounter();
            Graphics g = Graphics.FromImage(bitmapDetectBigBlobs);

            //filtering the blobs before searching for blobs 
            blobCounter.FilterBlobs = true;
            blobCounter.MinHeight = bitmapDetectBigBlobs.Height / 3;
            blobCounter.MinWidth = bitmapDetectBigBlobs.Width / 3;

            blobCounter.ProcessImage(bitmapDetectBigBlobs);
            Blob[] blobs = blobCounter.GetObjectsInformation();

            foreach (Blob b in blobs) {
                //getting the found blob edgepoints 
                List<IntPoint> edgePoints = blobCounter.GetBlobsEdgePoints(b);
                //if you want to mark every edge point RED 
                foreach (IntPoint point in edgePoints)
                    bitmapDetectBigBlobs.SetPixel(point.X, point.Y, Color.Red);
                //if you want to draw a rectangle around the blob 
                g.DrawRectangle(Pens.Blue, b.Rectangle);

            }

            g.Dispose();
            return bitmapDetectBigBlobs;
        }

        private Bitmap DetectCorners(Bitmap bitmapWithRectangle) {

            Graphics graphics = Graphics.FromImage(bitmapWithRectangle);
            SolidBrush brush = new SolidBrush(Color.Red);
            Pen pen = new Pen(brush);

            // Create corner detector and have it process the image
            MoravecCornersDetector mcd = new MoravecCornersDetector();
            List<IntPoint> corners = mcd.ProcessImage(bitmapWithRectangle);

            // Visualization: Draw 3x3 boxes around the corners
            foreach (IntPoint corner in corners) {
                graphics.DrawRectangle(pen, corner.X - 1, corner.Y - 1, 3, 3);
            }

            return bitmapWithRectangle;
        }

        private Bitmap DrawBitmapWithBorder(Bitmap bmp) {
            using (Graphics g = Graphics.FromImage(bmp)) {
                //g.DrawRectangle(new Pen(Brushes.Black, 20), new Rectangle(0, 0, bmp.Width, bmp.Height));
               /// g.DrawImage(bmp, new Rectangle(20, 20, bmp.Width+40, bmp.Height+40));
               /* Pen pen = new Pen(Color.Green, 20);
                pen.Alignment = PenAlignment.Inset; //<-- this
                g.DrawRectangle(pen, new Rectangle(20, 20, bmp.Width + 40, bmp.Height + 40));*/
                DrawRectangle(g, new Rectangle(0, 0, bmp.Width + 40, bmp.Height + 40), 10);
            }
            
            return bmp;
        }

        private void DrawRectangle(Graphics g, Rectangle rect, float penWidth) {
            using (Pen pen = new Pen(SystemColors.ControlDark, penWidth)) {
                float shrinkAmount = pen.Width / 2;
                g.DrawRectangle(
                    pen,
                    rect.X + shrinkAmount,   // move half a pen-width to the right
                    rect.Y + shrinkAmount,   // move half a pen-width to the down
                    rect.Width - penWidth,   // shrink width with one pen-width
                    rect.Height - penWidth); // shrink height with one pen-width
            }
        }

        private int getAVG_Hue(Bitmap bitmap) {
            int width = bitmap.Width;
            int height = bitmap.Height;
            int[] hueValues = new int[width * height];
            Color myColor;
            int grayValue;
            double grayValueD;
            int i = 0;
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    myColor = bitmap.GetPixel(x, y);
                    
                    //Color c = BitmapRGBtoGRAY.GetPixel(j, i);// Extract the color of a pixel 
                    //int rd = c.R; int gr = c.G; int bl = c.B;// extract the red,green, blue components from the color.
                    //GRAY = 0.2989 * RED + 0.5870 * GREEN + 0.1140 * BLUE
                    grayValueD = 0.2989 * (double)myColor.R + 0.5870 * (double)myColor.G + 0.1140 * (double)myColor.B;
                    grayValue = (int)Math.Round(grayValueD);
                    //Console.WriteLine(grayValue + " - grayValue");
                   if (grayValue > 10 && grayValue < 245) {// || grayValue > 245 ; > 170
                                        //hueValues[i] = 0;
                                        //richTextBox2.Text = getHueFromRGB((int)myColor.R, (int)myColor.G, (int)myColor.B) + " - Hue" + richTextBox2.Text;
                        //Console.WriteLine(getHueFromRGB((int)myColor.R, (int)myColor.G, (int)myColor.B)+"-hue; ");
                        //Console.WriteLine((int)myColor.R+" "+ (int)myColor.G+" "+ (int)myColor.B);
                        hueValues[i] = getHueFromRGB((int)myColor.R, (int)myColor.G, (int)myColor.B);
                        //Console.WriteLine(grayValue+ " - Console.WriteLine");
                    }/* else {
                        hueValues[i] = (int)myColor.GetHue();
                    }*/

                    
                    i++;
                }
            }

            int sum = 0, n = 1, avg = 0;

            foreach (int item in hueValues) {
                if (item != 0) {
                    sum = sum + item;
                    n++;
                }
            }
            if (n != 1) n--;

            avg = sum / n;
/*********
            Console.WriteLine("avg HUE = "+ avg + " - pruper vlastni; ");
            //richTextBox2.Text = avg + " - pruper vlastni\n" + richTextBox2.Text;
            //richTextBox2.Text = hueValues.Average() + " - pruper\n"+ richTextBox2.Text ;
            Console.WriteLine(avg + " - pruper vlastni");
            Console.WriteLine(hueValues.Average() + " - pruper");
            Console.WriteLine(hueValues.Max() + " - max");
            Console.WriteLine(hueValues.Min() + " - min");
**************/
            return avg;

        }

        private void btnCapture_Click(object sender, EventArgs e) {
            //D   if (videoSourcePlayer1.IsRunning) {
            if (pictureBox2.Image != null) {

                Bitmap bitmap = new Bitmap(pictureBox2.Image);
                //pictureBox1.Image = removeBackground((Bitmap)pictureBox2.Image);
                pictureBox1.Image = pictureBox2.Image;

                richTextBox1.Text = "HUE: " + getAVG_Hue(bitmap) + "\n" + richTextBox1.Text + "\n";

                blobArea = 6000;
            }
            //pictureBox1.Image = (Bitmap)videoSourcePlayer1.GetCurrentVideoFrame().Clone();
            //D   }
            label6.Text = "Captured Image";
        }

        private void btnSave_Click(object sender, EventArgs e) {
            if (pictureBox1.Image != null) {
                //string path = System.Reflection.Assembly.GetExecutingAssembly().Location;
                String cas = string.Format("{0:ddMMyy_HH-mm-ss}", DateTime.Now);
                Console.WriteLine("sccccsss- " + cas);
                pictureBox1.Image.Save(directoryPath + "TestCV" + cas + ".bmp", ImageFormat.Bmp);

            } else MessageBox.Show("Zadny obrazek k dispozici!");
            blobArea = 4444;
            label6.Text = "Saved picture";
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e) {
            if (videoSourcePlayer1.IsRunning) {
                videoSourcePlayer1.Stop();
            }
            this.Close();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e) {
            thresholdBG = (int)numericUpDown1Bakground.Value;
        }

        private int getHueFromRGB(int r, int g, int b) {
            // Console.WriteLine(r + " r " + g);
            float r2=r / 255.0f, g2 = g / 255.0f, b2 =b/ 255.0f;
            
          //  Console.WriteLine(r + " r " + g);
            int max = Math.Max(r, Math.Max( g, b)), min = Math.Min(r, Math.Min(g, b));
            float max2 = Math.Max(r2, Math.Max( g2, b2)), min2 = Math.Min(r2, Math.Min(g2, b2));
            //Console.WriteLine(max+" "+max2+" maaaax "+min+" "+min2);
            //Console.WriteLine(r2 + " " + g2 + " rrrrrrrrr " + b2 + " " + 153.0/255.0);
            //var h, s, l = (max + min) / 2;
            int hue;
            if (max == min) {
                hue = 0; 
            } else {
                if (max == r) {
                    hue =  (int)(60*((g2 - b2) / (max2 - min2)  ) + (g < b ? 6.0 : 0.0));//+(g < b ? 6 : 0)
                } else if (max==g) {
                    hue =  (int)(60 * (((b2 - r2) / (max2 - min2)) + 2.0));
                } else {
                    hue =  (int)(60 * (((r2 - g2) / (max2 - min2)) + 4.0) );
                }
            }
           // Console.WriteLine("hue = "+hue);
           // richTextBox1.Text = richTextBox1.Text + "hue = " + hue+"\n";
            return hue;
        }



        private Bitmap RGBtoGRAYConversion(Bitmap BitmapRGBtoGRAY) {
            
            int height = BitmapRGBtoGRAY.Height;
            int width = BitmapRGBtoGRAY.Width;
            Bitmap GRAYbitmap = new Bitmap(width, height);// GRAY is the resultant matrix 

            for (int i = 0; i < height; i++) {
                for (int j = 0; j < width; j++) {
                    Color c = BitmapRGBtoGRAY.GetPixel(j, i);// Extract the color of a pixel 
                    int rd = c.R; int gr = c.G; int bl = c.B;// extract the red,green, blue components from the color.
                    //GRAY = 0.2989 * RED + 0.5870 * GREEN + 0.1140 * BLUE
                    double d1 = 0.2989 * (double)rd + 0.5870 * (double)gr + 0.1140 * (double)bl;
                    int c1 = (int)Math.Round(d1);
                    Color c2 = Color.FromArgb(c1, c1, c1);
                    GRAYbitmap.SetPixel(j, i, c2);
                }
            }
            return GRAYbitmap;
        }

        //D
        protected bool validData;
        string path;
        protected System.Drawing.Image imageD;
        protected Thread getImageThread;
        private static bool stop;

        private void pictureBox2_DragDrop(object sender, DragEventArgs e) {
            if (validData) {
                while (getImageThread.IsAlive) {
                    Application.DoEvents();
                    Thread.Sleep(0);
                }

                //----------------------
                Bitmap bitmap2 = (Bitmap)imageD;
                // pozdeji treba switch
                if (checkBox1RemBack.Checked || automaticRun) {
                    bitmap2 = removeBackground(bitmap2);
                }

                if (checkBox2Blob.Checked || automaticRun) {
                   
                    bitmap2 = BlobDetection(bitmap2);
                    /*Grayscale filter = new Grayscale(0.2125, 0.7154, 0.0721);
                    // apply the filter
                    bitmap2 = filter.Apply(bitmap2);
                    */
                }
                //pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
                pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
                pictureBox2.Image = bitmap2;
                pictureBox2.Refresh();

                //----------------------
                /*
                Bitmap bitmap2, bitmapGRAY;
                bitmap2 = removeBackground((Bitmap)imageD);
                bitmapGRAY = (Bitmap)imageD;
                pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
                pictureBox2.Image = bitmap2;
                */
                /*for (int i = 0; i < 10; i++) {
                    for (int j = 0; j < 10; j++) {
                        //bitmapGRAY.GetPixel(i, j);
                        Console.WriteLine(bitmapGRAY.GetPixel(i, j) + " - bitmapGRAY.GetPixel(i, j)");
                        Console.WriteLine(bitmapGRAY.GetPixel(i, j).GetHue() + " - bitmapGRAY.GetHue(i, j)");
                       // Console.WriteLine(bitmapGRAY.GetPixel(i, j) + " - bitmapGRAY.GetPixel(i, j)");


                    }
                }*/

                /*  // create grayscale filter (BT709)
                  Grayscale filter = new Grayscale(0.2125, 0.7154, 0.0721);
                  // apply the filter
                  bitmap2 = filter.Apply(bitmap2);
                  */
                /*
                                // create an instance of blob counter algorithm
                                BlobCounter blobCounter = new BlobCounter();
                                blobCounter.MinWidth = 5;
                                blobCounter.MinHeight = 5;
                                blobCounter.FilterBlobs = true;
                                // process binary image
                                blobCounter.ProcessImage(bitmap2);
                                Rectangle[] rects = blobCounter.GetObjectsRectangles() ;
                                // process blobs
                                foreach (Rectangle recs in rects)
                                    if (rects.Length > 0) {
                                        foreach (Rectangle objectRect in rects) {

                                            Graphics g = Graphics.FromImage(bitmap2);

                                            using (Pen pen = new Pen(Color.FromArgb(160, 255, 160), 5)) {
                                                g.DrawRectangle(pen, objectRect);
                                            }

                                            g.Dispose();
                                        }

                                    }

                                */
                //  pictureBox2.Image = bitmap2;
            }

        }

        private void pictureBox2_DragEnter(object sender, DragEventArgs e) {
            string filename;
            validData = GetFilename(out filename, e);
            if (validData) {
                path = filename;
                getImageThread = new Thread(new ThreadStart(LoadImage));
                getImageThread.Start();
                e.Effect = DragDropEffects.Copy;
            } else
                e.Effect = DragDropEffects.None;

        }

        protected void LoadImage() {
            imageD = new Bitmap(path);
        }
        private bool GetFilename(out string filename, DragEventArgs e) {
            bool ret = false;
            filename = String.Empty;
            if ((e.AllowedEffect & DragDropEffects.Copy) == DragDropEffects.Copy) {
                Array data = ((IDataObject)e.Data).GetData("FileDrop") as Array;
                if (data != null) {
                    if ((data.Length == 1) && (data.GetValue(0) is String)) {
                        filename = ((string[])data)[0];
                        string ext = Path.GetExtension(filename).ToLower();
                        if ((ext == ".jpg") || (ext == ".png") || (ext == ".bmp")) {
                            ret = true;
                        }
                    }
                }
            }
            return ret;
        }//D

        public static Bitmap CropImage(System.Drawing.Image source, int x, int y, int width, int height) {
            Rectangle crop = new Rectangle(x, y, width, height);

            var bmp = new Bitmap(crop.Width, crop.Height);
            using (var gr = Graphics.FromImage(bmp)) {
                gr.DrawImage(source, new Rectangle(0, 0, bmp.Width, bmp.Height), crop, GraphicsUnit.Pixel);
            }
            return bmp;
        }
        private void btnCropImg_Click(object sender, EventArgs e) {
            if (rectangleForCropping != null && rectangleForCropping.Width > 0 && rectangleForCropping.Height > 0) croppedRectangle = rectangleForCropping;
            croppingImagePressed = true;
            rectangleForCropping.Width = 0;
            //pictureBox2.Invalidate(rectangleForCropping);
            /*  if (rectangleForCropping.Contains(5,5)) {
                  Debug.WriteLine("Right click");

          }*/
            //pictureBox2.Invalidate();
            /*  Stare orezavani v novem formu
            Form2CroppingImage formCropImg = new Form2CroppingImage(this, pictureBox2.Image, pictureBox2.Width, pictureBox2.Height);
            formCropImg.Show();
            */
        }

        private void btnPropPage_Click(object sender, EventArgs e) {
            videoDevice.DisplayPropertyPage(IntPtr.Zero);
            //System.Threading.Thread.Sleep(5000);
            videoSourcePlayer1.VideoSource = videoDevice;
        }

        private void numericUpDown2BlobSize_ValueChanged(object sender, EventArgs e) {
            blobMinWidth = (int)numericUpDown2BlobSizeW.Value;
        }

        private void setFolderToolStripMenuItem_Click(object sender, EventArgs e) {
            FolderBrowserDialog directchoosedlg = new FolderBrowserDialog();
            if (directchoosedlg.ShowDialog() == DialogResult.OK) {
                directoryPath = @directchoosedlg.SelectedPath+"\\";
            }
            Console.WriteLine("directoryPath: "+ directoryPath);
        }

        private void btnFolderImages_Click(object sender, EventArgs e) {
            Process.Start(directoryPath);
        }

      /*  private void pictureBox2_Paint(object sender, PaintEventArgs e) {
            using (Font myFont = new Font("Arial", 9)) {
                e.Graphics.DrawString(biggestBlobInfo, myFont, Brushes.MidnightBlue, new System.Drawing.Point(2, 2));
            }
        }*/
        private void checkBox1_CheckedChanged(object sender, EventArgs e) {
            automaticRun = checkBox1.Checked;
        }

        private void checkBox1RemBack_CheckedChanged(object sender, EventArgs e) {
            if (checkBox1RemBack.Checked) {
                groupBox1.Enabled = true;
            } else if (!checkBox1RemBack.Checked) {
                groupBox1.Enabled = false;
                groupBox2.Enabled = false;
                checkBoxEdgeCorners.Enabled = false;
                checkBox2Blob.Checked = false;
                checkBox3Biggest.Checked = false;
                checkBoxEdgeCorners.Checked = false;
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e) {

        }

        // vytvori se obrazek podlozky pod kamerou, aby se pozdeji od neho odecetl obraz s predmetem
        private Bitmap imageOfBackground = null;
        private void btnMakeImageOfPad_Click(object sender, EventArgs e) {
            imageOfBackground = (Bitmap)pictureBox2.Image.Clone();
            pictureBox1.Image = imageOfBackground;
            label6.Text = "The Image Of the pad";
        }

        private void button1_Click(object sender, EventArgs e) {
            croppingImagePressed = false;
            pictureBox1.Image = null;
            Invalidate();
        }

        private void checkBox2Blob_CheckedChanged(object sender, EventArgs e) {
            if (checkBox2Blob.Checked) {
                groupBox2.Enabled = true;
            } else if (!checkBox2Blob.Checked) {
                groupBox2.Enabled = false;
                checkBoxEdgeCorners.Enabled = false;
                checkBox3Biggest.Checked = false;
                checkBoxEdgeCorners.Checked = false;
            }
        }

        private void checkBox3Biggest_CheckedChanged(object sender, EventArgs e) {
            if (checkBox3Biggest.Checked) {
                checkBoxEdgeCorners.Enabled = true;
                checkBoxComputePixels.Enabled = true;
            } else if (!checkBox3Biggest.Checked) {
                checkBoxEdgeCorners.Enabled = false;
                checkBoxComputePixels.Enabled = false;
                checkBoxEdgeCorners.Checked = false;
            }
        }

        private void numericUpDown3BlobSizeH_ValueChanged(object sender, EventArgs e) {
            blobMinHeight = (int)numericUpDown3BlobSizeH.Value;
        }
        private void numUpDownMinBlobSize_ValueChanged(object sender, EventArgs e) {
            minBlobArea = (int)numUpDownMinBlobSize.Value;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e) {
            if (videoSourcePlayer1.IsRunning) {
                videoSourcePlayer1.Stop();
            }
            Application.ExitThread();
            Environment.Exit(1);
        }
    }
}

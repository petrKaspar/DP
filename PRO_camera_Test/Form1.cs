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
        public static NxtBrick brick = new NxtBrick(NxtCommLinkType.USB, 3);

        //-------konfigurace NXT blokuu -------
        FilterInfoCollection filterInfColl;
        private VideoCaptureDevice videoDevice;
        MotionDetector motionDetector;
        float f;
        private static int thresholdBG, blobMinWidth = 40, blobMinHeight = 40;
        private Boolean automaticRun = false;
        public int qqq;
        static int blobArea = 0;
        static int minBlobArea;

        Thread thread1;
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

                Console.WriteLine(resolution+ resolution_size);
            }
            videoDevice.VideoResolution = videoDevice.VideoCapabilities[2];
            videoSourcePlayer1.VideoSource = videoDevice;

         //   videoSourcePlayer1.NewFrame +=  new AForge.Controls.VideoSourcePlayer.NewFrameHandler(videoSourcePlayer1_NewFrame);
            videoSourcePlayer1.Start();

            /*if (videoSourcePlayer1.IsRunning) {
                pictureBox2.Image = (Bitmap)videoSourcePlayer1.GetCurrentVideoFrame();
            }*/

            thread1.Start();
            //Form1 thread1 = new Form1();
            //new Thread(thread1.motoryPasuuZvedak).Start();

        }

        private void btnStop_Click(object sender, EventArgs e) {
            if (videoSourcePlayer1.IsRunning) {
                videoSourcePlayer1.Stop();
                stop = true;
            }
            //thread1.Abort();
            //Color myColor = Color.FromArgb(0, 255, 0);
            //Console.WriteLine(myColor.GetHue() + " - HHHHHHHHHHHHHHHHHHHHHHHHHHH");
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
        private void videoSourcePlayer1_NewFrame(object sender, ref Bitmap image) {
        //public void videoSourcePlayer1_NewFrame(object sender, NewFrameEventArgs eventArgs) {
            
            Bitmap bitmap2 = new Bitmap(image);

            // create filter
            BrightnessCorrection filter = new BrightnessCorrection(-50);
            // apply the filter
            //filter.ApplyInPlace(bitmap2);
            
            // pozdeji treba switch
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

        private void timer1_Tick(object sender, EventArgs e) {
            label3.Text = "Value: " + f.ToString();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e) {
            if (videoSourcePlayer1.IsRunning) {
                videoSourcePlayer1.Stop();
            }
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
            brick.MotorA = new NxtMotor();
            brick.MotorB = new NxtMotor();
            brick.MotorC = new NxtMotor();

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
            brick.MotorA.Run(-15, 0);
            brick.MotorB.Run(-15, 0);
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
                if (Form1.blobArea > minBlobArea || Form1.stop) {
                    brick.MotorA.Brake();
                    brick.MotorB.Brake();
                    brick.MotorC.Brake();


                    /**********************************  vyklapeni a zklapeni
                   brick.MotorA.Brake();
                   // Thread.Sleep(milliseconds);
                   Thread.Sleep(500);
                   brick.MotorB.Run(25, 45);
                   Thread.Sleep(1000);
                   brick.MotorB.Run(-25, 45);
                   Thread.Sleep(1000);
                   **************************************/
                } else {
                    //   Thread.Sleep(milliseconds);
                    // brick.MotorA.Run(-30, 30);
                    brick.MotorA.Run(-15, 0);
                    brick.MotorB.Run(-15, 0);
                    brick.MotorC.Run(-20, 0);
                }
                // }
            }
            

            //   brick.Disconnect();
        }
        // ================== Ovladani NXT ===================

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
                if (checkBox3Biggest.Checked || automaticRun) {
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

                    Console.WriteLine("blobs[0].Area : " + blobs[0].Area+"  "+(int)blobs[0].ColorMean.GetHue());
                    //Console.WriteLine("blobs[0].Rectangle : " + blobs[0].Rectangle.Height + " * " + blobs[0].Rectangle.Width + " = " + blobs[0].Rectangle.Height * blobs[0].Rectangle.Width);
                    Console.WriteLine("blobs[0].Rectangle : " + blobs[0].Rectangle.Height + " * " + blobs[0].Rectangle.Width + " = " +blobs[0].Rectangle.X + "x "+blobs[0].Rectangle.Y+"y");

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
                


                return bitmap;
            } else {
                    if (rects.Length > 0) {
                        foreach (Rectangle objectRect in rects) {
                        
                        Graphics g = Graphics.FromImage(bitmap);

                        using (Pen pen = new Pen(Color.FromArgb(160, 255, 160), 3)) {
                            //g.DrawRectangle(pen, objectRect); //puvodni
                            g.DrawRectangle(pen, objectRect);
                            PointF drawPoin = new PointF(objectRect.X, objectRect.Y);
                            int objectX = objectRect.X + objectRect.Width / 2 - bitmap.Width / 2;
                            int objectY = bitmap.Height / 2 - (objectRect.Y + objectRect.Height / 2);
                            PointF drawPoin2 = new PointF(objectRect.X, objectRect.Y + objectRect.Height + 4);
                            String Blobinformation = "X= " + objectX.ToString() + "  Y= " + objectY.ToString() + "\nSize=" + objectRect.Width + ", " + objectRect.Height;
                            g.DrawString(Blobinformation, new Font("Arial", 12), new SolidBrush(Color.LightSkyBlue), drawPoin2);

                        }
                        g.Dispose();
                          }
                    }
                    return bitmap;
                }
           // return bitmap;
            
        }


        private int getAVG(Bitmap bitmap) {
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
                    if (grayValue > 170) {// || grayValue > 245
                        hueValues[i] = 0;
                        //Console.WriteLine(grayValue+ " - Console.WriteLine");
                    } else {
                        hueValues[i] = (int)myColor.GetHue();
                    }
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
            Console.WriteLine(avg + " - pruper vlastni");
            Console.WriteLine(hueValues.Average() + " - pruper");
            Console.WriteLine(hueValues.Max() + " - max");
            Console.WriteLine(hueValues.Min() + " - min");
            return avg;

        }

        private void btnCapture_Click(object sender, EventArgs e) {
            //D   if (videoSourcePlayer1.IsRunning) {

            Bitmap bitmap = new Bitmap(pictureBox2.Image);
            //pictureBox1.Image = removeBackground((Bitmap)pictureBox2.Image);
            pictureBox1.Image = pictureBox2.Image;

            richTextBox1.Text = getAVG(bitmap) + "\n" + richTextBox1.Text + "\n";

            blobArea = 6000;



            //pictureBox1.Image = (Bitmap)videoSourcePlayer1.GetCurrentVideoFrame().Clone();
            //D   }
        }

        private void btnSave_Click(object sender, EventArgs e) {
            if (pictureBox1.Image != null) {
                //string path = System.Reflection.Assembly.GetExecutingAssembly().Location;
                String cas = string.Format("{0:ddMMyy_HH-mm-ss}", DateTime.Now);
                Console.WriteLine("sccccsss- " + cas);
                pictureBox1.Image.Save(@".\TestCV" + cas + ".bmp", ImageFormat.Bmp);

            } else MessageBox.Show("Zadny obrazek k dispozici!");
            blobArea = 4444;
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


        private void btnCropImg_Click(object sender, EventArgs e) {
            Form2CroppingImage formCropImg = new Form2CroppingImage(this, pictureBox2.Image, pictureBox2.Width, pictureBox2.Height);
            formCropImg.Show();
        }

        private void btnPropPage_Click(object sender, EventArgs e) {
            videoDevice.DisplayPropertyPage(IntPtr.Zero);
            //System.Threading.Thread.Sleep(5000);
            videoSourcePlayer1.VideoSource = videoDevice;
        }

        private void numericUpDown2BlobSize_ValueChanged(object sender, EventArgs e) {
            blobMinWidth = (int)numericUpDown2BlobSizeW.Value;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e) {
            automaticRun = checkBox1.Checked;
        }

        private void numericUpDown3BlobSizeH_ValueChanged(object sender, EventArgs e) {
            blobMinHeight = (int)numericUpDown3BlobSizeH.Value;
        }
        private void numUpDownMinBlobSize_ValueChanged(object sender, EventArgs e) {
            minBlobArea = (int)numUpDownMinBlobSize.Value;
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PRO_camera_Test {
    public partial class Form2CroppingImage : Form {
        //private Image image;
        Form1 mainForm;


        public Form2CroppingImage() {
            InitializeComponent();
        }

        public Form2CroppingImage(Form1 incomingForm, Image image, int width, int height) {
            InitializeComponent();

            this.Width = width + 50;
            this.Height = height + 50;

            mainForm = incomingForm;

            pictureBox1.Width = width;
            pictureBox1.Height = height;
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.Image = image;
            //OriginalImage = (Bitmap) pictureBox1.Image;
        }

        private void btnCrop_Click(object sender, EventArgs e) {
            mainForm.qqq = 7777;
            mainForm.setCropRectangle();
            this.Close();
        }

        // -------------------- Crop Image -------------------------
        private int xDown;
        private int xUp;
        private int yDown;
        private int yUp;
        Rectangle rectCropArea;

        void pictureBox1_MouseUp(object sender, MouseEventArgs e) {
            xUp = e.X;
            yUp = e.Y;


            Rectangle rec = new Rectangle(xDown, yDown, Math.Abs(xUp - xDown), Math.Abs(yUp - yDown));

            using (Pen pen = new Pen(Color.YellowGreen, 3)) {

                pictureBox1.CreateGraphics().DrawRectangle(pen, rec);
            }

            xDown = xDown * pictureBox1.Image.Width / pictureBox1.Width;
            yDown = yDown * pictureBox1.Image.Height / pictureBox1.Height;

            xUp = xUp * pictureBox1.Image.Width / pictureBox1.Width;
            yUp = yUp * pictureBox1.Image.Height / pictureBox1.Height;

            rectCropArea = new Rectangle(xDown, yDown, Math.Abs(xUp - xDown), Math.Abs(yUp - yDown));


            Bitmap sourceBitmap = new Bitmap(pictureBox1.Image, pictureBox1.Width, pictureBox1.Height);
            Graphics g = pictureBox1.CreateGraphics();

            //Draw the image on the Graphics object with the new dimesions
            g.DrawImage(sourceBitmap, new Rectangle(0, 0, pictureBox1.Width, pictureBox1.Height), rectCropArea, GraphicsUnit.Pixel);
            sourceBitmap.Dispose();


        }

        void pictureBox1_MouseDown(object sender, MouseEventArgs e) {
            pictureBox1.Invalidate();

            xDown = e.X;
            yDown = e.Y;
        }


        
        /*// The original image.
        private Bitmap OriginalImage;

        // The currently cropped image.
        private Bitmap CroppedImage;

        // The cropped image with the selection rectangle.
        private Bitmap DisplayImage;
        private Graphics DisplayGraphics;
        // Let the user select an area.
        private bool Drawing = false;
        private Point StartPoint, EndPoint;
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e) {
            Drawing = true;
            StartPoint = e.Location;

            // Draw the area selected.
            DrawSelectionBox(e.Location);
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e) {
            if (!Drawing) return;

            // Draw the area selected.
            DrawSelectionBox(e.Location);
        }
        
        private void pictureBox1_MouseUp(object sender, MouseEventArgs e) {
            if (!Drawing) return;
            Drawing = false;

            // Crop.
            // Get the selected area's dimensions.
            int x = Math.Min(StartPoint.X, EndPoint.X);
            int y = Math.Min(StartPoint.Y, EndPoint.Y);
            int width = Math.Abs(StartPoint.X - EndPoint.X);
            int height = Math.Abs(StartPoint.Y - EndPoint.Y);
            Rectangle source_rect = new Rectangle(x, y, width, height);
            Rectangle dest_rect = new Rectangle(0, 0, width, height);

            // Copy that part of the image to a new bitmap.
            DisplayImage = new Bitmap(width, height);
            DisplayGraphics = Graphics.FromImage(DisplayImage);
            DisplayGraphics.DrawImage(CroppedImage,
                dest_rect, source_rect, GraphicsUnit.Pixel);

            // Display the new bitmap.
            CroppedImage = DisplayImage;
            DisplayImage = CroppedImage.Clone() as Bitmap;
            DisplayGraphics = Graphics.FromImage(DisplayImage);
            pictureBox1.Image = DisplayImage;
            pictureBox1.Refresh();
        }

        // Draw the area selected.
        private void DrawSelectionBox(Point end_point) {
            // Save the end point.
            EndPoint = end_point;
            if (EndPoint.X < 0) EndPoint.X = 0;
            if (EndPoint.X >= CroppedImage.Width) EndPoint.X = CroppedImage.Width - 1;
            if (EndPoint.Y < 0) EndPoint.Y = 0;
            if (EndPoint.Y >= CroppedImage.Height) EndPoint.Y = CroppedImage.Height - 1;
            // Reset the image.
            DisplayGraphics.DrawImageUnscaled(CroppedImage, 0, 0);
            // Draw the selection area.
            int x = Math.Min(StartPoint.X, EndPoint.X);
            int y = Math.Min(StartPoint.Y, EndPoint.Y);
            int width = Math.Abs(StartPoint.X - EndPoint.X);
            int height = Math.Abs(StartPoint.Y - EndPoint.Y);
            DisplayGraphics.DrawRectangle(Pens.Red, x, y, width, height);
            pictureBox1.Refresh();
        }
        */
        // -------------------- Crop Image -------------------------





    }
}

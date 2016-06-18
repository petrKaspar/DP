namespace PRO_camera_Test
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBoxDevice = new System.Windows.Forms.ComboBox();
            this.comboBoxMode = new System.Windows.Forms.ComboBox();
            this.videoSourcePlayer1 = new AForge.Controls.VideoSourcePlayer();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.pictureBox2 = new AForge.Controls.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btnCapture = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkBox1RemBack = new System.Windows.Forms.CheckBox();
            this.checkBox2Blob = new System.Windows.Forms.CheckBox();
            this.numericUpDown1Bakground = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown2BlobSizeW = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown3BlobSizeH = new System.Windows.Forms.NumericUpDown();
            this.checkBox3Biggest = new System.Windows.Forms.CheckBox();
            this.btnCropImg = new System.Windows.Forms.Button();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.richTextBox2 = new System.Windows.Forms.RichTextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnPropPage = new System.Windows.Forms.Button();
            this.numUpDownMinBlobSize = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1Bakground)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2BlobSizeW)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown3BlobSizeH)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDownMinBlobSize)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Video devices";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 68);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Video resolution";
            // 
            // comboBoxDevice
            // 
            this.comboBoxDevice.FormattingEnabled = true;
            this.comboBoxDevice.Location = new System.Drawing.Point(100, 37);
            this.comboBoxDevice.Name = "comboBoxDevice";
            this.comboBoxDevice.Size = new System.Drawing.Size(217, 21);
            this.comboBoxDevice.TabIndex = 2;
            // 
            // comboBoxMode
            // 
            this.comboBoxMode.FormattingEnabled = true;
            this.comboBoxMode.Location = new System.Drawing.Point(100, 65);
            this.comboBoxMode.Name = "comboBoxMode";
            this.comboBoxMode.Size = new System.Drawing.Size(121, 21);
            this.comboBoxMode.TabIndex = 3;
            // 
            // videoSourcePlayer1
            // 
            this.videoSourcePlayer1.Location = new System.Drawing.Point(3, 3);
            this.videoSourcePlayer1.Name = "videoSourcePlayer1";
            this.videoSourcePlayer1.Size = new System.Drawing.Size(305, 230);
            this.videoSourcePlayer1.TabIndex = 4;
            this.videoSourcePlayer1.Text = "videoSourcePlayer1";
            this.videoSourcePlayer1.VideoSource = null;
            this.videoSourcePlayer1.NewFrame += new AForge.Controls.VideoSourcePlayer.NewFrameHandler(this.videoSourcePlayer1_NewFrame);
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(241, 64);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 5;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(322, 65);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(75, 23);
            this.btnStop.TabIndex = 6;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 331);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(46, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Value: 0";
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 10;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.videoSourcePlayer1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.pictureBox2, 1, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(15, 92);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(623, 236);
            this.tableLayoutPanel1.TabIndex = 8;
            // 
            // pictureBox2
            // 
            this.pictureBox2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox2.Image = null;
            this.pictureBox2.Location = new System.Drawing.Point(314, 3);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(306, 230);
            this.pictureBox2.TabIndex = 5;
            this.pictureBox2.TabStop = false;
            this.pictureBox2.DragDrop += new System.Windows.Forms.DragEventHandler(this.pictureBox2_DragDrop);
            this.pictureBox2.DragEnter += new System.Windows.Forms.DragEventHandler(this.pictureBox2_DragEnter);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox1.Location = new System.Drawing.Point(18, 358);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(171, 128);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 5;
            this.pictureBox1.TabStop = false;
            // 
            // btnCapture
            // 
            this.btnCapture.Location = new System.Drawing.Point(195, 358);
            this.btnCapture.Name = "btnCapture";
            this.btnCapture.Size = new System.Drawing.Size(75, 23);
            this.btnCapture.TabIndex = 9;
            this.btnCapture.Text = "Capture";
            this.btnCapture.UseVisualStyleBackColor = true;
            this.btnCapture.Click += new System.EventHandler(this.btnCapture_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(195, 387);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 10;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(944, 24);
            this.menuStrip1.TabIndex = 11;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.openToolStripMenuItem.Text = "Open";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // checkBox1RemBack
            // 
            this.checkBox1RemBack.AutoSize = true;
            this.checkBox1RemBack.Location = new System.Drawing.Point(644, 156);
            this.checkBox1RemBack.Name = "checkBox1RemBack";
            this.checkBox1RemBack.Size = new System.Drawing.Size(129, 17);
            this.checkBox1RemBack.TabIndex = 12;
            this.checkBox1RemBack.Text = "Remove background ";
            this.checkBox1RemBack.UseVisualStyleBackColor = true;
            // 
            // checkBox2Blob
            // 
            this.checkBox2Blob.AutoSize = true;
            this.checkBox2Blob.Location = new System.Drawing.Point(644, 208);
            this.checkBox2Blob.Name = "checkBox2Blob";
            this.checkBox2Blob.Size = new System.Drawing.Size(109, 17);
            this.checkBox2Blob.TabIndex = 13;
            this.checkBox2Blob.Text = "Objects detection";
            this.checkBox2Blob.UseVisualStyleBackColor = true;
            // 
            // numericUpDown1Bakground
            // 
            this.numericUpDown1Bakground.Location = new System.Drawing.Point(730, 179);
            this.numericUpDown1Bakground.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numericUpDown1Bakground.Name = "numericUpDown1Bakground";
            this.numericUpDown1Bakground.Size = new System.Drawing.Size(43, 20);
            this.numericUpDown1Bakground.TabIndex = 14;
            this.numericUpDown1Bakground.Value = new decimal(new int[] {
            230,
            0,
            0,
            0});
            this.numericUpDown1Bakground.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
            // 
            // numericUpDown2BlobSizeW
            // 
            this.numericUpDown2BlobSizeW.Location = new System.Drawing.Point(681, 231);
            this.numericUpDown2BlobSizeW.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numericUpDown2BlobSizeW.Name = "numericUpDown2BlobSizeW";
            this.numericUpDown2BlobSizeW.Size = new System.Drawing.Size(43, 20);
            this.numericUpDown2BlobSizeW.TabIndex = 15;
            this.numericUpDown2BlobSizeW.ValueChanged += new System.EventHandler(this.numericUpDown2BlobSize_ValueChanged);
            // 
            // numericUpDown3BlobSizeH
            // 
            this.numericUpDown3BlobSizeH.Location = new System.Drawing.Point(730, 231);
            this.numericUpDown3BlobSizeH.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numericUpDown3BlobSizeH.Name = "numericUpDown3BlobSizeH";
            this.numericUpDown3BlobSizeH.Size = new System.Drawing.Size(43, 20);
            this.numericUpDown3BlobSizeH.TabIndex = 16;
            this.numericUpDown3BlobSizeH.ValueChanged += new System.EventHandler(this.numericUpDown3BlobSizeH_ValueChanged);
            // 
            // checkBox3Biggest
            // 
            this.checkBox3Biggest.AutoSize = true;
            this.checkBox3Biggest.Location = new System.Drawing.Point(679, 264);
            this.checkBox3Biggest.Name = "checkBox3Biggest";
            this.checkBox3Biggest.Size = new System.Drawing.Size(105, 17);
            this.checkBox3Biggest.TabIndex = 17;
            this.checkBox3Biggest.Text = "Only the biggest ";
            this.checkBox3Biggest.UseVisualStyleBackColor = true;
            // 
            // btnCropImg
            // 
            this.btnCropImg.Location = new System.Drawing.Point(645, 92);
            this.btnCropImg.Name = "btnCropImg";
            this.btnCropImg.Size = new System.Drawing.Size(75, 23);
            this.btnCropImg.TabIndex = 18;
            this.btnCropImg.Text = "Crop image";
            this.btnCropImg.UseVisualStyleBackColor = true;
            this.btnCropImg.Click += new System.EventHandler(this.btnCropImg_Click);
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(195, 416);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(75, 130);
            this.richTextBox1.TabIndex = 20;
            this.richTextBox1.Text = "";
            // 
            // pictureBox3
            // 
            this.pictureBox3.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.pictureBox3.Image = global::PRO_camera_Test.Properties.Resources.hsl_color_wheel;
            this.pictureBox3.Location = new System.Drawing.Point(276, 358);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(362, 313);
            this.pictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox3.TabIndex = 21;
            this.pictureBox3.TabStop = false;
            // 
            // richTextBox2
            // 
            this.richTextBox2.Location = new System.Drawing.Point(648, 362);
            this.richTextBox2.Name = "richTextBox2";
            this.richTextBox2.Size = new System.Drawing.Size(296, 301);
            this.richTextBox2.TabIndex = 22;
            this.richTextBox2.Text = "";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(326, 331);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(35, 13);
            this.label4.TabIndex = 23;
            this.label4.Text = "label4";
            // 
            // btnPropPage
            // 
            this.btnPropPage.Location = new System.Drawing.Point(322, 34);
            this.btnPropPage.Name = "btnPropPage";
            this.btnPropPage.Size = new System.Drawing.Size(75, 23);
            this.btnPropPage.TabIndex = 24;
            this.btnPropPage.Text = "button1";
            this.btnPropPage.UseVisualStyleBackColor = true;
            this.btnPropPage.Click += new System.EventHandler(this.btnPropPage_Click);
            // 
            // numUpDownMinBlobSize
            // 
            this.numUpDownMinBlobSize.Location = new System.Drawing.Point(677, 305);
            this.numUpDownMinBlobSize.Maximum = new decimal(new int[] {
            300000,
            0,
            0,
            0});
            this.numUpDownMinBlobSize.Name = "numUpDownMinBlobSize";
            this.numUpDownMinBlobSize.Size = new System.Drawing.Size(62, 20);
            this.numUpDownMinBlobSize.TabIndex = 25;
            this.numUpDownMinBlobSize.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numUpDownMinBlobSize.ValueChanged += new System.EventHandler(this.numUpDownMinBlobSize_ValueChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(745, 312);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(71, 13);
            this.label5.TabIndex = 26;
            this.label5.Text = "Max blob size";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(414, 70);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(91, 17);
            this.checkBox1.TabIndex = 27;
            this.checkBox1.Text = "Automatic run";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(944, 675);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.numUpDownMinBlobSize);
            this.Controls.Add(this.btnPropPage);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.richTextBox2);
            this.Controls.Add(this.pictureBox3);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.btnCropImg);
            this.Controls.Add(this.checkBox3Biggest);
            this.Controls.Add(this.numericUpDown3BlobSizeH);
            this.Controls.Add(this.numericUpDown2BlobSizeW);
            this.Controls.Add(this.numericUpDown1Bakground);
            this.Controls.Add(this.checkBox2Blob);
            this.Controls.Add(this.checkBox1RemBack);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.btnCapture);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.comboBoxMode);
            this.Controls.Add(this.comboBoxDevice);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.menuStrip1);
            this.DoubleBuffered = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "PRO - camera test";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1Bakground)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2BlobSizeW)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown3BlobSizeH)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDownMinBlobSize)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBoxDevice;
        private System.Windows.Forms.ComboBox comboBoxMode;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private AForge.Controls.VideoSourcePlayer videoSourcePlayer1;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button btnCapture;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private AForge.Controls.PictureBox pictureBox2;
        private System.Windows.Forms.CheckBox checkBox1RemBack;
        private System.Windows.Forms.CheckBox checkBox2Blob;
        private System.Windows.Forms.NumericUpDown numericUpDown1Bakground;
        private System.Windows.Forms.NumericUpDown numericUpDown2BlobSizeW;
        private System.Windows.Forms.NumericUpDown numericUpDown3BlobSizeH;
        private System.Windows.Forms.CheckBox checkBox3Biggest;
        private System.Windows.Forms.Button btnCropImg;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.RichTextBox richTextBox2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnPropPage;
        private System.Windows.Forms.NumericUpDown numUpDownMinBlobSize;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox checkBox1;
    }
}


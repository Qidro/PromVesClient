namespace PromVesClient
{
    partial class MediaWeighingForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MediaWeighingForm));
            button1 = new Button();
            panel1 = new Panel();
            btnOpenFolderVideo = new Button();
            btnOpenFolderPhoto = new Button();
            textBoxVagonNumber = new TextBox();
            label10 = new Label();
            textBoxVideoName = new TextBox();
            label9 = new Label();
            textBoxPhotoName = new TextBox();
            textBoxDataTime = new TextBox();
            label8 = new Label();
            label1 = new Label();
            panel2 = new Panel();
            label7 = new Label();
            panel3 = new Panel();
            labelDateVideo = new Label();
            labelNameVideo = new Label();
            pictureBox1 = new PictureBox();
            labelTimeVideo = new Label();
            pictureBoxWagon = new PictureBox();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxWagon).BeginInit();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Location = new Point(303, 55);
            button1.Name = "button1";
            button1.Size = new Size(197, 31);
            button1.TabIndex = 1;
            button1.Text = "Открыть";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // panel1
            // 
            panel1.BorderStyle = BorderStyle.FixedSingle;
            panel1.Controls.Add(btnOpenFolderVideo);
            panel1.Controls.Add(btnOpenFolderPhoto);
            panel1.Controls.Add(textBoxVagonNumber);
            panel1.Controls.Add(label10);
            panel1.Controls.Add(textBoxVideoName);
            panel1.Controls.Add(label9);
            panel1.Controls.Add(textBoxPhotoName);
            panel1.Controls.Add(textBoxDataTime);
            panel1.Controls.Add(label8);
            panel1.Controls.Add(label1);
            panel1.Location = new Point(551, 3);
            panel1.Name = "panel1";
            panel1.Size = new Size(518, 314);
            panel1.TabIndex = 2;
            // 
            // btnOpenFolderVideo
            // 
            btnOpenFolderVideo.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
            btnOpenFolderVideo.Location = new Point(261, 247);
            btnOpenFolderVideo.Name = "btnOpenFolderVideo";
            btnOpenFolderVideo.Size = new Size(239, 38);
            btnOpenFolderVideo.TabIndex = 34;
            btnOpenFolderVideo.Text = "Открыть папку с видео";
            btnOpenFolderVideo.UseVisualStyleBackColor = true;
            btnOpenFolderVideo.Click += button2_Click;
            // 
            // btnOpenFolderPhoto
            // 
            btnOpenFolderPhoto.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
            btnOpenFolderPhoto.Location = new Point(16, 247);
            btnOpenFolderPhoto.Name = "btnOpenFolderPhoto";
            btnOpenFolderPhoto.Size = new Size(239, 38);
            btnOpenFolderPhoto.TabIndex = 33;
            btnOpenFolderPhoto.Text = "Открыть папку с фотографией";
            btnOpenFolderPhoto.UseVisualStyleBackColor = true;
            btnOpenFolderPhoto.Click += btnReadZeroCode_Click;
            // 
            // textBoxVagonNumber
            // 
            textBoxVagonNumber.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 204);
            textBoxVagonNumber.Location = new Point(263, 70);
            textBoxVagonNumber.Multiline = true;
            textBoxVagonNumber.Name = "textBoxVagonNumber";
            textBoxVagonNumber.ReadOnly = true;
            textBoxVagonNumber.Size = new Size(237, 30);
            textBoxVagonNumber.TabIndex = 32;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 204);
            label10.Location = new Point(16, 181);
            label10.Name = "label10";
            label10.Size = new Size(125, 21);
            label10.TabIndex = 29;
            label10.Text = "Название видео";
            // 
            // textBoxVideoName
            // 
            textBoxVideoName.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 204);
            textBoxVideoName.Location = new Point(263, 181);
            textBoxVideoName.Multiline = true;
            textBoxVideoName.Name = "textBoxVideoName";
            textBoxVideoName.ReadOnly = true;
            textBoxVideoName.Size = new Size(237, 30);
            textBoxVideoName.TabIndex = 28;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 204);
            label9.Location = new Point(16, 145);
            label9.Name = "label9";
            label9.Size = new Size(170, 21);
            label9.TabIndex = 27;
            label9.Text = "Название фотографии";
            // 
            // textBoxPhotoName
            // 
            textBoxPhotoName.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 204);
            textBoxPhotoName.Location = new Point(263, 145);
            textBoxPhotoName.Multiline = true;
            textBoxPhotoName.Name = "textBoxPhotoName";
            textBoxPhotoName.ReadOnly = true;
            textBoxPhotoName.Size = new Size(237, 30);
            textBoxPhotoName.TabIndex = 26;
            // 
            // textBoxDataTime
            // 
            textBoxDataTime.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 204);
            textBoxDataTime.Location = new Point(263, 109);
            textBoxDataTime.Multiline = true;
            textBoxDataTime.Name = "textBoxDataTime";
            textBoxDataTime.ReadOnly = true;
            textBoxDataTime.Size = new Size(237, 30);
            textBoxDataTime.TabIndex = 25;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 204);
            label8.Location = new Point(16, 112);
            label8.Name = "label8";
            label8.Size = new Size(143, 21);
            label8.TabIndex = 24;
            label8.Text = "Дата взвешивания";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 204);
            label1.Location = new Point(16, 73);
            label1.Name = "label1";
            label1.Size = new Size(110, 21);
            label1.TabIndex = 23;
            label1.Text = "Номер вагона";
            // 
            // panel2
            // 
            panel2.BackColor = SystemColors.GradientInactiveCaption;
            panel2.BorderStyle = BorderStyle.FixedSingle;
            panel2.Controls.Add(label7);
            panel2.Location = new Point(551, 3);
            panel2.Name = "panel2";
            panel2.Size = new Size(518, 49);
            panel2.TabIndex = 1;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("Segoe UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
            label7.Location = new Point(15, 11);
            label7.Name = "label7";
            label7.Size = new Size(207, 25);
            label7.TabIndex = 0;
            label7.Text = "Информация о вагоне";
            // 
            // panel3
            // 
            panel3.BorderStyle = BorderStyle.FixedSingle;
            panel3.Controls.Add(labelDateVideo);
            panel3.Controls.Add(labelNameVideo);
            panel3.Controls.Add(pictureBox1);
            panel3.Controls.Add(labelTimeVideo);
            panel3.Controls.Add(button1);
            panel3.Location = new Point(551, 334);
            panel3.Name = "panel3";
            panel3.Size = new Size(518, 99);
            panel3.TabIndex = 3;
            // 
            // labelDateVideo
            // 
            labelDateVideo.AutoSize = true;
            labelDateVideo.Location = new Point(60, 37);
            labelDateVideo.Name = "labelDateVideo";
            labelDateVideo.Size = new Size(38, 15);
            labelDateVideo.TabIndex = 3;
            labelDateVideo.Text = "label3";
            // 
            // labelNameVideo
            // 
            labelNameVideo.AutoSize = true;
            labelNameVideo.Location = new Point(60, 11);
            labelNameVideo.Name = "labelNameVideo";
            labelNameVideo.Size = new Size(38, 15);
            labelNameVideo.TabIndex = 2;
            labelNameVideo.Text = "label2";
            // 
            // pictureBox1
            // 
            pictureBox1.Image = Properties.Resources.png_transparent_video_production_freemake_video_er_video_icon_free_angle_text_rectangle;
            pictureBox1.Location = new Point(-1, -1);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(74, 61);
            pictureBox1.TabIndex = 5;
            pictureBox1.TabStop = false;
            // 
            // labelTimeVideo
            // 
            labelTimeVideo.AutoSize = true;
            labelTimeVideo.Location = new Point(16, 63);
            labelTimeVideo.Name = "labelTimeVideo";
            labelTimeVideo.Size = new Size(38, 15);
            labelTimeVideo.TabIndex = 4;
            labelTimeVideo.Text = "label2";
            // 
            // pictureBoxWagon
            // 
            pictureBoxWagon.Location = new Point(12, 3);
            pictureBoxWagon.Name = "pictureBoxWagon";
            pictureBoxWagon.Size = new Size(533, 441);
            pictureBoxWagon.TabIndex = 4;
            pictureBoxWagon.TabStop = false;
            // 
            // MediaWeighingForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1071, 603);
            Controls.Add(pictureBoxWagon);
            Controls.Add(panel3);
            Controls.Add(panel2);
            Controls.Add(panel1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "MediaWeighingForm";
            Text = "Фотовидеофиксация";
            Load += MediaWeighingForm_Load;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            panel3.ResumeLayout(false);
            panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxWagon).EndInit();
            ResumeLayout(false);
        }

        #endregion
        private Button button1;
        private Panel panel1;
        private Panel panel2;
        private Label label7;
        private TextBox textBoxVagonNumber;
        private Label label10;
        private TextBox textBoxVideoName;
        private Label label9;
        private TextBox textBoxPhotoName;
        private TextBox textBoxDataTime;
        private Label label8;
        private Label label1;
        private Button btnOpenFolderVideo;
        private Button btnOpenFolderPhoto;
        private Panel panel3;
        private Label labelTimeVideo;
        private Label labelDateVideo;
        private Label labelNameVideo;
        private PictureBox pictureBox1;
        private PictureBox pictureBoxWagon;
    }
}
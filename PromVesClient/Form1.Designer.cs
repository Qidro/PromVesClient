namespace PromVesClient
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            label1 = new Label();
            label2 = new Label();
            button1 = new Button();
            textBoxLogin = new TextBox();
            textBoxPassword = new TextBox();
            programVersion = new Label();
            pictureBox1 = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 20.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
            label1.Location = new Point(225, 310);
            label1.Name = "label1";
            label1.Size = new Size(189, 37);
            label1.TabIndex = 0;
            label1.Text = "Пользователь";
            label1.Click += label1_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 20.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
            label2.Location = new Point(225, 380);
            label2.Name = "label2";
            label2.Size = new Size(110, 37);
            label2.TabIndex = 1;
            label2.Text = "Пароль";
            // 
            // button1
            // 
            button1.Font = new Font("Segoe UI", 20.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
            button1.Location = new Point(472, 448);
            button1.Name = "button1";
            button1.Size = new Size(394, 49);
            button1.TabIndex = 2;
            button1.Text = "Принять";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // textBoxLogin
            // 
            textBoxLogin.Font = new Font("Segoe UI", 21.75F, FontStyle.Regular, GraphicsUnit.Point, 204);
            textBoxLogin.Location = new Point(472, 310);
            textBoxLogin.Multiline = true;
            textBoxLogin.Name = "textBoxLogin";
            textBoxLogin.Size = new Size(394, 49);
            textBoxLogin.TabIndex = 3;
            // 
            // textBoxPassword
            // 
            textBoxPassword.Font = new Font("Segoe UI", 21.75F, FontStyle.Regular, GraphicsUnit.Point, 204);
            textBoxPassword.Location = new Point(472, 380);
            textBoxPassword.Multiline = true;
            textBoxPassword.Name = "textBoxPassword";
            textBoxPassword.Size = new Size(394, 49);
            textBoxPassword.TabIndex = 4;
            // 
            // programVersion
            // 
            programVersion.AutoSize = true;
            programVersion.Location = new Point(1099, 537);
            programVersion.Name = "programVersion";
            programVersion.Size = new Size(38, 15);
            programVersion.TabIndex = 5;
            programVersion.Text = "label3";
            programVersion.Click += programVersion_Click;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = Properties.Resources.vesy_vagonnie;
            pictureBox1.Location = new Point(-3, -25);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(1234, 305);
            pictureBox1.TabIndex = 6;
            pictureBox1.TabStop = false;
            pictureBox1.Click += pictureBox1_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1230, 561);
            Controls.Add(pictureBox1);
            Controls.Add(programVersion);
            Controls.Add(textBoxPassword);
            Controls.Add(textBoxLogin);
            Controls.Add(button1);
            Controls.Add(label2);
            Controls.Add(label1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "Form1";
            Text = "Авторизация";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Label label2;
        private Button button1;
        private TextBox textBoxLogin;
        private TextBox textBoxPassword;
        private Label programVersion;
        private PictureBox pictureBox1;
    }
}

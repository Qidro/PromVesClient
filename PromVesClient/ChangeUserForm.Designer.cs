namespace PromVesClient
{
    partial class ChangeUserForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChangeUserForm));
            btnSave = new Button();
            txtLogin = new TextBox();
            cbRole = new ComboBox();
            txtPassword = new TextBox();
            btnDelete = new Button();
            cbStatus = new ComboBox();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            SuspendLayout();
            // 
            // btnSave
            // 
            btnSave.Location = new Point(274, 45);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(122, 42);
            btnSave.TabIndex = 0;
            btnSave.Text = "Изменить";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnSave_Click;
            // 
            // txtLogin
            // 
            txtLogin.Location = new Point(92, 45);
            txtLogin.Name = "txtLogin";
            txtLogin.Size = new Size(162, 23);
            txtLogin.TabIndex = 1;
            // 
            // cbRole
            // 
            cbRole.FormattingEnabled = true;
            cbRole.Location = new Point(92, 103);
            cbRole.Name = "cbRole";
            cbRole.Size = new Size(162, 23);
            cbRole.TabIndex = 2;
            // 
            // txtPassword
            // 
            txtPassword.Location = new Point(92, 74);
            txtPassword.Name = "txtPassword";
            txtPassword.Size = new Size(162, 23);
            txtPassword.TabIndex = 3;
            // 
            // btnDelete
            // 
            btnDelete.BackColor = Color.Red;
            btnDelete.ForeColor = SystemColors.ButtonHighlight;
            btnDelete.Location = new Point(274, 93);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(122, 42);
            btnDelete.TabIndex = 4;
            btnDelete.Text = "Удалить";
            btnDelete.UseVisualStyleBackColor = false;
            btnDelete.Click += btnDelete_Click;
            // 
            // cbStatus
            // 
            cbStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            cbStatus.FormattingEnabled = true;
            cbStatus.Items.AddRange(new object[] { "Активный", "Неактивный" });
            cbStatus.Location = new Point(92, 132);
            cbStatus.Name = "cbStatus";
            cbStatus.Size = new Size(162, 23);
            cbStatus.TabIndex = 5;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(16, 48);
            label1.Name = "label1";
            label1.Size = new Size(41, 15);
            label1.TabIndex = 6;
            label1.Text = "Логин";
            label1.Click += label1_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(16, 77);
            label2.Name = "label2";
            label2.Size = new Size(49, 15);
            label2.TabIndex = 7;
            label2.Text = "Пароль";
            label2.Click += label2_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(16, 106);
            label3.Name = "label3";
            label3.Size = new Size(34, 15);
            label3.TabIndex = 8;
            label3.Text = "Роль";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(16, 135);
            label4.Name = "label4";
            label4.Size = new Size(70, 15);
            label4.TabIndex = 9;
            label4.Text = "Активность";
            label4.Click += label4_Click;
            // 
            // ChangeUserForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(408, 233);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(cbStatus);
            Controls.Add(btnDelete);
            Controls.Add(txtPassword);
            Controls.Add(cbRole);
            Controls.Add(txtLogin);
            Controls.Add(btnSave);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "ChangeUserForm";
            Text = "Изменение пользователя";
            Load += ChangeUserForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnSave;
        private TextBox txtLogin;
        private ComboBox cbRole;
        private TextBox txtPassword;
        private Button btnDelete;
        private ComboBox cbStatus;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
    }
}
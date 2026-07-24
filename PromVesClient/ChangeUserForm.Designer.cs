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
            btnSave = new Button();
            txtLogin = new TextBox();
            cbRole = new ComboBox();
            txtPassword = new TextBox();
            btnDelete = new Button();
            cbStatus = new ComboBox();
            SuspendLayout();
            // 
            // btnSave
            // 
            btnSave.Location = new Point(211, 45);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(99, 42);
            btnSave.TabIndex = 0;
            btnSave.Text = "Изменить";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnSave_Click;
            // 
            // txtLogin
            // 
            txtLogin.Location = new Point(12, 45);
            txtLogin.Name = "txtLogin";
            txtLogin.Size = new Size(162, 23);
            txtLogin.TabIndex = 1;
            // 
            // cbRole
            // 
            cbRole.FormattingEnabled = true;
            cbRole.Location = new Point(12, 103);
            cbRole.Name = "cbRole";
            cbRole.Size = new Size(162, 23);
            cbRole.TabIndex = 2;
            // 
            // txtPassword
            // 
            txtPassword.Location = new Point(12, 74);
            txtPassword.Name = "txtPassword";
            txtPassword.Size = new Size(162, 23);
            txtPassword.TabIndex = 3;
            // 
            // btnDelete
            // 
            btnDelete.Location = new Point(211, 113);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(99, 42);
            btnDelete.TabIndex = 4;
            btnDelete.Text = "Удалить";
            btnDelete.UseVisualStyleBackColor = true;
            // 
            // cbStatus
            // 
            cbStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            cbStatus.FormattingEnabled = true;
            cbStatus.Items.AddRange(new object[] { "Активный", "Неактивный" });
            cbStatus.Location = new Point(12, 132);
            cbStatus.Name = "cbStatus";
            cbStatus.Size = new Size(162, 23);
            cbStatus.TabIndex = 5;
            // 
            // ChangeUserForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(357, 194);
            Controls.Add(cbStatus);
            Controls.Add(btnDelete);
            Controls.Add(txtPassword);
            Controls.Add(cbRole);
            Controls.Add(txtLogin);
            Controls.Add(btnSave);
            Name = "ChangeUserForm";
            Text = "ChangeUserForm";
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
    }
}
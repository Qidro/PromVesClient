namespace PromVesClient
{
    partial class UserManagementForm
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
            dgvUser = new DataGridView();
            dgvUserList = new DataGridView();
            btnCreateUser = new Button();
            btnEditUser = new Button();
            btnDeleteUser = new Button();
            ((System.ComponentModel.ISupportInitialize)dgvUser).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvUserList).BeginInit();
            SuspendLayout();
            // 
            // dgvUser
            // 
            dgvUser.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvUser.Location = new Point(12, 12);
            dgvUser.Name = "dgvUser";
            dgvUser.Size = new Size(621, 82);
            dgvUser.TabIndex = 0;
            // 
            // dgvUserList
            // 
            dgvUserList.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvUserList.Location = new Point(12, 122);
            dgvUserList.Name = "dgvUserList";
            dgvUserList.Size = new Size(621, 252);
            dgvUserList.TabIndex = 1;
            // 
            // btnCreateUser
            // 
            btnCreateUser.Location = new Point(663, 22);
            btnCreateUser.Name = "btnCreateUser";
            btnCreateUser.Size = new Size(125, 61);
            btnCreateUser.TabIndex = 2;
            btnCreateUser.Text = "button1";
            btnCreateUser.UseVisualStyleBackColor = true;
            // 
            // btnEditUser
            // 
            btnEditUser.Location = new Point(663, 153);
            btnEditUser.Name = "btnEditUser";
            btnEditUser.Size = new Size(125, 61);
            btnEditUser.TabIndex = 3;
            btnEditUser.Text = "button2";
            btnEditUser.UseVisualStyleBackColor = true;
            // 
            // btnDeleteUser
            // 
            btnDeleteUser.Location = new Point(663, 289);
            btnDeleteUser.Name = "btnDeleteUser";
            btnDeleteUser.Size = new Size(125, 61);
            btnDeleteUser.TabIndex = 4;
            btnDeleteUser.Text = "button3";
            btnDeleteUser.UseVisualStyleBackColor = true;
            // 
            // UserManagementForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 395);
            Controls.Add(btnDeleteUser);
            Controls.Add(btnEditUser);
            Controls.Add(btnCreateUser);
            Controls.Add(dgvUserList);
            Controls.Add(dgvUser);
            Name = "UserManagementForm";
            Text = "UserForm";
            ((System.ComponentModel.ISupportInitialize)dgvUser).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvUserList).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DataGridView dgvUser;
        private DataGridView dgvUserList;
        private Button btnCreateUser;
        private Button btnEditUser;
        private Button btnDeleteUser;
    }
}
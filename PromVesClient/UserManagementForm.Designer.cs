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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UserManagementForm));
            dgvUser = new DataGridView();
            colLogin = new DataGridViewTextBoxColumn();
            colPassword = new DataGridViewTextBoxColumn();
            colNewRole = new DataGridViewComboBoxColumn();
            dgvUserList = new DataGridView();
            colId = new DataGridViewTextBoxColumn();
            colName = new DataGridViewTextBoxColumn();
            colRole = new DataGridViewTextBoxColumn();
            colIsActive = new DataGridViewTextBoxColumn();
            btnCreateUser = new Button();
            btnChangeUser = new Button();
            btnDeleteUser = new Button();
            ((System.ComponentModel.ISupportInitialize)dgvUser).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvUserList).BeginInit();
            SuspendLayout();
            // 
            // dgvUser
            // 
            dgvUser.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvUser.Columns.AddRange(new DataGridViewColumn[] { colLogin, colPassword, colNewRole });
            dgvUser.Location = new Point(12, 12);
            dgvUser.Name = "dgvUser";
            dgvUser.Size = new Size(610, 82);
            dgvUser.TabIndex = 0;
            // 
            // colLogin
            // 
            colLogin.HeaderText = "Логин";
            colLogin.Name = "colLogin";
            colLogin.Width = 190;
            // 
            // colPassword
            // 
            colPassword.HeaderText = "Пароль";
            colPassword.Name = "colPassword";
            colPassword.Width = 190;
            // 
            // colNewRole
            // 
            colNewRole.HeaderText = "Роль";
            colNewRole.Items.AddRange(new object[] { "admin", "operator" });
            colNewRole.Name = "colNewRole";
            colNewRole.Width = 190;
            // 
            // dgvUserList
            // 
            dgvUserList.AllowUserToAddRows = false;
            dgvUserList.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvUserList.Columns.AddRange(new DataGridViewColumn[] { colId, colName, colRole, colIsActive });
            dgvUserList.Location = new Point(12, 131);
            dgvUserList.MultiSelect = false;
            dgvUserList.Name = "dgvUserList";
            dgvUserList.ReadOnly = true;
            dgvUserList.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvUserList.Size = new Size(610, 252);
            dgvUserList.TabIndex = 1;
            dgvUserList.CellContentClick += dgvUserList_CellContentClick;
            // 
            // colId
            // 
            colId.DataPropertyName = "Id";
            colId.HeaderText = "Id\n";
            colId.Name = "colId";
            colId.ReadOnly = true;
            colId.Visible = false;
            colId.Width = 42;
            // 
            // colName
            // 
            colName.DataPropertyName = "Name";
            colName.HeaderText = "Логин";
            colName.Name = "colName";
            colName.ReadOnly = true;
            colName.Width = 190;
            // 
            // colRole
            // 
            colRole.DataPropertyName = "Role";
            colRole.HeaderText = "Роль";
            colRole.Name = "colRole";
            colRole.ReadOnly = true;
            colRole.Width = 190;
            // 
            // colIsActive
            // 
            colIsActive.DataPropertyName = "IsActive";
            colIsActive.HeaderText = "Статус";
            colIsActive.Name = "colIsActive";
            colIsActive.ReadOnly = true;
            colIsActive.Width = 190;
            // 
            // btnCreateUser
            // 
            btnCreateUser.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 204);
            btnCreateUser.Location = new Point(648, 12);
            btnCreateUser.Name = "btnCreateUser";
            btnCreateUser.Size = new Size(168, 45);
            btnCreateUser.TabIndex = 2;
            btnCreateUser.Text = "Добавить пользователя";
            btnCreateUser.UseVisualStyleBackColor = true;
            btnCreateUser.Click += btnCreateUser_Click;
            // 
            // btnChangeUser
            // 
            btnChangeUser.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 204);
            btnChangeUser.Location = new Point(648, 63);
            btnChangeUser.Name = "btnChangeUser";
            btnChangeUser.Size = new Size(168, 48);
            btnChangeUser.TabIndex = 3;
            btnChangeUser.Text = "Изменить пользователя";
            btnChangeUser.UseVisualStyleBackColor = true;
            btnChangeUser.Click += btnChangeUser_Click;
            // 
            // btnDeleteUser
            // 
            btnDeleteUser.BackColor = Color.Red;
            btnDeleteUser.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 204);
            btnDeleteUser.ForeColor = SystemColors.ButtonHighlight;
            btnDeleteUser.ImageAlign = ContentAlignment.TopCenter;
            btnDeleteUser.Location = new Point(648, 117);
            btnDeleteUser.Name = "btnDeleteUser";
            btnDeleteUser.Size = new Size(168, 46);
            btnDeleteUser.TabIndex = 4;
            btnDeleteUser.Text = "Удалить пользователя";
            btnDeleteUser.UseVisualStyleBackColor = false;
            btnDeleteUser.Click += btnDeleteUser_Click;
            // 
            // UserManagementForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(848, 395);
            Controls.Add(btnDeleteUser);
            Controls.Add(btnChangeUser);
            Controls.Add(btnCreateUser);
            Controls.Add(dgvUserList);
            Controls.Add(dgvUser);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "UserManagementForm";
            Text = "Пользователи";
            Load += UserManagementForm_Load;
            ((System.ComponentModel.ISupportInitialize)dgvUser).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvUserList).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DataGridView dgvUser;
        private DataGridView dgvUserList;
        private Button btnCreateUser;
        private Button btnChangeUser;
        private Button btnDeleteUser;
        private DataGridViewTextBoxColumn colLogin;
        private DataGridViewTextBoxColumn colPassword;
        private DataGridViewComboBoxColumn colNewRole;
        private DataGridViewTextBoxColumn colId;
        private DataGridViewTextBoxColumn colName;
        private DataGridViewTextBoxColumn colRole;
        private DataGridViewTextBoxColumn colIsActive;
    }
}
namespace PromVesClient
{
    partial class WagonForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WagonForm));
            dgvWagon = new DataGridView();
            dgvWagonList = new DataGridView();
            btnCreate = new Button();
            btnChangeStatus = new Button();
            colNumber = new DataGridViewTextBoxColumn();
            colTareWeight = new DataGridViewTextBoxColumn();
            colLoadCapacity = new DataGridViewTextBoxColumn();
            colId = new DataGridViewTextBoxColumn();
            colListNumber = new DataGridViewTextBoxColumn();
            colListTareWeight = new DataGridViewTextBoxColumn();
            colListLoadCapacity = new DataGridViewTextBoxColumn();
            colListStatus = new DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)dgvWagon).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvWagonList).BeginInit();
            SuspendLayout();
            // 
            // dgvWagon
            // 
            dgvWagon.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvWagon.Columns.AddRange(new DataGridViewColumn[] { colNumber, colTareWeight, colLoadCapacity });
            dgvWagon.Location = new Point(12, 12);
            dgvWagon.Name = "dgvWagon";
            dgvWagon.Size = new Size(598, 98);
            dgvWagon.TabIndex = 0;
            // 
            // dgvWagonList
            // 
            dgvWagonList.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvWagonList.Columns.AddRange(new DataGridViewColumn[] { colId, colListNumber, colListTareWeight, colListLoadCapacity, colListStatus });
            dgvWagonList.Location = new Point(12, 206);
            dgvWagonList.Name = "dgvWagonList";
            dgvWagonList.Size = new Size(598, 149);
            dgvWagonList.TabIndex = 1;
            // 
            // btnCreate
            // 
            btnCreate.Location = new Point(12, 129);
            btnCreate.Name = "btnCreate";
            btnCreate.Size = new Size(138, 50);
            btnCreate.TabIndex = 2;
            btnCreate.Text = "Добавить";
            btnCreate.UseVisualStyleBackColor = true;
            btnCreate.Click += btnCreate_Click;
            // 
            // btnChangeStatus
            // 
            btnChangeStatus.Location = new Point(418, 129);
            btnChangeStatus.Name = "btnChangeStatus";
            btnChangeStatus.Size = new Size(192, 50);
            btnChangeStatus.TabIndex = 4;
            btnChangeStatus.Text = "Активировать/Деактивировать";
            btnChangeStatus.UseVisualStyleBackColor = true;
            btnChangeStatus.Click += btnChangeStatus_Click;
            // 
            // colNumber
            // 
            colNumber.HeaderText = "Номер вагона";
            colNumber.Name = "colNumber";
            colNumber.Width = 150;
            // 
            // colTareWeight
            // 
            colTareWeight.HeaderText = "Тара";
            colTareWeight.Name = "colTareWeight";
            colTareWeight.Width = 210;
            // 
            // colLoadCapacity
            // 
            colLoadCapacity.HeaderText = "Грузоподьемность";
            colLoadCapacity.Name = "colLoadCapacity";
            colLoadCapacity.Width = 195;
            // 
            // colId
            // 
            colId.HeaderText = "Column1";
            colId.Name = "colId";
            colId.Visible = false;
            colId.Width = 5;
            // 
            // colListNumber
            // 
            colListNumber.HeaderText = "Номер вагона";
            colListNumber.Name = "colListNumber";
            colListNumber.Width = 140;
            // 
            // colListTareWeight
            // 
            colListTareWeight.HeaderText = "Тара";
            colListTareWeight.Name = "colListTareWeight";
            colListTareWeight.Width = 140;
            // 
            // colListLoadCapacity
            // 
            colListLoadCapacity.HeaderText = "Грузоподьемность";
            colListLoadCapacity.Name = "colListLoadCapacity";
            colListLoadCapacity.Width = 150;
            // 
            // colListStatus
            // 
            colListStatus.HeaderText = "Статус";
            colListStatus.Name = "colListStatus";
            colListStatus.Width = 125;
            // 
            // WagonForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(673, 367);
            Controls.Add(btnChangeStatus);
            Controls.Add(btnCreate);
            Controls.Add(dgvWagonList);
            Controls.Add(dgvWagon);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "WagonForm";
            Text = "Справочник вагонов";
            Load += WagonForm_Load;
            ((System.ComponentModel.ISupportInitialize)dgvWagon).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvWagonList).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DataGridView dgvWagon;
        private DataGridView dgvWagonList;
        private Button btnCreate;
        private Button btnChangeStatus;
        private DataGridViewTextBoxColumn colNumber;
        private DataGridViewTextBoxColumn colTareWeight;
        private DataGridViewTextBoxColumn colLoadCapacity;
        private DataGridViewTextBoxColumn colId;
        private DataGridViewTextBoxColumn colListNumber;
        private DataGridViewTextBoxColumn colListTareWeight;
        private DataGridViewTextBoxColumn colListLoadCapacity;
        private DataGridViewTextBoxColumn colListStatus;
    }
}
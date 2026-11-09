namespace PromVesClient
{
    partial class DynamicWeighing
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DynamicWeighing));
            btnWeighing = new Button();
            dataGridViewReceipts = new DataGridView();
            NumberColumn = new DataGridViewTextBoxColumn();
            StartTimeColumn = new DataGridViewTextBoxColumn();
            EndTimeColumn = new DataGridViewTextBoxColumn();
            TotalWeightColumn = new DataGridViewTextBoxColumn();
            button1 = new Button();
            ((System.ComponentModel.ISupportInitialize)dataGridViewReceipts).BeginInit();
            SuspendLayout();
            // 
            // btnWeighing
            // 
            btnWeighing.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 204);
            btnWeighing.Location = new Point(288, 12);
            btnWeighing.Name = "btnWeighing";
            btnWeighing.Size = new Size(248, 43);
            btnWeighing.TabIndex = 24;
            btnWeighing.Text = "Начать взвешивание";
            btnWeighing.UseVisualStyleBackColor = true;
            btnWeighing.Click += btnWeighing_Click;
            // 
            // dataGridViewReceipts
            // 
            dataGridViewReceipts.AllowUserToOrderColumns = true;
            dataGridViewReceipts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewReceipts.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewReceipts.Columns.AddRange(new DataGridViewColumn[] { NumberColumn, StartTimeColumn, EndTimeColumn, TotalWeightColumn });
            dataGridViewReceipts.Location = new Point(28, 70);
            dataGridViewReceipts.Name = "dataGridViewReceipts";
            dataGridViewReceipts.ReadOnly = true;
            dataGridViewReceipts.Size = new Size(608, 419);
            dataGridViewReceipts.TabIndex = 25;
            dataGridViewReceipts.CellContentClick += dataGridViewReceipts_CellContentClick;
            // 
            // NumberColumn
            // 
            NumberColumn.HeaderText = "Номер";
            NumberColumn.Name = "NumberColumn";
            NumberColumn.ReadOnly = true;
            // 
            // StartTimeColumn
            // 
            StartTimeColumn.HeaderText = "Начало";
            StartTimeColumn.Name = "StartTimeColumn";
            StartTimeColumn.ReadOnly = true;
            // 
            // EndTimeColumn
            // 
            EndTimeColumn.HeaderText = "Окончание";
            EndTimeColumn.Name = "EndTimeColumn";
            EndTimeColumn.ReadOnly = true;
            // 
            // TotalWeightColumn
            // 
            TotalWeightColumn.HeaderText = "Общий вес";
            TotalWeightColumn.Name = "TotalWeightColumn";
            TotalWeightColumn.ReadOnly = true;
            // 
            // button1
            // 
            button1.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 204);
            button1.Location = new Point(288, 495);
            button1.Name = "button1";
            button1.Size = new Size(248, 43);
            button1.TabIndex = 26;
            button1.Text = "Сохранить взвешивание";
            button1.UseVisualStyleBackColor = true;
            // 
            // DynamicWeighing
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(648, 579);
            Controls.Add(button1);
            Controls.Add(dataGridViewReceipts);
            Controls.Add(btnWeighing);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "DynamicWeighing";
            Text = "Динамическое взвешивание";
            Load += DynamicWeighing_Load;
            ((System.ComponentModel.ISupportInitialize)dataGridViewReceipts).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Button btnWeighing;
        private DataGridView dataGridViewReceipts;
        private Button button1;
        private DataGridViewTextBoxColumn NumberColumn;
        private DataGridViewTextBoxColumn StartTimeColumn;
        private DataGridViewTextBoxColumn EndTimeColumn;
        private DataGridViewTextBoxColumn TotalWeightColumn;
    }
}
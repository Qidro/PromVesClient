namespace PromVesClient
{
    partial class frmWeighingReceipts
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmWeighingReceipts));
            btnReportFilter = new Button();
            dataGridViewReceipts = new DataGridView();
            groupBox1 = new GroupBox();
            operatorTextBox = new TextBox();
            operatorCheckBox = new CheckBox();
            vagonNumberBox = new CheckBox();
            vagonNumberTextBox = new TextBox();
            dateTimePicker2 = new DateTimePicker();
            label2 = new Label();
            label1 = new Label();
            dateTimePicker1 = new DateTimePicker();
            dataGridViewСards = new DataGridView();
            btnResetFilter = new Button();
            btnSaveReceipt = new Button();
            btnDeleteCard = new Button();
            btnDeleteReceipt = new Button();
            receiptInfoLabel = new Label();
            btnPrintReceipt = new Button();
            ((System.ComponentModel.ISupportInitialize)dataGridViewReceipts).BeginInit();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridViewСards).BeginInit();
            SuspendLayout();
            // 
            // btnReportFilter
            // 
            btnReportFilter.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 204);
            btnReportFilter.Location = new Point(1132, 333);
            btnReportFilter.Name = "btnReportFilter";
            btnReportFilter.Size = new Size(321, 36);
            btnReportFilter.TabIndex = 0;
            btnReportFilter.Text = "Применить фильтр";
            btnReportFilter.UseVisualStyleBackColor = true;
            btnReportFilter.Click += btnReportFilter_Click;
            // 
            // dataGridViewReceipts
            // 
            dataGridViewReceipts.AllowUserToOrderColumns = true;
            dataGridViewReceipts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewReceipts.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewReceipts.Location = new Point(12, 12);
            dataGridViewReceipts.Name = "dataGridViewReceipts";
            dataGridViewReceipts.ReadOnly = true;
            dataGridViewReceipts.Size = new Size(1097, 294);
            dataGridViewReceipts.TabIndex = 1;
            dataGridViewReceipts.CellClick += dataGridViewReceipts_CellClick;
            dataGridViewReceipts.CellContentClick += dataGridViewReceipts_CellContentClick;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(operatorTextBox);
            groupBox1.Controls.Add(operatorCheckBox);
            groupBox1.Controls.Add(vagonNumberBox);
            groupBox1.Controls.Add(vagonNumberTextBox);
            groupBox1.Controls.Add(dateTimePicker2);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(dateTimePicker1);
            groupBox1.Location = new Point(1132, 12);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(321, 305);
            groupBox1.TabIndex = 2;
            groupBox1.TabStop = false;
            groupBox1.Text = "Фильтр квитанции";
            // 
            // operatorTextBox
            // 
            operatorTextBox.Location = new Point(125, 140);
            operatorTextBox.Name = "operatorTextBox";
            operatorTextBox.Size = new Size(166, 23);
            operatorTextBox.TabIndex = 10;
            // 
            // operatorCheckBox
            // 
            operatorCheckBox.AutoSize = true;
            operatorCheckBox.Location = new Point(20, 144);
            operatorCheckBox.Name = "operatorCheckBox";
            operatorCheckBox.Size = new Size(80, 19);
            operatorCheckBox.TabIndex = 8;
            operatorCheckBox.Text = "Оператор";
            operatorCheckBox.UseVisualStyleBackColor = true;
            operatorCheckBox.CheckedChanged += checkBox2_CheckedChanged;
            // 
            // vagonNumberBox
            // 
            vagonNumberBox.AutoSize = true;
            vagonNumberBox.Location = new Point(20, 104);
            vagonNumberBox.Name = "vagonNumberBox";
            vagonNumberBox.Size = new Size(104, 19);
            vagonNumberBox.TabIndex = 7;
            vagonNumberBox.Text = "Номер вагона";
            vagonNumberBox.UseVisualStyleBackColor = true;
            vagonNumberBox.CheckedChanged += vagonNumberBox_CheckedChanged;
            // 
            // vagonNumberTextBox
            // 
            vagonNumberTextBox.Location = new Point(125, 104);
            vagonNumberTextBox.Name = "vagonNumberTextBox";
            vagonNumberTextBox.Size = new Size(166, 23);
            vagonNumberTextBox.TabIndex = 6;
            // 
            // dateTimePicker2
            // 
            dateTimePicker2.Location = new Point(125, 64);
            dateTimePicker2.Name = "dateTimePicker2";
            dateTimePicker2.Size = new Size(166, 23);
            dateTimePicker2.TabIndex = 3;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(72, 68);
            label2.Name = "label2";
            label2.Size = new Size(21, 15);
            label2.TabIndex = 2;
            label2.Text = "по";
            label2.Click += label2_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(52, 30);
            label1.Name = "label1";
            label1.Size = new Size(41, 15);
            label1.TabIndex = 1;
            label1.Text = "Дата с";
            label1.Click += label1_Click;
            // 
            // dateTimePicker1
            // 
            dateTimePicker1.Location = new Point(125, 24);
            dateTimePicker1.Name = "dateTimePicker1";
            dateTimePicker1.Size = new Size(166, 23);
            dateTimePicker1.TabIndex = 0;
            // 
            // dataGridViewСards
            // 
            dataGridViewСards.AllowUserToOrderColumns = true;
            dataGridViewСards.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewСards.Location = new Point(12, 365);
            dataGridViewСards.Name = "dataGridViewСards";
            dataGridViewСards.ReadOnly = true;
            dataGridViewСards.Size = new Size(1097, 417);
            dataGridViewСards.TabIndex = 3;
            // 
            // btnResetFilter
            // 
            btnResetFilter.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 204);
            btnResetFilter.Location = new Point(1132, 375);
            btnResetFilter.Name = "btnResetFilter";
            btnResetFilter.Size = new Size(321, 36);
            btnResetFilter.TabIndex = 4;
            btnResetFilter.Text = "Сбросить фильтр";
            btnResetFilter.UseVisualStyleBackColor = true;
            btnResetFilter.Click += button1_Click_1;
            // 
            // btnSaveReceipt
            // 
            btnSaveReceipt.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 204);
            btnSaveReceipt.Location = new Point(1132, 417);
            btnSaveReceipt.Name = "btnSaveReceipt";
            btnSaveReceipt.Size = new Size(321, 36);
            btnSaveReceipt.TabIndex = 5;
            btnSaveReceipt.Text = "Сохранить квитанцию";
            btnSaveReceipt.UseVisualStyleBackColor = true;
            btnSaveReceipt.Click += btnChangeReceipt_Click;
            // 
            // btnDeleteCard
            // 
            btnDeleteCard.BackColor = Color.Red;
            btnDeleteCard.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 204);
            btnDeleteCard.ForeColor = SystemColors.ControlLightLight;
            btnDeleteCard.Location = new Point(1132, 501);
            btnDeleteCard.Name = "btnDeleteCard";
            btnDeleteCard.Size = new Size(321, 36);
            btnDeleteCard.TabIndex = 6;
            btnDeleteCard.Text = "Удалить карточку вагона";
            btnDeleteCard.UseVisualStyleBackColor = false;
            btnDeleteCard.Click += btnDeleteCard_Click;
            // 
            // btnDeleteReceipt
            // 
            btnDeleteReceipt.BackColor = Color.Red;
            btnDeleteReceipt.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 204);
            btnDeleteReceipt.ForeColor = SystemColors.ControlLightLight;
            btnDeleteReceipt.Location = new Point(1132, 543);
            btnDeleteReceipt.Name = "btnDeleteReceipt";
            btnDeleteReceipt.Size = new Size(321, 36);
            btnDeleteReceipt.TabIndex = 7;
            btnDeleteReceipt.Text = "Удалить квитанцию";
            btnDeleteReceipt.UseVisualStyleBackColor = false;
            btnDeleteReceipt.Click += button4_Click;
            // 
            // receiptInfoLabel
            // 
            receiptInfoLabel.AutoSize = true;
            receiptInfoLabel.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 204);
            receiptInfoLabel.Location = new Point(356, 323);
            receiptInfoLabel.Name = "receiptInfoLabel";
            receiptInfoLabel.Size = new Size(107, 21);
            receiptInfoLabel.TabIndex = 8;
            receiptInfoLabel.Text = "Квитанция от";
            // 
            // btnPrintReceipt
            // 
            btnPrintReceipt.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 204);
            btnPrintReceipt.Location = new Point(1132, 459);
            btnPrintReceipt.Name = "btnPrintReceipt";
            btnPrintReceipt.Size = new Size(321, 36);
            btnPrintReceipt.TabIndex = 10;
            btnPrintReceipt.Text = "Распечатать квитанцию";
            btnPrintReceipt.UseVisualStyleBackColor = true;
            btnPrintReceipt.Click += btnPrintReceipt_Click;
            // 
            // frmWeighingReceipts
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1518, 909);
            Controls.Add(btnPrintReceipt);
            Controls.Add(receiptInfoLabel);
            Controls.Add(btnDeleteReceipt);
            Controls.Add(btnDeleteCard);
            Controls.Add(btnSaveReceipt);
            Controls.Add(btnResetFilter);
            Controls.Add(dataGridViewСards);
            Controls.Add(groupBox1);
            Controls.Add(dataGridViewReceipts);
            Controls.Add(btnReportFilter);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "frmWeighingReceipts";
            Text = "Квитанции взвешивания";
            Load += frmWeighingReceipts_Load;
            ((System.ComponentModel.ISupportInitialize)dataGridViewReceipts).EndInit();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridViewСards).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnReportFilter;
        private DataGridView dataGridViewReceipts;
        private GroupBox groupBox1;
        private DataGridView dataGridViewСards;
        private Button btnResetFilter;
        private Button btnSaveReceipt;
        private Button btnDeleteCard;
        private Button btnDeleteReceipt;
        private TextBox vagonNumberTextBox;
        private DateTimePicker dateTimePicker2;
        private Label label2;
        private Label label1;
        private DateTimePicker dateTimePicker1;
        private CheckBox operatorCheckBox;
        private CheckBox vagonNumberBox;
        private TextBox operatorTextBox;
        private Label receiptInfoLabel;
        private Button btnPrintReceipt;
    }
}
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
            button1 = new Button();
            button2 = new Button();
            button3 = new Button();
            button4 = new Button();
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
            dataGridViewСards.Location = new Point(12, 333);
            dataGridViewСards.Name = "dataGridViewСards";
            dataGridViewСards.Size = new Size(1097, 417);
            dataGridViewСards.TabIndex = 3;
            // 
            // button1
            // 
            button1.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 204);
            button1.Location = new Point(1132, 375);
            button1.Name = "button1";
            button1.Size = new Size(321, 36);
            button1.TabIndex = 4;
            button1.Text = "Сбросить фильтр";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click_1;
            // 
            // button2
            // 
            button2.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 204);
            button2.Location = new Point(1132, 417);
            button2.Name = "button2";
            button2.Size = new Size(321, 36);
            button2.TabIndex = 5;
            button2.Text = "Изменить квитанцию";
            button2.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            button3.BackColor = Color.Red;
            button3.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 204);
            button3.ForeColor = SystemColors.ControlLightLight;
            button3.Location = new Point(1132, 459);
            button3.Name = "button3";
            button3.Size = new Size(321, 36);
            button3.TabIndex = 6;
            button3.Text = "Удалить карточку вагона";
            button3.UseVisualStyleBackColor = false;
            // 
            // button4
            // 
            button4.BackColor = Color.Red;
            button4.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 204);
            button4.ForeColor = SystemColors.ControlLightLight;
            button4.Location = new Point(1132, 501);
            button4.Name = "button4";
            button4.Size = new Size(321, 36);
            button4.TabIndex = 7;
            button4.Text = "Удалить квитанцию";
            button4.UseVisualStyleBackColor = false;
            // 
            // frmWeighingReceipts
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1518, 909);
            Controls.Add(button4);
            Controls.Add(button3);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(dataGridViewСards);
            Controls.Add(groupBox1);
            Controls.Add(dataGridViewReceipts);
            Controls.Add(btnReportFilter);
            Name = "frmWeighingReceipts";
            Text = "Квитанции взвешивания";
            ((System.ComponentModel.ISupportInitialize)dataGridViewReceipts).EndInit();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridViewСards).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Button btnReportFilter;
        private DataGridView dataGridViewReceipts;
        private GroupBox groupBox1;
        private DataGridView dataGridViewСards;
        private Button button1;
        private Button button2;
        private Button button3;
        private Button button4;
        private TextBox vagonNumberTextBox;
        private DateTimePicker dateTimePicker2;
        private Label label2;
        private Label label1;
        private DateTimePicker dateTimePicker1;
        private CheckBox operatorCheckBox;
        private CheckBox vagonNumberBox;
        private TextBox operatorTextBox;
    }
}
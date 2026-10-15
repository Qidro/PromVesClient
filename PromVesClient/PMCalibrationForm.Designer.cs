namespace PromVesClient
{
    partial class PMCalibrationForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PMCalibrationForm));
            btnReadZeroCode = new Button();
            label1 = new Label();
            dataGridView2 = new DataGridView();
            ChannelNumber = new DataGridViewTextBoxColumn();
            ZeroCode = new DataGridViewTextBoxColumn();
            ReferenceCode = new DataGridViewTextBoxColumn();
            ReferenceWeight = new DataGridViewTextBoxColumn();
            WeightCoefficient = new DataGridViewTextBoxColumn();
            panel1 = new Panel();
            comboBoxChanel = new ComboBox();
            btnCalculateTheCoefficient = new Button();
            btnReadReferenceValue = new Button();
            textBoxWeightCoefficient = new TextBox();
            textBoxReferenceWeight = new TextBox();
            label12 = new Label();
            label11 = new Label();
            label10 = new Label();
            textBoxReferenceCode = new TextBox();
            label9 = new Label();
            textBoxZeroCode = new TextBox();
            textBoxCurrentADCcode = new TextBox();
            label8 = new Label();
            panel2 = new Panel();
            label7 = new Label();
            panel3 = new Panel();
            panel4 = new Panel();
            label13 = new Label();
            panel5 = new Panel();
            btnCancelChange = new Button();
            btnSaveChange = new Button();
            label2 = new Label();
            ((System.ComponentModel.ISupportInitialize)dataGridView2).BeginInit();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            panel3.SuspendLayout();
            panel4.SuspendLayout();
            panel5.SuspendLayout();
            SuspendLayout();
            // 
            // btnReadZeroCode
            // 
            btnReadZeroCode.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
            btnReadZeroCode.Location = new Point(16, 281);
            btnReadZeroCode.Name = "btnReadZeroCode";
            btnReadZeroCode.Size = new Size(239, 38);
            btnReadZeroCode.TabIndex = 2;
            btnReadZeroCode.Text = "Считать код нуля";
            btnReadZeroCode.UseVisualStyleBackColor = true;
            btnReadZeroCode.Click += btnReadZeroCode_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 204);
            label1.Location = new Point(16, 65);
            label1.Name = "label1";
            label1.Size = new Size(52, 21);
            label1.TabIndex = 8;
            label1.Text = "Канал";
            label1.Click += label1_Click;
            // 
            // dataGridView2
            // 
            dataGridView2.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView2.Columns.AddRange(new DataGridViewColumn[] { ChannelNumber, ZeroCode, ReferenceCode, ReferenceWeight, WeightCoefficient });
            dataGridView2.Location = new Point(3, 54);
            dataGridView2.Name = "dataGridView2";
            dataGridView2.Size = new Size(623, 438);
            dataGridView2.TabIndex = 1;
            // 
            // ChannelNumber
            // 
            ChannelNumber.HeaderText = "Канал";
            ChannelNumber.Name = "ChannelNumber";
            ChannelNumber.Width = 120;
            // 
            // ZeroCode
            // 
            ZeroCode.HeaderText = "Код нуля";
            ZeroCode.Name = "ZeroCode";
            ZeroCode.Width = 120;
            // 
            // ReferenceCode
            // 
            ReferenceCode.HeaderText = "Код эталона";
            ReferenceCode.Name = "ReferenceCode";
            ReferenceCode.Width = 120;
            // 
            // ReferenceWeight
            // 
            ReferenceWeight.HeaderText = "Эталонный вес, кг. ";
            ReferenceWeight.Name = "ReferenceWeight";
            ReferenceWeight.Width = 120;
            // 
            // WeightCoefficient
            // 
            WeightCoefficient.HeaderText = "Коэффицент";
            WeightCoefficient.Name = "WeightCoefficient";
            // 
            // panel1
            // 
            panel1.BorderStyle = BorderStyle.FixedSingle;
            panel1.Controls.Add(comboBoxChanel);
            panel1.Controls.Add(btnCalculateTheCoefficient);
            panel1.Controls.Add(btnReadReferenceValue);
            panel1.Controls.Add(textBoxWeightCoefficient);
            panel1.Controls.Add(textBoxReferenceWeight);
            panel1.Controls.Add(label12);
            panel1.Controls.Add(label11);
            panel1.Controls.Add(label10);
            panel1.Controls.Add(textBoxReferenceCode);
            panel1.Controls.Add(label9);
            panel1.Controls.Add(textBoxZeroCode);
            panel1.Controls.Add(textBoxCurrentADCcode);
            panel1.Controls.Add(label8);
            panel1.Controls.Add(panel2);
            panel1.Controls.Add(btnReadZeroCode);
            panel1.Controls.Add(label1);
            panel1.Location = new Point(655, 12);
            panel1.Name = "panel1";
            panel1.Size = new Size(518, 384);
            panel1.TabIndex = 15;
            panel1.Paint += panel1_Paint;
            // 
            // comboBoxChanel
            // 
            comboBoxChanel.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 204);
            comboBoxChanel.FormattingEnabled = true;
            comboBoxChanel.Items.AddRange(new object[] { "Канал 1", "Канал 2", "Канал 3", "Канал 4", "Канал 5", "Канал 6" });
            comboBoxChanel.Location = new Point(263, 66);
            comboBoxChanel.Name = "comboBoxChanel";
            comboBoxChanel.Size = new Size(237, 29);
            comboBoxChanel.TabIndex = 22;
            comboBoxChanel.SelectedIndexChanged += comboBoxChanel_SelectedIndexChanged;
            // 
            // btnCalculateTheCoefficient
            // 
            btnCalculateTheCoefficient.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
            btnCalculateTheCoefficient.Location = new Point(16, 332);
            btnCalculateTheCoefficient.Name = "btnCalculateTheCoefficient";
            btnCalculateTheCoefficient.Size = new Size(484, 38);
            btnCalculateTheCoefficient.TabIndex = 21;
            btnCalculateTheCoefficient.Text = "Рассчитать коэффициент";
            btnCalculateTheCoefficient.UseVisualStyleBackColor = true;
            btnCalculateTheCoefficient.Click += btnCalculateTheCoefficient_Click;
            // 
            // btnReadReferenceValue
            // 
            btnReadReferenceValue.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
            btnReadReferenceValue.Location = new Point(261, 281);
            btnReadReferenceValue.Name = "btnReadReferenceValue";
            btnReadReferenceValue.Size = new Size(239, 38);
            btnReadReferenceValue.TabIndex = 20;
            btnReadReferenceValue.Text = "Считать эталон";
            btnReadReferenceValue.UseVisualStyleBackColor = true;
            btnReadReferenceValue.Click += btnReadReferenceValue_Click;
            // 
            // textBoxWeightCoefficient
            // 
            textBoxWeightCoefficient.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 204);
            textBoxWeightCoefficient.Location = new Point(263, 245);
            textBoxWeightCoefficient.Multiline = true;
            textBoxWeightCoefficient.Name = "textBoxWeightCoefficient";
            textBoxWeightCoefficient.ReadOnly = true;
            textBoxWeightCoefficient.Size = new Size(237, 30);
            textBoxWeightCoefficient.TabIndex = 19;
            // 
            // textBoxReferenceWeight
            // 
            textBoxReferenceWeight.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 204);
            textBoxReferenceWeight.Location = new Point(263, 209);
            textBoxReferenceWeight.Multiline = true;
            textBoxReferenceWeight.Name = "textBoxReferenceWeight";
            textBoxReferenceWeight.Size = new Size(237, 30);
            textBoxReferenceWeight.TabIndex = 18;
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 204);
            label12.Location = new Point(16, 245);
            label12.Name = "label12";
            label12.Size = new Size(203, 21);
            label12.TabIndex = 17;
            label12.Text = "Коэффициент перерасчёта";
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 204);
            label11.Location = new Point(16, 209);
            label11.Name = "label11";
            label11.Size = new Size(136, 21);
            label11.TabIndex = 16;
            label11.Text = "Известный вес, кг";
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 204);
            label10.Location = new Point(16, 173);
            label10.Name = "label10";
            label10.Size = new Size(128, 21);
            label10.TabIndex = 15;
            label10.Text = "Код при эталоне";
            // 
            // textBoxReferenceCode
            // 
            textBoxReferenceCode.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 204);
            textBoxReferenceCode.Location = new Point(263, 173);
            textBoxReferenceCode.Multiline = true;
            textBoxReferenceCode.Name = "textBoxReferenceCode";
            textBoxReferenceCode.Size = new Size(237, 30);
            textBoxReferenceCode.TabIndex = 14;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 204);
            label9.Location = new Point(16, 137);
            label9.Name = "label9";
            label9.Size = new Size(74, 21);
            label9.TabIndex = 13;
            label9.Text = "Код нуля";
            // 
            // textBoxZeroCode
            // 
            textBoxZeroCode.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 204);
            textBoxZeroCode.Location = new Point(263, 137);
            textBoxZeroCode.Multiline = true;
            textBoxZeroCode.Name = "textBoxZeroCode";
            textBoxZeroCode.Size = new Size(237, 30);
            textBoxZeroCode.TabIndex = 12;
            // 
            // textBoxCurrentADCcode
            // 
            textBoxCurrentADCcode.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 204);
            textBoxCurrentADCcode.Location = new Point(263, 101);
            textBoxCurrentADCcode.Multiline = true;
            textBoxCurrentADCcode.Name = "textBoxCurrentADCcode";
            textBoxCurrentADCcode.Size = new Size(237, 30);
            textBoxCurrentADCcode.TabIndex = 11;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 204);
            label8.Location = new Point(16, 104);
            label8.Name = "label8";
            label8.Size = new Size(139, 21);
            label8.TabIndex = 10;
            label8.Text = "Текущуй код АЦП";
            // 
            // panel2
            // 
            panel2.BackColor = SystemColors.GradientInactiveCaption;
            panel2.BorderStyle = BorderStyle.FixedSingle;
            panel2.Controls.Add(label7);
            panel2.Location = new Point(0, -1);
            panel2.Name = "panel2";
            panel2.Size = new Size(517, 49);
            panel2.TabIndex = 0;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("Segoe UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
            label7.Location = new Point(15, 11);
            label7.Name = "label7";
            label7.Size = new Size(292, 25);
            label7.TabIndex = 0;
            label7.Text = "Калибровка выбранного канала";
            // 
            // panel3
            // 
            panel3.BorderStyle = BorderStyle.FixedSingle;
            panel3.Controls.Add(panel4);
            panel3.Controls.Add(dataGridView2);
            panel3.Location = new Point(12, 12);
            panel3.Name = "panel3";
            panel3.Size = new Size(637, 501);
            panel3.TabIndex = 16;
            // 
            // panel4
            // 
            panel4.BackColor = SystemColors.GradientInactiveCaption;
            panel4.BorderStyle = BorderStyle.FixedSingle;
            panel4.Controls.Add(label13);
            panel4.Location = new Point(0, -1);
            panel4.Name = "panel4";
            panel4.Size = new Size(636, 49);
            panel4.TabIndex = 0;
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Font = new Font("Segoe UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
            label13.Location = new Point(15, 11);
            label13.Name = "label13";
            label13.Size = new Size(188, 25);
            label13.TabIndex = 0;
            label13.Text = "Параметры каналов";
            // 
            // panel5
            // 
            panel5.BackColor = Color.FromArgb(224, 224, 224);
            panel5.BorderStyle = BorderStyle.FixedSingle;
            panel5.Controls.Add(btnCancelChange);
            panel5.Controls.Add(btnSaveChange);
            panel5.Controls.Add(label2);
            panel5.Location = new Point(-10, 519);
            panel5.Name = "panel5";
            panel5.Size = new Size(1210, 54);
            panel5.TabIndex = 17;
            // 
            // btnCancelChange
            // 
            btnCancelChange.Location = new Point(970, 10);
            btnCancelChange.Name = "btnCancelChange";
            btnCancelChange.Size = new Size(210, 35);
            btnCancelChange.TabIndex = 2;
            btnCancelChange.Text = "Отменить изменения";
            btnCancelChange.UseVisualStyleBackColor = true;
            // 
            // btnSaveChange
            // 
            btnSaveChange.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 204);
            btnSaveChange.Location = new Point(754, 10);
            btnSaveChange.Name = "btnSaveChange";
            btnSaveChange.Size = new Size(210, 35);
            btnSaveChange.TabIndex = 1;
            btnSaveChange.Text = "Сохранить изменения";
            btnSaveChange.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 204);
            label2.Location = new Point(38, 16);
            label2.Name = "label2";
            label2.Size = new Size(318, 21);
            label2.TabIndex = 0;
            label2.Text = "Файд конфигурации: ConfigСalibration.json";
            // 
            // PMCalibrationForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1184, 586);
            Controls.Add(panel5);
            Controls.Add(panel3);
            Controls.Add(panel1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "PMCalibrationForm";
            Text = "Калибровка каналов ПН-12";
            Load += PMCalibrationForm_Load;
            ((System.ComponentModel.ISupportInitialize)dataGridView2).EndInit();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            panel3.ResumeLayout(false);
            panel4.ResumeLayout(false);
            panel4.PerformLayout();
            panel5.ResumeLayout(false);
            panel5.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        private Button btnReadZeroCode;
        private Label label1;
        private DataGridView dataGridView2;
        private Panel panel1;
        private Panel panel2;
        private Label label7;
        private TextBox textBoxCurrentADCcode;
        private Label label8;
        private TextBox textBoxWeightCoefficient;
        private TextBox textBoxReferenceWeight;
        private Label label12;
        private Label label11;
        private Label label10;
        private TextBox textBoxReferenceCode;
        private Label label9;
        private TextBox textBoxZeroCode;
        private Button btnReadReferenceValue;
        private Button btnCalculateTheCoefficient;
        private Panel panel3;
        private Panel panel4;
        private Label label13;
        private Panel panel5;
        private Label label2;
        private Button btnCancelChange;
        private Button btnSaveChange;
        private ComboBox comboBoxChanel;
        private DataGridViewTextBoxColumn ChannelNumber;
        private DataGridViewTextBoxColumn ZeroCode;
        private DataGridViewTextBoxColumn ReferenceCode;
        private DataGridViewTextBoxColumn ReferenceWeight;
        private DataGridViewTextBoxColumn WeightCoefficient;
    }
}
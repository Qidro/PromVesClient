using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using PromVesClient.DTO;
using PromVesClient.Models;
using PromVesClient.Service.ReceiptsService;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PromVesClient
{
    public partial class frmWeighingReceipts : Form
    {
        private readonly ReceiptsService _receiptsService;

        private readonly ILogger<StaticWeighing> _logger;

        private List<ReceiptDto> receiptList;

        private List<СardsDto> cardsList;
        public frmWeighingReceipts(ReceiptsService receiptsService)
        {
            _receiptsService = receiptsService;
            InitializeComponent();
            dataGridViewСards.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            this.Load += Form1_Load;
        }
        //метод нажатия на кнопку фильтра поиска квитанции
        private async void btnReportFilter_Click(object sender, EventArgs e)
        {
            //WeighingDto dto = new WeighingDto
            //{
            //    Platform1Left = 32,
            //    Platform1Right = 32,
            //    Platform2Left = 32,
            //    Platform2Right = 32,
            //    VagonNumber = "comboBoxVagonNumber.Text",
            //    TareWeight = 32,
            //    GrossWeight = 23,
            //    //IdReceipt = IdReceipt
            //};

        }
        private async void Form1_Load(object sender, EventArgs e)
        {
            await loadingTableData();
        }
        //загрузка первоначальных (всех) данных таблицы квитанций
        private async Task loadingTableData()
        {
            var result = await _receiptsService.GetReceiptsAsync();
            //var resulet = await _receiptsService.GetWeighingAsync();
            if (result.Success == false)
            {
                MessageBox.Show($"Данные не были найдены, причина: {result.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            //List <WeighingDto> receipts = new List<WeighingDto>();
            //receipts.Add(dto);
            //dataGridView1.AutoGenerateColumns = false;

            //копируем результат запроса в поле
            receiptList = result.Data;
            dataGridViewReceipts.DataSource = result.Data;
            //dataGridView2.DataSource = resulet.Data;
            dataGridViewReceipts.Columns["Id"].Visible = false;
            //dataGridView1.Columns["Weighings"].Visible = false;
            dataGridViewReceipts.Columns["DateTime"].HeaderText = "Дата и время";
            dataGridViewReceipts.Columns["TypeWeighng"].HeaderText = "Тип взвешивания";
            dataGridViewReceipts.Columns["Operator"].HeaderText = "Оператор";
        }

        private void button1_Click_1(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void vagonNumberBox_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void dataGridViewReceipts_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            //if (e.RowIndex >= 0 && e.RowIndex < receiptList.Count)
            //{
            //    MessageBox.Show(
            //        $"Столбец: {e.ColumnIndex}\n" +
            //        $"Строка: {e.RowIndex}\n" +
            //        $"Id: {receiptList[e.RowIndex].Id}\n" +
            //        $"Дата: {receiptList[e.RowIndex].DateTime}"
            //    );
            //}
        }

        private async void dataGridViewReceipts_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < receiptList.Count)
            {
                //MessageBox.Show(
                //    //$"Столбец: {e.ColumnIndex}\n" +
                //   // $"Строка: {e.RowIndex}\n" +
                //    $"Id: {receiptList[e.RowIndex].Id}\n" +
                //    $"Дата: {receiptList[e.RowIndex].DateTime}"
                //);

                //выводим информацию о времени создания квитанции
                receiptInfoLabel.Text = "Квитанция от " + receiptList[e.RowIndex].DateTime.ToString();

                var result = await _receiptsService.GetWeighingAsync(receiptList[e.RowIndex].Id);
                if (result.Success == true)
                {
                    cardsList = result.Data;
                    dataGridViewСards.DataSource = result.Data;
                    settingViewTable();
                    
                }
                else 
                { 
                    
                }
            }
        }

        private void settingViewTable()
        {
            dataGridViewСards.Columns["Id"].Visible = false;
            dataGridViewСards.Columns["ReceiptId"].Visible = false;
            dataGridViewСards.Columns["VagonNumber"].HeaderText = "Номер вагона";
            dataGridViewСards.Columns["TareWeight"].HeaderText = "Тара т.";
            dataGridViewСards.Columns["GrossWeight"].HeaderText = "Брутто т.";
            dataGridViewСards.Columns["NetWeight"].HeaderText = "Нетто т.";
            dataGridViewСards.Columns["LoadCapacity"].HeaderText = "Грузоподъемность";
            dataGridViewСards.Columns["LoadDeviation"].HeaderText = "недогруз/перегруз т.";
            dataGridViewСards.Columns["FirstCart"].HeaderText = "первая тележка т.";
            dataGridViewСards.Columns["SecondCart"].HeaderText = "вторая тележка т.";
            dataGridViewСards.Columns["DifferenceCarts"].HeaderText = "разница тележек т.";
            dataGridViewСards.Columns["LeftSide"].HeaderText = "левый борт т.";
            dataGridViewСards.Columns["RightSide"].HeaderText = "правый борт т.";
            dataGridViewСards.Columns["DifferenceSides"].HeaderText = "разница бортов т.";
            dataGridViewСards.Columns["TypeWeighing"].HeaderText = "Тип взвешивания";


        }
    }
}

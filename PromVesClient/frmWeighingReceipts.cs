using Microsoft.EntityFrameworkCore.Metadata.Internal;
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
        public frmWeighingReceipts(ReceiptsService receiptsService)
        {
            _receiptsService = receiptsService;
            InitializeComponent();
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
        private async Task loadingTableData()
        {
            var result = await _receiptsService.GetReceiptsAsync();
            //var resulet = await _receiptsService.GetWeighingAsync();

            //List <WeighingDto> receipts = new List<WeighingDto>();
            //receipts.Add(dto);
            //dataGridView1.AutoGenerateColumns = false;

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
    }
}

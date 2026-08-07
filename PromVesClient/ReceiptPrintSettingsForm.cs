using PromVesClient.Models;
using PromVesClient.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PromVesClient
{
    public partial class ReceiptPrintSettingsForm : Form
    {
        private readonly ReceiptPrintSettingsService _receiptPrintSettingsService;

        public ReceiptPrintSettingsForm(ReceiptPrintSettingsService receiptPrintSettingsService)
        {
            InitializeComponent();

            _receiptPrintSettingsService = receiptPrintSettingsService;
        }

        private void ReceiptPrintSettingsForm_Load(object sender, EventArgs e)
        {
            LoadSettings();
        }
        //метод загрузки в форму настроек из ReceiptPrintSettingsServic
        private void LoadSettings()
        {
            var result = _receiptPrintSettingsService.Load();

            if (!result.Success)
            {
                MessageBox.Show(
                    result.Message,
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return;
            }
            var settings = result.Data;

            if (settings == null)
            {
                MessageBox.Show(
                    "Настройки не загружены.",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return;
            }


            chkVagonNumber.Checked = settings.VagonNumber;

            chkL1.Checked = settings.L1;
            chkR1.Checked = settings.R1;
            chkL2.Checked = settings.L2;
            chkR2.Checked = settings.R2;

            chkTareWeight.Checked = settings.TareWeight;
            chkGrossWeight.Checked = settings.GrossWeight;
            chkNetWeight.Checked = settings.NetWeight;

            chkLoadCapacity.Checked = settings.LoadCapacity;
            chkLoadDeviation.Checked = settings.LoadDeviation;

            chkFirstCart.Checked = settings.FirstCart;
            chkSecondCart.Checked = settings.SecondCart;
            chkDifferenceCarts.Checked = settings.DifferenceCarts;

            chkLeftSide.Checked = settings.LeftSide;
            chkRightSide.Checked = settings.RightSide;
            chkDifferenceSides.Checked = settings.DifferenceSides;

            chkTypeWeighing.Checked = settings.TypeWeighing;

            chkShipper.Checked = settings.Shipper;
            chkConsignee.Checked = settings.Consignee;
            chkCargo.Checked = settings.Cargo;

            chkInvoiceNumber.Checked = settings.InvoiceNumber;
            chkInvoiceDateTime.Checked = settings.InvoiceDateTime;
            chkInvoiceWeighing.Checked = settings.InvoiceWeighing;
        }
        // Метод сохранения настроек из формы в JSON
        private void SaveSettings()
        {
            // Создаем объект настроек и передаем ему
            // текущие значения всех CheckBox
            var settings = new ReceiptPrintSettings
            {
                VagonNumber = chkVagonNumber.Checked,

                L1 = chkL1.Checked,
                R1 = chkR1.Checked,
                L2 = chkL2.Checked,
                R2 = chkR2.Checked,

                TareWeight = chkTareWeight.Checked,
                GrossWeight = chkGrossWeight.Checked,
                NetWeight = chkNetWeight.Checked,

                LoadCapacity = chkLoadCapacity.Checked,
                LoadDeviation = chkLoadDeviation.Checked,

                FirstCart = chkFirstCart.Checked,
                SecondCart = chkSecondCart.Checked,
                DifferenceCarts = chkDifferenceCarts.Checked,

                LeftSide = chkLeftSide.Checked,
                RightSide = chkRightSide.Checked,
                DifferenceSides = chkDifferenceSides.Checked,

                TypeWeighing = chkTypeWeighing.Checked,

                Shipper = chkShipper.Checked,
                Consignee = chkConsignee.Checked,
                Cargo = chkCargo.Checked,

                InvoiceNumber = chkInvoiceNumber.Checked,
                InvoiceDateTime = chkInvoiceDateTime.Checked,
                InvoiceWeighing = chkInvoiceWeighing.Checked
            };

            // Передаем настройки в сервис для сохранения
            var result = _receiptPrintSettingsService.Save(settings);

            // Проверяем результат сохранения
            if (!result.Success)
            {
                MessageBox.Show(
                    result.Message,
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return;
            }

            // Сообщаем пользователю об успешном сохранении
            MessageBox.Show(
                "Настройки успешно сохранены.",
                "Сохранение",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveSettings();
        }
    }
}

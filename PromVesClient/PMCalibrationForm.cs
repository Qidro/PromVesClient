using Microsoft.Extensions.Logging;
using PromVesClient.Models.ConfigurationModels;
using PromVesClient.Service.PNCalibrationService;
using PromVesClient.Service.UserService;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PromVesClient
{
    public partial class PMCalibrationForm : Form
    {
        private readonly CalibrationService _calibrationService;
        private readonly ILogger<PMCalibrationForm> _logger;

        private List<PNSetting> PNSettingList = new List<PNSetting>();
        public PMCalibrationForm(CalibrationService calibrationService, ILogger<PMCalibrationForm> logger)
        {
            _logger = logger;
            _calibrationService = calibrationService;
            InitializeComponent();
            dataGridView2.Columns["ChannelNumber"].DataPropertyName = "ChannelNumber";
            dataGridView2.Columns["ZeroCode"].DataPropertyName = "ZeroCode";
            dataGridView2.Columns["ReferenceCode"].DataPropertyName = "ReferenceCode";
            dataGridView2.Columns["ReferenceWeight"].DataPropertyName = "ReferenceWeight";
            dataGridView2.Columns["WeightCoefficient"].DataPropertyName = "WeightCoefficient";
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void comboBoxChanel_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxChanel.SelectedItem == null || PNSettingList == null)
                return;

            // Получаем строку, например "Канал 1"
            string selectedChannel = comboBoxChanel.SelectedItem.ToString();

            // Извлекаем номер канала из строки
            string number = new string(selectedChannel.Where(char.IsDigit).ToArray());

            if (!int.TryParse(number, out int channelNumber))
                return;

            // Ищем настройки выбранного канала
            var setting = PNSettingList.FirstOrDefault(
                x => x.ChannelNumber == channelNumber);

            if (setting == null)
                return;

            textBoxZeroCode.Text = setting.ZeroCode.ToString();
            textBoxReferenceCode.Text = setting.ReferenceCode.ToString();
            textBoxReferenceWeight.Text = setting.ReferenceWeight.ToString();
            textBoxWeightCoefficient.Text = setting.WeightCoefficient.ToString();
        }

        private async void PMCalibrationForm_Load(object sender, EventArgs e)
        {
            await loadingTableData();
        }
        //загрузка первоначальных (всех) данных таблицы калибровки
        private async Task loadingTableData()
        {
            var result = await _calibrationService.GetSettingCalibration();
            if (result.Success == true && result.Data != null)
            {
                PNSettingList = result.Data;
                dataGridView2.DataSource = PNSettingList;
            }
        }
        //--------------------------------кнопка отвечающая за калиброку нуля------------------------------------
        private async void btnReadZeroCode_Click(object sender, EventArgs e)
        {
            //проверка на прогрузку данных и выбора канала
            if (comboBoxChanel.SelectedItem == null || PNSettingList == null)
            {
                MessageBox.Show("Выберите канал для калибровки", "Предупрждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Получаем номер из строки "Канал 1"
            string selectedChannel = comboBoxChanel.SelectedItem.ToString();
            string number = new string(selectedChannel.Where(char.IsDigit).ToArray());

            if (!int.TryParse(number, out int channelNumber))
                return;

            // Получаем код нуля из TextBox
            if (!decimal.TryParse(
                textBoxCurrentADCcode.Text,
                System.Globalization.NumberStyles.Number,
                System.Globalization.CultureInfo.CurrentCulture,
                out decimal zeroCode))
            {
                MessageBox.Show("Введите корректный код нуля", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Находим настройки выбранного канала
            var setting = PNSettingList.FirstOrDefault(
                x => x.ChannelNumber == channelNumber);

            if (setting == null)
            {
                MessageBox.Show("Настройки выбранного канала не найдены", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Обновляем код нуля
            setting.ZeroCode = zeroCode;
            textBoxZeroCode.Text = zeroCode.ToString();
            // Обновляем отображение таблицы
            dataGridView2.Refresh();
        }

        //------------------------------калибровка известного веса--------------------------------------
        private void btnReadReferenceValue_Click(object sender, EventArgs e)
        {
            //проверка на прогрузку данных и выбора канала
            if (comboBoxChanel.SelectedItem == null || PNSettingList == null)
            {
                MessageBox.Show("Выберите канал для калибровки", "Предупрждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Получаем номер из строки "Канал"
            string selectedChannel = comboBoxChanel.SelectedItem.ToString();
            string number = new string(selectedChannel.Where(char.IsDigit).ToArray());

            if (!int.TryParse(number, out int channelNumber))
                return;

            // Получаем код нуля из TextBox
            if (!decimal.TryParse(
                textBoxCurrentADCcode.Text,
                System.Globalization.NumberStyles.Number,
                System.Globalization.CultureInfo.CurrentCulture,
                out decimal referenceCode))
            {
                MessageBox.Show("Введите корректный код нуля", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            // Получаем эталонный вес из TextBox
            if (!decimal.TryParse(
                textBoxReferenceWeight.Text,
                System.Globalization.NumberStyles.Number,
                System.Globalization.CultureInfo.CurrentCulture,
                out decimal referenceWeight))
            {
                MessageBox.Show("Введите корректный эталонный вес", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (referenceWeight <= 0)
            {
                MessageBox.Show("Эталонный вес должен быть больше нуля.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            // Находим настройки выбранного канала
            var setting = PNSettingList.FirstOrDefault(
                x => x.ChannelNumber == channelNumber);

            if (setting == null)
            {
                MessageBox.Show("Настройки выбранного канала не найдены", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Обновляем код нуля
            setting.ReferenceCode = referenceCode;
            setting.ReferenceWeight = referenceWeight;
            textBoxReferenceCode.Text = referenceCode.ToString();
            // Обновляем отображение таблицы
            dataGridView2.Refresh();
        }

        //--------------------расчет коэффицента----------------------------------
        private async void btnCalculateTheCoefficient_Click(object sender, EventArgs e)
        {
            //проверка на прогрузку данных и выбора канала
            if (comboBoxChanel.SelectedItem == null || PNSettingList == null)
            {
                MessageBox.Show("Выберите канал для калибровки", "Предупрждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Получаем номер из строки "Канал"
            string selectedChannel = comboBoxChanel.SelectedItem.ToString();
            string number = new string(selectedChannel.Where(char.IsDigit).ToArray());

            if (!int.TryParse(number, out int channelNumber))
                return;

            // Получаем код нуля из TextBox
            if (!decimal.TryParse(
                textBoxZeroCode.Text,
                System.Globalization.NumberStyles.Number,
                System.Globalization.CultureInfo.CurrentCulture,
                out decimal codeZero))
            {
                MessageBox.Show("Введите корректный код нуля", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            // Получаем эталонный вес из TextBox
            if (!decimal.TryParse(
                textBoxReferenceWeight.Text,
                System.Globalization.NumberStyles.Number,
                System.Globalization.CultureInfo.CurrentCulture,
                out decimal referenceWeight))
            {
                MessageBox.Show("Введите корректный эталонный вес", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Получаем КОД эталонного веса
            if (!decimal.TryParse(
                textBoxReferenceCode.Text,
                System.Globalization.NumberStyles.Number,
                System.Globalization.CultureInfo.CurrentCulture,
                out decimal referenceCode))
            {
                MessageBox.Show("Введите корректный эталонный вес", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Находим настройки выбранного канала
            var setting = PNSettingList.FirstOrDefault(
                x => x.ChannelNumber == channelNumber);

            if (setting == null)
            {
                MessageBox.Show("Настройки выбранного канала не найдены", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            //расчет делты
            decimal delta = referenceCode - codeZero;
            //проверка деления на ноль
            if (delta == 0)
            {
                MessageBox.Show(
                    "Невозможно рассчитать коэффициент: " +
                    "код АЦП с эталоном совпадает с кодом нуля.",
                    "Ошибка калибровки",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }
            if (referenceWeight <= 0)
            {
                MessageBox.Show("Эталонный вес должен быть больше нуля.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            //расчте коэффицента
            decimal coefficient = referenceWeight / delta;
            // Обновляем код нуля
            setting.WeightCoefficient = coefficient;
            textBoxWeightCoefficient.Text = coefficient.ToString();
            // Обновляем отображение таблицы
            dataGridView2.Refresh();
        }
    }
}

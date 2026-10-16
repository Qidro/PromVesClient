using Microsoft.Extensions.Logging;
using PromVesClient.Models.ConfigurationModels;
using PromVesClient.Service.PNCalibrationService;
using PromVesClient.Service.TcpService;
using PromVesClient.Service.UserService;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Windows.Forms;

namespace PromVesClient
{
    public partial class PMCalibrationForm : Form
    {
        private readonly CalibrationService _calibrationService;
        private readonly ILogger<PMCalibrationForm> _logger;

        private List<PNSetting> PNSettingList = new List<PNSetting>();
        private List<PNSetting> PNSettingSaveList = new List<PNSetting>();
        //обьект, который отвечает за подключение/отключение/получение данных сервера
        private readonly TcpService _tcpService;

        //переменная предназначенная для соханения данных веса с бортов
        private readonly decimal[] cartSideWeights = new decimal[6];
        public PMCalibrationForm(CalibrationService calibrationService, ILogger<PMCalibrationForm> logger, TcpService tcpService)
        {
            _logger = logger;
            _calibrationService = calibrationService;
            _tcpService = tcpService;
            InitializeComponent();
            dataGridView2.Columns["ChannelNumber"].DataPropertyName = "ChannelNumber";
            dataGridView2.Columns["ZeroCode"].DataPropertyName = "ZeroCode";
            dataGridView2.Columns["ReferenceCode"].DataPropertyName = "ReferenceCode";
            dataGridView2.Columns["ReferenceWeight"].DataPropertyName = "ReferenceWeight";
            dataGridView2.Columns["WeightCoefficient"].DataPropertyName = "WeightCoefficient";

            //регистрации метода на ожидание новых данных
            _tcpService.MessageReceived += ProcessMessage;
            //регистрация метода на ожидание ошибок
            _tcpService.ConnectionError += OnConnectionError;
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
            await ConnectToServerAsync();

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
            else 
            {
                MessageBox.Show($"Ошибка загрузки первончальных данных, причина: {result.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task ConnectToServerAsync()
        {
            await _tcpService.ConnectAsync();
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

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void panel6_Paint(object sender, PaintEventArgs e)
        {
            //LimeGreen
        }

        private async void btnSaveChange_Click(object sender, EventArgs e)
        {
            //PNSettingList.Clear();
            foreach (DataGridViewRow row in dataGridView2.Rows)
            {
                // Пропускаем пустую строку для добавления новой записи
                if (row.IsNewRow)
                    continue;

                var setting = new PNSetting
                {
                    ChannelNumber = Convert.ToInt32(
                        row.Cells["ChannelNumber"].Value),

                    ZeroCode = Convert.ToDecimal(
                        row.Cells["ZeroCode"].Value),

                    ReferenceCode = Convert.ToDecimal(
                        row.Cells["ReferenceCode"].Value),

                    ReferenceWeight = Convert.ToDecimal(
                        row.Cells["ReferenceWeight"].Value),

                    WeightCoefficient = Convert.ToDecimal(
                        row.Cells["WeightCoefficient"].Value)
                };

                PNSettingSaveList.Add(setting);
            }

            // Сохраняем обновлённый список в JSON
            await _calibrationService.SaveSettingCalibrationAsync(PNSettingSaveList);
            PNSettingSaveList.Clear();
        }

        private async void btnCancelChange_Click(object sender, EventArgs e)
        {
            await loadingTableData();
        }

        //метод для события(получения данных с сервака) по обработке полцченных данных
        private void ProcessMessage(string message)
        {
            try
            {
                bool connectScales = ConnectionScalesCheck(message);
                if (connectScales == false)
                {
                    return;
                }
                //Console.WriteLine("мы находится в событии");
                string[] parts = message.Split(';');
                if (cartSideWeights.Length == parts.Length)
                {
                    //for (int i = 0; i < cartSideWeights.Length; i++)
                    //{
                    //    if (decimal.TryParse(parts[i], NumberStyles.Number, CultureInfo.InvariantCulture, out var value))
                    //        cartSideWeights[i] = value;
                    //}
                    if (comboBoxChanel.SelectedIndex != -1)
                    {
                        int index = comboBoxChanel.SelectedIndex;
                        textBoxCurrentADCcode.Text = parts[index];
                        // Элемент выбран
                    }
                    else
                    {
                        // Ничего не выбрано
                    }

                }
            }
            catch (FormatException ex)
            {
                _logger.LogWarning(ex, "Некорректный формат данных");
            }
            catch (IndexOutOfRangeException ex)
            {
                _logger.LogWarning(ex, "Получено неполное сообщение");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка обработки сообщения");
            }

            //AddPoint(cartSideWeights[0]);
            //BeginInvoke(() =>
            //{
            //    listBox1.Items.Add(message);
            //    // либо:
            //    // textBox1.AppendText(message + Environment.NewLine);
            //});
        }

        //проверка сообщения от сервера на связь с весами
        private bool ConnectionScalesCheck(string data)
        {
            string[] parts = data.Split(';');
            for (int i = 0; parts.Length > i; i++)
            {
                //проверка на то, что сервер прислал, что соединения с весами нет - обозначаем это
                if (parts[i] == "OFFLINE")
                {
                    //выводим, что соединение нет
                    panel6.BackColor = Color.Red;
                    //btnSaveWeight.Enabled = false;
                    btnSaveChange.Enabled = false;
                    return false;
                }
            }
            //выводим, что соединение есть
            panel6.BackColor = Color.Green;
            btnSaveChange.Enabled = true;
            // btnSaveWeight.Enabled = true;
            return true;
        }

        //событие ошибки
        private async void OnConnectionError(Exception ex)
        {
            BeginInvoke(() =>
            {

                MessageBox.Show(
                    ex.Message + ". Пытаемся переподключиться",
                    "Ошибка сервера",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            });
            //переподключение к серверу
            while (true)
            {
                try
                {

                        await _tcpService.DisconnectAsync();

                        await Task.Delay(5000);

                        await _tcpService.ConnectAsync();
                        panel6.BackColor = Color.Green;
                        break;

                }
                catch (Exception reconnectEx)
                {
                    panel6.BackColor = Color.Red;
                    _logger.LogWarning(reconnectEx,
                        "Не удалось подключиться серверу калибровки прибора. Повтор через 5 секунд.");

                }
            }
        }
    }
}

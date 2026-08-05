using PromVesClient.Models;
using PromVesClient.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Text;
using System.Windows.Forms;


namespace PromVesClient
{
    public partial class ComPortSettingsForm : Form
    {
        private readonly ComPortService _comPortService;
        //для портов, чтобы не повторялись
        private bool _updatingPorts;
     

        private List<ComboBox> _portBoxes;
        private List<ComboBox> _baudRateBoxes;
        private List<ComboBox> _dataBitsBoxes;
        private List<ComboBox> _parityBoxes;
        private List<ComboBox> _stopBitsBoxes;
        private List<ComboBox> _handshakeBoxes;
        public ComPortSettingsForm(ComPortService comPortService)
        {
            InitializeComponent();

            _comPortService = comPortService;
            //иницилизация настроек компрта
            InitializeCollections();
            //подгрузка данных для компротов
            SubscribePortEvents();
        }

        private void ComPortSettingsForm_Load(object sender, EventArgs e)
        {
            FillComboBox();
            LoadSettings();
        }
        private void FillComboBox()
        {
            FillPorts();
            FillBaudRates();
            FillDataBits();
            FillParity();
            FillStopBits();
            FillHandshake();
        }
        // обьединяем в колекции комбо боксы
        private void InitializeCollections()
        {
            _portBoxes = new List<ComboBox>
            {
                cbPort1,
                cbPort2,
                cbPort3,
                cbPort4
            };

            _baudRateBoxes = new List<ComboBox>
            {
                cbBaudRate1,
                cbBaudRate2,
                cbBaudRate3,
                cbBaudRate4
            };

            _dataBitsBoxes = new List<ComboBox>
            {
                cbDataBits1,
                cbDataBits2,
                cbDataBits3,
                cbDataBits4
            };

            _parityBoxes = new List<ComboBox>
            {
                cbParity1,
                cbParity2,
                cbParity3,
                cbParity4
            };

            _stopBitsBoxes = new List<ComboBox>
            {
                cbStopBits1,
                cbStopBits2,
                cbStopBits3,
                cbStopBits4
            };

            _handshakeBoxes = new List<ComboBox>
            {
                cbHandshake1,
                cbHandshake2,
                cbHandshake3,
                cbHandshake4
            };
        }
        // метод универсального заполнения
        private void FillComboBoxes<T>(
    IEnumerable<ComboBox> comboBoxes,
    IEnumerable<T> values)
        {
            foreach (var comboBox in comboBoxes)
            {
                comboBox.Items.Clear();
                comboBox.DropDownStyle = ComboBoxStyle.DropDownList;

                foreach (var value in values)
                {
                    comboBox.Items.Add(value);
                }
            }
        }
        //поиск портов и добавления их
        private void FillPorts()
        {
            FillComboBoxes(
                _portBoxes,
                SerialPort.GetPortNames());
        }
        //метод заполнения BaudRates
        private void FillBaudRates()
        {
            FillComboBoxes(
                _baudRateBoxes,
                new[]
                {
            300,
            600,
            1200,
            2400,
            4800,
            9600,
            19200,
            38400,
            57600,
            115200
                });
        }
        private void FillDataBits()
        {
            FillComboBoxes(
                _dataBitsBoxes,
                new[]
                {
            5,
            6,
            7,
            8
                });
        }
        // следующие методы значения подгружаются из .NET
        private void FillParity()
        {
            FillComboBoxes(
                _parityBoxes,
                Enum.GetNames<Parity>());
        }
        private void FillStopBits()
        {
            FillComboBoxes(
                _stopBitsBoxes,
                Enum.GetNames<StopBits>());
        }
        private void FillHandshake()
        {
            FillComboBoxes(
                _handshakeBoxes,
                Enum.GetNames<Handshake>());
        }
        // заполнение одного порта
        private void LoadSettings()
        {
            var result = _comPortService.Load();

            if (!result.Success)
            {
                MessageBox.Show(
                    result.Message,
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return;
            }

            var configuration = result.Data;

            for (int i = 0; i < configuration.SerialPorts.Count; i++)
            {
                LoadPortToControls(
                    configuration.SerialPorts[i],
                    _portBoxes[i],
                    _baudRateBoxes[i],
                    _dataBitsBoxes[i],
                    _parityBoxes[i],
                    _stopBitsBoxes[i],
                    _handshakeBoxes[i]);
            }
        }
        //заполнение атоматически всех портов
        private void LoadPortToControls(
    SerialPortSettings settings,
    ComboBox portBox,
    ComboBox baudRateBox,
    ComboBox dataBitsBox,
    ComboBox parityBox,
    ComboBox stopBitsBox,
    ComboBox handshakeBox)
        {
            if (string.IsNullOrWhiteSpace(settings.PortName))
            {
                portBox.SelectedItem = null;
            }
            else
            {
                portBox.SelectedItem = settings.PortName;
            }

            baudRateBox.SelectedItem = settings.BaudRate;

            dataBitsBox.SelectedItem = settings.DataBits;

            parityBox.SelectedItem = settings.Parity.ToString();

            stopBitsBox.SelectedItem = settings.StopBits.ToString();

            handshakeBox.SelectedItem = settings.Handshake.ToString();
        }
        // метод чтения одного порта
        private SerialPortSettings ReadPortFromControls(
    ComboBox portBox,
    ComboBox baudRateBox,
    ComboBox dataBitsBox,
    ComboBox parityBox,
    ComboBox stopBitsBox,
    ComboBox handshakeBox,
    int id)
        {
            return new SerialPortSettings
            {
                Id = id,
                PortName = portBox.Text,
                BaudRate = int.Parse(baudRateBox.Text),
                DataBits = int.Parse(dataBitsBox.Text),
                Parity = Enum.Parse<Parity>(parityBox.Text),
                StopBits = Enum.Parse<StopBits>(stopBitsBox.Text),
                Handshake = Enum.Parse<Handshake>(handshakeBox.Text)
            };
        }
        //теперь общий метод для сохранения
        private void SaveSettings()
        {
            var configuration = new SerialPortConfiguration();

            for (int i = 0; i < _portBoxes.Count; i++)
            {
                configuration.SerialPorts.Add(
                    ReadPortFromControls(
                        _portBoxes[i],
                        _baudRateBoxes[i],
                        _dataBitsBoxes[i],
                        _parityBoxes[i],
                        _stopBitsBoxes[i],
                        _handshakeBoxes[i],
                        i + 1));
            }

            var result = _comPortService.Save(configuration);

            if (!result.Success)
            {
                MessageBox.Show(
                    result.Message,
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
        //метод исключения ком портов
        private void SubscribePortEvents()
        {
            foreach (var comboBox in _portBoxes)
            {
                comboBox.SelectedIndexChanged += Port_SelectedIndexChanged;
            }
        }
        //обработчик
        private void Port_SelectedIndexChanged(object? sender, EventArgs e)
        {
            UpdateAvailablePorts();
        }
        //сам метод сброса компортов в форме
        private void UpdateAvailablePorts()
        {
            if (_updatingPorts)
                return;

            _updatingPorts = true;

            try
            {
                var allPorts = _comPortService.GetAvailablePorts().ToList();

                var selectedPorts = _portBoxes
                    .Where(cb => cb.SelectedItem != null)
                    .Select(cb => cb.SelectedItem!.ToString()!)
                    .ToList();

                foreach (var comboBox in _portBoxes)
                {
                    string? currentPort = comboBox.SelectedItem?.ToString();

                    comboBox.Items.Clear();

                    foreach (var port in allPorts)
                    {
                        if (!selectedPorts.Contains(port) || port == currentPort)
                        {
                            comboBox.Items.Add(port);
                        }
                    }

                    if (currentPort != null)
                    {
                        comboBox.SelectedItem = currentPort;
                    }
                }
            }
            finally
            {
                _updatingPorts = false;
            }
        }
        // кнопка сохранения заданных настроек
        private void btnSave_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < _portBoxes.Count; i++)
            {
                if (_portBoxes[i].SelectedItem == null ||
                    _baudRateBoxes[i].SelectedItem == null ||
                    _dataBitsBoxes[i].SelectedItem == null ||
                    _parityBoxes[i].SelectedItem == null ||
                    _stopBitsBoxes[i].SelectedItem == null ||
                    _handshakeBoxes[i].SelectedItem == null)
                {
                    MessageBox.Show(
                        $"Не заполнены настройки для COM-порта №{i + 1}.",
                        "Ошибка",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }
            }
            SaveSettings();

            MessageBox.Show(
                "Настройки успешно сохранены.",
                "COM-порты",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        // кнопка востановления дефолтных настроек
        private void btnRestoreDefaults_Click(object sender, EventArgs e)
        {
            var dialogResult = MessageBox.Show(
                "Восстановить настройки по умолчанию?",
                "Подтверждение",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (dialogResult != DialogResult.Yes)
                return;

            var result = _comPortService.RestoreDefaults();

            if (!result.Success)
            {
                MessageBox.Show(
                    result.Message,
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return;
            }

            var configuration = result.Data;

            for (int i = 0; i < configuration.SerialPorts.Count; i++)
            {
                LoadPortToControls(
                    configuration.SerialPorts[i],
                    _portBoxes[i],
                    _baudRateBoxes[i],
                    _dataBitsBoxes[i],
                    _parityBoxes[i],
                    _stopBitsBoxes[i],
                    _handshakeBoxes[i]);
            }

            UpdateAvailablePorts();

            MessageBox.Show(
                "Настройки по умолчанию загружены. Для применения нажмите «Сохранить».",
                "COM-порты",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
    }
}

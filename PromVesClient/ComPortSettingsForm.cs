using DocumentFormat.OpenXml.Vml.Spreadsheet;
using PromVesClient.Models;
using PromVesClient.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Forms;


namespace PromVesClient
{
    public partial class ComPortSettingsForm : Form
    {
        private readonly ComPortService _comPortService;
        // для работы с MOXA
        private readonly ModbusTcpService _modbusTcpService;
        // для работы с табло
        private readonly SerialPortBoardService _serialPortBoardService;
        // для работы с табло
        private readonly GeneralConfiguratorService _generalConfiguratorService;
        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            WriteIndented = true,
            Converters =
    {
        new JsonStringEnumConverter()
    }
        };
        //для портов, чтобы не повторялись
        private bool _updatingPorts;
        private bool _updatingDeviceAddresses;
        // количество используемых COM-портов
        private int _selectedPortCount = 4;

        // коллекции для компртов
        private List<ComboBox> _portBoxes;
        private List<ComboBox> _baudRateBoxes;
        private List<ComboBox> _dataBitsBoxes;
        private List<ComboBox> _parityBoxes;
        private List<ComboBox> _stopBitsBoxes;
        private List<ComboBox> _handshakeBoxes;
        private List<ComboBox> _deviceAddressBoxes;

        // коллекции для MOXA
        private List<TextBox> _moxaIpBoxes;
        private List<TextBox> _moxaPortBoxes;
        private List<ComboBox> _moxaSlaveIdBoxes;
        private List<GroupBox> _moxaGroupBoxes;
        public ComPortSettingsForm(ComPortService comPortService, ModbusTcpService modbusTcpService, SerialPortBoardService serialPortBoardService, GeneralConfiguratorService generalConfiguratorService)
        {
            InitializeComponent();

            _comPortService = comPortService;
            _modbusTcpService = modbusTcpService;
            _serialPortBoardService = serialPortBoardService;
            _generalConfiguratorService = generalConfiguratorService;
            //иницилизация настроек компрта
            InitializeCollections();
            //инициализация настроек MOXA
            InitializeMoxaCollections();
            //подгрузка данных для компртов
            SubscribePortEvents();
            //подгрузка данных для MOXA
            InitializeMoxaControls();
            //инициализация настроек табла
            InitializeBoardSettings();
            // инициализация выбора количества COM-портов
            InitializePortCountComboBox();
            //подписка на события выбора адреса устройства
            SubscribePortEvents();
        }
        // обработчик события загрузки формы
        private async void ComPortSettingsForm_Load(object sender, EventArgs e)
        {
            FillComboBox();
            LoadSettings();
            UpdatePortVisibility();
            UpdateDeviceAddressAvailability();
            LoadMoxaSettings();
            LoadBoardSettings();
            UpdateBoardVisibility();
            FillBoardComboBox();
        }
        // метод заполнения всех комбобоксов
        private async void FillComboBox()
        {
            FillPorts();
            FillBaudRates();
            FillDataBits();
            FillParity();
            FillStopBits();
            FillHandshake();
        }
        // обьединяем в колекции комбо боксы
        private async void InitializeCollections()
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
            _deviceAddressBoxes = new List<ComboBox>
            {
                cbAddress1,
                cbAddress2,
                cbAddress3,
                cbAddress4
            };
            foreach (var comboBox in _deviceAddressBoxes)
            {
                comboBox.DropDownStyle = ComboBoxStyle.DropDownList;

                for (int i = 1; i <= 247; i++)
                {
                    comboBox.Items.Add(i);
                }

                comboBox.SelectedIndexChanged += DeviceAddress_SelectedIndexChanged;
            }
        }
        // метод универсального заполнения
        private async void FillComboBoxes<T>(
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
        private async void FillPorts()
        {
            FillComboBoxes(
                _portBoxes,
                SerialPort.GetPortNames());
        }
        //метод заполнения BaudRates
        private async void FillBaudRates()
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
        private async void FillDataBits()
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
        private async void FillParity()
        {
            FillComboBoxes(
                _parityBoxes,
                Enum.GetNames<Parity>());
        }
        private async void FillStopBits()
        {
            FillComboBoxes(
                _stopBitsBoxes,
                Enum.GetNames<StopBits>());
        }
        private async void FillHandshake()
        {
            FillComboBoxes(
                _handshakeBoxes,
                Enum.GetNames<Handshake>());
        }
        // заполнение одного порта
        private async void LoadSettings()
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
            // Определяем количество COM-портов из ConfigPort.json
            _selectedPortCount = Math.Clamp(
                configuration.SerialPorts.Count,
                1,
                4);

            // Показываем это количество в ComboBox
            cbChoicePort.SelectedItem = _selectedPortCount;

            // Загружаем настройки портов
            int portsToLoad = Math.Min(
                configuration.SerialPorts.Count,
                _portBoxes.Count);
            for (int i = 0; i < configuration.SerialPorts.Count; i++)
            {
                LoadPortToControls(
                    configuration.SerialPorts[i],
                    _portBoxes[i],
                    _baudRateBoxes[i],
                    _dataBitsBoxes[i],
                    _parityBoxes[i],
                    _stopBitsBoxes[i],
                    _handshakeBoxes[i],
                    _deviceAddressBoxes[i]);
            }
        }
        //заполнение атоматически всех портов
        private async void LoadPortToControls(
    SerialPortSettings settings,
    ComboBox portBox,
    ComboBox baudRateBox,
    ComboBox dataBitsBox,
    ComboBox parityBox,
    ComboBox stopBitsBox,
    ComboBox handshakeBox,
    ComboBox deviceAddressBox)
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
            deviceAddressBox.Text = settings.DeviceAddress.ToString();
        }
        // метод чтения одного порта
        private SerialPortSettings ReadPortFromControls(
    ComboBox portBox,
    ComboBox baudRateBox,
    ComboBox dataBitsBox,
    ComboBox parityBox,
    ComboBox stopBitsBox,
    ComboBox handshakeBox,
    ComboBox deviceAddressBox,
    int id)
        {
            if (!int.TryParse(deviceAddressBox.Text, out int deviceAddress))
            {
                throw new Exception("Адрес устройства должен быть числом.");
            }

            if (deviceAddress < 1 || deviceAddress > 247)
            {
                throw new Exception("Адрес устройства должен быть от 1 до 247.");
            }
            return new SerialPortSettings
            {
                Id = id,
                PortName = portBox.Text,
                BaudRate = int.Parse(baudRateBox.Text),
                DataBits = int.Parse(dataBitsBox.Text),
                Parity = Enum.Parse<Parity>(parityBox.Text),
                StopBits = Enum.Parse<StopBits>(stopBitsBox.Text),
                Handshake = Enum.Parse<Handshake>(handshakeBox.Text),
                DeviceAddress = deviceAddress
            };
        }
        //теперь общий метод для сохранения
        private async void SaveSettings()
        {
            var configuration = new SerialPortConfiguration();

            for (int i = 0; i < _selectedPortCount; i++)
            {
                configuration.SerialPorts.Add(
                    ReadPortFromControls(
                        _portBoxes[i],
                        _baudRateBoxes[i],
                        _dataBitsBoxes[i],
                        _parityBoxes[i],
                        _stopBitsBoxes[i],
                        _handshakeBoxes[i],
                        _deviceAddressBoxes[i],
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
        private async void SubscribePortEvents()
        {
            foreach (var comboBox in _portBoxes)
            {
                comboBox.SelectedIndexChanged += Port_SelectedIndexChanged;
            }
        }
        //обработчик
        private async void Port_SelectedIndexChanged(object? sender, EventArgs e)
        {
            UpdateAvailablePorts();
        }
        //сам метод сброса компортов в форме
        private async void UpdateAvailablePorts()
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
        private async void btnSave_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < _selectedPortCount; i++)
            {
                if (_portBoxes[i].SelectedItem == null ||
                    _baudRateBoxes[i].SelectedItem == null ||
                    _dataBitsBoxes[i].SelectedItem == null ||
                    _parityBoxes[i].SelectedItem == null ||
                    _stopBitsBoxes[i].SelectedItem == null ||
                    _handshakeBoxes[i].SelectedItem == null ||
                    string.IsNullOrWhiteSpace(_deviceAddressBoxes[i].Text))
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
        private async void btnRestoreDefaults_Click(object sender, EventArgs e)
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

            _selectedPortCount = Math.Clamp(
        configuration.SerialPorts.Count,
        1,
        4);

            cbChoicePort.SelectedItem = _selectedPortCount;
            int portsToLoad = Math.Min(
        configuration.SerialPorts.Count,
        _portBoxes.Count);


            for (int i = 0; i < configuration.SerialPorts.Count; i++)
            {
                LoadPortToControls(
                    configuration.SerialPorts[i],
                    _portBoxes[i],
                    _baudRateBoxes[i],
                    _dataBitsBoxes[i],
                    _parityBoxes[i],
                    _stopBitsBoxes[i],
                    _handshakeBoxes[i],
                    _deviceAddressBoxes[i]);
            }

            UpdateAvailablePorts();
            UpdatePortVisibility();

            MessageBox.Show(
                "Настройки по умолчанию загружены. Для применения нажмите «Сохранить».",
                "COM-порты",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        // метод инициализации выбора количества COM-портов
        private async void InitializePortCountComboBox()
        {
            cbChoicePort.Items.Clear();

            cbChoicePort.Items.Add(1);
            cbChoicePort.Items.Add(2);
            cbChoicePort.Items.Add(3);
            cbChoicePort.Items.Add(4);

            cbChoicePort.DropDownStyle = ComboBoxStyle.DropDownList;

            cbChoicePort.SelectedItem = _selectedPortCount;

            cbChoicePort.SelectedIndexChanged += CbChoicePort_SelectedIndexChanged;
        }
        // обработчик события изменения выбранного количества COM-портов
        private async void CbChoicePort_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (cbChoicePort.SelectedItem == null)
                return;

            _selectedPortCount = (int)cbChoicePort.SelectedItem;

            UpdatePortVisibility();
            UpdateMoxaVisibility();
        }
        // метод обновления видимости групповых элементов для COM-портов
        private async void UpdatePortVisibility()
        {
            groupBox1.Visible = _selectedPortCount >= 1;
            groupBox2.Visible = _selectedPortCount >= 2;
            groupBox3.Visible = _selectedPortCount >= 3;
            groupBox4.Visible = _selectedPortCount >= 4;
        }
        // обработчик события изменения выбранного адреса устройства
        private async void DeviceAddress_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_updatingDeviceAddresses)
                return;
            UpdateDeviceAddressAvailability();
        }
        // метод обновления доступности адресов устройств в ComboBox
        private async void UpdateDeviceAddressAvailability()
        {
            if (_updatingDeviceAddresses)
                return;

            try
            {
                _updatingDeviceAddresses = true;

                var selectedAddresses = new HashSet<int>();

                // Сначала собираем уже выбранные адреса
                foreach (var comboBox in _deviceAddressBoxes)
                {
                    if (int.TryParse(comboBox.Text, out int address))
                    {
                        selectedAddresses.Add(address);
                    }
                }

                // Убираем выбранные адреса из остальных ComboBox
                foreach (var comboBox in _deviceAddressBoxes)
                {
                    int? currentValue = null;

                    if (int.TryParse(comboBox.Text, out int currentAddress))
                    {
                        currentValue = currentAddress;
                    }

                    comboBox.Items.Clear();

                    for (int address = 1; address <= 32; address++)
                    {
                        // Текущий выбранный адрес оставляем
                        // Остальные выбранные в других COM-портах убираем
                        if (!selectedAddresses.Contains(address) ||
                            currentValue == address)
                        {
                            comboBox.Items.Add(address);
                        }
                    }

                    if (currentValue.HasValue)
                    {
                        comboBox.SelectedItem = currentValue.Value;
                    }
                }
            }
            finally
            {
                _updatingDeviceAddresses = false;
            }
        }
        // метод инициализации коллекций для MOXA
        private async void InitializeMoxaCollections()
        {
            _moxaIpBoxes = new List<TextBox>
    {
        IPtb1,
        IPtb2,
        IPtb3,
        IPtb4
    };

            _moxaPortBoxes = new List<TextBox>
    {
        Port1,
        Port2,
        Port3,
        Port4
    };

            _moxaSlaveIdBoxes = new List<ComboBox>
    {
        cbAddress5,
        cbAddress6,
        cbAddress7,
        cbAddress8
    };

            _moxaGroupBoxes = new List<GroupBox>
    {
        groupBox5,
        groupBox6,
        groupBox7,
        groupBox8
    };
            foreach (var textBox in _moxaIpBoxes)
            {
                textBox.KeyPress += MoxaIpTextBox_KeyPress;
            }
        }
        // метод инициализации контролов для MOXA
        private void InitializeMoxaControls()
        {
            foreach (var comboBox in _moxaSlaveIdBoxes)
            {
                comboBox.Items.Clear();
                comboBox.DropDownStyle = ComboBoxStyle.DropDownList;

                for (int i = 1; i <= 32; i++)
                {
                    comboBox.Items.Add(i);
                }
            }
        }
        // метод обновления видимости групповых элементов для MOXA
        private void UpdateMoxaVisibility()
        {
            if (_moxaGroupBoxes == null)
                return;

            for (int i = 0; i < _moxaGroupBoxes.Count; i++)
            {
                _moxaGroupBoxes[i].Visible = i < _selectedPortCount;
            }
        }
        // метод загрузки настроек MOXA
        private async void LoadMoxaSettings()
        {
            var result = _modbusTcpService.Load();

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

            int count = Math.Min(
                configuration.ModbusTcpSetting.Count,
                _moxaIpBoxes.Count);

            for (int i = 0; i < count; i++)
            {
                var setting = configuration.ModbusTcpSetting[i];

                _moxaIpBoxes[i].Text = setting.NportIp;
                _moxaPortBoxes[i].Text = setting.NportPort.ToString();

                _moxaSlaveIdBoxes[i].SelectedItem = setting.SlaveId;
            }
        }
        // метод сохранения настроек MOXA
        private void SaveMoxaSettings()
        {
            var configuration = new ModbusTcpConfiguration();

            for (int i = 0; i < _selectedPortCount; i++)
            {
                string ipAddress = _moxaIpBoxes[i].Text.Trim();

                if (string.IsNullOrWhiteSpace(ipAddress))
                {
                    throw new Exception(
                        $"Не указан IP-адрес MOXA {i + 1}.");
                }

                if (!IsValidIpAddress(ipAddress))
                {
                    throw new Exception(
                        $"Некорректный IP-адрес MOXA {i + 1}: {ipAddress}");
                }
                if (!int.TryParse(
                        _moxaPortBoxes[i].Text,
                        out int port))
                {
                    throw new Exception(
                        $"Некорректный порт подключения MOXA {i + 1}.");
                }

                if (!int.TryParse(
                        _moxaSlaveIdBoxes[i].Text,
                        out int slaveId))
                {
                    throw new Exception(
                        $"Не выбран адрес устройства MOXA {i + 1}.");
                }

                configuration.ModbusTcpSetting.Add(
                    new ModbusTcpSetting
                    {
                        Id = i + 1,
                        NportIp = _moxaIpBoxes[i].Text.Trim(),
                        NportPort = port,
                        SlaveId = slaveId
                    });
            }

            var result = _modbusTcpService.Save(configuration);

            if (!result.Success)
            {
                throw new Exception(result.Message);
            }
        }
        // обработчик события нажатия кнопки сохранения настроек MOXA
        private void SaveTcpbtn_Click(object sender, EventArgs e)
        {
            try
            {
                SaveMoxaSettings();

                MessageBox.Show(
                    "Настройки успешно сохранены.",
                    "Успешно",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
        // зпратение метода проверки корректности IP-адреса
        private async void MoxaIpTextBox_KeyPress(object? sender, KeyPressEventArgs e)
        {
            // Разрешаем цифры, точку и управляющие символы (Backspace и т.п.)
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }
        }
        // метод проверки корректности IP-адреса
        private static bool IsValidIpAddress(string ip)
        {
            return IPAddress.TryParse(ip, out _);
        }
        // метод инициализации контролов для табло
        private async void InitializeBoardSettings()
        {
            Boardcb.Items.Clear();

            Boardcb.Items.Add("GreenBoard");
            Boardcb.Items.Add("YHLBoard");
            Boardcb.Items.Add("None");

            Boardcb.DropDownStyle = ComboBoxStyle.DropDownList;


            protocolcb.Items.Clear();

            protocolcb.Items.Add("ModbusTcp");
            protocolcb.Items.Add("ModbusRtu");
            protocolcb.Items.Add("St");

            protocolcb.DropDownStyle = ComboBoxStyle.DropDownList;
            Boardcb.SelectedIndexChanged += Boardcb_SelectedIndexChanged;


            cbPort5.DropDownStyle = ComboBoxStyle.DropDownList;

            cbBaudRate5.DropDownStyle = ComboBoxStyle.DropDownList;

            cbDataBits5.DropDownStyle = ComboBoxStyle.DropDownList;

            cbHandshake5.DropDownStyle = ComboBoxStyle.DropDownList;

            cbParity5.DropDownStyle = ComboBoxStyle.DropDownList;

            cbStopBits5.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        // метод сохранения настроек COM-порта для табло
        private async Task SaveBoardSerialPortSettings()
        {
            if (string.IsNullOrWhiteSpace(cbPort5.Text))
                throw new Exception("Не выбран COM-порт табло.");

            if (!int.TryParse(cbBaudRate5.Text, out int baudRate))
                throw new Exception("Некорректный BaudRate.");

            if (!int.TryParse(cbDataBits5.Text, out int dataBits))
                throw new Exception("Некорректный DataBits.");

            if (!Enum.TryParse<Parity>(cbParity5.Text, out Parity parity))
                throw new Exception("Некорректный Parity.");

            if (!Enum.TryParse<StopBits>(cbStopBits5.Text, out StopBits stopBits))
                throw new Exception("Некорректный StopBits.");

            if (!Enum.TryParse<Handshake>(cbHandshake5.Text, out Handshake handshake))
                throw new Exception("Некорректный Handshake.");

            var configuration = new SerialPortBoardConfiguration();
            configuration.SerialPortsBoards.Add(new SerialPortBoard
            {
                Id = 1,
                PortName = cbPort5.Text,
                BaudRate = baudRate,
                DataBits = dataBits,
                Parity = parity,
                StopBits = stopBits,
                Handshake = handshake
            });

            // Если сервис имеет асинхронный метод SaveAsync — используйте его.
            // Иначе обёрните синхронный вызов в Task.Run, чтобы не блокировать UI.
            var result = await Task.Run(() => _serialPortBoardService.Save(configuration));

            if (!result.Success)
                throw new Exception(result.Message);
        }
        private async void SaveGeneralConfigurator()
        {
            if (string.IsNullOrWhiteSpace(Boardcb.Text))
                throw new Exception("Не выбрано табло.");

            if (string.IsNullOrWhiteSpace(protocolcb.Text))
                throw new Exception("Не выбран протокол.");

            var configuration = new GeneralConfiguratorConfiguration
            {
                GeneralConfigurator = new GeneralConfigurator
                {
                    Protocol = protocolcb.Text,
                    Board = Boardcb.Text
                }
            };

            var result = _generalConfiguratorService.Save(configuration);

            if (!result.Success)
                throw new Exception(result.Message);
        }
        // обработчик события нажатия кнопки сохранения настроек табло
        private async void SaveTablebtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (Boardcb.Text == "None")
                {
                    // COM-настройки табло не сохраняем
                }
                else
                {
                    await SaveBoardSerialPortSettings();
                }
                SaveGeneralConfigurator();

                MessageBox.Show(
                    "Настройки успешно сохранены.",
                    "Успешно",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
        // метод загрузки настроек COM-порта для табло
        private void LoadBoardSettings()
        {
            var serialResult = _serialPortBoardService.Load();

            if (serialResult.Success &&
                serialResult.Data.SerialPortsBoards.Count > 0)
            {
                var settings =
                    serialResult.Data.SerialPortsBoards[0];

                cbPort5.SelectedItem = settings.PortName;
                cbBaudRate5.SelectedItem = settings.BaudRate;
                cbDataBits5.SelectedItem = settings.DataBits;
                cbParity5.SelectedItem = settings.Parity.ToString();
                cbStopBits5.SelectedItem = settings.StopBits.ToString();
                cbHandshake5.SelectedItem = settings.Handshake.ToString();


                var generalResult = _generalConfiguratorService.Load();

                if (generalResult.Success)
                {
                    protocolcb.Text =
                        generalResult.Data.GeneralConfigurator.Protocol;

                    Boardcb.Text =
                        generalResult.Data.GeneralConfigurator.Board;
                }
            }
        }
        // метод заполнения комбобоксов для табло
        private async void FillBoardComboBox()
        {
            FillBoardPort();
            FillBoardBaudRate();
            FillBoardDataBits();
            FillBoardParity();
            FillBoardStopBits();
            FillBoardHandshake();
        }
        private async void FillBoardPort()
        {
            FillComboBoxes(
                new[] { cbPort5 },
                SerialPort.GetPortNames());
        }

        private async void FillBoardBaudRate()
        {
            FillComboBoxes(
                new[] { cbBaudRate5 },
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

        private async void FillBoardDataBits()
        {
            FillComboBoxes(
                new[] { cbDataBits5 },
                new[]
                {
            5,
            6,
            7,
            8
                });
        }

        private async void FillBoardParity()
        {
            FillComboBoxes(
                new[] { cbParity5 },
                Enum.GetNames<Parity>());
        }

        private async void FillBoardStopBits()
        {
            FillComboBoxes(
                new[] { cbStopBits5 },
                Enum.GetNames<StopBits>());
        }

        private async void FillBoardHandshake()
        {
            FillComboBoxes(
                new[] { cbHandshake5 },
                Enum.GetNames<Handshake>());
        }
        private void Boardcb_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool isNone = Boardcb.Text == "None";

            groupBox9.Visible = !isNone;
            defaultboardbtn.Visible = !isNone;
        }
        private async void UpdateBoardVisibility()
        {
            groupBox9.Visible = Boardcb.Text != "None";
        }
        private async void FillBoardDefaults()
        {
            // COM-порт
            if (cbPort5.Items.Count > 0)
            {
                cbPort5.SelectedIndex = 0;
            }
            else
            {
                cbPort5.SelectedItem = null;
            }

            // BaudRate
            cbBaudRate5.SelectedItem = 1200;

            // DataBits
            cbDataBits5.SelectedItem = 8;

            // Parity
            cbParity5.SelectedItem = Parity.None.ToString();

            // StopBits
            cbStopBits5.SelectedItem = StopBits.One.ToString();

            // Handshake
            cbHandshake5.SelectedItem = Handshake.None.ToString();
        }
        // обработчик события нажатия кнопки восстановления настроек табло по умолчанию
        private async void defaultboardbtn_Click(object sender, EventArgs e)
        {
            FillBoardDefaults();
        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }
    }
}
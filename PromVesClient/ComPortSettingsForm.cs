using PromVesClient.Models;
using PromVesClient.Service;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ServiceProcess;
using System.Threading.Tasks;

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
        // События выбора не должны вмешиваться в начальную загрузку настроек.
        private bool _initializing = true;
        private bool _updatingPorts;
        private string[] _availablePorts = Array.Empty<string>();
        // Сервис для перезапуска службы PromVesServer
        private readonly PromVesServerService _promVesServerService;

        // количество используемых COM-портов
        private int _selectedPortCount = 4;
        private int _selectedMoxaCount = 4;

        // Коллекции элементов для COM-портов
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

        public ComPortSettingsForm(
            ComPortService comPortService,
            ModbusTcpService modbusTcpService,
            SerialPortBoardService serialPortBoardService,
            GeneralConfiguratorService generalConfiguratorService,
            PromVesServerService promVesServerService)
        {
            InitializeComponent();

            _comPortService = comPortService;
            _modbusTcpService = modbusTcpService;
            _serialPortBoardService = serialPortBoardService;
            _generalConfiguratorService = generalConfiguratorService;
            _promVesServerService = promVesServerService;

            InitializeCollections();
            InitializeMoxaCollections();
            InitializeMoxaControls();
            InitializeBoardSettings();
            InitializePortCountComboBox();
            InitializePortCountComboBoxMoxa();
            SubscribePortEvents();
            SubscribeDefaultButtons();

            // Работает и при наличии, и при отсутствии этой подписки в дизайнере.
            Load -= ComPortSettingsForm_Load;
            Load += ComPortSettingsForm_Load;

            UpdatePortVisibility();
            UpdateMoxaVisibility();
            UpdateBoardVisibility();
        }

        // обработчик события загрузки формы
        private void ComPortSettingsForm_Load(object sender, EventArgs e)
        {
            _initializing = true;
            try
            {
                // Программно заполняются только списки имён COM-портов.
                RefreshAvailablePorts();
                LoadSettings();
                LoadMoxaSettings();
                LoadBoardSettings();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _initializing = false;
                UpdatePortVisibility();
                UpdateMoxaVisibility();
                UpdateBoardVisibility();
            }
        }

        // Коллекции элементов формы для COM-портов
        private void InitializeCollections()
        {
            _portBoxes = new List<ComboBox> { cbPort1, cbPort2, cbPort3, cbPort4 };
            _baudRateBoxes = new List<ComboBox> { cbBaudRate1, cbBaudRate2, cbBaudRate3, cbBaudRate4 };
            _dataBitsBoxes = new List<ComboBox> { cbDataBits1, cbDataBits2, cbDataBits3, cbDataBits4 };
            _parityBoxes = new List<ComboBox> { cbParity1, cbParity2, cbParity3, cbParity4 };
            _stopBitsBoxes = new List<ComboBox> { cbStopBits1, cbStopBits2, cbStopBits3, cbStopBits4 };
            _handshakeBoxes = new List<ComboBox> { cbHandshake1, cbHandshake2, cbHandshake3, cbHandshake4 };
            _deviceAddressBoxes = new List<ComboBox> { cbAddress1, cbAddress2, cbAddress3, cbAddress4 };

            // Настройка поведения элементов без изменения их списков.
            foreach (var boxes in new[]
            {
                _portBoxes, _baudRateBoxes, _dataBitsBoxes, _parityBoxes,
                _stopBitsBoxes, _handshakeBoxes, _deviceAddressBoxes
            })
            {
                foreach (var comboBox in boxes)
                    comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            }
        }

        // Выбирает существующий пункт, не добавляя и не удаляя Items.
        // Пробелы по краям и регистр не влияют на выбор.
        private static bool SelectComboBoxItem(
            ComboBox comboBox, object? value, ICollection<string>? missingItems = null)
        {
            string text = value?.ToString()?.Trim() ?? string.Empty;
            int selectedIndex = -1;

            if (text.Length > 0)
            {
                for (int i = 0; i < comboBox.Items.Count; i++)
                {
                    string itemText = comboBox.GetItemText(comboBox.Items[i]).Trim();
                    if (string.Equals(itemText, text, StringComparison.OrdinalIgnoreCase))
                    {
                        selectedIndex = i;
                        break;
                    }
                }

                // Также поддерживаем числовые коды enum в дизайнере:
                // например, StopBits.One соответствует коду 1.
                if (selectedIndex < 0 && value is Enum enumValue)
                {
                    long code = Convert.ToInt64(enumValue);
                    for (int i = 0; i < comboBox.Items.Count; i++)
                    {
                        if (long.TryParse(comboBox.GetItemText(comboBox.Items[i]), out long itemCode)
                            && itemCode == code)
                        {
                            selectedIndex = i;
                            break;
                        }
                    }
                }
            }

            comboBox.SelectedIndex = selectedIndex;
            if (selectedIndex < 0)
            {
                comboBox.Text = string.Empty;
                if (text.Length > 0)
                    missingItems?.Add($"{comboBox.Name}: отсутствует значение «{text}».");
            }

            return selectedIndex >= 0;
        }

        // Загрузка сохранённых настроек COM-портов
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

            ApplyPortSettings(result.Data);
        }

        // При загрузке и восстановлении сначала возвращаем полные списки портов.
        // Фильтрацию включаем только после применения всех сохранённых значений.
        private void ApplyPortSettings(
     SerialPortConfiguration configuration,
     ICollection<string>? missingItems = null)
        {
            if (configuration?.SerialPorts == null)
                throw new InvalidOperationException(
                    "Не получена конфигурация COM-портов.");

            _updatingPorts = true;

            try
            {
                // Количество COM-портов берём ТОЛЬКО из ConfigPort.json
                _selectedPortCount = Math.Clamp(
                    configuration.SerialPorts.Count,
                    1,
                    32);

                SelectComboBoxItem(
                    cbChoicePort,
                    _selectedPortCount,
                    missingItems);

                // Заполняем ComboBox'ы доступными COM-портами
                foreach (var comboBox in _portBoxes)
                {
                    SetPortItems(
                        comboBox,
                        _availablePorts,
                        comboBox.Text);
                }

                // Загружаем настройки каждого COM-порта
                int count = Math.Min(
                    configuration.SerialPorts.Count,
                    _portBoxes.Count);

                for (int i = 0; i < count; i++)
                {
                    LoadPortToControls(
                        configuration.SerialPorts[i],
                        _portBoxes[i],
                        _baudRateBoxes[i],
                        _dataBitsBoxes[i],
                        _parityBoxes[i],
                        _stopBitsBoxes[i],
                        _handshakeBoxes[i],
                        _deviceAddressBoxes[i],
                        missingItems);
                }
            }
            finally
            {
                _updatingPorts = false;
            }

            UpdateAvailablePorts();
            UpdatePortVisibility();
        }

        // Выбор сохранённых значений без изменения списков
        private void LoadPortToControls(
            SerialPortSettings settings,
            ComboBox portBox,
            ComboBox baudRateBox,
            ComboBox dataBitsBox,
            ComboBox parityBox,
            ComboBox stopBitsBox,
            ComboBox handshakeBox,
            ComboBox deviceAddressBox,
            ICollection<string>? missingItems = null)
        {
            SelectComboBoxItem(portBox, settings.PortName, missingItems);
            SelectComboBoxItem(baudRateBox, settings.BaudRate, missingItems);
            SelectComboBoxItem(dataBitsBox, settings.DataBits, missingItems);
            SelectComboBoxItem(parityBox, settings.Parity, missingItems);
            SelectComboBoxItem(stopBitsBox, settings.StopBits, missingItems);
            SelectComboBoxItem(handshakeBox, settings.Handshake, missingItems);
            SelectComboBoxItem(deviceAddressBox, settings.DeviceAddress, missingItems);
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
                Parity = Enum.Parse<Parity>(parityBox.Text.Trim(), true),
                StopBits = Enum.Parse<StopBits>(stopBitsBox.Text.Trim(), true),
                Handshake = Enum.Parse<Handshake>(handshakeBox.Text.Trim(), true),
                DeviceAddress = deviceAddress
            };
        }

        //теперь общий метод для сохранения
        private void SaveSettings()
        {
            var configuration = new SerialPortConfiguration();
            var selectedPorts = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var selectedAddresses = new HashSet<int>();

            for (int i = 0; i < _selectedPortCount; i++)
            {
                var settings = ReadPortFromControls(
                    _portBoxes[i],
                    _baudRateBoxes[i],
                    _dataBitsBoxes[i],
                    _parityBoxes[i],
                    _stopBitsBoxes[i],
                    _handshakeBoxes[i],
                    _deviceAddressBoxes[i],
                    i + 1);

                // Дополнительная проверка, в том числе для загруженной конфигурации.
                if (!selectedPorts.Add(settings.PortName))
                    throw new Exception($"COM-порт {settings.PortName} выбран несколько раз.");

                if (!selectedAddresses.Add(settings.DeviceAddress))
                    throw new Exception($"Адрес устройства {settings.DeviceAddress} выбран несколько раз.");

                configuration.SerialPorts.Add(settings);
            }

            var result = _comPortService.Save(configuration);

            if (!result.Success)
                throw new Exception(result.Message);
        }

        //обработчик
        private void Port_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (_initializing || _updatingPorts)
                return;

            UpdateAvailablePorts();
        }

        private void SubscribePortEvents()
        {
            foreach (var comboBox in _portBoxes)
            {
                comboBox.SelectedIndexChanged -= Port_SelectedIndexChanged;
                comboBox.SelectedIndexChanged += Port_SelectedIndexChanged;
                comboBox.DropDown -= PortComboBox_DropDown;
                comboBox.DropDown += PortComboBox_DropDown;
            }

            // COM-порт табло, как в исходной форме, выбирается отдельно.
            cbPort5.DropDown -= PortComboBox_DropDown;
            cbPort5.DropDown += PortComboBox_DropDown;
        }

        // При раскрытии списка повторно проверяем доступные COM-порты.
        private void PortComboBox_DropDown(object? sender, EventArgs e)
        {
            if (_initializing || _updatingPorts)
                return;

            try
            {
                RefreshAvailablePorts();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RefreshAvailablePorts()
        {
            _availablePorts = _comPortService.GetAvailablePorts()
                .Where(port => !string.IsNullOrWhiteSpace(port))
                .Select(port => port.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();

            UpdateAvailablePorts();
        }

        // Выбранный порт остаётся в своём ComboBox и исключается из остальных.
        // После смены выбора освободившийся порт снова появляется в их списках.
        private void UpdateAvailablePorts()
        {
            if (_updatingPorts)
                return;

            _updatingPorts = true;
            try
            {
                // Снимок делается до очистки Items, иначе выбор может потеряться.
                var currentPorts = _portBoxes.Select(box => box.Text.Trim()).ToArray();
                var selectedPorts = new HashSet<string>(
                    currentPorts.Where(port => !string.IsNullOrWhiteSpace(port)),
                    StringComparer.OrdinalIgnoreCase);

                for (int i = 0; i < _portBoxes.Count; i++)
                {
                    string currentPort = currentPorts[i];
                    var ports = _availablePorts.Where(port =>
                        !selectedPorts.Contains(port) ||
                        string.Equals(port, currentPort, StringComparison.OrdinalIgnoreCase));

                    SetPortItems(_portBoxes[i], ports, currentPort);
                }

                // Табло не участвует во взаимном исключении cbPort1–cbPort4.
                SetPortItems(cbPort5, _availablePorts, cbPort5.Text);
            }
            finally
            {
                _updatingPorts = false;
            }
        }

        // Используется только для cbPort1–cbPort5. Остальные Items задаёт дизайнер.
        private static void SetPortItems(
            ComboBox comboBox, IEnumerable<string> ports, string? selectedPort)
        {
            comboBox.BeginUpdate();
            try
            {
                comboBox.Items.Clear();
                foreach (string port in ports)
                    comboBox.Items.Add(port);

                SelectComboBoxItem(comboBox, selectedPort);
            }
            finally
            {
                comboBox.EndUpdate();
            }
        }

        // кнопка сохранения заданных настроек
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // кнопка закрытия формы
        private void SubscribeDefaultButtons()
        {

            foreach (var button in Controls.Find("btnRestoreDefaults", true).OfType<Button>())
            {
                button.Click -= btnRestoreDefaults_Click;
                button.Click += btnRestoreDefaults_Click;
            }

            defaultboardbtn.Click -= defaultboardbtn_Click;
            defaultboardbtn.Click += defaultboardbtn_Click;
        }

        // Кнопка восстановления настроек по умолчанию
        private void btnRestoreDefaults_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(
                "Восстановить настройки по умолчанию?",
                "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                != DialogResult.Yes)
                return;

            try
            {
                var result = _comPortService.RestoreDefaults();
                if (!result.Success)
                    throw new InvalidOperationException(result.Message);

                if (result.Data?.SerialPorts == null || result.Data.SerialPorts.Count == 0)
                    throw new InvalidOperationException(
                        "Сервис RestoreDefaults() не вернул настройки COM-портов.");

                // Состав подключённых портов мог измениться после открытия формы.
                RefreshAvailablePorts();
                var missingItems = new List<string>();
                ApplyPortSettings(result.Data, missingItems);

                if (ShowMissingDefaults(missingItems, "COM-порты"))
                    return;

                MessageBox.Show(
                    "Настройки по умолчанию загружены. Для применения нажите «Сохранить».",
                    "COM-порты", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Вместо сообщения об успехе показываем, какие значения не удалось выбрать.
        private static bool ShowMissingDefaults(ICollection<string> missingItems, string caption)
        {
            if (missingItems.Count == 0)
                return false;

            MessageBox.Show(
                "Не все значения по умолчанию удалось выбрать:\n\n" +
                string.Join(Environment.NewLine, missingItems) +
                "\n\nДля обычных параметров проверьте Items соответствующего ComboBox " +
                "в дизайнере. Имена COM-портов берутся из списка доступных портов " +
                "компьютера. Отсутствующие значения не добавляются автоматически.",
                caption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return true;
        }

        // метод инициализации выбора количества COM-портов
        private void InitializePortCountComboBox()
        {
            cbChoicePort.DropDownStyle = ComboBoxStyle.DropDownList;
            cbChoicePort.SelectedIndexChanged -= CbChoicePort_SelectedIndexChanged;
            cbChoicePort.SelectedIndexChanged += CbChoicePort_SelectedIndexChanged;
            SelectComboBoxItem(cbChoicePort, _selectedPortCount);
        }

        // обработчик события изменения выбранного количества COM-портов
        private void CbChoicePort_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (_initializing || _updatingPorts)
                return;

            if (!int.TryParse(cbChoicePort.Text, out int portCount) ||
                portCount < 1 || portCount > 4)
                return;

            _selectedPortCount = portCount;
            UpdatePortVisibility();
        }

        // метод обновления видимости групповых элементов для COM-портов
        private void UpdatePortVisibility()
        {
            groupBox1.Visible = _selectedPortCount >= 1;
            groupBox2.Visible = _selectedPortCount >= 2;
            groupBox3.Visible = _selectedPortCount >= 3;
            groupBox4.Visible = _selectedPortCount >= 4;
        }

        // обработчик события изменения выбранного адреса устройства
        private void DeviceAddress_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Оставлен для совместимости с возможной привязкой в Designer.cs.
            // Списки адресов больше не перестраиваются при выборе.
        }

        // метод инициализации коллекций для MOXA
        private void InitializeMoxaCollections()
        {
            _moxaIpBoxes = new List<TextBox> { IPtb1, IPtb2, IPtb3, IPtb4 };
            _moxaPortBoxes = new List<TextBox> { Port1, Port2, Port3, Port4 };
            _moxaSlaveIdBoxes = new List<ComboBox> { cbAddress5, cbAddress6, cbAddress7, cbAddress8 };
            _moxaGroupBoxes = new List<GroupBox> { groupBox5, groupBox6, groupBox7, groupBox8 };

            foreach (var textBox in _moxaIpBoxes)
            {
                textBox.KeyPress -= MoxaIpTextBox_KeyPress;
                textBox.KeyPress += MoxaIpTextBox_KeyPress;
            }
        }

        // метод инициализации контролов для MOXA
        private void InitializeMoxaControls()
        {
            foreach (var comboBox in _moxaSlaveIdBoxes)
                comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        }
        // метод загрузки настроек MOXA
        private void LoadMoxaSettings()
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

            if (configuration?.ModbusTcpSetting == null)
                return;

            // Количество MOXA берём ТОЛЬКО из ConfigModbusTcp.json
            _selectedMoxaCount = Math.Clamp(
                configuration.ModbusTcpSetting.Count,
                1,
                _moxaGroupBoxes.Count);

            // Показываем это количество в ComboBox
            SelectComboBoxItem(
                cbChoiceMoxa,
                _selectedMoxaCount);

            // Загружаем значения MOXA
            int count = Math.Min(
                _selectedMoxaCount,
                configuration.ModbusTcpSetting.Count);

            for (int i = 0; i < count; i++)
            {
                var setting = configuration.ModbusTcpSetting[i];

                _moxaIpBoxes[i].Text = setting.NportIp;
                _moxaPortBoxes[i].Text = setting.NportPort.ToString();

                SelectComboBoxItem(
                    _moxaSlaveIdBoxes[i],
                    setting.SlaveId);
            }

            // Показываем нужное количество GroupBox
            UpdateMoxaVisibility();
        }

        // метод сохранения настроек MOXA
        private void SaveMoxaSettings()
        {
            var configuration = new ModbusTcpConfiguration();

            for (int i = 0; i < _selectedMoxaCount ; i++)
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

        // Ограничение ввода символов IP-адреса
        private void MoxaIpTextBox_KeyPress(object? sender, KeyPressEventArgs e)
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
        private void InitializeBoardSettings()
        {
            foreach (var comboBox in new[]
            {
                Boardcb, protocolcb, cbPort5, cbBaudRate5, cbDataBits5,
                cbHandshake5, cbParity5, cbStopBits5
            })
            {
                comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            }

            Boardcb.SelectedIndexChanged -= Boardcb_SelectedIndexChanged;
            Boardcb.SelectedIndexChanged += Boardcb_SelectedIndexChanged;
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

            if (!Enum.TryParse<Parity>(cbParity5.Text.Trim(), true, out Parity parity))
                throw new Exception("Некорректный Parity.");

            if (!Enum.TryParse<StopBits>(cbStopBits5.Text.Trim(), true, out StopBits stopBits))
                throw new Exception("Некорректный StopBits.");

            if (!Enum.TryParse<Handshake>(cbHandshake5.Text.Trim(), true, out Handshake handshake))
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

            // Сохраняем настройки табло без блокировки интерфейса.
            var result = await Task.Run(() => _serialPortBoardService.Save(configuration));

            if (!result.Success)
                throw new Exception(result.Message);
        }
        // метод сохранения настроек протокола и табло
        private void SaveGeneralConfigurator()
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

                SelectComboBoxItem(cbPort5, settings.PortName);
                SelectComboBoxItem(cbBaudRate5, settings.BaudRate);
                SelectComboBoxItem(cbDataBits5, settings.DataBits);
                SelectComboBoxItem(cbParity5, settings.Parity);
                SelectComboBoxItem(cbStopBits5, settings.StopBits);
                SelectComboBoxItem(cbHandshake5, settings.Handshake);

                var generalResult = _generalConfiguratorService.Load();

                if (generalResult.Success)
                {
                    SelectComboBoxItem(protocolcb, generalResult.Data.GeneralConfigurator.Protocol);

                    SelectComboBoxItem(Boardcb, generalResult.Data.GeneralConfigurator.Board);
                }
            }
        }

        private void Boardcb_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_initializing)
                return;

            UpdateBoardVisibility();
        }

        private void UpdateBoardVisibility()
        {
            bool showBoardSettings = Boardcb.Text != "None";
            groupBox9.Visible = showBoardSettings;
            defaultboardbtn.Visible = showBoardSettings;
        }

        private void SelectBoardDefaults(ICollection<string>? missingItems = null)
        {
            // Сначала выбираем параметры из списков, заданных в дизайнере.
            SelectComboBoxItem(cbBaudRate5, 1200, missingItems);
            SelectComboBoxItem(cbDataBits5, 8, missingItems);
            SelectComboBoxItem(cbParity5, Parity.None, missingItems);
            SelectComboBoxItem(cbStopBits5, StopBits.One, missingItems);
            SelectComboBoxItem(cbHandshake5, Handshake.None, missingItems);

            // Как в исходной форме: первый доступный COM-порт табло.
            cbPort5.SelectedIndex = cbPort5.Items.Count > 0 ? 0 : -1;
            if (cbPort5.SelectedIndex < 0)
                missingItems?.Add("cbPort5: доступные COM-порты не найдены.");
        }

        // обработчик события нажатия кнопки восстановления настроек табло по умолчанию
        private void defaultboardbtn_Click(object sender, EventArgs e)
        {
            try
            {
                RefreshAvailablePorts();
                var missingItems = new List<string>();
                SelectBoardDefaults(missingItems);
                ShowMissingDefaults(missingItems, "Табло");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnRestartServerCom_Click(object sender, EventArgs e)
        {
            await _promVesServerService.RestartServerAsync();
        }

        private async void btnRestartServerMoxa_Click(object sender, EventArgs e)
        {
            await _promVesServerService.RestartServerAsync();
        }

        private async void btnRestartServerGeneral_Click(object sender, EventArgs e)
        {
            await _promVesServerService.RestartServerAsync();
        }
        // метод инициализации выбора количества MOXA
        private void InitializePortCountComboBoxMoxa()
        {
            cbChoiceMoxa.DropDownStyle = ComboBoxStyle.DropDownList;

            cbChoiceMoxa.SelectedIndexChanged -= CbChoiceMoxa_SelectedIndexChanged;
            cbChoiceMoxa.SelectedIndexChanged += CbChoiceMoxa_SelectedIndexChanged;

            SelectComboBoxItem(cbChoiceMoxa, _selectedMoxaCount);
        }

        // обработчик события изменения выбранного количества MOXA
        private void CbChoiceMoxa_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (_initializing)
                return;

            if (!int.TryParse(cbChoiceMoxa.Text, out int moxaCount) ||
                moxaCount < 1 ||
                moxaCount > _moxaGroupBoxes.Count)
                return;

            _selectedMoxaCount = moxaCount;

            UpdateMoxaVisibility();
        }

        // метод обновления видимости групповых элементов для MOXA
        private void UpdateMoxaVisibility()
        {
            groupBox5.Visible = _selectedMoxaCount >= 1;
            groupBox6.Visible = _selectedMoxaCount >= 2;
            groupBox7.Visible = _selectedMoxaCount >= 3;
            groupBox8.Visible = _selectedMoxaCount >= 4;
        }
    }
}

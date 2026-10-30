using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PromVesClient.DTO;
using PromVesClient.Service;
using PromVesClient.Service.StaticWeighingService;
using PromVesClient.Service.TcpService;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text;
using System.Windows.Forms;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace PromVesClient
{
    public partial class StaticWeighing : Form
    {
        //DI
        private readonly CurrentUserService _currentUserService;
        //логи
        private readonly ILogger<StaticWeighing> _logger;

        private readonly StaticWeighingService _staticWeighingService;
        //обьект, который отвечает за подключение/отключение/получение данных сервера
        private readonly TcpService _tcpService;
        //отдельный канал команд камеры (тот же IP, порт 5005):
        //отправка StartCapture/EndCapture и приём данных сервера
        private readonly TcpService _commandTcpService;

        //private TcpClient? _client;
        //private NetworkStream? _stream;
        //private CancellationTokenSource? _cts;

        //коллекциями с ссылка на картинки 
        private List<PictureBox> pictureBoxesList;
        private ScottPlot.Plottables.Signal signal;
        //сохранение ссылок на обьекты графиков
        private List<ScottPlot.WinForms.FormsPlot> plots;
        //сохранение ссылок на обьекты надписей
        private List<Label> labelsList;
        //
        private List<ReferenceDataDto> ReferenceWagonList;
        //Предназначен для создания точек на графике
        private readonly Queue<decimal>[] values;

        //таймер предназначен для создания точек для 4 графиков
        private readonly System.Windows.Forms.Timer graphTimer = new();
        //переменная предназначенная для соханения данных веса с бортов
        private readonly decimal[] cartSideWeights;
        //изнчальные данные взвешивания
        private List<decimal> cartAxesWeightsList = new List<decimal> { 0, 0, 0, 0 };
        //переменная, которая сохраняет полученные значения для расчета стабильности
        private decimal[] stableWeight = new decimal[100];
        //Полный набор значений, полученный от сервера.
        //При GraphsCount = 6 массив содержит шесть значений:
        //  0 и 1 — левая и правая стороны первой платформы;
        //  2 и 3 — левая и правая стороны второй платформы;
        //  4 и 5 — левая и правая стороны третьей платформы.
        //Этот массив хранит исходные данные, чтобы при переключении режима
        //можно было повторно выбрать нужные значения без нового сообщения сервера.
        private readonly decimal[] serverWeights;

        //Флаг текущего режима длины вагона:
        //false — стандартная длина, используются значения 0, 1, 2, 3;
        //true — удлинённая длина, используются значения 0, 1, 4, 5.
        private bool isExtendedLengthMode;
        private volatile bool _isClosing;
        private volatile bool _isWeighingActive;
        private int _reconnectInProgress;
        //поля предназначенные для передачи данных в методы сохранения данных  в БД
        private decimal TareWeight;
        private decimal GrossWeight;
        private decimal LoadCapacity;
        private Guid IdReceipt;
        //количества платформ
        private int CountPlatform;
        private decimal? InvoiceWeighing;
        // Настраивает форму, графики, табло и подписывает форму на события TCP-сервиса.
        public StaticWeighing(ILogger<StaticWeighing> logger, StaticWeighingService staticWeighingService, CurrentUserService currentUserService, TcpService tcpService,
            [FromKeyedServices("CommandTcpService")] TcpService commandTcpService)
        {
            _staticWeighingService = staticWeighingService;
            _logger = logger;
            _currentUserService = currentUserService;
            _tcpService = tcpService;
            _commandTcpService = commandTcpService;
            int graphCount = _currentUserService.GetGraphsCount();

            //Сервер при GraphsCount = 6 присылает данные для трёх платформ,
            //но форма должна отображать только четыре графика: по два значения
            //для выбранной конфигурации вагона. Поэтому количество элементов
            //интерфейса отделено от количества входных значений сервера.
            int displayedGraphCount = graphCount == 6 ? 4 : graphCount;

            //Очередь values содержит историю точек для каждого отображаемого графика.
            //Количество очередей равно числу графиков на форме, а не числу значений
            //в сообщении сервера.
            values = new Queue<decimal>[displayedGraphCount];
            //количества платформ
            CountPlatform = displayedGraphCount;
            for (int i = 0; i < displayedGraphCount; i++)
            {
                values[i] = new Queue<decimal>();
            }
            cartSideWeights = new decimal[displayedGraphCount];

            //serverWeights должен иметь размер graphCount, потому что в нём
            //сохраняется всё сообщение сервера, включая значения невидимой
            //в текущем режиме третьей платформы.
            serverWeights = new decimal[graphCount];
            InitializeComponent();

            comboBoxVagonNumber.DropDownStyle = ComboBoxStyle.DropDown;

            comboBoxVagonNumber.DropDown += comboBoxVagonNumber_DropDown;
            comboBoxVagonNumber.SelectedIndexChanged += comboBoxVagonNumber_SelectedIndexChanged;
            //comboBoxVagonNumber.TextUpdate += comboBoxVagonNumber_TextUpdate;
            //регистрации метода на ожидание новых данных
            _tcpService.MessageReceived += ProcessMessage;
            //регистрация метода на ожидание ошибок
            _tcpService.ConnectionError += OnConnectionError;
            //приём данных канала команд (5005): на время теста показываем полученное в MessageBox
            _commandTcpService.MessageReceived += OnCommandMessageReceived;
            //ошибки канала команд не должны влиять на взвешивание — только лог
            _commandTcpService.ConnectionError += ex =>
                _logger.LogWarning(ex, "Ошибка канала команд (порт 5005).");
            //добавляем при закрытии формы проверку на окончания взвешивания
            this.FormClosing += Form1_FormClosing;
            //formsPlot1.Plot.Axes.Bottom.TickLabelStyle.IsVisible = false;
            //formsPlot1.Refresh();
            //сохраняем обьекты label в List
            labelsList = new()
            {
                lblPlatform1Left,
                lblPlatform1Right,
                lblPlatform2Left,
                lblPlatform2Right
            };
            //сохраняем обьекты в List
            plots = new()
            {
                formsPlot1,
                formsPlot2,
                formsPlot3,
                formsPlot4
            };
            //отображение label согласно количеству графиков
            for (int i = 0; i < displayedGraphCount; i++)
            {
                labelsList[i].Visible = true;
                //labelsList[i].Font.Size = 14;
                //labelsList[i].Font = new Font(label1.Font.FontFamily, 25);
            }
            //отображение графиков согласно их количеству
            for (int i = 0; i < displayedGraphCount; i++)
            {
                plots[i].Visible = true;
                plots[i].Plot.Axes.Bottom.TickLabelStyle.IsVisible = false;
            }
            //1078; 668
            //74; 806
            if (graphCount == 1)
            {
                plots[0].Size = new Size(1078, 668);
                labelsList[0].Font = new Font(label1.Font.FontFamily, 25);
                labelsList[0].Location = new System.Drawing.Point(74, 776);
            }
            //изменяем размер формы для двух графиков
            if (graphCount == 2)
            {
                this.Size = new Size(1561, 558);
            }
            graphTimer.Interval = 1000; // 1 секунда
            //сохраняем функцию, которая будет работать с тиком
            graphTimer.Tick += GraphTimer_Tick;
            formsPlot1.Refresh();
            //сохраняем ссылки
            pictureBoxesList = new List<PictureBox>
            {
                pictureBox6,
                pictureBox5,
                pictureBox4,
                pictureBox3,
                pictureBox2,
                pictureBox1
            };
            //var f = Properties.Resources._00;

            //задаем изначальное на табло (все нули)
            pictureBox1.Image = Properties.Resources._00;
            pictureBox2.Image = Properties.Resources._00;
            pictureBox3.Image = Properties.Resources._00;
            pictureBox4.Image = Properties.Resources._0t;
            pictureBox5.Image = Properties.Resources._0;
            pictureBox6.Image = Properties.Resources._0;
        }
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            //pictureBox1.Image = Properties.Resources._1t;
        }
        //подгрузка данных
        private async void StaticWeighing_Load(object sender, EventArgs e)
        {
            await GetWagonReference();
        }
        private async Task GetWagonReference()
        {
            var result = await _staticWeighingService.GetWagonInfoAsync();
            if (result.Success == false)
            {
                MessageBox.Show($"Произошла ошибка получения вагонов из справочника, причина: {result.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                ReferenceWagonList = result.Data;
                foreach (var item in ReferenceWagonList)
                {
                    comboBoxVagonNumber.Items.Add(item.WagonNumber);
                }
                //comboBoxVagonNumber
                //MessageBox.Show(ReferenceWagonList[1].WagonNumber.ToString());
                //comboBoxVagonNumber
            }
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {

        }

        // Выполняет действие в UI-потоке, если форма ещё существует и не закрывается.
        private void InvokeOnUiThread(Action action)
        {
            if (_isClosing || IsDisposed || Disposing || !IsHandleCreated)
            {
                return;
            }

            if (!InvokeRequired)
            {
                action();
                return;
            }

            try
            {
                BeginInvoke(action);
            }
            catch (InvalidOperationException)
            {
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        // Подключает форму к серверу или завершает текущее взвешивание и отключает соединение.
        private async void button2_Click(object sender, EventArgs e)
        {
            if (btnWeighing.Text == "Начать взвешивание")
            {
                try
                {
                    //_client = new TcpClient();
                    //подключение локального ip адреса
                    //IPAddress ipAddress = await _staticWeighingService.GetLocalIPAddressAsync();
                    //подключение в серверу
                    //await _client.ConnectAsync(ipAddress, 5002)
                    //.WaitAsync(TimeSpan.FromSeconds(5));
                    await _tcpService.ConnectAsync(5002);
                    // _stream = _client.GetStream();

                    // _cts = new CancellationTokenSource();



                    //_ = _tcpService.ReceiveMessagesAsync(_tcpService.Token);
                    //для отладки
                    //MessageBox.Show("Подключено");

                    //начали взвешивание - данные можно сохранить
                    btnSaveWeight.Enabled = true;
                    graphTimer.Start();
                    IdReceipt = Guid.NewGuid();
                    _isWeighingActive = true;

                    if (_currentUserService.UseCamera)
                    {
                        // Второе соединение к тому же IP, но на порт 5005 — канал команд.
                        await ConnectCommandChannelAsync();
                        // Уведомляем сервер о начале захвата. Ошибка связи не должна
                        // отменять локальное начало операции взвешивания.
                        await TrySendCaptureCommandAsync("StartCapture");
                    }

                    btnWeighing.Text = "Закончить взвешивание";
                }
                catch (Exception ex)
                {
                    _logger.LogError("Ошибка: " + ex.Message.ToString());
                    //_cts?.Cancel();

                    //_stream?.Close();
                    //_client?.Close();
                    await _tcpService.DisconnectAsync();
                    //закрываем и канал команд, если он успел подключиться
                    await _commandTcpService.DisconnectAsync();
                    //MessageBox.Show("Соединение закрыто");
                    graphTimer.Stop();
                    MessageBox.Show(
                    ex.Message,
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                }

            }
            else
            {
                //_cts?.Cancel();

                //_stream?.Close();
                //_client?.Close();
                if(_currentUserService.UseCamera)
                {
                    // Уведомляем сервер о завершении захвата. Ошибка связи не должна
                    // отменять локальное завершение операции взвешивания.
                    await TrySendCaptureCommandAsync("EndCapture");
                }
                await _tcpService.DisconnectAsync();
                //закрываем канал команд (порт 5005) вместе с каналом веса
                await _commandTcpService.DisconnectAsync();

                //MessageBox.Show("Соединение закрыто");
                graphTimer.Stop();
                _isWeighingActive = false;

                btnWeighing.Text = "Начать взвешивание";
            }
        }

        // Подключает канал команд к тому же IP, но на порт 5005.
        // Ошибка подключения не отменяет взвешивание — только предупреждение.
        private async Task ConnectCommandChannelAsync()
        {
            try
            {
                await _commandTcpService.ConnectAsync(5005);
                _logger.LogInformation("Успешное подключение к серверу 5005 порта");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Не удалось подключиться к каналу команд (порт 5005).");
                MessageBox.Show(
                    $"Не удалось подключиться к каналу команд (порт 5005): {ex.Message}",
                    "Ошибка подключения",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        // Безопасно отправляет команду управления захватом через канал команд (порт 5005)
        // и не выбрасывает исключение в основной сценарий взвешивания.
        private async Task TrySendCaptureCommandAsync(string command)
        {
            try
            {
                if (!await _commandTcpService.SendMessageAsync(command))
                {
                    MessageBox.Show(
                        $"Не удалось отправить серверу команду {command}.",
                        "Ошибка отправки",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка отправки команды {Command}.", command);
                MessageBox.Show(
                    $"Не удалось отправить серверу команду {command}: {ex.Message}",
                    "Ошибка отправки",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        // Выводит данные, принятые сервером по каналу команд (порт 5005).
        // Временный MessageBox — только для теста.
        private void OnCommandMessageReceived(string message)
        {
            InvokeOnUiThread(() => MessageBox.Show(
                message,
                "Канал команд (порт 5005)",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information));
        }

        //получение значения с весов
        //private async Task ReceiveMessagesAsync(CancellationToken token)
        //{
        //    byte[] buffer = new byte[4096];

        //    try
        //    {
        //        while (!token.IsCancellationRequested)
        //        {
        //            int count = await _stream.ReadAsync(buffer, 0, buffer.Length, token);

        //            if (count == 0)
        //                break;

        //            string message = Encoding.UTF8.GetString(buffer, 0, count);

        //            string[] parts = message.Split(';');
        //            for (int i = 0; i < 4; i++)
        //            {
        //                cartSideWeights[i] = double.Parse(parts[i]) / 1000;
        //            }
        //            lblPlatform1Left.Text = "Платформа 1 левый борт: " + cartSideWeights[0].ToString("F2") + " Т.";
        //            lblPlatform1Right.Text = "Платформа 1 правый борт: "+cartSideWeights[1].ToString("F2")+" Т.";
        //            lblPlatform2Left.Text = "Платформа 2 левый борт: " + cartSideWeights[2].ToString("F2") + " Т.";
        //            lblPlatform2Right.Text = "Платформа 2 правый борт: " + cartSideWeights[3].ToString("F2") + " Т.";
        //            //вызов метода по вывода значения на табло
        //            _ = DisplayingValue(cartSideWeights.Sum());
        //            stable(cartSideWeights.Sum());
        //           //AddPoint(cartSideWeights[0]);
        //            //BeginInvoke(() =>
        //            //{
        //            //    listBox1.Items.Add(message);
        //            //    // либо:
        //            //    // textBox1.AppendText(message + Environment.NewLine);
        //            //});
        //        }
        //    }
        //    //сервер разорвал соединение
        //    catch (IOException ex)
        //    {
        //        MessageBox.Show("Ошибка: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //        _logger.LogError(ex, "Ошибка ввода-вывода. Сервер разорвал соединение");
        //    }
        //    //ошибка потока/работа с закрытым потоком, обьектом которго больше нет
        //    catch (ObjectDisposedException ex)
        //    {
        //        _logger.LogError(ex, "Попытка считывания закрытого потока");
        //    }
        //    //ошибка сокета
        //    catch (SocketException ex)
        //    {
        //        MessageBox.Show("Ошибка", $"Ошибка сокета: {ex.SocketErrorCode}", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //        _logger.LogError(ex, "Ошибка сокета");
        //    }
        //    catch (OperationCanceledException)
        //    {
        //    }
        //    catch (Exception ex)
        //    {
        //        BeginInvoke(() =>
        //        {
        //            //MessageBox.Show(ex.Message);
        //        });
        //        _logger.LogError("Ошибка: " + ex.Message.ToString());
        //    }
        //}
        //private async Task CreateGraphAsync(double coordinatePoint)
        //{ 

        //}

        // Показывает ошибку соединения и выполняет ограниченное переподключение,
        // пока форма открыта и взвешивание остаётся активным.
        private async void OnConnectionError(Exception ex)
        {
            if (_isClosing || !_isWeighingActive || Interlocked.Exchange(ref _reconnectInProgress, 1) != 0)
            {
                return;
            }

            InvokeOnUiThread(() =>
            {
                graphTimer.Stop();
                //btnSaveWeight.Enabled = false;
                //btnWeighing.Text = "Начать взвешивание";

                MessageBox.Show(
                    ex.Message + ". Пытаемся переподключиться",
                    "Ошибка сервера",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            });

            try
            {
                //переподключение к серверу
                while (!_isClosing && _isWeighingActive)
                {
                    try
                    {
                        if (!_isClosing && _isWeighingActive && !_tcpService.ServerConnected)
                        {
                            await _tcpService.DisconnectAsync();

                            await Task.Delay(5000);

                            if (_isClosing || !_isWeighingActive)
                            {
                                break;
                            }

                            await _tcpService.ConnectAsync(5002);

                            InvokeOnUiThread(() =>
                            {
                                graphTimer.Start();
                                lblConnectScale.BackColor = Color.Green;
                                //btnSaveWeight.Enabled = true;
                            });
                            break;
                        }

                        break;
                    }
                    catch (Exception reconnectEx)
                    {
                        InvokeOnUiThread(() => lblConnectScale.BackColor = Color.Red);
                        _logger.LogWarning(reconnectEx,
                            "Не удалось подключиться. Повтор через 5 секунд.");

                        await Task.Delay(5000);
                    }
                }
            }
            finally
            {
                Interlocked.Exchange(ref _reconnectInProgress, 0);
            }
        }
        // Принимает сообщения TCP-сервиса и передаёт их обработку в UI-поток.
        private void ProcessMessage(string message)
        {
            InvokeOnUiThread(() => ProcessMessageOnUiThread(message));
        }

        // Проверяет сообщение, обновляет текущие веса, подписи, табло и стабильность.
        private void ProcessMessageOnUiThread(string message)
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

                //Сообщение принимается только при полном совпадении количества
                //значений и ожидаемого количества платформ. Например, при GraphsCount = 6
                //корректным будет сообщение вида "43;54;76;34;65;76".
                if (serverWeights.Length == parts.Length)
                {
                    //Сначала сохраняем все значения сервера в исходном порядке.
                    //Деление на 1000 переводит значение из единиц сервера в тонны.
                    decimal[] parsedWeights = new decimal[serverWeights.Length];
                    for (int i = 0; i < serverWeights.Length; i++)
                    {
                        if (!decimal.TryParse(parts[i], NumberStyles.Number, CultureInfo.InvariantCulture, out var value))
                        {
                            _logger.LogWarning("Получено некорректное значение веса: {Value}", parts[i]);
                            return;
                        }

                        parsedWeights[i] = value / 1000m;
                    }

                    Array.Copy(parsedWeights, serverWeights, serverWeights.Length);

                    //Получаем индексы четырёх значений, которые должны отображаться
                    //в текущем режиме, и переносим их в массив данных формы.
                    int[] displayedIndexes = GetDisplayedIndexes();
                    for (int i = 0; i < cartSideWeights.Length; i++)
                    {
                        cartSideWeights[i] = serverWeights[displayedIndexes[i]];
                    }

                    if (cartSideWeights.Length > 3)
                    {
                        lblPlatform1Left.Text = $"Платформа 1 левый борт: {cartSideWeights[0]:F2} Т.";
                        lblPlatform1Right.Text = $"Платформа 1 правый борт: {cartSideWeights[1]:F2} Т.";
                        lblPlatform2Left.Text = $"Платформа 2 левый борт: {cartSideWeights[2]:F2} Т.";
                        lblPlatform2Right.Text = $"Платформа 2 правый борт: {cartSideWeights[3]:F2} Т.";
                    }
                    else
                    {
                        if (cartSideWeights.Length > 0)
                        {
                            lblPlatform1Left.Text = $"Платформа 1: {cartSideWeights[0]:F2} Т.";
                        }
                        if (cartSideWeights.Length > 1)
                        {
                            lblPlatform1Right.Text = $"Платформа 2: {cartSideWeights[1]:F2} Т.";
                        }
                        if (cartSideWeights.Length > 2)
                        {
                            lblPlatform2Left.Text = $"Платформа 3: {cartSideWeights[2]:F2} Т.";
                        }
                    }
                    //вызов метода по вывода значения на табло
                    _ = DisplayingValue(cartSideWeights.Sum());
                    stable(cartSideWeights.Sum());
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


        // Проверяет, удерживается ли общий вес в допустимом диапазоне стабильности.
        private void stable(decimal data)
        {
            for (int i = 0; i < stableWeight.Length; i++)
            {
                if (Math.Abs(stableWeight[i] - data) <= 0.100m)
                {
                    if (stableWeight.Length - 1 == i)
                    {
                        pictureBoxStabilityTrue.Visible = true;
                        pictureBoxStabilityFalse.Visible = false;
                    }
                }
                else
                {
                    pictureBoxStabilityTrue.Visible = false;
                    pictureBoxStabilityFalse.Visible = true;
                    stableWeight[i] = data;
                    break;
                }
            }
        }
        // Проверяет введённые данные и сохраняет результат взвешивания в базу данных.
        private async void btnSaveWeight_Click(object sender, EventArgs e)
        {
            btnSaveWeight.Enabled = false;
            if (comboBoxVagonNumber.Text.Length != 8)
            {
                MessageBox.Show("Перед сохранением введите корректный номер вагона", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                btnSaveWeight.Enabled = true;
                return;
            }
            //проверка на стабильность веса перед сохранением
            if (pictureBoxStabilityFalse.Visible == true && pictureBoxStabilityTrue.Visible == false)
            {
                MessageBox.Show("Перед сохранением дождитесь, чтобы вес был стабилен", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                btnSaveWeight.Enabled = true;
                return;
            }

            if (cBoxTypeWeighing.Text == "Тара")
            {
                TareWeight = cartSideWeights.Sum();
                GrossWeight = 0;
            }
            else if (cBoxTypeWeighing.Text == "Брутто")
            {
                GrossWeight = cartSideWeights.Sum();
                TareWeight = 0;
            }
            else
            {
                MessageBox.Show("Выберите тип взвешивания", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                btnSaveWeight.Enabled = true;
                return;
            }
            //проверка и преобразования поля в decimal для записи в модель
            if (string.IsNullOrWhiteSpace(textBoxInvoiceWeighing.Text))
            {
                InvoiceWeighing = null;
            }
            else
            {
                string text = textBoxInvoiceWeighing.Text.Trim().Replace(',', '.');

                if (decimal.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal value))
                {
                    InvoiceWeighing = value;
                }
                else
                {
                    MessageBox.Show("Введите корректное значение веса по накладной", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    btnSaveWeight.Enabled = true;
                    return;
                }
                //проверка поля грузоподьемности
                //string textLoadCapacity = txtLoadCapacity.Text.Trim()
                //    .Replace('.', ',');
                //if (!decimal.TryParse(textLoadCapacity.Trim(), out LoadCapacity))
                //{
                //    MessageBox.Show("Введите корректное значение грузоподъёмности");
                //    return;
                //}
            }
            //проверка поля грузоподьемности
            string textLoadCapacity = txtLoadCapacity.Text.Trim()
                .Replace('.', ',');
            if (string.IsNullOrWhiteSpace(textLoadCapacity))
            {
                LoadCapacity = 0;
            }
            else if (!decimal.TryParse(textLoadCapacity.Trim(), out LoadCapacity))
            {
                MessageBox.Show("Введите корректное значение грузоподъёмности", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                btnSaveWeight.Enabled = true;
                return;
            }
            //ссоздание квитанции
            var resultsaveReceipt = await _staticWeighingService.saveReceiptAsync(IdReceipt, "Статическое взвешивание", _currentUserService.CurrentUser.Name);
            //проверка на создание квитанции
            if (resultsaveReceipt.Success == false)
            {
                MessageBox.Show($"Квитанция не была создана, причина: {resultsaveReceipt.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                btnSaveWeight.Enabled = true;
                return;
            }
            for (int i = 0; i < cartSideWeights.Length; i++)
            {
                cartAxesWeightsList[i] = cartSideWeights[i];
            }
            WeighingDto dto = new WeighingDto
            {
                Platform1Left = cartAxesWeightsList[0],
                Platform1Right = cartAxesWeightsList[1],
                Platform2Left = cartAxesWeightsList[2],
                Platform2Right = cartAxesWeightsList[3],
                VagonNumber = comboBoxVagonNumber.Text,
                TareWeight = TareWeight,
                GrossWeight = GrossWeight,
                TypeWeighing = cBoxTypeWeighing.Text,
                LoadCapacity = LoadCapacity,
                Shipper = textBoxShipper.Text,
                Сonsignee = textBoxСonsignee.Text,
                Сargo = textBoxСargo.Text,
                InvoiceNumber = textBoxInvoiceNumber.Text,
                InvoiceDataTime = dateTimePickerInvoice.Value,
                InvoiceWeighing = InvoiceWeighing,
                IdReceipt = IdReceipt
            };
            //Для 3-х платформенных
            if (CountPlatform == 3)
            {
                dto.Platform1Left = cartAxesWeightsList[0];
                dto.Platform1Right = 0;
                dto.Platform2Left = cartAxesWeightsList[1];
                dto.Platform2Right = cartAxesWeightsList[2];
            }
            var result = await _staticWeighingService.saveWeighingAsync(dto);
            //проерка на сохранение данных
            if (result.Success == false)
            {
                MessageBox.Show("Данные взвешивания не были сохранены в БД. Причина: " + result.Message, "Возникла ошибки при сохранении в БД", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnSaveWeight.Enabled = true;
            }
            else
            {
                MessageBox.Show("Данные успешно сохранены в БД", "Данные сохранены", MessageBoxButtons.OK, MessageBoxIcon.Information);
                btnSaveWeight.Enabled = true;
            }
        }
        // Добавляет новое значение в историю графика и обновляет его отображение.
        private void AddPoint(int indexObject, decimal value)
        {
            //foreach (var plot in plots)
            //{
            if (values[indexObject].Count == 300)
                values[indexObject].Dequeue();

            values[indexObject].Enqueue(value);

            plots[indexObject].Plot.Clear();
            plots[indexObject].Plot.Add.Signal(values[indexObject].ToArray());

            plots[indexObject].Plot.Axes.SetLimits(
                left: 0,
                right: values[indexObject].Count - 1);

            plots[indexObject].Plot.Axes.AutoScaleY();

            plots[indexObject].Refresh();
            //plot.Plot.Clear();
            //plot.Refresh();
            //}

        }
        // Не позволяет закрыть форму во время активного взвешивания.
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!btnWeighing.Text.Equals("Начать взвешивание", StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show(
                "Сначала закончите взвешивание!",
                "Предупреждение",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);

                e.Cancel = true;
                return;
            }

            _isClosing = true;
            _isWeighingActive = false;
        }
        // Преобразует общий вес в изображения и выводит их на табло.
        private async Task DisplayingValue(decimal sumeWeight)
        {
            var ListValuesImage = await _staticWeighingService.GetImageWeighingAsync(sumeWeight);
            for (int i = 0; ListValuesImage.Count > i; i++)
            {
                if (pictureBoxesList.Count - 1 >= i)
                {
                    pictureBoxesList[i].Image = ListValuesImage[i];
                }

            }
        }
        // Периодически добавляет текущие значения бортов на графики.
        private void GraphTimer_Tick(object? sender, EventArgs e)
        {
            for (int i = 0; i < cartSideWeights.Length; i++)
            {
                AddPoint(i, cartSideWeights[i]);
            }
            //AddPoint(0, cartSideWeights[0]);
            //AddPoint(1, cartSideWeights[1]);
            //AddPoint(2, cartSideWeights[2]);
            //AddPoint(3, cartSideWeights[3]);
        }

        private void label10_Click(object sender, EventArgs e)
        {

        }
        // Обрабатывает служебный статус соединения весов, полученный от сервера.
        private bool ConnectionScalesCheck(string data)
        {
            string[] parts = data.Split(';');
            for (int i = 0; parts.Length > i; i++)
            {
                //проверка на то, что сервер прислал, что соединения с весами нет - обозначаем это
                if (parts[i] == "OFFLINE")
                {
                    //выводим, что соединение нет
                    lblConnectScale.BackColor = Color.Red;
                    btnSaveWeight.Enabled = false;
                    return false;
                }
            }
            //выводим, что соединение есть
            lblConnectScale.BackColor = Color.Green;
            btnSaveWeight.Enabled = true;
            return true;
        }
        // Фильтрует список вагонов по уже введённому префиксу номера.
        private void comboBoxVagonNumber_DropDown(object sender, EventArgs e)
        {
            if (ReferenceWagonList == null)
                return;

            string searchText = comboBoxVagonNumber.Text;

            var filteredWagons = ReferenceWagonList
                .Where(x => x.WagonNumber.StartsWith(searchText))
                .Select(x => x.WagonNumber)
                .ToArray();

            comboBoxVagonNumber.Items.Clear();
            comboBoxVagonNumber.Items.AddRange(filteredWagons);
        }
        // Показывает грузоподъёмность выбранного вагона.
        private void comboBoxVagonNumber_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ReferenceWagonList == null)
                return;

            string selectedWagonNumber = comboBoxVagonNumber.Text;

            var wagon = ReferenceWagonList
                .FirstOrDefault(x => x.WagonNumber == selectedWagonNumber);

            if (wagon != null)
            {
                txtLoadCapacity.Text = wagon.LoadCapacity.ToString("0.##");
            }
            else
            {
                txtLoadCapacity.Clear();
            }
        }
        private void lblPlatform1Right_Click(object sender, EventArgs e)
        {

        }

        // Переключает набор платформ для шестизначного сообщения сервера.
        private async void btnSwitchingPlatform_Click(object sender, EventArgs e)
        {
            //Переключение режимов имеет смысл только для сообщения из шести значений.
            //Для других конфигураций количество отображаемых графиков не меняется.
            if (serverWeights.Length != 6)
            {
                return;
            }

            //Меняем режим на противоположный и синхронно обновляем подпись кнопки.
            isExtendedLengthMode = !isExtendedLengthMode;
            btnSwitchingPlatform.Text = isExtendedLengthMode
                ? "Удлиненная длина вагона"
                : "Стандартная длина вагона";

            // При активном соединении передаём на сервер текущее состояние длины вагона.
            if (_tcpService.ServerConnected)
            {
                await _tcpService.SendMessageAsync(isExtendedLengthMode ? "1" : "0");
            }

            //Сразу обновляем четыре отображаемых значения из уже полученного
            //сообщения сервера. Ожидание следующего сообщения не требуется.
            int[] displayedIndexes = GetDisplayedIndexes();
            for (int i = 0; i < cartSideWeights.Length; i++)
            {
                cartSideWeights[i] = serverWeights[displayedIndexes[i]];
            }

            //После смены платформы ранее накопленная стабильность больше
            //не относится к новому набору данных, поэтому начинаем проверку заново.
            stableWeight = new decimal[100];
            pictureBoxStabilityTrue.Visible = false;
            pictureBoxStabilityFalse.Visible = true;

            //Графики 3 и 4 являются нижними графиками формы.
            //Их история удаляется, после чего в каждый добавляется одна нулевая точка,
            //чтобы пользователь сразу видел обнуление, не дожидаясь тика таймера.
            for (int i = 2; i < values.Length; i++)
            {
                values[i].Clear();
                plots[i].Plot.Clear();
                plots[i].Plot.Add.Signal(new[] { 0d });
                plots[i].Plot.Axes.SetLimits(left: 0, right: 0);
                plots[i].Plot.Axes.AutoScaleY();
                plots[i].Refresh();
                cartSideWeights[i] = 0;
            }

            UpdateDisplayedLabels();
        }

        // Возвращает индексы серверных значений для текущего режима длины вагона.
        private int[] GetDisplayedIndexes()
        {
            //Первые два значения относятся к первой платформе и используются
            //в обоих режимах. Для стандартной длины берутся значения второй платформы
            //(2, 3), а для удлинённой — третьей платформы (4, 5).
            return isExtendedLengthMode && serverWeights.Length == 6
                ? new[] { 0, 1, 4, 5 }
                : new[] { 0, 1, 2, 3 };
        }

        // Обновляет подписи платформ после переключения режима отображения.
        private void UpdateDisplayedLabels()
        {
            //После переключения режима обновляем подписи теми же значениями,
            //которые используются для графиков и расчёта общего веса.
            if (cartSideWeights.Length > 3)
            {
                lblPlatform1Left.Text = $"Платформа 1 левый борт: {cartSideWeights[0]:F2} Т.";
                lblPlatform1Right.Text = $"Платформа 1 правый борт: {cartSideWeights[1]:F2} Т.";
                lblPlatform2Left.Text = $"Платформа 2 левый борт: {cartSideWeights[2]:F2} Т.";
                lblPlatform2Right.Text = $"Платформа 2 правый борт: {cartSideWeights[3]:F2} Т.";
            }
        }     
    }
}

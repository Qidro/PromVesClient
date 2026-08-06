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

        //private TcpClient? _client;
        //private NetworkStream? _stream;
        //private CancellationTokenSource? _cts;

        //коллекциями с ссылка на картинки 
        private List<PictureBox> pictureBoxesList;
        private ScottPlot.Plottables.Signal signal;
        //сохранение ссылок на обьекты графиков
        private List<ScottPlot.WinForms.FormsPlot> plots;
        //Предназначен для создания точек на графике
        private readonly Queue<decimal>[] values =
        {
            new Queue<decimal>(),
            new Queue<decimal>(),
            new Queue<decimal>(),
            new Queue<decimal>()
        };
        //таймер предназначен для создания точек для 4 графиков
        private readonly System.Windows.Forms.Timer graphTimer = new();
        //переменная предназначенная для соханения данных веса с бортов
        private decimal[] cartSideWeights = new decimal[4];
        //переменная, которая сохраняет полученные значения для расчета стабильности
        private decimal[] stableWeight = new decimal[100];
        //поля предназначенные для передачи данных в методы сохранения данных  в БД
        private decimal TareWeight;
        private decimal GrossWeight;
        private Guid IdReceipt;

        private decimal? InvoiceWeighing;
        public StaticWeighing(ILogger<StaticWeighing> logger, StaticWeighingService staticWeighingService, CurrentUserService currentUserService, TcpService tcpService)
        {
            _staticWeighingService = staticWeighingService;
            _logger = logger;
            _currentUserService = currentUserService;
            _tcpService = tcpService;
            InitializeComponent();
            //регистрации метода на ожидание новых данных
            _tcpService.MessageReceived += ProcessMessage;
            //регистрация метода на ожидание ошибок
            _tcpService.ConnectionError += OnConnectionError;
            //добавляем при закрытии формы проверку на окончания взвешивания
            this.FormClosing += Form1_FormClosing;
            //сохраняем обьекты в List
            plots = new()
            {
                formsPlot1,
                formsPlot2,
                formsPlot3,
                formsPlot4
            };
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

        private void StaticWeighing_Load(object sender, EventArgs e)
        {

        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {

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
        //нажатие на кнопку, которое отвечает за подключение к серверу и получению данных от него
        //либо его отключение от сервера
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
                    await _tcpService.ConnectAsync();
                    // _stream = _client.GetStream();

                    // _cts = new CancellationTokenSource();

                    

                    //_ = _tcpService.ReceiveMessagesAsync(_tcpService.Token);
                    //для отладки
                    //MessageBox.Show("Подключено");

                    //начали взвешивание - данные можно сохранить
                    btnSaveWeight.Enabled = true;
                    graphTimer.Start();
                    IdReceipt = Guid.NewGuid();
                    btnWeighing.Text = "Закончить взвешивание";
                }
                catch (Exception ex)
                {
                    _logger.LogError("Ошибка: " + ex.Message.ToString());
                    //_cts?.Cancel();

                    //_stream?.Close();
                    //_client?.Close();
                    await _tcpService.DisconnectAsync();
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
                await _tcpService.DisconnectAsync();

                //MessageBox.Show("Соединение закрыто");
                graphTimer.Stop();
                btnWeighing.Text = "Начать взвешивание";
            }
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

        //событие ошибки
        private async void OnConnectionError(Exception ex)
        {
            BeginInvoke(() =>
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
            //переподключение к серверу
            while (true)
            {
                try
                {
                    await _tcpService.DisconnectAsync();

                    await Task.Delay(5000);

                    await _tcpService.ConnectAsync();

                    BeginInvoke(() =>
                    {
                        graphTimer.Start();
                        //btnSaveWeight.Enabled = true;
                    });

                    break;
                }
                catch (Exception reconnectEx)
                {
                    _logger.LogWarning(reconnectEx,
                        "Не удалось подключиться. Повтор через 5 секунд.");
                }
            }
        }
        //метод для события(получения данных с сервака) по обработке полцченных данных
        private void ProcessMessage(string message)
        {
            try 
            {
                //Console.WriteLine("мы находится в событии");
                string[] parts = message.Split(';');
                //обработка 4 графиков
                for (int i = 0; i < 4; i++)
                {
                    cartSideWeights[i] = decimal.Parse(parts[i]) / 1000;
                }
                lblPlatform1Left.Text = "Платформа 1 левый борт: " + cartSideWeights[0].ToString("F2") + " Т.";
                lblPlatform1Right.Text = "Платформа 1 правый борт: " + cartSideWeights[1].ToString("F2") + " Т.";
                lblPlatform2Left.Text = "Платформа 2 левый борт: " + cartSideWeights[2].ToString("F2") + " Т.";
                lblPlatform2Right.Text = "Платформа 2 правый борт: " + cartSideWeights[3].ToString("F2") + " Т.";
                //вызов метода по вывода значения на табло
                _ = DisplayingValue(cartSideWeights.Sum());
                stable(cartSideWeights.Sum());
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


        //расчет стабильности вагона
        private void stable(decimal data)
        {
            for (int i = 0; i<stableWeight.Length; i++)
            {
                if (stableWeight[i] == data && i == stableWeight.Length - 1)
                {
                    pictureBoxStabilityTrue.Visible = true;
                    pictureBoxStabilityFalse.Visible = false;
                }
                else if(stableWeight[i] < data || stableWeight[i] > data)
                {
                    pictureBoxStabilityTrue.Visible = false;
                    pictureBoxStabilityFalse.Visible = true;
                    stableWeight[i] = data;
                    break;
                }
            }
        }
        //сохранение
        private async void btnSaveWeight_Click(object sender, EventArgs e)
        {
            if (comboBoxVagonNumber.Text.Length != 8)
            {
                MessageBox.Show("Перед сохранением введите корректный номер вагона", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            //проверка на стабильность веса перед сохранением
            if (pictureBoxStabilityFalse.Visible == true && pictureBoxStabilityTrue.Visible == false)
            {
                MessageBox.Show("Перед сохранением дождитесь, чтобы вес был стабилен", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                    return;
                }
            }

            var resultt = await _staticWeighingService.saveReceiptAsync(IdReceipt, "Статическое взвешивание", _currentUserService.CurrentUser.Name);
            WeighingDto dto = new WeighingDto
            {
                Platform1Left = cartSideWeights[0],
                Platform1Right = cartSideWeights[1],
                Platform2Left = cartSideWeights[2],
                Platform2Right = cartSideWeights[3],
                VagonNumber = comboBoxVagonNumber.Text,
                TareWeight = TareWeight,
                GrossWeight = GrossWeight,
                TypeWeighing = cBoxTypeWeighing.Text,
                Shipper = textBoxShipper.Text,
                Сonsignee = textBoxСonsignee.Text,
                Сargo = textBoxСargo.Text,
                InvoiceNumber = textBoxInvoiceNumber.Text,
                InvoiceDataTime = dateTimePickerInvoice.Value,
                InvoiceWeighing = InvoiceWeighing,
                IdReceipt = IdReceipt
            };
            var result = await _staticWeighingService.saveWeighingAsync(dto);
            //проерка на сохранение данных
            if (result.Success == false)
            {
                MessageBox.Show("Данные взвешивания не были сохранены в БД. Причина: "+ result.Message, "Возникла ошибки при сохранении в БД", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else 
            {
                MessageBox.Show("Данные успешно сохранены в БД", "Данные сохранены", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        //создание точек на графике
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
        //ивент на закрытие формы, если взвешивание активно - форма не будет закрыта и будет предупреждение
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
            }
        }
        //метод записи значений на табло
        private async Task DisplayingValue(decimal sumeWeight)
        {
            var ListValuesImage =  await _staticWeighingService.GetImageWeighingAsync(sumeWeight);
            for(int i = 0; ListValuesImage.Count > i; i++)
            {
                if (pictureBoxesList.Count-1 >= i)
                {
                    pictureBoxesList[i].Image = ListValuesImage[i];
                }
                
            }
        }
        //метод таймера
        private void GraphTimer_Tick(object? sender, EventArgs e)
        {
            AddPoint(0, cartSideWeights[0]);
            AddPoint(1, cartSideWeights[1]);
            AddPoint(2, cartSideWeights[2]);
            AddPoint(3, cartSideWeights[3]);
        }
    }
}

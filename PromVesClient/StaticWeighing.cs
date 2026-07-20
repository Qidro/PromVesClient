using Microsoft.Extensions.Logging;
using PromVesClient.DTO;
using PromVesClient.Service;
using PromVesClient.Service.StaticWeighingService;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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

        private TcpClient? _client;
        private NetworkStream? _stream;
        private CancellationTokenSource? _cts;

        //коллекциями с ссылка на картинки 
        private List<PictureBox> pictureBoxesList;
        private ScottPlot.Plottables.Signal signal;
        //сохранение ссылок на обьекты графиков
        private List<ScottPlot.WinForms.FormsPlot> plots;
        //Предназначен для создания точек на графике
        private readonly Queue<double>[] values =
        {
            new Queue<double>(),
            new Queue<double>(),
            new Queue<double>(),
            new Queue<double>()
        };
        //таймер предназначен для создания точек для 4 графиков
        private readonly System.Windows.Forms.Timer graphTimer = new();
        //переменная предназначенная для соханения данных веса с бортов
        private double[] cartSideWeights = new double[4];
        //переменная, которая сохраняет полученные значения для расчета стабильности
        private double[] stableWeight = new double[100];
        //поля предназначенные для передачи данных в методы сохранения данных  в БД
        private double TareWeight;
        private double GrossWeight;
        private Guid IdReceipt;

        public StaticWeighing(ILogger<StaticWeighing> logger, StaticWeighingService staticWeighingService, CurrentUserService currentUserService)
        {
            _staticWeighingService = staticWeighingService;
            _logger = logger;
            _currentUserService = currentUserService;
            InitializeComponent();
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
            
            //         cartesianChart1.Size = new Size(600, 300);
            //         //отключение всплывающей подсказки
            //         cartesianChart1.TooltipPosition = LiveChartsCore.Measure.TooltipPosition.Hidden;
            //         cartesianChart1.Series = new ISeries[]
            //{
            //     new LineSeries<double>
            //     {
            //         Values = values,
            //          GeometrySize = 0,      // не рисовать кружки
            //     Fill = null,           // убрать заливку под линией
            //     LineSmoothness = 0     // прямая линия без сглаживания (по желанию)
            //         //Values = new double[] { 1,2,3,4,5,6,7,8,9, 10, 11,12, 13,14,15,16,17,18,19,20 }
            //     }
            //};
            //         values.Add(10.1);
            //    double[] values =
            //        {
            //    5,8,3,7,4,9,2,500,10
            //};

            //    formsPlot1.Plot.Add.Signal(values);

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
            pictureBox1.Image = Properties.Resources._00;
            pictureBox2.Image = Properties.Resources._00;
            pictureBox3.Image = Properties.Resources._00;
            pictureBox4.Image = Properties.Resources._0t;
            pictureBox5.Image = Properties.Resources._0;
            pictureBox6.Image = Properties.Resources._0;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = Properties.Resources._1t;
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
                    _client = new TcpClient();
                    //подключение локального ip адреса
                    IPAddress ipAddress = GetLocalIPAddress();
                    //подключение в серверу
                    await _client.ConnectAsync(ipAddress, 5002)
             .WaitAsync(TimeSpan.FromSeconds(5));

                    _stream = _client.GetStream();

                    _cts = new CancellationTokenSource();
                    //ожидание новых данных
                    _ = ReceiveMessagesAsync(_cts.Token);
                    //для отладки
                    MessageBox.Show("Подключено");
                    //начали взвешивание - данные можно сохранить
                    btnSaveWeight.Enabled = true;
                    graphTimer.Start();
                    IdReceipt = Guid.NewGuid();
                    btnWeighing.Text = "Закончить взвешивание";
                }
                catch (Exception ex)
                {
                    _logger.LogError("Ошибка: " + ex.Message.ToString());

                    MessageBox.Show(
                    ex.Message,
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                }

            }
            else
            {
                _cts?.Cancel();

                _stream?.Close();
                _client?.Close();

                //MessageBox.Show("Соединение закрыто");
                graphTimer.Stop();
                btnWeighing.Text = "Начать взвешивание";
            }
        }
        private static IPAddress GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());

            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                    return ip;
            }

            throw new Exception("Локальный IPv4 адрес не найден.");
        }
        //получение значения с весов
        private async Task ReceiveMessagesAsync(CancellationToken token)
        {
            byte[] buffer = new byte[4096];

            try
            {
                while (!token.IsCancellationRequested)
                {
                    int count = await _stream.ReadAsync(buffer, 0, buffer.Length, token);

                    if (count == 0)
                        break;

                    string message = Encoding.UTF8.GetString(buffer, 0, count);

                    string[] parts = message.Split(';');
                    for (int i = 0; i < 4; i++)
                    {
                        cartSideWeights[i] = double.Parse(parts[i]) / 1000;
                    }
                    lblPlatform1Left.Text = "Платформа 1 левый борт: " + cartSideWeights[0].ToString("F2") + " Т.";
                    lblPlatform1Right.Text = "Платформа 1 правый борт: "+cartSideWeights[1].ToString("F2")+" Т.";
                    lblPlatform2Left.Text = "Платформа 2 левый борт: " + cartSideWeights[2].ToString("F2") + " Т.";
                    lblPlatform2Right.Text = "Платформа 2 правый борт: " + cartSideWeights[3].ToString("F2") + " Т.";
                    //вызов метода по вывода значения на табло
                    _ = DisplayingValue(cartSideWeights.Sum());
                    stable(cartSideWeights.Sum());
                   //AddPoint(cartSideWeights[0]);
                    //BeginInvoke(() =>
                    //{
                    //    listBox1.Items.Add(message);
                    //    // либо:
                    //    // textBox1.AppendText(message + Environment.NewLine);
                    //});
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                BeginInvoke(() =>
                {
                    MessageBox.Show(ex.Message);
                });
                _logger.LogError("Ошибка: " + ex.Message.ToString());
            }
        }
        private async Task CreateGraphAsync(double coordinatePoint)
        { 
            
        }
        //расчет стабильности вагона
        private void stable(double data)
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
        private async void btnSaveWeight_Click(object sender, EventArgs e)
        {
            var resultt = await _staticWeighingService.saveReceiptAsync(IdReceipt, cBoxTypeWeighing.Text, "123");
            if (cBoxTypeWeighing.Text == "Тара")
            {
                TareWeight = cartSideWeights.Sum();
                GrossWeight = 0;
            }
            else if(cBoxTypeWeighing.Text =="Брутто")
            {
                GrossWeight = cartSideWeights.Sum();
                TareWeight = 0;
            }
            WeighingDto dto = new WeighingDto
            {
                Platform1Left = cartSideWeights[0],
                Platform1Right = cartSideWeights[1],
                Platform2Left = cartSideWeights[2],
                Platform2Right = cartSideWeights[3],
                VagonNumber = comboBoxVagonNumber.Text,
                TareWeight = TareWeight,
                GrossWeight = GrossWeight,
                IdReceipt = IdReceipt
            };
            var result = await _staticWeighingService.saveWeighingAsync(dto);
        }
        private void AddPoint(int indexObject, double value)
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
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!btnWeighing.Text.Equals("Начать взвешивание", StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("Сначала закончите взвешивание!");

                e.Cancel = true;
            }
        }
        //метод записи значений на табло
        private async Task DisplayingValue(double sumeWeight)
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

        private void GraphTimer_Tick(object? sender, EventArgs e)
        {
            AddPoint(0, cartSideWeights[0]);
            AddPoint(1, cartSideWeights[1]);
            AddPoint(2, cartSideWeights[2]);
            AddPoint(3, cartSideWeights[3]);
        }
    }
}

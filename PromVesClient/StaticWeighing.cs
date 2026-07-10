using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Net.Sockets;
using System.Text;
using System.Text;
using System.Windows.Forms;


namespace PromVesClient
{
    public partial class StaticWeighing : Form
    {
        private TcpClient? _client;
        private NetworkStream? _stream;
        private CancellationTokenSource? _cts;
        //логи
        private readonly ILogger<StaticWeighing> _logger;


        private ObservableCollection<double> values = new(Enumerable.Range(1, 500).Select(x => (double)x).ToArray());
        public StaticWeighing(ILogger<StaticWeighing> logger)
        {
            _logger = logger;

            InitializeComponent();
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

                    await _client.ConnectAsync("192.168.1.100", 5002).WaitAsync(TimeSpan.FromSeconds(5));

                    _stream = _client.GetStream();

                    _cts = new CancellationTokenSource();

                    _ = ReceiveMessagesAsync(_cts.Token);

                    MessageBox.Show("Подключено");
                    btnSaveWeight.Enabled = true;
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

                MessageBox.Show("Соединение закрыто");
                btnWeighing.Text = "Начать взвешивание";
            }
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
    }
}

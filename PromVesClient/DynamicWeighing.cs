using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Windows.Forms;
using PromVesClient.DTO;
using PromVesClient.Service.TcpService;

namespace PromVesClient
{
    public partial class DynamicWeighing : Form
    {
        // Сервис отвечает за подключение к серверу и получение сообщений по TCP.
        private readonly TcpService _tcpService;

        // Настройки разбора JSON: сервер может отдавать свойства как в PascalCase,
        // так и в camelCase, а даты — в ISO 8601 или в старом формате.
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new FlexibleDateTimeConverter() }
        };

        // Инициализирует форму и подписывает её на события TCP-сервиса.
        public DynamicWeighing(TcpService tcpService)
        {
            _tcpService = tcpService;
            InitializeComponent();

            _tcpService.MessageReceived += OnMessageReceived;
            _tcpService.ConnectionError += OnConnectionError;
            FormClosing += DynamicWeighing_FormClosing;
        }

        private void dataGridViewReceipts_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void DynamicWeighing_Load(object sender, EventArgs e)
        {

        }

        private async void btnWeighing_Click(object sender, EventArgs e)
        {
            // Первое нажатие запускает взвешивание и подключается к порту 5004.
            if (btnWeighing.Text == "Начать взвешивание")
            {
                try
                {
                    await _tcpService.ConnectAsync(5004);
                    btnWeighing.Text = "Закончить взвешивание";
                }
                catch (Exception ex)
                {
                    await _tcpService.DisconnectAsync();
                    MessageBox.Show(ex.Message, "Ошибка подключения", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                // Повторное нажатие завершает взвешивание и закрывает соединение.
                await _tcpService.DisconnectAsync();
                btnWeighing.Text = "Начать взвешивание";
            }
        }

        // Обрабатывает сообщение сервера и добавляет результат в таблицу.
        private void OnMessageReceived(string message)
        {
            // TCP-сервис принимает данные в фоновом потоке, поэтому обновление
            // элементов Windows Forms необходимо выполнить в UI-потоке.
            if (InvokeRequired)
            {
                BeginInvoke(() => OnMessageReceived(message));
                return;
            }

            if (IsDisposed || !TryParseWeighing(message, out List<RailcarWeightResultDto> results))
            {
                return;
            }

            foreach (RailcarWeightResultDto result in results)
            {
                dataGridViewReceipts.Rows.Add(
                    result.Number.ToString(CultureInfo.CurrentCulture),
                    result.StartTime.ToString("dd.MM.yyyy HH:mm:ss"),
                    result.EndTime.ToString("dd.MM.yyyy HH:mm:ss"),
                    FormatWeight(result.TotalWeight));
            }

            ShowResults(results);
        }

        // Разбирает JSON с результатом взвешивания вагона (или массивом результатов).
        private static bool TryParseWeighing(string message,
            out List<RailcarWeightResultDto> results)
        {
            results = new List<RailcarWeightResultDto>();

            try
            {
                using JsonDocument document = JsonDocument.Parse(message);
                JsonElement root = document.RootElement;

                // Сервер может прислать как один результат, так и массив результатов.
                if (root.ValueKind == JsonValueKind.Array)
                {
                    foreach (JsonElement element in root.EnumerateArray())
                    {
                        RailcarWeightResultDto? result =
                            element.Deserialize<RailcarWeightResultDto>(JsonOptions);

                        if (result is not null)
                        {
                            results.Add(result);
                        }
                    }
                }
                else
                {
                    RailcarWeightResultDto? result =
                        root.Deserialize<RailcarWeightResultDto>(JsonOptions);

                    if (result is not null)
                    {
                        results.Add(result);
                    }
                }

                return results.Count > 0;
            }
            catch (JsonException)
            {
                return false;
            }
            catch (NotSupportedException)
            {
                return false;
            }
        }

        // Показывает все полученные значения в MessageBox (временная проверка).
        private static void ShowResults(IReadOnlyList<RailcarWeightResultDto> results)
        {
            var builder = new StringBuilder();

            foreach (RailcarWeightResultDto result in results)
            {
                builder.AppendLine($"Номер: {result.Number}");
                builder.AppendLine($"Вес стороны 1: {FormatWeight(result.Side1Weight)}");
                builder.AppendLine($"Вес стороны 2: {FormatWeight(result.Side2Weight)}");
                builder.AppendLine($"Вес стороны 3: {FormatWeight(result.Side3Weight)}");
                builder.AppendLine($"Вес стороны 4: {FormatWeight(result.Side4Weight)}");
                builder.AppendLine($"Общий вес: {FormatWeight(result.TotalWeight)}");
                builder.AppendLine("Веса портов:");

                if (result.PortWeights.Count == 0)
                {
                    builder.AppendLine("  (нет данных)");
                }
                else
                {
                    foreach (KeyValuePair<int, decimal> port in result.PortWeights)
                    {
                        builder.AppendLine($"  Порт {port.Key}: {FormatWeight(port.Value)}");
                    }
                }

                builder.AppendLine($"Начало: {result.StartTime:dd.MM.yyyy HH:mm:ss}");
                builder.AppendLine($"Окончание: {result.EndTime:dd.MM.yyyy HH:mm:ss}");
                builder.AppendLine();
            }

            MessageBox.Show(
                builder.ToString().TrimEnd(),
                "Данные от сервера",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

        }

        private static string FormatWeight(decimal weight)
        {
            return weight.ToString("0.##", CultureInfo.CurrentCulture);
        }

        private void OnConnectionError(Exception exception)
        {
            if (IsDisposed)
            {
                return;
            }

            BeginInvoke(() =>
            {
                // При потере связи форма возвращается в состояние ожидания подключения.
                btnWeighing.Text = "Начать взвешивание";
                MessageBox.Show(exception.Message, "Ошибка сервера", MessageBoxButtons.OK, MessageBoxIcon.Error);
            });
        }

        private async void DynamicWeighing_FormClosing(object? sender, FormClosingEventArgs e)
        {
            // Отписываем обработчики и закрываем соединение перед закрытием формы.
            _tcpService.MessageReceived -= OnMessageReceived;
            _tcpService.ConnectionError -= OnConnectionError;
            await _tcpService.DisconnectAsync();
        }
    }
}

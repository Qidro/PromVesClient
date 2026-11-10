using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Text.Json;
using System.Windows.Forms;
using Microsoft.Extensions.Logging;
using PromVesClient.DTO;
using PromVesClient.Service.TcpService;

namespace PromVesClient
{
    public partial class DynamicWeighing : Form
    {
        // Сервис отвечает за подключение к серверу и получение сообщений по TCP.
        private readonly TcpService _tcpService;

        private readonly ILogger<DynamicWeighing> _logger;

        // Время начала результатов, которые уже были обработаны в текущем сеансе.
        private readonly HashSet<DateTime> _knownResultStartTimes = new();

        // Не показывать пользователю ошибку, если соединение закрывается
        // после завершения динамического взвешивания.
        private bool _isFinishingWeighing;
        // Показывает, что пользователь запустил взвешивание и переподключение разрешено.
        private bool _isWeighingActive;
        // Не допускает запуск нескольких циклов переподключения одновременно.
        private int _reconnectInProgress;
        // Гарантирует одно уведомление за один случай потери связи.
        private int _serverErrorShown;

        // Таймер проверки активности сервера.
        private readonly System.Windows.Forms.Timer _pingTimer = new();
        // Не допускает отправку нового Ping, пока предыдущая проверка ещё выполняется.
        private int _pingInProgress;
        // Ожидание ответа Pong от сервера на текущий Ping.
        private TaskCompletionSource<bool>? _pendingPong;

        // Интервал между проверками доступности сервера.
        private const int PingIntervalMs = 5000;
        // Максимальное время отправки Ping и ожидания ответа Pong.
        private const int PingTimeoutMs = 3000;

        // Настройки разбора JSON: сервер может отдавать свойства как в PascalCase,
        // так и в camelCase, а даты — в ISO 8601 или в старом формате.
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new FlexibleDateTimeConverter() }
        };

        // Инициализирует форму и подписывает её на события TCP-сервиса.
        public DynamicWeighing(TcpService tcpService, ILogger<DynamicWeighing> logger)
        {
            _tcpService = tcpService;
            _logger = logger;
            InitializeComponent();

            _tcpService.MessageReceived += OnMessageReceived;
            _tcpService.ConnectionError += OnConnectionError;
            FormClosing += DynamicWeighing_FormClosing;

            _pingTimer.Interval = PingIntervalMs;
            _pingTimer.Tick += PingTimer_Tick;
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
                    // При новом подключении начинаем отслеживание списка заново.
                    _knownResultStartTimes.Clear();
                    _isFinishingWeighing = false;
                    await _tcpService.ConnectAsync(5004);
                    _isWeighingActive = true;
                    Interlocked.Exchange(ref _serverErrorShown, 0);

                    // Сразу после подключения проверяем, что сервер действительно
                    // активен и отвечает на команду Ping.
                    await CheckServerConnectionAsync();
                    _pingTimer.Start();
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
                // Перед закрытием соединения сообщаем серверу о завершении
                // динамического взвешивания. Сервис выполнит максимум 5 попыток.
                _isFinishingWeighing = true;
                _isWeighingActive = false;
                _pingTimer.Stop();
                await _tcpService.SendMessageAsync("EndDynamic", 5);
                await _tcpService.DisconnectAsync();
                btnWeighing.Text = "Начать взвешивание";
            }
        }

        // Обрабатывает сообщение сервера и добавляет результат в таблицу.
        private void OnMessageReceived(string message)
        {
            _logger.LogInformation("Получено сообщение от сервера."+ message);

            // Ответ Pong подтверждает, что сервер получил Ping и продолжает работать.
            if (string.Equals(message.Trim(), "Pong", StringComparison.OrdinalIgnoreCase))
            {
                SetServerConnectionStatus(Color.Green);
                _pendingPong?.TrySetResult(true);
                return;
            }

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
                // Повторные сообщения с тем же временем начала игнорируются.   
                if (_knownResultStartTimes.Contains(result.StartTime)
                    || IsResultAlreadyAdded(result.StartTime))
                {
                    continue;
                }

                dataGridViewReceipts.Rows.Add(
                    result.StartTime.ToString("dd.MM.yyyy HH:mm:ss"),
                    result.EndTime.ToString("dd.MM.yyyy HH:mm:ss"),
                    FormatWeight(result.TotalWeight));

                // Запоминаем время сразу после добавления, чтобы повтор этого
                // результата в следующем сообщении не создал дубликат.
                _knownResultStartTimes.Add(result.StartTime);
            }
        }

        // Каждые 5 секунд отправляет Ping и ожидает ответ Pong.
        private async void PingTimer_Tick(object? sender, EventArgs e)
        {
            await CheckServerConnectionAsync();
        }

        // Выполняет одну проверку доступности сервера.
        private async Task<bool> CheckServerConnectionAsync()
        {
            // Проверку не запускаем во время завершения или если другая проверка уже идёт.
            if (_isFinishingWeighing
                || !_tcpService.ServerConnected
                || Interlocked.Exchange(ref _pingInProgress, 1) != 0)
            {
                return false;
            }

            var pong = new TaskCompletionSource<bool>(
                TaskCreationOptions.RunContinuationsAsynchronously);
            _pendingPong = pong;

            try
            {
                using var timeoutCts = new CancellationTokenSource(PingTimeoutMs);
                bool sent = await _tcpService.SendMessageAsync(
                    "Ping",
                    1,
                    timeoutCts.Token);

                if (!sent)
                {
                    // Команда не записалась в сокет — соединение считаем недоступным.
                    SetServerConnectionStatus(Color.Red);
                    _logger.LogWarning("Не удалось отправить Ping серверу.");
                    return false;
                }

                try
                {
                    // Отправка успешна, но сервер должен дополнительно подтвердить
                    // активность ответом Pong.
                    await pong.Task.WaitAsync(TimeSpan.FromMilliseconds(PingTimeoutMs));
                    _logger.LogDebug("Сервер подтвердил доступность соединения: Pong.");
                    return true;
                }
                catch (TimeoutException)
                {
                    SetServerConnectionStatus(Color.Red);
                    _logger.LogWarning("Сервер не ответил на Ping в течение 3 секунд.");
                    return false;
                }
            }
            catch (OperationCanceledException)
            {
                SetServerConnectionStatus(Color.Red);
                _logger.LogWarning("Истёк таймаут отправки Ping.");
                return false;
            }
            finally
            {
                if (ReferenceEquals(_pendingPong, pong))
                {
                    _pendingPong = null;
                }

                Interlocked.Exchange(ref _pingInProgress, 0);
            }
        }

        // Переподключается к серверу, пока пользователь не завершил взвешивание.
        private async Task ReconnectToServerAsync()
        {
            // Событие ошибки может прийти несколько раз от разных фоновых операций.
            // Только первый обработчик получает право выполнять переподключение.
            if (Interlocked.Exchange(ref _reconnectInProgress, 1) != 0)
            {
                return;
            }

            try
            {
                while (_isWeighingActive && !_isFinishingWeighing && !IsDisposed)
                {
                    try
                    {
                        // Сначала закрываем старый экземпляр соединения, затем
                        // создаём новое подключение к порту динамического взвешивания.
                        await _tcpService.DisconnectAsync();
                        await _tcpService.ConnectAsync(5004);

                        // Подключение считается восстановленным только после Pong.
                        if (await CheckServerConnectionAsync())
                        {
                            Interlocked.Exchange(ref _serverErrorShown, 0);
                            _pingTimer.Start();
                            return;
                        }

                        await _tcpService.DisconnectAsync();
                    }
                    catch (Exception ex)
                    {
                        // Ошибка отдельной попытки не прекращает цикл восстановления.
                        _logger.LogWarning(ex, "Не удалось переподключиться к серверу.");
                    }

                    // Пауза перед следующей попыткой, чтобы не создавать частые подключения.
                    await Task.Delay(5000);
                }
            }
            finally
            {
                Interlocked.Exchange(ref _reconnectInProgress, 0);
            }
        }

        // Меняет цвет индикатора в UI-потоке, так как Ping обрабатывается асинхронно.
        private void SetServerConnectionStatus(Color color)
        {
            if (IsDisposed)
            {
                return;
            }

            if (InvokeRequired)
            {
                BeginInvoke(() => SetServerConnectionStatus(color));
                return;
            }

            lblConnectServer.BackColor = color;
        }

        // Проверяет, был ли результат с таким временем начала уже добавлен в таблицу.
        private bool IsResultAlreadyAdded(DateTime startTime)
        {
            foreach (DataGridViewRow row in dataGridViewReceipts.Rows)
            {
                if (row.IsNewRow || row.Cells[0].Value is null)
                {
                    continue;
                }

                if (DateTime.TryParseExact(
                        Convert.ToString(row.Cells[0].Value),
                        "dd.MM.yyyy HH:mm:ss",
                        CultureInfo.CurrentCulture,
                        DateTimeStyles.None,
                        out DateTime existingStartTime)
                    && existingStartTime == startTime)
                {
                    return true;
                }
            }

            return false;
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

        private static string FormatWeight(decimal weight)
        {
            return weight.ToString("0.##", CultureInfo.CurrentCulture);
        }

        private void OnConnectionError(Exception exception)
        {
            if (IsDisposed || _isFinishingWeighing || !_isWeighingActive)
            {
                return;
            }

            _pingTimer.Stop();
            SetServerConnectionStatus(Color.Red);

            // Запускаем восстановление в фоне, чтобы не блокировать поток интерфейса.
            _ = ReconnectToServerAsync();

            // Повторные события ошибки до восстановления не показывают новые MessageBox.
            if (Interlocked.Exchange(ref _serverErrorShown, 1) == 0)
            {
                BeginInvoke(() => MessageBox.Show(
                    exception.Message,
                    "Ошибка сервера",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error));
            }
        }

        private async void DynamicWeighing_FormClosing(object? sender, FormClosingEventArgs e)
        {
            // Отписываем обработчики и закрываем соединение перед закрытием формы.
            _isWeighingActive = false;
            _isFinishingWeighing = true;
            _pingTimer.Stop();
            _pingTimer.Tick -= PingTimer_Tick;
            _tcpService.MessageReceived -= OnMessageReceived;
            _tcpService.ConnectionError -= OnConnectionError;
            await _tcpService.DisconnectAsync();
        }
    }
}

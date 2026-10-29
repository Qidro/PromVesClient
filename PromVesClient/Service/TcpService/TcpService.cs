using Microsoft.Extensions.Logging;
using PromVesClient.Service.StaticWeighingService;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace PromVesClient.Service.TcpService
{
    public class TcpService
    {
        private TcpClient? _client;
        private NetworkStream? _stream;
        private CancellationTokenSource? _cts;
        private readonly ILogger<TcpService> _logger;

        //DI
        private readonly CurrentUserService _currentUserService;
        //логи
        //поле подключения к серверу
        public bool ServerConnected { get; private set; }
        public CancellationToken Token =>
    _cts?.Token ?? CancellationToken.None;

        public TcpService(ILogger<TcpService> logger)
        {
            _logger = logger;
        }

        // Устанавливает TCP-соединение, создаёт токен отмены и запускает приём сообщений.
        public async Task ConnectAsync(int port)
        {
            _client = new TcpClient();
            //получение локального IP адреса
            IPAddress ipAddress = await GetLocalIPAddressAsync();
            //подключение к серверу
            await _client.ConnectAsync(ipAddress, port)
                        .WaitAsync(TimeSpan.FromSeconds(5));

            _stream = _client.GetStream();
            _cts = new CancellationTokenSource();
            ServerConnected = true;
            //вызов метода по получению значения с сервера
            _ = ReceiveMessagesAsync(_cts.Token);
        }

        // Находит первый доступный IPv4-адрес компьютера для подключения к серверу.
        public async Task<IPAddress> GetLocalIPAddressAsync()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());

            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                    return ip;
            }

            throw new Exception("Локальный IPv4 адрес не найден.");
        }
        // Отменяет чтение и освобождает ресурсы TCP-соединения.
        public async Task DisconnectAsync()
        {
            _cts?.Cancel();
            _cts?.Dispose();

            _stream?.Dispose();

            _client?.Close();
            _client?.Dispose();
            ServerConnected = false;
            _cts = null;
            _stream = null;
            _client = null;
        }
        // Читает строковые сообщения сервера в фоновом режиме и передаёт их подписчикам.
        public async Task ReceiveMessagesAsync(CancellationToken token)
        {
            try
            {
                using var reader = new StreamReader(
                    _stream,
                    Encoding.UTF8,
                    detectEncodingFromByteOrderMarks: false,
                    leaveOpen: true);

                while (!token.IsCancellationRequested)
                {
                    // Таймаут защищает соединение от бесконечного ожидания данных.
                    using var timeoutCts =
                        CancellationTokenSource.CreateLinkedTokenSource(token);

                    timeoutCts.CancelAfter(TimeSpan.FromSeconds(10));

                    string? message;

                    try
                    {
                        message = await reader.ReadLineAsync(timeoutCts.Token);
                    }
                    catch (OperationCanceledException) when (token.IsCancellationRequested)
                    {
                        // Штатная отмена всего процесса чтения не является ошибкой.
                        _logger.LogInformation(
                            "Получение сообщений от сервера остановлено.");

                        ServerConnected = false;
                        return;
                    }
                    catch (OperationCanceledException)
                    {
                        // Истёк таймаут ожидания очередного сообщения.
                        ServerConnected = false;

                        _logger.LogWarning(
                            "Нет сообщений от сервера в течение 10 секунд.");

                        ConnectionError?.Invoke(
                            new TimeoutException(
                                "Нет сообщений от сервера в течение 10 секунд."));

                        return;
                    }

                    // null означает, что сервер закрыл соединение.
                    if (message is null)
                    {
                        ServerConnected = false;

                        _logger.LogWarning(
                            "Сервер закрыл соединение.");

                        ConnectionError?.Invoke(
                            new IOException(
                                "Сервер закрыл соединение."));

                        return;
                    }

                    // Убираем пробелы и символы перевода строки.
                    message = message.Trim();

                    // Пустые сообщения не содержат данных для обработки.
                    if (string.IsNullOrWhiteSpace(message))
                        continue;

                    _logger.LogDebug(
                        "Получено сообщение от сервера: {Message}",
                        message);

                    MessageReceived?.Invoke(message);
                }
            }
            catch (IOException ex)
            {
                ServerConnected = false;

                // При штатном отключении ошибка не передаётся в UI и не запускает переподключение.
                if (token.IsCancellationRequested)
                {
                    return;
                }

                _logger.LogError(
                    ex,
                    "Ошибка ввода-вывода при получении данных от сервера.");

                ConnectionError?.Invoke(
                    new IOException(
                        "Ошибка ввода-вывода. Соединение с сервером потеряно.",
                        ex));
            }
            catch (ObjectDisposedException ex)
            {
                ServerConnected = false;

                // Поток мог быть закрыт штатной отменой соединения.
                if (token.IsCancellationRequested)
                {
                    return;
                }

                _logger.LogError(
                    ex,
                    "Попытка чтения из закрытого потока.");

                ConnectionError?.Invoke(
                    new ObjectDisposedException(
                        "NetworkStream",
                        "Поток соединения с сервером был закрыт."));
            }
            catch (SocketException ex)
            {
                ServerConnected = false;

                // Не уведомляем подписчиков об ошибке, если соединение закрывается штатно.
                if (token.IsCancellationRequested)
                {
                    return;
                }

                _logger.LogError(
                    ex,
                    "Ошибка сокета: {SocketErrorCode}",
                    ex.SocketErrorCode);

                ConnectionError?.Invoke(
                    new SocketException((int)ex.SocketErrorCode));
            }
            catch (OperationCanceledException) when (token.IsCancellationRequested)
            {
                ServerConnected = false;

                _logger.LogInformation(
                    "Получение сообщений от сервера остановлено.");
            }
            catch (Exception ex)
            {
                ServerConnected = false;

                // Игнорируем исключение, возникшее во время штатной остановки чтения.
                if (token.IsCancellationRequested)
                {
                    return;
                }

                _logger.LogError(
                    ex,
                    "Необработанная ошибка при получении данных от сервера.");

                ConnectionError?.Invoke(
                    new Exception(
                        "Ошибка при получении данных от сервера.",
                        ex));
            }
        }

        // Отправляет команду серверу строкой "команда\n" ("\n" обязателен — разделитель для ReadLineAsync на приёме).
        // При ошибке отправки повторяет попытки каждые 500 мс, пока соединение живо.
        // Возвращает false, если соединения нет или оно закрылось до успешной отправки.
        // ВАЖНО: true означает лишь запись в сокет — подтверждений сервер не шлёт.
        public async Task<bool> SendMessageAsync(string message)
        {
            // Нет соединения (до подключения или после DisconnectAsync).
            if (_client is null || _stream is null || !_client.Connected)
            {
                return false;
            }

            // Кодируем один раз: при повторах содержимое не меняется.
            byte[] data = Encoding.UTF8.GetBytes($"{message}\n");

            // Цикл, пока соединение живо. _client/_stream могут обнулиться в
            // DisconnectAsync параллельно с отправкой, поэтому проверяем на каждой итерации.
            while (ServerConnected
                && _client is not null
                && _stream is not null
                && _client.Connected)
            {
                try
                {
                    // Token прерывает запись при штатном закрытии соединения.
                    await _stream.WriteAsync(data, 0, data.Length, Token);
                    await _stream.FlushAsync(Token);

                    return true;
                }
                catch (OperationCanceledException) when (Token.IsCancellationRequested)
                {
                    // Соединение закрывается штатно — повторять бессмысленно.
                    break;
                }
                catch (IOException ex)
                {
                    // Временный сбой ввода-вывода: повторяем. При окончательном разрыве
                    // ServerConnected станет false в ReceiveMessagesAsync и цикл остановится.
                    _logger.LogWarning(
                        ex,
                        "Не удалось отправить команду серверу, будет повтор: {Message}",
                        message);
                }
                catch (ObjectDisposedException ex)
                {
                    // Поток уже освобождён — повторы невозможны.
                    _logger.LogWarning(
                        ex,
                        "Соединение закрыто во время отправки команды: {Message}",
                        message);

                    break;
                }
                catch (SocketException ex)
                {
                    // Ошибка сокета: повторяем, как и с IOException.
                    _logger.LogWarning(
                        ex,
                        "Ошибка сокета при отправке команды, будет повтор: {Message}",
                        message);
                }

                // Пауза между попытками; при закрытии соединения во время паузы выйдет OCE.
                try
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(500), Token);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }

            // Соединение закрылось раньше, чем команда ушла в сеть.
            _logger.LogError(
                "Команда не доставлена: соединение с сервером закрыто. {Message}",
                message);

            return false;
        }

        // Сообщение от сервера (строка без "\n"); команды обрабатывают формы-подписчики.
        public event Action<string>? MessageReceived;

        // Потеряна связь с сервером (таймаут, обрыв, закрытие сервером); используется для переподключения.
        public event Action<Exception>? ConnectionError;
    }
}


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
        //поле подключения к серверу
        public bool ServerConnected { get; private set; }
        public CancellationToken Token =>
    _cts?.Token ?? CancellationToken.None;

        public TcpService(ILogger<TcpService> logger)
        {
            _logger = logger;
        }

        //подключение к серверу
        public async Task ConnectAsync()
        {
            _client = new TcpClient();
            //получение локального IP адреса
            IPAddress ipAddress = await GetLocalIPAddressAsync();
            //подключение к серверу
            await _client.ConnectAsync(ipAddress, 5002)
                        .WaitAsync(TimeSpan.FromSeconds(5));

            _stream = _client.GetStream();
            _cts = new CancellationTokenSource();
            ServerConnected = true;
            //вызов метода по получению значения с сервера
            _ = ReceiveMessagesAsync(_cts.Token);
        }

        //получение локального ip адреса компьютера
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
        //отключаемся от сервера
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
        //считывание с сервера присланных данных
        // Считывание данных, присланных сервером
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
                    // Таймаут ожидания сообщения — 10 секунд
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
                        // Штатная отмена всего процесса чтения
                        _logger.LogInformation(
                            "Получение сообщений от сервера остановлено.");

                        ServerConnected = false;
                        return;
                    }
                    catch (OperationCanceledException)
                    {
                        // Истек таймаут 10 секунд
                        ServerConnected = false;

                        _logger.LogWarning(
                            "Нет сообщений от сервера в течение 10 секунд.");

                        ConnectionError?.Invoke(
                            new TimeoutException(
                                "Нет сообщений от сервера в течение 10 секунд."));

                        return;
                    }

                    // null означает, что сервер закрыл соединение
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

                    // Убираем пробелы и \r
                    message = message.Trim();

                    // Пустые сообщения игнорируем
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

                _logger.LogError(
                    ex,
                    "Необработанная ошибка при получении данных от сервера.");

                ConnectionError?.Invoke(
                    new Exception(
                        "Ошибка при получении данных от сервера.",
                        ex));
            }
        }
        //обьявление событий
        public event Action<string>? MessageReceived;
        public event Action<Exception>? ConnectionError;
    }
}

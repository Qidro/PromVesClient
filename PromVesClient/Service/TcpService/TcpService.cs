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

            _cts = null;
            _stream = null;
            _client = null;
        }
        //считывание с сервера присланных данных
        public async Task ReceiveMessagesAsync(CancellationToken token)
        {
            try 
            {
                byte[] buffer = new byte[4096];

                while (!token.IsCancellationRequested)
                {
                    //создание токена отмены с задержкой 10 секунд
                    using var timeoutCts =
                    CancellationTokenSource.CreateLinkedTokenSource(token);
                    timeoutCts.CancelAfter(TimeSpan.FromSeconds(10));

                    int count =
                        await _stream.ReadAsync(buffer, timeoutCts.Token);

                    if (count == 0)
                        break;

                    string message = Encoding.UTF8.GetString(buffer, 0, count);
                    //Console.WriteLine("выключаем событие");
                    //
                    MessageReceived?.Invoke(message);
                }
            }
            //сервер разорвал соединение
            catch (IOException ex)
            {
                //MessageBox.Show("Ошибка: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _logger.LogError(ex, "Ошибка ввода-вывода. Сервер разорвал соединение");
                ConnectionError?.Invoke(new Exception("Сервер разорвал соединение", ex));
                return;
            }
            //ошибка потока/работа с закрытым потоком, обьектом которго больше нет
            catch (ObjectDisposedException ex)
            {
                _logger.LogError(ex, "Попытка считывания закрытого потока");
                ConnectionError?.Invoke(new Exception("Сервер разорвал соединение", ex));
                return;
            }
            //ошибка сокета
            catch (SocketException ex)
            {
               // MessageBox.Show("Ошибка", $"Ошибка сокета: {ex.SocketErrorCode}", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _logger.LogError(ex, "Ошибка сокета");
                ConnectionError?.Invoke(new Exception("Сервер разорвал соединение", ex));
                return;
            }
            catch (OperationCanceledException)
            {
                return;
            }
            catch (Exception ex)
            {
                //BeginInvoke(() =>
                //{
                //    //MessageBox.Show(ex.Message);
                //});
                _logger.LogError("Ошибка: " + ex.Message.ToString());
                ConnectionError?.Invoke(new Exception("Сервер разорвал соединение", ex));
                return;
            }
            
        }
        //обьявление событий
        public event Action<string>? MessageReceived;
        public event Action<Exception>? ConnectionError;
    }
}

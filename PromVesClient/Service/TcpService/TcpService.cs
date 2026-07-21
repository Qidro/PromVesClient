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
        public CancellationToken Token =>
    _cts?.Token ?? CancellationToken.None;
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
            byte[] buffer = new byte[4096];

            while (!token.IsCancellationRequested)
            {
                int count = await _stream.ReadAsync(buffer, token);

                if (count == 0)
                    break;

                string message = Encoding.UTF8.GetString(buffer, 0, count);
                //Console.WriteLine("выключаем событие");
                //
                MessageReceived?.Invoke(message);
            }
        }
        //обьявление событий
        public event Action<string>? MessageReceived;
        public event Action<Exception>? ConnectionError;
    }
}

using Microsoft.Extensions.Logging;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PromVesClient.Service
{
    public class PromVesServerService
    {
        private const string ServiceName = "PromVesServerNew";
        private readonly ILogger<PromVesServerService> _logger;
        //private readonly ILogger<PromVesServerService> _logger;

        public PromVesServerService(ILogger<PromVesServerService> logger)
        {
            _logger = logger;
        }
        public async Task RestartAsync()
        {
            using var service = new ServiceController(ServiceName);

            if (service.Status != ServiceControllerStatus.Stopped)
            {
                service.Stop();

                await Task.Run(() =>
                    service.WaitForStatus(
                        ServiceControllerStatus.Stopped,
                        TimeSpan.FromSeconds(30)));
            }

            service.Start();

            await Task.Run(() =>
                service.WaitForStatus(
                    ServiceControllerStatus.Running,
                    TimeSpan.FromSeconds(30)));
        }

        // Метод для перезапуска службы PromVesServer с обработкой ошибок
        public async Task RestartServerAsync()
        {
            try
            {
                // Вызов метода текущего экземпляра вместо несуществующего поля
                await RestartAsync();

                MessageBox.Show(
                    "Служба PromVesServerNew успешно перезапущена.",
                    "Служба сервера",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(
                    $"Служба PromVesServerNew не найдена, либо нет прав доступа",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                _logger.LogError($"Служба PromVesServerNew не найдена, либо нет прав доступа {ex}");
            }
            catch (System.ComponentModel.Win32Exception ex)
            {
                MessageBox.Show(
                    $"Не удалось перезапустить службу.\n\n{ex.Message}",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                _logger.LogError($"Не удалось перезапустить службу {ex}");
            }
            catch (System.TimeoutException ex)
            {
                MessageBox.Show(
                    "Служба не успела запуститься или остановиться за 30 секунд.",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                _logger.LogError($"Служба не успела запуститься или остановиться за 30 секунд {ex}");
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Ошибка при перезапуске службы:\n\n{ex.Message}",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                _logger.LogError($"Ошибка при перезапуске службы: {ex}");
            }
        }
    }
}

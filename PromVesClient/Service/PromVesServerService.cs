using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.Windows.Forms; 

namespace PromVesClient.Service
{
    public class PromVesServerService
    {
        private const string ServiceName = "PromVesServer";

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
                    "Служба PromVesServer успешно перезапущена.",
                    "Служба сервера",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show(
                    "Служба PromVesServer не найдена.",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            catch (System.ComponentModel.Win32Exception ex)
            {
                MessageBox.Show(
                    $"Не удалось перезапустить службу.\n\n{ex.Message}",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            catch (System.TimeoutException)
            {
                MessageBox.Show(
                    "Служба не успела запуститься или остановиться за 30 секунд.",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Ошибка при перезапуске службы:\n\n{ex.Message}",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}

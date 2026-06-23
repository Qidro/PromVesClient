using Microsoft.Extensions.Logging;
using PromVesClient.Service.UserService;
using Serilog;
using Serilog.Core;
using System.Text.Json;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace PromVesClient
{
    public partial class Form1 : Form
    {
        private readonly ILogger<Form1> _logger;
        
        private readonly UserService _userService;
        //в конструкторе открываем файл о версии приложения
        public Form1(ILogger<Form1> logger, UserService userService)
        {
            _logger = logger;
            _userService = userService;
            //        Log.Logger = new LoggerConfiguration()
            //.WriteTo.File("logs/log.txt")
            //.CreateLogger();
            InitializeComponent();
            _logger.LogInformation("Приложение запущено");
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            try
            {
                string json = File.ReadAllText("appinfo.json");

                using var doc = JsonDocument.Parse(json);

                string version = doc.RootElement
                .GetProperty("application")
                .GetProperty("version")
                .GetString();

                programVersion.Text = $"Версия: {version}";
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex,
                    "Некорректный формат appinfo.json");

                programVersion.Text = "Версия: 1.0.0";
            }
            catch (FileNotFoundException ex)
            {
                _logger.LogError("Файл appinfo.json не найден");
                programVersion.Text = $"Версия: 1.0.0";
            }
            catch (Exception ex)
            {
                _logger.LogError("Неизвестная ошибка:", ex.ToString() );
                programVersion.Text = $"Версия: 1.0.0";
            }
            
        }
        
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private async void button1_Click(object sender, EventArgs e)
        {
            //метод авторизации, временно закоменчен
            var result = await _userService.userAuthorizationAsync(textBoxLogin.Text, textBoxPassword.Text);

            //временный метод создания пользователя
            //var result = await _userService.createUserAsync(textBoxLogin.Text, textBoxPassword.Text);
            //результат авторизации
            if (result.Success == true)
            {
                MessageBox.Show("успешно", result.Data.PasswordHash);
            }
            else
            {
                MessageBox.Show(
                result.Message,
                "Ошибка авторизации",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
            }
        }

        private void programVersion_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}

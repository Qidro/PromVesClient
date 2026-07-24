using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PromVesClient.Service;
using PromVesClient.Service.AppInfoService;
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
        private readonly CurrentUserService _currentUserService;
        private readonly IServiceProvider _serviceProvider;
        private readonly AppInfoService _appInfoService;
        //в конструкторе открываем файл о версии приложения
        public Form1(ILogger<Form1> logger, UserService userService, CurrentUserService currentUserService, IServiceProvider serviceProvider, AppInfoService appInfoService)
        {
            _logger = logger;
            _userService = userService;
            _currentUserService = currentUserService;
            _serviceProvider = serviceProvider;
            _appInfoService = appInfoService;
            //        Log.Logger = new LoggerConfiguration()
            //.WriteTo.File("logs/log.txt")
            //.CreateLogger();
            InitializeComponent();
            _logger.LogInformation("Приложение запущено");
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            programVersion.Text = _appInfoService.VersionInfo();
            //programVersion.Text = "Версия: 1.0.0";

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
            var result = await _userService.UserAuthorizationAsync(textBoxLogin.Text, textBoxPassword.Text);

            //временный метод создания пользователя
            //var result = await _userService.createUserAsync(textBoxLogin.Text, textBoxPassword.Text);
            //результат авторизации
            if (result.Success == true)
            {
                _currentUserService.Login(result.Data!);
                //MessageBox.Show("успешно", result.Data.PasswordHash);
                var form = _serviceProvider.GetRequiredService<MainMenu>();
                //var form = new MainMenu();

                form.ShowDialog();

                //this.Hide();
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

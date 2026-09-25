using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PromVesClient.Service;
using PromVesClient.Service.AppInfoService;
using PromVesClient.Service.ConfigSevice;
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
        private readonly ConfigService _configService;

        private int CountGraphs;
        //в конструкторе открываем файл о версии приложения
        public Form1(ILogger<Form1> logger, UserService userService, CurrentUserService currentUserService, IServiceProvider serviceProvider
            , AppInfoService appInfoService
            , ConfigService configService)
        {
            InitializeComponent();
            _logger = logger;
            _userService = userService;
            _currentUserService = currentUserService;
            _serviceProvider = serviceProvider;
            _appInfoService = appInfoService;
            _configService = configService;
            //        Log.Logger = new LoggerConfiguration()
            //.WriteTo.File("logs/log.txt")
            //.CreateLogger();
            this.Shown += Form1_Shown;
            _logger.LogInformation("Приложение запущено");
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            programVersion.Text = _appInfoService.VersionInfo();
            //programVersion.Text = "Версия: 1.0.0";

        }
        //загрузка предварительных данных 
        private async void Form1_Load(object sender, EventArgs e)
        {

           // await loadConfig();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
        private async void Form1_Shown(object sender, EventArgs e)
        {
            await loadConfig();
        }
        //загрузка данных количества графиков
        private async Task loadConfig()
        {
            //получаем результат
            var result = await _configService.GetGraphsCountAsync();
            //проверка на успешность
            if (result.Success == true)
            {
                CountGraphs = result.Data;
            }
            else 
            {
                MessageBox.Show($"Произошла ошибка, причина: {result.Message}","Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
                _currentUserService.Login(result.Data!, CountGraphs);
                //MessageBox.Show("успешно", result.Data.PasswordHash);
                var form = _serviceProvider.GetRequiredService<MainMenu>();
                //var form = new MainMenu();
                this.Hide();
                form.ShowDialog();
                this.Show();
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

using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using System.Text.Json;

namespace PromVesClient
{
    public partial class Form1 : Form
    {
        private readonly ILogger<Form1> _logger;
        //в конструкторе открываем файл о версии приложения
        public Form1(ILogger<Form1> logger)
        {
            _logger = logger;
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

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void programVersion_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}

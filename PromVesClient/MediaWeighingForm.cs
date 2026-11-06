using System;
using System.Drawing;
using System.Globalization;
using System.Diagnostics;
using PromVesClient.DTO;
using PromVesClient.Service.ReceiptsService;

namespace PromVesClient
{
    public partial class MediaWeighingForm : Form
    {
        // Сервис загружает из БД данные выбранного взвешивания и связанные медиафайлы.
        private readonly ReceiptsService _receiptsService;
        // Id записи Weighing, выбранной в таблице карточек вагонов.
        private Guid? _weighingId;
        // Полный путь к видео, используемый кнопкой открытия видео.
        private string? _videoPath;
        // Пути к каталогам, в которых хранятся фото и видео.
        private string? _photoFolderPath;
        private string? _videoFolderPath;

        // Форма получает ReceiptsService через DI-контейнер.
        public MediaWeighingForm(ReceiptsService receiptsService)
        {
            _receiptsService = receiptsService;
            InitializeComponent();
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxWagon.SizeMode = PictureBoxSizeMode.Zoom;
        }

        // Вызывается формой квитанций до открытия окна медиа.
        // После установки Id форма может загрузить данные при событии Load.
        public void SetWeighingId(Guid weighingId)
        {
            _weighingId = weighingId;
        }

        private async void MediaWeighingForm_Load(object sender, EventArgs e)
        {
            // Без Id невозможно определить, для какого взвешивания загружать медиа.
            if (_weighingId == null)
            {
                MessageBox.Show("Не выбрано взвешивание.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Запрос выполняется после открытия формы, чтобы не блокировать интерфейс.
            var result = await _receiptsService.GetMediaWeighingAsync(_weighingId.Value);
            if (!result.Success || result.Data == null)
            {
                MessageBox.Show(result.Message, "Ошибка загрузки медиа", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Передаём полученные данные в отдельный метод заполнения элементов формы.
            ShowMediaData(result.Data);
        }

        // Заполняет поля формы и подготавливает пути к фото и видео.
        private void ShowMediaData(MediaWeighingDto media)
        {
            textBoxVagonNumber.Text = media.VagonNumber;
            textBoxDataTime.Text = media.WeighingDateTime.ToString("dd.MM.yyyy HH:mm:ss");
            textBoxPhotoName.Text = media.PhotoName;
            textBoxVideoName.Text = media.VideoName;
            labelNameVideo.Text = media.VideoName;

            // Путь сохраняется в поле, чтобы его можно было использовать при нажатии кнопки видео.
            _videoPath = BuildMediaPath(media.VideoPath, media.VideoName);
            _photoFolderPath = media.PhotoPath;
            _videoFolderPath = media.VideoPath;
            // Сервер формирует имя видео с датой и временем в начале имени файла.
            DateTime? videoDateTime = GetVideoDateTime(media.VideoName);
            labelDateVideo.Text = videoDateTime.HasValue
                ? $"Дата видео: {videoDateTime.Value:dd.MM.yyyy}"
                : "Дата видео: не определена";

            // Если видео найдено на диске, показываем фактическую дату создания файла.
            if (!string.IsNullOrWhiteSpace(_videoPath) && File.Exists(_videoPath))
            {
                labelTimeVideo.Text = $"Создано: {File.GetCreationTime(_videoPath):dd.MM.yyyy HH:mm:ss}";
            }
            // Если файл недоступен, используем дату, разобранную из имени видео.
            else if (videoDateTime.HasValue)
            {
                labelTimeVideo.Text = $"Создано: {videoDateTime.Value:dd.MM.yyyy HH:mm:ss}";
            }
            else
            {
                labelTimeVideo.Text = "Создано: не определено";
            }

            // Отсутствие фотографии не является ошибкой загрузки всей формы.
            ShowPhoto(media.PhotoPath, media.PhotoName);
        }

        // Объединяет путь к каталогу и имя файла. Возвращает null при неполных данных.
        private static string? BuildMediaPath(string? mediaPath, string? fileName)
        {
            if (string.IsNullOrWhiteSpace(mediaPath) || string.IsNullOrWhiteSpace(fileName))
            {
                return null;
            }

            return Path.Combine(mediaPath, fileName);
        }

        private void ShowPhoto(string photoPath, string photoName)
        {
            string? fullPath = BuildMediaPath(photoPath, photoName);
            // Путь из БД может указывать на недоступный каталог или отсутствующий файл.
            if (string.IsNullOrWhiteSpace(fullPath) || !File.Exists(fullPath))
            {
                pictureBoxWagon.Image = null;
                return;
            }

            // Копируем изображение в Bitmap, чтобы освободить блокировку исходного файла.
            using var image = Image.FromFile(fullPath);
            pictureBoxWagon.Image = new Bitmap(image);
        }

        // Разбирает дату из имени видео вида yyyyMMdd_HHmmss_fff_номер.mp4.
        private static DateTime? GetVideoDateTime(string videoName) 
        {
            if (string.IsNullOrWhiteSpace(videoName))
            {
                return null;
            }

            string[] parts = Path.GetFileNameWithoutExtension(videoName).Split('_');
            if (parts.Length < 3)
            {
                return null;
            }

            return DateTime.TryParseExact(
                $"{parts[0]}_{parts[1]}_{parts[2]}",
                "yyyyMMdd_HHmmss_fff",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateTime result)
                ? result
                : null;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Открываем видео средствами ОС через приложение по умолчанию.
            string? path = _videoPath;
            if (!string.IsNullOrWhiteSpace(path) && File.Exists(path))
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = path,
                    UseShellExecute = true
                });
            }
            else
            {
                MessageBox.Show("Видеофайл не найден:\n" + (path ?? "Путь не указан"));
            }
        }

        private void btnReadZeroCode_Click(object sender, EventArgs e)
        {
            // Открываем каталог, указанный для фотографии в записи MediaWeighing.
            OpenMediaFolder(_photoFolderPath, "Папка с фотографиями не найдена.");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Открываем каталог, указанный для видео в записи MediaWeighing.
            OpenMediaFolder(_videoFolderPath, "Папка с видео не найдена.");
        }

        private static void OpenMediaFolder(string? folderPath, string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(folderPath) || !Directory.Exists(folderPath))
            {
                MessageBox.Show(errorMessage, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Process.Start(new ProcessStartInfo
            {
                FileName = folderPath,
                UseShellExecute = true
            });
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PromVesClient.Models;

namespace PromVesClient.Service
{
    // Сервис сохраняет сведения о фото и видео, связанные с конкретным взвешиванием.
    public class PhotofixationService
    {
        // Временный общий путь для фото и видео.
        // При подключении реального хранилища значение следует заменить настройкой.
        private const string PhotoPath = @"C:\PromVesNew\PromVesServer\PhotoWagon";
        private const string VideoPath = @"C:\PromVesNew\PromVesServer\СameraVideo";

        // Контекст БД используется для добавления записи MediaWeighing.
        private readonly ApplicationDbContext _dbContext;
        // Логгер сохраняет подробности ошибок, которые не нужно показывать пользователю.
        private readonly ILogger<PhotofixationService> _logger;

        // Зависимости сервиса передаются через DI-контейнер приложения.
        public PhotofixationService(
            ApplicationDbContext dbContext,
            ILogger<PhotofixationService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        // Сохраняет имена медиафайлов и связывает их с записью взвешивания.
        // WeighingId должен принадлежать уже сохранённой записи Weighing.
        public async Task<ServiceResult> SaveAsync(
            Guid weighingId,
            string? fileNamePhoto,
            string? fileNameVideo)
        {
            // Если сервер не вернул имя файла, сохраняем пустую строку,
            // чтобы запись MediaWeighing всё равно была создана.
            var mediaWeighing = new MediaWeighing
            {
                Id = Guid.NewGuid(),
                WeighingId = weighingId,
                FileNamePhoto = fileNamePhoto ?? string.Empty,
                FilePathPhoto = PhotoPath,
                FileNameVideo = fileNameVideo ?? string.Empty,
                FilePathVideo = VideoPath
            };

            try
            {
                // Сохраняем одну запись с данными фото и видео.
                _dbContext.MediaWeighings.Add(mediaWeighing);
                await _dbContext.SaveChangesAsync();
                return ServiceResult.Ok();
            }
            catch (DbUpdateException ex)
            {
                // Ошибка ограничений или подключения при выполнении SQL-запроса.
                _logger.LogError(ex, "Ошибка сохранения фотофиксации для взвешивания {WeighingId}.", weighingId);
                return ServiceResult.Fail("Ошибка сохранения фотофиксации в БД: " + ex.Message);
            }
            catch (Exception ex)
            {
                // Непредвиденная ошибка при формировании или сохранении записи.
                _logger.LogError(ex, "Ошибка сохранения фотофиксации для взвешивания {WeighingId}.", weighingId);
                return ServiceResult.Fail("Ошибка сохранения фотофиксации в БД: " + ex.Message);
            }
        }
    }
}

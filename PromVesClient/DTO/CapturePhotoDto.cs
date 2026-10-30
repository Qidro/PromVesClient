namespace PromVesClient.DTO
{
    //Команда серверу на фотофиксацию взвешивания (канал команд, порт 5005)
    public class CapturePhotoRequestDto
    {
        //имя команды
        public string command { get; set; } = "CapturePhoto";
        //уникальный идентификатор запроса для сверки ответа
        public string requestId { get; set; } = string.Empty;
    }

    //Ответ сервера на команду CapturePhoto
    public class CapturePhotoResponseDto
    {
        //признак успешного выполнения команды
        public bool success { get; set; }
        //идентификатор запроса, на который отвечает сервер
        public string? requestId { get; set; }
        //имя сохранённого файла фотофиксации
        public string? fileName { get; set; }
    }
}

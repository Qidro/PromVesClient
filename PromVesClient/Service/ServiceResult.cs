using System;
using System.Collections.Generic;
using System.Text;

namespace PromVesClient.Service
{
    //класс предназначен для унифицированного способа (общий подход или принцип, которые помогают привести что-либо к единому стандартизированному виду)
    //введения результатов всех Service программы
    public class ServiceResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;

        public void Fail()
        {
            
        }
        //указывает, что метод успешно завершен без ошибки
        public static ServiceResult Ok() =>
            new() { Success = true};
        //указывает, что метод завершен с ошибкой, сохраняет текст ошибки в поле Message
        public static ServiceResult Fail(string msg) =>
            new() { Success = false, Message = msg };
    }
    //если предстоить передавать какие нибуь обьекты, например обьект класса User
    public class ServiceResult<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }

        public static ServiceResult<T> Ok(T data) =>
            new() { Success = true, Data = data };

        public static ServiceResult<T> Fail(string message) =>
            new() { Success = false, Message = message };
    }
}

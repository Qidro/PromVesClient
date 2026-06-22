using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
namespace PromVesClient.Service.UserService
{
    public class HashPasswordService
    {
        //private readonly ILogger<HashPasswordService> _logger;
        public HashPasswordService()
        { 
        }
        //создание хэша пароля
        public string getHashPasswordUser(string passwordUser)
        {
            string hash = BCrypt.Net.BCrypt.HashPassword(passwordUser);
            return hash;
        }
        //проверка пароля на соотвествие хэша
        public bool passwordСheck(string password, string hashPassword)
        {
            bool ok = BCrypt.Net.BCrypt.Verify(
            password,
            hashPassword);
            return ok;
        }
    }
}

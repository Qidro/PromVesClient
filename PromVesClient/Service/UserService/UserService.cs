using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace PromVesClient.Service.UserService
{
    public class UserService
    {
        private readonly ILogger<UserService> _logger;

        private readonly ApplicationDbContext _dbcontext;

        private readonly HashPasswordService _hashPasswordService;
        public UserService(ILogger<UserService> logger, ApplicationDbContext dbcontext, HashPasswordService hashPasswordService)
        {
            _logger = logger;
            _dbcontext = dbcontext;
            _hashPasswordService = hashPasswordService;
        }

        public async Task<ServiceResult> userAuthorizationAsync(string userName, string password)
        {
            try
            {
                //проверка: пустой ли userName
                if (string.IsNullOrWhiteSpace(userName))
                    return ServiceResult.Fail("Логин пустой");
                // поиск пользователя
                var user = await _dbcontext.Users.FirstOrDefaultAsync(u => u.Name == userName);

                
                //проверяем результат поиска
                if (user == null)
                    return ServiceResult.Fail("Пользователь не найден");
                
                //проверка введенего пароля пользователя
                if (!_hashPasswordService.passwordСheck(password ,user.PasswordHash))
                {
                    return ServiceResult.Fail("Неверный пароль");
                }

                return ServiceResult.Ok();
            }
            catch (Exception ex)
            {
                return ServiceResult.Fail("Неизвестная ошибка: "+ ex.ToString());
            }
            
        }
        //метод создания пользователя
        public async Task<ServiceResult> createUserAsync(string login, string password)
        {
            //проверяет пустые ли строки лоигна и пароля
            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                return ServiceResult.Fail("Логин или пароль пустой");
            }
            
            try
            {
                string hashPassword = _hashPasswordService.getHashPasswordUser(password);
                
                return ServiceResult.Ok();
            }
            catch (Exception ex)
            {
                return ServiceResult.Fail("Неизвестная ошибка: "+ex.ToString());
            }

            
        }
    }
}

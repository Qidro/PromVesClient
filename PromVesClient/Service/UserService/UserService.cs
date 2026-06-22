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
        public UserService(ILogger<UserService> logger, ApplicationDbContext dbcontext)
        {
            _logger = logger;
            _dbcontext = dbcontext;
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
                
                return ServiceResult.Ok();
            }
            catch (Exception ex)
            {
                return ServiceResult.Fail("Неизвестная ошибка: "+ ex.ToString());
            }
            
        }

        //public 
    }
}

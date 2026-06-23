using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using PromVesClient.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PromVesClient.Service.UserService
{
    public class UserService
    {
        private readonly ILogger<UserService> _logger;

        private readonly ApplicationDbContext _dbContext;

        private readonly HashPasswordService _hashPasswordService;
        public UserService(ILogger<UserService> logger, ApplicationDbContext dbcontext, HashPasswordService hashPasswordService)
        {
            _logger = logger;
            _dbContext = dbcontext;
            _hashPasswordService = hashPasswordService;
        }

        public async Task<ServiceResult<User>> userAuthorizationAsync(string userName, string password)
        {
            try
            {
                //проверка: пустой ли userName
                if (string.IsNullOrWhiteSpace(userName))
                    return ServiceResult<User>.Fail("Логин пустой");
                // поиск пользователя
                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Name == userName);

                
                //проверяем результат поиска
                if (user == null)
                    return ServiceResult<User>.Fail("Пользователь не найден");
                
                //проверка введенего пароля пользователя
                if (!_hashPasswordService.passwordСheck(password ,user.PasswordHash))
                {
                    return ServiceResult<User>.Fail("Неверный пароль");
                }

                return ServiceResult<User>.Ok(user);
            }
            catch (Exception ex)
            {
                return ServiceResult<User>.Fail("Неизвестная ошибка: "+ ex.ToString());
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
                var user = new User
                {
                    Id = Guid.NewGuid(),
                    Name = login,
                    PasswordHash = _hashPasswordService.getHashPasswordUser(password)
                };
                _dbContext.Users.Add(user);
                await _dbContext.SaveChangesAsync();
                //string hashPassword = _hashPasswordService.getHashPasswordUser(password);

                return ServiceResult.Ok();
            }
            catch (DbUpdateException ex)
            {
                return ServiceResult.Fail("Неизвестная ошибка: " + ex.ToString());
            }
            catch (TimeoutException ex)
            {
                return ServiceResult.Fail("Неизвестная ошибка: " + ex.ToString());
            }
            catch (NpgsqlException ex)
            {
                return ServiceResult.Fail("Неизвестная ошибка: " + ex.ToString());
            }
            catch (Exception ex)
            {
                return ServiceResult.Fail("Неизвестная ошибка: "+ex.ToString());
            }

            
        }
    }
}

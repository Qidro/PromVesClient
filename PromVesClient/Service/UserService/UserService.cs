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
        public async Task<ServiceResult> createUserAsync(string login, string password, string role)
        {
            //проверяет пустые ли строки лоигна и пароля
            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                return ServiceResult.Fail("Логин или пароль пустой");
            }
            //проверка есть ли уже такой пользователь
            if (await UserExistsAsync(login))
                return ServiceResult.Fail("Пользователь уже существует.");

            try
            {
                var user = new User
                {
                    Id = Guid.NewGuid(),
                    Name = login,
                    Role = role,
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
        //метод получения всех пользователей
        public async Task<List<User>> GetUsersAsync()
        {
            return await _dbContext.Users
                .OrderBy(u => u.Name)
                .ToListAsync();
        }
        // проверка существования пользователя
        public async Task<bool> UserExistsAsync(string login)
        {
            return await _dbContext.Users
                .AnyAsync(u => u.Name == login);
        }
        //получения пользователя по id
        public async Task<User?> GetUserAsync(Guid id)
        {
            return await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Id == id);
        }
        //метод удаления пользователя
        public async Task<ServiceResult> DeleteUserAsync(Guid id)
        {
            try
            {
                var user = await _dbContext.Users.FindAsync(id);

                if (user == null)
                    return ServiceResult.Fail("Пользователь не найден.");

                _dbContext.Users.Remove(user);

                await _dbContext.SaveChangesAsync();

                return ServiceResult.Ok();
            }
            catch (Exception ex)
            {
                return ServiceResult.Fail(ex.Message);
            }
        }
        //изменение пользователя
        public async Task<ServiceResult> UpdateUserAsync(
    Guid id,
    string login,
    string password,
    string role)
        {
            try
            {
                var user = await _dbContext.Users.FindAsync(id);

                if (user == null)
                    return ServiceResult.Fail("Пользователь не найден.");

                var userWithSameName = await _dbContext.Users
    .FirstOrDefaultAsync(u => u.Name == login && u.Id != id);

                if (userWithSameName != null)
                    return ServiceResult.Fail("Пользователь уже существует.");

                user.Name = login;
                user.Role = role;

                if (!string.IsNullOrWhiteSpace(password))
                {
                    user.PasswordHash =
                        _hashPasswordService.getHashPasswordUser(password);
                }

                await _dbContext.SaveChangesAsync();

                return ServiceResult.Ok();
            }
            catch (Exception ex)
            {
                return ServiceResult.Fail(ex.Message);
            }
        }
    }
}

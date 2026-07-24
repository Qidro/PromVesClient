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
        private readonly IDbContextFactory<ApplicationDbContext> _dbContext;
        //private readonly ApplicationDbContext _dbContext;

        private readonly HashPasswordService _hashPasswordService;
        public UserService(ILogger<UserService> logger, IDbContextFactory<ApplicationDbContext> dbcontext, HashPasswordService hashPasswordService)
        {
            _logger = logger;
            _dbContext = dbcontext;
            _hashPasswordService = hashPasswordService;
        }

        public async Task<ServiceResult<User>> UserAuthorizationAsync(string userName, string password)
        {
            try
            {
                await using var db = await _dbContext.CreateDbContextAsync();
                //проверка: пустой ли userName
                if (string.IsNullOrWhiteSpace(userName))
                    return ServiceResult<User>.Fail("Логин пустой");
                // поиск пользователя
                var user = await db.Users.FirstOrDefaultAsync(u => u.Name == userName);

                
                //проверяем результат поиска
                if (user == null)
                    return ServiceResult<User>.Fail("Пользователь не найден");
                // проверка активности пользователя
                if (!user.IsActive)
                {
                    return ServiceResult<User>.Fail("Пользователь неактивен. Обратитесь к администратору.");
                }

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
        public async Task<ServiceResult> CreateUserAsync(string login, string password, string role)
        {
            //проверяет пустые ли строки лоигна и пароля
            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                return ServiceResult.Fail("Логин или пароль пустой");
            }
            //проверка есть ли уже такой пользователь
            var reasultSearchUser = await UserExistsAsync(login);
            if (reasultSearchUser.Success == false)
            {
                return ServiceResult.Fail($"{reasultSearchUser.Message}");
            }
                

            try
            {
                var user = new User
                {
                    Id = Guid.NewGuid(),
                    Name = login,
                    Role = role,
                    PasswordHash = _hashPasswordService.getHashPasswordUser(password)
                };
                await using var db = await _dbContext.CreateDbContextAsync();

                db.Users.Add(user);
                await db.SaveChangesAsync();
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
        public async Task<ServiceResult<List<User>>> GetUsersAsync()
        {
            try
            {
                await using var db = await _dbContext.CreateDbContextAsync();

                var users = await db.Users
                    .AsNoTracking()
                    .OrderBy(u => u.Name)
                    .ToListAsync();

                return ServiceResult<List<User>>.Ok(users);
            }
            catch (TimeoutException ex)
            {
                _logger.LogError(ex, "Ошибка получения списка пользователей");
                return ServiceResult<List<User>>.Fail("Неизвестная ошибка: " + ex.Message);
            }
            catch (NpgsqlException ex)
            {
                _logger.LogError(ex, "Ошибка получения списка пользователей");
                return ServiceResult<List<User>>.Fail("Неизвестная ошибка: " + ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка получения списка пользователей");

                return ServiceResult<List<User>>.Fail("Не удалось получить список пользователей" + ex.Message);
            }
        }
        // проверка существования пользователя
        public async Task<ServiceResult> UserExistsAsync(string login)
        {
            try
            {
                await using var db = await _dbContext.CreateDbContextAsync();

                var user =  await db.Users
                    .AnyAsync(u => u.Name == login);
                if (user == null)
                {
                    return ServiceResult.Ok();
                }
                else
                {
                    return ServiceResult.Fail("Пользователь существует");
                }
            }
            catch (TimeoutException ex)
            {
                _logger.LogError(ex, "Ошибка получения списка пользователей");
                return ServiceResult.Fail("Неизвестная ошибка: " + ex.Message);
            }
            catch (NpgsqlException ex)
            {
                _logger.LogError(ex, "Ошибка получения списка пользователей");
                return ServiceResult.Fail("Неизвестная ошибка: " + ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка получения списка пользователей");

                return ServiceResult.Fail("Не удалось получить список пользователей" + ex.Message);
            }
        }
        //получения пользователя по id
        public async Task<User?> GetUserAsync(Guid id)
        {
            await using var db = await _dbContext.CreateDbContextAsync();

            var user = await db.Users.FindAsync(id);

            return user;
        }
        //метод удаления пользователя
        public async Task<ServiceResult> DeleteUserAsync(Guid id)
        {
            try
            {
                await using var db = await _dbContext.CreateDbContextAsync();

                var user = await db.Users.FindAsync(id);

                if (user == null)
                    return ServiceResult.Fail("Пользователь не найден.");

                db.Users.Remove(user);

                await db.SaveChangesAsync();

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
    string role,
    bool isActive)
        {
            try
            {
                await using var db = await _dbContext.CreateDbContextAsync();

                var user = await db.Users.FindAsync(id);

                if (user == null)
                    return ServiceResult.Fail("Пользователь не найден.");

                var userWithSameName =
     await db.Users
     .FirstOrDefaultAsync(u => u.Name == login && u.Id != id);

                if (userWithSameName != null)
                    return ServiceResult.Fail("Пользователь уже существует.");

                user.Name = login;
                user.Role = role;
                user.IsActive = isActive;

                if (!string.IsNullOrWhiteSpace(password))
                {
                    user.PasswordHash =
                        _hashPasswordService.getHashPasswordUser(password);
                }

                await db.SaveChangesAsync();

                return ServiceResult.Ok();
            }
            catch (Exception ex)
            {
                return ServiceResult.Fail(ex.Message);
            }
        }
        

    }
}

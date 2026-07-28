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
        private readonly CurrentUserService _currentUserService;
        //private readonly ApplicationDbContext _dbContext;

        private readonly HashPasswordService _hashPasswordService;
        public UserService(ILogger<UserService> logger, IDbContextFactory<ApplicationDbContext> dbcontext, HashPasswordService hashPasswordService, CurrentUserService currentUserService)
        {
            _logger = logger;
            _dbContext = dbcontext;
            _hashPasswordService = hashPasswordService;
            _currentUserService = currentUserService;
        }

        public async Task<ServiceResult<User>> UserAuthorizationAsync(string userName, string password)
        {
            try
            {
                await using var db = await _dbContext.CreateDbContextAsync();

                // Проверка логина
                if (string.IsNullOrWhiteSpace(userName))
                    return ServiceResult<User>.Fail("Логин пустой");

                // Поиск пользователя
                var user = await db.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Name == userName);

                if (user == null)
                    return ServiceResult<User>.Fail("Пользователь не найден.");

                // Проверка активности
                if (!user.IsActive)
                    return ServiceResult<User>.Fail("Пользователь неактивен. Обратитесь к администратору.");

                // Проверка пароля
                if (!_hashPasswordService.passwordСheck(password, user.PasswordHash))
                    return ServiceResult<User>.Fail("Неверный пароль.");

                return ServiceResult<User>.Ok(user);
            }
            catch (TimeoutException ex)
            {
                _logger.LogError(ex, "Превышено время ожидания при авторизации пользователя {UserName}.", userName);

                return ServiceResult<User>.Fail("Превышено время ожидания при обращении к базе данных.");
            }
            catch (NpgsqlException ex)
            {
                _logger.LogError(ex, "Ошибка базы данных при авторизации пользователя {UserName}.", userName);

                return ServiceResult<User>.Fail("Ошибка базы данных.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Неизвестная ошибка при авторизации пользователя {UserName}.", userName);

                return ServiceResult<User>.Fail("Не удалось выполнить авторизацию.");
            }
        }

        // метод создания пользователя
        public async Task<ServiceResult> CreateUserAsync(string login, string password, string role)
        {
            try
            {
                // Проверка пустых полей
                if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
                    return ServiceResult.Fail("Логин или пароль пустой.");

                // Проверка существования пользователя
                var resultSearchUser = await UserExistsAsync(login);
                if (!resultSearchUser.Success)
                    return ServiceResult.Fail(resultSearchUser.Message);

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

                return ServiceResult.Ok();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Ошибка сохранения пользователя {Login}.", login);

                return ServiceResult.Fail("Не удалось сохранить пользователя.");
            }
            catch (TimeoutException ex)
            {
                _logger.LogError(ex, "Превышено время ожидания при создании пользователя {Login}.", login);

                return ServiceResult.Fail("Превышено время ожидания при создании пользователя.");
            }
            catch (NpgsqlException ex)
            {
                _logger.LogError(ex, "Ошибка базы данных при создании пользователя {Login}.", login);

                return ServiceResult.Fail("Ошибка базы данных.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Неизвестная ошибка при создании пользователя {Login}.", login);

                return ServiceResult.Fail("Не удалось создать пользователя.");
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
                if (user == false)
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
            try
            {
                await using var db = await _dbContext.CreateDbContextAsync();

                return await db.Users.FindAsync(id);
            }
            catch (TimeoutException ex)
            {
                _logger.LogError(ex, "Ошибка получения пользователя {Id}: превышено время ожидания.", id);
                return null;
            }
            catch (NpgsqlException ex)
            {
                _logger.LogError(ex, "Ошибка базы данных при получении пользователя {Id}.", id);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Неизвестная ошибка при получении пользователя {Id}.", id);
                return null;
            }
        }
        //метод удаления пользователя
        public async Task<ServiceResult> DeleteUserAsync(Guid id)
        {
            try
            {
                
                if (_currentUserService.CurrentUser?.Id == id)
                {
                    return ServiceResult.Fail("Нельзя удалить текущего пользователя.");
                }
                await using var db = await _dbContext.CreateDbContextAsync();

                var user = await db.Users.FindAsync(id);

                if (user == null)
                    return ServiceResult.Fail("Пользователь не найден.");

                db.Users.Remove(user);

                await db.SaveChangesAsync();

                return ServiceResult.Ok();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Ошибка при удалении пользователя {Id}.", id);

                return ServiceResult.Fail("Не удалось удалить пользователя.");
            }
            catch (TimeoutException ex)
            {
                _logger.LogError(ex, "Превышено время ожидания при удалении пользователя {Id}.", id);

                return ServiceResult.Fail("Превышено время ожидания при удалении пользователя.");
            }
            catch (NpgsqlException ex)
            {
                _logger.LogError(ex, "Ошибка базы данных при удалении пользователя {Id}.", id);

                return ServiceResult.Fail("Ошибка базы данных.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Неизвестная ошибка при удалении пользователя {Id}.", id);

                return ServiceResult.Fail("Не удалось удалить пользователя.");
            }
        }
        // изменение пользователя
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

                var userWithSameName = await db.Users
                    .FirstOrDefaultAsync(u => u.Name == login && u.Id != id);

                if (userWithSameName != null)
                    return ServiceResult.Fail("Пользователь уже существует.");

                user.Name = login;
                user.Role = role;
                user.IsActive = isActive;

                if (!string.IsNullOrWhiteSpace(password))
                {
                    user.PasswordHash = _hashPasswordService.getHashPasswordUser(password);
                }

                await db.SaveChangesAsync();

                return ServiceResult.Ok();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Ошибка обновления пользователя {Id}.", id);

                return ServiceResult.Fail("Не удалось сохранить изменения.");
            }
            catch (TimeoutException ex)
            {
                _logger.LogError(ex, "Превышено время ожидания при обновлении пользователя {Id}.", id);

                return ServiceResult.Fail("Превышено время ожидания при обновлении пользователя.");
            }
            catch (NpgsqlException ex)
            {
                _logger.LogError(ex, "Ошибка базы данных при обновлении пользователя {Id}.", id);

                return ServiceResult.Fail("Ошибка базы данных.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Неизвестная ошибка при обновлении пользователя {Id}.", id);

                return ServiceResult.Fail("Не удалось обновить пользователя.");
            }
        }
    }
}

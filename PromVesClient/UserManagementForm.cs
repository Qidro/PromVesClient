using Microsoft.Extensions.DependencyInjection;
using PromVesClient.Models;
using PromVesClient.Service.UserService;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PromVesClient
{
    public partial class UserManagementForm : Form
    {
        private readonly UserService _userService;
        private readonly IServiceProvider _serviceProvider;
        public List<User> userList;
        public UserManagementForm(UserService userService, IServiceProvider serviceProvider
            )
        {
            InitializeComponent();

            dgvUserList.CellFormatting += dgvUserList_CellFormatting;

            Load += UserManagementForm_Load;

            _userService = userService;

            dgvUserList.ReadOnly = true;
            dgvUserList.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvUserList.AllowUserToAddRows = false;

            dgvUser.AllowUserToAddRows = false;

            if (dgvUser.Rows.Count == 0)
                dgvUser.Rows.Add();
            _serviceProvider = serviceProvider;
        }

        private async void UserManagementForm_Load(object sender, EventArgs e)
        {
            await LoadUsersAsync();
        }
        //метод загрузки пользователей в таблицу
        private async Task LoadUsersAsync()
        {
            dgvUserList.AutoGenerateColumns = false;
            var result = await _userService.GetUsersAsync();
            if (result.Success == true)
            {
                userList = result.Data;
                dgvUserList.DataSource = userList;
            }
            else 
            {
                MessageBox.Show($"Не удалось загрузить пользователей в таблицу {result.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }
       

        private async void dgvUserList_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            await LoadUsersAsync();
        }

        //метод для заполнения  статуса 
        private async void dgvUserList_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvUserList.Columns[e.ColumnIndex].Name == "colIsActive")
            {
                if (e.Value is bool isActive)
                {
                    e.Value = isActive ? "Активный" : "Неактивный";
                    e.FormattingApplied = true;
                }
            }
        }

        private async void btnCreateUser_Click(object sender, EventArgs e)
        {
            string? login =
       dgvUser.Rows[0].Cells["colLogin"].Value?.ToString();

            string? password =
                dgvUser.Rows[0].Cells["colPassword"].Value?.ToString();

            string? role =
                dgvUser.Rows[0].Cells["colNewRole"].Value?.ToString();

            if (string.IsNullOrWhiteSpace(login) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(role))
            {
                MessageBox.Show("Заполните все поля.");
                return;
            }

            var result = await _userService.CreateUserAsync(
                login,
                password,
                role);

            if (!result.Success)
            {
                MessageBox.Show(result.Message);
                return;
            }

            await LoadUsersAsync();

            dgvUser.Rows[0].Cells["colLogin"].Value = "";
            dgvUser.Rows[0].Cells["colPassword"].Value = "";
            dgvUser.Rows[0].Cells["colNewRole"].Value = null;

            MessageBox.Show("Пользователь успешно создан.");
        }

        private async void btnDeleteUser_Click(object sender, EventArgs e)
        {
            if (dgvUserList.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите пользователя.");
                return;
            }

            Guid id = (Guid)dgvUserList.SelectedRows[0]
                .Cells["colId"].Value;

            string login = dgvUserList.SelectedRows[0]
                .Cells["colName"].Value.ToString();

            if (MessageBox.Show(
                $"Удалить пользователя {login}?",
                "Подтверждение",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question)
                != DialogResult.Yes)
                return;

            var result = await _userService.DeleteUserAsync(id);

            if (!result.Success)
            {
                MessageBox.Show(result.Message);
                return;
            }

            await LoadUsersAsync();

            MessageBox.Show("Пользователь удалён.");
        }

        private async void btnChangeUser_Click(object sender, EventArgs e)
        {
            if (dgvUserList.CurrentRow == null)
            {
                MessageBox.Show("Выберите пользователя.");
                return;
            }

            //var user = dgvUserList.CurrentRow.DataBoundItem as User;

            //if (user == null)
            //{
            //    MessageBox.Show("Не удалось получить пользователя.");
            //    return;
            //}

            var user = (User)dgvUserList.CurrentRow.DataBoundItem;

            var form = _serviceProvider.GetRequiredService<ChangeUserForm>();
            form.UserId = user.Id;

            if (form.ShowDialog() == DialogResult.OK)
            {
                await LoadUsersAsync();
            }
        }
    }
}

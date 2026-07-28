using PromVesClient.Service.UserService;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace PromVesClient
{
    public partial class ChangeUserForm : Form
    {
        private readonly UserService _userService;
        public Guid UserId;

        public ChangeUserForm(
            UserService userService)
        {
            InitializeComponent();

            _userService = userService;


            cbRole.Items.Add("admin");
            cbRole.Items.Add("operator");
            cbRole.DropDownStyle = ComboBoxStyle.DropDownList;

            Load += ChangeUserForm_Load;
        }

        private async void ChangeUserForm_Load(object sender, EventArgs e)
        {
            var user = await _userService.GetUserAsync(UserId);

            if (user == null)
            {
                MessageBox.Show("Пользователь не найден.");
                Close();
                return;
            }

            txtLogin.Text = user.Name;
            txtPassword.Text = "";
            cbRole.SelectedItem = user.Role;
            cbStatus.SelectedItem = user.IsActive
        ? "Активный"
        : "Неактивный";
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            var login = txtLogin.Text.Trim();
            var password = txtPassword.Text;
            var role = cbRole.SelectedItem?.ToString();

            if (string.IsNullOrWhiteSpace(login))
            {
                MessageBox.Show("Введите логин.");
                return;
            }

            if (string.IsNullOrWhiteSpace(role))
            {
                MessageBox.Show("Выберите роль.");
                return;
            }

            bool isActive = cbStatus.SelectedItem?.ToString() == "Активный";

            var result = await _userService.UpdateUserAsync(
                UserId,
                login,
                password,
                role,
                isActive);

            if (!result.Success)
            {
                MessageBox.Show(result.Message);
                return;
            }

            MessageBox.Show("Пользователь успешно изменен.");

            DialogResult = DialogResult.OK;
            Close();
        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
        "Вы действительно хотите удалить пользователя?",
        "Подтверждение удаления",
        MessageBoxButtons.YesNo,
        MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
                return;

            var serviceResult = await _userService.DeleteUserAsync(UserId);

            if (!serviceResult.Success)
            {
                MessageBox.Show(
                    serviceResult.Message,
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return;
            }

            MessageBox.Show(
                "Пользователь успешно удален.",
                "Удаление",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            DialogResult = DialogResult.OK;
            Close();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }
    }
}

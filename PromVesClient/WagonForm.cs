using PromVesClient.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PromVesClient
{
    public partial class WagonForm : Form
    {
        private readonly WagonService _wagonService;

        public WagonForm(WagonService wagonService)
        {
            InitializeComponent();

            _wagonService = wagonService;

            // Верхняя таблица — только одна строка для ввода
            dgvWagon.AllowUserToAddRows = false;
            dgvWagon.AllowUserToDeleteRows = false;
            dgvWagon.MultiSelect = false;

            // Создаем одну строку для нового вагона
            dgvWagon.Rows.Add();

            // Нижняя таблица — только для просмотра
            dgvWagonList.ReadOnly = true;
            dgvWagonList.AllowUserToAddRows = false;
            dgvWagonList.AllowUserToDeleteRows = false;
            dgvWagonList.MultiSelect = false;
            dgvWagonList.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;
        }


        private async void WagonForm_Load(object sender, EventArgs e)
        {
            await LoadWagonsAsync();
        }
        private async Task LoadWagonsAsync()
        {
            var result = await _wagonService.GetAllAsync();

            if (!result.Success)
            {
                MessageBox.Show(
                    result.Message,
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return;
            }

            dgvWagonList.Rows.Clear();

            foreach (var wagon in result.Data)
            {
                dgvWagonList.Rows.Add(
                    wagon.Id,
                    wagon.Number,
                    wagon.TareWeight,
                    wagon.IsActive ? "Активный" : "Неактивный");
            }
        }

        private async void btnCreate_Click(object sender, EventArgs e)
        {
            // Проверяем, что в верхней таблице есть строка
            if (dgvWagon.Rows.Count == 0)
            {
                MessageBox.Show(
                    "Нет строки для ввода вагона.",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return;
            }

            // Получаем значения из верхнего DataGridView
            var number = dgvWagon.Rows[0].Cells["colNumber"].Value?.ToString()?.Trim();

            if (string.IsNullOrWhiteSpace(number))
            {
                MessageBox.Show(
                    "Введите номер вагона.",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            // Получаем тару
            decimal tareWeight;

            var tareValue = dgvWagon.Rows[0]
                .Cells["colTareWeight"]
                .Value;

            if (tareValue == null ||
                !decimal.TryParse(tareValue.ToString(), out tareWeight))
            {
                MessageBox.Show(
                    "Введите корректное значение тары.",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            // Проверяем, что тара не отрицательная
            if (tareWeight < 0)
            {
                MessageBox.Show(
                    "Тара не может быть отрицательной.",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            // Передаем данные в сервис
            var result = await _wagonService.CreateAsync(
                number,
                tareWeight);

            // Проверяем результат
            if (!result.Success)
            {
                MessageBox.Show(
                    result.Message,
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return;
            }

            MessageBox.Show(
                "Вагон успешно добавлен.",
                "Успех",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            // Очищаем строку ввода
            dgvWagon.Rows[0].Cells["colNumber"].Value = null;
            dgvWagon.Rows[0].Cells["colTareWeight"].Value = null;

            // Обновляем список вагонов
            await LoadWagonsAsync();
        }

        private async void btnChangeStatus_Click(object sender, EventArgs e)
        {
            // Проверяем, выбран ли вагон
            if (dgvWagonList.SelectedRows.Count == 0)
            {
                MessageBox.Show(
                    "Выберите вагон.",
                    "Внимание",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            // Получаем выбранную строку
            var row = dgvWagonList.SelectedRows[0];

            // Получаем Id вагона
            var idValue = row.Cells["colId"].Value;

            if (idValue == null || !Guid.TryParse(idValue.ToString(), out Guid wagonId))
            {
                MessageBox.Show(
                    "Не удалось определить вагон.",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return;
            }

            // Получаем текущий статус
            var status = row.Cells["colListStatus"].Value?.ToString();

            bool isActive;

            if (status == "Активный")
            {
                isActive = false;
            }
            else
            {
                isActive = true;
            }

            // Меняем статус через сервис
            var result = await _wagonService.SetActiveAsync(
                wagonId,
                isActive);

            if (!result.Success)
            {
                MessageBox.Show(
                    result.Message,
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return;
            }

            // Обновляем список
            await LoadWagonsAsync();
        }
    }
}

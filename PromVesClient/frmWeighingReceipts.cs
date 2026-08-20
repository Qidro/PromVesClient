using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using PromVesClient.DTO;
using PromVesClient.Models;
using PromVesClient.Service;
using PromVesClient.Service.ReceiptsService;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PromVesClient
{
    public partial class frmWeighingReceipts : Form
    {
        private readonly ReceiptsService _receiptsService;
        private readonly CurrentUserService _currentUserService;
        private readonly ExcelReportService _excelReportService;
        private readonly ILogger<frmWeighingReceipts> _logger;

        private List<ReceiptDto> receiptList;

        private List<CardsDto> cardsList = new();
        private List<ReceiptDtoExcel> ListReceiptExcel = new();
        private string VagonNumber;
        //Поле для фмльтра
        private string Operator;
        //поле для печати квитанции
        private string Cargo;
        //поле для фильтра (грузоотправитель)
        private string Shipper;
        //поле для печати квитанции
        private string OperatorReceipt;
        public frmWeighingReceipts(ReceiptsService receiptsService, CurrentUserService currentUserService, ILogger<frmWeighingReceipts> logger, ExcelReportService excelReportService)
        {
            _receiptsService = receiptsService;
            _currentUserService = currentUserService;
            _excelReportService = excelReportService;
            _logger = logger;
            InitializeComponent();
            dataGridViewСards.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridViewСards.CellValidating += dataGridViewСards_CellValidating;
            dataGridViewСards.DataError += dataGridViewСards_DataError;
            this.Load += Form1_Load;
            receiptInfoLabel.Text = "";
            _excelReportService = excelReportService;

        }
        //метод нажатия на кнопку фильтра поиска квитанции
        private async void btnReportFilter_Click(object sender, EventArgs e)
        {
            _logger.LogInformation($"Пользователь {_currentUserService.CurrentUser?.Name} нажал на кнопку формирование фильтра");
            //проверка на поиск фильтра с номером вагона
            if (vagonNumberBox.Checked == true)
            {
                if (vagonNumberTextBox.Text != null)
                {
                    VagonNumber = vagonNumberTextBox.Text;
                }
            }
            //проверка на поиск фильтра с именем оператора
            if (operatorCheckBox.Checked == true)
            {
                if (operatorTextBox.Text != null)
                {
                    Operator = operatorTextBox.Text;
                }
            }
            //проверка на поиск фильтра с грузом
            if (cargoCheckBox.Checked == true)
            {
                if (cargoTextBox.Text != null)
                {
                    Cargo = cargoTextBox.Text;
                }
            }
            //проверка на поиск фильтра с грузоотправителем
            if (shipperСheckBox.Checked == true)
            {
                if (shipperTextBox.Text != null)
                {
                    Shipper = shipperTextBox.Text;
                }
            }
            //заполнение DTO
            SearchReceiptDto searchReceiptDto = new SearchReceiptDto
            {
                periodStart = dateTimePicker1.Value.ToUniversalTime(),
                periodEnd = dateTimePicker2.Value.ToUniversalTime(),
                vagonNumber = VagonNumber,
                operatorName = Operator
            };
            //выполнение запроса на получений квитанций с помощью фильтра
            var result = await _receiptsService.GetReceiptFilter(searchReceiptDto);
            //проверка запроса
            if (result.Success == false)
            {
                MessageBox.Show($"Ошибка поиска квитанций, причина: {result.Message}", "Произошла ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            receiptList = result.Data;
            dataGridViewReceipts.DataSource = result.Data;
            VagonNumber = null;
            Operator = null;
            dataGridViewСards.DataSource = null;
            receiptInfoLabel.Text = "";
            //GetReceiptFilter
            //WeighingDto dto = new WeighingDto
            //{
            //    Platform1Left = 32,
            //    Platform1Right = 32,
            //    Platform2Left = 32,
            //    Platform2Right = 32,
            //    VagonNumber = "comboBoxVagonNumber.Text",
            //    TareWeight = 32,
            //    GrossWeight = 23,
            //    //IdReceipt = IdReceipt
            //};

        }
        private async void Form1_Load(object sender, EventArgs e)
        {
            await loadingTableData();
        }
        //загрузка первоначальных (всех) данных таблицы квитанций
        private async Task loadingTableData()
        {
            var result = await _receiptsService.GetReceiptsAsync();
            //var resulet = await _receiptsService.GetWeighingAsync();
            if (result.Success == false)
            {
                MessageBox.Show($"Данные не были найдены, причина: {result.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            //List <WeighingDto> receipts = new List<WeighingDto>();
            //receipts.Add(dto);
            //dataGridView1.AutoGenerateColumns = false;

            //копируем результат запроса в поле
            receiptList = result.Data;
            dataGridViewReceipts.DataSource = result.Data;
            //dataGridView2.DataSource = resulet.Data;
            dataGridViewReceipts.Columns["Id"].Visible = false;
            //dataGridView1.Columns["Weighings"].Visible = false;
            dataGridViewReceipts.Columns["DateTime"].HeaderText = "Дата и время";
            dataGridViewReceipts.Columns["TypeWeighng"].HeaderText = "Тип взвешивания";
            dataGridViewReceipts.Columns["Operator"].HeaderText = "Оператор";
        }

        private async void button1_Click_1(object sender, EventArgs e)
        {
            _logger.LogInformation($"Пользователь {_currentUserService.CurrentUser?.Name} нажал на кнопку cброса фильтра");
            await loadingTableData();
            dataGridViewСards.DataSource = null;
            receiptInfoLabel.Text = "";
        }



        private void dataGridViewReceipts_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            //if (e.RowIndex >= 0 && e.RowIndex < receiptList.Count)
            //{
            //    MessageBox.Show(
            //        $"Столбец: {e.ColumnIndex}\n" +
            //        $"Строка: {e.RowIndex}\n" +
            //        $"Id: {receiptList[e.RowIndex].Id}\n" +
            //        $"Дата: {receiptList[e.RowIndex].DateTime}"
            //    );
            //}
        }

        private async void dataGridViewReceipts_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < receiptList.Count)
            {
                //MessageBox.Show(
                //    //$"Столбец: {e.ColumnIndex}\n" +
                //   // $"Строка: {e.RowIndex}\n" +
                //    $"Id: {receiptList[e.RowIndex].Id}\n" +
                //    $"Дата: {receiptList[e.RowIndex].DateTime}"
                //);

                //выводим информацию о времени создания квитанции
                receiptInfoLabel.Text = "Квитанция от " + receiptList[e.RowIndex].DateTime.ToString();
                //сохраняем выбранную квитанцию в поле
                OperatorReceipt = receiptList[e.RowIndex].Operator;
                var result = await _receiptsService.GetCardsAsync(receiptList[e.RowIndex].Id);
                if (result.Success == true)
                {
                    cardsList = result.Data;
                    dataGridViewСards.DataSource = result.Data;
                    settingViewTable();

                }
                else
                {
                    MessageBox.Show("Не удалось вывести квитанцию, причина: " + result.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async void settingViewTable()
        {
            var resultVisibal = await _receiptsService.GetVisibalColumn();
            if (resultVisibal.Success == false)
            {
                MessageBox.Show($"Данные не были найдены, причина: {resultVisibal.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            foreach (DataGridViewColumn column in dataGridViewСards.Columns)
            {
                //MessageBox.Show(
                //    $"Name: {column.Name}\nHeaderText: {column.HeaderText}");
            }
            for (int i = 0; i < resultVisibal.Data.Count; i++)
            {
                try
                {
                    //MessageBox.Show("пиздец:" + dataGridViewСards.Columns[resultVisibal.Data.ElementAt(i).Key].ToString());
                    dataGridViewСards.Columns[resultVisibal.Data.ElementAt(i).Key].Visible = resultVisibal.Data.ElementAt(i).Value;
                }
                catch (Exception ex)
                {

                }

            }
            dataGridViewСards.Columns["Id"].Visible = false;
            dataGridViewСards.Columns["ReceiptId"].Visible = false;
            dataGridViewСards.Columns["VagonNumber"].HeaderText = "Номер вагона";
            dataGridViewСards.Columns["L1"].HeaderText = "Левая сторона 1";
            dataGridViewСards.Columns["R1"].HeaderText = "Правая сторона 1";
            dataGridViewСards.Columns["L2"].HeaderText = "Левая сторона 2";
            dataGridViewСards.Columns["R2"].HeaderText = "Правая сторона 2";
            dataGridViewСards.Columns["TareWeight"].HeaderText = "Тара т.";
            dataGridViewСards.Columns["GrossWeight"].HeaderText = "Брутто т.";
            dataGridViewСards.Columns["NetWeight"].HeaderText = "Нетто т.";
            dataGridViewСards.Columns["LoadCapacity"].HeaderText = "Грузоподъемность";
            dataGridViewСards.Columns["LoadDeviation"].HeaderText = "недогруз/перегруз т.";
            dataGridViewСards.Columns["FirstCart"].HeaderText = "первая тележка т.";
            dataGridViewСards.Columns["SecondCart"].HeaderText = "вторая тележка т.";
            dataGridViewСards.Columns["DifferenceCarts"].HeaderText = "разница тележек т.";
            dataGridViewСards.Columns["LeftSide"].HeaderText = "левый борт т.";
            dataGridViewСards.Columns["RightSide"].HeaderText = "правый борт т.";
            dataGridViewСards.Columns["DifferenceSides"].HeaderText = "разница бортов т.";
            dataGridViewСards.Columns["TypeWeighing"].HeaderText = "Тип взвешивания";
            dataGridViewСards.Columns["Shipper"].HeaderText = "Грузоотправитель";
            dataGridViewСards.Columns["Consignee"].HeaderText = "Грузополучатель";
            dataGridViewСards.Columns["Cargo"].HeaderText = "Груз";
            dataGridViewСards.Columns["InvoiceNumber"].HeaderText = "Номер накладной";
            dataGridViewСards.Columns["InvoiceDateTime"].HeaderText = "Номер накладной";
            dataGridViewСards.Columns["InvoiceWeighing"].HeaderText = "Вес по накладной";

            dataGridViewСards.ReadOnly = false;

            // По умолчанию все поля запрещены для редактирования
            foreach (DataGridViewColumn column in dataGridViewСards.Columns)
            {
                column.ReadOnly = true;
            }

            // Разрешаем редактировать только данные накладной
            dataGridViewСards.Columns["Shipper"].ReadOnly = false;
            dataGridViewСards.Columns["Consignee"].ReadOnly = false;
            dataGridViewСards.Columns["Cargo"].ReadOnly = false;
            dataGridViewСards.Columns["InvoiceNumber"].ReadOnly = false;
            dataGridViewСards.Columns["InvoiceDateTime"].ReadOnly = false;
            dataGridViewСards.Columns["InvoiceWeighing"].ReadOnly = false;

            dataGridViewСards.Columns["InvoiceDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy";
        }


        //метод нажатия на кнопку для удаления квитанции
        private async void button4_Click(object sender, EventArgs e)
        {
            if (dataGridViewReceipts.CurrentRow != null)
            {
                //MessageBox.Show($"Номер строки: {dataGridViewСards.CurrentRow.Index}");
                DialogResult resultConfirmation = MessageBox.Show(
                "Вы действительно хотите удалить карточку вагона?",
                "Подтверждение",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);
                //проверка на выбор пользователя
                if (resultConfirmation == DialogResult.Yes)
                {
                    _logger.LogInformation($"Пользователь {_currentUserService.CurrentUser?.Name} нажал кнопку удаления квитанции");
                    // Выполнить удаление
                    var result = await _receiptsService.deletingReceipt(receiptList[dataGridViewReceipts.CurrentRow.Index].Id);
                    if (result.Success == true)
                    {
                        MessageBox.Show(
                        "Квитанция успешно удалена",
                        "Удаление квитанции",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                        await loadingTableData();
                        dataGridViewСards.DataSource = null;
                        receiptInfoLabel.Text = "";
                    }
                    else
                    {
                        MessageBox.Show(
                        $"Квитанция не удалина, причина: {result.Message}",
                        "Ошибка удаления квитанции",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    }

                }
                else
                {
                    // Пользователь нажал "Нет"
                }
            }
            else
            {
                MessageBox.Show("Перед удалением выберите квитанцию, которую хотели бы удалить", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }



        }


        //метод кнопки сохранения отчетов
        private async void btnChangeReceipt_Click(object sender, EventArgs e)
        {
            if (cardsList == null || cardsList.Count == 0)
            {
                MessageBox.Show("Сначала выберите квитанцию.");
                return;
            }
            _logger.LogInformation($"Пользователь {_currentUserService.CurrentUser?.Name} нажал на кнопку сохранения квитанции");
            List<ReceiptDtoExcel> receiptExcel = new();
            //перебираем колекцию
            foreach (var card in cardsList)
            {
                receiptExcel.Add(new ReceiptDtoExcel
                {
                    VagonNumber = card.VagonNumber,
                    L1 = card.L1,
                    R1 = card.R1,
                    L2 = card.L2,
                    R2 = card.R2,
                    TareWeight = card.TareWeight,
                    GrossWeight = card.GrossWeight,
                    NetWeight = card.NetWeight,
                    LoadCapacity = card.LoadCapacity,
                    LoadDeviation = card.LoadDeviation,
                    FirstCart = card.FirstCart,
                    SecondCart = card.SecondCart,
                    DifferenceCarts = card.DifferenceCarts,
                    LeftSide = card.LeftSide,
                    RightSide = card.RightSide,
                    DifferenceSides = card.DifferenceSides,
                    TypeWeighing = card.TypeWeighing,
                    Shipper = card.Shipper,
                    Consignee = card.Consignee,
                    Cargo = card.Cargo,
                    InvoiceNumber = card.InvoiceNumber,
                    InvoiceDateTime = card.InvoiceDateTime?.Date,
                    InvoiceWeighing = card.InvoiceWeighing
                });
            }

            using SaveFileDialog dialog = new SaveFileDialog
            {
                Title = "Сохранить квитанцию",
                Filter = "Excel (*.xlsx)|*.xlsx",
                DefaultExt = "xlsx",
                FileName = $"Квитанция_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
            };

            if (dialog.ShowDialog() != DialogResult.OK)
                return;
            //получаем результат операции по сохранению отчета
            var result = await _excelReportService.SaveReport(receiptExcel, dialog.FileName);
            //проверка результата
            if (!result.Success)
            {
                MessageBox.Show(result.Message);
                return;
            }

            MessageBox.Show("Квитанция успешно сохранена", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        //Метод уаления карточки вагона
        private async void btnDeleteCard_Click(object sender, EventArgs e)
        {
            if (dataGridViewСards.CurrentRow != null)
            {

                //MessageBox.Show($"Номер строки: {dataGridViewСards.CurrentRow.Index}");
                DialogResult resultConfirmation = MessageBox.Show(
                "Вы действительно хотите удалить карточку вагона?",
                "Подтверждение",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);
                //проверка на выбор пользователя
                if (resultConfirmation == DialogResult.Yes)
                {
                    _logger.LogInformation($"Пользователь {_currentUserService.CurrentUser?.Name} нажал кнопку удаления карточки вагона");
                    // Выполнить удаление
                    var result = await _receiptsService.deletingCard(cardsList[dataGridViewСards.CurrentRow.Index].Id);
                    if (result.Success == true)
                    {
                        MessageBox.Show(
                        "Каточка успешно удалена",
                        "Удаление карточки",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                        cardsList.RemoveAt(dataGridViewСards.CurrentRow.Index);
                        dataGridViewСards.DataSource = null;
                        dataGridViewСards.DataSource = cardsList;
                        settingViewTable();
                        //await loadingTableData();
                        //dataGridViewСards.DataSource = null;
                        //receiptInfoLabel.Text = "";
                    }
                    else
                    {
                        MessageBox.Show(
                        $"Карточка не удалина, причина: {result.Message}",
                        "Ошибка удаления квитанции",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    }

                }
                else
                {
                    // Пользователь нажал "Нет"
                    MessageBox.Show(
                        "В начале выберите карточку",
                        "Предупреждение",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("Перед удалением выберите карточку, которую хотели бы удалить", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        //метод нажатия на кнопку для печати квитанции взвешивания
        private async void btnPrintReceipt_Click(object sender, EventArgs e)
        {
            //проверка на выбора квитанции
            if (cardsList?.Count > 0)
            {
                _logger.LogInformation($"Пользователь {_currentUserService.CurrentUser?.Name} нажал кнопку печати квитанции");
                //перебор данных квитанции для значений DTO
                foreach (var card in cardsList)
                {
                    ReceiptDtoExcel receiptExcel = new ReceiptDtoExcel
                    {
                        VagonNumber = card.VagonNumber,
                        L1 = card.L1,
                        R1 = card.R1,
                        L2 = card.L2,
                        R2 = card.R2,
                        TareWeight = card.TareWeight,
                        GrossWeight = card.GrossWeight,
                        NetWeight = card.NetWeight,
                        LoadCapacity = card.LoadCapacity,
                        LoadDeviation = card.LoadDeviation,
                        FirstCart = card.FirstCart,
                        SecondCart = card.SecondCart,
                        DifferenceCarts = card.DifferenceCarts,
                        LeftSide = card.LeftSide,
                        RightSide = card.RightSide,
                        DifferenceSides = card.DifferenceSides,
                        TypeWeighing = card.TypeWeighing,
                        Shipper = card.Shipper,
                        Consignee = card.Consignee,
                        Cargo = card.Cargo,
                        InvoiceNumber = card.InvoiceNumber,
                        InvoiceDateTime = card.InvoiceDateTime?.Date,
                        InvoiceWeighing = card.InvoiceWeighing
                    };
                    ListReceiptExcel.Add(receiptExcel);
                }
                var result = await _excelReportService.CreateReport(ListReceiptExcel, OperatorReceipt);
                if (result.Success == false)
                {

                }
                ListReceiptExcel.Clear();
            }
            else
            {
                MessageBox.Show("Выберите квитанцию для печати", "Предупрждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            //ReceiptDtoExcel receiptExcel = new ReceiptDtoExcel
            //{
            //    VagonNumber = "VagonNumber1",
            //    TareWeight = "TareWeight2",
            //    GrossWeight = "GrossWeight3",
            //    NetWeight = "NetWeight4"
            //};
            // ListReceiptExcel.Add(receiptExcel);
            // ListReceiptExcel.Add(receiptExcel);

        }

        private async void btnSaveChanges_Click(object sender, EventArgs e)
        {
            //проверка на выбор карточки
            if (cardsList == null || cardsList.Count == 0)
            {
                MessageBox.Show(
                    "Сначала выберите карточку вагона.",
                    "Предупреждение",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            // Завершаем редактирование текущей ячейки
            dataGridViewСards.EndEdit();

            try
            {
                // Перебираем строки таблицы
                for (int i = 0; i < dataGridViewСards.Rows.Count; i++)
                {
                    var row = dataGridViewСards.Rows[i];

                    // Получаем соответствующую карточку
                    var card = cardsList[i];

                    // Изменяем только разрешённые поля
                    card.Shipper = row.Cells["Shipper"].Value?.ToString();
                    card.Consignee = row.Cells["Consignee"].Value?.ToString();
                    card.Cargo = row.Cells["Cargo"].Value?.ToString();
                    card.InvoiceNumber = row.Cells["InvoiceNumber"].Value?.ToString();

                    // Дата накладной
                    if (row.Cells["InvoiceDateTime"].Value != null &&
                        row.Cells["InvoiceDateTime"].Value != DBNull.Value)
                    {
                        if (DateTime.TryParse(
                            row.Cells["InvoiceDateTime"].Value.ToString(),
                            out DateTime invoiceDate))
                        {
                            // PostgreSQL использует timestamp with time zone,
                            // поэтому перед сохранением указываем UTC
                            card.InvoiceDateTime = DateTime.SpecifyKind(
                                invoiceDate,
                                DateTimeKind.Utc);
                        }
                    }

                    // Вес по накладной
                    if (row.Cells["InvoiceWeighing"].Value != null &&
                        row.Cells["InvoiceWeighing"].Value != DBNull.Value)
                    {
                        if (decimal.TryParse(
                            row.Cells["InvoiceWeighing"].Value.ToString(),
                            out decimal invoiceWeight))
                        {
                            card.InvoiceWeighing = invoiceWeight;
                        }
                    }

                    // Сохраняем изменения в БД
                    var result = await _receiptsService.UpdateCardInvoiceAsync(card);

                    if (!result.Success)
                    {
                        MessageBox.Show(
                            $"Не удалось сохранить карточку № {i + 1}.\n\n{result.Message}",
                            "Ошибка сохранения",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);

                        return;
                    }
                }

                MessageBox.Show(
                    "Изменения успешно сохранены.",
                    "Сохранение",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                // После сохранения заново загружаем данные из БД
                if (dataGridViewReceipts.CurrentRow != null)
                {
                    var receiptId =
                        receiptList[dataGridViewReceipts.CurrentRow.Index].Id;

                    var result = await _receiptsService.GetCardsAsync(receiptId);

                    if (result.Success)
                    {
                        cardsList = result.Data;

                        dataGridViewСards.DataSource = null;
                        dataGridViewСards.DataSource = cardsList;

                        settingViewTable();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка сохранения изменений квитанции");

                string errorMessage = ex.Message;

                if (ex.InnerException != null)
                {
                    errorMessage += "\n\nInnerException:\n" +
                                    ex.InnerException.Message;
                }

                if (ex.InnerException?.InnerException != null)
                {
                    errorMessage += "\n\nInnerException 2:\n" +
                                    ex.InnerException.InnerException.Message;
                }

                MessageBox.Show(
                    "Произошла ошибка при сохранении:\n\n" + errorMessage,
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // проверка на ввод данных в таблицу квитанции
        private void dataGridViewСards_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            string columnName = dataGridViewСards.Columns[e.ColumnIndex].Name;
            string value = e.FormattedValue?.ToString().Trim().Replace(',', '.');

            // Проверка InvoiceDateTime
            if (columnName == "InvoiceDateTime")
            {
                if (!string.IsNullOrWhiteSpace(value) &&
                    !DateTime.TryParse(value, out _))
                {
                    MessageBox.Show(
                        "Введите корректную дату.\nНапример: 17.08.2026",
                        "Ошибка ввода",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    e.Cancel = true;
                }
            }

            // Проверка InvoiceWeighing
            if (columnName == "InvoiceWeighing")
            {
                if (!string.IsNullOrWhiteSpace(value) &&
                    !decimal.TryParse(value, out _))
                {
                    MessageBox.Show(
                        "Введите корректный вес.\nНапример: 45,5",
                        "Ошибка ввода",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    e.Cancel = true;
                }
            }
        }
        // обработка ошибок при вводе данных в таблицу квитанции, чтобы не выскакивало окно с ошибкой при вводе данных в таблицу квитанции
        private async void dataGridViewСards_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        private async void btnSaveChanges_Click_1(object sender, EventArgs e)
        {
            //проверка на выбор карточки
            if (cardsList == null || cardsList.Count == 0)
            {
                MessageBox.Show(
                    "Сначала выберите карточку вагона.",
                    "Предупреждение",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            // Завершаем редактирование текущей ячейки
            dataGridViewСards.EndEdit();

            try
            {
                // Перебираем строки таблицы
                for (int i = 0; i < dataGridViewСards.Rows.Count; i++)
                {
                    var row = dataGridViewСards.Rows[i];

                    // Получаем соответствующую карточку
                    var card = cardsList[i];

                    // Изменяем только разрешённые поля
                    card.Shipper = row.Cells["Shipper"].Value?.ToString();
                    card.Consignee = row.Cells["Consignee"].Value?.ToString();
                    card.Cargo = row.Cells["Cargo"].Value?.ToString();
                    card.InvoiceNumber = row.Cells["InvoiceNumber"].Value?.ToString();

                    // Дата накладной
                    if (row.Cells["InvoiceDateTime"].Value != null &&
                        row.Cells["InvoiceDateTime"].Value != DBNull.Value)
                    {
                        if (DateTime.TryParse(
                            row.Cells["InvoiceDateTime"].Value.ToString(),
                            out DateTime invoiceDate))
                        {
                            // PostgreSQL использует timestamp with time zone,
                            // поэтому перед сохранением указываем UTC
                            card.InvoiceDateTime = DateTime.SpecifyKind(
                                invoiceDate,
                                DateTimeKind.Utc);
                        }
                    }

                    // Вес по накладной
                    if (row.Cells["InvoiceWeighing"].Value != null &&
                        row.Cells["InvoiceWeighing"].Value != DBNull.Value)
                    {
                        if (decimal.TryParse(
                            row.Cells["InvoiceWeighing"].Value.ToString(),
                            out decimal invoiceWeight))
                        {
                            card.InvoiceWeighing = invoiceWeight;
                        }
                    }

                    // Сохраняем изменения в БД
                    var result = await _receiptsService.UpdateCardInvoiceAsync(card);

                    if (!result.Success)
                    {
                        MessageBox.Show(
                            $"Не удалось сохранить карточку № {i + 1}.\n\n{result.Message}",
                            "Ошибка сохранения",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);

                        return;
                    }
                }

                MessageBox.Show(
                    "Изменения успешно сохранены.",
                    "Сохранение",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                // После сохранения заново загружаем данные из БД
                if (dataGridViewReceipts.CurrentRow != null)
                {
                    var receiptId =
                        receiptList[dataGridViewReceipts.CurrentRow.Index].Id;

                    var result = await _receiptsService.GetCardsAsync(receiptId);

                    if (result.Success)
                    {
                        cardsList = result.Data;

                        dataGridViewСards.DataSource = null;
                        dataGridViewСards.DataSource = cardsList;

                        settingViewTable();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка сохранения изменений квитанции");

                string errorMessage = ex.Message;

                if (ex.InnerException != null)
                {
                    errorMessage += "\n\nInnerException:\n" +
                                    ex.InnerException.Message;
                }

                if (ex.InnerException?.InnerException != null)
                {
                    errorMessage += "\n\nInnerException 2:\n" +
                                    ex.InnerException.InnerException.Message;
                }

                MessageBox.Show(
                    "Произошла ошибка при сохранении:\n\n" + errorMessage,
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}
using Microsoft.Extensions.DependencyInjection;
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
    public partial class MainMenu : Form
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly CurrentUserService _currentUserService;

        public MainMenu(CurrentUserService currentUserService, IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _serviceProvider = serviceProvider;
            _currentUserService = currentUserService;

            if (_currentUserService.CurrentUser?.Role != "admin")
            {
                menuStrip1.Enabled = false;
            }
            //     label1.Text =
            //$"Пользователь: {_currentUserService.CurrentUser?.Name}";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var form = _serviceProvider.GetRequiredService<StaticWeighing>();
            //var form = new MainMenu();

            form.ShowDialog();
        }

        private void r3rToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void r23r23rToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = _serviceProvider.GetRequiredService<UserManagementForm>();
            form.ShowDialog();
        }

        private void r23r23rToolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void ComSetitem_Click(object sender, EventArgs e)
        {
            var form = _serviceProvider.GetRequiredService<ComPortSettingsForm>();

            form.ShowDialog();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            var form = _serviceProvider.GetRequiredService<frmWeighingReceipts>();
            //var form = new MainMenu();

            form.ShowDialog();
        }

        private void MainMenu_Load(object sender, EventArgs e)
        {

        }

        private void отчетыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = _serviceProvider.GetRequiredService<ReceiptPrintSettingsForm>();

            form.ShowDialog();
        }

        private void справочникВагоновToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = _serviceProvider.GetRequiredService<WagonForm>();

            form.ShowDialog();
        }
    }

}

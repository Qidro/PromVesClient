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
            label1.Text =
       $"Пользователь: {_currentUserService.CurrentUser?.Name}";
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

        }

        private void r23r23rToolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }
    }
}

namespace PromVesClient
{
    partial class MainMenu
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainMenu));
            staticWeighing = new Button();
            dynamicWeighing = new Button();
            button1 = new Button();
            menuStrip1 = new MenuStrip();
            SettingToolStrip = new ToolStripMenuItem();
            r23r23rToolStripMenuItem = new ToolStripMenuItem();
            r23r23rToolStripMenuItem1 = new ToolStripMenuItem();
            ServiceToolStrip = new ToolStripMenuItem();
            ComSetitem = new ToolStripMenuItem();
            отчетыToolStripMenuItem = new ToolStripMenuItem();
            label1 = new Label();
            справочникВагоновToolStripMenuItem = new ToolStripMenuItem();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // staticWeighing
            // 
            staticWeighing.Font = new Font("Segoe UI", 15.75F, FontStyle.Regular, GraphicsUnit.Point, 204);
            staticWeighing.Location = new Point(12, 36);
            staticWeighing.Name = "staticWeighing";
            staticWeighing.Size = new Size(340, 71);
            staticWeighing.TabIndex = 0;
            staticWeighing.Text = "Статическое взвешивание";
            staticWeighing.UseVisualStyleBackColor = true;
            staticWeighing.Click += button1_Click;
            // 
            // dynamicWeighing
            // 
            dynamicWeighing.Font = new Font("Segoe UI", 15.75F, FontStyle.Regular, GraphicsUnit.Point, 204);
            dynamicWeighing.Location = new Point(12, 123);
            dynamicWeighing.Name = "dynamicWeighing";
            dynamicWeighing.Size = new Size(340, 71);
            dynamicWeighing.TabIndex = 1;
            dynamicWeighing.Text = "Динамическое взвешивание";
            dynamicWeighing.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            button1.Font = new Font("Segoe UI", 15.75F, FontStyle.Regular, GraphicsUnit.Point, 204);
            button1.Location = new Point(12, 209);
            button1.Name = "button1";
            button1.Size = new Size(340, 71);
            button1.TabIndex = 2;
            button1.Text = "Квитанции взвешивания";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click_1;
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { SettingToolStrip, ServiceToolStrip });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(364, 24);
            menuStrip1.TabIndex = 3;
            menuStrip1.Text = "menuStrip1";
            // 
            // SettingToolStrip
            // 
            SettingToolStrip.DropDownItems.AddRange(new ToolStripItem[] { r23r23rToolStripMenuItem, r23r23rToolStripMenuItem1 });
            SettingToolStrip.Name = "SettingToolStrip";
            SettingToolStrip.Size = new Size(79, 20);
            SettingToolStrip.Text = "Настройки";
            SettingToolStrip.Click += r3rToolStripMenuItem_Click;
            // 
            // r23r23rToolStripMenuItem
            // 
            r23r23rToolStripMenuItem.Name = "r23r23rToolStripMenuItem";
            r23r23rToolStripMenuItem.Size = new Size(180, 22);
            r23r23rToolStripMenuItem.Text = "Пользователи";
            r23r23rToolStripMenuItem.Click += r23r23rToolStripMenuItem_Click;
            // 
            // r23r23rToolStripMenuItem1
            // 
            r23r23rToolStripMenuItem1.Name = "r23r23rToolStripMenuItem1";
            r23r23rToolStripMenuItem1.Size = new Size(180, 22);
            r23r23rToolStripMenuItem1.Text = "Отчеты";
            r23r23rToolStripMenuItem1.Click += r23r23rToolStripMenuItem1_Click;
            // 
            // ServiceToolStrip
            // 
            ServiceToolStrip.DropDownItems.AddRange(new ToolStripItem[] { ComSetitem, отчетыToolStripMenuItem, справочникВагоновToolStripMenuItem });
            ServiceToolStrip.Name = "ServiceToolStrip";
            ServiceToolStrip.Size = new Size(59, 20);
            ServiceToolStrip.Text = "Сервис";
            // 
            // ComSetitem
            // 
            ComSetitem.Name = "ComSetitem";
            ComSetitem.Size = new Size(195, 22);
            ComSetitem.Text = "Наладка";
            ComSetitem.Click += ComSetitem_Click;
            // 
            // отчетыToolStripMenuItem
            // 
            отчетыToolStripMenuItem.Name = "отчетыToolStripMenuItem";
            отчетыToolStripMenuItem.Size = new Size(195, 22);
            отчетыToolStripMenuItem.Text = "Настройки квитанции";
            отчетыToolStripMenuItem.Click += отчетыToolStripMenuItem_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(218, 319);
            label1.Name = "label1";
            label1.Size = new Size(38, 15);
            label1.TabIndex = 4;
            label1.Text = "label1";
            label1.Visible = false;
            // 
            // справочникВагоновToolStripMenuItem
            // 
            справочникВагоновToolStripMenuItem.Name = "справочникВагоновToolStripMenuItem";
            справочникВагоновToolStripMenuItem.Size = new Size(195, 22);
            справочникВагоновToolStripMenuItem.Text = "Справочник вагонов";
            справочникВагоновToolStripMenuItem.Click += справочникВагоновToolStripMenuItem_Click;
            // 
            // MainMenu
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(364, 346);
            Controls.Add(label1);
            Controls.Add(button1);
            Controls.Add(dynamicWeighing);
            Controls.Add(staticWeighing);
            Controls.Add(menuStrip1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = menuStrip1;
            Name = "MainMenu";
            Text = "Главное меню";
            Load += MainMenu_Load;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button staticWeighing;
        private Button dynamicWeighing;
        private Button button1;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem SettingToolStrip;
        private ToolStripMenuItem r23r23rToolStripMenuItem;
        private ToolStripMenuItem r23r23rToolStripMenuItem1;
        private ToolStripMenuItem ServiceToolStrip;
        private ToolStripMenuItem ComSetitem;
        private Label label1;
        private ToolStripMenuItem отчетыToolStripMenuItem;
        private ToolStripMenuItem справочникВагоновToolStripMenuItem;
    }
}
namespace TorrentPatcher
{
    partial class frmMain
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.lblTorrentName = new System.Windows.Forms.Label();
            this.lblTorrentPath = new System.Windows.Forms.Label();
            this.grbMain = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel8 = new System.Windows.Forms.TableLayoutPanel();
            this.lstTrackers = new System.Windows.Forms.ListView();
            this.clhTracker = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clhLeechers = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clhSeeders = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clhDownloaded = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.txtTorrentPath = new System.Windows.Forms.TextBox();
            this.txtTorrentName = new System.Windows.Forms.TextBox();
            this.lblTrackers = new System.Windows.Forms.Label();
            this.cmsStructure = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tmiExpandNode = new System.Windows.Forms.ToolStripMenuItem();
            this.tmiCollapseNode = new System.Windows.Forms.ToolStripMenuItem();
            this.tssMain = new System.Windows.Forms.ToolStripSeparator();
            this.tmiExpandAll = new System.Windows.Forms.ToolStripMenuItem();
            this.tmiCollapseAll = new System.Windows.Forms.ToolStripMenuItem();
            this.imgTypes = new System.Windows.Forms.ImageList(this.components);
            this.tabControlMain = new System.Windows.Forms.TabControl();
            this.tabData = new System.Windows.Forms.TabPage();
            this.tabStructure = new System.Windows.Forms.TabPage();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnStructExport = new System.Windows.Forms.Button();
            this.btnStructDown = new System.Windows.Forms.Button();
            this.btnStructUp = new System.Windows.Forms.Button();
            this.btnStructReload = new System.Windows.Forms.Button();
            this.btnStructEdit = new System.Windows.Forms.Button();
            this.btnStructRemove = new System.Windows.Forms.Button();
            this.btnStructAdd = new System.Windows.Forms.Button();
            this.lblStructPos = new System.Windows.Forms.Label();
            this.trvTorrent = new System.Windows.Forms.TreeView();
            this.tabSettings = new System.Windows.Forms.TabPage();
            this.tabControlSettings = new System.Windows.Forms.TabControl();
            this.tabSettingsMain = new System.Windows.Forms.TabPage();
            this.chkStat = new System.Windows.Forms.CheckBox();
            this.barCheck = new System.Windows.Forms.ProgressBar();
            this.chkPingCheck = new System.Windows.Forms.CheckBox();
            this.chkTrackersCheck = new System.Windows.Forms.CheckBox();
            this.btnCheckTrackers = new System.Windows.Forms.Button();
            this.lblCity = new System.Windows.Forms.Label();
            this.cmbISP = new System.Windows.Forms.ComboBox();
            this.cmbCity = new System.Windows.Forms.ComboBox();
            this.lblISP = new System.Windows.Forms.Label();
            this.txtArguments = new System.Windows.Forms.TextBox();
            this.chkAutoLaunchAllow = new System.Windows.Forms.CheckBox();
            this.btnLaunchBrowse = new System.Windows.Forms.Button();
            this.txtLaunchPath = new System.Windows.Forms.TextBox();
            this.lblArgs = new System.Windows.Forms.Label();
            this.lblTorrentClientPath = new System.Windows.Forms.Label();
            this.tabSettingsAdditional = new System.Windows.Forms.TabPage();
            this.chkMagnet = new System.Windows.Forms.CheckBox();
            this.lstTrackersAdd = new System.Windows.Forms.ListView();
            this.clhTrackerAdd = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chkSecureEditing = new System.Windows.Forms.CheckBox();
            this.chkPatchAnnouncer = new System.Windows.Forms.CheckBox();
            this.buttonTrackersFile = new System.Windows.Forms.Button();
            this.btnAssocFile = new System.Windows.Forms.Button();
            this.tabSettingsUpdates = new System.Windows.Forms.TabPage();
            this.chkUpdatePatcher = new System.Windows.Forms.CheckBox();
            this.chkUpdateTrackers = new System.Windows.Forms.CheckBox();
            this.txtUpdatePatcher = new System.Windows.Forms.TextBox();
            this.chkAutoCheckUpdates = new System.Windows.Forms.CheckBox();
            this.btnCheckUpdates = new System.Windows.Forms.Button();
            this.txtUpdateTrackers = new System.Windows.Forms.TextBox();
            this.imgFiles = new System.Windows.Forms.ImageList(this.components);
            this.tltMain = new System.Windows.Forms.ToolTip(this.components);
            this.statusStripMain = new System.Windows.Forms.StatusStrip();
            this.tslStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.tslAuthor = new System.Windows.Forms.ToolStripStatusLabel();
            this.tmAutoLaunch = new System.Windows.Forms.Timer(this.components);
            this.tmTrackers = new System.Windows.Forms.Timer(this.components);
            this.grbMain.SuspendLayout();
            this.tableLayoutPanel8.SuspendLayout();
            this.cmsStructure.SuspendLayout();
            this.tabControlMain.SuspendLayout();
            this.tabData.SuspendLayout();
            this.tabStructure.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tabSettings.SuspendLayout();
            this.tabControlSettings.SuspendLayout();
            this.tabSettingsMain.SuspendLayout();
            this.tabSettingsAdditional.SuspendLayout();
            this.tabSettingsUpdates.SuspendLayout();
            this.statusStripMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblTorrentName
            // 
            this.lblTorrentName.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblTorrentName.AutoSize = true;
            this.lblTorrentName.Location = new System.Drawing.Point(5, 40);
            this.lblTorrentName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTorrentName.Name = "lblTorrentName";
            this.lblTorrentName.Size = new System.Drawing.Size(35, 17);
            this.lblTorrentName.TabIndex = 0;
            this.lblTorrentName.Text = "Имя";
            // 
            // lblTorrentPath
            // 
            this.lblTorrentPath.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblTorrentPath.AutoSize = true;
            this.lblTorrentPath.Location = new System.Drawing.Point(5, 8);
            this.lblTorrentPath.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTorrentPath.Name = "lblTorrentPath";
            this.lblTorrentPath.Size = new System.Drawing.Size(39, 17);
            this.lblTorrentPath.TabIndex = 1;
            this.lblTorrentPath.Text = "Путь";
            // 
            // grbMain
            // 
            this.grbMain.AutoSize = true;
            this.grbMain.Controls.Add(this.tableLayoutPanel8);
            this.grbMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grbMain.Location = new System.Drawing.Point(4, 4);
            this.grbMain.Margin = new System.Windows.Forms.Padding(4);
            this.grbMain.Name = "grbMain";
            this.grbMain.Padding = new System.Windows.Forms.Padding(4);
            this.grbMain.Size = new System.Drawing.Size(364, 288);
            this.grbMain.TabIndex = 2;
            this.grbMain.TabStop = false;
            this.grbMain.Text = "Данные";
            // 
            // tableLayoutPanel8
            // 
            this.tableLayoutPanel8.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel8.AutoSize = true;
            this.tableLayoutPanel8.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel8.ColumnCount = 2;
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 87F));
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel8.Controls.Add(this.lstTrackers, 1, 2);
            this.tableLayoutPanel8.Controls.Add(this.lblTorrentPath, 0, 0);
            this.tableLayoutPanel8.Controls.Add(this.lblTorrentName, 0, 1);
            this.tableLayoutPanel8.Controls.Add(this.txtTorrentPath, 1, 0);
            this.tableLayoutPanel8.Controls.Add(this.txtTorrentName, 1, 1);
            this.tableLayoutPanel8.Controls.Add(this.lblTrackers, 0, 2);
            this.tableLayoutPanel8.Location = new System.Drawing.Point(4, 20);
            this.tableLayoutPanel8.Margin = new System.Windows.Forms.Padding(4);
            this.tableLayoutPanel8.Name = "tableLayoutPanel8";
            this.tableLayoutPanel8.RowCount = 3;
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 31F));
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 31F));
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel8.Size = new System.Drawing.Size(356, 265);
            this.tableLayoutPanel8.TabIndex = 0;
            // 
            // lstTrackers
            // 
            this.lstTrackers.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.clhTracker,
            this.clhLeechers,
            this.clhSeeders,
            this.clhDownloaded});
            this.lstTrackers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstTrackers.FullRowSelect = true;
            this.lstTrackers.GridLines = true;
            this.lstTrackers.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lstTrackers.LabelEdit = true;
            this.lstTrackers.Location = new System.Drawing.Point(93, 69);
            this.lstTrackers.Margin = new System.Windows.Forms.Padding(4);
            this.lstTrackers.Name = "lstTrackers";
            this.lstTrackers.ShowItemToolTips = true;
            this.lstTrackers.Size = new System.Drawing.Size(258, 191);
            this.lstTrackers.TabIndex = 16;
            this.lstTrackers.UseCompatibleStateImageBehavior = false;
            this.lstTrackers.View = System.Windows.Forms.View.Details;
            // 
            // clhTracker
            // 
            this.clhTracker.Text = "Tracker";
            this.clhTracker.Width = 100;
            // 
            // clhLeechers
            // 
            this.clhLeechers.Text = "Leechers";
            this.clhLeechers.Width = 30;
            // 
            // clhSeeders
            // 
            this.clhSeeders.Text = "Seeders";
            this.clhSeeders.Width = 30;
            // 
            // clhDownloaded
            // 
            this.clhDownloaded.Text = "Downloaded";
            this.clhDownloaded.Width = 30;
            // 
            // txtTorrentPath
            // 
            this.txtTorrentPath.BackColor = System.Drawing.SystemColors.Window;
            this.txtTorrentPath.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtTorrentPath.Location = new System.Drawing.Point(93, 5);
            this.txtTorrentPath.Margin = new System.Windows.Forms.Padding(4);
            this.txtTorrentPath.Name = "txtTorrentPath";
            this.txtTorrentPath.ReadOnly = true;
            this.txtTorrentPath.Size = new System.Drawing.Size(258, 22);
            this.txtTorrentPath.TabIndex = 0;
            // 
            // txtTorrentName
            // 
            this.txtTorrentName.BackColor = System.Drawing.SystemColors.Window;
            this.txtTorrentName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtTorrentName.Location = new System.Drawing.Point(93, 37);
            this.txtTorrentName.Margin = new System.Windows.Forms.Padding(4);
            this.txtTorrentName.Name = "txtTorrentName";
            this.txtTorrentName.ReadOnly = true;
            this.txtTorrentName.Size = new System.Drawing.Size(258, 22);
            this.txtTorrentName.TabIndex = 1;
            this.txtTorrentName.Leave += new System.EventHandler(this.txtName_Leave);
            // 
            // lblTrackers
            // 
            this.lblTrackers.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTrackers.Location = new System.Drawing.Point(5, 148);
            this.lblTrackers.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTrackers.Name = "lblTrackers";
            this.lblTrackers.Size = new System.Drawing.Size(79, 32);
            this.lblTrackers.TabIndex = 4;
            this.lblTrackers.Text = "Трекеры";
            this.tltMain.SetToolTip(this.lblTrackers, "Личей/Сидов/Загружено");
            // 
            // cmsStructure
            // 
            this.cmsStructure.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tmiExpandNode,
            this.tmiCollapseNode,
            this.tssMain,
            this.tmiExpandAll,
            this.tmiCollapseAll});
            this.cmsStructure.Name = "cmsStructure";
            this.cmsStructure.Size = new System.Drawing.Size(170, 106);
            this.cmsStructure.Opened += new System.EventHandler(this.cmsStructure_Opened);
            // 
            // tmiExpandNode
            // 
            this.tmiExpandNode.Name = "tmiExpandNode";
            this.tmiExpandNode.Size = new System.Drawing.Size(169, 24);
            this.tmiExpandNode.Text = "Раскрыть";
            this.tmiExpandNode.Click += new System.EventHandler(this.tmiNode_Click);
            // 
            // tmiCollapseNode
            // 
            this.tmiCollapseNode.Name = "tmiCollapseNode";
            this.tmiCollapseNode.Size = new System.Drawing.Size(169, 24);
            this.tmiCollapseNode.Text = "Свернуть";
            this.tmiCollapseNode.Click += new System.EventHandler(this.tmiNode_Click);
            // 
            // tssMain
            // 
            this.tssMain.Name = "tssMain";
            this.tssMain.Size = new System.Drawing.Size(166, 6);
            // 
            // tmiExpandAll
            // 
            this.tmiExpandAll.Name = "tmiExpandAll";
            this.tmiExpandAll.Size = new System.Drawing.Size(169, 24);
            this.tmiExpandAll.Text = "Раскрыть все";
            this.tmiExpandAll.Click += new System.EventHandler(this.tmiNode_Click);
            // 
            // tmiCollapseAll
            // 
            this.tmiCollapseAll.Name = "tmiCollapseAll";
            this.tmiCollapseAll.Size = new System.Drawing.Size(169, 24);
            this.tmiCollapseAll.Text = "Свернуть все";
            this.tmiCollapseAll.Click += new System.EventHandler(this.tmiNode_Click);
            // 
            // imgTypes
            // 
            this.imgTypes.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgTypes.ImageStream")));
            this.imgTypes.TransparentColor = System.Drawing.Color.Transparent;
            this.imgTypes.Images.SetKeyName(0, "d");
            this.imgTypes.Images.SetKeyName(1, "l");
            this.imgTypes.Images.SetKeyName(2, "s");
            this.imgTypes.Images.SetKeyName(3, "i");
            // 
            // tabControlMain
            // 
            this.tabControlMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControlMain.Controls.Add(this.tabData);
            this.tabControlMain.Controls.Add(this.tabStructure);
            this.tabControlMain.Controls.Add(this.tabSettings);
            this.tabControlMain.Location = new System.Drawing.Point(4, 4);
            this.tabControlMain.Margin = new System.Windows.Forms.Padding(4);
            this.tabControlMain.Multiline = true;
            this.tabControlMain.Name = "tabControlMain";
            this.tabControlMain.SelectedIndex = 0;
            this.tabControlMain.Size = new System.Drawing.Size(380, 325);
            this.tabControlMain.TabIndex = 0;
            // 
            // tabData
            // 
            this.tabData.Controls.Add(this.grbMain);
            this.tabData.Location = new System.Drawing.Point(4, 25);
            this.tabData.Margin = new System.Windows.Forms.Padding(4);
            this.tabData.Name = "tabData";
            this.tabData.Padding = new System.Windows.Forms.Padding(4);
            this.tabData.Size = new System.Drawing.Size(372, 296);
            this.tabData.TabIndex = 3;
            this.tabData.Text = "Данные";
            this.tabData.UseVisualStyleBackColor = true;
            // 
            // tabStructure
            // 
            this.tabStructure.Controls.Add(this.panel1);
            this.tabStructure.Controls.Add(this.lblStructPos);
            this.tabStructure.Controls.Add(this.trvTorrent);
            this.tabStructure.Location = new System.Drawing.Point(4, 25);
            this.tabStructure.Margin = new System.Windows.Forms.Padding(4);
            this.tabStructure.Name = "tabStructure";
            this.tabStructure.Padding = new System.Windows.Forms.Padding(4);
            this.tabStructure.Size = new System.Drawing.Size(372, 296);
            this.tabStructure.TabIndex = 0;
            this.tabStructure.Text = "Структура";
            this.tabStructure.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.btnStructExport);
            this.panel1.Controls.Add(this.btnStructDown);
            this.panel1.Controls.Add(this.btnStructUp);
            this.panel1.Controls.Add(this.btnStructReload);
            this.panel1.Controls.Add(this.btnStructEdit);
            this.panel1.Controls.Add(this.btnStructRemove);
            this.panel1.Controls.Add(this.btnStructAdd);
            this.panel1.Location = new System.Drawing.Point(234, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(135, 291);
            this.panel1.TabIndex = 18;
            // 
            // btnStructExport
            // 
            this.btnStructExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStructExport.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnStructExport.Location = new System.Drawing.Point(12, 251);
            this.btnStructExport.Margin = new System.Windows.Forms.Padding(4);
            this.btnStructExport.Name = "btnStructExport";
            this.btnStructExport.Size = new System.Drawing.Size(115, 28);
            this.btnStructExport.TabIndex = 24;
            this.btnStructExport.Text = "Экспорт";
            this.btnStructExport.UseVisualStyleBackColor = true;
            // 
            // btnStructDown
            // 
            this.btnStructDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStructDown.AutoSize = true;
            this.btnStructDown.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnStructDown.Font = new System.Drawing.Font("Webdings", 10F);
            this.btnStructDown.Location = new System.Drawing.Point(72, 128);
            this.btnStructDown.Margin = new System.Windows.Forms.Padding(4);
            this.btnStructDown.Name = "btnStructDown";
            this.btnStructDown.Size = new System.Drawing.Size(55, 38);
            this.btnStructDown.TabIndex = 22;
            this.btnStructDown.Text = "6";
            this.btnStructDown.UseVisualStyleBackColor = true;
            // 
            // btnStructUp
            // 
            this.btnStructUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStructUp.AutoSize = true;
            this.btnStructUp.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnStructUp.Font = new System.Drawing.Font("Webdings", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnStructUp.Location = new System.Drawing.Point(12, 128);
            this.btnStructUp.Margin = new System.Windows.Forms.Padding(4);
            this.btnStructUp.Name = "btnStructUp";
            this.btnStructUp.Size = new System.Drawing.Size(55, 38);
            this.btnStructUp.TabIndex = 21;
            this.btnStructUp.Text = "5";
            this.btnStructUp.UseVisualStyleBackColor = true;
            // 
            // btnStructReload
            // 
            this.btnStructReload.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStructReload.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnStructReload.Location = new System.Drawing.Point(12, 174);
            this.btnStructReload.Margin = new System.Windows.Forms.Padding(4);
            this.btnStructReload.Name = "btnStructReload";
            this.btnStructReload.Size = new System.Drawing.Size(115, 28);
            this.btnStructReload.TabIndex = 23;
            this.btnStructReload.Text = "Обновить";
            this.btnStructReload.UseVisualStyleBackColor = true;
            // 
            // btnStructEdit
            // 
            this.btnStructEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStructEdit.AutoSize = true;
            this.btnStructEdit.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnStructEdit.Location = new System.Drawing.Point(12, 89);
            this.btnStructEdit.Margin = new System.Windows.Forms.Padding(4);
            this.btnStructEdit.Name = "btnStructEdit";
            this.btnStructEdit.Size = new System.Drawing.Size(115, 32);
            this.btnStructEdit.TabIndex = 20;
            this.btnStructEdit.Text = "Правка";
            this.btnStructEdit.UseVisualStyleBackColor = true;
            // 
            // btnStructRemove
            // 
            this.btnStructRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStructRemove.AutoSize = true;
            this.btnStructRemove.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnStructRemove.Location = new System.Drawing.Point(12, 49);
            this.btnStructRemove.Margin = new System.Windows.Forms.Padding(4);
            this.btnStructRemove.Name = "btnStructRemove";
            this.btnStructRemove.Size = new System.Drawing.Size(115, 32);
            this.btnStructRemove.TabIndex = 19;
            this.btnStructRemove.Text = "Удалить";
            this.tltMain.SetToolTip(this.btnStructRemove, "Внимание! Нарушение структуры торрента может привести к непредсказуемым последств" +
        "иям!!!");
            this.btnStructRemove.UseVisualStyleBackColor = true;
            // 
            // btnStructAdd
            // 
            this.btnStructAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStructAdd.AutoSize = true;
            this.btnStructAdd.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnStructAdd.Location = new System.Drawing.Point(12, 8);
            this.btnStructAdd.Margin = new System.Windows.Forms.Padding(4);
            this.btnStructAdd.Name = "btnStructAdd";
            this.btnStructAdd.Size = new System.Drawing.Size(115, 32);
            this.btnStructAdd.TabIndex = 18;
            this.btnStructAdd.Text = "Добавить";
            this.btnStructAdd.UseVisualStyleBackColor = true;
            // 
            // lblStructPos
            // 
            this.lblStructPos.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblStructPos.AutoSize = true;
            this.lblStructPos.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.lblStructPos.Location = new System.Drawing.Point(31, 215);
            this.lblStructPos.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblStructPos.Name = "lblStructPos";
            this.lblStructPos.Size = new System.Drawing.Size(0, 17);
            this.lblStructPos.TabIndex = 16;
            // 
            // trvTorrent
            // 
            this.trvTorrent.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.trvTorrent.ContextMenuStrip = this.cmsStructure;
            this.trvTorrent.ImageKey = "s";
            this.trvTorrent.Location = new System.Drawing.Point(8, 7);
            this.trvTorrent.Margin = new System.Windows.Forms.Padding(4);
            this.trvTorrent.Name = "trvTorrent";
            this.trvTorrent.PathSeparator = "/";
            this.trvTorrent.SelectedImageKey = "d";
            this.trvTorrent.Size = new System.Drawing.Size(219, 278);
            this.trvTorrent.TabIndex = 9;
            this.tltMain.SetToolTip(this.trvTorrent, "(d) = Словарь   (l) = Список   (s) = Строка   (i) = Число");
            // 
            // tabSettings
            // 
            this.tabSettings.Controls.Add(this.tabControlSettings);
            this.tabSettings.Location = new System.Drawing.Point(4, 25);
            this.tabSettings.Margin = new System.Windows.Forms.Padding(4);
            this.tabSettings.Name = "tabSettings";
            this.tabSettings.Padding = new System.Windows.Forms.Padding(4);
            this.tabSettings.Size = new System.Drawing.Size(372, 296);
            this.tabSettings.TabIndex = 4;
            this.tabSettings.Text = "Настройки";
            this.tabSettings.UseVisualStyleBackColor = true;
            // 
            // tabControlSettings
            // 
            this.tabControlSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControlSettings.Controls.Add(this.tabSettingsMain);
            this.tabControlSettings.Controls.Add(this.tabSettingsAdditional);
            this.tabControlSettings.Controls.Add(this.tabSettingsUpdates);
            this.tabControlSettings.Location = new System.Drawing.Point(0, 0);
            this.tabControlSettings.Margin = new System.Windows.Forms.Padding(4);
            this.tabControlSettings.Name = "tabControlSettings";
            this.tabControlSettings.SelectedIndex = 0;
            this.tabControlSettings.Size = new System.Drawing.Size(366, 290);
            this.tabControlSettings.TabIndex = 0;
            this.tabControlSettings.SelectedIndexChanged += new System.EventHandler(this.TabControl2SelectedIndexChanged);
            // 
            // tabSettingsMain
            // 
            this.tabSettingsMain.Controls.Add(this.chkStat);
            this.tabSettingsMain.Controls.Add(this.barCheck);
            this.tabSettingsMain.Controls.Add(this.chkPingCheck);
            this.tabSettingsMain.Controls.Add(this.chkTrackersCheck);
            this.tabSettingsMain.Controls.Add(this.btnCheckTrackers);
            this.tabSettingsMain.Controls.Add(this.lblCity);
            this.tabSettingsMain.Controls.Add(this.cmbISP);
            this.tabSettingsMain.Controls.Add(this.cmbCity);
            this.tabSettingsMain.Controls.Add(this.lblISP);
            this.tabSettingsMain.Controls.Add(this.txtArguments);
            this.tabSettingsMain.Controls.Add(this.chkAutoLaunchAllow);
            this.tabSettingsMain.Controls.Add(this.btnLaunchBrowse);
            this.tabSettingsMain.Controls.Add(this.txtLaunchPath);
            this.tabSettingsMain.Controls.Add(this.lblArgs);
            this.tabSettingsMain.Controls.Add(this.lblTorrentClientPath);
            this.tabSettingsMain.Location = new System.Drawing.Point(4, 25);
            this.tabSettingsMain.Margin = new System.Windows.Forms.Padding(4);
            this.tabSettingsMain.Name = "tabSettingsMain";
            this.tabSettingsMain.Size = new System.Drawing.Size(358, 261);
            this.tabSettingsMain.TabIndex = 2;
            this.tabSettingsMain.Text = "Основные";
            this.tabSettingsMain.UseVisualStyleBackColor = true;
            // 
            // chkStat
            // 
            this.chkStat.AutoSize = true;
            this.chkStat.Location = new System.Drawing.Point(133, 81);
            this.chkStat.Margin = new System.Windows.Forms.Padding(4);
            this.chkStat.Name = "chkStat";
            this.chkStat.Size = new System.Drawing.Size(190, 21);
            this.chkStat.TabIndex = 33;
            this.chkStat.Text = "Добавление статистики";
            this.tltMain.SetToolTip(this.chkStat, "Добавляется:\n- Имя торрента (name)\n- Размер (size)\n- Комментарий (comment)\n- Пров" +
        "айдер (isp)");
            this.chkStat.UseVisualStyleBackColor = true;
            // 
            // barCheck
            // 
            this.barCheck.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.barCheck.Location = new System.Drawing.Point(80, 164);
            this.barCheck.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.barCheck.Name = "barCheck";
            this.barCheck.Size = new System.Drawing.Size(224, 23);
            this.barCheck.TabIndex = 32;
            this.barCheck.Visible = false;
            // 
            // chkPingCheck
            // 
            this.chkPingCheck.AutoSize = true;
            this.chkPingCheck.Checked = true;
            this.chkPingCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkPingCheck.Location = new System.Drawing.Point(96, 222);
            this.chkPingCheck.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.chkPingCheck.Name = "chkPingCheck";
            this.chkPingCheck.Size = new System.Drawing.Size(145, 21);
            this.chkPingCheck.TabIndex = 31;
            this.chkPingCheck.Text = "Проверка ping\'ом";
            this.chkPingCheck.UseVisualStyleBackColor = true;
            // 
            // chkTrackersCheck
            // 
            this.chkTrackersCheck.AutoSize = true;
            this.chkTrackersCheck.Checked = true;
            this.chkTrackersCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkTrackersCheck.Location = new System.Drawing.Point(96, 194);
            this.chkTrackersCheck.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.chkTrackersCheck.Name = "chkTrackersCheck";
            this.chkTrackersCheck.Size = new System.Drawing.Size(182, 21);
            this.chkTrackersCheck.TabIndex = 30;
            this.chkTrackersCheck.Text = "Только принудительно";
            this.chkTrackersCheck.UseVisualStyleBackColor = true;
            // 
            // btnCheckTrackers
            // 
            this.btnCheckTrackers.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCheckTrackers.Location = new System.Drawing.Point(75, 159);
            this.btnCheckTrackers.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnCheckTrackers.Name = "btnCheckTrackers";
            this.btnCheckTrackers.Size = new System.Drawing.Size(233, 33);
            this.btnCheckTrackers.TabIndex = 29;
            this.btnCheckTrackers.Text = "Проверить доступность";
            this.btnCheckTrackers.UseVisualStyleBackColor = true;
            this.btnCheckTrackers.Click += new System.EventHandler(this.btnCheckTrackers_Click);
            // 
            // lblCity
            // 
            this.lblCity.AutoSize = true;
            this.lblCity.Location = new System.Drawing.Point(7, 107);
            this.lblCity.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCity.Name = "lblCity";
            this.lblCity.Size = new System.Drawing.Size(48, 17);
            this.lblCity.TabIndex = 28;
            this.lblCity.Text = "Город";
            // 
            // cmbISP
            // 
            this.cmbISP.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cmbISP.FormattingEnabled = true;
            this.cmbISP.Location = new System.Drawing.Point(180, 128);
            this.cmbISP.Margin = new System.Windows.Forms.Padding(4);
            this.cmbISP.Name = "cmbISP";
            this.cmbISP.Size = new System.Drawing.Size(165, 24);
            this.cmbISP.TabIndex = 26;
            this.cmbISP.SelectedIndexChanged += new System.EventHandler(this.comboBoxISP_SelectedIndexChanged);
            // 
            // cmbCity
            // 
            this.cmbCity.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cmbCity.FormattingEnabled = true;
            this.cmbCity.Location = new System.Drawing.Point(7, 128);
            this.cmbCity.Margin = new System.Windows.Forms.Padding(4);
            this.cmbCity.Name = "cmbCity";
            this.cmbCity.Size = new System.Drawing.Size(165, 24);
            this.cmbCity.TabIndex = 25;
            this.cmbCity.SelectedIndexChanged += new System.EventHandler(this.comboBoxCity_SelectedIndexChanged);
            // 
            // lblISP
            // 
            this.lblISP.AutoSize = true;
            this.lblISP.BackColor = System.Drawing.SystemColors.Window;
            this.lblISP.Location = new System.Drawing.Point(180, 107);
            this.lblISP.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblISP.Name = "lblISP";
            this.lblISP.Size = new System.Drawing.Size(81, 17);
            this.lblISP.TabIndex = 24;
            this.lblISP.Text = "Провайдер";
            // 
            // txtArguments
            // 
            this.txtArguments.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtArguments.Location = new System.Drawing.Point(96, 50);
            this.txtArguments.Margin = new System.Windows.Forms.Padding(4);
            this.txtArguments.Name = "txtArguments";
            this.txtArguments.Size = new System.Drawing.Size(209, 22);
            this.txtArguments.TabIndex = 23;
            this.txtArguments.Text = "%1";
            // 
            // chkAutoLaunchAllow
            // 
            this.chkAutoLaunchAllow.AutoSize = true;
            this.chkAutoLaunchAllow.Checked = true;
            this.chkAutoLaunchAllow.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAutoLaunchAllow.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.chkAutoLaunchAllow.Location = new System.Drawing.Point(10, 80);
            this.chkAutoLaunchAllow.Margin = new System.Windows.Forms.Padding(4);
            this.chkAutoLaunchAllow.Name = "chkAutoLaunchAllow";
            this.chkAutoLaunchAllow.Size = new System.Drawing.Size(108, 22);
            this.chkAutoLaunchAllow.TabIndex = 2;
            this.chkAutoLaunchAllow.Text = "Автозапуск";
            this.chkAutoLaunchAllow.UseVisualStyleBackColor = true;
            // 
            // btnLaunchBrowse
            // 
            this.btnLaunchBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLaunchBrowse.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnLaunchBrowse.Location = new System.Drawing.Point(312, 19);
            this.btnLaunchBrowse.Margin = new System.Windows.Forms.Padding(4);
            this.btnLaunchBrowse.Name = "btnLaunchBrowse";
            this.btnLaunchBrowse.Size = new System.Drawing.Size(33, 27);
            this.btnLaunchBrowse.TabIndex = 1;
            this.btnLaunchBrowse.Text = "...";
            this.tltMain.SetToolTip(this.btnLaunchBrowse, "Найти программу...");
            this.btnLaunchBrowse.UseVisualStyleBackColor = true;
            this.btnLaunchBrowse.Click += new System.EventHandler(this.btnLaunchBrowse_Click);
            // 
            // txtLaunchPath
            // 
            this.txtLaunchPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLaunchPath.Location = new System.Drawing.Point(7, 21);
            this.txtLaunchPath.Margin = new System.Windows.Forms.Padding(4);
            this.txtLaunchPath.Name = "txtLaunchPath";
            this.txtLaunchPath.Size = new System.Drawing.Size(298, 22);
            this.txtLaunchPath.TabIndex = 0;
            // 
            // lblArgs
            // 
            this.lblArgs.AutoSize = true;
            this.lblArgs.Location = new System.Drawing.Point(7, 53);
            this.lblArgs.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblArgs.Name = "lblArgs";
            this.lblArgs.Size = new System.Drawing.Size(84, 17);
            this.lblArgs.TabIndex = 23;
            this.lblArgs.Text = "Аргументы*";
            this.tltMain.SetToolTip(this.lblArgs, "(*) \"%1\" путь к файлу, \"%2\" имя файла и \"%3\" название торрента");
            // 
            // lblTorrentClientPath
            // 
            this.lblTorrentClientPath.AutoSize = true;
            this.lblTorrentClientPath.Location = new System.Drawing.Point(7, 2);
            this.lblTorrentClientPath.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTorrentClientPath.Name = "lblTorrentClientPath";
            this.lblTorrentClientPath.Size = new System.Drawing.Size(125, 17);
            this.lblTorrentClientPath.TabIndex = 23;
            this.lblTorrentClientPath.Text = "Путь к программе";
            // 
            // tabSettingsAdditional
            // 
            this.tabSettingsAdditional.Controls.Add(this.chkMagnet);
            this.tabSettingsAdditional.Controls.Add(this.lstTrackersAdd);
            this.tabSettingsAdditional.Controls.Add(this.chkSecureEditing);
            this.tabSettingsAdditional.Controls.Add(this.chkPatchAnnouncer);
            this.tabSettingsAdditional.Controls.Add(this.buttonTrackersFile);
            this.tabSettingsAdditional.Controls.Add(this.btnAssocFile);
            this.tabSettingsAdditional.Location = new System.Drawing.Point(4, 25);
            this.tabSettingsAdditional.Margin = new System.Windows.Forms.Padding(4);
            this.tabSettingsAdditional.Name = "tabSettingsAdditional";
            this.tabSettingsAdditional.Padding = new System.Windows.Forms.Padding(4);
            this.tabSettingsAdditional.Size = new System.Drawing.Size(358, 261);
            this.tabSettingsAdditional.TabIndex = 0;
            this.tabSettingsAdditional.Text = "Дополнительные";
            this.tabSettingsAdditional.UseVisualStyleBackColor = true;
            // 
            // chkMagnet
            // 
            this.chkMagnet.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chkMagnet.AutoSize = true;
            this.chkMagnet.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.chkMagnet.Location = new System.Drawing.Point(219, 7);
            this.chkMagnet.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.chkMagnet.Name = "chkMagnet";
            this.chkMagnet.Size = new System.Drawing.Size(80, 22);
            this.chkMagnet.TabIndex = 2;
            this.chkMagnet.Text = "Magnet";
            this.chkMagnet.UseVisualStyleBackColor = true;
            // 
            // lstTrackersAdd
            // 
            this.lstTrackersAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstTrackersAdd.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.clhTrackerAdd});
            this.lstTrackersAdd.FullRowSelect = true;
            this.lstTrackersAdd.GridLines = true;
            this.lstTrackersAdd.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lstTrackersAdd.HideSelection = false;
            this.lstTrackersAdd.LabelEdit = true;
            this.lstTrackersAdd.Location = new System.Drawing.Point(4, 106);
            this.lstTrackersAdd.Margin = new System.Windows.Forms.Padding(4);
            this.lstTrackersAdd.Name = "lstTrackersAdd";
            this.lstTrackersAdd.ShowItemToolTips = true;
            this.lstTrackersAdd.Size = new System.Drawing.Size(350, 146);
            this.lstTrackersAdd.TabIndex = 6;
            this.lstTrackersAdd.UseCompatibleStateImageBehavior = false;
            this.lstTrackersAdd.View = System.Windows.Forms.View.Details;
            // 
            // clhTrackerAdd
            // 
            this.clhTrackerAdd.Text = "Tracker";
            this.clhTrackerAdd.Width = 400;
            // 
            // chkSecureEditing
            // 
            this.chkSecureEditing.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chkSecureEditing.AutoSize = true;
            this.chkSecureEditing.Checked = true;
            this.chkSecureEditing.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSecureEditing.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.chkSecureEditing.Location = new System.Drawing.Point(8, 7);
            this.chkSecureEditing.Margin = new System.Windows.Forms.Padding(4);
            this.chkSecureEditing.Name = "chkSecureEditing";
            this.chkSecureEditing.Size = new System.Drawing.Size(162, 22);
            this.chkSecureEditing.TabIndex = 1;
            this.chkSecureEditing.Text = "Безопасная правка";
            this.tltMain.SetToolTip(this.chkSecureEditing, "Блокирует словарь \"root/info\". Предотвращает изменение хэша.");
            this.chkSecureEditing.UseVisualStyleBackColor = true;
            this.chkSecureEditing.CheckedChanged += new System.EventHandler(this.chkSecureEditing_CheckedChanged);
            // 
            // chkPatchAnnouncer
            // 
            this.chkPatchAnnouncer.AutoSize = true;
            this.chkPatchAnnouncer.Checked = true;
            this.chkPatchAnnouncer.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkPatchAnnouncer.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.chkPatchAnnouncer.Location = new System.Drawing.Point(8, 76);
            this.chkPatchAnnouncer.Margin = new System.Windows.Forms.Padding(4);
            this.chkPatchAnnouncer.Name = "chkPatchAnnouncer";
            this.chkPatchAnnouncer.Size = new System.Drawing.Size(171, 22);
            this.chkPatchAnnouncer.TabIndex = 4;
            this.chkPatchAnnouncer.Text = "Править и сохранять";
            this.chkPatchAnnouncer.UseVisualStyleBackColor = true;
            // 
            // buttonTrackersFile
            // 
            this.buttonTrackersFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonTrackersFile.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonTrackersFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buttonTrackersFile.Location = new System.Drawing.Point(219, 70);
            this.buttonTrackersFile.Margin = new System.Windows.Forms.Padding(4);
            this.buttonTrackersFile.Name = "buttonTrackersFile";
            this.buttonTrackersFile.Size = new System.Drawing.Size(130, 32);
            this.buttonTrackersFile.TabIndex = 5;
            this.buttonTrackersFile.Text = "Файл";
            this.tltMain.SetToolTip(this.buttonTrackersFile, "Путь к файлу трекеров и папка сохранения пропатченных торрентов");
            this.buttonTrackersFile.UseVisualStyleBackColor = true;
            this.buttonTrackersFile.Click += new System.EventHandler(this.buttonTrackersFile_Click);
            // 
            // btnAssocFile
            // 
            this.btnAssocFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAssocFile.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnAssocFile.Location = new System.Drawing.Point(8, 36);
            this.btnAssocFile.Margin = new System.Windows.Forms.Padding(4);
            this.btnAssocFile.Name = "btnAssocFile";
            this.btnAssocFile.Size = new System.Drawing.Size(341, 32);
            this.btnAssocFile.TabIndex = 3;
            this.btnAssocFile.Text = "Сделать программой по умолчанию";
            this.btnAssocFile.UseVisualStyleBackColor = true;
            this.btnAssocFile.Click += new System.EventHandler(this.btnAssocFile_Click);
            // 
            // tabSettingsUpdates
            // 
            this.tabSettingsUpdates.Controls.Add(this.chkUpdatePatcher);
            this.tabSettingsUpdates.Controls.Add(this.chkUpdateTrackers);
            this.tabSettingsUpdates.Controls.Add(this.txtUpdatePatcher);
            this.tabSettingsUpdates.Controls.Add(this.chkAutoCheckUpdates);
            this.tabSettingsUpdates.Controls.Add(this.btnCheckUpdates);
            this.tabSettingsUpdates.Controls.Add(this.txtUpdateTrackers);
            this.tabSettingsUpdates.Location = new System.Drawing.Point(4, 25);
            this.tabSettingsUpdates.Margin = new System.Windows.Forms.Padding(4);
            this.tabSettingsUpdates.Name = "tabSettingsUpdates";
            this.tabSettingsUpdates.Size = new System.Drawing.Size(358, 261);
            this.tabSettingsUpdates.TabIndex = 3;
            this.tabSettingsUpdates.Text = "Обновления";
            this.tabSettingsUpdates.UseVisualStyleBackColor = true;
            // 
            // chkUpdatePatcher
            // 
            this.chkUpdatePatcher.AutoSize = true;
            this.chkUpdatePatcher.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.chkUpdatePatcher.Location = new System.Drawing.Point(4, 93);
            this.chkUpdatePatcher.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.chkUpdatePatcher.Name = "chkUpdatePatcher";
            this.chkUpdatePatcher.Size = new System.Drawing.Size(192, 22);
            this.chkUpdatePatcher.TabIndex = 26;
            this.chkUpdatePatcher.Text = "Обновление программы";
            this.chkUpdatePatcher.UseVisualStyleBackColor = true;
            // 
            // chkUpdateTrackers
            // 
            this.chkUpdateTrackers.AutoSize = true;
            this.chkUpdateTrackers.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.chkUpdateTrackers.Location = new System.Drawing.Point(4, 37);
            this.chkUpdateTrackers.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.chkUpdateTrackers.Name = "chkUpdateTrackers";
            this.chkUpdateTrackers.Size = new System.Drawing.Size(208, 22);
            this.chkUpdateTrackers.TabIndex = 25;
            this.chkUpdateTrackers.Text = "Обновление трекер-листа";
            this.chkUpdateTrackers.UseVisualStyleBackColor = true;
            // 
            // txtUpdatePatcher
            // 
            this.txtUpdatePatcher.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtUpdatePatcher.Location = new System.Drawing.Point(4, 119);
            this.txtUpdatePatcher.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtUpdatePatcher.Name = "txtUpdatePatcher";
            this.txtUpdatePatcher.Size = new System.Drawing.Size(349, 22);
            this.txtUpdatePatcher.TabIndex = 24;
            // 
            // chkAutoCheckUpdates
            // 
            this.chkAutoCheckUpdates.AutoSize = true;
            this.chkAutoCheckUpdates.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.chkAutoCheckUpdates.Location = new System.Drawing.Point(4, 9);
            this.chkAutoCheckUpdates.Margin = new System.Windows.Forms.Padding(4);
            this.chkAutoCheckUpdates.Name = "chkAutoCheckUpdates";
            this.chkAutoCheckUpdates.Size = new System.Drawing.Size(106, 22);
            this.chkAutoCheckUpdates.TabIndex = 0;
            this.chkAutoCheckUpdates.Text = "Ежедневно";
            this.chkAutoCheckUpdates.UseVisualStyleBackColor = true;
            // 
            // btnCheckUpdates
            // 
            this.btnCheckUpdates.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCheckUpdates.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnCheckUpdates.Location = new System.Drawing.Point(131, 6);
            this.btnCheckUpdates.Margin = new System.Windows.Forms.Padding(4);
            this.btnCheckUpdates.Name = "btnCheckUpdates";
            this.btnCheckUpdates.Size = new System.Drawing.Size(222, 27);
            this.btnCheckUpdates.TabIndex = 1;
            this.btnCheckUpdates.Text = "Проверить";
            this.btnCheckUpdates.UseVisualStyleBackColor = true;
            this.btnCheckUpdates.Click += new System.EventHandler(this.btnCheckUpdates_Click);
            // 
            // txtUpdateTrackers
            // 
            this.txtUpdateTrackers.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtUpdateTrackers.Location = new System.Drawing.Point(3, 65);
            this.txtUpdateTrackers.Margin = new System.Windows.Forms.Padding(4);
            this.txtUpdateTrackers.Name = "txtUpdateTrackers";
            this.txtUpdateTrackers.Size = new System.Drawing.Size(350, 22);
            this.txtUpdateTrackers.TabIndex = 21;
            // 
            // imgFiles
            // 
            this.imgFiles.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.imgFiles.ImageSize = new System.Drawing.Size(16, 16);
            this.imgFiles.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // tltMain
            // 
            this.tltMain.AutomaticDelay = 1;
            this.tltMain.AutoPopDelay = 5000;
            this.tltMain.InitialDelay = 1;
            this.tltMain.ReshowDelay = 0;
            this.tltMain.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Warning;
            this.tltMain.ToolTipTitle = "Информация:";
            // 
            // statusStripMain
            // 
            this.statusStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tslStatus,
            this.tslAuthor});
            this.statusStripMain.Location = new System.Drawing.Point(0, 326);
            this.statusStripMain.Name = "statusStripMain";
            this.statusStripMain.Padding = new System.Windows.Forms.Padding(1, 0, 19, 0);
            this.statusStripMain.Size = new System.Drawing.Size(382, 29);
            this.statusStripMain.TabIndex = 15;
            // 
            // tslStatus
            // 
            this.tslStatus.Name = "tslStatus";
            this.tslStatus.Size = new System.Drawing.Size(62, 24);
            this.tslStatus.Text = "Ждем-с";
            // 
            // tslAuthor
            // 
            this.tslAuthor.AutoToolTip = true;
            this.tslAuthor.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.tslAuthor.IsLink = true;
            this.tslAuthor.Name = "tslAuthor";
            this.tslAuthor.Size = new System.Drawing.Size(300, 24);
            this.tslAuthor.Spring = true;
            this.tslAuthor.Tag = "http://re-tracker.ru/index.php?showforum=9";
            this.tslAuthor.Text = "Сайт проекта";
            this.tslAuthor.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.tslAuthor.ToolTipText = "Контакт";
            this.tslAuthor.Click += new System.EventHandler(this.ToolStripStatusLabel1Click);
            // 
            // tmAutoLaunch
            // 
            this.tmAutoLaunch.Tick += new System.EventHandler(this.tmAutoLaunch_Tick);
            // 
            // frmMain
            // 
            this.AllowDrop = true;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(382, 355);
            this.Controls.Add(this.statusStripMain);
            this.Controls.Add(this.tabControlMain);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Location = new System.Drawing.Point(50, 50);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(400, 400);
            this.Name = "frmMain";
            this.Text = "Патчер торрентов";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmMain_FormClosed);
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.Shown += new System.EventHandler(this.frmMain_Shown);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.frmMain_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.frmMain_DragEnter);
            this.grbMain.ResumeLayout(false);
            this.grbMain.PerformLayout();
            this.tableLayoutPanel8.ResumeLayout(false);
            this.tableLayoutPanel8.PerformLayout();
            this.cmsStructure.ResumeLayout(false);
            this.tabControlMain.ResumeLayout(false);
            this.tabData.ResumeLayout(false);
            this.tabData.PerformLayout();
            this.tabStructure.ResumeLayout(false);
            this.tabStructure.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tabSettings.ResumeLayout(false);
            this.tabControlSettings.ResumeLayout(false);
            this.tabSettingsMain.ResumeLayout(false);
            this.tabSettingsMain.PerformLayout();
            this.tabSettingsAdditional.ResumeLayout(false);
            this.tabSettingsAdditional.PerformLayout();
            this.tabSettingsUpdates.ResumeLayout(false);
            this.tabSettingsUpdates.PerformLayout();
            this.statusStripMain.ResumeLayout(false);
            this.statusStripMain.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar barCheck;
        private System.Windows.Forms.Button btnAssocFile;
        private System.Windows.Forms.Button btnCheckTrackers;
        private System.Windows.Forms.Button btnCheckUpdates;
        private System.Windows.Forms.Button btnLaunchBrowse;
        private System.Windows.Forms.Button buttonTrackersFile;
        private System.Windows.Forms.CheckBox chkAutoCheckUpdates;
        private System.Windows.Forms.CheckBox chkAutoLaunchAllow;
        private System.Windows.Forms.CheckBox chkMagnet;
        private System.Windows.Forms.CheckBox chkPatchAnnouncer;
        private System.Windows.Forms.CheckBox chkPingCheck;
        private System.Windows.Forms.CheckBox chkSecureEditing;
        private System.Windows.Forms.CheckBox chkStat;
        private System.Windows.Forms.CheckBox chkTrackersCheck;
        private System.Windows.Forms.CheckBox chkUpdatePatcher;
        private System.Windows.Forms.CheckBox chkUpdateTrackers;
        private System.Windows.Forms.ColumnHeader clhDownloaded;
        private System.Windows.Forms.ColumnHeader clhLeechers;
        private System.Windows.Forms.ColumnHeader clhSeeders;
        private System.Windows.Forms.ColumnHeader clhTracker;
        private System.Windows.Forms.ColumnHeader clhTrackerAdd;
        private System.Windows.Forms.ComboBox cmbCity;
        private System.Windows.Forms.ComboBox cmbISP;
        private System.Windows.Forms.ContextMenuStrip cmsStructure;
        private System.Windows.Forms.GroupBox grbMain;
        private System.Windows.Forms.ImageList imgFiles;
        private System.Windows.Forms.ImageList imgTypes;
        private System.Windows.Forms.Label lblArgs;
        private System.Windows.Forms.Label lblCity;
        private System.Windows.Forms.Label lblISP;
        private System.Windows.Forms.Label lblStructPos;
        private System.Windows.Forms.Label lblTorrentClientPath;
        private System.Windows.Forms.Label lblTorrentName;
        private System.Windows.Forms.Label lblTorrentPath;
        private System.Windows.Forms.Label lblTrackers;
        private System.Windows.Forms.ListView lstTrackers;
        private System.Windows.Forms.ListView lstTrackersAdd;
        private System.Windows.Forms.StatusStrip statusStripMain;
        private System.Windows.Forms.TabControl tabControlMain;
        private System.Windows.Forms.TabControl tabControlSettings;
        private System.Windows.Forms.TabPage tabData;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel8;
        private System.Windows.Forms.TabPage tabSettings;
        private System.Windows.Forms.TabPage tabSettingsAdditional;
        private System.Windows.Forms.TabPage tabSettingsMain;
        private System.Windows.Forms.TabPage tabSettingsUpdates;
        private System.Windows.Forms.TabPage tabStructure;
        private System.Windows.Forms.ToolTip tltMain;
        private System.Windows.Forms.Timer tmAutoLaunch;
        private System.Windows.Forms.ToolStripMenuItem tmiCollapseAll;
        private System.Windows.Forms.ToolStripMenuItem tmiCollapseNode;
        private System.Windows.Forms.ToolStripMenuItem tmiExpandAll;
        private System.Windows.Forms.ToolStripMenuItem tmiExpandNode;
        private System.Windows.Forms.Timer tmTrackers;
        private System.Windows.Forms.TreeView trvTorrent;
        private System.Windows.Forms.ToolStripStatusLabel tslAuthor;
        private System.Windows.Forms.ToolStripStatusLabel tslStatus;
        private System.Windows.Forms.ToolStripSeparator tssMain;
        private System.Windows.Forms.TextBox txtArguments;
        private System.Windows.Forms.TextBox txtLaunchPath;
        private System.Windows.Forms.TextBox txtTorrentName;
        private System.Windows.Forms.TextBox txtTorrentPath;
        private System.Windows.Forms.TextBox txtUpdatePatcher;
        private System.Windows.Forms.TextBox txtUpdateTrackers;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnStructExport;
        private System.Windows.Forms.Button btnStructDown;
        private System.Windows.Forms.Button btnStructUp;
        private System.Windows.Forms.Button btnStructReload;
        private System.Windows.Forms.Button btnStructEdit;
        private System.Windows.Forms.Button btnStructRemove;
        private System.Windows.Forms.Button btnStructAdd;
    }
}
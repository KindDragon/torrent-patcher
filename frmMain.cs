using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;
using TorrentPatcher.TorrentLoader;

namespace TorrentPatcher
{
    public partial class frmMain : Form
    {
        private bool _InnerCheck;
        private List<Scrape> _ScrapeList = new List<Scrape>();
        private TorrentParser _torrent;
        public IniFile ini = new IniFile(GetSettingPath() + @"Settings.ini");
        private List<Control> RequireLoad = new List<Control>();
        private List<string> UneditableList = new List<string>();

        public frmMain()
        {
            InitializeComponent();
            ThreadStart start = new ThreadStart(CheckUpdatesNow);
            Thread thread = new Thread(start);
            if (!System.IO.File.Exists(GetSettingPath() + @"Settings.ini"))
            {
                System.IO.File.WriteAllText(GetSettingPath() + @"Settings.ini", "[Settings]" + Environment.NewLine + 
                    "FirstRun=True" + Environment.NewLine + 
                    "AutoCheckUpdates=True" + Environment.NewLine + 
                    "CheckHosts=False" + Environment.NewLine + 
                    "CheckPing=False" + Environment.NewLine + 
                    "AddStat=False" + Environment.NewLine + 
                    "UpdatePatcher=True" + Environment.NewLine + 
                    "UpdateTrackers=True" + Environment.NewLine +
                    "LaunchPath=" + Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + @"\uTorrent\uTorrent.exe" + Environment.NewLine + 
                    "LaunchArguments=%1" + Environment.NewLine + 
                    "SecureEdit=True" + Environment.NewLine + 
                    "AutoLaunch=True" + Environment.NewLine + 
                    "PatchAnnouncer=True" + Environment.NewLine + 
                    "MagnetAnnounce=False" + Environment.NewLine + 
                    "DownloadURL=http://re-tracker.ru/trackerssimple.ini" + Environment.NewLine + 
                    "VersionCheck=http://re-tracker.ru/versioncheck.php" + Environment.NewLine + 
                    "UpdateURL=http://re-tracker.ru/TorrentPatcher.exe" + Environment.NewLine + 
                    "TrackerIniIndex=0 0" + Environment.NewLine + 
                    "TrackerCheck=1 2 3 4 5 6 7" + Environment.NewLine + 
                    "FormPosX=50" + Environment.NewLine + 
                    "FormPosY=50" + Environment.NewLine + 
                    "FormSizeHeight=300" + Environment.NewLine + 
                    "FormSizeWidth=275" + Environment.NewLine + 
                    "FormState=Normal" + Environment.NewLine +
                    "TrackersFile=" + GetSettingPath() + @"trackerssimple.ini" + Environment.NewLine + 
                    "LastLaunch=" + DateTime.Today.AddDays(-1.0).ToString());
            }
            if (ini.IniReadBoolValue("Settings", "FirstRun"))
            {
                btnAssocFile_Click(null, null);
                ini.IniWriteValue("Settings", "TrackersFile", GetSettingPath() + @"trackerssimple.ini");
                txtUpdateTrackers.Text = ini.IniReadValue("Settings", "DownloadURL");
                if (chkAutoCheckUpdates.Checked)
                {
                    thread.Start();
                }
                MessageBox.Show("Пожалуйста, выберите своего провайдера в выпадающем списке\nи путь к программе закачки торрент-файлов", 
                    Application.ProductName + Application.ProductVersion, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                tabControlMain.SelectTab(2);
                tabControlSettings.SelectTab(0);
            }
        }

        static string settingsPack = null;

        static private string GetSettingPath()
        {
            if (settingsPack == null)
            {
                if (System.IO.File.Exists(Application.StartupPath + @"\Settings.ini"))
                    settingsPack = Application.StartupPath + @"\";
                else
                {
                    settingsPack = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\TorrentPatcher\";
                    Directory.CreateDirectory(settingsPack);
                }
            }

            return settingsPack;
        }

        private ListViewItem AddTracker(string URL)
        {
            ListViewItem item = new ListViewItem(URL);
            item.SubItems.Add("-");
            item.SubItems.Add("-");
            item.SubItems.Add("-");
            item.ToolTipText = URL;
            return item;
        }

        private string AssocImageIndex(string FileName)
        {
            string key = FileName.Substring(FileName.LastIndexOf('.') + 1);
            if (!imgFiles.Images.ContainsKey(key))
            {
                Icon icon = IconHandler.IconFromExtension(key, IconSize.Small);
                imgFiles.Images.Add(key, icon);
            }
            return key;
        }

        private void AutoTasks()
        {
            Application.DoEvents();
            if (chkAutoLaunchAllow.Checked)
            {
                StartAutoLaunch();
            }
            Application.DoEvents();
        }

        private void btnAssocFile_Click(object sender, EventArgs e)
        {
            try
            {
                string subkey = "TorrentPatcher";
                string str2 = "Torrent Loader File";
                string str3 = ".torrent";
                string str4 = "\"" + Application.ExecutablePath + "\" %1";
                RegistryKey key = Registry.ClassesRoot.CreateSubKey(subkey);
                key.SetValue("", str2);
                key.CreateSubKey("DefaultIcon").SetValue("", Application.ExecutablePath + ",0");
                key = key.CreateSubKey("shell");
                key.SetValue("", "open");
                key = key.CreateSubKey("open").CreateSubKey("command");
                key.SetValue("", str4);
                key.Close();
                key = Registry.ClassesRoot.CreateSubKey(str3);
                key.SetValue("", subkey);
                key.Close();
            }
            catch (System.UnauthorizedAccessException /*ex*/)
            {
                MessageBox.Show(this, "Недостаточно прав чтобы сделать программой по умолчанию. " +
                    "Пожалуйста перезапустите TorrentPatcher с правами администратора");
            	
            }
        }

        private void btnCheckTrackers_Click(object sender, EventArgs e)
        {
            btnCheckTrackers.Enabled = false;
            btnCheckTrackers.Visible = false;
            lstTrackersAdd.LabelEdit = false;
            barCheck.Visible = true;
            tslStatus.Text = "Проверяю доступность ретрекеров";
            tslStatus.PerformClick();
            ini.IniWriteValue("Settings", "TrackerCheck", "0");
            comboBoxISP_SelectedIndexChanged(null, null);
            base.WindowState = FormWindowState.Normal;
            string[] result = new string[lstTrackersAdd.Items.Count - 1];
            int[] numbers = new int[lstTrackersAdd.Items.Count - 1];
            string str = null;
            new Regex("(http|https|udp)://(.*)");
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            numbers = CheckTrackers();
            for (int i = 0; i < numbers.Length; i++)
            {
                str = str + numbers[i].ToString() + " ";
            }
            ini.IniWriteValue("Settings", "TrackerCheck", str);
            lstTrackersAddWorking(numbers, ref result);
            comboBoxISP_SelectedIndexChanged(null, null);
            btnCheckTrackers.Enabled = true;
            lstTrackersAdd.LabelEdit = false;
            stopwatch.Stop();
            barCheck.Visible = false;
            btnCheckTrackers.Visible = true;
            barCheck.Refresh();
            btnCheckTrackers.Refresh();
        }

        private void btnCheckUpdates_Click(object sender, EventArgs e)
        {
            btnCheckUpdates.Enabled = false;
            txtUpdateTrackers.ReadOnly = true;
            new Thread(new ThreadStart(CheckUpdatesNow)).Start();
            btnCheckUpdates.Enabled = true;
            txtUpdateTrackers.ReadOnly = false;
        }

        private void BtnFileExportClick(object sender, EventArgs e)
        {
            Export(false);
        }

        private void btnLaunchBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.CheckFileExists = true;
            dialog.CheckPathExists = true;
            dialog.Filter = "Исполнимые файлы (*.exe)|*.exe|Все файлы (*.*)|*.*";
            dialog.FilterIndex = 0;
            dialog.Multiselect = false;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                txtLaunchPath.Text = dialog.FileName;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string name = dNode.NodeVal(NodeConvert("root/encoding"));
            name = (name != "") ? name : "UTF-8";
            Encoding enc = Encoding.GetEncoding(name);
            var tw = new TorrentWriter(txtTorrentPath.Text, ConvertToDict(trvTorrent.Nodes["root"]), enc);
            LoadTorrent(txtTorrentPath.Text);
        }

        private void btnStructAdd_Click(object sender, EventArgs e)
        {
            TreeNode selectedNode;
            if (trvTorrent.SelectedNode == null)
            {
                trvTorrent.SelectedNode = trvTorrent.Nodes[0];
            }
            switch (dNode.NodeType(trvTorrent.SelectedNode))
            {
                case DataType.List:
                case DataType.Dictionary:
                    selectedNode = trvTorrent.SelectedNode;
                    break;

                default:
                    selectedNode = trvTorrent.SelectedNode.Parent;
                    break;
            }
            bool parentList = false;
            if (dNode.NodeType(selectedNode) == DataType.List)
            {
                parentList = true;
                trvTorrent.SelectedNode = selectedNode.Nodes.Add(dNode.NodeText(selectedNode.Nodes.Count.ToString(), DataType.String, ""));
            }
            else
            {
                string key = CheckExist(selectedNode, "newval");
                trvTorrent.SelectedNode = selectedNode.Nodes.Add(key, dNode.NodeText(key, DataType.String, ""));
            }
            if (UneditableList.Exists(new Predicate<string>(isUneditable)))
            {
                tslStatus.Text = "Значение недоступно для правки";
                trvTorrent.SelectedNode.Remove();
            }
            else
            {
                new frmEdit(false, dNode.NodePath(trvTorrent.SelectedNode), DataType.String, "", parentList, new dSructureUpdate(UpdateStructCallBack));
            }
        }

        private void btnStructDown_Click(object sender, EventArgs e)
        {
            MoveNode(trvTorrent.SelectedNode, false);
        }

        private void btnStructEdit_Click(object sender, EventArgs e)
        {
            if ((trvTorrent.SelectedNode != null) && (dNode.NodePath(trvTorrent.SelectedNode) != "root"))
            {
                DataType type = dNode.NodeType(trvTorrent.SelectedNode);
                if (UneditableList.Exists(new Predicate<string>(isUneditable)))
                {
                    tslStatus.Text = "Значение недоступно для правки";
                }
                else
                {
                    bool parentList = dNode.NodeType(trvTorrent.SelectedNode.Parent) == DataType.List;
                    new frmEdit(true, dNode.NodePath(trvTorrent.SelectedNode), type, 
                        dNode.NodeVal(trvTorrent.SelectedNode), parentList, new dSructureUpdate(UpdateStructCallBack));
                }
            }
        }

        private void BtnStructExportClick(object sender, EventArgs e)
        {
            Export(true);
        }

        private void btnStructReload_Click(object sender, EventArgs e)
        {
            RequireLoad.ForEach(new Action<Control>(ControlDisable));
            trvTorrent.SuspendLayout();
            StructurePopulationStart();
            trvTorrent.ResumeLayout();
            tslStatus.Text = "Структура успешно перезагружена";
            RequireLoad.ForEach(new Action<Control>(ControlEnable));
        }

        private void btnStructRemove_Click(object sender, EventArgs e)
        {
            if (((trvTorrent.SelectedNode == null) || (dNode.NodePath(trvTorrent.SelectedNode) == "root")) || 
                UneditableList.Exists(new Predicate<string>(isUneditable)))
            {
                tslStatus.Text = "Значение недоступно для правки";
            }
            else
            {
                string path = dNode.NodePath(trvTorrent.SelectedNode);
                TreeNode parent = trvTorrent.SelectedNode.Parent;
                trvTorrent.SelectedNode.Remove();
                if (dNode.NodeType(parent) == DataType.List)
                {
                    ListRenamer(parent);
                }
                CheckForMainInfo(path);
            }
        }

        private void btnStructUp_Click(object sender, EventArgs e)
        {
            MoveNode(trvTorrent.SelectedNode, true);
        }

        private void buttonTrackersFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.CheckFileExists = true;
            dialog.CheckPathExists = true;
            dialog.Filter = "INI файлы (*.ini)|*.ini|Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*";
            dialog.FilterIndex = 0;
            dialog.Multiselect = false;
            if (dialog.ShowDialog() != DialogResult.OK)
                return;

            try
            {
                IniFile file = new IniFile(dialog.FileName);
                ini.IniWriteValue("Settings", "TrackersFile", dialog.FileName);
                int num = 0;
                cmbCity.Items.Clear();
                for (int i = 1; i <= file.IniReadIntValue("Город", "Количество"); i++)
                {
                    cmbCity.Items.Add(file.IniReadValue("Город", i.ToString()));
                    num = i - 1;
                }
                if (num >= Convert.ToInt32(ini.IniReadArray("Settings", "TrackerIniIndex")[0]))
                {
                    cmbCity.SelectedIndex = Convert.ToInt32(ini.IniReadArray("Settings", "TrackerIniIndex")[0]);
                }
                else
                {
                    cmbCity.SelectedIndex = 0;
                }
                cmbCity.Refresh();
                if (!chkTrackersCheck.Checked)
                {
                    btnCheckTrackers_Click(null, null);
                }
            }
            catch (System.Exception /*ex*/)
            {
            	
            }
        }

        private void ChangeInt(TreeNode changeNT, string name, string NewInt)
        {
            changeNT.Text = dNode.NodeText(name, DataType.Int, NewInt);
        }

        private void ChangeInt(TreeNode changeNT, string name, string NewInt, bool DeleteIfEmpty)
        {
            if ((NewInt == "") && DeleteIfEmpty)
            {
                changeNT.Remove();
            }
            else
            {
                ChangeString(changeNT, name, NewInt);
            }
        }

        private void ChangeString(TreeNode changeTN, string name, string NewString)
        {
            changeTN.Text = dNode.NodeText(name, DataType.String, NewString);
        }

        private void ChangeString(TreeNode changeTN, string name, string NewString, bool DeleteIfEmpty)
        {
            if ((NewString == "") && DeleteIfEmpty)
            {
                changeTN.Remove();
            }
            else
            {
                ChangeString(changeTN, name, NewString);
            }
        }

        private void CheckCommandLine()
        {
            if (Environment.GetCommandLineArgs().Length <= 1)
            {
                base.WindowState = FormWindowState.Normal;
                return;
            }

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            base.WindowState = FormWindowState.Minimized;
            string torrentPath = "";
            for (int i = 1; i < Environment.GetCommandLineArgs().Length; i++)
            {
                if (Environment.GetCommandLineArgs()[i].EndsWith(".torrent"))
                {
                    torrentPath = torrentPath + Environment.GetCommandLineArgs()[i];
                    LoadTorrent(torrentPath);
                }
                else
                {
                    torrentPath = torrentPath + Environment.GetCommandLineArgs()[i] + " ";
                }
            }
            if (!torrentPath.EndsWith(".torrent"))
            {
                throw new FileLoadException();
            }
            if (chkPatchAnnouncer.Checked & chkAutoLaunchAllow.Checked)
            {
                tmAutoLaunch.Enabled = false;
                if (!chkMagnet.Checked)
                    PatchTorrentFile(torrentPath, stopwatch);
                else
                    PatchMagnetLink(stopwatch);
            }
        }

        private void PatchTorrentFile(string torrentPath, Stopwatch stopwatch)
        {
            Regex regex = new Regex("(http|https|udp)://(.*)");
            for (int j = 0; j < lstTrackersAdd.Items.Count; j++)
            {
                bool flag = false;
                for (int k = 0; k < lstTrackers.Items.Count; k++)
                {
                    if (!regex.IsMatch(lstTrackers.Items[k].SubItems[0].Text))
                    {
                        lstTrackers.Items[k].Remove();
                        k--;
                        flag = false;
                    }
                    else if (lstTrackers.Items[k].SubItems[0].Text == lstTrackersAdd.Items[j].Text)
                    {
                        flag = true;
                    }
                    else
                    {
                        flag = false;
                    }
                }
                if (!flag & regex.IsMatch(lstTrackersAdd.Items[j].Text))
                {
                    if (!chkStat.Checked)
                    {
                        lstTrackers.Items.Add(lstTrackersAdd.Items[j].Text);
                    }
                    else
                    {
                        object[] objArray = new object[] { lstTrackersAdd.Items[j].Text, 
                                    "?name=", _torrent.Name, "&size=", _torrent.Size, "&comment=", 
                                    _torrent.Comment, "&isp=", (cmbCity.SelectedIndex + 1).ToString(), "+", 
                                    (cmbISP.SelectedIndex + 1).ToString() };
                        lstTrackers.Items.Add(string.Concat(objArray));
                    }
                }
            }
            UpdateTrackerStructure();
            string str2 = FileVersionInfo.GetVersionInfo(ini.IniReadValue("Settings", "LaunchPath")).ToString();
            str2 = ini.IniReadValue("Settings", "TrackersFile");
            int startIndex = str2.LastIndexOf(@"\");
            str2 = str2.Remove(startIndex);
            startIndex = torrentPath.LastIndexOf(@"\");
            str2 = str2 + torrentPath.Substring(startIndex);
            txtTorrentPath.Text = str2;
            btnSave_Click(null, null);
            stopwatch.Stop();
            ini.IniWriteValue("Performance", DateTime.Now.ToString() + " rewrite", stopwatch.ElapsedMilliseconds.ToString());
            tmAutoLaunch.Enabled = true;
        }

        private void PatchMagnetLink(Stopwatch stopwatch)
        {
            Regex regex = new Regex("(http|https|udp)://(.*)");
            string arguments = "magnet:?xt=urn:btih:" + _torrent.SHAHash;
            for (int m = 0; m < lstTrackersAdd.Items.Count; m++)
            {
                if (regex.IsMatch(lstTrackersAdd.Items[m].Text))
                {
                    if (!chkStat.Checked)
                    {
                        arguments = arguments + "&tr=" + lstTrackersAdd.Items[m].Text;
                    }
                    else
                    {
                        object obj2 = arguments;
                        object[] objArray2 = new object[] { obj2, "&tr=", lstTrackersAdd.Items[m].Text, 
                                    "?name=", _torrent.Name, "&size=", _torrent.Size, "&comment=", _torrent.Comment, 
                                    "&isp=", (cmbCity.SelectedIndex + 1).ToString(), "+", (cmbISP.SelectedIndex + 1).ToString() };
                        arguments = string.Concat(objArray2);
                    }
                }
            }
            try
            {
                Process.Start(txtLaunchPath.Text, LaunchArgs());
                tmAutoLaunch.Enabled = false;
                tslStatus.Text = "Файл успешно передан";
                stopwatch.Stop();
                ini.IniWriteValue("Performance", DateTime.Now.ToString() + " magnet", stopwatch.ElapsedMilliseconds.ToString());
                if (MessageBox.Show("Торрент файл будет пропатчен безопасно по одной из причин:\nа) наличия в торренте флага private\nб) установленной настройке MAGNET в Настройки->Дополнительно\n\nПОЖАЛУЙСТА НЕ НАЖИМАЙТЕ ОК, ПОКА НЕ ДОБАВИТЕ ЗАКАЧКУ В ТОРРЕНТ-КЛИЕНТ", Application.ProductName + Application.ProductVersion, MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk) == DialogResult.OK)
                {
                    Process.Start(txtLaunchPath.Text, arguments);
                    tslStatus.Text = "Magnet успешно передан";
                }
                else
                {
                    tslStatus.Text = "Передача Magnet отменена";
                }
                if (chkPatchAnnouncer.Checked)
                {
                    Application.Exit();
                }
            }
            catch
            {
                tslStatus.Text = "Ошибка запуска";
            }
        }

        private string CheckExist(string Path, string Name)
        {
            if (!NodeConvert(Path).Nodes.ContainsKey(Name))
            {
                return "";
            }
            Name = CheckExist(Path, Name + "_new");
            return Name;
        }

        private string CheckExist(TreeNode Parent, string Name)
        {
            if (Parent.Nodes.ContainsKey(Name))
            {
                Name = CheckExist(Parent, Name + "_new");
            }
            return Name;
        }

        private bool CheckForAssoc()
        {
            string name = (string) Registry.ClassesRoot.OpenSubKey(".torrent").GetValue("");
            if (name == "TorrentPatcher")
            {
                RegistryKey key = Registry.ClassesRoot.OpenSubKey(name);
                if (((((string) key.GetValue("")) == "Torrent Loader File") && 
                    (((string) key.OpenSubKey("DefaultIcon").GetValue("")) == (Application.ExecutablePath + ",0"))) && 
                    (((string) key.OpenSubKey("shell").OpenSubKey("open").OpenSubKey("command").GetValue("")) == ("\"" + Application.ExecutablePath + "\" %1")))
                {
                    return true;
                }
            }
            return false;
        }

        private bool CheckForContext()
        {
            string name = (string) Registry.ClassesRoot.OpenSubKey(".torrent").GetValue("");
            RegistryKey key = Registry.ClassesRoot.OpenSubKey(name, true).OpenSubKey("shell").OpenSubKey("Open_With_Torrent_Loader");
            return (((key != null) && (((string) key.GetValue("")) == "Open With Torrent Loader")) && 
                (((string) key.OpenSubKey("command").GetValue("")) == ("\"" + Application.ExecutablePath + "\" \"%1\"")));
        }

        private void CheckForMainInfo(string Path)
        {
            string str2;
            string str = "";
            if (NodeConvert(Path) != null)
            {
                str = dNode.NodeVal(NodeConvert(Path));
            }
            if (((str2 = Path) != null) && (str2 == "root/info/name"))
            {
                txtTorrentName.Text = str;
            }
            if (Path.StartsWith("root/announce"))
            {
                GetAnnounceList();
            }
        }

        private int[] CheckTrackers()
        {
            bool res = false;
            string[] separator = new string[] { "http://", "/", ":" };
            int[] numArray = new int[lstTrackersAdd.Items.Count - 1];
            int num = 0;
            int result = 0;
            ManualResetEvent[] waitHandles = new ManualResetEvent[lstTrackersAdd.Items.Count - 1];
            TaskInfo[] infoArray = new TaskInfo[lstTrackersAdd.Items.Count - 1];
            for (int i = 0; i < (lstTrackersAdd.Items.Count - 1); i++)
            {
                string[] strArray2 = lstTrackersAdd.Items[i].Text.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                waitHandles[i] = new ManualResetEvent(false);
                if (int.TryParse(strArray2[1], out result))
                {
                    if (chkPingCheck.Checked)
                    {
                        TaskInfo info = new TaskInfo(strArray2[0], res, waitHandles[i]);
                        infoArray[i] = info;
                        ThreadPool.QueueUserWorkItem(new WaitCallback(info.ThreadPoolCallback), i);
                    }
                    else
                    {
                        TaskInfo info2 = new TaskInfo(strArray2[0], result, res, waitHandles[i]);
                        infoArray[i] = info2;
                        ThreadPool.QueueUserWorkItem(new WaitCallback(info2.ThreadPoolCallback), i);
                    }
                }
                else if (chkPingCheck.Checked)
                {
                    TaskInfo info3 = new TaskInfo(strArray2[0], res, waitHandles[i]);
                    infoArray[i] = info3;
                    ThreadPool.QueueUserWorkItem(new WaitCallback(info3.ThreadPoolCallback), i);
                }
                else
                {
                    TaskInfo info4 = new TaskInfo(strArray2[0], 80, res, waitHandles[i]);
                    infoArray[i] = info4;
                    ThreadPool.QueueUserWorkItem(new WaitCallback(info4.ThreadPoolCallback), i);
                }
            }
            WaitAll(waitHandles);
            result = 0;
            for (int j = 0; j < (lstTrackersAdd.Items.Count - 1); j++)
            {
                if (infoArray[j].Result)
                {
                    numArray[result++] = j + 1;
                }
                else
                {
                    num++;
                }
            }
            if (num > 0)
            {
                string[] strArray4 = new string[] { "Доступно ", ((lstTrackersAdd.Items.Count - num) - 1).ToString(), 
                    " из ", (lstTrackersAdd.Items.Count - 1).ToString(), " ретрекеров" };
                tslStatus.Text = string.Concat(strArray4);
                return numArray;
            }
            tslStatus.Text = "Доступны все ретрекеры";
            return numArray;
        }

        private void CheckUpdatesNow()
        {
            tslStatus.Text = "Проверка обновлений...";
            string str = null;
            HttpWebRequest request = null;
            if (chkUpdatePatcher.Checked)
            {
                request = (HttpWebRequest) WebRequest.Create(ini.IniReadValue("Settings", "VersionCheck"));
                request.UserAgent = "TorrentPatcher/" + Application.ProductVersion;
                request.Credentials = CredentialCache.DefaultCredentials;
                try
                {
                    string str2 = new StreamReader(request.GetResponse().GetResponseStream()).ReadToEnd();
                    if (String.CompareOrdinal(Application.ProductVersion, str2) >= 0)
                    {
                        tslStatus.Text = "У вас последняя версия патчера.";
                    }
                    else
                    {
                        tslStatus.Text = "Новая версия (" + str2 + ")";
                        if (MessageBox.Show("Версия " + str2 + " доступна. Хотите загрузить новую версию?", 
                            Application.ProductName + Application.ProductVersion, MessageBoxButtons.YesNo, 
                            MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                        {
                            Process.Start(txtUpdatePatcher.Text);
                        }
                    }
                }
                catch (Exception exception)
                {
                    str = str + exception.Message;
                    tslStatus.Text = "ОШИБКА:" + str;
                }
            }
            if (chkUpdateTrackers.Checked)
            {
                request = (HttpWebRequest) WebRequest.Create(txtUpdateTrackers.Text);
                request.UserAgent = "TorrentPatcher/" + Application.ProductVersion;
                string contents = null;
                request.Credentials = CredentialCache.DefaultCredentials;
                WebResponse response = null;
                str = null;
                try
                {
                    response = request.GetResponse();
                    if (response.ContentLength > -1L)
                    {
                        long length;
                        if (System.IO.File.Exists(ini.IniReadValue("Settings", "TrackersFile")))
                        {
                            FileInfo info = new FileInfo(ini.IniReadValue("Settings", "TrackersFile"));
                            length = info.Length;
                        }
                        else
                        {
                            length = -1L;
                        }
                        if (response.ContentLength != length)
                        {
                            contents = new StreamReader(response.GetResponseStream()).ReadToEnd();
                            if (contents == null)
                            {
                                return;
                            }
                            System.IO.File.WriteAllText(ini.IniReadValue("Settings", "TrackersFile"), contents, Encoding.Unicode);
                            IniFile file = new IniFile(ini.IniReadValue("Settings", "TrackersFile"));
                            int num2 = 0;
                            cmbCity.Items.Clear();
                            for (int i = 1; i <= file.IniReadIntValue("Город", "Количество"); i++)
                            {
                                cmbCity.Items.Add(file.IniReadValue("Город", i.ToString()));
                                num2 = i - 1;
                            }
                            if (num2 >= Convert.ToInt32(ini.IniReadArray("Settings", "TrackerIniIndex")[0]))
                            {
                                cmbCity.SelectedIndex = Convert.ToInt32(ini.IniReadArray("Settings", "TrackerIniIndex")[0]);
                            }
                            else
                            {
                                cmbCity.SelectedIndex = 0;
                            }
                            cmbCity.Refresh();
                            tslStatus.Text = "Трекер-лист обновлен успешно.";
                        }
                        else
                        {
                            tslStatus.Text = "У вас последняя версия трекер-листа.";
                        }
                    }
                    else
                    {
                        str = "Нет связи с " + txtUpdateTrackers.Text;
                    }
                    if ((!chkTrackersCheck.Checked & !ini.IniReadBoolValue("Settings", "FirstRun")) & 
                        (ini.IniReadDateValue("Settings", "LastLaunch").AddDays(7.0) < DateTime.Now.Date))
                    {
                        btnCheckTrackers_Click(null, null);
                    }
                }
                catch (Exception exception2)
                {
                    str = str + exception2.Message;
                    tslStatus.Text = "ОШИБКА:" + str;
                }
            }
        }

        private void chkSecureEditing_CheckedChanged(object sender, EventArgs e)
        {
            if (!_InnerCheck)
            {
                _InnerCheck = true;
                if (chkSecureEditing.Checked)
                {
                    UneditableList.Add("^root/info");
                    txtTorrentName.ReadOnly = true;
                }
                else if (MessageBox.Show("Be Careful!\r\nSome unsecure changes can corrupt your torrent file or change it's hash\r\n" +
                    "Are you sure you know what you're doing?", Application.ProductName + Application.ProductVersion, 
                    MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                {
                    UneditableList.Remove("^root/info");
                    txtTorrentName.ReadOnly = false;
                }
                else
                {
                    chkSecureEditing.CheckState = CheckState.Checked;
                }
                _InnerCheck = false;
            }
        }

        private void cmsStructure_Opened(object sender, EventArgs e)
        {
            if (trvTorrent.Nodes.Count == 0)
            {
                cmsStructure.Enabled = false;
            }
            else if ((dNode.NodeType(trvTorrent.SelectedNode) == DataType.Dictionary) || 
                (dNode.NodeType(trvTorrent.SelectedNode) == DataType.List))
            {
                tmiCollapseNode.Enabled = true;
                tmiExpandNode.Enabled = true;
            }
            else
            {
                tmiCollapseNode.Enabled = false;
                tmiExpandNode.Enabled = false;
            }
        }

        private void comboBoxCity_SelectedIndexChanged(object sender, EventArgs e)
        {
            int num4;
            IniFile file = new IniFile(ini.IniReadValue("Settings", "TrackersFile").Replace("txt", "ini"));
            int num = 0;
            cmbISP.Items.Clear();
            int num2 = 1;
            while (true)
            {
                num4 = cmbCity.SelectedIndex + 1;
                if (num2 <= file.IniReadIntValue("Провайдеры " + file.IniReadValue("Город", num4.ToString()), "Количество"))
                {
                    int num3 = cmbCity.SelectedIndex + 1;
                    cmbISP.Items.Add(file.IniReadValue("Провайдеры " + file.IniReadValue("Город", num3.ToString()), num2.ToString()));
                    num = num2 - 1;
                    num2++;
                    continue;
                }
                break;
            }
            int index = 0;
            if (num >= Convert.ToInt32(ini.IniReadArray("Settings", "TrackerIniIndex")[1]))
            {
                index = Convert.ToInt32(ini.IniReadArray("Settings", "TrackerIniIndex")[1]);
            }
            if (cmbISP.Items.Count > 0)
                cmbISP.SelectedIndex = index;
            else
                cmbISP.Text = "";
            cmbISP.Refresh();
        }

        string GetRetrackerIniName(IniFile file, int cityIndex, int providerIndex)
        {
            StringBuilder sb = new StringBuilder("Ретрекеры ");
            string sCity = file.IniReadValue("Город", cityIndex);
            sb.Append(sCity);
            sb.Append(" ");
            string sProvider = "Провайдеры " + sCity;
            sb.Append(file.IniReadValue(sProvider, providerIndex));
            return sb.ToString();
        }

        private void comboBoxISP_SelectedIndexChanged(object sender, EventArgs e)
        {
            IniFile file = new IniFile(ini.IniReadValue("Settings", "TrackersFile"));
            lstTrackersAdd.Items.Clear();
            int cityIndex = cmbCity.SelectedIndex + 1;
            int ISPIndex = cmbISP.SelectedIndex + 1;
            var trackerCheck = ini.IniReadArray("Settings", "TrackerCheck");
            string trackerName = GetRetrackerIniName(file, cityIndex, ISPIndex);
            int count = file.IniReadIntValue(trackerName, "Количество");
            if (trackerCheck.Length == 1 && trackerCheck[0] == "0")
            {
                for (int num = 1; num <= count; num++)
                    lstTrackersAdd.Items.Add(file.IniReadValue(trackerName, num));
            }
            else
            {
                for (int index = 0; index < trackerCheck.Length; index++)
                {
                    int tracker = Convert.ToInt32(trackerCheck[index]);
                    if (tracker <= 0)
                        break;
                    if (tracker <= count)
                        lstTrackersAdd.Items.Add(file.IniReadValue(trackerName, tracker));
                }
            }
            lstTrackersAdd.Items.Add("New..");
            lstTrackersAdd.Refresh();
        }

        private void ControlDisable(Control Ctrl)
        {
            Ctrl.Enabled = false;
        }

        private void ControlEnable(Control Ctrl)
        {
            Ctrl.Enabled = true;
        }

        private Dictionary<string, TorrentLoader.TVal> ConvertToDict(TreeNode Node)
        {
            Dictionary<string, TVal> dictionary = new Dictionary<string, TVal>();
            foreach (TreeNode node in Node.Nodes)
            {
                dictionary.Add(node.Name, ConvertTree(node));
            }
            return dictionary;
        }

        private List<TorrentLoader.TVal> ConvertToList(TreeNode Node)
        {
            List<TVal> list = new List<TVal>();
            foreach (TreeNode node in Node.Nodes)
            {
                list.Add(ConvertTree(node));
            }
            return list;
        }

        private TVal ConvertTree(TreeNode Node)
        {
            switch (dNode.NodeType(Node))
            {
                case DataType.Int:
                    return new TVal(DataType.Int, long.Parse(dNode.NodeVal(Node)));

                case DataType.List:
                    return new TVal(DataType.List, ConvertToList(Node));

                case DataType.Dictionary:
                    return new TVal(DataType.Dictionary, ConvertToDict(Node));
            }
            if (dNode.NodePath(Node) == "root/info/pieces")
            {
                return new TVal(DataType.Byte, _torrent.Pieces);
            }
            return new TVal(DataType.String, dNode.NodeVal(Node));
        }

        private void EditStructFile(int index, string Name, string Path)
        {
            if (_torrent.IsSingle)
            {
                EditStructOneFile(Name);
            }
            else
            {
                TreeNode node = NodeConvert("root/info/files/" + index.ToString() + "/path");
                node.Nodes.Clear();
                int num = 0;
                foreach (string str in (Path + @"\" + Name).Split(new string[] { @"\" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    string text = num.ToString() + "(s)[" + str.Length.ToString() + "]=" + str;
                    node.Nodes.Add("", text, "s").SelectedImageKey = "s";
                    num++;
                }
            }
        }

        private void EditStructOneFile(string Name)
        {
            NodeConvert("root/info/name").Text = "name(s)[" + Name.Length.ToString() + "]=" + Name;
            CheckForMainInfo("root/info/name");
        }

        private void Export(bool StructExport)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.OverwritePrompt = true;
            dialog.Title = "Export";
            dialog.FileName = Path.GetFileNameWithoutExtension(txtTorrentPath.Text) + (StructExport ? ".Structure" : ".Files") + ".txt";
            dialog.Filter = "Structure Export|*.*|File List Export|*.*";
            dialog.FilterIndex = StructExport ? 1 : 2;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                RequireLoad.ForEach(new Action<Control>(ControlDisable));
                if (dialog.FilterIndex == 1)
                {
                    ExportStructure(dialog.FileName);
                }
                RequireLoad.ForEach(new Action<Control>(ControlEnable));
            }
        }

        private void ExportStructure(string FileName)
        {
            tslStatus.Text = "Экспорт структуры...";
            try
            {
                var se = new StructureExport(txtTorrentName.Text, trvTorrent.Nodes[0], FileName, trvTorrent.GetNodeCount(true));
                tslStatus.Text = "Структура успешно экспортирована";
            }
            catch
            {
                tslStatus.Text = "Ошибка экспорта структуры";
            }
        }

        private void frmMain_DragDrop(object sender, DragEventArgs e)
        {
            string[] data = (string[]) e.Data.GetData(DataFormats.FileDrop, false);
            LoadTorrent(data[0]);
        }

        private void frmMain_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void frmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveSettings();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            UneditableList.Add("^root/info/pieces");
            UneditableList.Add("^root/info/files[/0-9]+$");
            UneditableList.Add("^root/info/files/[0-9]+/length");
            RequireLoad.Add(txtTorrentName);
            RequireLoad.Add(lstTrackers);
            RequireLoad.Add(btnStructAdd);
            RequireLoad.Add(btnStructRemove);
            RequireLoad.Add(btnStructEdit);
            RequireLoad.Add(btnStructUp);
            RequireLoad.Add(btnStructDown);
            RequireLoad.Add(btnStructReload);
            RequireLoad.ForEach(new Action<Control>(ControlDisable));
            FormPosAndSize();
            tabControlMain.SelectedTab = tabData;
            tabControlSettings.SelectedTab = tabSettingsMain;
        }

        private void frmMain_Shown(object sender, EventArgs e)
        {
            LoadSettings();
            CheckCommandLine();
            tabStructure.Hide();
        }

        private void FormPosAndSize()
        {
            base.Location = new Point(ini.IniReadIntValue("Settings", "FormPosX"), ini.IniReadIntValue("Settings", "FormPosY"));
            base.Size = new Size(ini.IniReadIntValue("Settings", "FormSizeWidth"), ini.IniReadIntValue("Settings", "FormSizeHeight"));
            if (ini.IniReadValue("Settings", "Formstate") == "Normal")
            {
                base.WindowState = FormWindowState.Normal;
            }
            else if (ini.IniReadValue("Settings", "Formstate") == "Minimized")
            {
                base.WindowState = FormWindowState.Minimized;
            }
            else
            {
                base.WindowState = FormWindowState.Normal;
            }
        }

        private void GetAnnounceList()
        {
            List<ListViewItem> list = new List<ListViewItem>();
            string uRL = dNode.NodeVal(NodeConvert("root/announce"));
            list.Add(AddTracker(uRL));
            if (NodeConvert("root/announce-list") != null)
            {
                foreach (TreeNode node in NodeConvert("root/announce-list").Nodes)
                {
                    foreach (TreeNode node2 in node.Nodes)
                    {
                        uRL = dNode.NodeVal(node2) + Environment.NewLine;
                        list.Add(AddTracker(uRL));
                    }
                }
            }
            list.Add(new ListViewItem(new string[] { "New...", "", "", "" }));
            lstTrackers.Items.Clear();
            lstTrackers.Items.AddRange(list.ToArray());
        }

        private bool isUneditable(string Path)
        {
            return Regex.IsMatch(dNode.NodePath(trvTorrent.SelectedNode), Path);
        }

        private void Launch()
        {
            try
            {
                Process.Start(txtLaunchPath.Text, LaunchArgs());
                tslStatus.Text = "Файл успешно передан";
                tmAutoLaunch.Enabled = false;
                if (chkPatchAnnouncer.Checked)
                {
                    Application.Exit();
                }
            }
            catch
            {
                tslStatus.Text = "Ошибка запуска";
            }
        }

        private string LaunchArgs()
        {
            return txtArguments.Text.Replace("%1", "\"" + txtTorrentPath.Text + "\"").Replace("%2", "\"" + Path.GetFileName(txtTorrentPath.Text) + "\"").Replace("%3", "\"" + txtTorrentName.Text + "\"");
        }

        private void ListRenamer(TreeNode Parent)
        {
            if (dNode.NodeType(Parent) == DataType.List)
            {
                foreach (TreeNode node in Parent.Nodes)
                {
                    node.Text = dNode.NodeText(node.Index.ToString(), dNode.NodeType(node), dNode.NodeVal(node));
                }
            }
        }

        private ListViewItem[] ListViewTrackers()
        {
            List<ListViewItem> list = new List<ListViewItem>();
            list.Add(AddTracker(_torrent.AnnounceURL));
            foreach (string str in _torrent.AnnounceList)
            {
                if (str != _torrent.AnnounceURL)
                {
                    list.Add(AddTracker(str));
                }
            }
            list.Add(new ListViewItem(new string[] { "New...", "", "", "" }));
            return list.ToArray();
        }

        private void LoadSettings()
        {
            try
            {
                chkAutoCheckUpdates.Checked = Convert.ToBoolean(ini.IniReadValue("Settings", "AutoCheckUpdates"));
                chkUpdatePatcher.Checked = Convert.ToBoolean(ini.IniReadValue("Settings", "UpdatePatcher"));
                chkUpdateTrackers.Checked = Convert.ToBoolean(ini.IniReadValue("Settings", "UpdateTrackers"));
                txtLaunchPath.Text = ini.IniReadValue("Settings", "LaunchPath");
                txtArguments.Text = ini.IniReadValue("Settings", "LaunchArguments");
                chkSecureEditing.Checked = Convert.ToBoolean(ini.IniReadValue("Settings", "SecureEdit"));
                chkAutoLaunchAllow.Checked = Convert.ToBoolean(ini.IniReadValue("Settings", "AutoLaunch"));
                chkPatchAnnouncer.Checked = Convert.ToBoolean(ini.IniReadValue("Settings", "PatchAnnouncer"));
                chkMagnet.Checked = Convert.ToBoolean(ini.IniReadValue("Settings", "MagnetAnnounce"));
                txtUpdateTrackers.Text = ini.IniReadValue("Settings", "DownloadURL");
                txtUpdatePatcher.Text = ini.IniReadValue("Settings", "UpdateURL");
                chkTrackersCheck.Checked = Convert.ToBoolean(ini.IniReadValue("Settings", "CheckHosts"));
                chkPingCheck.Checked = Convert.ToBoolean(ini.IniReadValue("Settings", "CheckPing"));
                chkStat.Checked = Convert.ToBoolean(ini.IniReadValue("Settings", "AddStat"));
                if (!System.IO.File.Exists(ini.IniReadValue("Settings", "TrackersFile")))
                {
                    System.IO.File.WriteAllText(GetSettingPath() + @"trackerssimple.ini", "[Город]" + Environment.NewLine + "Количество=1" + Environment.NewLine + "1=Санкт-Петербург" + Environment.NewLine + "[Провайдеры Санкт-Петербург]" + Environment.NewLine + "Количество=1" + Environment.NewLine + "1=Корбина" + Environment.NewLine + "[Ретрекеры Санкт-Петербург Корбина]" + Environment.NewLine + "Количество=7" + Environment.NewLine + "1=http://10.121.10.1:2710/announce" + Environment.NewLine + "2=http://netmaster.dyndns.ws:2710/announce" + Environment.NewLine + "3=http://netmaster2.dyndns.ws:2710/announce" + Environment.NewLine + "4=http://netmaster3.dyndns.ws:2710/announce" + Environment.NewLine + "5=http://corbinaretracker.dyndns.org:80/announce.php" + Environment.NewLine + "6=http://netmaster4.dyndns.ws:2710/announce" + Environment.NewLine + "7=http://local-torrent-stats.no-ip.org:2710/announce", Encoding.Default);
                    ini.IniWriteValue("Settings", "TrackersFile", GetSettingPath() + @"trackerssimple.ini");
                }
                IniFile file = new IniFile(ini.IniReadValue("Settings", "TrackersFile"));
                int num = 0;
                cmbCity.Items.Clear();
                for (int i = 1; i <= Convert.ToInt32(file.IniReadValue("Город", "Количество")); i++)
                {
                    cmbCity.Items.Add(file.IniReadValue("Город", i.ToString()));
                    num = i - 1;
                }
                if (num >= Convert.ToInt32(ini.IniReadArray("Settings", "TrackerIniIndex")[0]))
                {
                    cmbCity.SelectedIndex = Convert.ToInt32(ini.IniReadArray("Settings", "TrackerIniIndex")[0]);
                }
                else
                {
                    cmbCity.SelectedIndex = 0;
                }
                cmbCity.Refresh();
            }
            catch (System.Exception /*ex*/)
            {

            }
            if (chkSecureEditing.Checked)
            {
                UneditableList.Add("^root/info");
            }
            if (chkAutoCheckUpdates.Checked && (ini.IniReadDateValue("Settings", "LastLaunch") < DateTime.Now.Date))
            {
                CheckUpdatesNow();
                ini.IniWriteValue("Settings", "LastLaunch", DateTime.Now.Date.ToString());
            }
        }

        private void LoadTorrent(string TorrentPath)
        {
            tslStatus.Text = "Загрузка торрента";
            txtTorrentPath.Text = TorrentPath;
            tslAuthor.Text = "";
            tslAuthor.IsLink = false;
            imgFiles.Images.Clear();
            RequireLoad.ForEach(new Action<Control>(ControlDisable));
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            try
            {
                _torrent = new TorrentParser(TorrentPath);
            }
            catch (Exception exception)
            {
                tslStatus.Text = "Ошибка файла:" + exception.Message;
                stopwatch.Stop();
                return;
            }
            RefreshForm();
            StructurePopulationStart();
            RequireLoad.ForEach(new Action<Control>(ControlEnable));
            stopwatch.Stop();
            tslStatus.Text = "Торрент загружен за " + stopwatch.Elapsed.TotalSeconds.ToString("#0.000") + " секунд";
            AutoTasks();
        }

        private void lstTrackersAddWorking(int[] numbers, ref string[] result)
        {
            for (int i = 0; i < numbers.Length; i++)
            {
                if (numbers[i] > 0)
                {
                    result[i] = lstTrackersAdd.Items[numbers[i] - 1].Text;
                }
            }
        }

        private void MoveNode(TreeNode Node, bool MoveUp)
        {
            if (((trvTorrent.SelectedNode.Index == 0) && MoveUp) || ((trvTorrent.SelectedNode.Index == (trvTorrent.SelectedNode.Parent.Nodes.Count - 1)) && !MoveUp))
            {
                trvTorrent.Focus();
            }
            else if (isUneditable("^root/info/files/[0-9]+$") || (chkSecureEditing.Checked && isUneditable("^root/info/")))
            {
                tslStatus.Text = "Не могу передвинуть защищенные узлы";
                trvTorrent.Focus();
            }
            else
            {
                trvTorrent.SuspendLayout();
                int index = Node.Index + (MoveUp ? -1 : 1);
                TreeNode parent = Node.Parent;
                if (dNode.NodeType(parent) == DataType.List)
                {
                    Node.Text = index.ToString() + Node.Text.Remove(0, Node.Text.IndexOf('('));
                    parent.Nodes[index].Text = Node.Index.ToString() + parent.Nodes[index].Text.Remove(0, parent.Nodes[index].Text.IndexOf('('));
                }
                Node.Remove();
                parent.Nodes.Insert(index, Node);
                trvTorrent.SelectedNode = Node;
                CheckForMainInfo(dNode.NodePath(Node));
                trvTorrent.ResumeLayout();
                trvTorrent.Focus();
            }
        }

        private TreeNode NodeConvert(string NodePath)
        {
            TreeNode node;
            try
            {
                node = trvTorrent.Nodes[NodePath.Substring(0, NodePath.IndexOf("/"))];
            }
            catch
            {
                return trvTorrent.Nodes[NodePath];
            }
            NodePath = NodePath.Remove(0, NodePath.IndexOf("/") + 1);
            foreach (string str in NodePath.Split(new char[] { '/' }))
            {
                if (dNode.NodeType(node) == DataType.List)
                {
                    node = node.Nodes[int.Parse(str)];
                }
                else
                {
                    node = node.Nodes[str];
                }
            }
            return node;
        }

        private TreeNode NodeConvert(string NodePath, bool CreateIfNotFound, DataType Type)
        {
            if (CreateIfNotFound && (NodeConvert(NodePath) == null))
            {
                string key = NodePath.Substring(NodePath.LastIndexOf('/') + 1);
                string nodePath = NodePath.Substring(0, NodePath.LastIndexOf('/'));
                SetNodeImage(NodeConvert(nodePath).Nodes.Add(key, dNode.NodeText(key, Type, "")));
            }
            return NodeConvert(NodePath);
        }

        private string OptimalSize(long s)
        {
            if (s > 0x3b9aca00L)
            {
                decimal num = s / 1073741824M;
                return (num.ToString("####.##") + " GB");
            }
            if (s > 0xf4240L)
            {
                decimal num2 = s / 1048576M;
                return (num2.ToString("####.##") + " MB");
            }
            if (s > 0x3e8L)
            {
                decimal num3 = s / 1024M;
                return (num3.ToString("####.##") + " KB");
            }
            return (s.ToString("####.##") + " B");
        }

        private void PopulateStructure(TreeNode ParentTN, Dictionary<string, TVal> ValToAdd)
        {
            string str = "";
            foreach (string str2 in ValToAdd.Keys)
            {
                switch (ValToAdd[str2].Type)
                {
                    case DataType.Int:
                    {
                        long dObject = (long) ValToAdd[str2].dObject;
                        ParentTN.Nodes.Add(str2, str2 + "(i)=" + dObject.ToString());
                        break;
                    }
                    case DataType.List:
                        ParentTN.Nodes.Add(str2, str2 + "(l)");
                        PopulateStructure(ParentTN.Nodes[str2], (List<TVal>) ValToAdd[str2].dObject);
                        break;

                    case DataType.Dictionary:
                        ParentTN.Nodes.Add(str2, str2 + "(d)");
                        PopulateStructure(ParentTN.Nodes[str2], (Dictionary<string, TVal>) ValToAdd[str2].dObject);
                        break;

                    case DataType.Byte:
                    {
                        str = "";
                        byte[] buffer = (byte[]) ValToAdd[str2].dObject;
                        str = (buffer.Length > 100) ? (BitConverter.ToString(buffer).Replace("-", "").Substring(0, 100) + "...") : BitConverter.ToString(buffer).Replace("-", "");
                        string[] strArray = new string[] { str2, "(s)[", buffer.Length.ToString(), "]=", str };
                        ParentTN.Nodes.Add(str2, string.Concat(strArray));
                        break;
                    }
                    default:
                        str = (string) ValToAdd[str2].dObject;
                        ParentTN.Nodes.Add(str2, str2 + "(s)[" + str.Length.ToString() + "]=" + str);
                        break;
                }
                SetNodeImage(ParentTN.Nodes[str2]);
            }
        }

        private void PopulateStructure(TreeNode ParentTN, List<TVal> ValToAdd)
        {
            int num = 0;
            string str = "";
            foreach (TVal val in ValToAdd)
            {
                switch (val.Type)
                {
                    case DataType.Int:
                        ParentTN.Nodes.Add(num.ToString() + "(i)=" + val.dObject.ToString());
                        break;

                    case DataType.List:
                        ParentTN.Nodes.Add(num.ToString() + "(l)");
                        PopulateStructure(ParentTN.Nodes[num], (List<TVal>) val.dObject);
                        break;

                    case DataType.Dictionary:
                        ParentTN.Nodes.Add(num.ToString() + "(d)");
                        PopulateStructure(ParentTN.Nodes[num], (Dictionary<string, TVal>) val.dObject);
                        break;

                    case DataType.Byte:
                    {
                        str = "";
                        byte[] dObject = (byte[]) val.dObject;
                        str = (dObject.Length > 100) ? (BitConverter.ToString(dObject).Replace("-", "").Substring(0, 100) + "...") : BitConverter.ToString(dObject).Replace("-", "");
                        string[] strArray = new string[] { num.ToString(), "(s)[", dObject.Length.ToString(), "]=", str };
                        ParentTN.Nodes.Add(string.Concat(strArray));
                        break;
                    }
                    default:
                        str = (string) val.dObject;
                        ParentTN.Nodes.Add(num.ToString() + "(s)[" + str.Length.ToString() + "]=" + str);
                        break;
                }
                SetNodeImage(ParentTN.Nodes[num]);
                num++;
            }
        }

        private void RefreshForm()
        {
            txtTorrentName.Text = _torrent.Name;
            lstTrackers.Items.Clear();
            lstTrackers.Items.AddRange(ListViewTrackers());
        }

        private void SaveSettings()
        {
            ini.IniWriteValue("Settings", "AutoCheckUpdates", chkAutoCheckUpdates.Checked.ToString());
            ini.IniWriteValue("Settings", "UpdatePatcher", chkUpdatePatcher.Checked.ToString());
            ini.IniWriteValue("Settings", "UpdateTrackers", chkUpdateTrackers.Checked.ToString());
            ini.IniWriteValue("Settings", "CheckHosts", chkTrackersCheck.Checked.ToString());
            ini.IniWriteValue("Settings", "CheckPing", chkPingCheck.Checked.ToString());
            ini.IniWriteValue("Settings", "AddStat", chkStat.Checked.ToString());
            ini.IniWriteValue("Settings", "LaunchPath", txtLaunchPath.Text);
            ini.IniWriteValue("Settings", "LaunchArguments", txtArguments.Text);
            ini.IniWriteValue("Settings", "SecureEdit", chkSecureEditing.Checked.ToString());
            ini.IniWriteValue("Settings", "AutoLaunch", chkAutoLaunchAllow.Checked.ToString());
            ini.IniWriteValue("Settings", "PatchAnnouncer", chkPatchAnnouncer.Checked.ToString());
            ini.IniWriteValue("Settings", "MagnetAnnounce", chkMagnet.Checked.ToString());
            ini.IniWriteValue("Settings", "DownloadURL", txtUpdateTrackers.Text);
            ini.IniWriteValue("Settings", "UpdateURL", txtUpdatePatcher.Text);
            ini.IniWriteValue("Settings", "TrackerIniIndex", cmbCity.SelectedIndex.ToString() + " " + cmbISP.SelectedIndex.ToString());
            if (ini.IniReadBoolValue("Settings", "FirstRun") & !chkTrackersCheck.Checked)
            {
                tabControlMain.SelectedTab = tabSettings;
                tabControlSettings.SelectedTab = tabSettingsMain;
                Application.DoEvents();
                btnCheckTrackers_Click(null, null);
            }
            ini.IniWriteValue("Settings", "FirstRun", false.ToString());
            if (base.WindowState != FormWindowState.Normal)
            {
                ini.IniWriteValue("Settings", "FormPosX", base.RestoreBounds.Location.X.ToString());
                ini.IniWriteValue("Settings", "FormPosY", base.RestoreBounds.Location.Y.ToString());
                ini.IniWriteValue("Settings", "FormSizeHeight", base.RestoreBounds.Size.Height.ToString());
                ini.IniWriteValue("Settings", "FormSizeWidth", base.RestoreBounds.Size.Width.ToString());
            }
            else
            {
                ini.IniWriteValue("Settings", "FormPosX", base.Location.X.ToString());
                ini.IniWriteValue("Settings", "FormPosY", base.Location.Y.ToString());
                ini.IniWriteValue("Settings", "FormSizeHeight", base.Size.Height.ToString());
                ini.IniWriteValue("Settings", "FormSizeWidth", base.Size.Width.ToString());
            }
            ini.IniWriteValue("Settings", "FormState", base.WindowState.ToString());
        }

        private void SetNodeImage(TreeNode Node)
        {
            Node.ImageKey = str.GetBetween(Node.Text, "(", 1);
            Node.SelectedImageKey = Node.ImageKey;
        }

        private void StartAutoLaunch()
        {
            if (chkAutoLaunchAllow.Checked)
            {
                tmAutoLaunch.Enabled = true;
            }
        }

        private void StructurePopulationStart()
        {
            tslStatus.Text = "Создание структуры...";
            trvTorrent.SuspendLayout();
            trvTorrent.Nodes.Clear();
            trvTorrent.Nodes.Add("root", "root(d)");
            trvTorrent.Nodes["root"].ImageKey = "d";
            trvTorrent.Nodes["root"].SelectedImageKey = "d";
            PopulateStructure(trvTorrent.Nodes["root"], _torrent.Root);
            trvTorrent.Nodes["root"].Expand();
            trvTorrent.Nodes["root"].Nodes["info"].Expand();
            trvTorrent.ResumeLayout();
        }

        private void TabControl2SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControlSettings.SelectedIndex == 1)
            {
                btnAssocFile.Enabled = !CheckForAssoc();
            }
        }

        private void tmAutoLaunch_Tick(object sender, EventArgs e)
        {
            Application.DoEvents();
            Launch();
        }

        private void tmiNode_Click(object sender, EventArgs e)
        {
            switch (((ToolStripMenuItem) sender).Name)
            {
                case "tmiCollapseAll":
                    trvTorrent.CollapseAll();
                    return;

                case "tmiExpandAll":
                    trvTorrent.ExpandAll();
                    return;

                case "tmiCollapseNode":
                    trvTorrent.SelectedNode.Collapse(false);
                    return;
            }
            trvTorrent.SelectedNode.ExpandAll();
        }

        private void ToolStripStatusLabel1Click(object sender, EventArgs e)
        {
            Process.Start("http://re-tracker.ru/index.php?showforum=9");
        }

        private void trvTorrent_MouseClick(object sender, MouseEventArgs e)
        {
            if (trvTorrent.Enabled && (e.Button == MouseButtons.Right))
            {
                trvTorrent.SelectedNode = trvTorrent.GetNodeAt(e.Location);
                TrvTorrentAfterSelect(null, new TreeViewEventArgs(trvTorrent.SelectedNode, TreeViewAction.ByMouse));
                cmsStructure_Opened(null, null);
            }
        }

        private void TrvTorrentAfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Action != TreeViewAction.Unknown)
            {
                lblStructPos.Text = dNode.NodePath(e.Node).Replace("/", " / ");
            }
            else
            {
                lblStructPos.Text = "";
            }
        }

        private void txtName_Leave(object sender, EventArgs e)
        {
            ChangeString(NodeConvert("root/info/name"), "name", txtTorrentName.Text);
        }

        private void UpdateStructCallBack(string Path, string Name, DataType Type, string Value)
        {
            string str;
            if (Path.EndsWith(Name))
            {
                str = dNode.NodeText(Name, Type, Value);
                NodeConvert(Path, true, Type).Text = str;
                trvTorrent.SelectedNode = NodeConvert(Path);
            }
            else
            {
                TreeNode node = NodeConvert(Path);
                TreeNode parent = node.Parent;
                Name = CheckExist(parent, Name);
                str = dNode.NodeText(Name, Type, Value);
                node.Name = Name;
                trvTorrent.SelectedNode = node;
            }
            if (dNode.NodeType(trvTorrent.SelectedNode.Parent) == DataType.List)
            {
                ListRenamer(trvTorrent.SelectedNode.Parent);
            }
            SetNodeImage(trvTorrent.SelectedNode);
            if ((Type == DataType.Int) || (Type == DataType.String))
            {
                trvTorrent.SelectedNode.Nodes.Clear();
            }
            CheckForMainInfo(dNode.NodePath(trvTorrent.SelectedNode));
            trvTorrent.Focus();
        }

        private void UpdateTrackerStructure()
        {
            NodeConvert("root/announce").Text = dNode.NodeText("announce", DataType.String, lstTrackers.Items[0].Text);
            List<TreeNode> list = new List<TreeNode>();
            Regex regex = new Regex("(http|https|udp)://(.*)");
            for (int i = 0; i < lstTrackers.Items.Count; i++)
            {
                if (regex.IsMatch(lstTrackers.Items[i].Text))
                {
                    string text = lstTrackers.Items[i].Text;
                    TreeNode node = new TreeNode("0(s)[" + text.Length.ToString() + "]=" + text);
                    SetNodeImage(node);
                    TreeNode node2 = new TreeNode(list.Count.ToString() + "(l)");
                    SetNodeImage(node2);
                    node2.Nodes.Add(node);
                    list.Add(node2);
                }
            }
            TreeNode node3 = NodeConvert("root/announce-list", true, DataType.List);
            if (list.Count == 0)
            {
                node3.Remove();
            }
            else
            {
                node3.Nodes.Clear();
                node3.Nodes.AddRange(list.ToArray());
            }
        }

        private void WaitAll(WaitHandle[] waitHandles)
        {
            barCheck.Maximum = waitHandles.Length;
            barCheck.Minimum = 1;
            barCheck.Value = 1;
            barCheck.Step = 1;
            for (int i = 0; i < waitHandles.Length; i++)
            {
                barCheck.PerformStep();
                barCheck.Refresh();
                WaitHandle.WaitAny(new WaitHandle[] { waitHandles[i] });
            }
        }
    }
}

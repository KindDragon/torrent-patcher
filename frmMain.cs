using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;
using TorrentUtilities;
using Windows7.DesktopIntegration;

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
		private int timeout = 500;

		bool IsWin7OrAbove()
		{
			OperatingSystem os = Environment.OSVersion;
			Version vs = os.Version;
			if (os.Platform == PlatformID.Win32NT)
			{
				if (vs.Major == 6 && vs.Minor >= 1 || vs.Major > 6)
					return true;
			}
			return false;
		}

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
					"Timeout=500" + Environment.NewLine +
					"AddStat=False" + Environment.NewLine + 
					"AddRetrackerLocal=True" + Environment.NewLine + 
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
					"FormSizeWidth=" + MinimumSize.Width.ToString() + Environment.NewLine +
					"FormSizeHeight=" + MinimumSize.Height.ToString() + Environment.NewLine + 
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
				MessageBox.Show("Пожалуйста, выберите своего провайдера в выпадающем списке\n" +
					"и путь к программе закачки торрент-файлов", 
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
			int retrackersCount = lstTrackersAdd.Items.Count - 1;
			string[] result = new string[retrackersCount];
			int[] numbers = new int[retrackersCount];
			string str = null;
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

		private void btnFileExport_Click(object sender, EventArgs e)
		{
			Export(false);
		}

		private void btnLaunchBrowse_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog dialog = new OpenFileDialog())
			{
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
		}

		private void btnSave_Click(object sender, EventArgs e)
		{
			string name = dNode.NodeVal(FindNode("root/encoding"));
			name = (name != "") ? name : "UTF-8";
			Encoding enc = Encoding.GetEncoding(name);
			using (TorrentWriter tw = new TorrentWriter(UpdateDictionary(trvTorrent.Nodes["root"])))
			{
				tw.Write(txtTorrentPath.Text);
			}
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
				trvTorrent.SelectedNode = AddNode(selectedNode, selectedNode.Nodes.Count.ToString());
			}
			else
			{
				string key = CheckExist(selectedNode, "newval");
				TVal val = new TVal(DataType.String, "");
				trvTorrent.SelectedNode = AddNode(selectedNode, key, val);
			}
			if (UneditableList.Exists(new Predicate<string>(isUneditable)))
			{
				tslStatus.Text = "Значение недоступно для правки";
				trvTorrent.SelectedNode.Remove();
			}
			else
			{
				using (frmEdit frm = new frmEdit(false, dNode.NodePath(trvTorrent.SelectedNode), DataType.String, "", parentList,
					new dSructureUpdate(UpdateStructCallBack)))
				{
					frm.ShowDialog();
				}
			}
		}

		private TreeNode AddNode(string key, TVal val)
		{
			TreeNode node = new TreeNode(key + val.ToString());
			node.Name = key;
			node.Tag = val;
			SetNodeImage(node);
			return node;
		}

		private TreeNode AddNode(string key, string str = "")
		{
			TVal val = new TVal(DataType.String, str);
			return AddNode(key, val);
		}

		private TreeNode AddNode(TreeNode rootnode, string key, TVal val)
		{
			TreeNode node = rootnode.Nodes.Add(key, key + val.ToString());
			node.Tag = val;
			SetNodeImage(node);
			return node;
		}

		private TreeNode AddNode(TreeView rootnode, string key, TVal val)
		{
			TreeNode node = rootnode.Nodes.Add(key, key + val.ToString());
			node.Tag = val;
			SetNodeImage(node);
			return node;
		}

		private TreeNode AddNode(TreeNode rootnode, string key, string str = "")
		{
			TVal val = new TVal(DataType.String, str);
			return AddNode(rootnode, key, val);
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
					using (frmEdit frm = new frmEdit(true, dNode.NodePath(trvTorrent.SelectedNode), type,
						dNode.NodeVal(trvTorrent.SelectedNode), parentList, new dSructureUpdate(UpdateStructCallBack)))
					{ 
						frm.ShowDialog();
					}
				}
			}
		}

		private void btnStructExport_Click(object sender, EventArgs e)
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
			using (OpenFileDialog dialog = new OpenFileDialog())
			{
				dialog.CheckFileExists = true;
				dialog.CheckPathExists = true;
				dialog.Filter = "INI файлы (*.ini)|*.ini|Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*";
				dialog.FilterIndex = 0;
				dialog.Multiselect = false;
				if (dialog.ShowDialog() != DialogResult.OK)
					return;

				ini.IniWriteValue("Settings", "TrackersFile", dialog.FileName);
				LoadRetrackersFile();

				if (!chkTrackersCheck.Checked)
					btnCheckTrackers_Click(null, null);
			}
		}

		private void ChangeValue(TreeNode changeNT, string name, TVal val)
		{
			changeNT.Text = name + val.ToString();
			changeNT.Tag = val;
		}

		private void ChangeString(TreeNode changeTN, string name, string NewString)
		{
			TVal val = new TVal(DataType.String, NewString);
			ChangeValue(changeTN, name, val);
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
			bool bAddStatistic = chkStat.Checked;
			int lastIndex = lstTrackers.Items.Count - 1;
			if (lstTrackers.Items[lastIndex].SubItems[0].Text == addNewTracker)
				lstTrackers.Items[lastIndex].Remove();
			Regex regex = new Regex("(http|https|udp)://(.*)");
			for (int j = 0; j < lstTrackersAdd.Items.Count; j++)
			{
				string retracker = lstTrackersAdd.Items[j].Text;
				bool exist = !regex.IsMatch(retracker);
				if (!exist)
				{
					for (int k = 0; k < lstTrackers.Items.Count; k++) // skip last New... item
					{
						string announce = lstTrackers.Items[k].SubItems[0].Text;
						string announceWithoutStat = announce.Split('?')[0];
						if (!regex.IsMatch(announce))
						{
							lstTrackers.Items[k].Remove();
							k--;
							exist = false;
						}
						else if (retracker == announceWithoutStat)
						{
							exist = true;
						}
					}
				}
				if (!exist)
				{
					if (!bAddStatistic)
					{
						lstTrackers.Items.Add(retracker);
					}
					else
					{
						string retrackerWithStat = String.Concat( retracker, "?name=", _torrent.Name,
									"&size=", _torrent.Size.ToString(), "&comment=", _torrent.Comment,
									"&isp=", (cmbCity.SelectedIndex + 1).ToString(), "+", 
									(cmbISP.SelectedIndex + 1).ToString() );
						lstTrackers.Items.Add(retrackerWithStat);
					}
				}
			}
			if (chkAddRetrackerLocal.Checked)
			{
				string retracker = "http://retracker.local/announce";
				bool exist = false;
				for (int k = 0; k < lstTrackers.Items.Count; k++) // skip last New... item
				{
					string announce = lstTrackers.Items[k].SubItems[0].Text;
					string announceWithoutStat = announce.Split('?')[0];
					if (!regex.IsMatch(announce))
					{
						lstTrackers.Items[k].Remove();
						k--;
						exist = false;
					}
					else if (retracker == announceWithoutStat)
					{
						exist = true;
					}
				}
				if (!exist)
					lstTrackers.Items.Add(retracker);
			}
			UpdateTrackerStructure();
			lstTrackers.Items.Add(new ListViewItem(new string[] { addNewTracker, "", "", "" }));
			string folder = GetSettingPath() + "Torrents";
			Directory.CreateDirectory(folder);
			int startIndex = torrentPath.LastIndexOf(@"\");
			string destTorrentFile = folder + torrentPath.Substring(startIndex);
			txtTorrentPath.Text = destTorrentFile;
			btnSave_Click(null, null);
			stopwatch.Stop();
			ini.IniWriteValue("Performance", DateTime.Now.ToString() + " rewrite", stopwatch.ElapsedMilliseconds.ToString());
			tmAutoLaunch.Enabled = true;
		}

		private void PatchMagnetLink(Stopwatch stopwatch)
		{
			Regex regex = new Regex("(http|https|udp)://(.*)");
			StringBuilder arguments = new StringBuilder("magnet:?xt=urn:btih:" + _torrent.SHAHash);
			for (int m = 0; m < lstTrackersAdd.Items.Count; m++)
			{
				string retracker = lstTrackersAdd.Items[m].Text;
				if (regex.IsMatch(retracker))
				{
					if (!chkStat.Checked)
					{
						arguments.Append("&tr=");
						arguments.Append(retracker);
					}
					else
					{
						arguments.AppendFormat(
							"&tr={0}?name={1}&size={2}&comment={3}&isp={4}+{5}", 
							retracker, _torrent.Name, _torrent.Size, _torrent.Comment, 
							cmbCity.SelectedIndex + 1, cmbISP.SelectedIndex + 1);
					}
				}
			}
			if (chkAddRetrackerLocal.Checked)
			{
				string retracker = "http://retracker.local/announce";
				arguments.Append("&tr=");
				arguments.Append(retracker);
			}
			try
			{
				Process.Start(txtLaunchPath.Text, LaunchArgs());
				tslStatus.Text = "Файл успешно передан";
				stopwatch.Stop();
				ini.IniWriteValue("Performance", DateTime.Now.ToString() + " magnet", stopwatch.ElapsedMilliseconds.ToString());
				if (MessageBox.Show("Торрент файл будет пропатчен безопасно по одной из причин:\n" +
					"а) наличия в торренте флага private\n" +
					"б) установленной настройке MAGNET в Настройки->Дополнительно\n\n" + 
					"ПОЖАЛУЙСТА НЕ НАЖИМАЙТЕ ОК, ПОКА НЕ ДОБАВИТЕ ЗАКАЧКУ В ТОРРЕНТ-КЛИЕНТ", 
					Application.ProductName + Application.ProductVersion, MessageBoxButtons.OKCancel, 
					MessageBoxIcon.Asterisk) == DialogResult.OK)
				{
					Process.Start(txtLaunchPath.Text, arguments.ToString());
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
			if (!FindNode(Path).Nodes.ContainsKey(Name))
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
					(((string) key.OpenSubKey("shell").OpenSubKey("open").OpenSubKey("command").GetValue("")) == ("\"" + 
					Application.ExecutablePath + "\" %1")))
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
			if (FindNode(Path) != null)
			{
				str = dNode.NodeVal(FindNode(Path));
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

		/// <summary>
		/// checks connection to all addresses
		/// </summary>
		/// <returns>list of successful connections</returns>
		private int[] CheckTrackers()
		{
			bool result = false;
			string[] splitter = new string[] { "http://", "/", ":" };
			int[] numbers = new int[lstTrackersAdd.Items.Count - 1];
			int errors = 0;

			ManualResetEvent[] doneEvents = new ManualResetEvent[lstTrackersAdd.Items.Count - 1];
			TaskInfo[] CheckArray = new TaskInfo[lstTrackersAdd.Items.Count - 1];

			for (int i = 0; i < lstTrackersAdd.Items.Count - 1; i++)
			{
				string[] split = lstTrackersAdd.Items[i].Text.Split(splitter, StringSplitOptions.RemoveEmptyEntries);
				doneEvents[i] = new ManualResetEvent(false);
				int port;
				if (!Int32.TryParse(split[1], out port))
					port = 80;
				TaskInfo ti;
				if (chkPingCheck.Checked)
					ti = new TaskInfo(split[0], result, doneEvents[i], timeout);
				else
					ti = new TaskInfo(split[0], port, result, doneEvents[i], timeout);
				CheckArray[i] = ti;
				ThreadPool.QueueUserWorkItem(ti.ThreadPoolCallback, i);
			}

			WaitAll(doneEvents);
			int j = 0;
			for (int i = 0; i < lstTrackersAdd.Items.Count - 1; i++)
			{
				if (CheckArray[i].Result)
					numbers[j++] = i + 1;
				else
					errors++;
			}
			if (errors > 0)
				tslStatus.Text = "Доступно " + (lstTrackersAdd.Items.Count - errors - 1).ToString() + " из " + (lstTrackersAdd.Items.Count - 1).ToString() + " ретрекеров";
			else
				tslStatus.Text = "Доступны все ретрекеры";
			return numbers;

		}

		public IPAddress GetExternalIP(string ipAddressOrHostName)
		{
			IPAddress ipAddress = Dns.GetHostEntry(ipAddressOrHostName).AddressList[0];
			StringBuilder traceResults = new StringBuilder();
			using (Ping pingSender = new Ping())
			{
				PingOptions pingOptions = new PingOptions();
				byte[] bytes = new byte[32];
				pingOptions.DontFragment = true;
				pingOptions.Ttl = 1;
				PingReply pingReply = pingSender.Send(
					ipAddress,
					5000,
					new byte[32], pingOptions);
				return pingReply.Address;
			}
		}

		private string GetCity()
		{
			IPAddress externalIP = GetExternalIP("www.google.com");
			if (externalIP == null)
				return "";

			try
			{
				GEOIP.IPInfo? s = GEOIP.getSingleIPInfo(externalIP.ToString());
				if (!s.HasValue)
					return "";
				return s.Value.city;
			}
			catch (System.Exception /*ex*/)
			{
				return "";
			}
		}

		private void CheckUpdatesNow()
		{
			tslStatus.Text = "Проверка обновлений...";
			Application.DoEvents();
			if (chkUpdatePatcher.Checked)
				CheckPatcherUpdates();
			if (chkUpdateTrackers.Checked)
				CheckTrackersListUpdates();
			tslStatus.Text = "";
		}

		private void CheckPatcherUpdates()
		{
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ini.IniReadValue("Settings", "VersionCheck"));
			request.UserAgent = "TorrentPatcher/" + Application.ProductVersion;
			request.Credentials = CredentialCache.DefaultCredentials;
			try
			{
				Stream webStream = request.GetResponse().GetResponseStream();
				string version = new StreamReader(webStream).ReadToEnd();
				if (String.CompareOrdinal(Application.ProductVersion, version) >= 0)
				{
					tslStatus.Text = "У вас последняя версия патчера.";
				}
				else
				{
					tslStatus.Text = "Новая версия (" + version + ")";
					if (MessageBox.Show("Версия " + version + " доступна. Хотите загрузить новую версию?",
						Application.ProductName + Application.ProductVersion, MessageBoxButtons.YesNo,
						MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
					{
						Process.Start(txtUpdatePatcher.Text);
					}
				}
			}
			catch (Exception exception)
			{
				tslStatus.Text = "ОШИБКА:" + exception.Message;
			}
		}

		private void CheckTrackersListUpdates()
		{
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(txtUpdateTrackers.Text);
			request.UserAgent = "TorrentPatcher/" + Application.ProductVersion;
			string contents = null;
			request.Credentials = CredentialCache.DefaultCredentials;
			WebResponse response = null;
			string errorMsg = "";
			try
			{
				response = request.GetResponse();
				if (response.ContentLength > -1L)
				{
					long length;
					string trackersFile = ini.IniReadValue("Settings", "TrackersFile");
					if (System.IO.File.Exists(trackersFile))
					{
						FileInfo info = new FileInfo(trackersFile);
						length = info.Length;
					}
					else
					{
						length = -1;
					}
					if (response.ContentLength != length)
					{
						contents = new StreamReader(response.GetResponseStream()).ReadToEnd();
						if (contents == null)
							return;
						System.IO.File.WriteAllText(trackersFile, contents, Encoding.Unicode);
						LoadRetrackersFile();
						tslStatus.Text = "Трекер-лист обновлен успешно.";
					}
					else
					{
						tslStatus.Text = "У вас последняя версия трекер-листа.";
					}
				}
				else
				{
					errorMsg = "Нет связи с " + txtUpdateTrackers.Text;
				}
				if (ini.IniReadBoolValue("Settings", "FirstRun")) // autodetect current city
				{
					string city = GetCity();
					if (city != "")
					{
						for (int i = 0; i < cmbCity.Items.Count; i++)
						{
							string currentCity = cmbCity.Items[i].ToString();
							if (String.Compare(currentCity, city, true) == 0)
							{
								cmbCity.SelectedIndex = i;
								cmbCity.Refresh();
								break;
							}
						}
					}
				}
				else if (!chkTrackersCheck.Checked &&
					(ini.IniReadDateValue("Settings", "LastLaunch").AddDays(7.0) < DateTime.Now.Date))
				{
					btnCheckTrackers_Click(null, null);
				}
			}
			catch (Exception exception2)
			{
				errorMsg = errorMsg + exception2.Message;
				tslStatus.Text = "ОШИБКА:" + errorMsg;
			}
		}

		private void chkSecureEditing_CheckedChanged(object sender, EventArgs e)
		{
			if (_InnerCheck)
				return;

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

		string GetISPIniSectionName(string sCity)
		{
			StringBuilder sb = new StringBuilder("Провайдеры ");
			sb.Append(sCity);
			return sb.ToString();
		}

		string GetISPIniSectionName(IniFile file, int cityIndex)
		{
			string sCity = file.IniReadValue("Город", cityIndex);
			return GetISPIniSectionName(sCity);
		}

		string GetRetrackerIniSectionName(IniFile file, int cityIndex, int providerIndex)
		{
			StringBuilder sb = new StringBuilder("Ретрекеры ");
			string sCity = file.IniReadValue("Город", cityIndex);
			sb.Append(sCity);
			sb.Append(" ");
			string sProvider = GetISPIniSectionName(sCity);
			sb.Append(file.IniReadValue(sProvider, providerIndex));
			return sb.ToString();
		}

		private void comboBoxCity_SelectedIndexChanged(object sender, EventArgs e)
		{
			string trackersFile = ini.IniReadValue("Settings", "TrackersFile").Replace("txt", "ini");
			IniFile initrackers = new IniFile(trackersFile);
			cmbISP.Items.Clear();
			string ISPCity = GetISPIniSectionName(initrackers, cmbCity.SelectedIndex + 1);
			int ISPCount = initrackers.IniReadIntValue(ISPCity, "Количество");
			for (int i = 1; i <= ISPCount; i++)
				cmbISP.Items.Add(initrackers.IniReadValue(ISPCity, i.ToString()));
			var indexes = ini.IniReadArray("Settings", "TrackerIniIndex");
			int ISPIndex = indexes.Length >= 2 ? Convert.ToInt32(indexes[1]) : 0;
			if (ISPIndex < 0 || ISPIndex >= cmbISP.Items.Count)
				ISPIndex = 0;
			if (cmbISP.Items.Count > 0)
				cmbISP.SelectedIndex = ISPIndex;
			else
				cmbISP.Text = "";
			cmbISP.Refresh();
		}

		private void comboBoxISP_SelectedIndexChanged(object sender, EventArgs e)
		{
			IniFile file = new IniFile(ini.IniReadValue("Settings", "TrackersFile"));
			lstTrackersAdd.Items.Clear();
			int cityIndex = cmbCity.SelectedIndex + 1;
			int ISPIndex = cmbISP.SelectedIndex + 1;
			var trackerCheck = ini.IniReadArray("Settings", "TrackerCheck");
			string trackerName = GetRetrackerIniSectionName(file, cityIndex, ISPIndex);
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

		private TVal UpdateDictionary(TreeNode Node)
		{
			TVal val = (TVal)Node.Tag;
			Debug.Assert(val.Type == DataType.Dictionary);
			Dictionary<string, TVal> dictionary = new Dictionary<string, TVal>();
			val = new TVal(DataType.Dictionary, dictionary);
			foreach (TreeNode node in Node.Nodes)
				dictionary.Add(node.Name, UpdateValue(node));
			return val;
		}

		private TVal UpdateList(TreeNode Node)
		{
			TVal val = (TVal)Node.Tag;
			Debug.Assert(val.Type == DataType.List);
			List<TVal> list = new List<TVal>();
			val = new TVal(DataType.List, list);
			foreach (TreeNode node in Node.Nodes)
				list.Add(UpdateValue(node));
			return val;
		}

		private TVal UpdateValue(TreeNode Node)
		{
			TVal val = (TVal)Node.Tag;
			switch (val.Type)
			{
				case DataType.Dictionary:
					val = UpdateDictionary(Node);
					break;
				case DataType.List:
					val = UpdateList(Node);
					break;
				case DataType.Byte:
					if (dNode.NodePath(Node) == "root/info/pieces")
						val = new TVal(DataType.Byte, _torrent.GetPieces());
					break;
			}
			Node.Tag = val;
			return val;
		}

		private void EditStructFile(int index, string Name, string Path)
		{
			if (_torrent.IsSingle)
			{
				EditStructOneFile(Name);
			}
			else
			{
				TreeNode node = FindNode("root/info/files/" + index.ToString() + "/path");
				node.Nodes.Clear();
				int num = 0;
				string[] elements = (Path + @"\" + Name).Split(new string[] { @"\" }, StringSplitOptions.RemoveEmptyEntries);
				foreach (string str in elements)
				{
					AddNode(node, num.ToString(), str);
					num++;
				}
			}
		}

		private void EditStructOneFile(string Name)
		{
			FindNode("root/info/name").Text = "name(s)[" + Name.Length.ToString() + "]=" + Name;
			CheckForMainInfo("root/info/name");
		}

		private void Export(bool StructExport)
		{
			using (SaveFileDialog dialog = new SaveFileDialog())
			{
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
		}

		private void ExportStructure(string FileName)
		{
			tslStatus.Text = "Экспорт структуры...";
			try
			{
				using (var se = new StructureExport())
				{
					se.WriteToFile(txtTorrentName.Text, trvTorrent.Nodes[0], FileName, trvTorrent.GetNodeCount(true));
				}
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
			LoadTorrent(data[0], true);
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
			Point formPos = new Point(ini.IniReadIntValue("Settings", "FormPosX"), ini.IniReadIntValue("Settings", "FormPosY"));
			Size formSize = new Size(ini.IniReadIntValue("Settings", "FormSizeWidth"), ini.IniReadIntValue("Settings", "FormSizeHeight"));
			base.Location = formPos;
			base.Size = formSize;
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

		static string addNewTracker = "New...";

		private void GetAnnounceList()
		{
			List<ListViewItem> list = new List<ListViewItem>();
			string uRL = dNode.NodeVal(FindNode("root/announce"));
			list.Add(AddTracker(uRL));
			if (FindNode("root/announce-list") != null)
			{
				foreach (TreeNode node in FindNode("root/announce-list").Nodes)
				{
					foreach (TreeNode node2 in node.Nodes)
					{
						uRL = dNode.NodeVal(node2) + Environment.NewLine;
						list.Add(AddTracker(uRL));
					}
				}
			}
			list.Add(new ListViewItem(new string[] { addNewTracker, "", "", "" }));
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
			return txtArguments.Text.Replace("%1", "\"" + txtTorrentPath.Text + "\"").
				Replace("%2", "\"" + Path.GetFileName(txtTorrentPath.Text) + "\"").
				Replace("%3", "\"" + txtTorrentName.Text + "\"");
		}

		private void ListRenamer(TreeNode Parent)
		{
			if (dNode.NodeType(Parent) == DataType.List)
			{
				foreach (TreeNode node in Parent.Nodes)
				{
					TVal val = (TVal)node.Tag;
					node.Text = node.Index.ToString() + val.ToString();
				}
			}
		}

		private ListViewItem[] ListViewTrackers()
		{
			List<ListViewItem> list = new List<ListViewItem>();
			list.Add(AddTracker(_torrent.AnnounceURL));
			foreach (string str in _torrent.GetAnnounceList())
			{
				if (str != _torrent.AnnounceURL)
				{
					list.Add(AddTracker(str));
				}
			}
			list.Add(new ListViewItem(new string[] { addNewTracker, "", "", "" }));
			return list.ToArray();
		}

		private void LoadSettings()
		{
			try
			{
				chkAutoCheckUpdates.Checked = ini.IniReadBoolValue("Settings", "AutoCheckUpdates");
				chkUpdatePatcher.Checked = ini.IniReadBoolValue("Settings", "UpdatePatcher");
				chkUpdateTrackers.Checked = ini.IniReadBoolValue("Settings", "UpdateTrackers");
				txtLaunchPath.Text = ini.IniReadValue("Settings", "LaunchPath");
				txtArguments.Text = ini.IniReadValue("Settings", "LaunchArguments");
				chkSecureEditing.Checked = ini.IniReadBoolValue("Settings", "SecureEdit");
				chkAutoLaunchAllow.Checked = ini.IniReadBoolValue("Settings", "AutoLaunch");
				chkPatchAnnouncer.Checked = ini.IniReadBoolValue("Settings", "PatchAnnouncer");
				chkMagnet.Checked = ini.IniReadBoolValue("Settings", "MagnetAnnounce");
				txtUpdateTrackers.Text = ini.IniReadValue("Settings", "DownloadURL");
				txtUpdatePatcher.Text = ini.IniReadValue("Settings", "UpdateURL");
				chkTrackersCheck.Checked = ini.IniReadBoolValue("Settings", "CheckHosts");
				chkPingCheck.Checked = ini.IniReadBoolValue("Settings", "CheckPing");
				timeout = ini.IniReadIntValue("Settings", "Timeout", 500);
				chkStat.Checked = ini.IniReadBoolValue("Settings", "AddStat");
				chkAddRetrackerLocal.Checked = ini.IniReadBoolValue("Settings", "AddRetrackerLocal", true);
				string trackersFile = ini.IniReadValue("Settings", "TrackersFile");
				if (!System.IO.File.Exists(trackersFile))
				{
					trackersFile = GetSettingPath() + @"trackerssimple.ini";
					System.IO.File.WriteAllText(trackersFile, 
						"[Город]" + Environment.NewLine + 
						"Количество=1" + Environment.NewLine + 
						"1=Санкт-Петербург" + Environment.NewLine + 
						"[Провайдеры Санкт-Петербург]" + Environment.NewLine + 
						"Количество=1" + Environment.NewLine + 
						"1=Корбина" + Environment.NewLine + 
						"[Ретрекеры Санкт-Петербург Корбина]" + Environment.NewLine + 
						"Количество=7" + Environment.NewLine + 
						"1=http://10.121.10.1:2710/announce" + Environment.NewLine + 
						"2=http://netmaster.dyndns.ws:2710/announce" + Environment.NewLine + 
						"3=http://netmaster2.dyndns.ws:2710/announce" + Environment.NewLine + 
						"4=http://netmaster3.dyndns.ws:2710/announce" + Environment.NewLine + 
						"5=http://corbinaretracker.dyndns.org:80/announce.php" + Environment.NewLine + 
						"6=http://netmaster4.dyndns.ws:2710/announce" + Environment.NewLine + 
						"7=http://local-torrent-stats.no-ip.org:2710/announce", Encoding.Default);
					ini.IniWriteValue("Settings", "TrackersFile", trackersFile);
				}
				LoadRetrackersFile();
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

		private void LoadRetrackersFile()
		{
			try
			{
				string trackersFile = ini.IniReadValue("Settings", "TrackersFile");
				IniFile file = new IniFile(trackersFile);
				int cityCount = file.IniReadIntValue("Город", "Количество");
				cmbCity.Items.Clear();
				for (int i = 1; i <= cityCount; i++)
					cmbCity.Items.Add(file.IniReadValue("Город", i.ToString()));
				var array = ini.IniReadArray("Settings", "TrackerIniIndex");
				int cityIndex = array.Length >= 1 ? Convert.ToInt32(array[0]) : 0;
				if (cityIndex < 0 || cityIndex >= cmbCity.Items.Count)
					cityIndex = 0;
				if (cmbCity.Items.Count > 0)
					cmbCity.SelectedIndex = cityIndex;
				else
					cmbCity.Text = "";
				cmbCity.Refresh();
			}
			catch (System.Exception /*ex*/)
			{

			}
		}

		private void LoadTorrent(string TorrentPath, bool bNotLaunch = false)
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
			if (!bNotLaunch)
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
			if (((trvTorrent.SelectedNode.Index == 0) && MoveUp) || 
				((trvTorrent.SelectedNode.Index == (trvTorrent.SelectedNode.Parent.Nodes.Count - 1)) && !MoveUp))
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
					parent.Nodes[index].Text = Node.Index.ToString() + 
						parent.Nodes[index].Text.Remove(0, parent.Nodes[index].Text.IndexOf('('));
				}
				Node.Remove();
				parent.Nodes.Insert(index, Node);
				trvTorrent.SelectedNode = Node;
				CheckForMainInfo(dNode.NodePath(Node));
				trvTorrent.ResumeLayout();
				trvTorrent.Focus();
			}
		}

		private TreeNode FindNode(string NodePath)
		{
			TreeNode node;
			int ind = NodePath.IndexOf("/");
			if (ind <= 0)
				return trvTorrent.Nodes[NodePath];

			node = trvTorrent.Nodes[NodePath.Substring(0, ind)];
			NodePath = NodePath.Remove(0, ind + 1);
			foreach (string str in NodePath.Split(new char[] { '/' }))
			{
				if (dNode.NodeType(node) == DataType.List)
					node = node.Nodes[int.Parse(str)];
				else
					node = node.Nodes[str];
			}
			return node;
		}

		private TreeNode FindOrCreateNode(string NodePath, TVal val)
		{
			TreeNode node = FindNode(NodePath);
			if (node != null)
			{
				Debug.Assert(dNode.NodeType(node) == val.Type);
				return node;
			}

			string key = NodePath.Substring(NodePath.LastIndexOf('/') + 1);
			string nodePath = NodePath.Substring(0, NodePath.LastIndexOf('/'));
			node = AddNode(FindNode(nodePath), key, val);
			return node;
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
			foreach (string key in ValToAdd.Keys)
			{
				TVal val = ValToAdd[key];
				TreeNode node = AddNode(ParentTN, key, val);
				switch (val.Type)
				{
					case DataType.List:
						PopulateStructure(node, (List<TVal>)val);
						break;

					case DataType.Dictionary:
						PopulateStructure(node, (Dictionary<string, TVal>)val);
						break;
				}
			}
		}

		private void PopulateStructure(TreeNode ParentTN, List<TVal> ValToAdd)
		{
			int num = 0;
			foreach (TVal val in ValToAdd)
			{
				TreeNode node = AddNode(ParentTN, num.ToString(), val);
				switch (val.Type)
				{
					case DataType.List:
						PopulateStructure(node, (List<TVal>)val);
						break;

					case DataType.Dictionary:
						PopulateStructure(node, (Dictionary<string, TVal>)val);
						break;
				}
				SetNodeImage(node);
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
			ini.IniWriteValue("Settings", "AutoCheckUpdates", chkAutoCheckUpdates.Checked);
			ini.IniWriteValue("Settings", "UpdatePatcher", chkUpdatePatcher.Checked);
			ini.IniWriteValue("Settings", "UpdateTrackers", chkUpdateTrackers.Checked);
			ini.IniWriteValue("Settings", "CheckHosts", chkTrackersCheck.Checked);
			ini.IniWriteValue("Settings", "CheckPing", chkPingCheck.Checked);
			ini.IniWriteValue("Settings", "Timeout", timeout);
			ini.IniWriteValue("Settings", "AddStat", chkStat.Checked);
			ini.IniWriteValue("Settings", "AddRetrackerLocal", chkAddRetrackerLocal.Checked);
			ini.IniWriteValue("Settings", "LaunchPath", txtLaunchPath.Text);
			ini.IniWriteValue("Settings", "LaunchArguments", txtArguments.Text);
			ini.IniWriteValue("Settings", "SecureEdit", chkSecureEditing.Checked);
			ini.IniWriteValue("Settings", "AutoLaunch", chkAutoLaunchAllow.Checked);
			ini.IniWriteValue("Settings", "PatchAnnouncer", chkPatchAnnouncer.Checked);
			ini.IniWriteValue("Settings", "MagnetAnnounce", chkMagnet.Checked);
			ini.IniWriteValue("Settings", "DownloadURL", txtUpdateTrackers.Text);
			ini.IniWriteValue("Settings", "UpdateURL", txtUpdatePatcher.Text);
			ini.IniWriteValue("Settings", "TrackerIniIndex", cmbCity.SelectedIndex.ToString() + " " + 
				cmbISP.SelectedIndex.ToString());

			if (ini.IniReadBoolValue("Settings", "FirstRun") & !chkTrackersCheck.Checked)
			{
				tabControlMain.SelectedTab = tabSettings;
				tabControlSettings.SelectedTab = tabSettingsMain;
				Application.DoEvents();
				btnCheckTrackers_Click(null, null);
			}
			ini.IniWriteValue("Settings", "FirstRun", false);
			if (base.WindowState != FormWindowState.Normal)
			{
				ini.IniWriteValue("Settings", "FormPosX", base.RestoreBounds.Location.X);
				ini.IniWriteValue("Settings", "FormPosY", base.RestoreBounds.Location.Y);
				ini.IniWriteValue("Settings", "FormSizeHeight", base.RestoreBounds.Size.Height);
				ini.IniWriteValue("Settings", "FormSizeWidth", base.RestoreBounds.Size.Width);
			}
			else
			{
				ini.IniWriteValue("Settings", "FormPosX", base.Location.X);
				ini.IniWriteValue("Settings", "FormPosY", base.Location.Y);
				ini.IniWriteValue("Settings", "FormSizeHeight", base.Size.Height);
				ini.IniWriteValue("Settings", "FormSizeWidth", base.Size.Width);
			}
			ini.IniWriteValue("Settings", "FormState", base.WindowState.ToString());
		}

		private void SetNodeImage(TreeNode Node)
		{
			Debug.Assert(Node.Tag != null);
			TVal val = (TVal)Node.Tag;
			Node.ImageKey = val.GetTypeStr();
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
			trvTorrent.BeginUpdate();
			trvTorrent.Nodes.Clear();
			TVal val = new TVal(DataType.Dictionary, _torrent.Root);
			TreeNode node = AddNode(trvTorrent, "root", val);
			PopulateStructure(node, _torrent.Root);
			node.Expand();
			node.Nodes["info"].Expand();
			trvTorrent.EndUpdate();
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
				trvTorrent_AfterSelect(null, new TreeViewEventArgs(trvTorrent.SelectedNode, TreeViewAction.ByMouse));
				cmsStructure_Opened(null, null);
			}
		}

		private void trvTorrent_AfterSelect(object sender, TreeViewEventArgs e)
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
			ChangeString(FindNode("root/info/name"), "name", txtTorrentName.Text);
		}

		private void UpdateStructCallBack(string Path, string Name, TVal Value)
		{
			if (Path.EndsWith(Name))
			{
				TreeNode node = FindOrCreateNode(Path, Value);
				node.Text = Name + Value.ToString();
				node.Tag = Value;
				trvTorrent.SelectedNode = node;
			}
			else
			{
				TreeNode node = FindNode(Path);
				TreeNode parent = node.Parent;
				Name = CheckExist(parent, Name);
				node.Name = Name;
				node.Text = Name + Value.ToString();
				node.Tag = Value;
				trvTorrent.SelectedNode = node;
			}
			if (dNode.NodeType(trvTorrent.SelectedNode.Parent) == DataType.List)
			{
				ListRenamer(trvTorrent.SelectedNode.Parent);
			}
			SetNodeImage(trvTorrent.SelectedNode);
			if ((Value.Type == DataType.Int) || (Value.Type == DataType.String))
			{
				trvTorrent.SelectedNode.Nodes.Clear();
			}
			CheckForMainInfo(dNode.NodePath(trvTorrent.SelectedNode));
			trvTorrent.Focus();
		}

		private void UpdateTrackerStructure()
		{
			ChangeString(FindNode("root/announce"), "announce", lstTrackers.Items[0].Text);
			List<TreeNode> list = new List<TreeNode>();
			Regex regex = new Regex("(http|https|udp)://(.*)");
			for (int i = 0; i < lstTrackers.Items.Count; i++)
			{
				if (regex.IsMatch(lstTrackers.Items[i].Text))
				{
					string retracker = lstTrackers.Items[i].Text;
					TVal retrackerVal = new TVal(DataType.String, retracker);
					TreeNode retrackerNode = AddNode("0", retrackerVal);

					TVal retrackerNodeAsListVal = new TVal(DataType.List, null);
					int listIndex = list.Count;
					TreeNode retrackerNodeAsList = AddNode(listIndex.ToString(), retrackerNodeAsListVal);
					retrackerNodeAsList.Nodes.Add(retrackerNode);
					list.Add(retrackerNodeAsList);
				}
			}
			TVal announceListVal = new TVal(DataType.List, null);
			TreeNode announceList = FindOrCreateNode("root/announce-list", announceListVal);
			if (list.Count == 0)
			{
				announceList.Remove();
			}
			else
			{
				announceList.Nodes.Clear();
				announceList.Nodes.AddRange(list.ToArray());
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
				if (IsWin7OrAbove())
					Windows7Taskbar.SetProgressValue(Handle, (ulong)barCheck.Value, (ulong)waitHandles.Length);	
				WaitHandle.WaitAny(new WaitHandle[] { waitHandles[i] });
			}
			if (IsWin7OrAbove())
				Windows7Taskbar.SetProgressState(Handle, Windows7Taskbar.ThumbnailProgressState.NoProgress);
		}
	}
}

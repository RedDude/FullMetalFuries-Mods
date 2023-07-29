using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Xml;
using CDGEngine;
using Microsoft.Win32;
using SpriteSystem2;

namespace BrawlerEditor
{
	// Token: 0x0200004B RID: 75
	public partial class MainWindow : Window
	{
		// Token: 0x17000098 RID: 152
		// (get) Token: 0x060002CF RID: 719 RVA: 0x0001779C File Offset: 0x0001599C
		public CodeDialog CodeDialog
		{
			get
			{
				return this.m_codeDialog;
			}
		}

		// Token: 0x17000099 RID: 153
		// (get) Token: 0x060002D0 RID: 720 RVA: 0x000177B4 File Offset: 0x000159B4
		// (set) Token: 0x060002D1 RID: 721 RVA: 0x000177CC File Offset: 0x000159CC
		public bool ChangesMade
		{
			get
			{
				return this.m_changesMade;
			}
			set
			{
				this.m_changesMade = value;
				if (this.m_changesMade)
				{
					if (base.Title[base.Title.Length - 1] != '*')
					{
						base.Title += "*";
					}
				}
				else if (base.Title[base.Title.Length - 1] == '*')
				{
					base.Title = base.Title.Substring(0, base.Title.Length - 1);
				}
			}
		}

		// Token: 0x1700009A RID: 154
		// (get) Token: 0x060002D2 RID: 722 RVA: 0x00017870 File Offset: 0x00015A70
		public bool CtrlHeld
		{
			get
			{
				return Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
			}
		}

		// Token: 0x1700009B RID: 155
		// (get) Token: 0x060002D3 RID: 723 RVA: 0x00017898 File Offset: 0x00015A98
		public bool ShiftHeld
		{
			get
			{
				return Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);
			}
		}

		// Token: 0x1700009C RID: 156
		// (get) Token: 0x060002D4 RID: 724 RVA: 0x000178C0 File Offset: 0x00015AC0
		public bool ZHeld
		{
			get
			{
				return Keyboard.IsKeyDown(Key.Z);
			}
		}

		// Token: 0x060002D5 RID: 725 RVA: 0x000178DC File Offset: 0x00015ADC
		public MainWindow()
		{
			this.m_ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
			this.m_ci.NumberFormat.CurrencyDecimalSeparator = ".";
			this.InitializeComponent();
			base.WindowState = WindowState.Maximized;
			SpriteLibrary.Initialize(this.m_ci);
			ConfigFile.Instance.LoadFile(this);
			this.gameScreenControl.MainWindow = this;
			this.contentScreenControl.MainWindow = this;
			this.outputScrollViewer.MainWindow = this;
			this.propertiesStackPanel.MainWindow = this;
			this.layerTabControl.MainWindow = this;
			this.spriteListBox.MainWindow = this;
			this.layerPropertiesStackPanel.MainWindow = this;
			this.layerPropertiesPage2StackPanel.MainWindow = this;
			this.enemyScreenControl.MainWindow = this;
			base.MouseRightButtonUp += this.ResetMouseFocus;
			base.MouseLeftButtonUp += this.ResetMouseFocus;
			this.m_saveDialog = new Microsoft.Win32.SaveFileDialog();
			this.m_saveDialog.DefaultExt = ".xml";
			this.m_saveDialog.Filter = "XML file (.xml) |*.xml";
			this.m_saveDialog.RestoreDirectory = false;
			this.m_saveDialog.Title = "Save As...";
			this.m_openDialog = new Microsoft.Win32.OpenFileDialog();
			this.m_openDialog.FileName = "*.xml";
			this.m_openDialog.DefaultExt = ".xml";
			this.m_openDialog.Filter = "XML file (.xml) |*.xml";
			this.m_openDialog.RestoreDirectory = false;
			this.m_openDialog.Title = "Open File...";
			base.Closing += this.ApplicationClosing;
			base.Loaded += this.OnLoad;
		}

		// Token: 0x060002D6 RID: 726 RVA: 0x00017AAC File Offset: 0x00015CAC
		private void OnLoad(object sender, RoutedEventArgs e)
		{
			HwndSource hwndSource = HwndSource.FromHwnd(new WindowInteropHelper(this).Handle);
			hwndSource.AddHook(new HwndSourceHook(this.WndProc));
			this.DisplayCodeDialog();
		}

		// Token: 0x060002D7 RID: 727 RVA: 0x00017AE8 File Offset: 0x00015CE8
		private void ApplicationClosing(object sender, CancelEventArgs e)
		{
			if (this.ChangesMade)
			{
				string messageBoxText = "Unsaved changes detected.\nAre you sure you want to close the application?";
				MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(messageBoxText, "Close Brawler Editor", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
				if (messageBoxResult == MessageBoxResult.No)
				{
					e.Cancel = true;
				}
				else
				{
					this.CloseAllWindows();
				}
			}
			else
			{
				this.CloseAllWindows();
			}
		}

		// Token: 0x060002D8 RID: 728 RVA: 0x00017B44 File Offset: 0x00015D44
		private void CloseAllWindows()
		{
			foreach (object obj in System.Windows.Application.Current.Windows)
			{
				Window window = (Window)obj;
				if (window != this)
				{
					window.Close();
				}
			}
		}

		// Token: 0x060002D9 RID: 729 RVA: 0x00017BB4 File Offset: 0x00015DB4
		private void CloseWindow(Type type)
		{
			foreach (object obj in System.Windows.Application.Current.Windows)
			{
				Window window = (Window)obj;
				if (window.GetType() == type)
				{
					window.Close();
				}
			}
		}

		// Token: 0x060002DA RID: 730 RVA: 0x00017C34 File Offset: 0x00015E34
		private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			if (msg == 522)
			{
				this.gameScreenControl.ForceMouseWheelInput(wParam);
			}
			return IntPtr.Zero;
		}

		// Token: 0x060002DB RID: 731 RVA: 0x00017C6C File Offset: 0x00015E6C
		protected override void OnKeyDown(System.Windows.Input.KeyEventArgs e)
		{
			if (!e.IsRepeat)
			{
				if (!this.propertiesStackPanel.IsKeyboardFocusWithin && (this.CodeDialog == null || !this.CodeDialog.codeScrollViewer.IsKeyboardFocusWithin) && !this.gridSizeTextBox.IsFocused)
				{
					if (!this.CtrlHeld)
					{
						this.HotkeyPressed(e);
					}
				}
			}
			if (this.CtrlHeld && !e.IsRepeat)
			{
				this.CtrlHotKeyPressed(e);
			}
			this.gameScreenControl.ArrowKeysHandler(e);
			base.OnKeyDown(e);
		}

		// Token: 0x060002DC RID: 732 RVA: 0x00017D07 File Offset: 0x00015F07
		protected override void OnKeyUp(System.Windows.Input.KeyEventArgs e)
		{
			this.gameScreenControl.ArrowKeysHandler(e);
			base.OnKeyUp(e);
		}

		// Token: 0x060002DD RID: 733 RVA: 0x00017D20 File Offset: 0x00015F20
		private void CtrlHotKeyPressed(System.Windows.Input.KeyEventArgs e)
		{
			Key key = e.Key;
			if (key <= Key.F)
			{
				if (key != Key.C)
				{
					if (key == Key.F)
					{
						if (this.ShiftHeld)
						{
							this.gameScreenControl.FlipAllSelectedObjects();
						}
					}
				}
				else
				{
					this.CopyToClipboard();
				}
			}
			else
			{
				switch (key)
				{
				case Key.N:
					this.ClearProject();
					break;
				case Key.O:
					this.OpenButton_Clicked(null, null);
					break;
				case Key.P:
				case Key.R:
				case Key.T:
				case Key.U:
				case Key.W:
					break;
				case Key.Q:
					base.Close();
					break;
				case Key.S:
					if (this.ShiftHeld)
					{
						this.SaveAsButton_Clicked(null, null);
					}
					else
					{
						this.SaveButton_Clicked(null, null);
					}
					break;
				case Key.V:
					this.PasteFromClipboard();
					break;
				case Key.X:
					this.CutToClipboard();
					break;
				case Key.Y:
					this.gameScreenControl.UndoManager.RedoLastAction();
					break;
				case Key.Z:
					this.gameScreenControl.UndoManager.UndoLastAction();
					break;
				default:
					switch (key)
					{
					case Key.NumPad1:
					case Key.NumPad3:
						break;
					case Key.NumPad2:
						return;
					default:
						switch (key)
						{
						case Key.NumPad7:
						case Key.NumPad9:
							break;
						case Key.NumPad8:
							return;
						default:
							return;
						}
						break;
					}
					this.gameScreenControl.CreateCornerHull(e.Key);
					break;
				}
			}
		}

		// Token: 0x060002DE RID: 734 RVA: 0x00017E74 File Offset: 0x00016074
		private void HotkeyPressed(System.Windows.Input.KeyEventArgs e)
		{
			ToggleButton toggleButton = null;
			Key key = e.Key;
			if (key <= Key.G)
			{
				switch (key)
				{
				case Key.Prior:
					this.gameScreenControl.ShiftSelectedObjectsUp();
					break;
				case Key.Next:
					this.gameScreenControl.ShiftSelectedObjectsDown();
					break;
				default:
					switch (key)
					{
					case Key.A:
						toggleButton = this.rotationTool;
						break;
					case Key.D:
						toggleButton = this.playerSpawnTool;
						break;
					case Key.E:
						toggleButton = this.scaleTool;
						break;
					case Key.F:
						toggleButton = this.playerSpawnDebugTool;
						break;
					case Key.G:
						if (this.gameScreenControl.SnapToGrid)
						{
							this.snapToGridCheckBox.IsChecked = new bool?(false);
						}
						else
						{
							this.snapToGridCheckBox.IsChecked = new bool?(true);
						}
						this.gameScreenControl.SnapToGrid = this.snapToGridCheckBox.IsChecked.Value;
						ConfigFile.Instance.SaveFile();
						return;
					}
					break;
				}
			}
			else
			{
				switch (key)
				{
				case Key.Q:
					toggleButton = this.selectionTool;
					break;
				case Key.R:
					toggleButton = this.roomTool;
					break;
				case Key.S:
					toggleButton = this.markerTool;
					break;
				case Key.T:
				case Key.U:
					break;
				case Key.V:
					if (this.gameScreenControl.GridVisible)
					{
						this.gridVisibleCheckBox.IsChecked = new bool?(false);
					}
					else
					{
						this.gridVisibleCheckBox.IsChecked = new bool?(true);
					}
					this.gameScreenControl.GridVisible = this.gridVisibleCheckBox.IsChecked.Value;
					ConfigFile.Instance.SaveFile();
					return;
				case Key.W:
					toggleButton = this.collHullTool;
					break;
				default:
					switch (key)
					{
					case Key.F1:
					{
						bool flag = false;
						foreach (object obj in System.Windows.Application.Current.Windows)
						{
							Window window = (Window)obj;
							if (window is CommandsDialog)
							{
								flag = true;
							}
						}
						if (!flag)
						{
							CommandsDialog commandsDialog = new CommandsDialog();
							commandsDialog.Show();
						}
						else
						{
							this.CloseWindow(typeof(CommandsDialog));
						}
						break;
					}
					case Key.F2:
					{
						bool flag2 = false;
						foreach (object obj2 in System.Windows.Application.Current.Windows)
						{
							Window window = (Window)obj2;
							if (window is CodeDialog)
							{
								flag2 = true;
							}
						}
						if (!flag2)
						{
							this.DisplayCodeDialog();
						}
						else
						{
							this.CloseWindow(typeof(CodeDialog));
						}
						break;
					}
					case Key.F5:
						this.CompileProject(true);
						break;
					case Key.F6:
						this.CompileProject(false);
						break;
					case Key.F7:
						this.CompileAllFiles();
						break;
					}
					break;
				}
			}
			if (toggleButton != null && toggleButton.IsChecked == false)
			{
				toggleButton.IsChecked = new bool?(true);
				this.ToolBarButton_Clicked(toggleButton, null);
			}
		}

		// Token: 0x060002DF RID: 735 RVA: 0x00018220 File Offset: 0x00016420
		public void DisplayCodeDialog()
		{
			this.m_codeDialog = new CodeDialog(this);
			foreach (System.Windows.Forms.Screen screen in System.Windows.Forms.Screen.AllScreens)
			{
				if (!screen.Primary)
				{
					this.m_codeDialog.Top = (double)((float)screen.WorkingArea.Height / 2f) - this.m_codeDialog.Height / 2.0;
					this.m_codeDialog.Left = (double)((float)screen.WorkingArea.Left + (float)screen.WorkingArea.Width / 2f) - this.m_codeDialog.Width / 2.0;
					break;
				}
			}
			RoomObj roomObj = null;
			foreach (GameObj gameObj in this.gameScreenControl.SelectedObjects)
			{
				RoomObj roomObj2 = gameObj as RoomObj;
				if (roomObj2 != null)
				{
					roomObj = roomObj2;
					break;
				}
			}
			if (roomObj != null)
			{
				this.CodeDialog.codeScrollViewer.ShowCode(roomObj);
			}
			else if (this.gameScreenControl.SelectedObjects.Count == 1)
			{
				this.CodeDialog.codeScrollViewer.ShowCode(this.gameScreenControl.SelectedObjects[0]);
			}
			this.m_codeDialog.Show();
		}

		// Token: 0x060002E0 RID: 736 RVA: 0x000183D4 File Offset: 0x000165D4
		private void ResetMouseFocus(object sender, MouseButtonEventArgs e)
		{
			this.gameScreenControl.ForceMouseFocusRemoval();
		}

		// Token: 0x060002E1 RID: 737 RVA: 0x000183E4 File Offset: 0x000165E4
		public void SnapToGridButtonClicked(object sender, RoutedEventArgs e)
		{
			if ((sender as System.Windows.Controls.CheckBox).IsChecked == true)
			{
				this.gameScreenControl.SnapToGrid = false;
			}
			else
			{
				this.gameScreenControl.SnapToGrid = true;
			}
			ConfigFile.Instance.SaveFile();
		}

		// Token: 0x060002E2 RID: 738 RVA: 0x00018444 File Offset: 0x00016644
		public void GridVisibleButtonClicked(object sender, RoutedEventArgs e)
		{
			if ((sender as System.Windows.Controls.CheckBox).IsChecked == true)
			{
				this.gameScreenControl.GridVisible = false;
			}
			else
			{
				this.gameScreenControl.GridVisible = true;
			}
			ConfigFile.Instance.SaveFile();
		}

		// Token: 0x060002E3 RID: 739 RVA: 0x000184A4 File Offset: 0x000166A4
		private void UpdateGridSize(object sender, KeyboardFocusChangedEventArgs e)
		{
			string text = (sender as System.Windows.Controls.TextBox).Text;
			int num = 0;
			if (int.TryParse(text, out num))
			{
				if (num < 5)
				{
					num = 5;
				}
				else if (num > 500)
				{
					num = 500;
				}
				this.gameScreenControl.GridUnitSize = num;
				(sender as System.Windows.Controls.TextBox).Text = num.ToString();
				ConfigFile.Instance.SaveFile();
			}
		}

		// Token: 0x060002E4 RID: 740 RVA: 0x00018520 File Offset: 0x00016720
		private void ResetZoomButton_Clicked(object sender, RoutedEventArgs e)
		{
			this.gameScreenControl.ResetZoom();
			this.zoomTextBlock.Text = "Zoom: 1.00";
		}

		// Token: 0x060002E5 RID: 741 RVA: 0x00018540 File Offset: 0x00016740
		private void ResetPositionButton_Clicked(object sender, RoutedEventArgs e)
		{
			this.gameScreenControl.ResetCameraPosition();
			this.coordinatesTextBlock.Text = "(0, 0)";
		}

		// Token: 0x060002E6 RID: 742 RVA: 0x00018560 File Offset: 0x00016760
		private void ToolBarButton_Clicked(object sender, RoutedEventArgs e)
		{
			ToggleButton toggleButton = sender as ToggleButton;
			if (toggleButton.IsChecked == true)
			{
				foreach (object obj in this.toolBarStackPanel.Children)
				{
					ToggleButton toggleButton2 = (ToggleButton)obj;
					toggleButton2.IsChecked = new bool?(false);
				}
				toggleButton.IsChecked = new bool?(true);
				this.gameScreenControl.SetTool(byte.Parse((string)toggleButton.Tag));
			}
			else
			{
				this.gameScreenControl.SetTool(0);
			}
		}

		// Token: 0x060002E7 RID: 743 RVA: 0x0001863C File Offset: 0x0001683C
		private void FileMenuItem_Click(object sender, RoutedEventArgs e)
		{
			System.Windows.Controls.MenuItem menuItem = sender as System.Windows.Controls.MenuItem;
			string name = menuItem.Name;
			switch (name)
			{
			case "Exit":
				base.Close();
				break;
			case "Save":
				this.SaveButton_Clicked(sender, e);
				break;
			case "SaveAs":
				this.SaveAsButton_Clicked(sender, e);
				break;
			case "Open":
				this.OpenButton_Clicked(sender, e);
				break;
			case "New":
				this.ClearProject();
				break;
			case "Compile":
				this.CompileProject(false);
				break;
			case "CompileAll":
				this.CompileAllFiles();
				break;
			case "CompileAndRun":
				this.CompileProject(true);
				break;
			case "SetCompileDirectory":
				this.SetCompileToDirectory();
				break;
			case "SetGameEXEDirectory":
				this.SetGameExecutableDirectory();
				break;
			case "SelectSprites":
				ConfigFile.Instance.selectSprites = this.SelectSprites.IsChecked;
				ConfigFile.Instance.SaveFile();
				break;
			case "SelectCollHulls":
				ConfigFile.Instance.selectCollHulls = this.SelectCollHulls.IsChecked;
				ConfigFile.Instance.SaveFile();
				break;
			case "SelectRooms":
				ConfigFile.Instance.selectRooms = this.SelectRooms.IsChecked;
				ConfigFile.Instance.SaveFile();
				break;
			case "ShowSpriteCollHulls":
				ConfigFile.Instance.showSpriteCollHulls = this.ShowSpriteCollHulls.IsChecked;
				ConfigFile.Instance.SaveFile();
				break;
			}
		}

		// Token: 0x060002E8 RID: 744 RVA: 0x00018888 File Offset: 0x00016A88
		private void LayerMenuItem_Click(object sender, RoutedEventArgs e)
		{
			System.Windows.Controls.MenuItem menuItem = sender as System.Windows.Controls.MenuItem;
			string name = menuItem.Name;
			if (name != null)
			{
				if (!(name == "AddLayer"))
				{
					if (!(name == "RemoveLayer"))
					{
						if (name == "ChangeLayerName")
						{
							if (this.layerTabControl.SelectedItem == this.layerTabControl.GameLayer)
							{
								System.Windows.MessageBox.Show("Cannot rename Game Layer", "Change Layer Name", MessageBoxButton.OK, MessageBoxImage.Hand);
							}
							else
							{
								ChangeLayerNameDialog changeLayerNameDialog = new ChangeLayerNameDialog(this, false);
								changeLayerNameDialog.ShowDialog();
							}
						}
					}
					else if (this.layerTabControl.SelectedItem == this.layerTabControl.GameLayer)
					{
						System.Windows.MessageBox.Show("Cannot remove Game Layer", "Remove Layer", MessageBoxButton.OK, MessageBoxImage.Hand);
					}
					else if (System.Windows.MessageBox.Show("Are you sure you want to delete layer '" + ((this.layerTabControl.SelectedItem as TabItem).Header as TextBlock).Text + "'?\nAll layer info will be lost.", "Confirm Layer Delete", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
					{
						this.layerTabControl.RemoveLayerAtIndex(this.layerTabControl.SelectedIndex, true);
					}
				}
				else
				{
					ChangeLayerNameDialog changeLayerNameDialog2 = new ChangeLayerNameDialog(this, true);
					changeLayerNameDialog2.ShowDialog();
				}
			}
		}

		// Token: 0x060002E9 RID: 745 RVA: 0x000189D4 File Offset: 0x00016BD4
		private void LoadDirectoryButton_Clicked(object sender, RoutedEventArgs e)
		{
			FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
			folderBrowserDialog.Description = "Select a directory for your spritesheets";
			folderBrowserDialog.SelectedPath = ConfigFile.Instance.SpritesheetDirectory;
			folderBrowserDialog.ShowNewFolderButton = false;
			if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				if (ConfigFile.Instance.SpritesheetDirectory != folderBrowserDialog.SelectedPath)
				{
					ConfigFile.Instance.SpritesheetDirectory = folderBrowserDialog.SelectedPath;
					ConfigFile.Instance.SaveFile();
					this.spriteListBox.LoadInitialDirectories();
				}
			}
		}

		// Token: 0x060002EA RID: 746 RVA: 0x00018A65 File Offset: 0x00016C65
		public void ChangeTitle(string newTitle)
		{
			base.Title = newTitle;
		}

		// Token: 0x060002EB RID: 747 RVA: 0x00018A70 File Offset: 0x00016C70
		public void SaveButton_Clicked(object sender, RoutedEventArgs e)
		{
			if (SaveHelper.FilePath == "")
			{
				this.SaveAsButton_Clicked(sender, e);
			}
			else
			{
				SaveHelper.SaveXMLFile(this, false);
				this.ChangesMade = false;
			}
		}

		// Token: 0x060002EC RID: 748 RVA: 0x00018AB4 File Offset: 0x00016CB4
		public void SaveAsButton_Clicked(object sender, RoutedEventArgs e)
		{
			if (this.m_saveDialog.ShowDialog() == true)
			{
				SaveHelper.FilePath = this.m_saveDialog.FileName;
				SaveHelper.SaveXMLFile(this, false);
				this.ChangeTitle("Brawler Editor - " + SaveHelper.FilePath);
				this.ChangesMade = false;
			}
		}

		// Token: 0x060002ED RID: 749 RVA: 0x00018B24 File Offset: 0x00016D24
		private void OpenButton_Clicked(object sender, RoutedEventArgs e)
		{
			if (this.ClearProject())
			{
				if (this.m_openDialog.ShowDialog() == true)
				{
					SaveHelper.FilePath = this.m_openDialog.FileName;
					SaveHelper.LoadXMLFile(this, false);
					this.layerTabControl.RefreshLayerTabs();
					this.ChangeTitle("Brawler Editor - " + SaveHelper.FilePath);
					this.ChangesMade = false;
					this.outputScrollViewer.ClearText();
					this.gameScreenControl.FocusCameraOnPlayerStart();
				}
			}
		}

		// Token: 0x060002EE RID: 750 RVA: 0x00018BC7 File Offset: 0x00016DC7
		public void CopyToClipboard()
		{
			this.SaveClipboard();
		}

		// Token: 0x060002EF RID: 751 RVA: 0x00018BD4 File Offset: 0x00016DD4
		public void CutToClipboard()
		{
			this.SaveClipboard();
			List<GameObj> list = new List<GameObj>();
			foreach (GameObj gameObj in this.gameScreenControl.SelectedObjects)
			{
				if (!(gameObj is PlayerStartObj))
				{
					list.Add(gameObj);
				}
			}
			this.gameScreenControl.RemoveGameObjs(list, true);
			this.gameScreenControl.DeselectAllGameObjs();
		}

		// Token: 0x060002F0 RID: 752 RVA: 0x00018C68 File Offset: 0x00016E68
		public void PasteFromClipboard()
		{
			this.gameScreenControl.DeselectAllGameObjs();
			SaveHelper.LoadXMLFile(this, true);
		}

		// Token: 0x060002F1 RID: 753 RVA: 0x00018C7F File Offset: 0x00016E7F
		public void SaveClipboard()
		{
			SaveHelper.SaveXMLFile(this, true);
		}

		// Token: 0x060002F2 RID: 754 RVA: 0x00018C8C File Offset: 0x00016E8C
		private void WriteSpecialObjAttributes(XmlWriter writer, GameObj obj)
		{
			EditorCollHullObj editorCollHullObj = obj as EditorCollHullObj;
			if (editorCollHullObj != null)
			{
				writer.WriteAttributeString("Width", editorCollHullObj.Width.ToString());
				writer.WriteAttributeString("Height", editorCollHullObj.Height.ToString());
			}
		}

		// Token: 0x060002F3 RID: 755 RVA: 0x00018CE0 File Offset: 0x00016EE0
		private void ReadSpecialObjAttributes(XmlReader reader, GameObj obj)
		{
			EditorCollHullObj editorCollHullObj = obj as EditorCollHullObj;
			if (editorCollHullObj != null)
			{
				reader.MoveToAttribute("Width");
				float width = float.Parse(reader.Value, NumberStyles.Any, this.m_ci);
				editorCollHullObj.Width = width;
				reader.MoveToAttribute("Height");
				float height = float.Parse(reader.Value, NumberStyles.Any, this.m_ci);
				editorCollHullObj.Height = height;
			}
		}

		// Token: 0x060002F4 RID: 756 RVA: 0x00018D58 File Offset: 0x00016F58
		private void gridSizeTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		{
			if (!e.IsRepeat && e.Key == Key.Return)
			{
				this.gameScreenControl.Focus();
			}
		}

		// Token: 0x060002F5 RID: 757 RVA: 0x00018D90 File Offset: 0x00016F90
		public void SetCompileToDirectory()
		{
			FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
			folderBrowserDialog.Description = "Select a directory that contains all files you want to compile";
			folderBrowserDialog.SelectedPath = ConfigFile.Instance.compileDirectory;
			folderBrowserDialog.ShowNewFolderButton = false;
			if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				if (ConfigFile.Instance.compileDirectory != folderBrowserDialog.SelectedPath)
				{
					ConfigFile.Instance.compileDirectory = folderBrowserDialog.SelectedPath;
					ConfigFile.Instance.SaveFile();
				}
			}
		}

		// Token: 0x060002F6 RID: 758 RVA: 0x00018E18 File Offset: 0x00017018
		public void SetGameExecutableDirectory()
		{
			Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
			openFileDialog.FileName = "*.exe";
			openFileDialog.DefaultExt = ".exe";
			openFileDialog.Filter = "EXE file (.exe) |*.exe";
			openFileDialog.RestoreDirectory = false;
			openFileDialog.Title = "Select Game Executable...";
			if (openFileDialog.ShowDialog() == true)
			{
				ConfigFile.Instance.executableDirectory = openFileDialog.FileName;
				ConfigFile.Instance.SaveFile();
			}
		}

		// Token: 0x060002F7 RID: 759 RVA: 0x00018EA4 File Offset: 0x000170A4
		public void CompileProject(bool runGame)
		{
			this.outputScrollViewer.ClearText();
			string arenaFilePath = SaveHelper.SaveBEFFile(this);
			if (runGame)
			{
				try
				{
					this.outputScrollViewer.AddOutputText("Loading Game EXE");
					this.RunGameExecutable(arenaFilePath);
				}
				catch (Exception ex)
				{
					this.outputScrollViewer.AddOutputText("ERROR: " + ex.Message);
					this.outputScrollViewer.AddOutputText("ERROR: Compile failed");
				}
			}
		}

		// Token: 0x060002F8 RID: 760 RVA: 0x00018F30 File Offset: 0x00017130
		public void CompileAllFiles()
		{
			this.outputScrollViewer.ClearText();
			DirectoryInfo directoryInfo = new DirectoryInfo(ConfigFile.Instance.compileDirectory);
			if (!directoryInfo.Exists)
			{
				throw new DirectoryNotFoundException();
			}
			List<string> list = new List<string>();
			FileInfo[] files = directoryInfo.GetFiles("*.*");
			foreach (FileInfo fileInfo in files)
			{
				list.Add(fileInfo.FullName);
			}
			foreach (string text in list)
			{
				this.ClearProject();
				SaveHelper.FilePath = text;
				try
				{
					SaveHelper.LoadXMLFile(this, false);
					SaveHelper.SaveBEFFile(this);
					this.outputScrollViewer.AddOutputText("Compiling Level - " + text);
				}
				catch (Exception ex)
				{
					this.outputScrollViewer.AddOutputText("ERROR: " + ex.Message);
					this.outputScrollViewer.AddOutputText("ERROR: Failed to compile level - " + text);
				}
			}
		}

		// Token: 0x060002F9 RID: 761 RVA: 0x00019078 File Offset: 0x00017278
		public void RunGameExecutable(string arenaFilePath)
		{
			string str = "<UNDEFINED>";
			try
			{
				if (ConfigFile.Instance.executableDirectory == "")
				{
					throw new Exception("Executable file not set.");
				}
				int num = ConfigFile.Instance.executableDirectory.LastIndexOf("\\") + 1;
				str = ConfigFile.Instance.executableDirectory.Substring(num, ConfigFile.Instance.executableDirectory.Length - num);
				using (Process.Start(new ProcessStartInfo
				{
					FileName = ConfigFile.Instance.executableDirectory,
					WorkingDirectory = ConfigFile.Instance.executableDirectory.Substring(0, num),
					Arguments = arenaFilePath
				}))
				{
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Could not load " + str + ".  Original Error: " + ex.Message);
			}
		}

		// Token: 0x060002FA RID: 762 RVA: 0x00019184 File Offset: 0x00017384
		public void SetSpriteNameBoxText(string text)
		{
			this.spriteNameBox.Text = text;
		}

		// Token: 0x060002FB RID: 763 RVA: 0x00019194 File Offset: 0x00017394
		public void ClearSpriteNameBox()
		{
			this.spriteNameBox.Text = "";
		}

		// Token: 0x060002FC RID: 764 RVA: 0x000191A8 File Offset: 0x000173A8
		public bool ClearProject()
		{
			bool result;
			if (this.ChangesMade)
			{
				string messageBoxText = "Unsaved changes detected.\nAre you sure you want to clear this project?";
				MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(messageBoxText, "Clear project", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
				if (messageBoxResult == MessageBoxResult.Yes)
				{
					this.ChangeTitle("Brawler Editor - New Arena");
					this.ChangesMade = false;
					SaveHelper.FilePath = "";
					this.layerTabControl.ResetControl();
					this.gameScreenControl.ResetControl();
					result = true;
				}
				else
				{
					result = false;
				}
			}
			else
			{
				this.ChangeTitle("Brawler Editor - New Arena");
				this.ChangesMade = false;
				SaveHelper.FilePath = "";
				this.layerTabControl.ResetControl();
				this.gameScreenControl.ResetControl();
				result = true;
			}
			return result;
		}

		// Token: 0x04000166 RID: 358
		private CultureInfo m_ci;

		// Token: 0x04000167 RID: 359
		private Microsoft.Win32.SaveFileDialog m_saveDialog;

		// Token: 0x04000168 RID: 360
		private Microsoft.Win32.OpenFileDialog m_openDialog;

		// Token: 0x04000169 RID: 361
		private bool m_changesMade;

		// Token: 0x0400016A RID: 362
		private CodeDialog m_codeDialog;
	}
}

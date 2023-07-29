using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace BrawlerEditor
{
	// Token: 0x02000030 RID: 48
	public sealed class ConfigFile
	{
		// Token: 0x17000057 RID: 87
		// (get) Token: 0x0600019E RID: 414 RVA: 0x0000CCF4 File Offset: 0x0000AEF4
		// (set) Token: 0x0600019F RID: 415 RVA: 0x0000CD0C File Offset: 0x0000AF0C
		public bool showSpriteCollHulls
		{
			get
			{
				return this.m_showSpriteCollHulls;
			}
			set
			{
				this.m_showSpriteCollHulls = value;
			}
		}

		// Token: 0x17000058 RID: 88
		// (get) Token: 0x060001A0 RID: 416 RVA: 0x0000CD18 File Offset: 0x0000AF18
		// (set) Token: 0x060001A1 RID: 417 RVA: 0x0000CD30 File Offset: 0x0000AF30
		public bool selectSprites
		{
			get
			{
				return this.m_selectSprites;
			}
			set
			{
				this.m_selectSprites = value;
			}
		}

		// Token: 0x17000059 RID: 89
		// (get) Token: 0x060001A2 RID: 418 RVA: 0x0000CD3C File Offset: 0x0000AF3C
		// (set) Token: 0x060001A3 RID: 419 RVA: 0x0000CD54 File Offset: 0x0000AF54
		public bool selectCollHulls
		{
			get
			{
				return this.m_selectCollHulls;
			}
			set
			{
				this.m_selectCollHulls = value;
			}
		}

		// Token: 0x1700005A RID: 90
		// (get) Token: 0x060001A4 RID: 420 RVA: 0x0000CD60 File Offset: 0x0000AF60
		// (set) Token: 0x060001A5 RID: 421 RVA: 0x0000CD78 File Offset: 0x0000AF78
		public bool selectRooms
		{
			get
			{
				return this.m_selectRooms;
			}
			set
			{
				this.m_selectRooms = value;
			}
		}

		// Token: 0x1700005B RID: 91
		// (get) Token: 0x060001A6 RID: 422 RVA: 0x0000CD84 File Offset: 0x0000AF84
		public static ConfigFile Instance
		{
			get
			{
				if (ConfigFile.m_instance == null)
				{
					ConfigFile.m_instance = new ConfigFile();
				}
				return ConfigFile.m_instance;
			}
		}

		// Token: 0x1700005C RID: 92
		// (get) Token: 0x060001A7 RID: 423 RVA: 0x0000CDB4 File Offset: 0x0000AFB4
		// (set) Token: 0x060001A8 RID: 424 RVA: 0x0000CDCC File Offset: 0x0000AFCC
		public bool SnapToGrid
		{
			get
			{
				return this.m_snapToGrid;
			}
			set
			{
				this.m_snapToGrid = value;
			}
		}

		// Token: 0x1700005D RID: 93
		// (get) Token: 0x060001A9 RID: 425 RVA: 0x0000CDD8 File Offset: 0x0000AFD8
		// (set) Token: 0x060001AA RID: 426 RVA: 0x0000CDF0 File Offset: 0x0000AFF0
		public bool GridVisible
		{
			get
			{
				return this.m_gridVisible;
			}
			set
			{
				this.m_gridVisible = value;
			}
		}

		// Token: 0x1700005E RID: 94
		// (get) Token: 0x060001AB RID: 427 RVA: 0x0000CDFC File Offset: 0x0000AFFC
		// (set) Token: 0x060001AC RID: 428 RVA: 0x0000CE14 File Offset: 0x0000B014
		public int GridUnitSize
		{
			get
			{
				return this.m_gridUnitSize;
			}
			set
			{
				this.m_gridUnitSize = value;
			}
		}

		// Token: 0x1700005F RID: 95
		// (get) Token: 0x060001AD RID: 429 RVA: 0x0000CE20 File Offset: 0x0000B020
		// (set) Token: 0x060001AE RID: 430 RVA: 0x0000CE38 File Offset: 0x0000B038
		public string SpritesheetDirectory
		{
			get
			{
				return this.m_spritesheetDirectory;
			}
			set
			{
				this.m_spritesheetDirectory = value;
			}
		}

		// Token: 0x17000060 RID: 96
		// (get) Token: 0x060001AF RID: 431 RVA: 0x0000CE44 File Offset: 0x0000B044
		// (set) Token: 0x060001B0 RID: 432 RVA: 0x0000CE5C File Offset: 0x0000B05C
		public string compileDirectory
		{
			get
			{
				return this.m_compileDirectory;
			}
			set
			{
				this.m_compileDirectory = value;
			}
		}

		// Token: 0x17000061 RID: 97
		// (get) Token: 0x060001B1 RID: 433 RVA: 0x0000CE68 File Offset: 0x0000B068
		// (set) Token: 0x060001B2 RID: 434 RVA: 0x0000CE80 File Offset: 0x0000B080
		public string executableDirectory
		{
			get
			{
				return this.m_executableDirectory;
			}
			set
			{
				this.m_executableDirectory = value;
			}
		}

		// Token: 0x060001B3 RID: 435 RVA: 0x0000CE8A File Offset: 0x0000B08A
		private ConfigFile()
		{
		}

		// Token: 0x060001B4 RID: 436 RVA: 0x0000CE95 File Offset: 0x0000B095
		public void SetDefaultValues()
		{
			this.m_snapToGrid = true;
			this.m_gridUnitSize = 50;
			this.m_gridVisible = true;
			this.m_spritesheetDirectory = "";
			this.m_compileDirectory = "";
			this.m_executableDirectory = "";
		}

		// Token: 0x060001B5 RID: 437 RVA: 0x0000CED0 File Offset: 0x0000B0D0
		public void SaveFile()
		{
			Console.WriteLine("Saving confile file");
			using (StreamWriter streamWriter = new StreamWriter("Config.ini", false))
			{
				try
				{
					streamWriter.WriteLine("SnapToGrid=" + this.SnapToGrid);
					streamWriter.WriteLine("GridVisible=" + this.GridVisible);
					streamWriter.WriteLine("GridUnitSize=" + this.GridUnitSize);
					streamWriter.WriteLine("SpritesheetDirectory=" + this.SpritesheetDirectory);
					streamWriter.WriteLine("CompileDirectory=" + this.compileDirectory);
					streamWriter.WriteLine("ExecutableDirectory=" + this.executableDirectory);
					streamWriter.WriteLine("SelectSprites=" + this.selectSprites);
					streamWriter.WriteLine("SelectCollHulls=" + this.selectCollHulls);
					streamWriter.WriteLine("SelectRooms=" + this.selectRooms);
					streamWriter.WriteLine("ShowSpriteCollHulls=" + this.showSpriteCollHulls);
					Console.WriteLine("Config file saved");
				}
				catch (Exception ex)
				{
					Console.WriteLine("Config file save FAILED");
					MessageBox.Show("ERROR: Could not write to config file to disk. Original error: " + ex.Message);
				}
				finally
				{
					if (streamWriter != null)
					{
						streamWriter.Close();
					}
				}
			}
		}

		// Token: 0x060001B6 RID: 438 RVA: 0x0000D0AC File Offset: 0x0000B2AC
		public void LoadFile(MainWindow mainWindow)
		{
			CultureInfo cultureInfo = (CultureInfo)CultureInfo.CurrentCulture.Clone();
			cultureInfo.NumberFormat.CurrencyDecimalSeparator = ".";
			Console.WriteLine("Loading config file");
			bool flag = false;
			using (StreamReader streamReader = new StreamReader("Config.ini"))
			{
				try
				{
					string text;
					while ((text = streamReader.ReadLine()) != null)
					{
						int num = text.IndexOf("=");
						if (num != -1)
						{
							string text2 = text.Substring(0, num);
							string text3 = text.Substring(num + 1);
							string text4 = text2;
							switch (text4)
							{
							case "SnapToGrid":
								this.SnapToGrid = bool.Parse(text3);
								mainWindow.snapToGridCheckBox.IsChecked = new bool?(this.SnapToGrid);
								break;
							case "GridVisible":
								this.GridVisible = bool.Parse(text3);
								mainWindow.gridVisibleCheckBox.IsChecked = new bool?(this.GridVisible);
								break;
							case "GridUnitSize":
							{
								this.GridUnitSize = int.Parse(text3, NumberStyles.Any, cultureInfo);
								TextBox gridSizeTextBox = mainWindow.gridSizeTextBox;
								int gridUnitSize = this.GridUnitSize;
								gridSizeTextBox.Text = gridUnitSize.ToString();
								break;
							}
							case "SpritesheetDirectory":
								this.SpritesheetDirectory = text3;
								mainWindow.spriteListBox.LoadInitialDirectories();
								break;
							case "CompileDirectory":
								this.compileDirectory = text3;
								break;
							case "ExecutableDirectory":
								this.executableDirectory = text3;
								break;
							case "SelectSprites":
								this.selectSprites = bool.Parse(text3);
								mainWindow.SelectSprites.IsChecked = this.selectSprites;
								break;
							case "SelectCollHulls":
								this.selectCollHulls = bool.Parse(text3);
								mainWindow.SelectCollHulls.IsChecked = this.selectCollHulls;
								break;
							case "SelectRooms":
								this.selectRooms = bool.Parse(text3);
								mainWindow.SelectRooms.IsChecked = this.selectRooms;
								break;
							case "ShowSpriteCollHulls":
								this.showSpriteCollHulls = bool.Parse(text3);
								mainWindow.ShowSpriteCollHulls.IsChecked = this.showSpriteCollHulls;
								break;
							}
						}
					}
					Console.WriteLine("Config file loaded");
				}
				catch
				{
					flag = true;
				}
				finally
				{
					if (streamReader != null)
					{
						streamReader.Close();
					}
				}
			}
			if (flag)
			{
				Console.WriteLine("Config file not found. Creating default config file.");
				this.SetDefaultValues();
				this.SaveFile();
				this.LoadFile(mainWindow);
			}
		}

		// Token: 0x040000B9 RID: 185
		private bool m_snapToGrid;

		// Token: 0x040000BA RID: 186
		private bool m_gridVisible;

		// Token: 0x040000BB RID: 187
		private int m_gridUnitSize;

		// Token: 0x040000BC RID: 188
		private string m_spritesheetDirectory;

		// Token: 0x040000BD RID: 189
		private string m_compileDirectory;

		// Token: 0x040000BE RID: 190
		private string m_executableDirectory;

		// Token: 0x040000BF RID: 191
		private bool m_selectSprites;

		// Token: 0x040000C0 RID: 192
		private bool m_selectCollHulls;

		// Token: 0x040000C1 RID: 193
		private bool m_selectRooms;

		// Token: 0x040000C2 RID: 194
		private bool m_showSpriteCollHulls;

		// Token: 0x040000C3 RID: 195
		private static ConfigFile m_instance;
	}
}

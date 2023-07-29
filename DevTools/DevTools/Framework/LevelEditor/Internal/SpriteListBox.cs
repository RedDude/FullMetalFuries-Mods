using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Controls;
using System.Windows.Input;

namespace BrawlerEditor
{
	// Token: 0x0200002E RID: 46
	public class SpriteListBox : ListBox, IControl
	{
		// Token: 0x17000056 RID: 86
		// (get) Token: 0x06000192 RID: 402 RVA: 0x0000C82C File Offset: 0x0000AA2C
		// (set) Token: 0x06000193 RID: 403 RVA: 0x0000C843 File Offset: 0x0000AA43
		public MainWindow MainWindow { get; set; }

		// Token: 0x06000194 RID: 404 RVA: 0x0000C84C File Offset: 0x0000AA4C
		public SpriteListBox()
		{
			this.m_spritesheetNames = new List<string>();
			base.PreviewMouseLeftButtonUp += this.LeftMouseButton_Up;
		}

		// Token: 0x06000195 RID: 405 RVA: 0x0000C880 File Offset: 0x0000AA80
		private void LeftMouseButton_Up(object sender, MouseButtonEventArgs e)
		{
			if (base.SelectedItem != null)
			{
				SpriteListBoxItem spriteListBoxItem = base.SelectedItem as SpriteListBoxItem;
				if (spriteListBoxItem.isDirectory)
				{
					this.SelectEntry(spriteListBoxItem.Content as string, spriteListBoxItem.isDirectory);
				}
				else
				{
					this.SelectEntry(spriteListBoxItem.semiDirectoryPath + "\\" + spriteListBoxItem.Content, spriteListBoxItem.isDirectory);
				}
			}
		}

		// Token: 0x06000196 RID: 406 RVA: 0x0000C8F3 File Offset: 0x0000AAF3
		public void LoadInitialDirectories()
		{
			this.m_spritesheetNames.Clear();
			base.Items.Clear();
			this.LoadDirectory("");
		}

		// Token: 0x06000197 RID: 407 RVA: 0x0000C91C File Offset: 0x0000AB1C
		public void LoadDirectory(string filePath)
		{
			this.m_spritesheetNames.Clear();
			base.Items.Clear();
			if (filePath == "...")
			{
				filePath = this.m_currentDirectory.Substring(0, this.m_currentDirectory.LastIndexOf("\\"));
				this.m_currentDirectory = filePath;
			}
			else
			{
				this.m_currentDirectory = filePath;
			}
			string text = ConfigFile.Instance.SpritesheetDirectory + filePath;
			if (text != "")
			{
				bool flag = true;
				string[] array = Directory.GetDirectories(text);
				if (array.Length == 0)
				{
					flag = false;
					array = Directory.GetFiles(text, "*.png");
				}
				if (filePath != "")
				{
					base.Items.Add(new SpriteListBoxItem
					{
						Content = "...",
						isDirectory = true
					});
				}
				string[] array2 = array;
				int i = 0;
				while (i < array2.Length)
				{
					string text2 = array2[i];
					string text3 = text2.Substring(text2.LastIndexOf("\\") + 1);
					this.m_spritesheetNames.Add(text3);
					if (flag)
					{
						if (filePath != "...")
						{
							base.Items.Add(new SpriteListBoxItem
							{
								Content = filePath + "\\" + text3,
								isDirectory = true
							});
						}
						else
						{
							base.Items.Add(new SpriteListBoxItem
							{
								Content = "\\" + text3,
								isDirectory = true
							});
						}
					}
					else
					{
						string path = text2.Substring(0, text2.Length - 4) + ".xml";
						if (!File.Exists(path))
						{
							Console.WriteLine("Cannot load " + text2 + " into spritesheet as XML file does not exist");
						}
						else
						{
							base.Items.Add(new SpriteListBoxItem
							{
								Content = text3,
								isDirectory = false,
								semiDirectoryPath = filePath
							});
						}
					}
					IL_221:
					i++;
					continue;
					goto IL_221;
				}
			}
		}

		// Token: 0x06000198 RID: 408 RVA: 0x0000CB64 File Offset: 0x0000AD64
		public void SelectEntry(string fileName, bool isDirectory)
		{
			if (!isDirectory)
			{
				this.MainWindow.contentScreenControl.LoadSpritesheet(fileName, false);
				this.MainWindow.contentScreenControl.LoadSprites(fileName, false);
			}
			else
			{
				this.LoadDirectory(fileName);
			}
		}

		// Token: 0x040000B3 RID: 179
		private List<string> m_spritesheetNames;

		// Token: 0x040000B4 RID: 180
		private string m_currentDirectory = "";
	}
}

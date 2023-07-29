using System;
using System.Windows.Controls;

namespace BrawlerEditor
{
	// Token: 0x0200000A RID: 10
	internal class SpriteListBoxItem : ListBoxItem
	{
		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000024 RID: 36 RVA: 0x000024CC File Offset: 0x000006CC
		// (set) Token: 0x06000025 RID: 37 RVA: 0x000024E4 File Offset: 0x000006E4
		public string semiDirectoryPath
		{
			get
			{
				return this.m_fullDirectoryPath;
			}
			set
			{
				this.m_fullDirectoryPath = value;
			}
		}

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000026 RID: 38 RVA: 0x000024F0 File Offset: 0x000006F0
		// (set) Token: 0x06000027 RID: 39 RVA: 0x00002508 File Offset: 0x00000708
		public bool isDirectory
		{
			get
			{
				return this.m_isDirectory;
			}
			set
			{
				this.m_isDirectory = value;
			}
		}

		// Token: 0x04000008 RID: 8
		private bool m_isDirectory;

		// Token: 0x04000009 RID: 9
		private string m_fullDirectoryPath;
	}
}

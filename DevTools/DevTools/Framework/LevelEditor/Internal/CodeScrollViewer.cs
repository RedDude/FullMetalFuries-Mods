using System;
using System.Windows.Controls;
using System.Windows.Documents;

namespace BrawlerEditor
{
	// Token: 0x0200001C RID: 28
	public class CodeScrollViewer : ScrollViewer, IControl
	{
		// Token: 0x17000036 RID: 54
		// (get) Token: 0x06000103 RID: 259 RVA: 0x00006A94 File Offset: 0x00004C94
		// (set) Token: 0x06000104 RID: 260 RVA: 0x00006AAB File Offset: 0x00004CAB
		public MainWindow MainWindow { get; set; }

		// Token: 0x17000037 RID: 55
		// (get) Token: 0x06000105 RID: 261 RVA: 0x00006AB4 File Offset: 0x00004CB4
		public bool TextBoxHasFocus
		{
			get
			{
				return this.m_codeTextBlock != null && this.m_codeTextBlock.IsMouseOver;
			}
		}

		// Token: 0x06000106 RID: 262 RVA: 0x00006AE8 File Offset: 0x00004CE8
		public override void EndInit()
		{
			this.m_codeTextBlock = base.Content as RichTextBox;
			(this.m_codeTextBlock.Document.Blocks.FirstBlock as Paragraph).LineHeight = 1.0;
			this.m_codeTextBlock.TextChanged += this.m_codeTextBlock_TextChanged;
			base.EndInit();
		}

		// Token: 0x06000107 RID: 263 RVA: 0x00006B50 File Offset: 0x00004D50
		private void m_codeTextBlock_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (this.m_currentCodeObj != null)
			{
				TextRange textRange = new TextRange(this.m_codeTextBlock.Document.ContentStart, this.m_codeTextBlock.Document.ContentEnd);
				this.m_currentCodeObj.code = textRange.Text.Substring(0, textRange.Text.Length - 2);
			}
		}

		// Token: 0x06000108 RID: 264 RVA: 0x00006BB8 File Offset: 0x00004DB8
		public void ShowCode(object obj)
		{
			ICodeObj codeObj = obj as ICodeObj;
			if (codeObj != null)
			{
				this.ClearText();
				this.m_codeTextBlock.IsEnabled = true;
				this.m_currentCodeObj = codeObj;
				this.m_codeTextBlock.AppendText(this.m_currentCodeObj.code);
			}
		}

		// Token: 0x06000109 RID: 265 RVA: 0x00006C0C File Offset: 0x00004E0C
		public void ClearText()
		{
			this.m_currentCodeObj = null;
			this.m_codeTextBlock.SelectAll();
			this.m_codeTextBlock.Selection.Text = "";
			this.m_codeTextBlock.Document.Blocks.Add(new Paragraph());
			(this.m_codeTextBlock.Document.Blocks.FirstBlock as Paragraph).LineHeight = 1.0;
		}

		// Token: 0x0600010A RID: 266 RVA: 0x00006C87 File Offset: 0x00004E87
		public void ClearPanel()
		{
			this.ClearText();
			this.m_codeTextBlock.IsEnabled = false;
		}

		// Token: 0x0400007E RID: 126
		private RichTextBox m_codeTextBlock;

		// Token: 0x0400007F RID: 127
		private ICodeObj m_currentCodeObj;
	}
}

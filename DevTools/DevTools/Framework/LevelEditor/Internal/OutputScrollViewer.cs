using System;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace BrawlerEditor
{
	// Token: 0x02000040 RID: 64
	public class OutputScrollViewer : ScrollViewer, IControl
	{
		// Token: 0x1700007A RID: 122
		// (get) Token: 0x06000232 RID: 562 RVA: 0x000110D4 File Offset: 0x0000F2D4
		// (set) Token: 0x06000233 RID: 563 RVA: 0x000110EB File Offset: 0x0000F2EB
		public MainWindow MainWindow { get; set; }

		// Token: 0x1700007B RID: 123
		// (get) Token: 0x06000234 RID: 564 RVA: 0x000110F4 File Offset: 0x0000F2F4
		public bool TextBoxHasFocus
		{
			get
			{
				return this.m_outputTextBlock != null && this.m_outputTextBlock.IsMouseOver;
			}
		}

		// Token: 0x06000235 RID: 565 RVA: 0x00011128 File Offset: 0x0000F328
		public override void EndInit()
		{
			this.m_outputTextBlock = base.Content as RichTextBox;
			(this.m_outputTextBlock.Document.Blocks.FirstBlock as Paragraph).LineHeight = 1.0;
			base.EndInit();
		}

		// Token: 0x06000236 RID: 566 RVA: 0x00011178 File Offset: 0x0000F378
		public void AddOutputText(string text)
		{
			TextRange textRange = new TextRange(this.m_outputTextBlock.Document.ContentEnd, this.m_outputTextBlock.Document.ContentEnd);
			textRange.Text = text + "\n";
			Brush value = Brushes.Black;
			if (text.Length > 5 && text.Substring(0, 5).ToUpper() == "ERROR")
			{
				value = Brushes.Red;
			}
			else if (text.Length > 7 && text.Substring(0, 7).ToUpper() == "WARNING")
			{
				value = Brushes.Blue;
			}
			else if (text.Length > 7 && text.Substring(0, 7).ToUpper() == "SUCCESS")
			{
				value = Brushes.Green;
			}
			textRange.ApplyPropertyValue(TextElement.ForegroundProperty, value);
		}

		// Token: 0x06000237 RID: 567 RVA: 0x00011268 File Offset: 0x0000F468
		public void ClearText()
		{
			this.m_outputTextBlock.SelectAll();
			this.m_outputTextBlock.Selection.Text = "";
			this.m_outputTextBlock.Document.Blocks.Add(new Paragraph());
			(this.m_outputTextBlock.Document.Blocks.FirstBlock as Paragraph).LineHeight = 1.0;
		}

		// Token: 0x04000129 RID: 297
		private RichTextBox m_outputTextBlock;
	}
}

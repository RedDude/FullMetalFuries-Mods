using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace BrawlerEditor
{
	// Token: 0x02000002 RID: 2
	public class CodeDialog : Window, IComponentConnector
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		// (set) Token: 0x06000002 RID: 2 RVA: 0x00002067 File Offset: 0x00000267
		public MainWindow mainWindow { get; set; }

		// Token: 0x06000003 RID: 3 RVA: 0x00002070 File Offset: 0x00000270
		public CodeDialog(MainWindow mainWindow)
		{
			this.InitializeComponent();
			this.mainWindow = mainWindow;
		}

		// Token: 0x06000004 RID: 4 RVA: 0x0000208A File Offset: 0x0000028A
		private void Button_Click(object sender, RoutedEventArgs e)
		{
			base.Close();
		}

		// Token: 0x06000005 RID: 5 RVA: 0x00002094 File Offset: 0x00000294
		protected override void OnKeyDown(KeyEventArgs e)
		{
			if (e.Key == Key.F2)
			{
				base.Close();
			}
			if (this.mainWindow != null && this.mainWindow.CtrlHeld && !e.IsRepeat)
			{
				if (e.Key == Key.S)
				{
					if (this.mainWindow.ShiftHeld)
					{
						this.mainWindow.SaveAsButton_Clicked(null, null);
					}
					else
					{
						this.mainWindow.SaveButton_Clicked(null, null);
					}
				}
			}
			base.OnKeyDown(e);
		}

		// Token: 0x06000006 RID: 6 RVA: 0x0000212A File Offset: 0x0000032A
		protected override void OnClosed(EventArgs e)
		{
			this.mainWindow = null;
			base.OnClosed(e);
		}

		// Token: 0x06000007 RID: 7 RVA: 0x00002140 File Offset: 0x00000340
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void InitializeComponent()
		{
			if (!this._contentLoaded)
			{
				this._contentLoaded = true;
				Uri resourceLocator = new Uri("/BrawlerEditor;component/dialog%20windows/codedialog.xaml", UriKind.Relative);
				Application.LoadComponent(this, resourceLocator);
			}
		}

		// Token: 0x06000008 RID: 8 RVA: 0x0000217C File Offset: 0x0000037C
		[DebuggerNonUserCode]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		void IComponentConnector.Connect(int connectionId, object target)
		{
			switch (connectionId)
			{
			case 1:
				this.codeScrollViewer = (CodeScrollViewer)target;
				break;
			case 2:
				this.codeTextBlock = (RichTextBox)target;
				break;
			default:
				this._contentLoaded = true;
				break;
			}
		}

		// Token: 0x04000001 RID: 1
		internal CodeScrollViewer codeScrollViewer;

		// Token: 0x04000002 RID: 2
		internal RichTextBox codeTextBlock;

		// Token: 0x04000003 RID: 3
		private bool _contentLoaded;
	}
}

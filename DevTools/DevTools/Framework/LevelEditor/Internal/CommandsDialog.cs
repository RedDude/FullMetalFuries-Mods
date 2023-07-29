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
	// Token: 0x02000003 RID: 3
	public class CommandsDialog : Window, IComponentConnector
	{
		// Token: 0x06000009 RID: 9 RVA: 0x000021C1 File Offset: 0x000003C1
		public CommandsDialog()
		{
			this.InitializeComponent();
		}

		// Token: 0x0600000A RID: 10 RVA: 0x000021D3 File Offset: 0x000003D3
		private void Button_Click(object sender, RoutedEventArgs e)
		{
			base.Close();
		}

		// Token: 0x0600000B RID: 11 RVA: 0x000021E0 File Offset: 0x000003E0
		protected override void OnKeyDown(KeyEventArgs e)
		{
			if (e.Key == Key.F1)
			{
				base.Close();
			}
			base.OnKeyDown(e);
		}

		// Token: 0x0600000C RID: 12 RVA: 0x00002210 File Offset: 0x00000410
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void InitializeComponent()
		{
			if (!this._contentLoaded)
			{
				this._contentLoaded = true;
				Uri resourceLocator = new Uri("/BrawlerEditor;component/dialog%20windows/commandsdialog.xaml", UriKind.Relative);
				Application.LoadComponent(this, resourceLocator);
			}
		}

		// Token: 0x0600000D RID: 13 RVA: 0x0000224C File Offset: 0x0000044C
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		void IComponentConnector.Connect(int connectionId, object target)
		{
			if (connectionId != 1)
			{
				this._contentLoaded = true;
			}
			else
			{
				((Button)target).Click += this.Button_Click;
			}
		}

		// Token: 0x04000005 RID: 5
		private bool _contentLoaded;
	}
}

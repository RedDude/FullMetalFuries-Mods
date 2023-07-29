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
	// Token: 0x02000031 RID: 49
	public class ChangeLayerNameDialog : Window, IComponentConnector
	{
		// Token: 0x060001B7 RID: 439 RVA: 0x0000D434 File Offset: 0x0000B634
		public ChangeLayerNameDialog(MainWindow mainWindow, bool createName)
		{
			this.m_createName = createName;
			this.m_mainWindow = mainWindow;
			this.InitializeComponent();
			if (this.m_createName)
			{
				base.Title = "Add New Layer";
				this.changeLayerNameTextBox.Text = "New Layer";
				this.changeLayerNameTextBox.SelectAll();
			}
			else
			{
				base.Title = "Change Layer Name";
				this.changeLayerNameTextBox.Text = ((this.m_mainWindow.layerTabControl.SelectedItem as TabItem).Header as TextBlock).Text;
				this.changeLayerNameTextBox.SelectAll();
			}
			this.changeLayerNameTextBox.Focus();
			this.changeLayerNameTextBox.KeyDown += this.changeLayerNameTextBox_KeyDown;
		}

		// Token: 0x060001B8 RID: 440 RVA: 0x0000D508 File Offset: 0x0000B708
		private void changeLayerNameTextBox_KeyDown(object sender, KeyEventArgs e)
		{
			if (!e.IsRepeat && e.Key == Key.Return)
			{
				this.Button_Click(null, null);
			}
		}

		// Token: 0x060001B9 RID: 441 RVA: 0x0000D53C File Offset: 0x0000B73C
		private void Button_Click(object sender, RoutedEventArgs e)
		{
			if (this.changeLayerNameTextBox.Text.Trim() == "Game Layer")
			{
				MessageBox.Show("Cannot rename layer to \"Game Layer\"", "Change Layer Name", MessageBoxButton.OK, MessageBoxImage.Hand);
			}
			else if (this.changeLayerNameTextBox.Text != null && this.changeLayerNameTextBox.Text != "")
			{
				if (this.m_createName)
				{
					this.m_mainWindow.layerTabControl.AddLayer(this.changeLayerNameTextBox.Text, true);
				}
				else
				{
					this.m_mainWindow.layerTabControl.ChangeSelectedLayerName(this.changeLayerNameTextBox.Text);
				}
				base.DialogResult = new bool?(true);
			}
			else if (this.m_createName)
			{
				MessageBox.Show("Cannot create layer with no name", "Invalid Layer Name", MessageBoxButton.OK, MessageBoxImage.Hand);
			}
			else
			{
				MessageBox.Show("Cannot change layer name to nothing", "Invalid Layer Name", MessageBoxButton.OK, MessageBoxImage.Hand);
			}
		}

		// Token: 0x060001BA RID: 442 RVA: 0x0000D641 File Offset: 0x0000B841
		protected override void OnClosed(EventArgs e)
		{
			this.m_mainWindow = null;
			base.OnClosed(e);
		}

		// Token: 0x060001BB RID: 443 RVA: 0x0000D654 File Offset: 0x0000B854
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[DebuggerNonUserCode]
		public void InitializeComponent()
		{
			if (!this._contentLoaded)
			{
				this._contentLoaded = true;
				Uri resourceLocator = new Uri("/BrawlerEditor;component/dialog%20windows/changelayernamedialog.xaml", UriKind.Relative);
				Application.LoadComponent(this, resourceLocator);
			}
		}

		// Token: 0x060001BC RID: 444 RVA: 0x0000D690 File Offset: 0x0000B890
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DebuggerNonUserCode]
		void IComponentConnector.Connect(int connectionId, object target)
		{
			switch (connectionId)
			{
			case 1:
				this.changeLayerNameTextBox = (TextBox)target;
				break;
			case 2:
				((Button)target).Click += this.Button_Click;
				break;
			default:
				this._contentLoaded = true;
				break;
			}
		}

		// Token: 0x040000C4 RID: 196
		private MainWindow m_mainWindow;

		// Token: 0x040000C5 RID: 197
		private bool m_createName;

		// Token: 0x040000C6 RID: 198
		internal TextBox changeLayerNameTextBox;

		// Token: 0x040000C7 RID: 199
		private bool _contentLoaded;
	}
}

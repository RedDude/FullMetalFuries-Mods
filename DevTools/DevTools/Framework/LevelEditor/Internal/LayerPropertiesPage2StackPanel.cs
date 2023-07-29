using System;
using System.Windows;
using System.Windows.Controls;

namespace BrawlerEditor
{
	// Token: 0x02000009 RID: 9
	public class LayerPropertiesPage2StackPanel : StackPanel, IControl
	{
		// Token: 0x1700000A RID: 10
		// (get) Token: 0x0600001D RID: 29 RVA: 0x00002284 File Offset: 0x00000484
		// (set) Token: 0x0600001E RID: 30 RVA: 0x0000229B File Offset: 0x0000049B
		public MainWindow MainWindow { get; set; }

		// Token: 0x0600001F RID: 31 RVA: 0x000022A4 File Offset: 0x000004A4
		public void ShowProperties(LayerPropertyObj obj)
		{
			this.ClearPanel();
			if (obj != null)
			{
				this.m_selectedObj = obj;
				this.AddCheckBox("Apply Gaussian Blur", "Gaussian", obj.applyGaussianBlur);
				this.AddCheckBox("Add to Game Layer", "AddGameLayer", obj.addToGameLayer);
			}
		}

		// Token: 0x06000020 RID: 32 RVA: 0x000022FC File Offset: 0x000004FC
		public void AddCheckBox(string checkBoxText, string checkBoxName, bool isChecked)
		{
			TextBlock textBlock = new TextBlock();
			textBlock.Margin = new Thickness(0.0, 10.0, 0.0, 0.0);
			textBlock.Text = checkBoxText;
			base.Children.Add(textBlock);
			CheckBox checkBox = new CheckBox();
			checkBox.Name = checkBoxName;
			checkBox.IsChecked = new bool?(isChecked);
			checkBox.Margin = new Thickness(0.0, 5.0, 0.0, 0.0);
			checkBox.Checked += this.CheckBoxEventHandler;
			checkBox.Unchecked += this.CheckBoxEventHandler;
			base.Children.Add(checkBox);
		}

		// Token: 0x06000021 RID: 33 RVA: 0x000023D4 File Offset: 0x000005D4
		private void CheckBoxEventHandler(object sender, RoutedEventArgs args)
		{
			CheckBox checkBox = sender as CheckBox;
			string name = checkBox.Name;
			if (name != null)
			{
				if (!(name == "Gaussian"))
				{
					if (name == "AddGameLayer")
					{
						if (checkBox.IsChecked == true)
						{
							this.m_selectedObj.addToGameLayer = true;
						}
						else
						{
							this.m_selectedObj.addToGameLayer = false;
						}
					}
				}
				else if (checkBox.IsChecked == true)
				{
					this.m_selectedObj.applyGaussianBlur = true;
				}
				else
				{
					this.m_selectedObj.applyGaussianBlur = false;
				}
			}
		}

		// Token: 0x06000022 RID: 34 RVA: 0x00002495 File Offset: 0x00000695
		public void ClearPanel()
		{
			(base.Parent as TabItem).Header = "2";
			this.m_selectedObj = null;
			base.Children.Clear();
		}

		// Token: 0x04000006 RID: 6
		private LayerPropertyObj m_selectedObj;
	}
}

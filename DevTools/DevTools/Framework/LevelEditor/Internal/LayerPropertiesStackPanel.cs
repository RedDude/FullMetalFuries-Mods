using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BrawlerEditor
{
	// Token: 0x0200001A RID: 26
	public class LayerPropertiesStackPanel : StackPanel, IControl
	{
		// Token: 0x1700002D RID: 45
		// (get) Token: 0x060000E8 RID: 232 RVA: 0x00006458 File Offset: 0x00004658
		// (set) Token: 0x060000E9 RID: 233 RVA: 0x0000646F File Offset: 0x0000466F
		public MainWindow MainWindow { get; set; }

		// Token: 0x060000EA RID: 234 RVA: 0x00006478 File Offset: 0x00004678
		public void ShowProperties(LayerPropertyObj obj)
		{
			this.ClearPanel();
			string text = ((this.MainWindow.layerTabControl.SelectedItem as TabItem).Header as TextBlock).Text;
			if (text != "Game Layer")
			{
				(base.Parent as TabItem).Header = text + " Properties";
				if (obj != null)
				{
					this.m_selectedObj = obj;
					this.AddTextBox("Horizontal Parallax (%): ", obj.parallaxX.ToString(), "ParallaxX");
					this.AddTextBox("Vertical Parallax (%): ", obj.parallaxY.ToString(), "ParallaxY");
					this.AddCheckBox("Sort Layer", "SortLayer", obj.sortLayer);
					this.AddTextBox("Layer Overlay (RGBA)", obj.layerOverlay.ToString(), "LayerOverlay");
				}
			}
		}

		// Token: 0x060000EB RID: 235 RVA: 0x0000656C File Offset: 0x0000476C
		public void AddTextBox(string textBlockName, string textBoxText, string textBoxName)
		{
			TextBlock textBlock = new TextBlock();
			textBlock.Margin = new Thickness(0.0, 7.0, 0.0, 0.0);
			textBlock.Text = textBlockName;
			base.Children.Add(textBlock);
			TextBox textBox = new TextBox();
			textBox.Text = textBoxText;
			textBox.Name = textBoxName;
			textBox.TextChanged += this.TextBoxEventHandler;
			textBox.PreviewMouseDoubleClick += new MouseButtonEventHandler(this.HighlightAllText);
			textBox.VerticalAlignment = VerticalAlignment.Top;
			base.Children.Add(textBox);
		}

		// Token: 0x060000EC RID: 236 RVA: 0x00006618 File Offset: 0x00004818
		private void TextBoxEventHandler(object sender, TextChangedEventArgs args)
		{
			TextBox textBox = sender as TextBox;
			float num = 0f;
			string name = textBox.Name;
			if (name != null)
			{
				if (!(name == "ParallaxX"))
				{
					if (!(name == "ParallaxY"))
					{
						if (!(name == "ScrollSpeedX"))
						{
							if (!(name == "ScrollSpeedY"))
							{
								if (name == "LayerOverlay")
								{
									if (textBox.Text == "" || textBox.Text == null)
									{
										this.m_selectedObj.layerOverlay = "255, 255, 255, 255";
									}
									else
									{
										this.m_selectedObj.layerOverlay = textBox.Text;
									}
								}
							}
							else
							{
								float num2 = this.m_selectedObj.scrollSpeedY;
								if (float.TryParse(textBox.Text, out num))
								{
									this.m_selectedObj.scrollSpeedY = num;
								}
							}
						}
						else
						{
							float num2 = this.m_selectedObj.scrollSpeedX;
							if (float.TryParse(textBox.Text, out num))
							{
								this.m_selectedObj.scrollSpeedX = num;
							}
						}
					}
					else
					{
						float num2 = this.m_selectedObj.parallaxY;
						if (float.TryParse(textBox.Text, out num))
						{
							this.m_selectedObj.parallaxY = num;
						}
					}
				}
				else
				{
					float num2 = this.m_selectedObj.parallaxX;
					if (float.TryParse(textBox.Text, out num))
					{
						this.m_selectedObj.parallaxX = num;
					}
				}
			}
			this.MainWindow.ChangesMade = true;
		}

		// Token: 0x060000ED RID: 237 RVA: 0x000067C0 File Offset: 0x000049C0
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

		// Token: 0x060000EE RID: 238 RVA: 0x00006898 File Offset: 0x00004A98
		private void CheckBoxEventHandler(object sender, RoutedEventArgs args)
		{
			CheckBox checkBox = sender as CheckBox;
			string name = checkBox.Name;
			if (name != null)
			{
				if (name == "SortLayer")
				{
					if (checkBox.IsChecked == true)
					{
						this.m_selectedObj.sortLayer = true;
					}
					else
					{
						this.m_selectedObj.sortLayer = false;
					}
				}
			}
		}

		// Token: 0x060000EF RID: 239 RVA: 0x00006908 File Offset: 0x00004B08
		private void HighlightAllText(object sender, RoutedEventArgs e)
		{
			TextBox textBox = sender as TextBox;
			textBox.SelectAll();
		}

		// Token: 0x060000F0 RID: 240 RVA: 0x00006924 File Offset: 0x00004B24
		public void ClearPanel()
		{
			(base.Parent as TabItem).Header = "Layer Properties";
			this.m_selectedObj = null;
			base.Children.Clear();
		}

		// Token: 0x04000074 RID: 116
		private LayerPropertyObj m_selectedObj;
	}
}

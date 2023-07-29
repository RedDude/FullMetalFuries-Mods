using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BrawlerEditor
{
	// Token: 0x02000041 RID: 65
	public class LayerTabControl : TabControl, IControl
	{
		// Token: 0x1700007C RID: 124
		// (get) Token: 0x06000239 RID: 569 RVA: 0x000112E4 File Offset: 0x0000F4E4
		// (set) Token: 0x0600023A RID: 570 RVA: 0x000112FB File Offset: 0x0000F4FB
		public MainWindow MainWindow { get; set; }

		// Token: 0x1700007D RID: 125
		// (get) Token: 0x0600023B RID: 571 RVA: 0x00011304 File Offset: 0x0000F504
		public TabItem GameLayer
		{
			get
			{
				return this.m_gameLayer;
			}
		}

		// Token: 0x1700007E RID: 126
		// (get) Token: 0x0600023C RID: 572 RVA: 0x0001131C File Offset: 0x0000F51C
		public LayerPropertyObj selectedLayerPropertyObj
		{
			get
			{
				LayerPropertyObj result;
				if (base.SelectedIndex != -1)
				{
					result = this.m_layerPropertyObjList[base.SelectedIndex];
				}
				else
				{
					result = null;
				}
				return result;
			}
		}

		// Token: 0x1700007F RID: 127
		// (get) Token: 0x0600023D RID: 573 RVA: 0x00011350 File Offset: 0x0000F550
		public List<LayerPropertyObj> layerPropertyObjList
		{
			get
			{
				return this.m_layerPropertyObjList;
			}
		}

		// Token: 0x17000080 RID: 128
		// (get) Token: 0x0600023E RID: 574 RVA: 0x00011368 File Offset: 0x0000F568
		public int GameLayerIndex
		{
			get
			{
				int num = 0;
				foreach (object obj in ((IEnumerable)base.Items))
				{
					if (obj == this.GameLayer)
					{
						return num;
					}
					num++;
				}
				return -1;
			}
		}

		// Token: 0x0600023F RID: 575 RVA: 0x000113EC File Offset: 0x0000F5EC
		public LayerTabControl()
		{
			this.m_layerPropertyObjList = new List<LayerPropertyObj>();
		}

		// Token: 0x06000240 RID: 576 RVA: 0x00011404 File Offset: 0x0000F604
		protected override void OnInitialized(EventArgs e)
		{
			try
			{
				this.m_gameLayer = base.Items[0] as TabItem;
				this.m_gameLayer.MouseRightButtonDown += this.SelectLayer;
				this.m_layerPropertyObjList.Add(new LayerPropertyObj());
				base.SelectionChanged += this.ActiveLayerChanged;
			}
			catch
			{
			}
			base.OnInitialized(e);
		}

		// Token: 0x06000241 RID: 577 RVA: 0x00011488 File Offset: 0x0000F688
		public void AddLayer(string layerName, bool addUndoAction)
		{
			this.MainWindow.gameScreenControl.AddLayer();
			TabItem tabItem = new TabItem();
			tabItem.MouseRightButtonDown += this.SelectLayer;
			tabItem.Header = new TextBlock
			{
				Text = layerName
			};
			(tabItem.Header as TextBlock).Style = base.FindResource("HeaderTextBlockStyle") as Style;
			base.Items.Add(tabItem);
			this.m_layerPropertyObjList.Add(new LayerPropertyObj());
			base.SelectedItem = tabItem;
			if (addUndoAction)
			{
				this.MainWindow.gameScreenControl.UndoManager.AddAction(new AddLayerUndoAction(this.MainWindow.gameScreenControl.LayerList.Count - 1, layerName, this));
			}
		}

		// Token: 0x06000242 RID: 578 RVA: 0x0001155C File Offset: 0x0000F75C
		public void AddLayerAt(string layerName, int index, bool addUndoAction)
		{
			if (index >= 0 && index < this.MainWindow.gameScreenControl.LayerList.Count)
			{
				this.MainWindow.gameScreenControl.AddLayerAt(index);
				TabItem tabItem = new TabItem();
				tabItem.MouseRightButtonDown += this.SelectLayer;
				tabItem.Header = new TextBlock
				{
					Text = layerName
				};
				(tabItem.Header as TextBlock).Style = base.FindResource("HeaderTextBlockStyle") as Style;
				base.Items.Insert(index, tabItem);
				this.m_layerPropertyObjList.Insert(index, new LayerPropertyObj());
				if (addUndoAction)
				{
					this.MainWindow.gameScreenControl.UndoManager.AddAction(new AddLayerUndoAction(index, layerName, this));
				}
			}
			else
			{
				Console.WriteLine("Cannot add layer at index: " + index + " as it is out of bounds.");
			}
		}

		// Token: 0x06000243 RID: 579 RVA: 0x0001165C File Offset: 0x0000F85C
		public void ChangeLayerPropertiesAt(int index, float parallaxX, float parallaxY, float scrollSpeedX, float scrollSpeedY, bool sortLayer, string layerOverlay, bool applyGaussian, bool addToGameLayer)
		{
			LayerPropertyObj layerPropertyObj = this.m_layerPropertyObjList[index];
			layerPropertyObj.parallaxX = parallaxX;
			layerPropertyObj.parallaxY = parallaxY;
			layerPropertyObj.scrollSpeedX = scrollSpeedX;
			layerPropertyObj.scrollSpeedY = scrollSpeedY;
			layerPropertyObj.sortLayer = sortLayer;
			layerPropertyObj.layerOverlay = layerOverlay;
			layerPropertyObj.addToGameLayer = addToGameLayer;
			layerPropertyObj.applyGaussianBlur = applyGaussian;
		}

		// Token: 0x06000244 RID: 580 RVA: 0x000116C0 File Offset: 0x0000F8C0
		public void ChangeSelectedLayerName(string layerName)
		{
			TabItem tabItem = base.SelectedItem as TabItem;
			if (tabItem != this.m_gameLayer)
			{
				(tabItem.Header as TextBlock).Text = layerName;
			}
			else
			{
				MessageBox.Show("Cannot rename Game Layer", "Change Layer Name", MessageBoxButton.OK, MessageBoxImage.Hand);
			}
		}

		// Token: 0x06000245 RID: 581 RVA: 0x00011710 File Offset: 0x0000F910
		public void SelectLayer(object sender, MouseButtonEventArgs e)
		{
			TabItem tabItem = sender as TabItem;
			tabItem.IsSelected = true;
		}

		// Token: 0x06000246 RID: 582 RVA: 0x00011730 File Offset: 0x0000F930
		private void ActiveLayerChanged(object sender, SelectionChangedEventArgs e)
		{
			if (base.SelectedItem != null)
			{
				(base.SelectedItem as TabItem).Content = this.m_gameLayer.Content;
				if (this.MainWindow != null)
				{
					this.MainWindow.layerPropertiesStackPanel.ShowProperties(this.selectedLayerPropertyObj);
					this.MainWindow.layerPropertiesPage2StackPanel.ShowProperties(this.selectedLayerPropertyObj);
					this.MainWindow.gameScreenControl.DeselectAllGameObjs();
					this.MainWindow.gameScreenControl.UpdateAllObjOpacity(base.SelectedIndex);
				}
			}
		}

		// Token: 0x06000247 RID: 583 RVA: 0x000117D4 File Offset: 0x0000F9D4
		public void RefreshLayerTabs()
		{
			if (this.MainWindow != null)
			{
				this.MainWindow.layerPropertiesStackPanel.ShowProperties(this.selectedLayerPropertyObj);
				this.MainWindow.layerPropertiesPage2StackPanel.ShowProperties(this.selectedLayerPropertyObj);
			}
		}

		// Token: 0x06000248 RID: 584 RVA: 0x00011820 File Offset: 0x0000FA20
		public void TabsRearranged(int indexRemoved, int insertionIndex, bool addUndoAction)
		{
			if (indexRemoved != insertionIndex)
			{
				LayerPropertyObj item = this.m_layerPropertyObjList[indexRemoved];
				this.m_layerPropertyObjList.RemoveAt(indexRemoved);
				this.m_layerPropertyObjList.Insert(insertionIndex, item);
				this.MainWindow.gameScreenControl.SwapLayerObjLists(indexRemoved, insertionIndex);
				if (addUndoAction)
				{
					this.MainWindow.gameScreenControl.UndoManager.AddAction(new MoveLayerUndoAction(this, indexRemoved, insertionIndex));
				}
			}
			base.SelectedIndex = insertionIndex;
		}

		// Token: 0x06000249 RID: 585 RVA: 0x000118A4 File Offset: 0x0000FAA4
		public void RemoveLayerAtIndex(int index, bool addUndoAction)
		{
			TabItem tabItem = base.Items[index] as TabItem;
			if (tabItem != this.m_gameLayer)
			{
				if (addUndoAction)
				{
					this.MainWindow.gameScreenControl.UndoManager.AddAction(new RemoveLayerUndoAction(this, index, (tabItem.Header as TextBlock).Text));
				}
				this.MainWindow.gameScreenControl.RemoveLayerObjList(index);
				base.Items.Remove(tabItem);
				this.m_layerPropertyObjList.RemoveAt(index);
			}
			else
			{
				MessageBox.Show("Cannot remove Game Layer", "Remove Layer", MessageBoxButton.OK, MessageBoxImage.Hand);
			}
		}

		// Token: 0x0600024A RID: 586 RVA: 0x0001194C File Offset: 0x0000FB4C
		public void ResetControl()
		{
			base.Items.Clear();
			base.Items.Add(this.m_gameLayer);
			this.m_layerPropertyObjList.Clear();
			this.m_layerPropertyObjList.Add(new LayerPropertyObj());
			base.SelectedIndex = 0;
		}

		// Token: 0x0400012B RID: 299
		private TabItem m_gameLayer;

		// Token: 0x0400012C RID: 300
		private List<LayerPropertyObj> m_layerPropertyObjList;
	}
}

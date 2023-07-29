using System;
using System.Collections.ObjectModel;
using System.Linq;
using CDGEngine;

namespace BrawlerEditor
{
	// Token: 0x02000025 RID: 37
	public class RemoveLayerUndoAction : UndoAction
	{
		// Token: 0x0600015D RID: 349 RVA: 0x00007E84 File Offset: 0x00006084
		public RemoveLayerUndoAction(LayerTabControl layerTabControl, int index, string layerName)
		{
			this.m_objList = layerTabControl.MainWindow.gameScreenControl.LayerList[index];
			this.m_layerTabControl = layerTabControl;
			this.m_index = index;
			this.m_layerName = layerName;
		}

		// Token: 0x0600015E RID: 350 RVA: 0x00007ED4 File Offset: 0x000060D4
		public override void ExecuteUndo()
		{
			if (this.m_index >= this.m_layerTabControl.MainWindow.gameScreenControl.LayerList.Count)
			{
				this.m_layerTabControl.AddLayer(this.m_layerName, false);
			}
			else
			{
				this.m_layerTabControl.AddLayerAt(this.m_layerName, this.m_index, false);
			}
			this.m_layerTabControl.MainWindow.gameScreenControl.AddGameObjsInLayer(this.m_objList.ToList<GameObj>(), this.m_index, false);
		}

		// Token: 0x0600015F RID: 351 RVA: 0x00007F5E File Offset: 0x0000615E
		public override void ExecuteRedo()
		{
			this.m_layerTabControl.RemoveLayerAtIndex(this.m_index, false);
		}

		// Token: 0x06000160 RID: 352 RVA: 0x00007F74 File Offset: 0x00006174
		public override void Dispose()
		{
			if (!base.IsDisposed)
			{
				foreach (GameObj gameObj in this.m_objList)
				{
					gameObj.Dispose();
				}
				this.m_objList.Clear();
				this.m_objList = null;
				this.m_layerTabControl = null;
				base.Dispose();
			}
		}

		// Token: 0x0400009A RID: 154
		private LayerTabControl m_layerTabControl;

		// Token: 0x0400009B RID: 155
		private ObservableCollection<GameObj> m_objList;

		// Token: 0x0400009C RID: 156
		private int m_index = -1;

		// Token: 0x0400009D RID: 157
		private string m_layerName;
	}
}

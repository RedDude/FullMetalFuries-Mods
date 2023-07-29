using System;
using System.Windows.Controls;

namespace BrawlerEditor
{
	// Token: 0x0200001F RID: 31
	public class MoveLayerUndoAction : UndoAction
	{
		// Token: 0x06000118 RID: 280 RVA: 0x000070C3 File Offset: 0x000052C3
		public MoveLayerUndoAction(LayerTabControl layerTabControl, int indexRemoved, int insertionIndex)
		{
			this.m_layerTabControl = layerTabControl;
			this.m_indexRemoved = indexRemoved;
			this.m_insertionIndex = insertionIndex;
		}

		// Token: 0x06000119 RID: 281 RVA: 0x000070E4 File Offset: 0x000052E4
		public override void ExecuteUndo()
		{
			TabItem insertItem = this.m_layerTabControl.Items[this.m_insertionIndex] as TabItem;
			this.m_layerTabControl.Items.RemoveAt(this.m_insertionIndex);
			this.m_layerTabControl.Items.Insert(this.m_indexRemoved, insertItem);
			this.m_layerTabControl.TabsRearranged(this.m_insertionIndex, this.m_indexRemoved, false);
		}

		// Token: 0x0600011A RID: 282 RVA: 0x00007158 File Offset: 0x00005358
		public override void ExecuteRedo()
		{
			TabItem insertItem = this.m_layerTabControl.Items[this.m_indexRemoved] as TabItem;
			this.m_layerTabControl.Items.RemoveAt(this.m_indexRemoved);
			this.m_layerTabControl.Items.Insert(this.m_insertionIndex, insertItem);
			this.m_layerTabControl.TabsRearranged(this.m_indexRemoved, this.m_insertionIndex, false);
		}

		// Token: 0x0600011B RID: 283 RVA: 0x000071CC File Offset: 0x000053CC
		public override void Dispose()
		{
			if (!base.IsDisposed)
			{
				this.m_layerTabControl = null;
				base.Dispose();
			}
		}

		// Token: 0x04000085 RID: 133
		private int m_indexRemoved;

		// Token: 0x04000086 RID: 134
		private int m_insertionIndex;

		// Token: 0x04000087 RID: 135
		private LayerTabControl m_layerTabControl;
	}
}

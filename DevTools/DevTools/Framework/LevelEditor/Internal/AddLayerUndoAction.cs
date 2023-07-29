using System;

namespace BrawlerEditor
{
	// Token: 0x02000020 RID: 32
	public class AddLayerUndoAction : UndoAction
	{
		// Token: 0x0600011C RID: 284 RVA: 0x000071F4 File Offset: 0x000053F4
		public AddLayerUndoAction(int index, string layerName, LayerTabControl layerTabControl)
		{
			this.m_layerName = layerName;
			this.m_layerTabControl = layerTabControl;
			this.m_index = index;
		}

		// Token: 0x0600011D RID: 285 RVA: 0x0000721B File Offset: 0x0000541B
		public override void ExecuteUndo()
		{
			this.m_layerTabControl.RemoveLayerAtIndex(this.m_index, false);
		}

		// Token: 0x0600011E RID: 286 RVA: 0x00007231 File Offset: 0x00005431
		public override void ExecuteRedo()
		{
			this.m_layerTabControl.AddLayer(this.m_layerName, false);
		}

		// Token: 0x0600011F RID: 287 RVA: 0x00007248 File Offset: 0x00005448
		public override void Dispose()
		{
			if (!base.IsDisposed)
			{
				this.m_layerTabControl = null;
				base.Dispose();
			}
		}

		// Token: 0x04000088 RID: 136
		private LayerTabControl m_layerTabControl;

		// Token: 0x04000089 RID: 137
		private int m_index = -1;

		// Token: 0x0400008A RID: 138
		private string m_layerName;
	}
}

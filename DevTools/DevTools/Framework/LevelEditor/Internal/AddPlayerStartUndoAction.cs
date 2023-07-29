using System;
using CDGEngine;

namespace BrawlerEditor
{
	// Token: 0x02000011 RID: 17
	public class AddPlayerStartUndoAction : UndoAction
	{
		// Token: 0x06000055 RID: 85 RVA: 0x000036A6 File Offset: 0x000018A6
		public AddPlayerStartUndoAction(GameObj objAdded, GameObj objRemoved, int objRemovedLayer, GameScreenControl gameScreenControl)
		{
			this.m_objAdded = objAdded;
			this.m_objRemoved = objRemoved;
			this.m_objRemovedLayer = objRemovedLayer;
			this.m_gameScreenControl = gameScreenControl;
			this.m_layer = gameScreenControl.MainWindow.layerTabControl.SelectedIndex;
		}

		// Token: 0x06000056 RID: 86 RVA: 0x000036E8 File Offset: 0x000018E8
		public override void ExecuteUndo()
		{
			this.m_gameScreenControl.DeselectAllGameObjs();
			this.m_gameScreenControl.RemoveGameObjInLayer(this.m_objAdded, this.m_layer, false);
			if (this.m_objRemoved != null)
			{
				this.m_gameScreenControl.AddGameObjInLayer(this.m_objRemoved, this.m_objRemovedLayer, false);
			}
		}

		// Token: 0x06000057 RID: 87 RVA: 0x00003744 File Offset: 0x00001944
		public override void ExecuteRedo()
		{
			this.m_gameScreenControl.DeselectAllGameObjs();
			if (this.m_objRemoved != null)
			{
				this.m_gameScreenControl.RemoveGameObjInLayer(this.m_objRemoved, this.m_objRemovedLayer, false);
			}
			this.m_gameScreenControl.AddGameObjInLayer(this.m_objAdded, this.m_layer, false);
		}

		// Token: 0x06000058 RID: 88 RVA: 0x000037A0 File Offset: 0x000019A0
		public override void Dispose()
		{
			if (!base.IsDisposed)
			{
				this.m_objRemoved = null;
				this.m_objAdded = null;
				this.m_gameScreenControl = null;
				base.Dispose();
			}
		}

		// Token: 0x0400002B RID: 43
		private GameScreenControl m_gameScreenControl;

		// Token: 0x0400002C RID: 44
		private int m_layer;

		// Token: 0x0400002D RID: 45
		private GameObj m_objAdded;

		// Token: 0x0400002E RID: 46
		private GameObj m_objRemoved;

		// Token: 0x0400002F RID: 47
		private int m_objRemovedLayer;
	}
}

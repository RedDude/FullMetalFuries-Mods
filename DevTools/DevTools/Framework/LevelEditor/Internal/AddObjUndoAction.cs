using System;
using System.Collections.Generic;
using CDGEngine;

namespace BrawlerEditor
{
	// Token: 0x0200002F RID: 47
	public class AddObjUndoAction : UndoAction
	{
		// Token: 0x06000199 RID: 409 RVA: 0x0000CBAB File Offset: 0x0000ADAB
		public AddObjUndoAction(List<GameObj> objs, GameScreenControl gameScreenControl)
		{
			this.m_objList = objs;
			this.m_gameScreenControl = gameScreenControl;
			this.m_layer = gameScreenControl.MainWindow.layerTabControl.SelectedIndex;
		}

		// Token: 0x0600019A RID: 410 RVA: 0x0000CBDA File Offset: 0x0000ADDA
		public AddObjUndoAction(GameObj obj, GameScreenControl gameScreenControl)
		{
			this.m_objList = new List<GameObj>();
			this.m_objList.Add(obj);
			this.m_gameScreenControl = gameScreenControl;
			this.m_layer = gameScreenControl.MainWindow.layerTabControl.SelectedIndex;
		}

		// Token: 0x0600019B RID: 411 RVA: 0x0000CC1A File Offset: 0x0000AE1A
		public override void ExecuteUndo()
		{
			this.m_gameScreenControl.DeselectAllGameObjs();
			this.m_gameScreenControl.RemoveGameObjsInLayer(this.m_objList, this.m_layer, false);
		}

		// Token: 0x0600019C RID: 412 RVA: 0x0000CC42 File Offset: 0x0000AE42
		public override void ExecuteRedo()
		{
			this.m_gameScreenControl.DeselectAllGameObjs();
			this.m_gameScreenControl.AddGameObjsInLayer(this.m_objList, this.m_layer, false);
		}

		// Token: 0x0600019D RID: 413 RVA: 0x0000CC6C File Offset: 0x0000AE6C
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
				this.m_gameScreenControl = null;
				base.Dispose();
			}
		}

		// Token: 0x040000B6 RID: 182
		private List<GameObj> m_objList;

		// Token: 0x040000B7 RID: 183
		private GameScreenControl m_gameScreenControl;

		// Token: 0x040000B8 RID: 184
		private int m_layer;
	}
}

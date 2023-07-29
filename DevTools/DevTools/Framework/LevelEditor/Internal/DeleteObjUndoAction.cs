using System;
using System.Collections.Generic;
using CDGEngine;

namespace BrawlerEditor
{
	// Token: 0x02000035 RID: 53
	public class DeleteObjUndoAction : UndoAction
	{
		// Token: 0x060001E9 RID: 489 RVA: 0x0000E697 File Offset: 0x0000C897
		public DeleteObjUndoAction(GameObj obj, GameScreenControl gameScreenControl)
		{
			this.m_objList = new List<GameObj>();
			this.m_objList.Add(obj);
			this.m_gameScreenControl = gameScreenControl;
			this.m_layer = gameScreenControl.MainWindow.layerTabControl.SelectedIndex;
		}

		// Token: 0x060001EA RID: 490 RVA: 0x0000E6D7 File Offset: 0x0000C8D7
		public DeleteObjUndoAction(List<GameObj> objsToDeleteList, GameScreenControl gameScreenControl)
		{
			this.m_objList = objsToDeleteList;
			this.m_gameScreenControl = gameScreenControl;
			this.m_layer = gameScreenControl.MainWindow.layerTabControl.SelectedIndex;
		}

		// Token: 0x060001EB RID: 491 RVA: 0x0000E706 File Offset: 0x0000C906
		public override void ExecuteUndo()
		{
			this.m_gameScreenControl.AddGameObjsInLayer(this.m_objList, this.m_layer, false);
		}

		// Token: 0x060001EC RID: 492 RVA: 0x0000E722 File Offset: 0x0000C922
		public override void ExecuteRedo()
		{
			this.m_gameScreenControl.RemoveGameObjsInLayer(this.m_objList, this.m_layer, false);
		}

		// Token: 0x060001ED RID: 493 RVA: 0x0000E740 File Offset: 0x0000C940
		public override void Dispose()
		{
			if (base.IsDisposed)
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

		// Token: 0x040000E3 RID: 227
		private List<GameObj> m_objList;

		// Token: 0x040000E4 RID: 228
		private GameScreenControl m_gameScreenControl;

		// Token: 0x040000E5 RID: 229
		private int m_layer;
	}
}

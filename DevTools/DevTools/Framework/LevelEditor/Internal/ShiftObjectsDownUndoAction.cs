using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CDGEngine;

namespace BrawlerEditor
{
	// Token: 0x0200001D RID: 29
	public class ShiftObjectsDownUndoAction : UndoAction
	{
		// Token: 0x0600010C RID: 268 RVA: 0x00006CA6 File Offset: 0x00004EA6
		public ShiftObjectsDownUndoAction(ObservableCollection<GameObj> selectedObjs, ObservableCollection<GameObj> layer)
		{
			this.m_selectedObjs = selectedObjs.ToList<GameObj>();
			this.m_layer = layer;
		}

		// Token: 0x0600010D RID: 269 RVA: 0x00006CF8 File Offset: 0x00004EF8
		public override void ExecuteUndo()
		{
			this.m_selectedObjs.Sort((GameObj obj1, GameObj obj2) => this.m_layer.IndexOf(obj2).CompareTo(this.m_layer.IndexOf(obj1)));
			foreach (GameObj item in this.m_selectedObjs)
			{
				int num = this.m_layer.IndexOf(item);
				if (num >= this.m_layer.Count - 1)
				{
					break;
				}
				this.m_layer.Remove(item);
				this.m_layer.Insert(num + 1, item);
			}
		}

		// Token: 0x0600010E RID: 270 RVA: 0x00006DD8 File Offset: 0x00004FD8
		public override void ExecuteRedo()
		{
			this.m_selectedObjs.Sort((GameObj obj1, GameObj obj2) => this.m_layer.IndexOf(obj1).CompareTo(this.m_layer.IndexOf(obj2)));
			foreach (GameObj item in this.m_selectedObjs)
			{
				int num = this.m_layer.IndexOf(item);
				if (num <= 0)
				{
					break;
				}
				this.m_layer.Remove(item);
				this.m_layer.Insert(num - 1, item);
			}
		}

		// Token: 0x0600010F RID: 271 RVA: 0x00006E78 File Offset: 0x00005078
		public override void Dispose()
		{
			if (!base.IsDisposed)
			{
				this.m_layer = null;
				this.m_selectedObjs.Clear();
				this.m_selectedObjs = null;
				base.Dispose();
			}
		}

		// Token: 0x04000081 RID: 129
		private List<GameObj> m_selectedObjs;

		// Token: 0x04000082 RID: 130
		private ObservableCollection<GameObj> m_layer;
	}
}

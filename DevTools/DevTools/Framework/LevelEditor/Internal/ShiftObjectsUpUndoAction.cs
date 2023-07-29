using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CDGEngine;

namespace BrawlerEditor
{
	// Token: 0x0200001E RID: 30
	public class ShiftObjectsUpUndoAction : UndoAction
	{
		// Token: 0x06000112 RID: 274 RVA: 0x00006EB3 File Offset: 0x000050B3
		public ShiftObjectsUpUndoAction(ObservableCollection<GameObj> selectedObjs, ObservableCollection<GameObj> layer)
		{
			this.m_selectedObjs = selectedObjs.ToList<GameObj>();
			this.m_layer = layer;
		}

		// Token: 0x06000113 RID: 275 RVA: 0x00006F08 File Offset: 0x00005108
		public override void ExecuteUndo()
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

		// Token: 0x06000114 RID: 276 RVA: 0x00006FDC File Offset: 0x000051DC
		public override void ExecuteRedo()
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

		// Token: 0x06000115 RID: 277 RVA: 0x00007088 File Offset: 0x00005288
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

		// Token: 0x04000083 RID: 131
		private List<GameObj> m_selectedObjs;

		// Token: 0x04000084 RID: 132
		private ObservableCollection<GameObj> m_layer;
	}
}

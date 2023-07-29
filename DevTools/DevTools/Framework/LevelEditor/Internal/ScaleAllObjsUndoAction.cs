using System;
using System.Collections.Generic;
using System.Linq;
using CDGEngine;
using Microsoft.Xna.Framework;

namespace BrawlerEditor
{
	// Token: 0x02000012 RID: 18
	public class ScaleAllObjsUndoAction : UndoAction
	{
		// Token: 0x06000059 RID: 89 RVA: 0x000037D8 File Offset: 0x000019D8
		public ScaleAllObjsUndoAction(List<GameObj> objList, List<Vector2> previousScaleList, List<Vector2> previousPosList)
		{
			this.m_objList = objList.ToList<GameObj>();
			this.m_newScaleList = new List<Vector2>();
			this.m_newPosList = new List<Vector2>();
			this.m_previousScaleList = previousScaleList.ToList<Vector2>();
			this.m_previousPosList = previousPosList.ToList<Vector2>();
			foreach (GameObj gameObj in this.m_objList)
			{
				this.m_newScaleList.Add(gameObj.Scale);
				this.m_newPosList.Add(gameObj.Position);
			}
		}

		// Token: 0x0600005A RID: 90 RVA: 0x00003894 File Offset: 0x00001A94
		public override void ExecuteUndo()
		{
			for (int i = 0; i < this.m_objList.Count; i++)
			{
				this.m_objList[i].Scale = this.m_previousScaleList[i];
				this.m_objList[i].Position = this.m_previousPosList[i];
			}
		}

		// Token: 0x0600005B RID: 91 RVA: 0x000038FC File Offset: 0x00001AFC
		public override void ExecuteRedo()
		{
			for (int i = 0; i < this.m_objList.Count; i++)
			{
				this.m_objList[i].Position = this.m_newScaleList[i];
				this.m_objList[i].Position = this.m_newPosList[i];
			}
		}

		// Token: 0x0600005C RID: 92 RVA: 0x00003964 File Offset: 0x00001B64
		public override void Dispose()
		{
			if (!base.IsDisposed)
			{
				this.m_objList.Clear();
				this.m_objList = null;
				this.m_newScaleList.Clear();
				this.m_newScaleList = null;
				this.m_previousScaleList.Clear();
				this.m_previousScaleList = null;
				base.Dispose();
			}
		}

		// Token: 0x04000030 RID: 48
		private List<GameObj> m_objList;

		// Token: 0x04000031 RID: 49
		private List<Vector2> m_previousScaleList;

		// Token: 0x04000032 RID: 50
		private List<Vector2> m_newScaleList;

		// Token: 0x04000033 RID: 51
		private List<Vector2> m_previousPosList;

		// Token: 0x04000034 RID: 52
		private List<Vector2> m_newPosList;
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using CDGEngine;
using Microsoft.Xna.Framework;

namespace BrawlerEditor
{
	// Token: 0x02000036 RID: 54
	public class MoveObjsUndoAction : UndoAction
	{
		// Token: 0x060001EE RID: 494 RVA: 0x0000E7CC File Offset: 0x0000C9CC
		public MoveObjsUndoAction(GameObj obj, Vector2 prevPos)
		{
			this.m_objList = new List<GameObj>();
			this.m_previousPositionList = new List<Vector2>();
			this.m_newPositionList = new List<Vector2>();
			this.m_objList.Add(obj);
			this.m_previousPositionList.Add(prevPos);
			this.m_newPositionList.Add(obj.Position);
		}

		// Token: 0x060001EF RID: 495 RVA: 0x0000E830 File Offset: 0x0000CA30
		public MoveObjsUndoAction(List<GameObj> objList, List<Vector2> previousPositionList)
		{
			this.m_objList = objList.ToList<GameObj>();
			this.m_previousPositionList = previousPositionList.ToList<Vector2>();
			this.m_newPositionList = new List<Vector2>();
			foreach (GameObj gameObj in this.m_objList)
			{
				this.m_newPositionList.Add(gameObj.Position);
			}
		}

		// Token: 0x060001F0 RID: 496 RVA: 0x0000E8C0 File Offset: 0x0000CAC0
		public override void ExecuteUndo()
		{
			for (int i = 0; i < this.m_objList.Count; i++)
			{
				this.m_objList[i].Position = this.m_previousPositionList[i];
			}
		}

		// Token: 0x060001F1 RID: 497 RVA: 0x0000E908 File Offset: 0x0000CB08
		public override void ExecuteRedo()
		{
			for (int i = 0; i < this.m_objList.Count; i++)
			{
				this.m_objList[i].Position = this.m_newPositionList[i];
			}
		}

		// Token: 0x060001F2 RID: 498 RVA: 0x0000E950 File Offset: 0x0000CB50
		public override void Dispose()
		{
			if (!base.IsDisposed)
			{
				this.m_objList.Clear();
				this.m_objList = null;
				this.m_previousPositionList.Clear();
				this.m_previousPositionList = null;
				this.m_newPositionList.Clear();
				this.m_newPositionList = null;
				base.Dispose();
			}
		}

		// Token: 0x040000E6 RID: 230
		private List<GameObj> m_objList;

		// Token: 0x040000E7 RID: 231
		private List<Vector2> m_previousPositionList;

		// Token: 0x040000E8 RID: 232
		private List<Vector2> m_newPositionList;
	}
}

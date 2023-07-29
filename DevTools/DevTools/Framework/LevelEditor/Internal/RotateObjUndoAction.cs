using System;
using System.Collections.Generic;
using System.Linq;
using CDGCore;
using CDGEngine;
using Microsoft.Xna.Framework;

namespace BrawlerEditor
{
	// Token: 0x02000013 RID: 19
	public class RotateObjUndoAction : UndoAction
	{
		// Token: 0x0600005D RID: 93 RVA: 0x000039C0 File Offset: 0x00001BC0
		public RotateObjUndoAction(List<GameObj> objsRotated, List<Vector2> startingPos, List<float> startingRots)
		{
			int count = objsRotated.Count;
			CDGDebug.Assert(startingPos.Count == count && startingRots.Count == count, "Mismatched objects rotated. Cannot create undo action.");
			this.m_objList = objsRotated.ToList<GameObj>();
			this.m_startingPositions = startingPos.ToList<Vector2>();
			this.m_startingRotations = startingRots.ToList<float>();
			this.m_endingPositions = new List<Vector2>();
			this.m_endingRotations = new List<float>();
			foreach (GameObj gameObj in objsRotated)
			{
				this.m_endingPositions.Add(gameObj.AbsPosition);
				this.m_endingRotations.Add(gameObj.AbsRotation);
			}
		}

		// Token: 0x0600005E RID: 94 RVA: 0x00003AA0 File Offset: 0x00001CA0
		public RotateObjUndoAction(GameObj objRotated, float startingRot)
		{
			this.m_objList = new List<GameObj>();
			this.m_objList.Add(objRotated);
			this.m_startingPositions = new List<Vector2>();
			this.m_startingPositions.Add(objRotated.AbsPosition);
			this.m_startingRotations = new List<float>();
			this.m_startingRotations.Add(startingRot);
			this.m_endingPositions = new List<Vector2>();
			this.m_endingRotations = new List<float>();
			foreach (GameObj gameObj in this.m_objList)
			{
				this.m_endingPositions.Add(gameObj.AbsPosition);
				this.m_endingRotations.Add(gameObj.AbsRotation);
			}
		}

		// Token: 0x0600005F RID: 95 RVA: 0x00003B84 File Offset: 0x00001D84
		public override void ExecuteUndo()
		{
			for (int i = 0; i < this.m_endingPositions.Count; i++)
			{
				this.m_objList[i].Position = this.m_startingPositions[i];
				this.m_objList[i].Rotation = this.m_startingRotations[i];
			}
		}

		// Token: 0x06000060 RID: 96 RVA: 0x00003BEC File Offset: 0x00001DEC
		public override void ExecuteRedo()
		{
			for (int i = 0; i < this.m_endingPositions.Count; i++)
			{
				this.m_objList[i].Position = this.m_endingPositions[i];
				this.m_objList[i].Rotation = this.m_endingRotations[i];
			}
		}

		// Token: 0x06000061 RID: 97 RVA: 0x00003C54 File Offset: 0x00001E54
		public override void Dispose()
		{
			if (base.IsDisposed)
			{
				this.m_objList.Clear();
				this.m_objList = null;
				this.m_startingPositions.Clear();
				this.m_startingPositions = null;
				this.m_startingRotations.Clear();
				this.m_startingRotations = null;
				this.m_endingPositions.Clear();
				this.m_endingPositions = null;
				this.m_endingRotations.Clear();
				this.m_endingRotations = null;
				base.Dispose();
			}
		}

		// Token: 0x04000035 RID: 53
		private List<GameObj> m_objList;

		// Token: 0x04000036 RID: 54
		private List<float> m_startingRotations;

		// Token: 0x04000037 RID: 55
		private List<float> m_endingRotations;

		// Token: 0x04000038 RID: 56
		private List<Vector2> m_startingPositions;

		// Token: 0x04000039 RID: 57
		private List<Vector2> m_endingPositions;
	}
}

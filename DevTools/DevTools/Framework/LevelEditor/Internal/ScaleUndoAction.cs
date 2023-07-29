using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CDGEngine;
using Microsoft.Xna.Framework;

namespace BrawlerEditor
{
	// Token: 0x02000028 RID: 40
	public class ScaleUndoAction : UndoAction
	{
		// Token: 0x0600016E RID: 366 RVA: 0x0000A3C8 File Offset: 0x000085C8
		public ScaleUndoAction(ObservableCollection<GameObj> objList, List<Vector2> previousScales, List<Vector2> previousPos)
		{
			this.m_objList = objList.ToList<GameObj>();
			this.m_previousScales = previousScales.ToList<Vector2>();
			this.m_previousPos = previousPos.ToList<Vector2>();
			this.m_currentPos = new List<Vector2>();
			this.m_currentScales = new List<Vector2>();
			foreach (GameObj gameObj in this.m_objList)
			{
				IScaleableObj scaleableObj = gameObj as IScaleableObj;
				if (scaleableObj != null)
				{
					this.m_currentScales.Add(new Vector2(scaleableObj.Width, scaleableObj.Height));
				}
				else
				{
					this.m_currentScales.Add(gameObj.Scale);
				}
				this.m_currentPos.Add(gameObj.Position);
			}
		}

		// Token: 0x0600016F RID: 367 RVA: 0x0000A4B0 File Offset: 0x000086B0
		public ScaleUndoAction(GameObj obj, Vector2 previousScale, Vector2 previousPos)
		{
			this.m_objList = new List<GameObj>();
			this.m_previousScales = new List<Vector2>();
			this.m_previousPos = new List<Vector2>();
			this.m_currentPos = new List<Vector2>();
			this.m_currentScales = new List<Vector2>();
			this.m_objList.Add(obj);
			this.m_previousScales.Add(previousScale);
			this.m_previousPos.Add(previousPos);
			this.m_currentPos.Add(obj.Position);
			IScaleableObj scaleableObj = obj as IScaleableObj;
			if (scaleableObj != null)
			{
				this.m_currentScales.Add(new Vector2(scaleableObj.Width, scaleableObj.Height));
			}
			else
			{
				this.m_currentScales.Add(obj.Scale);
			}
		}

		// Token: 0x06000170 RID: 368 RVA: 0x0000A578 File Offset: 0x00008778
		public override void ExecuteUndo()
		{
			for (int i = 0; i < this.m_objList.Count; i++)
			{
				GameObj gameObj = this.m_objList[i];
				Vector2 vector = this.m_previousScales[i];
				Vector2 position = this.m_previousPos[i];
				IScaleableObj scaleableObj = gameObj as IScaleableObj;
				if (scaleableObj != null)
				{
					scaleableObj.Width = vector.X;
					scaleableObj.Height = vector.Y;
				}
				else
				{
					gameObj.ScaleX = vector.X;
					gameObj.ScaleY = vector.Y;
				}
				gameObj.Position = position;
			}
		}

		// Token: 0x06000171 RID: 369 RVA: 0x0000A62C File Offset: 0x0000882C
		public override void ExecuteRedo()
		{
			for (int i = 0; i < this.m_objList.Count; i++)
			{
				GameObj gameObj = this.m_objList[i];
				Vector2 vector = this.m_currentScales[i];
				Vector2 position = this.m_currentPos[i];
				IScaleableObj scaleableObj = gameObj as IScaleableObj;
				if (scaleableObj != null)
				{
					scaleableObj.Width = vector.X;
					scaleableObj.Height = vector.Y;
				}
				else
				{
					gameObj.ScaleX = vector.X;
					gameObj.ScaleY = vector.Y;
				}
				gameObj.Position = position;
			}
		}

		// Token: 0x06000172 RID: 370 RVA: 0x0000A6E0 File Offset: 0x000088E0
		public override void Dispose()
		{
			if (!base.IsDisposed)
			{
				this.m_objList.Clear();
				this.m_objList = null;
				this.m_currentPos.Clear();
				this.m_currentPos = null;
				this.m_currentScales.Clear();
				this.m_currentScales = null;
				this.m_previousPos.Clear();
				this.m_previousPos = null;
				this.m_previousScales.Clear();
				this.m_previousScales = null;
				base.Dispose();
			}
		}

		// Token: 0x040000A1 RID: 161
		private List<GameObj> m_objList;

		// Token: 0x040000A2 RID: 162
		private List<Vector2> m_previousScales;

		// Token: 0x040000A3 RID: 163
		private List<Vector2> m_currentScales;

		// Token: 0x040000A4 RID: 164
		private List<Vector2> m_previousPos;

		// Token: 0x040000A5 RID: 165
		private List<Vector2> m_currentPos;
	}
}

using System;
using Brawler2D;
using CDGEngine;
using Microsoft.Xna.Framework;
using SpriteSystem2;

namespace BrawlerEditor
{
	// Token: 0x0200000C RID: 12
	public class EditorEnemyObj : ContainerObj, IEditorAnchorObj, IEditorDrawableObj
	{
		// Token: 0x1700000E RID: 14
		// (get) Token: 0x0600002B RID: 43 RVA: 0x0000251C File Offset: 0x0000071C
		// (set) Token: 0x0600002C RID: 44 RVA: 0x00002533 File Offset: 0x00000733
		public bool forceAddToStage { get; set; }

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x0600002D RID: 45 RVA: 0x0000253C File Offset: 0x0000073C
		// (set) Token: 0x0600002E RID: 46 RVA: 0x00002554 File Offset: 0x00000754
		public Vector2 editorAnchor
		{
			get
			{
				return this.m_editorAnchor;
			}
			set
			{
				this.m_editorAnchor = value;
			}
		}

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x0600002F RID: 47 RVA: 0x00002560 File Offset: 0x00000760
		// (set) Token: 0x06000030 RID: 48 RVA: 0x0000257D File Offset: 0x0000077D
		public float editorAnchorX
		{
			get
			{
				return this.m_editorAnchor.X;
			}
			set
			{
				this.m_editorAnchor.X = value;
			}
		}

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000031 RID: 49 RVA: 0x0000258C File Offset: 0x0000078C
		// (set) Token: 0x06000032 RID: 50 RVA: 0x000025A9 File Offset: 0x000007A9
		public float editorAnchorY
		{
			get
			{
				return this.m_editorAnchor.Y;
			}
			set
			{
				this.m_editorAnchor.Y = value;
			}
		}

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x06000033 RID: 51 RVA: 0x000025B8 File Offset: 0x000007B8
		// (set) Token: 0x06000034 RID: 52 RVA: 0x000025D0 File Offset: 0x000007D0
		public EnemyType enemyType
		{
			get
			{
				return this.m_enemyType;
			}
			set
			{
				this.m_enemyType = value;
			}
		}

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x06000035 RID: 53 RVA: 0x000025DC File Offset: 0x000007DC
		// (set) Token: 0x06000036 RID: 54 RVA: 0x000025F4 File Offset: 0x000007F4
		public Vector2 ingameScale
		{
			get
			{
				return this.m_ingameScale;
			}
			set
			{
				this.m_ingameScale = value;
			}
		}

		// Token: 0x06000037 RID: 55 RVA: 0x000025FE File Offset: 0x000007FE
		public EditorEnemyObj(string containerName)
			: base(containerName)
		{
			this.m_ingameScale = Vector2.One;
		}

		// Token: 0x06000038 RID: 56 RVA: 0x00002618 File Offset: 0x00000818
		public override void ChangeSprite(string spriteName)
		{
			base.ChangeSprite(spriteName);
			ContainerData[] containerDataList = SpriteLibrary.GetContainerDataList(spriteName);
			this.editorAnchorX = containerDataList[0].EditorAnchorX;
			this.editorAnchorY = containerDataList[0].EditorAnchorY;
		}

		// Token: 0x06000039 RID: 57 RVA: 0x0000265C File Offset: 0x0000085C
		public override void Draw(Camera2D camera, float elapsedSeconds)
		{
			base.Draw(camera, elapsedSeconds);
			camera.Draw(StaticTexture.GenericTexture, new Rectangle((int)this.X + (int)this.editorAnchorX - 2, (int)this.Y + (int)this.editorAnchorY - 2, 4, 4), Color.HotPink);
		}

		// Token: 0x0600003A RID: 58 RVA: 0x000026B0 File Offset: 0x000008B0
		public override void PopulateClone(object obj)
		{
			EditorEnemyObj editorEnemyObj = obj as EditorEnemyObj;
			editorEnemyObj.ingameScale = this.ingameScale;
			editorEnemyObj.enemyType = this.enemyType;
			base.PopulateClone(obj);
		}

		// Token: 0x0600003B RID: 59 RVA: 0x000026E8 File Offset: 0x000008E8
		public override GameObj CreateClone()
		{
			EditorEnemyObj editorEnemyObj = new EditorEnemyObj(this.spriteName);
			this.PopulateClone(editorEnemyObj);
			editorEnemyObj.editorAnchorX = this.editorAnchorX;
			editorEnemyObj.editorAnchorY = this.editorAnchorY;
			return editorEnemyObj;
		}

		// Token: 0x0400000A RID: 10
		private Vector2 m_ingameScale;

		// Token: 0x0400000B RID: 11
		private EnemyType m_enemyType;

		// Token: 0x0400000C RID: 12
		private Vector2 m_editorAnchor;
	}
}

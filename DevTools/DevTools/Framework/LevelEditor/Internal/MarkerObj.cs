using System;
using CDGEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpriteSystem2;

namespace BrawlerEditor
{
	// Token: 0x02000022 RID: 34
	public class MarkerObj : GameObj, ICodeObj
	{
		// Token: 0x1700003A RID: 58
		// (get) Token: 0x06000124 RID: 292 RVA: 0x00007270 File Offset: 0x00005470
		// (set) Token: 0x06000125 RID: 293 RVA: 0x00007287 File Offset: 0x00005487
		public string code { get; set; }

		// Token: 0x1700003B RID: 59
		// (get) Token: 0x06000126 RID: 294 RVA: 0x00007290 File Offset: 0x00005490
		// (set) Token: 0x06000127 RID: 295 RVA: 0x000072A8 File Offset: 0x000054A8
		public int ID
		{
			get
			{
				return this.m_id;
			}
			set
			{
				this.m_id = value;
			}
		}

		// Token: 0x1700003C RID: 60
		// (get) Token: 0x06000128 RID: 296 RVA: 0x000072B4 File Offset: 0x000054B4
		public override CDGRect Bounds
		{
			get
			{
				return this.AbsBounds;
			}
		}

		// Token: 0x1700003D RID: 61
		// (get) Token: 0x06000129 RID: 297 RVA: 0x000072CC File Offset: 0x000054CC
		public override CDGRect AbsBounds
		{
			get
			{
				return new CDGRect(this.X - (float)StaticTexture.MarkerTexture.Width / 2f, this.Y - (float)StaticTexture.MarkerTexture.Height, (float)StaticTexture.MarkerTexture.Width, (float)StaticTexture.MarkerTexture.Height);
			}
		}

		// Token: 0x1700003E RID: 62
		// (get) Token: 0x0600012A RID: 298 RVA: 0x00007324 File Offset: 0x00005524
		public override Hitbox[] HitboxesArray
		{
			get
			{
				return null;
			}
		}

		// Token: 0x1700003F RID: 63
		// (get) Token: 0x0600012B RID: 299 RVA: 0x00007338 File Offset: 0x00005538
		public override int HitboxesCount
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x0600012C RID: 300 RVA: 0x0000734B File Offset: 0x0000554B
		public MarkerObj()
		{
			this.Name = "New Marker";
		}

		// Token: 0x0600012D RID: 301 RVA: 0x00007362 File Offset: 0x00005562
		public override void CollisionResponse(GameObj otherObj, Vector2 mtd, Hitbox thisBox, Hitbox otherBox)
		{
		}

		// Token: 0x0600012E RID: 302 RVA: 0x00007368 File Offset: 0x00005568
		public override void Draw(Camera2D camera, float elapsedSeconds)
		{
			camera.Draw(StaticTexture.MarkerTexture, this.Position, null, Color.White * base.AbsOpacity, 0f, new Vector2((float)StaticTexture.MarkerTexture.Width / 2f, (float)StaticTexture.MarkerTexture.Height), 1f, SpriteEffects.None, 0f);
			camera.DrawString(StaticTexture.GenericFont, this.ID.ToString(), this.Position, Color.Black * base.AbsOpacity, 0f, new Vector2(5.5f, 33f), 2.5f, SpriteEffects.None, 1f);
		}

		// Token: 0x0600012F RID: 303 RVA: 0x00007424 File Offset: 0x00005624
		public override GameObj CreateClone()
		{
			MarkerObj markerObj = new MarkerObj();
			this.PopulateClone(markerObj);
			return markerObj;
		}

		// Token: 0x06000130 RID: 304 RVA: 0x00007448 File Offset: 0x00005648
		public override void PopulateClone(object obj)
		{
			MarkerObj markerObj = obj as MarkerObj;
			markerObj.ID = this.ID;
			base.PopulateClone(obj);
		}

		// Token: 0x0400008B RID: 139
		private int m_id;
	}
}

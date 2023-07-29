using System;
using CDGEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpriteSystem2;

namespace BrawlerEditor
{
	// Token: 0x02000024 RID: 36
	public class PlayerStartObj : GameObj
	{
		// Token: 0x17000050 RID: 80
		// (get) Token: 0x06000153 RID: 339 RVA: 0x00007C9C File Offset: 0x00005E9C
		// (set) Token: 0x06000154 RID: 340 RVA: 0x00007CB4 File Offset: 0x00005EB4
		public bool isDebugOnly
		{
			get
			{
				return this.m_isDebugOnly;
			}
			set
			{
				this.m_isDebugOnly = value;
			}
		}

		// Token: 0x17000051 RID: 81
		// (get) Token: 0x06000155 RID: 341 RVA: 0x00007CC0 File Offset: 0x00005EC0
		public override CDGRect Bounds
		{
			get
			{
				return this.AbsBounds;
			}
		}

		// Token: 0x17000052 RID: 82
		// (get) Token: 0x06000156 RID: 342 RVA: 0x00007CD8 File Offset: 0x00005ED8
		public override CDGRect AbsBounds
		{
			get
			{
				return new CDGRect(this.X - (float)StaticTexture.PlayerSpawnTexture.Width / 2f, this.Y - (float)StaticTexture.PlayerSpawnTexture.Height, (float)StaticTexture.PlayerSpawnTexture.Width, (float)StaticTexture.PlayerSpawnTexture.Height);
			}
		}

		// Token: 0x17000053 RID: 83
		// (get) Token: 0x06000157 RID: 343 RVA: 0x00007D30 File Offset: 0x00005F30
		public override Hitbox[] HitboxesArray
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17000054 RID: 84
		// (get) Token: 0x06000158 RID: 344 RVA: 0x00007D44 File Offset: 0x00005F44
		public override int HitboxesCount
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x06000159 RID: 345 RVA: 0x00007D57 File Offset: 0x00005F57
		public PlayerStartObj()
		{
			this.Name = "New Player Start";
			this.Scale = new Vector2(4f, 4f);
		}

		// Token: 0x0600015A RID: 346 RVA: 0x00007D84 File Offset: 0x00005F84
		public override void CollisionResponse(GameObj otherObj, Vector2 mtd, Hitbox thisBox, Hitbox otherBox)
		{
		}

		// Token: 0x0600015B RID: 347 RVA: 0x00007D88 File Offset: 0x00005F88
		public override void Draw(Camera2D camera, float elapsedSeconds)
		{
			if (this.isDebugOnly)
			{
				camera.Draw(StaticTexture.PlayerSpawnTexture, this.Position, null, Color.Blue * base.AbsOpacity, 0f, new Vector2((float)StaticTexture.PlayerSpawnTexture.Width / 2f, (float)StaticTexture.PlayerSpawnTexture.Height), this.ScaleX, SpriteEffects.None, 0f);
			}
			else
			{
				camera.Draw(StaticTexture.PlayerSpawnTexture, this.Position, null, Color.Red * base.AbsOpacity, 0f, new Vector2((float)StaticTexture.PlayerSpawnTexture.Width / 2f, (float)StaticTexture.PlayerSpawnTexture.Height), this.ScaleX, SpriteEffects.None, 0f);
			}
		}

		// Token: 0x0600015C RID: 348 RVA: 0x00007E60 File Offset: 0x00006060
		public override GameObj CreateClone()
		{
			PlayerStartObj playerStartObj = new PlayerStartObj();
			this.PopulateClone(playerStartObj);
			return playerStartObj;
		}

		// Token: 0x04000099 RID: 153
		private bool m_isDebugOnly;
	}
}

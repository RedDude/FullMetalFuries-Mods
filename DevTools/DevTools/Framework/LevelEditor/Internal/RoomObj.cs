using System;
using CDGEngine;
using Microsoft.Xna.Framework;
using SpriteSystem2;

namespace BrawlerEditor
{
	// Token: 0x02000023 RID: 35
	public class RoomObj : GameObj, IScaleableObj, ICodeObj
	{
		// Token: 0x17000040 RID: 64
		// (get) Token: 0x06000131 RID: 305 RVA: 0x00007474 File Offset: 0x00005674
		// (set) Token: 0x06000132 RID: 306 RVA: 0x0000748B File Offset: 0x0000568B
		public float maxCameraZoomOut { get; set; }

		// Token: 0x17000041 RID: 65
		// (get) Token: 0x06000133 RID: 307 RVA: 0x00007494 File Offset: 0x00005694
		// (set) Token: 0x06000134 RID: 308 RVA: 0x000074AC File Offset: 0x000056AC
		public bool hookProjectilesToNavMesh
		{
			get
			{
				return this.m_hookProjectilesToNavMesh;
			}
			set
			{
				this.m_hookProjectilesToNavMesh = value;
			}
		}

		// Token: 0x17000042 RID: 66
		// (get) Token: 0x06000135 RID: 309 RVA: 0x000074B8 File Offset: 0x000056B8
		// (set) Token: 0x06000136 RID: 310 RVA: 0x000074D0 File Offset: 0x000056D0
		public bool forceGreen
		{
			get
			{
				return this.m_forceGreen;
			}
			set
			{
				this.m_forceGreen = value;
			}
		}

		// Token: 0x17000043 RID: 67
		// (get) Token: 0x06000137 RID: 311 RVA: 0x000074DC File Offset: 0x000056DC
		// (set) Token: 0x06000138 RID: 312 RVA: 0x000074F4 File Offset: 0x000056F4
		public bool forceBlack
		{
			get
			{
				return this.m_forceBlack;
			}
			set
			{
				this.m_forceBlack = value;
			}
		}

		// Token: 0x17000044 RID: 68
		// (get) Token: 0x06000139 RID: 313 RVA: 0x00007500 File Offset: 0x00005700
		// (set) Token: 0x0600013A RID: 314 RVA: 0x00007518 File Offset: 0x00005718
		public bool forceRed
		{
			get
			{
				return this.m_forceRed;
			}
			set
			{
				this.m_forceRed = value;
			}
		}

		// Token: 0x17000045 RID: 69
		// (get) Token: 0x0600013B RID: 315 RVA: 0x00007524 File Offset: 0x00005724
		// (set) Token: 0x0600013C RID: 316 RVA: 0x0000753B File Offset: 0x0000573B
		public string code { get; set; }

		// Token: 0x17000046 RID: 70
		// (get) Token: 0x0600013D RID: 317 RVA: 0x00007544 File Offset: 0x00005744
		// (set) Token: 0x0600013E RID: 318 RVA: 0x0000755C File Offset: 0x0000575C
		public bool IsArenaZone
		{
			get
			{
				return this.m_isArenaZone;
			}
			set
			{
				this.m_isArenaZone = value;
			}
		}

		// Token: 0x17000047 RID: 71
		// (get) Token: 0x0600013F RID: 319 RVA: 0x00007568 File Offset: 0x00005768
		// (set) Token: 0x06000140 RID: 320 RVA: 0x00007591 File Offset: 0x00005791
		public bool SelectAllObjs
		{
			get
			{
				return !this.IsArenaZone && this.m_selectAllObjs;
			}
			set
			{
				this.m_selectAllObjs = value;
			}
		}

		// Token: 0x17000048 RID: 72
		// (get) Token: 0x06000141 RID: 321 RVA: 0x0000759C File Offset: 0x0000579C
		// (set) Token: 0x06000142 RID: 322 RVA: 0x000075B4 File Offset: 0x000057B4
		public float Width
		{
			get
			{
				return this.m_width;
			}
			set
			{
				this.m_width = value;
			}
		}

		// Token: 0x17000049 RID: 73
		// (get) Token: 0x06000143 RID: 323 RVA: 0x000075C0 File Offset: 0x000057C0
		// (set) Token: 0x06000144 RID: 324 RVA: 0x000075D8 File Offset: 0x000057D8
		public float Height
		{
			get
			{
				return this.m_height;
			}
			set
			{
				this.m_height = value;
			}
		}

		// Token: 0x1700004A RID: 74
		// (get) Token: 0x06000145 RID: 325 RVA: 0x000075E4 File Offset: 0x000057E4
		public override CDGRect AbsBounds
		{
			get
			{
				return new CDGRect(this.X, this.Y, this.Width, this.Height);
			}
		}

		// Token: 0x1700004B RID: 75
		// (get) Token: 0x06000146 RID: 326 RVA: 0x00007614 File Offset: 0x00005814
		public override CDGRect Bounds
		{
			get
			{
				return this.AbsBounds;
			}
		}

		// Token: 0x1700004C RID: 76
		// (get) Token: 0x06000147 RID: 327 RVA: 0x0000762C File Offset: 0x0000582C
		public override Hitbox[] HitboxesArray
		{
			get
			{
				return null;
			}
		}

		// Token: 0x1700004D RID: 77
		// (get) Token: 0x06000148 RID: 328 RVA: 0x00007640 File Offset: 0x00005840
		public override int HitboxesCount
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x1700004E RID: 78
		// (get) Token: 0x06000149 RID: 329 RVA: 0x00007654 File Offset: 0x00005854
		// (set) Token: 0x0600014A RID: 330 RVA: 0x0000766C File Offset: 0x0000586C
		public float innerZonePercent
		{
			get
			{
				return this.m_innerZonePercent;
			}
			set
			{
				this.m_innerZonePercent = value;
			}
		}

		// Token: 0x1700004F RID: 79
		// (get) Token: 0x0600014B RID: 331 RVA: 0x00007678 File Offset: 0x00005878
		// (set) Token: 0x0600014C RID: 332 RVA: 0x00007690 File Offset: 0x00005890
		public byte innerZonePos
		{
			get
			{
				return this.m_innerZonePos;
			}
			set
			{
				this.m_innerZonePos = value;
			}
		}

		// Token: 0x0600014D RID: 333 RVA: 0x0000769C File Offset: 0x0000589C
		public RoomObj(float x, float y, float width, float height)
		{
			this.X = x;
			this.Y = y;
			this.Width = width;
			this.Height = height;
			this.Opacity = EditorEV.CURRENT_LAYER_OPACITY;
			this.Name = "New Room";
			this.SelectAllObjs = false;
			this.innerZonePos = 0;
			this.innerZonePercent = 0.5f;
			this.maxCameraZoomOut = 1f;
		}

		// Token: 0x0600014E RID: 334 RVA: 0x00007713 File Offset: 0x00005913
		public override void CollisionResponse(GameObj otherObj, Vector2 mtd, Hitbox thisBox, Hitbox otherBox)
		{
			throw new NotImplementedException();
		}

		// Token: 0x0600014F RID: 335 RVA: 0x0000771C File Offset: 0x0000591C
		public override void Draw(Camera2D camera, float elapsedSeconds)
		{
			Color value = EditorEV.ARENAOBJ_COLOUR;
			if (!this.IsArenaZone)
			{
				value = EditorEV.ROOMOBJ_COLOUR;
			}
			if (this.forceRed)
			{
				value = Color.Red;
			}
			if (this.forceBlack)
			{
				value = Color.Black;
			}
			if (this.forceGreen)
			{
				value = Color.Green;
			}
			this.DrawHollowHull(camera, new Rectangle((int)this.X, (int)this.Y, (int)this.Width, (int)this.Height), value * this.Opacity);
			Vector2 zero = Vector2.Zero;
			CDGRect cdgrect = new CDGRect(0f, 0f, 960f * this.innerZonePercent, 540f * this.innerZonePercent);
			switch (this.innerZonePos)
			{
			case 0:
				cdgrect.X = this.Bounds.Center.X - cdgrect.Width / 2f;
				cdgrect.Y = this.Bounds.Center.Y - cdgrect.Height / 2f;
				break;
			case 1:
				cdgrect.X = this.X;
				cdgrect.Y = this.Bounds.Center.Y - cdgrect.Height / 2f;
				break;
			case 2:
				cdgrect.X = this.Bounds.Right - cdgrect.Width;
				cdgrect.Y = this.Bounds.Center.Y - cdgrect.Height / 2f;
				break;
			case 3:
				cdgrect.X = this.Bounds.Center.X - cdgrect.Width / 2f;
				cdgrect.Y = this.Y;
				break;
			case 4:
				cdgrect.X = this.Bounds.Center.X - cdgrect.Width / 2f;
				cdgrect.Y = this.Bounds.Bottom - cdgrect.Height;
				break;
			}
			this.DrawHollowHull(camera, cdgrect.ToRectangle(), Color.DarkBlue * this.Opacity);
			int num = 2;
			int num2 = num;
			num = (int)((float)num / camera.Zoom);
			if (num < num2)
			{
				num = num2;
			}
			camera.Draw(StaticTexture.GenericTexture, new Rectangle((int)this.X, (int)this.Y, (int)this.Width, num), Color.White * this.Opacity);
			camera.Draw(StaticTexture.GenericTexture, new Rectangle((int)this.X, (int)this.Y, num, (int)this.Height), Color.White * this.Opacity);
			camera.Draw(StaticTexture.GenericTexture, new Rectangle((int)this.X + (int)this.Width, (int)this.Y, num, (int)this.Height), Color.White * this.Opacity);
			camera.Draw(StaticTexture.GenericTexture, new Rectangle((int)this.X, (int)this.Y + (int)this.Height, (int)this.Width + num, num), Color.White * this.Opacity);
			if (this.IsArenaZone && this.Tag != "" && this.Tag != null)
			{
				camera.DrawString(StaticTexture.GenericFont, this.Tag, new Vector2(this.AbsBounds.Left, this.AbsBounds.Top), Color.White);
			}
		}

		// Token: 0x06000150 RID: 336 RVA: 0x00007B04 File Offset: 0x00005D04
		private void DrawHollowHull(Camera2D camera, Rectangle rect, Color color)
		{
			int num = 15;
			camera.Draw(StaticTexture.GenericTexture, new Rectangle(rect.X, rect.Y, rect.Width, num), color);
			camera.Draw(StaticTexture.GenericTexture, new Rectangle(rect.X, rect.Y + num, num, rect.Height - num), color);
			camera.Draw(StaticTexture.GenericTexture, new Rectangle(rect.X + rect.Width - num, rect.Y + num, num, rect.Height - num), color);
			camera.Draw(StaticTexture.GenericTexture, new Rectangle(rect.X + num, rect.Y + rect.Height - num, rect.Width - num * 2, num), color);
		}

		// Token: 0x06000151 RID: 337 RVA: 0x00007BD8 File Offset: 0x00005DD8
		public override GameObj CreateClone()
		{
			RoomObj roomObj = new RoomObj(this.X, this.Y, this.Width, this.Height);
			this.PopulateClone(roomObj);
			return roomObj;
		}

		// Token: 0x06000152 RID: 338 RVA: 0x00007C14 File Offset: 0x00005E14
		public override void PopulateClone(object obj)
		{
			RoomObj roomObj = obj as RoomObj;
			roomObj.innerZonePercent = this.innerZonePercent;
			roomObj.innerZonePos = this.innerZonePos;
			roomObj.IsArenaZone = this.IsArenaZone;
			roomObj.forceRed = this.forceRed;
			roomObj.forceBlack = this.forceBlack;
			roomObj.forceGreen = this.forceGreen;
			roomObj.hookProjectilesToNavMesh = this.hookProjectilesToNavMesh;
			roomObj.maxCameraZoomOut = this.maxCameraZoomOut;
			base.PopulateClone(obj);
		}

		// Token: 0x0400008D RID: 141
		private float m_width;

		// Token: 0x0400008E RID: 142
		private float m_height;

		// Token: 0x0400008F RID: 143
		private bool m_selectAllObjs;

		// Token: 0x04000090 RID: 144
		private float m_innerZonePercent;

		// Token: 0x04000091 RID: 145
		private byte m_innerZonePos;

		// Token: 0x04000092 RID: 146
		private bool m_isArenaZone;

		// Token: 0x04000093 RID: 147
		private bool m_forceRed;

		// Token: 0x04000094 RID: 148
		private bool m_forceBlack;

		// Token: 0x04000095 RID: 149
		private bool m_forceGreen;

		// Token: 0x04000096 RID: 150
		private bool m_hookProjectilesToNavMesh;
	}
}

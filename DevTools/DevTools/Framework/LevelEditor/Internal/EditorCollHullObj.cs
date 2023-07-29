using System;
using CDGEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpriteSystem2;

namespace BrawlerEditor
{
	// Token: 0x0200003A RID: 58
	public class EditorCollHullObj : GameObj, IPropertiesObj, IScaleableObj, ICodeObj, IJumpOverObj
	{
		// Token: 0x17000065 RID: 101
		// (get) Token: 0x06000201 RID: 513 RVA: 0x000101A0 File Offset: 0x0000E3A0
		// (set) Token: 0x06000202 RID: 514 RVA: 0x000101B8 File Offset: 0x0000E3B8
		public bool hidesEngineerUI
		{
			get
			{
				return this.m_hidesEngineerUI;
			}
			set
			{
				this.m_hidesEngineerUI = value;
			}
		}

		// Token: 0x17000066 RID: 102
		// (get) Token: 0x06000203 RID: 515 RVA: 0x000101C4 File Offset: 0x0000E3C4
		// (set) Token: 0x06000204 RID: 516 RVA: 0x000101DC File Offset: 0x0000E3DC
		public bool onlyCollidesWithEnemies
		{
			get
			{
				return this.m_onlyCollidesWithEnemies;
			}
			set
			{
				this.m_onlyCollidesWithEnemies = value;
			}
		}

		// Token: 0x17000067 RID: 103
		// (get) Token: 0x06000205 RID: 517 RVA: 0x000101E8 File Offset: 0x0000E3E8
		// (set) Token: 0x06000206 RID: 518 RVA: 0x00010200 File Offset: 0x0000E400
		public bool onlyCollidesWithPlayers
		{
			get
			{
				return this.m_onlyCollidesWithPlayers;
			}
			set
			{
				this.m_onlyCollidesWithPlayers = value;
			}
		}

		// Token: 0x17000068 RID: 104
		// (get) Token: 0x06000207 RID: 519 RVA: 0x0001020C File Offset: 0x0000E40C
		// (set) Token: 0x06000208 RID: 520 RVA: 0x00010224 File Offset: 0x0000E424
		public override string Tag
		{
			get
			{
				return base.Tag;
			}
			set
			{
				this.m_isGoCatTrigger = false;
				if (value.Contains("GoCat"))
				{
					this.m_isGoCatTrigger = true;
				}
				base.Tag = value;
			}
		}

		// Token: 0x17000069 RID: 105
		// (get) Token: 0x06000209 RID: 521 RVA: 0x0001025C File Offset: 0x0000E45C
		// (set) Token: 0x0600020A RID: 522 RVA: 0x00010274 File Offset: 0x0000E474
		public bool playerCanShootThrough
		{
			get
			{
				return this.m_playerCanShootThrough;
			}
			set
			{
				this.m_playerCanShootThrough = value;
			}
		}

		// Token: 0x1700006A RID: 106
		// (get) Token: 0x0600020B RID: 523 RVA: 0x00010280 File Offset: 0x0000E480
		// (set) Token: 0x0600020C RID: 524 RVA: 0x00010298 File Offset: 0x0000E498
		public bool canShootThrough
		{
			get
			{
				return this.m_canShootThrough;
			}
			set
			{
				this.m_canShootThrough = value;
			}
		}

		// Token: 0x1700006B RID: 107
		// (get) Token: 0x0600020D RID: 525 RVA: 0x000102A4 File Offset: 0x0000E4A4
		// (set) Token: 0x0600020E RID: 526 RVA: 0x000102BC File Offset: 0x0000E4BC
		public bool jumpable
		{
			get
			{
				return this.m_jumpable;
			}
			set
			{
				this.m_jumpable = value;
			}
		}

		// Token: 0x1700006C RID: 108
		// (get) Token: 0x0600020F RID: 527 RVA: 0x000102C8 File Offset: 0x0000E4C8
		// (set) Token: 0x06000210 RID: 528 RVA: 0x000102E0 File Offset: 0x0000E4E0
		public bool isArenaTrigger
		{
			get
			{
				return this.m_isArenaTrigger;
			}
			set
			{
				this.m_isArenaTrigger = value;
			}
		}

		// Token: 0x1700006D RID: 109
		// (get) Token: 0x06000211 RID: 529 RVA: 0x000102EC File Offset: 0x0000E4EC
		// (set) Token: 0x06000212 RID: 530 RVA: 0x00010304 File Offset: 0x0000E504
		public bool isNavMesh
		{
			get
			{
				return this.m_isNavMesh;
			}
			set
			{
				this.m_isNavMesh = value;
			}
		}

		// Token: 0x1700006E RID: 110
		// (get) Token: 0x06000213 RID: 531 RVA: 0x00010310 File Offset: 0x0000E510
		// (set) Token: 0x06000214 RID: 532 RVA: 0x00010339 File Offset: 0x0000E539
		public byte transitionZoneType
		{
			get
			{
				byte result;
				if (this.isNavMesh)
				{
					result = 0;
				}
				else
				{
					result = this.m_transitionType;
				}
				return result;
			}
			set
			{
				this.m_transitionType = value;
			}
		}

		// Token: 0x1700006F RID: 111
		// (get) Token: 0x06000215 RID: 533 RVA: 0x00010344 File Offset: 0x0000E544
		// (set) Token: 0x06000216 RID: 534 RVA: 0x0001035C File Offset: 0x0000E55C
		public string code
		{
			get
			{
				return this.m_code;
			}
			set
			{
				this.m_code = value;
				this.m_codeIsLegit = false;
				if (this.m_code.Contains("WrapTexture"))
				{
					int startIndex = this.m_code.IndexOf("WrapTexture") + 11;
					int num = this.m_code.IndexOf("(", startIndex) + 1;
					int num2 = this.m_code.IndexOf(")", startIndex);
					if (num != -1 && num2 != -1)
					{
						string spriteName = this.m_code.Substring(num, num2 - num).Trim();
						if (SpriteLibrary.HasSprite(spriteName))
						{
							this.m_wrapSpriteObj.ChangeSprite(spriteName);
							this.m_codeIsLegit = true;
						}
					}
				}
			}
		}

		// Token: 0x17000070 RID: 112
		// (get) Token: 0x06000217 RID: 535 RVA: 0x00010420 File Offset: 0x0000E620
		// (set) Token: 0x06000218 RID: 536 RVA: 0x00010438 File Offset: 0x0000E638
		public override Vector2 Scale
		{
			get
			{
				return base.Scale;
			}
			set
			{
				base.Scale = value;
				this.m_wrapSpriteObj.Scale = this.Scale;
			}
		}

		// Token: 0x17000071 RID: 113
		// (get) Token: 0x06000219 RID: 537 RVA: 0x00010458 File Offset: 0x0000E658
		// (set) Token: 0x0600021A RID: 538 RVA: 0x00010470 File Offset: 0x0000E670
		public override float ScaleX
		{
			get
			{
				return base.ScaleX;
			}
			set
			{
				base.ScaleX = value;
				this.m_wrapSpriteObj.ScaleX = this.ScaleX;
			}
		}

		// Token: 0x17000072 RID: 114
		// (get) Token: 0x0600021B RID: 539 RVA: 0x00010490 File Offset: 0x0000E690
		// (set) Token: 0x0600021C RID: 540 RVA: 0x000104A8 File Offset: 0x0000E6A8
		public override float ScaleY
		{
			get
			{
				return base.ScaleY;
			}
			set
			{
				base.ScaleY = value;
				this.m_wrapSpriteObj.ScaleY = this.ScaleY;
			}
		}

		// Token: 0x17000073 RID: 115
		// (get) Token: 0x0600021D RID: 541 RVA: 0x000104C8 File Offset: 0x0000E6C8
		// (set) Token: 0x0600021E RID: 542 RVA: 0x000104E0 File Offset: 0x0000E6E0
		public float Width
		{
			get
			{
				return this.m_width;
			}
			set
			{
				this.m_width = value;
				this.m_wrapSpriteObj.wrapWidth = this.m_width;
			}
		}

		// Token: 0x17000074 RID: 116
		// (get) Token: 0x0600021F RID: 543 RVA: 0x000104FC File Offset: 0x0000E6FC
		// (set) Token: 0x06000220 RID: 544 RVA: 0x00010514 File Offset: 0x0000E714
		public float Height
		{
			get
			{
				return this.m_height;
			}
			set
			{
				this.m_height = value;
				this.m_wrapSpriteObj.wrapHeight = this.m_height;
			}
		}

		// Token: 0x17000075 RID: 117
		// (get) Token: 0x06000221 RID: 545 RVA: 0x00010530 File Offset: 0x0000E730
		// (set) Token: 0x06000222 RID: 546 RVA: 0x0001056A File Offset: 0x0000E76A
		public byte triggerType
		{
			get
			{
				byte result;
				if (this.isNavMesh)
				{
					result = 0;
				}
				else if (this.transitionZoneType != 0)
				{
					result = 0;
				}
				else
				{
					result = this.m_triggerType;
				}
				return result;
			}
			set
			{
				this.m_triggerType = value;
			}
		}

		// Token: 0x17000076 RID: 118
		// (get) Token: 0x06000223 RID: 547 RVA: 0x00010574 File Offset: 0x0000E774
		public override CDGRect AbsBounds
		{
			get
			{
				return new CDGRect(this.X, this.Y, this.Width, this.Height);
			}
		}

		// Token: 0x17000077 RID: 119
		// (get) Token: 0x06000224 RID: 548 RVA: 0x000105A4 File Offset: 0x0000E7A4
		public override CDGRect Bounds
		{
			get
			{
				return this.AbsBounds;
			}
		}

		// Token: 0x17000078 RID: 120
		// (get) Token: 0x06000225 RID: 549 RVA: 0x000105BC File Offset: 0x0000E7BC
		public override Hitbox[] HitboxesArray
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17000079 RID: 121
		// (get) Token: 0x06000226 RID: 550 RVA: 0x000105D0 File Offset: 0x0000E7D0
		public override int HitboxesCount
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x06000227 RID: 551 RVA: 0x000105E4 File Offset: 0x0000E7E4
		public EditorCollHullObj(float x, float y, float width, float height)
		{
			this.m_wrapSpriteObj = new WrapSpriteObj("");
			this.X = x;
			this.Y = y;
			this.Width = width;
			this.Height = height;
			this.Opacity = EditorEV.CURRENT_LAYER_OPACITY;
			this.Name = "New Collision Hull";
		}

		// Token: 0x06000228 RID: 552 RVA: 0x00010651 File Offset: 0x0000E851
		public override void CollisionResponse(GameObj otherObj, Vector2 mtd, Hitbox thisBox, Hitbox otherBox)
		{
		}

		// Token: 0x06000229 RID: 553 RVA: 0x00010654 File Offset: 0x0000E854
		public override void Draw(Camera2D camera, float elapsedSeconds)
		{
			Color value = EditorEV.COLLHULL_COLOUR;
			if (this.onlyCollidesWithPlayers)
			{
				value = Color.OrangeRed;
			}
			else if (this.onlyCollidesWithEnemies)
			{
				value = new Color(250, 150, 120);
			}
			else if (this.jumpable && (this.canShootThrough || this.playerCanShootThrough))
			{
				value = EditorEV.COLLHULL_SHOOTTHROUGH_AND_JUMP;
			}
			else if (this.jumpable)
			{
				value = EditorEV.COLLHULL_JUMPABLE_COLOUR;
			}
			else if (this.canShootThrough || this.playerCanShootThrough)
			{
				value = EditorEV.COLLHULL_SHOOTTHROUGH;
			}
			if (this.triggerType != 0)
			{
				if (this.m_isGoCatTrigger)
				{
					value = Color.Orange;
				}
				else
				{
					value = EditorEV.TRIGGERHULL_COLOUR;
				}
			}
			if (this.transitionZoneType != 0)
			{
				if (this.transitionZoneType % 2 != 0)
				{
					value = EditorEV.TRANSITIONZONE_ARRIVE_COLOUR;
				}
				else
				{
					value = EditorEV.TRANSITIONZONE_LEAVE_COLOUR;
				}
			}
			if (this.isNavMesh)
			{
				value = EditorEV.NAVMESH_COLOUR;
			}
			if (this.triggerType == 0 && this.m_codeIsLegit)
			{
				this.m_wrapSpriteObj.Position = this.Position;
				this.m_wrapSpriteObj.Opacity = this.Opacity;
				this.m_wrapSpriteObj.Rotation = this.Rotation;
				this.m_wrapSpriteObj.Draw(camera, elapsedSeconds);
			}
			else
			{
				float rotation = MathHelper.ToRadians(this.Rotation);
				camera.Draw(StaticTexture.GenericTexture, new Rectangle((int)this.X, (int)this.Y, (int)this.Width, (int)this.Height), null, value * this.Opacity, rotation, Vector2.Zero, SpriteEffects.None, 1f);
			}
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
			if (this.triggerType == 2)
			{
				camera.Draw(StaticTexture.ArrowTexture, new Vector2(this.X, this.Y + this.Height / 2f), null, Color.White * this.Opacity, MathHelper.ToRadians(180f), new Vector2((float)StaticTexture.ArrowTexture.Width / 2f, (float)StaticTexture.ArrowTexture.Height / 2f), 1f, SpriteEffects.None, 0f);
			}
			else if (this.triggerType == 3)
			{
				camera.Draw(StaticTexture.ArrowTexture, new Vector2(this.X + this.Width, this.Y + this.Height / 2f), null, Color.White * this.Opacity, MathHelper.ToRadians(0f), new Vector2((float)StaticTexture.ArrowTexture.Width / 2f, (float)StaticTexture.ArrowTexture.Height / 2f), 1f, SpriteEffects.None, 0f);
			}
			else if (this.triggerType == 4)
			{
				camera.Draw(StaticTexture.ArrowTexture, new Vector2(this.X + this.Width / 2f, this.Y), null, Color.White * this.Opacity, MathHelper.ToRadians(-90f), new Vector2((float)StaticTexture.ArrowTexture.Width / 2f, (float)StaticTexture.ArrowTexture.Height / 2f), 1f, SpriteEffects.None, 0f);
			}
			else if (this.triggerType == 5)
			{
				camera.Draw(StaticTexture.ArrowTexture, new Vector2(this.X + this.Width / 2f, this.Y + this.Height), null, Color.White * this.Opacity, MathHelper.ToRadians(90f), new Vector2((float)StaticTexture.ArrowTexture.Width / 2f, (float)StaticTexture.ArrowTexture.Height / 2f), 1f, SpriteEffects.None, 0f);
			}
			if (this.transitionZoneType == 5 || this.transitionZoneType == 6)
			{
				camera.Draw(StaticTexture.ArrowTexture, new Vector2(this.X + this.Width / 2f, this.Y + this.Height / 2f), null, Color.White * this.Opacity, MathHelper.ToRadians(0f), new Vector2((float)StaticTexture.ArrowTexture.Width / 2f, (float)StaticTexture.ArrowTexture.Height / 2f), 1f, SpriteEffects.None, 1f);
			}
			else if (this.transitionZoneType == 1 || this.transitionZoneType == 2)
			{
				camera.Draw(StaticTexture.ArrowTexture, new Vector2(this.X + this.Width / 2f, this.Y + this.Height / 2f), null, Color.White * this.Opacity, MathHelper.ToRadians(180f), new Vector2((float)StaticTexture.ArrowTexture.Width / 2f, (float)StaticTexture.ArrowTexture.Height / 2f), 1f, SpriteEffects.None, 0f);
			}
			else if (this.transitionZoneType == 3 || this.transitionZoneType == 4)
			{
				camera.Draw(StaticTexture.ArrowTexture, new Vector2(this.X + this.Width / 2f, this.Y + this.Height / 2f), null, Color.White * this.Opacity, MathHelper.ToRadians(-90f), new Vector2((float)StaticTexture.ArrowTexture.Width / 2f, (float)StaticTexture.ArrowTexture.Height / 2f), 1f, SpriteEffects.None, 0f);
			}
			else if (this.transitionZoneType == 7 || this.transitionZoneType == 8)
			{
				camera.Draw(StaticTexture.ArrowTexture, new Vector2(this.X + this.Width / 2f, this.Y + this.Height / 2f), null, Color.White * this.Opacity, MathHelper.ToRadians(90f), new Vector2((float)StaticTexture.ArrowTexture.Width / 2f, (float)StaticTexture.ArrowTexture.Height / 2f), 1f, SpriteEffects.None, 0f);
			}
			if (this.code != "" && this.code != null)
			{
				camera.DrawString(StaticTexture.GenericFont, "<CODE>", new Vector2(this.AbsBounds.Right - 85f, this.AbsBounds.Top), Color.White);
			}
		}

		// Token: 0x0600022A RID: 554 RVA: 0x00010EF0 File Offset: 0x0000F0F0
		private void DrawHollowHull(Camera2D camera, Rectangle rect, Color color)
		{
			int num = 15;
			camera.Draw(StaticTexture.GenericTexture, new Rectangle(rect.X, rect.Y, rect.Width, num), color);
			camera.Draw(StaticTexture.GenericTexture, new Rectangle(rect.X, rect.Y + num, num, rect.Height - num), color);
			camera.Draw(StaticTexture.GenericTexture, new Rectangle(rect.X + rect.Width - num, rect.Y + num, num, rect.Height - num), color);
			camera.Draw(StaticTexture.GenericTexture, new Rectangle(rect.X + num, rect.Y + rect.Height - num, rect.Width - num * 2, num), color);
		}

		// Token: 0x0600022B RID: 555 RVA: 0x00010FC4 File Offset: 0x0000F1C4
		public override GameObj CreateClone()
		{
			EditorCollHullObj editorCollHullObj = new EditorCollHullObj(this.X, this.Y, this.Width, this.Height);
			this.PopulateClone(editorCollHullObj);
			return editorCollHullObj;
		}

		// Token: 0x0600022C RID: 556 RVA: 0x00011000 File Offset: 0x0000F200
		public override void PopulateClone(object obj)
		{
			EditorCollHullObj editorCollHullObj = obj as EditorCollHullObj;
			editorCollHullObj.code = this.code;
			editorCollHullObj.triggerType = this.triggerType;
			editorCollHullObj.transitionZoneType = this.transitionZoneType;
			editorCollHullObj.jumpable = this.jumpable;
			editorCollHullObj.isNavMesh = this.isNavMesh;
			editorCollHullObj.isArenaTrigger = this.isArenaTrigger;
			editorCollHullObj.canShootThrough = this.canShootThrough;
			editorCollHullObj.playerCanShootThrough = this.playerCanShootThrough;
			editorCollHullObj.onlyCollidesWithPlayers = this.onlyCollidesWithPlayers;
			editorCollHullObj.onlyCollidesWithEnemies = this.onlyCollidesWithEnemies;
			editorCollHullObj.hidesEngineerUI = this.hidesEngineerUI;
			base.PopulateClone(obj);
		}

		// Token: 0x040000F5 RID: 245
		private float m_width;

		// Token: 0x040000F6 RID: 246
		private float m_height;

		// Token: 0x040000F7 RID: 247
		private byte m_triggerType = 0;

		// Token: 0x040000F8 RID: 248
		private WrapSpriteObj m_wrapSpriteObj;

		// Token: 0x040000F9 RID: 249
		private string m_code;

		// Token: 0x040000FA RID: 250
		private bool m_codeIsLegit;

		// Token: 0x040000FB RID: 251
		private bool m_isNavMesh;

		// Token: 0x040000FC RID: 252
		private bool m_isArenaTrigger;

		// Token: 0x040000FD RID: 253
		private byte m_transitionType = 0;

		// Token: 0x040000FE RID: 254
		private bool m_jumpable;

		// Token: 0x040000FF RID: 255
		private bool m_canShootThrough;

		// Token: 0x04000100 RID: 256
		private bool m_playerCanShootThrough;

		// Token: 0x04000101 RID: 257
		private bool m_isGoCatTrigger;

		// Token: 0x04000102 RID: 258
		private bool m_onlyCollidesWithPlayers;

		// Token: 0x04000103 RID: 259
		private bool m_onlyCollidesWithEnemies;

		// Token: 0x04000104 RID: 260
		private bool m_hidesEngineerUI;
	}
}

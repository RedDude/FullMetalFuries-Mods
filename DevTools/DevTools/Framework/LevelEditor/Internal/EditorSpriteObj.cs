using System;
using CDGEngine;
using Microsoft.Xna.Framework;
using SpriteSystem2;

namespace BrawlerEditor
{
	// Token: 0x02000018 RID: 24
	public class EditorSpriteObj : SpriteObj, IJumpOverObj, IOpacityObj, IAddPhysicsObj, IEditorAnchorObj, IEditorDrawableObj
	{
		// Token: 0x17000023 RID: 35
		// (get) Token: 0x060000D1 RID: 209 RVA: 0x00006214 File Offset: 0x00004414
		// (set) Token: 0x060000D2 RID: 210 RVA: 0x0000622C File Offset: 0x0000442C
		public bool forceAddToStage
		{
			get
			{
				return this.m_forceAddToStage;
			}
			set
			{
				this.m_forceAddToStage = value;
			}
		}

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x060000D3 RID: 211 RVA: 0x00006238 File Offset: 0x00004438
		// (set) Token: 0x060000D4 RID: 212 RVA: 0x00006250 File Offset: 0x00004450
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

		// Token: 0x17000025 RID: 37
		// (get) Token: 0x060000D5 RID: 213 RVA: 0x0000625C File Offset: 0x0000445C
		// (set) Token: 0x060000D6 RID: 214 RVA: 0x00006279 File Offset: 0x00004479
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

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x060000D7 RID: 215 RVA: 0x00006288 File Offset: 0x00004488
		// (set) Token: 0x060000D8 RID: 216 RVA: 0x000062A5 File Offset: 0x000044A5
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

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x060000D9 RID: 217 RVA: 0x000062B4 File Offset: 0x000044B4
		// (set) Token: 0x060000DA RID: 218 RVA: 0x000062CC File Offset: 0x000044CC
		public bool addPhysics
		{
			get
			{
				return this.m_addPhysics;
			}
			set
			{
				this.m_addPhysics = value;
			}
		}

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x060000DB RID: 219 RVA: 0x000062D8 File Offset: 0x000044D8
		public float pureOpacity
		{
			get
			{
				return base.Opacity;
			}
		}

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x060000DC RID: 220 RVA: 0x000062F0 File Offset: 0x000044F0
		// (set) Token: 0x060000DD RID: 221 RVA: 0x0000630F File Offset: 0x0000450F
		public override float Opacity
		{
			get
			{
				return base.Opacity * this.editorOpacity;
			}
			set
			{
				base.Opacity = value;
			}
		}

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x060000DE RID: 222 RVA: 0x0000631C File Offset: 0x0000451C
		// (set) Token: 0x060000DF RID: 223 RVA: 0x00006334 File Offset: 0x00004534
		public float editorOpacity
		{
			get
			{
				return this.m_editorOpacity;
			}
			set
			{
				this.m_editorOpacity = value;
			}
		}

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x060000E0 RID: 224 RVA: 0x00006340 File Offset: 0x00004540
		// (set) Token: 0x060000E1 RID: 225 RVA: 0x00006358 File Offset: 0x00004558
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

		// Token: 0x060000E2 RID: 226 RVA: 0x00006362 File Offset: 0x00004562
		public EditorSpriteObj(string spriteName)
			: base(spriteName)
		{
			this.jumpable = true;
		}

		// Token: 0x060000E3 RID: 227 RVA: 0x00006384 File Offset: 0x00004584
		public override void ChangeSprite(string spriteName)
		{
			base.ChangeSprite(spriteName);
			SpriteData spriteData = this.m_spriteDataArray[this.CurrentFrame - 1];
			this.editorAnchorX = spriteData.EditorAnchorX;
			this.editorAnchorY = spriteData.EditorAnchorY;
		}

		// Token: 0x060000E4 RID: 228 RVA: 0x000063C4 File Offset: 0x000045C4
		public override GameObj CreateClone()
		{
			EditorSpriteObj editorSpriteObj = new EditorSpriteObj(this.spriteName);
			this.PopulateClone(editorSpriteObj);
			return editorSpriteObj;
		}

		// Token: 0x060000E5 RID: 229 RVA: 0x000063EC File Offset: 0x000045EC
		public override void PopulateClone(object obj)
		{
			EditorSpriteObj editorSpriteObj = obj as EditorSpriteObj;
			editorSpriteObj.jumpable = this.jumpable;
			editorSpriteObj.editorOpacity = this.editorOpacity;
			editorSpriteObj.addPhysics = this.addPhysics;
			editorSpriteObj.editorAnchorX = this.editorAnchorX;
			editorSpriteObj.editorAnchorY = this.editorAnchorY;
			editorSpriteObj.forceAddToStage = this.forceAddToStage;
			base.PopulateClone(obj);
		}

		// Token: 0x0400006F RID: 111
		private bool m_jumpable;

		// Token: 0x04000070 RID: 112
		private float m_editorOpacity = 1f;

		// Token: 0x04000071 RID: 113
		private bool m_addPhysics;

		// Token: 0x04000072 RID: 114
		private Vector2 m_editorAnchor;

		// Token: 0x04000073 RID: 115
		private bool m_forceAddToStage;
	}
}

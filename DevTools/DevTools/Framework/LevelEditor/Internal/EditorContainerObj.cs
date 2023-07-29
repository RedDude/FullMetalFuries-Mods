using System;
using CDGEngine;
using Microsoft.Xna.Framework;
using SpriteSystem2;

namespace BrawlerEditor
{
	// Token: 0x02000017 RID: 23
	public class EditorContainerObj : ContainerObj, IJumpOverObj, IOpacityObj, IAddPhysicsObj, IEditorAnchorObj, IEditorDrawableObj
	{
		// Token: 0x1700001A RID: 26
		// (get) Token: 0x060000BC RID: 188 RVA: 0x00005FCC File Offset: 0x000041CC
		// (set) Token: 0x060000BD RID: 189 RVA: 0x00005FE4 File Offset: 0x000041E4
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

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x060000BE RID: 190 RVA: 0x00005FF0 File Offset: 0x000041F0
		// (set) Token: 0x060000BF RID: 191 RVA: 0x00006008 File Offset: 0x00004208
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

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x060000C0 RID: 192 RVA: 0x00006014 File Offset: 0x00004214
		// (set) Token: 0x060000C1 RID: 193 RVA: 0x00006031 File Offset: 0x00004231
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

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x060000C2 RID: 194 RVA: 0x00006040 File Offset: 0x00004240
		// (set) Token: 0x060000C3 RID: 195 RVA: 0x0000605D File Offset: 0x0000425D
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

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x060000C4 RID: 196 RVA: 0x0000606C File Offset: 0x0000426C
		// (set) Token: 0x060000C5 RID: 197 RVA: 0x00006084 File Offset: 0x00004284
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

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x060000C6 RID: 198 RVA: 0x00006090 File Offset: 0x00004290
		public float pureOpacity
		{
			get
			{
				return base.Opacity;
			}
		}

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x060000C7 RID: 199 RVA: 0x000060A8 File Offset: 0x000042A8
		// (set) Token: 0x060000C8 RID: 200 RVA: 0x000060C7 File Offset: 0x000042C7
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

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x060000C9 RID: 201 RVA: 0x000060D4 File Offset: 0x000042D4
		// (set) Token: 0x060000CA RID: 202 RVA: 0x000060EC File Offset: 0x000042EC
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

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x060000CB RID: 203 RVA: 0x000060F8 File Offset: 0x000042F8
		// (set) Token: 0x060000CC RID: 204 RVA: 0x00006110 File Offset: 0x00004310
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

		// Token: 0x060000CD RID: 205 RVA: 0x0000611A File Offset: 0x0000431A
		public EditorContainerObj(string spriteName)
			: base(spriteName)
		{
			this.jumpable = true;
		}

		// Token: 0x060000CE RID: 206 RVA: 0x0000613C File Offset: 0x0000433C
		public override void ChangeSprite(string spriteName)
		{
			base.ChangeSprite(spriteName);
			ContainerData[] containerDataList = SpriteLibrary.GetContainerDataList(spriteName);
			this.editorAnchorX = containerDataList[0].EditorAnchorX;
			this.editorAnchorY = containerDataList[0].EditorAnchorY;
		}

		// Token: 0x060000CF RID: 207 RVA: 0x00006180 File Offset: 0x00004380
		public override GameObj CreateClone()
		{
			EditorContainerObj editorContainerObj = new EditorContainerObj(this.spriteName);
			this.PopulateClone(editorContainerObj);
			return editorContainerObj;
		}

		// Token: 0x060000D0 RID: 208 RVA: 0x000061A8 File Offset: 0x000043A8
		public override void PopulateClone(object obj)
		{
			EditorContainerObj editorContainerObj = obj as EditorContainerObj;
			editorContainerObj.jumpable = this.jumpable;
			editorContainerObj.editorOpacity = this.editorOpacity;
			editorContainerObj.addPhysics = this.addPhysics;
			editorContainerObj.editorAnchorX = this.editorAnchorX;
			editorContainerObj.editorAnchorY = this.editorAnchorY;
			editorContainerObj.forceAddToStage = this.forceAddToStage;
			base.PopulateClone(obj);
		}

		// Token: 0x0400006A RID: 106
		private bool m_jumpable;

		// Token: 0x0400006B RID: 107
		private float m_editorOpacity = 1f;

		// Token: 0x0400006C RID: 108
		private bool m_addPhysics;

		// Token: 0x0400006D RID: 109
		private bool m_forceAddToStage;

		// Token: 0x0400006E RID: 110
		private Vector2 m_editorAnchor;
	}
}

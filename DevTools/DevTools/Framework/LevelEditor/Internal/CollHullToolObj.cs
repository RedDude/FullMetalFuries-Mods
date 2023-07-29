using System;
using System.Windows.Input;
using CDGEngine;
using Microsoft.Xna.Framework;

namespace BrawlerEditor
{
	// Token: 0x02000038 RID: 56
	public class CollHullToolObj : ToolObj
	{
		// Token: 0x060001FC RID: 508 RVA: 0x0000FCE5 File Offset: 0x0000DEE5
		public CollHullToolObj(GameScreenControl gameScreenControl)
			: base(gameScreenControl)
		{
			this.m_toolType = 2;
		}

		// Token: 0x060001FD RID: 509 RVA: 0x0000FD04 File Offset: 0x0000DF04
		public override void LeftMouseDown(object sender, HwndMouseEventArgs e)
		{
			this.m_mouseDown = true;
			Camera2D camera = this.m_gameScreenControl.Camera;
			this.m_collHullRect.Width = 0f;
			this.m_collHullRect.Height = 0f;
			this.m_collHullRect.X = (float)((int)(e.Position.X * 1.0 / (double)camera.Zoom + (double)camera.RelTopLeftCorner.X));
			this.m_collHullRect.Y = (float)((int)(e.Position.Y * 1.0 / (double)camera.Zoom + (double)camera.RelTopLeftCorner.Y));
			if (this.m_gameScreenControl.SnapToGrid)
			{
				Vector2 vector = base.CalculateSnapTo(this.m_collHullRect.X, this.m_collHullRect.Y);
				this.m_collHullRect.X = this.m_collHullRect.X + vector.X;
				this.m_collHullRect.Y = this.m_collHullRect.Y + vector.Y;
			}
		}

		// Token: 0x060001FE RID: 510 RVA: 0x0000FE1C File Offset: 0x0000E01C
		public override void LeftMouseUp(object sender, HwndMouseEventArgs e)
		{
			if (this.m_mouseDown)
			{
				float x;
				float num;
				if (this.m_collHullRect.Width < 0f)
				{
					x = this.m_collHullRect.X + this.m_collHullRect.Width;
					num = this.m_collHullRect.Width * -1f;
				}
				else
				{
					x = this.m_collHullRect.X;
					num = this.m_collHullRect.Width;
				}
				float y;
				float num2;
				if (this.m_collHullRect.Height < 0f)
				{
					y = this.m_collHullRect.Y + this.m_collHullRect.Height;
					num2 = this.m_collHullRect.Height * -1f;
				}
				else
				{
					y = this.m_collHullRect.Y;
					num2 = this.m_collHullRect.Height;
				}
				if (num > 0f && num2 > 0f)
				{
					this.m_gameScreenControl.AddGameObj(new EditorCollHullObj(x, y, num, num2), true);
				}
			}
			this.m_mouseDown = false;
		}

		// Token: 0x060001FF RID: 511 RVA: 0x0000FF44 File Offset: 0x0000E144
		public override void MouseMove(object sender, HwndMouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				Camera2D camera = this.m_gameScreenControl.Camera;
				this.m_collHullRect.Width = (float)Math.Round(e.Position.X * 1.0 / (double)camera.Zoom + (double)camera.RelTopLeftCorner.X - (double)this.m_collHullRect.X, MidpointRounding.AwayFromZero);
				this.m_collHullRect.Height = (float)Math.Round(e.Position.Y * 1.0 / (double)camera.Zoom + (double)camera.RelTopLeftCorner.Y - (double)this.m_collHullRect.Y, MidpointRounding.AwayFromZero);
				if (this.m_gameScreenControl.SnapToGrid)
				{
					Vector2 vector = base.CalculateSnapTo(this.m_collHullRect.Width, this.m_collHullRect.Height);
					this.m_collHullRect.Width = this.m_collHullRect.Width + vector.X;
					this.m_collHullRect.Height = this.m_collHullRect.Height + vector.Y;
				}
			}
		}

		// Token: 0x06000200 RID: 512 RVA: 0x00010070 File Offset: 0x0000E270
		public override void Draw(Camera2D camera, float elapsedSeconds)
		{
			if (this.m_mouseDown)
			{
				float x;
				float width;
				if (this.m_collHullRect.Width < 0f)
				{
					x = this.m_collHullRect.X + this.m_collHullRect.Width;
					width = this.m_collHullRect.Width * -1f;
				}
				else
				{
					x = this.m_collHullRect.X;
					width = this.m_collHullRect.Width;
				}
				float y;
				float num;
				if (this.m_collHullRect.Height < 0f)
				{
					y = this.m_collHullRect.Y + this.m_collHullRect.Height;
					num = this.m_collHullRect.Height * -1f;
				}
				else
				{
					y = this.m_collHullRect.Y;
					num = this.m_collHullRect.Height;
				}
				if (num <= 1f)
				{
					num = 0f;
				}
				camera.Draw(StaticTexture.GenericTexture, new CDGRect(x, y, width, num).ToRectangle(), EditorEV.COLLHULL_COLOUR * 0.3f);
			}
		}

		// Token: 0x040000F3 RID: 243
		private CDGRect m_collHullRect = default(CDGRect);

		// Token: 0x040000F4 RID: 244
		private bool m_mouseDown;
	}
}

using System;
using System.Windows.Input;
using CDGEngine;
using Microsoft.Xna.Framework;

namespace BrawlerEditor
{
	// Token: 0x0200002B RID: 43
	public class RoomToolObj : ToolObj
	{
		// Token: 0x0600017D RID: 381 RVA: 0x0000B79F File Offset: 0x0000999F
		public RoomToolObj(GameScreenControl gameScreenControl)
			: base(gameScreenControl)
		{
			this.m_toolType = 4;
		}

		// Token: 0x0600017E RID: 382 RVA: 0x0000B7C0 File Offset: 0x000099C0
		public override void LeftMouseDown(object sender, HwndMouseEventArgs e)
		{
			this.m_mouseDown = true;
			Camera2D camera = this.m_gameScreenControl.Camera;
			this.m_roomRect.Width = 0f;
			this.m_roomRect.Height = 0f;
			this.m_roomRect.X = (float)((int)(e.Position.X * 1.0 / (double)camera.Zoom + (double)camera.RelTopLeftCorner.X));
			this.m_roomRect.Y = (float)((int)(e.Position.Y * 1.0 / (double)camera.Zoom + (double)camera.RelTopLeftCorner.Y));
			if (this.m_gameScreenControl.SnapToGrid)
			{
				Vector2 vector = base.CalculateSnapTo(this.m_roomRect.X, this.m_roomRect.Y);
				this.m_roomRect.X = this.m_roomRect.X + vector.X;
				this.m_roomRect.Y = this.m_roomRect.Y + vector.Y;
			}
		}

		// Token: 0x0600017F RID: 383 RVA: 0x0000B8DC File Offset: 0x00009ADC
		public override void LeftMouseUp(object sender, HwndMouseEventArgs e)
		{
			if (this.m_mouseDown)
			{
				float x;
				float num;
				if (this.m_roomRect.Width < 0f)
				{
					x = this.m_roomRect.X + this.m_roomRect.Width;
					num = this.m_roomRect.Width * -1f;
				}
				else
				{
					x = this.m_roomRect.X;
					num = this.m_roomRect.Width;
				}
				float y;
				float num2;
				if (this.m_roomRect.Height < 0f)
				{
					y = this.m_roomRect.Y + this.m_roomRect.Height;
					num2 = this.m_roomRect.Height * -1f;
				}
				else
				{
					y = this.m_roomRect.Y;
					num2 = this.m_roomRect.Height;
				}
				if (num > 0f && num2 > 0f)
				{
					this.m_gameScreenControl.AddGameObj(new RoomObj(x, y, num, num2), true);
				}
			}
			this.m_mouseDown = false;
		}

		// Token: 0x06000180 RID: 384 RVA: 0x0000BA04 File Offset: 0x00009C04
		public override void MouseMove(object sender, HwndMouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed && this.m_mouseDown)
			{
				Camera2D camera = this.m_gameScreenControl.Camera;
				this.m_roomRect.Width = (float)Math.Round(e.Position.X * 1.0 / (double)camera.Zoom + (double)camera.RelTopLeftCorner.X - (double)this.m_roomRect.X, MidpointRounding.AwayFromZero);
				this.m_roomRect.Height = (float)Math.Round(e.Position.Y * 1.0 / (double)camera.Zoom + (double)camera.RelTopLeftCorner.Y - (double)this.m_roomRect.Y, MidpointRounding.AwayFromZero);
				if (this.m_gameScreenControl.SnapToGrid)
				{
					Vector2 vector = base.CalculateSnapTo(this.m_roomRect.Width, this.m_roomRect.Height);
					this.m_roomRect.Width = this.m_roomRect.Width + vector.X;
					this.m_roomRect.Height = this.m_roomRect.Height + vector.Y;
				}
			}
		}

		// Token: 0x06000181 RID: 385 RVA: 0x0000BB38 File Offset: 0x00009D38
		public override void Draw(Camera2D camera, float elapsedSeconds)
		{
			if (this.m_mouseDown)
			{
				float x;
				float width;
				if (this.m_roomRect.Width < 0f)
				{
					x = this.m_roomRect.X + this.m_roomRect.Width;
					width = this.m_roomRect.Width * -1f;
				}
				else
				{
					x = this.m_roomRect.X;
					width = this.m_roomRect.Width;
				}
				float y;
				float num;
				if (this.m_roomRect.Height < 0f)
				{
					y = this.m_roomRect.Y + this.m_roomRect.Height;
					num = this.m_roomRect.Height * -1f;
				}
				else
				{
					y = this.m_roomRect.Y;
					num = this.m_roomRect.Height;
				}
				if (num <= 1f)
				{
					num = 0f;
				}
				camera.Draw(StaticTexture.GenericTexture, new CDGRect(x, y, width, num).ToRectangle(), EditorEV.ROOMOBJ_COLOUR * 0.3f);
			}
		}

		// Token: 0x040000AB RID: 171
		private CDGRect m_roomRect = default(CDGRect);

		// Token: 0x040000AC RID: 172
		private bool m_mouseDown;
	}
}

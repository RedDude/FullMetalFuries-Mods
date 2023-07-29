using System;
using System.Windows.Input;
using CDGEngine;
using Microsoft.Xna.Framework;

namespace BrawlerEditor
{
	// Token: 0x0200000E RID: 14
	public abstract class ToolObj
	{
		// Token: 0x17000014 RID: 20
		// (get) Token: 0x0600003E RID: 62 RVA: 0x00002834 File Offset: 0x00000A34
		public bool IsSelected
		{
			get
			{
				return this.m_isSelected;
			}
		}

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x0600003F RID: 63 RVA: 0x0000284C File Offset: 0x00000A4C
		public byte ToolType
		{
			get
			{
				return this.m_toolType;
			}
		}

		// Token: 0x06000040 RID: 64 RVA: 0x00002864 File Offset: 0x00000A64
		public ToolObj(GameScreenControl gameScreenControl)
		{
			this.m_toolType = 0;
			this.m_gameScreenControl = gameScreenControl;
		}

		// Token: 0x06000041 RID: 65 RVA: 0x0000287D File Offset: 0x00000A7D
		public virtual void MouseMove(object sender, HwndMouseEventArgs e)
		{
		}

		// Token: 0x06000042 RID: 66 RVA: 0x00002880 File Offset: 0x00000A80
		public virtual void LeftMouseDown(object sender, HwndMouseEventArgs e)
		{
		}

		// Token: 0x06000043 RID: 67 RVA: 0x00002883 File Offset: 0x00000A83
		public virtual void LeftMouseUp(object sender, HwndMouseEventArgs e)
		{
		}

		// Token: 0x06000044 RID: 68 RVA: 0x00002886 File Offset: 0x00000A86
		public virtual void KeyDown(KeyEventArgs e)
		{
		}

		// Token: 0x06000045 RID: 69 RVA: 0x00002889 File Offset: 0x00000A89
		public virtual void KeyUp(KeyEventArgs e)
		{
		}

		// Token: 0x06000046 RID: 70 RVA: 0x0000288C File Offset: 0x00000A8C
		public virtual void Draw(Camera2D camera, float elapsedSeconds)
		{
		}

		// Token: 0x06000047 RID: 71 RVA: 0x0000288F File Offset: 0x00000A8F
		public void SelectTool()
		{
			this.m_isSelected = true;
		}

		// Token: 0x06000048 RID: 72 RVA: 0x00002899 File Offset: 0x00000A99
		public void DeselectTool()
		{
			this.m_isSelected = false;
		}

		// Token: 0x06000049 RID: 73 RVA: 0x000028A4 File Offset: 0x00000AA4
		public Vector2 CalculateSnapTo(float x, float y)
		{
			int gridUnitSize = this.m_gameScreenControl.GridUnitSize;
			Vector2 result = default(Vector2);
			result.X = (float)((int)(x / (float)gridUnitSize));
			result.X *= (float)gridUnitSize;
			result.X = (float)((int)(x - result.X));
			if (result.X > (float)gridUnitSize * 0.5f)
			{
				result.X = (float)gridUnitSize - result.X;
			}
			else if (result.X < (float)(-(float)gridUnitSize) * 0.5f)
			{
				result.X = (float)(-(float)gridUnitSize) - result.X;
			}
			else
			{
				result.X = -result.X;
			}
			result.Y = (float)((int)(y / (float)gridUnitSize));
			result.Y *= (float)gridUnitSize;
			result.Y = (float)((int)(y - result.Y));
			if (result.Y > (float)gridUnitSize * 0.5f)
			{
				result.Y = (float)gridUnitSize - result.Y;
			}
			else if (result.Y < (float)(-(float)gridUnitSize) * 0.5f)
			{
				result.Y = (float)(-(float)gridUnitSize) - result.Y;
			}
			else
			{
				result.Y = -result.Y;
			}
			return result;
		}

		// Token: 0x0400001E RID: 30
		private bool m_isSelected;

		// Token: 0x0400001F RID: 31
		protected byte m_toolType;

		// Token: 0x04000020 RID: 32
		protected GameScreenControl m_gameScreenControl;
	}
}

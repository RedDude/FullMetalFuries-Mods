using System;
using CDGEngine;
using Microsoft.Xna.Framework;

namespace BrawlerEditor
{
	// Token: 0x0200002C RID: 44
	public class MarkerToolObj : ToolObj
	{
		// Token: 0x06000182 RID: 386 RVA: 0x0000BC67 File Offset: 0x00009E67
		public MarkerToolObj(GameScreenControl gameScreenControl)
			: base(gameScreenControl)
		{
			this.m_toolType = 6;
		}

		// Token: 0x06000183 RID: 387 RVA: 0x0000BC7C File Offset: 0x00009E7C
		public override void LeftMouseUp(object sender, HwndMouseEventArgs e)
		{
			Camera2D camera = this.m_gameScreenControl.Camera;
			float num = (float)(e.Position.X * 1.0 / (double)camera.Zoom + (double)camera.RelTopLeftCorner.X);
			float num2 = (float)(e.Position.Y * 1.0 / (double)camera.Zoom + (double)camera.RelTopLeftCorner.Y);
			if (this.m_gameScreenControl.SnapToGrid)
			{
				Vector2 vector = base.CalculateSnapTo(num, num2);
				num += vector.X;
				num2 += vector.Y;
			}
			MarkerObj markerObj = new MarkerObj();
			markerObj.Position = new Vector2(num, num2);
			this.m_gameScreenControl.AddGameObj(markerObj, true);
			base.LeftMouseUp(sender, e);
		}
	}
}

using System;
using CDGEngine;
using Microsoft.Xna.Framework;

namespace BrawlerEditor
{
	// Token: 0x0200002A RID: 42
	public class PlayerStartToolObj : ToolObj
	{
		// Token: 0x17000055 RID: 85
		// (get) Token: 0x06000179 RID: 377 RVA: 0x0000B674 File Offset: 0x00009874
		// (set) Token: 0x0600017A RID: 378 RVA: 0x0000B68C File Offset: 0x0000988C
		public bool setDebugOnly
		{
			get
			{
				return this.m_setDebugOnly;
			}
			set
			{
				this.m_setDebugOnly = value;
			}
		}

		// Token: 0x0600017B RID: 379 RVA: 0x0000B696 File Offset: 0x00009896
		public PlayerStartToolObj(GameScreenControl gameScreenControl)
			: base(gameScreenControl)
		{
			this.m_toolType = 7;
		}

		// Token: 0x0600017C RID: 380 RVA: 0x0000B6AC File Offset: 0x000098AC
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
			PlayerStartObj playerStartObj = new PlayerStartObj();
			playerStartObj.Position = new Vector2(num, num2);
			if (this.setDebugOnly)
			{
				playerStartObj.isDebugOnly = true;
			}
			this.m_gameScreenControl.AddGameObj(playerStartObj, true);
			base.LeftMouseUp(sender, e);
		}

		// Token: 0x040000AA RID: 170
		private bool m_setDebugOnly;
	}
}

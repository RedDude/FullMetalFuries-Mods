using System;
using Microsoft.Xna.Framework.Graphics;

namespace BrawlerEditor
{
	// Token: 0x02000047 RID: 71
	public class GraphicsDeviceEventArgs : EventArgs
	{
		// Token: 0x1700008C RID: 140
		// (get) Token: 0x060002A5 RID: 677 RVA: 0x0001717C File Offset: 0x0001537C
		// (set) Token: 0x060002A6 RID: 678 RVA: 0x00017193 File Offset: 0x00015393
		public GraphicsDevice GraphicsDevice { get; private set; }

		// Token: 0x060002A7 RID: 679 RVA: 0x0001719C File Offset: 0x0001539C
		public GraphicsDeviceEventArgs(GraphicsDevice device)
		{
			this.GraphicsDevice = device;
		}
	}
}

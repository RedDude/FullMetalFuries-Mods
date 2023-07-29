using System;
using System.Threading;
using Microsoft.Xna.Framework.Graphics;

namespace BrawlerEditor
{
	// Token: 0x02000048 RID: 72
	public class GraphicsDeviceService : IGraphicsDeviceService
	{
		// Token: 0x1700008D RID: 141
		// (get) Token: 0x060002A8 RID: 680 RVA: 0x000171B0 File Offset: 0x000153B0
		public GraphicsDevice GraphicsDevice
		{
			get
			{
				return this.graphicsDevice;
			}
		}

		// Token: 0x14000016 RID: 22
		// (add) Token: 0x060002A9 RID: 681 RVA: 0x000171C8 File Offset: 0x000153C8
		// (remove) Token: 0x060002AA RID: 682 RVA: 0x00017204 File Offset: 0x00015404
		public event EventHandler<EventArgs> DeviceCreated;

		// Token: 0x14000017 RID: 23
		// (add) Token: 0x060002AB RID: 683 RVA: 0x00017240 File Offset: 0x00015440
		// (remove) Token: 0x060002AC RID: 684 RVA: 0x0001727C File Offset: 0x0001547C
		public event EventHandler<EventArgs> DeviceDisposing;

		// Token: 0x14000018 RID: 24
		// (add) Token: 0x060002AD RID: 685 RVA: 0x000172B8 File Offset: 0x000154B8
		// (remove) Token: 0x060002AE RID: 686 RVA: 0x000172F4 File Offset: 0x000154F4
		public event EventHandler<EventArgs> DeviceReset;

		// Token: 0x14000019 RID: 25
		// (add) Token: 0x060002AF RID: 687 RVA: 0x00017330 File Offset: 0x00015530
		// (remove) Token: 0x060002B0 RID: 688 RVA: 0x0001736C File Offset: 0x0001556C
		public event EventHandler<EventArgs> DeviceResetting;

		// Token: 0x060002B1 RID: 689 RVA: 0x000173A8 File Offset: 0x000155A8
		private GraphicsDeviceService()
		{
		}

		// Token: 0x060002B2 RID: 690 RVA: 0x000173B4 File Offset: 0x000155B4
		private void CreateDevice(IntPtr windowHandle, int width, int height)
		{
			this.parameters = new PresentationParameters();
			this.parameters.BackBufferWidth = Math.Max(width, 1);
			this.parameters.BackBufferHeight = Math.Max(height, 1);
			this.parameters.BackBufferFormat = SurfaceFormat.Color;
			this.parameters.DepthStencilFormat = DepthFormat.Depth24;
			this.parameters.DeviceWindowHandle = windowHandle;
			this.parameters.PresentationInterval = PresentInterval.Immediate;
			this.parameters.IsFullScreen = false;
			this.graphicsDevice = new GraphicsDevice(GraphicsAdapter.DefaultAdapter, GraphicsProfile.Reach, this.parameters);
			if (this.DeviceCreated != null)
			{
				this.DeviceCreated(this, EventArgs.Empty);
			}
		}

		// Token: 0x060002B3 RID: 691 RVA: 0x0001746C File Offset: 0x0001566C
		public static GraphicsDeviceService AddRef(IntPtr windowHandle, int width, int height)
		{
			if (Interlocked.Increment(ref GraphicsDeviceService.referenceCount) == 1)
			{
				GraphicsDeviceService.singletonInstance.CreateDevice(windowHandle, width, height);
			}
			return GraphicsDeviceService.singletonInstance;
		}

		// Token: 0x060002B4 RID: 692 RVA: 0x000174A8 File Offset: 0x000156A8
		public void Release(bool disposing)
		{
			if (Interlocked.Decrement(ref GraphicsDeviceService.referenceCount) == 0)
			{
				if (disposing)
				{
					if (this.DeviceDisposing != null)
					{
						this.DeviceDisposing(this, EventArgs.Empty);
					}
					this.graphicsDevice.Dispose();
				}
				this.graphicsDevice = null;
			}
		}

		// Token: 0x060002B5 RID: 693 RVA: 0x00017508 File Offset: 0x00015708
		public void ResetDevice(int width, int height)
		{
			if (this.DeviceResetting != null)
			{
				this.DeviceResetting(this, EventArgs.Empty);
			}
			this.parameters.BackBufferWidth = Math.Max(this.parameters.BackBufferWidth, width);
			this.parameters.BackBufferHeight = Math.Max(this.parameters.BackBufferHeight, height);
			this.graphicsDevice.Reset(this.parameters);
			if (this.DeviceReset != null)
			{
				this.DeviceReset(this, EventArgs.Empty);
			}
		}

		// Token: 0x0400014D RID: 333
		private static readonly GraphicsDeviceService singletonInstance = new GraphicsDeviceService();

		// Token: 0x0400014E RID: 334
		private static int referenceCount;

		// Token: 0x0400014F RID: 335
		private GraphicsDevice graphicsDevice;

		// Token: 0x04000150 RID: 336
		private PresentationParameters parameters;
	}
}

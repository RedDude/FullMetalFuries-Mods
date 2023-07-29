using System;
using System.Diagnostics;
using CDGEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace BrawlerEditor
{
	// Token: 0x02000015 RID: 21
	public abstract class XnaControl : GraphicsDeviceControl, IControl
	{
		// Token: 0x17000018 RID: 24
		// (get) Token: 0x060000A0 RID: 160 RVA: 0x00005660 File Offset: 0x00003860
		// (set) Token: 0x060000A1 RID: 161 RVA: 0x00005677 File Offset: 0x00003877
		public MainWindow MainWindow { get; set; }

		// Token: 0x060000A2 RID: 162 RVA: 0x00005680 File Offset: 0x00003880
		public XnaControl()
		{
			base.LoadContent += this.loadContent;
			base.RenderXna += this.xnaControl_RenderXna;
			base.HwndMouseMove += this.Mouse_MouseMove;
			base.HwndLButtonDown += this.LeftButton_MouseDown;
			base.HwndLButtonUp += this.LeftButton_MouseUp;
			base.HwndRButtonDown += this.RightButton_MouseDown;
			base.HwndRButtonUp += this.RightButton_MouseUp;
			base.HwndMouseWheel += this.MiddleButton_Scroll;
		}

		// Token: 0x060000A3 RID: 163 RVA: 0x00005740 File Offset: 0x00003940
		public virtual void Initialize()
		{
		}

		// Token: 0x060000A4 RID: 164 RVA: 0x00005744 File Offset: 0x00003944
		protected virtual void loadContent(object sender, GraphicsDeviceEventArgs e)
		{
			this.GraphicsDevice = e.GraphicsDevice;
			this.m_camera = new Camera2D(this.GraphicsDevice, this.GraphicsDevice.Viewport.Width, this.GraphicsDevice.Viewport.Height);
			this.m_genericTexture = new Texture2D(this.GraphicsDevice, 1, 1);
			int[] data = new int[] { 16777215 };
			this.m_genericTexture.SetData<int>(data, 0, this.m_genericTexture.Width * this.m_genericTexture.Height);
			this.watch.Start();
			this.services = new GameServiceContainer();
			this.services.AddService(typeof(IGraphicsDeviceService), base.GraphicsService);
			this.Content = new ContentManager(this.services, "BrawlerEditorContent");
		}

		// Token: 0x060000A5 RID: 165 RVA: 0x00005827 File Offset: 0x00003A27
		private void xnaControl_RenderXna(object sender, GraphicsDeviceEventArgs e)
		{
			this.Update(this.watch);
			this.Draw(this.watch);
		}

		// Token: 0x060000A6 RID: 166
		protected abstract void Update(Stopwatch gameTime);

		// Token: 0x060000A7 RID: 167
		protected abstract void Draw(Stopwatch gameTime);

		// Token: 0x060000A8 RID: 168
		protected abstract void LeftButton_MouseDown(object sender, HwndMouseEventArgs e);

		// Token: 0x060000A9 RID: 169
		protected abstract void LeftButton_MouseUp(object sender, HwndMouseEventArgs e);

		// Token: 0x060000AA RID: 170
		protected abstract void RightButton_MouseDown(object sender, HwndMouseEventArgs e);

		// Token: 0x060000AB RID: 171
		protected abstract void RightButton_MouseUp(object sender, HwndMouseEventArgs e);

		// Token: 0x060000AC RID: 172
		protected abstract void MiddleButton_Scroll(object sender, HwndMouseEventArgs e);

		// Token: 0x060000AD RID: 173
		protected abstract void Mouse_MouseMove(object sender, HwndMouseEventArgs e);

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x060000AE RID: 174 RVA: 0x00005844 File Offset: 0x00003A44
		public Camera2D Camera
		{
			get
			{
				return this.m_camera;
			}
		}

		// Token: 0x0400005E RID: 94
		public GraphicsDevice GraphicsDevice;

		// Token: 0x0400005F RID: 95
		protected Camera2D m_camera;

		// Token: 0x04000060 RID: 96
		protected Stopwatch watch = new Stopwatch();

		// Token: 0x04000061 RID: 97
		protected Texture2D m_genericTexture;

		// Token: 0x04000062 RID: 98
		protected ContentManager Content;

		// Token: 0x04000063 RID: 99
		protected GameServiceContainer services;
	}
}

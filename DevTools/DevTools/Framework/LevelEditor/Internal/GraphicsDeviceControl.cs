using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BrawlerEditor
{
	// Token: 0x02000014 RID: 20
	public class GraphicsDeviceControl : HwndHost
	{
		// Token: 0x17000017 RID: 23
		// (get) Token: 0x06000062 RID: 98 RVA: 0x00003CD8 File Offset: 0x00001ED8
		public GraphicsDeviceService GraphicsService
		{
			get
			{
				return this.graphicsService;
			}
		}

		// Token: 0x14000001 RID: 1
		// (add) Token: 0x06000063 RID: 99 RVA: 0x00003CF0 File Offset: 0x00001EF0
		// (remove) Token: 0x06000064 RID: 100 RVA: 0x00003D2C File Offset: 0x00001F2C
		public event EventHandler<GraphicsDeviceEventArgs> LoadContent;

		// Token: 0x14000002 RID: 2
		// (add) Token: 0x06000065 RID: 101 RVA: 0x00003D68 File Offset: 0x00001F68
		// (remove) Token: 0x06000066 RID: 102 RVA: 0x00003DA4 File Offset: 0x00001FA4
		public event EventHandler<GraphicsDeviceEventArgs> RenderXna;

		// Token: 0x14000003 RID: 3
		// (add) Token: 0x06000067 RID: 103 RVA: 0x00003DE0 File Offset: 0x00001FE0
		// (remove) Token: 0x06000068 RID: 104 RVA: 0x00003E1C File Offset: 0x0000201C
		public event EventHandler<HwndMouseEventArgs> HwndLButtonDown;

		// Token: 0x14000004 RID: 4
		// (add) Token: 0x06000069 RID: 105 RVA: 0x00003E58 File Offset: 0x00002058
		// (remove) Token: 0x0600006A RID: 106 RVA: 0x00003E94 File Offset: 0x00002094
		public event EventHandler<HwndMouseEventArgs> HwndLButtonUp;

		// Token: 0x14000005 RID: 5
		// (add) Token: 0x0600006B RID: 107 RVA: 0x00003ED0 File Offset: 0x000020D0
		// (remove) Token: 0x0600006C RID: 108 RVA: 0x00003F0C File Offset: 0x0000210C
		public event EventHandler<HwndMouseEventArgs> HwndLButtonDblClick;

		// Token: 0x14000006 RID: 6
		// (add) Token: 0x0600006D RID: 109 RVA: 0x00003F48 File Offset: 0x00002148
		// (remove) Token: 0x0600006E RID: 110 RVA: 0x00003F84 File Offset: 0x00002184
		public event EventHandler<HwndMouseEventArgs> HwndRButtonDown;

		// Token: 0x14000007 RID: 7
		// (add) Token: 0x0600006F RID: 111 RVA: 0x00003FC0 File Offset: 0x000021C0
		// (remove) Token: 0x06000070 RID: 112 RVA: 0x00003FFC File Offset: 0x000021FC
		public event EventHandler<HwndMouseEventArgs> HwndRButtonUp;

		// Token: 0x14000008 RID: 8
		// (add) Token: 0x06000071 RID: 113 RVA: 0x00004038 File Offset: 0x00002238
		// (remove) Token: 0x06000072 RID: 114 RVA: 0x00004074 File Offset: 0x00002274
		public event EventHandler<HwndMouseEventArgs> HwndRButtonDblClick;

		// Token: 0x14000009 RID: 9
		// (add) Token: 0x06000073 RID: 115 RVA: 0x000040B0 File Offset: 0x000022B0
		// (remove) Token: 0x06000074 RID: 116 RVA: 0x000040EC File Offset: 0x000022EC
		public event EventHandler<HwndMouseEventArgs> HwndMButtonDown;

		// Token: 0x1400000A RID: 10
		// (add) Token: 0x06000075 RID: 117 RVA: 0x00004128 File Offset: 0x00002328
		// (remove) Token: 0x06000076 RID: 118 RVA: 0x00004164 File Offset: 0x00002364
		public event EventHandler<HwndMouseEventArgs> HwndMButtonUp;

		// Token: 0x1400000B RID: 11
		// (add) Token: 0x06000077 RID: 119 RVA: 0x000041A0 File Offset: 0x000023A0
		// (remove) Token: 0x06000078 RID: 120 RVA: 0x000041DC File Offset: 0x000023DC
		public event EventHandler<HwndMouseEventArgs> HwndMButtonDblClick;

		// Token: 0x1400000C RID: 12
		// (add) Token: 0x06000079 RID: 121 RVA: 0x00004218 File Offset: 0x00002418
		// (remove) Token: 0x0600007A RID: 122 RVA: 0x00004254 File Offset: 0x00002454
		public event EventHandler<HwndMouseEventArgs> HwndX1ButtonDown;

		// Token: 0x1400000D RID: 13
		// (add) Token: 0x0600007B RID: 123 RVA: 0x00004290 File Offset: 0x00002490
		// (remove) Token: 0x0600007C RID: 124 RVA: 0x000042CC File Offset: 0x000024CC
		public event EventHandler<HwndMouseEventArgs> HwndX1ButtonUp;

		// Token: 0x1400000E RID: 14
		// (add) Token: 0x0600007D RID: 125 RVA: 0x00004308 File Offset: 0x00002508
		// (remove) Token: 0x0600007E RID: 126 RVA: 0x00004344 File Offset: 0x00002544
		public event EventHandler<HwndMouseEventArgs> HwndX1ButtonDblClick;

		// Token: 0x1400000F RID: 15
		// (add) Token: 0x0600007F RID: 127 RVA: 0x00004380 File Offset: 0x00002580
		// (remove) Token: 0x06000080 RID: 128 RVA: 0x000043BC File Offset: 0x000025BC
		public event EventHandler<HwndMouseEventArgs> HwndX2ButtonDown;

		// Token: 0x14000010 RID: 16
		// (add) Token: 0x06000081 RID: 129 RVA: 0x000043F8 File Offset: 0x000025F8
		// (remove) Token: 0x06000082 RID: 130 RVA: 0x00004434 File Offset: 0x00002634
		public event EventHandler<HwndMouseEventArgs> HwndX2ButtonUp;

		// Token: 0x14000011 RID: 17
		// (add) Token: 0x06000083 RID: 131 RVA: 0x00004470 File Offset: 0x00002670
		// (remove) Token: 0x06000084 RID: 132 RVA: 0x000044AC File Offset: 0x000026AC
		public event EventHandler<HwndMouseEventArgs> HwndX2ButtonDblClick;

		// Token: 0x14000012 RID: 18
		// (add) Token: 0x06000085 RID: 133 RVA: 0x000044E8 File Offset: 0x000026E8
		// (remove) Token: 0x06000086 RID: 134 RVA: 0x00004524 File Offset: 0x00002724
		public event EventHandler<HwndMouseEventArgs> HwndMouseMove;

		// Token: 0x14000013 RID: 19
		// (add) Token: 0x06000087 RID: 135 RVA: 0x00004560 File Offset: 0x00002760
		// (remove) Token: 0x06000088 RID: 136 RVA: 0x0000459C File Offset: 0x0000279C
		public event EventHandler<HwndMouseEventArgs> HwndMouseEnter;

		// Token: 0x14000014 RID: 20
		// (add) Token: 0x06000089 RID: 137 RVA: 0x000045D8 File Offset: 0x000027D8
		// (remove) Token: 0x0600008A RID: 138 RVA: 0x00004614 File Offset: 0x00002814
		public event EventHandler<HwndMouseEventArgs> HwndMouseLeave;

		// Token: 0x14000015 RID: 21
		// (add) Token: 0x0600008B RID: 139 RVA: 0x00004650 File Offset: 0x00002850
		// (remove) Token: 0x0600008C RID: 140 RVA: 0x0000468C File Offset: 0x0000288C
		public event EventHandler<HwndMouseEventArgs> HwndMouseWheel;

		// Token: 0x0600008D RID: 141 RVA: 0x000046C8 File Offset: 0x000028C8
		public GraphicsDeviceControl()
		{
			base.Loaded += this.XnaWindowHost_Loaded;
			base.SizeChanged += this.XnaWindowHost_SizeChanged;
			Application.Current.Activated += this.Current_Activated;
			Application.Current.Deactivated += this.Current_Deactivated;
			CompositionTarget.Rendering += this.CompositionTarget_Rendering;
		}

		// Token: 0x0600008E RID: 142 RVA: 0x00004770 File Offset: 0x00002970
		protected override void Dispose(bool disposing)
		{
			if (this.graphicsService != null)
			{
				this.graphicsService.Release(disposing);
				this.graphicsService = null;
			}
			CompositionTarget.Rendering -= this.CompositionTarget_Rendering;
			base.Dispose(disposing);
		}

		// Token: 0x0600008F RID: 143 RVA: 0x000047BC File Offset: 0x000029BC
		public new void CaptureMouse()
		{
			if (!this.isMouseCaptured)
			{
				this.isMouseCaptured = true;
				NativeMethods.POINT point = default(NativeMethods.POINT);
				this.capturedMouseX = point.X;
				this.capturedMouseY = point.Y;
				NativeMethods.ScreenToClient(this.hWnd, ref point);
				this.capturedMouseClientX = point.X;
				this.capturedMouseClientY = point.Y;
			}
		}

		// Token: 0x06000090 RID: 144 RVA: 0x0000482C File Offset: 0x00002A2C
		public void SetCursor(Cursor cursor)
		{
			// base.Cursor = cursor;
			// int lpCursorName;
			// if (base.Cursor == Cursors.Arrow)
			// {
			// 	lpCursorName = 32512;
			// }
			// else if (base.Cursor == Cursors.IBeam)
			// {
			// 	lpCursorName = 32513;
			// }
			// else if (base.Cursor == Cursors.Wait)
			// {
			// 	lpCursorName = 32514;
			// }
			// else if (base.Cursor == Cursors.Cross)
			// {
			// 	lpCursorName = 32515;
			// }
			// else if (base.Cursor == Cursors.UpArrow)
			// {
			// 	lpCursorName = 32516;
			// }
			// else if (base.Cursor == Cursors.SizeNWSE)
			// {
			// 	lpCursorName = 32642;
			// }
			// else if (base.Cursor == Cursors.SizeNESW)
			// {
			// 	lpCursorName = 32643;
			// }
			// else if (base.Cursor == Cursors.SizeWE)
			// {
			// 	lpCursorName = 32644;
			// }
			// else if (base.Cursor == Cursors.SizeNS)
			// {
			// 	lpCursorName = 32645;
			// }
			// else if (base.Cursor == Cursors.SizeAll)
			// {
			// 	lpCursorName = 32646;
			// }
			// else if (base.Cursor == Cursors.No)
			// {
			// 	lpCursorName = 32648;
			// }
			// else if (base.Cursor == Cursors.Hand)
			// {
			// 	lpCursorName = 32649;
			// }
			// else if (base.Cursor == Cursors.AppStarting)
			// {
			// 	lpCursorName = 32650;
			// }
			// else if (base.Cursor == Cursors.Help)
			// {
			// 	lpCursorName = 32651;
			// }
			// else
			// {
			// 	lpCursorName = 32512;
			// }
			// IntPtr intPtr = NativeMethods.LoadCursor(IntPtr.Zero, lpCursorName);
			// if (this._cursorPtr != IntPtr.Zero)
			// {
			// 	NativeMethods.DestroyCursor(this._cursorPtr);
			// }
			// this._cursorPtr = intPtr;
			// NativeMethods.SetCursor(intPtr);
			// NativeMethods.SetClassLong(this.hWnd, -12, this._cursorPtr);
		}

		// Token: 0x06000091 RID: 145 RVA: 0x00004A40 File Offset: 0x00002C40
		public new void ReleaseMouseCapture()
		{
			if (this.isMouseCaptured)
			{
				this.isMouseCaptured = false;
			}
		}

		// Token: 0x06000092 RID: 146 RVA: 0x00004A64 File Offset: 0x00002C64
		private void CompositionTarget_Rendering(object sender, EventArgs e)
		{
			if (this.graphicsService != null)
			{
				int num = (int)base.ActualWidth;
				int num2 = (int)base.ActualHeight;
				if (num >= 1 && num2 >= 1)
				{
					Viewport viewport = new Viewport(0, 0, num, num2);
					this.graphicsService.GraphicsDevice.Viewport = viewport;
					if (this.RenderXna != null)
					{
						this.RenderXna(this, new GraphicsDeviceEventArgs(this.graphicsService.GraphicsDevice));
					}
					if (this.graphicsService.GraphicsDevice.GraphicsDeviceStatus != GraphicsDeviceStatus.Lost)
					{
						if (this.graphicsService.GraphicsDevice.GraphicsDeviceStatus == GraphicsDeviceStatus.NotReset)
						{
							this.graphicsService.ResetDevice(num, num2);
						}
						if (this.graphicsService.GraphicsDevice.GraphicsDeviceStatus == GraphicsDeviceStatus.Normal)
						{
							this.graphicsService.GraphicsDevice.Present(new Rectangle?(viewport.Bounds), null, this.hWnd);
						}
					}
				}
			}
		}

		// Token: 0x06000093 RID: 147 RVA: 0x00004B84 File Offset: 0x00002D84
		private void XnaWindowHost_Loaded(object sender, RoutedEventArgs e)
		{
			if (this.graphicsService == null)
			{
				if (base.ActualWidth == 0.0 && base.ActualHeight == 0.0)
				{
					this.graphicsService = GraphicsDeviceService.AddRef(this.hWnd, (int)GraphicsDeviceControl.lastWidth, (int)GraphicsDeviceControl.lastHeight);
					Viewport viewport = new Viewport(0, 0, (int)GraphicsDeviceControl.lastWidth, (int)GraphicsDeviceControl.lastHeight);
					this.graphicsService.GraphicsDevice.Viewport = viewport;
				}
				else
				{
					this.graphicsService = GraphicsDeviceService.AddRef(this.hWnd, (int)base.ActualWidth, (int)base.ActualHeight);
					Viewport viewport = new Viewport(0, 0, (int)base.ActualWidth, (int)base.ActualHeight);
					GraphicsDeviceControl.lastWidth = (float)viewport.Width;
					GraphicsDeviceControl.lastHeight = (float)viewport.Height;
					this.graphicsService.GraphicsDevice.Viewport = viewport;
				}
				if (this.LoadContent != null)
				{
					this.LoadContent(this, new GraphicsDeviceEventArgs(this.graphicsService.GraphicsDevice));
				}
			}
		}

		// Token: 0x06000094 RID: 148 RVA: 0x00004CAC File Offset: 0x00002EAC
		private void XnaWindowHost_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			if (this.graphicsService != null)
			{
				this.graphicsService.ResetDevice((int)base.ActualWidth, (int)base.ActualHeight);
			}
		}

		// Token: 0x06000095 RID: 149 RVA: 0x00004CE1 File Offset: 0x00002EE1
		private void Current_Activated(object sender, EventArgs e)
		{
			this.applicationHasFocus = true;
		}

		// Token: 0x06000096 RID: 150 RVA: 0x00004CEC File Offset: 0x00002EEC
		private void Current_Deactivated(object sender, EventArgs e)
		{
			this.applicationHasFocus = false;
			this.ResetMouseState();
			if (this.mouseInWindow)
			{
				this.mouseInWindow = false;
				if (this.HwndMouseLeave != null)
				{
					this.HwndMouseLeave(this, new HwndMouseEventArgs(this.mouseState));
				}
			}
			this.ReleaseMouseCapture();
		}

		// Token: 0x06000097 RID: 151 RVA: 0x00004D4C File Offset: 0x00002F4C
		private void ResetMouseState()
		{
			bool flag = this.mouseState.LeftButton == MouseButtonState.Pressed;
			bool flag2 = this.mouseState.MiddleButton == MouseButtonState.Pressed;
			bool flag3 = this.mouseState.RightButton == MouseButtonState.Pressed;
			bool flag4 = this.mouseState.X1Button == MouseButtonState.Pressed;
			bool flag5 = this.mouseState.X2Button == MouseButtonState.Pressed;
			this.mouseState.LeftButton = MouseButtonState.Released;
			this.mouseState.MiddleButton = MouseButtonState.Released;
			this.mouseState.RightButton = MouseButtonState.Released;
			this.mouseState.X1Button = MouseButtonState.Released;
			this.mouseState.X2Button = MouseButtonState.Released;
			HwndMouseEventArgs e = new HwndMouseEventArgs(this.mouseState);
			if (flag && this.HwndLButtonUp != null)
			{
				this.HwndLButtonUp(this, e);
			}
			if (flag2 && this.HwndMButtonUp != null)
			{
				this.HwndMButtonUp(this, e);
			}
			if (flag3 && this.HwndRButtonUp != null)
			{
				this.HwndRButtonUp(this, e);
			}
			if (flag4 && this.HwndX1ButtonUp != null)
			{
				this.HwndX1ButtonUp(this, e);
			}
			if (flag5 && this.HwndX2ButtonUp != null)
			{
				this.HwndX2ButtonUp(this, e);
			}
			this.mouseInWindow = false;
		}

		// Token: 0x06000098 RID: 152 RVA: 0x00004EB0 File Offset: 0x000030B0
		protected override HandleRef BuildWindowCore(HandleRef hwndParent)
		{
			this.hWnd = this.CreateHostWindow(hwndParent.Handle);
			return new HandleRef(this, this.hWnd);
		}

		// Token: 0x06000099 RID: 153 RVA: 0x00004EE1 File Offset: 0x000030E1
		protected override void DestroyWindowCore(HandleRef hwnd)
		{
			NativeMethods.DestroyWindow(hwnd.Handle);
			this.hWnd = IntPtr.Zero;
		}

		// Token: 0x0600009A RID: 154 RVA: 0x00004EFC File Offset: 0x000030FC
		private IntPtr CreateHostWindow(IntPtr hWndParent)
		{
			this.RegisterWindowClass();
			return NativeMethods.CreateWindowEx(0, "GraphicsDeviceControlHostWindowClass", "", 1342177280, 0, 0, (int)base.Width, (int)base.Height, hWndParent, IntPtr.Zero, IntPtr.Zero, 0);
		}

		// Token: 0x0600009B RID: 155 RVA: 0x00004F4C File Offset: 0x0000314C
		private void RegisterWindowClass()
		{
			NativeMethods.WNDCLASSEX structure = default(NativeMethods.WNDCLASSEX);
			structure.cbSize = (uint)Marshal.SizeOf<NativeMethods.WNDCLASSEX>(structure);
			structure.hInstance = NativeMethods.GetModuleHandle(null);
			structure.lpfnWndProc = NativeMethods.DefaultWindowProc;
			structure.lpszClassName = "GraphicsDeviceControlHostWindowClass";
			structure.hCursor = NativeMethods.LoadCursor(IntPtr.Zero, 32512);
			NativeMethods.RegisterClassEx(ref structure);
		}

		// Token: 0x0600009C RID: 156 RVA: 0x00004FB4 File Offset: 0x000031B4
		protected override IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			switch (msg)
			{
			case 512:
				this.mouseState.PreviousPosition = this.mouseState.Position;
				this.mouseState.Position = new System.Windows.Point((double)NativeMethods.GetXLParam((int)lParam), (double)NativeMethods.GetYLParam((int)lParam));
				if (this.mouseState.Position != this.mouseState.PreviousPosition)
				{
					if (this.HwndMouseMove != null)
					{
						this.HwndMouseMove(this, new HwndMouseEventArgs(this.mouseState));
					}
				}
				break;
			case 513:
				if (!this.mouseInWindow)
				{
					this.mouseInWindow = true;
					this.mouseState.PreviousPosition = this.mouseState.Position;
					if (this.HwndMouseEnter != null)
					{
						this.HwndMouseEnter(this, new HwndMouseEventArgs(this.mouseState));
					}
					this.hWndprev = NativeMethods.GetFocus();
					NativeMethods.SetFocus(this.hWnd);
					NativeMethods.TRACKMOUSEEVENT trackmouseevent = default(NativeMethods.TRACKMOUSEEVENT);
					trackmouseevent.cbSize = Marshal.SizeOf(typeof(NativeMethods.TRACKMOUSEEVENT));
					trackmouseevent.dwFlags = 2U;
					trackmouseevent.hWnd = hwnd;
					NativeMethods.TrackMouseEvent(ref trackmouseevent);
				}
				this.mouseState.LeftButton = MouseButtonState.Pressed;
				if (this.HwndLButtonDown != null)
				{
					this.HwndLButtonDown(this, new HwndMouseEventArgs(this.mouseState));
				}
				break;
			case 514:
				this.mouseState.LeftButton = MouseButtonState.Released;
				if (this.HwndLButtonUp != null)
				{
					this.HwndLButtonUp(this, new HwndMouseEventArgs(this.mouseState));
				}
				break;
			case 515:
				if (this.HwndLButtonDblClick != null)
				{
					this.HwndLButtonDblClick(this, new HwndMouseEventArgs(this.mouseState, MouseButton.Left));
				}
				break;
			case 516:
				if (!this.mouseInWindow)
				{
					this.mouseInWindow = true;
					this.mouseState.PreviousPosition = this.mouseState.Position;
					if (this.HwndMouseEnter != null)
					{
						this.HwndMouseEnter(this, new HwndMouseEventArgs(this.mouseState));
					}
					this.hWndprev = NativeMethods.GetFocus();
					NativeMethods.SetFocus(this.hWnd);
					NativeMethods.TRACKMOUSEEVENT trackmouseevent = default(NativeMethods.TRACKMOUSEEVENT);
					trackmouseevent.cbSize = Marshal.SizeOf(typeof(NativeMethods.TRACKMOUSEEVENT));
					trackmouseevent.dwFlags = 2U;
					trackmouseevent.hWnd = hwnd;
					NativeMethods.TrackMouseEvent(ref trackmouseevent);
				}
				this.mouseState.RightButton = MouseButtonState.Pressed;
				if (this.HwndRButtonDown != null)
				{
					this.HwndRButtonDown(this, new HwndMouseEventArgs(this.mouseState));
				}
				break;
			case 517:
				this.mouseState.RightButton = MouseButtonState.Released;
				if (this.HwndRButtonUp != null)
				{
					this.HwndRButtonUp(this, new HwndMouseEventArgs(this.mouseState));
				}
				break;
			case 518:
				if (this.HwndRButtonDblClick != null)
				{
					this.HwndRButtonDblClick(this, new HwndMouseEventArgs(this.mouseState, MouseButton.Right));
				}
				break;
			case 519:
				this.mouseState.MiddleButton = MouseButtonState.Pressed;
				if (this.HwndMButtonDown != null)
				{
					this.HwndMButtonDown(this, new HwndMouseEventArgs(this.mouseState));
				}
				break;
			case 520:
				this.mouseState.MiddleButton = MouseButtonState.Released;
				if (this.HwndMButtonUp != null)
				{
					this.HwndMButtonUp(this, new HwndMouseEventArgs(this.mouseState));
				}
				break;
			case 521:
				if (this.HwndMButtonDblClick != null)
				{
					this.HwndMButtonDblClick(this, new HwndMouseEventArgs(this.mouseState, MouseButton.Middle));
				}
				break;
			case 522:
				break;
			case 523:
				if (((int)wParam & 32) != 0)
				{
					this.mouseState.X1Button = MouseButtonState.Pressed;
					if (this.HwndX1ButtonDown != null)
					{
						this.HwndX1ButtonDown(this, new HwndMouseEventArgs(this.mouseState));
					}
				}
				else if (((int)wParam & 64) != 0)
				{
					this.mouseState.X2Button = MouseButtonState.Pressed;
					if (this.HwndX2ButtonDown != null)
					{
						this.HwndX2ButtonDown(this, new HwndMouseEventArgs(this.mouseState));
					}
				}
				break;
			case 524:
				if (((int)wParam & 32) != 0)
				{
					this.mouseState.X1Button = MouseButtonState.Released;
					if (this.HwndX1ButtonUp != null)
					{
						this.HwndX1ButtonUp(this, new HwndMouseEventArgs(this.mouseState));
					}
				}
				else if (((int)wParam & 64) != 0)
				{
					this.mouseState.X2Button = MouseButtonState.Released;
					if (this.HwndX2ButtonUp != null)
					{
						this.HwndX2ButtonUp(this, new HwndMouseEventArgs(this.mouseState));
					}
				}
				break;
			case 525:
				if (((int)wParam & 32) != 0)
				{
					if (this.HwndX1ButtonDblClick != null)
					{
						this.HwndX1ButtonDblClick(this, new HwndMouseEventArgs(this.mouseState, MouseButton.XButton1));
					}
				}
				else if (((int)wParam & 64) != 0)
				{
					if (this.HwndX2ButtonDblClick != null)
					{
						this.HwndX2ButtonDblClick(this, new HwndMouseEventArgs(this.mouseState, MouseButton.XButton2));
					}
				}
				break;
			default:
				if (msg == 675)
				{
					if (!this.isMouseCaptured)
					{
						this.ResetMouseState();
						if (this.HwndMouseLeave != null)
						{
							this.HwndMouseLeave(this, new HwndMouseEventArgs(this.mouseState));
						}
						NativeMethods.SetFocus(this.hWndprev);
					}
				}
				break;
			}
			return base.WndProc(hwnd, msg, wParam, lParam, ref handled);
		}

		// Token: 0x0600009D RID: 157 RVA: 0x00005594 File Offset: 0x00003794
		public void ForceMouseFocusRemoval()
		{
			this.ResetMouseState();
			if (this.HwndMouseLeave != null)
			{
				this.HwndMouseLeave(this, new HwndMouseEventArgs(this.mouseState));
			}
		}

		// Token: 0x0600009E RID: 158 RVA: 0x000055D0 File Offset: 0x000037D0
		public void ForceMouseWheelInput(IntPtr wParam)
		{
			int mouseWheelDelta = wParam.ToInt32();
			if (this.HwndMouseWheel != null)
			{
				this.HwndMouseWheel(this, new HwndMouseEventArgs(this.mouseState, mouseWheelDelta, 0));
			}
		}

		// Token: 0x0600009F RID: 159 RVA: 0x00005610 File Offset: 0x00003810
		public void ForceMousePos(int x, int y)
		{
			NativeMethods.SetCursorPos(x - this.capturedMouseClientX, y - this.capturedMouseClientY);
			this.mouseState.Position = (this.mouseState.PreviousPosition = new System.Windows.Point((double)this.capturedMouseClientX, (double)this.capturedMouseClientY));
		}

		// Token: 0x0400003A RID: 58
		private const string windowClass = "GraphicsDeviceControlHostWindowClass";

		// Token: 0x0400003B RID: 59
		private static float lastWidth;

		// Token: 0x0400003C RID: 60
		private static float lastHeight;

		// Token: 0x0400003D RID: 61
		private IntPtr hWnd;

		// Token: 0x0400003E RID: 62
		private IntPtr hWndprev;

		// Token: 0x0400003F RID: 63
		private GraphicsDeviceService graphicsService;

		// Token: 0x04000040 RID: 64
		private bool applicationHasFocus = false;

		// Token: 0x04000041 RID: 65
		private bool mouseInWindow = false;

		// Token: 0x04000042 RID: 66
		private HwndMouseState mouseState = new HwndMouseState();

		// Token: 0x04000043 RID: 67
		private bool isMouseCaptured = false;

		// Token: 0x04000044 RID: 68
		private int capturedMouseX;

		// Token: 0x04000045 RID: 69
		private int capturedMouseY;

		// Token: 0x04000046 RID: 70
		private int capturedMouseClientX;

		// Token: 0x04000047 RID: 71
		private int capturedMouseClientY;

		// Token: 0x0400005D RID: 93
		private IntPtr _cursorPtr = IntPtr.Zero;
	}
}

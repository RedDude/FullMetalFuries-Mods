using System;
using System.Runtime.InteropServices;

namespace BrawlerEditor
{
	// Token: 0x0200004C RID: 76
	internal static class NativeMethods
	{
		// Token: 0x06000300 RID: 768
		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern IntPtr CreateWindowEx(int exStyle, string className, string windowName, int style, int x, int y, int width, int height, IntPtr hwndParent, IntPtr hMenu, IntPtr hInstance, [MarshalAs(UnmanagedType.AsAny)] object pvParam);

		// Token: 0x06000301 RID: 769
		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern bool DestroyWindow(IntPtr hwnd);

		// Token: 0x06000302 RID: 770
		[DllImport("user32.dll")]
		public static extern IntPtr DefWindowProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);

		// Token: 0x06000303 RID: 771
		[DllImport("kernel32.dll")]
		public static extern IntPtr GetModuleHandle(string module);

		// Token: 0x06000304 RID: 772
		[DllImport("user32.dll")]
		public static extern IntPtr LoadCursor(IntPtr hInstance, int lpCursorName);

		// Token: 0x06000305 RID: 773
		[DllImport("user32.dll")]
		public static extern bool DestroyCursor(IntPtr hCursor);

		// Token: 0x06000306 RID: 774
		[DllImport("user32.dll")]
		public static extern IntPtr LoadCursorFromFile(string lpFileName);

		// Token: 0x06000307 RID: 775
		[DllImport("user32.dll")]
		public static extern IntPtr SetCursor(IntPtr hCursor);

		// Token: 0x06000308 RID: 776
		[DllImport("user32.dll")]
		public static extern IntPtr SetClassLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

		// Token: 0x06000309 RID: 777
		[DllImport("user32.dll")]
		public static extern bool SetSystemCursor(IntPtr hcur, uint id);

		// Token: 0x0600030A RID: 778
		[DllImport("user32.dll")]
		public static extern int TrackMouseEvent(ref NativeMethods.TRACKMOUSEEVENT lpEventTrack);

		// Token: 0x0600030B RID: 779
		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.U2)]
		public static extern short RegisterClassEx([In] ref NativeMethods.WNDCLASSEX lpwcx);

		// Token: 0x0600030C RID: 780
		[DllImport("user32.dll")]
		public static extern int ShowCursor(bool bShow);

		// Token: 0x0600030D RID: 781
		[DllImport("user32.dll")]
		public static extern bool GetCursorPos(ref NativeMethods.POINT point);

		// Token: 0x0600030E RID: 782
		[DllImport("user32.dll")]
		public static extern bool SetCursorPos(int x, int y);

		// Token: 0x0600030F RID: 783
		[DllImport("user32.dll")]
		public static extern int ScreenToClient(IntPtr hWnd, ref NativeMethods.POINT pt);

		// Token: 0x06000310 RID: 784
		[DllImport("user32.dll")]
		public static extern int SetFocus(IntPtr hWnd);

		// Token: 0x06000311 RID: 785
		[DllImport("user32.dll")]
		public static extern IntPtr GetFocus();

		// Token: 0x06000312 RID: 786 RVA: 0x00019964 File Offset: 0x00017B64
		public static int GetXLParam(int lParam)
		{
			return NativeMethods.LowWord(lParam);
		}

		// Token: 0x06000313 RID: 787 RVA: 0x0001997C File Offset: 0x00017B7C
		public static int GetYLParam(int lParam)
		{
			return NativeMethods.HighWord(lParam);
		}

		// Token: 0x06000314 RID: 788 RVA: 0x00019994 File Offset: 0x00017B94
		public static int LowWord(int input)
		{
			return input & 65535;
		}

		// Token: 0x06000315 RID: 789 RVA: 0x000199B0 File Offset: 0x00017BB0
		public static int HighWord(int input)
		{
			return input >> 16;
		}

		// Token: 0x04000197 RID: 407
		public const int WS_CHILD = 1073741824;

		// Token: 0x04000198 RID: 408
		public const int WS_VISIBLE = 268435456;

		// Token: 0x04000199 RID: 409
		public const int WM_MOUSEMOVE = 512;

		// Token: 0x0400019A RID: 410
		public const int WM_LBUTTONDOWN = 513;

		// Token: 0x0400019B RID: 411
		public const int WM_LBUTTONUP = 514;

		// Token: 0x0400019C RID: 412
		public const int WM_LBUTTONDBLCLK = 515;

		// Token: 0x0400019D RID: 413
		public const int WM_RBUTTONDOWN = 516;

		// Token: 0x0400019E RID: 414
		public const int WM_RBUTTONUP = 517;

		// Token: 0x0400019F RID: 415
		public const int WM_RBUTTONDBLCLK = 518;

		// Token: 0x040001A0 RID: 416
		public const int WM_MBUTTONDOWN = 519;

		// Token: 0x040001A1 RID: 417
		public const int WM_MBUTTONUP = 520;

		// Token: 0x040001A2 RID: 418
		public const int WM_MBUTTONDBLCLK = 521;

		// Token: 0x040001A3 RID: 419
		public const int WM_MOUSEWHEEL = 522;

		// Token: 0x040001A4 RID: 420
		public const int WM_XBUTTONDOWN = 523;

		// Token: 0x040001A5 RID: 421
		public const int WM_XBUTTONUP = 524;

		// Token: 0x040001A6 RID: 422
		public const int WM_XBUTTONDBLCLK = 525;

		// Token: 0x040001A7 RID: 423
		public const int WM_MOUSELEAVE = 675;

		// Token: 0x040001A8 RID: 424
		public const int MK_XBUTTON1 = 32;

		// Token: 0x040001A9 RID: 425
		public const int MK_XBUTTON2 = 64;

		// Token: 0x040001AA RID: 426
		public const int IDC_ARROW = 32512;

		// Token: 0x040001AB RID: 427
		public const int IDC_IBEAM = 32513;

		// Token: 0x040001AC RID: 428
		public const int IDC_WAIT = 32514;

		// Token: 0x040001AD RID: 429
		public const int IDC_CROSS = 32515;

		// Token: 0x040001AE RID: 430
		public const int IDC_UPARROW = 32516;

		// Token: 0x040001AF RID: 431
		public const int IDC_SIZENWSE = 32642;

		// Token: 0x040001B0 RID: 432
		public const int IDC_SIZENESW = 32643;

		// Token: 0x040001B1 RID: 433
		public const int IDC_SIZEWE = 32644;

		// Token: 0x040001B2 RID: 434
		public const int IDC_SIZENS = 32645;

		// Token: 0x040001B3 RID: 435
		public const int IDC_SIZEALL = 32646;

		// Token: 0x040001B4 RID: 436
		public const int IDC_NO = 32648;

		// Token: 0x040001B5 RID: 437
		public const int IDC_HAND = 32649;

		// Token: 0x040001B6 RID: 438
		public const int IDC_APPSTARTING = 32650;

		// Token: 0x040001B7 RID: 439
		public const int IDC_HELP = 32651;

		// Token: 0x040001B8 RID: 440
		public const int IDC_ICON = 32641;

		// Token: 0x040001B9 RID: 441
		public const int IDC_SIZE = 32640;

		// Token: 0x040001BA RID: 442
		public const uint TME_LEAVE = 2U;

		// Token: 0x040001BB RID: 443
		public static readonly NativeMethods.WndProc DefaultWindowProc = new NativeMethods.WndProc(NativeMethods.DefWindowProc);

		// Token: 0x0200004D RID: 77
		// (Invoke) Token: 0x06000318 RID: 792
		public delegate IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

		// Token: 0x0200004E RID: 78
		public struct TRACKMOUSEEVENT
		{
			// Token: 0x040001BC RID: 444
			public int cbSize;

			// Token: 0x040001BD RID: 445
			public uint dwFlags;

			// Token: 0x040001BE RID: 446
			public IntPtr hWnd;

			// Token: 0x040001BF RID: 447
			public uint dwHoverTime;
		}

		// Token: 0x0200004F RID: 79
		public struct WNDCLASSEX
		{
			// Token: 0x040001C0 RID: 448
			public uint cbSize;

			// Token: 0x040001C1 RID: 449
			public uint style;

			// Token: 0x040001C2 RID: 450
			[MarshalAs(UnmanagedType.FunctionPtr)]
			public NativeMethods.WndProc lpfnWndProc;

			// Token: 0x040001C3 RID: 451
			public int cbClsExtra;

			// Token: 0x040001C4 RID: 452
			public int cbWndExtra;

			// Token: 0x040001C5 RID: 453
			public IntPtr hInstance;

			// Token: 0x040001C6 RID: 454
			public IntPtr hIcon;

			// Token: 0x040001C7 RID: 455
			public IntPtr hCursor;

			// Token: 0x040001C8 RID: 456
			public IntPtr hbrBackground;

			// Token: 0x040001C9 RID: 457
			public string lpszMenuName;

			// Token: 0x040001CA RID: 458
			public string lpszClassName;

			// Token: 0x040001CB RID: 459
			public IntPtr hIconSm;
		}

		// Token: 0x02000050 RID: 80
		public struct POINT
		{
			// Token: 0x040001CC RID: 460
			public int X;

			// Token: 0x040001CD RID: 461
			public int Y;
		}
	}
}

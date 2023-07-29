using System;
using System.Windows;
using System.Windows.Input;

namespace BrawlerEditor
{
	// Token: 0x02000049 RID: 73
	public class HwndMouseEventArgs : EventArgs
	{
		// Token: 0x1700008E RID: 142
		// (get) Token: 0x060002B7 RID: 695 RVA: 0x000175AC File Offset: 0x000157AC
		// (set) Token: 0x060002B8 RID: 696 RVA: 0x000175C3 File Offset: 0x000157C3
		public MouseButtonState LeftButton { get; private set; }

		// Token: 0x1700008F RID: 143
		// (get) Token: 0x060002B9 RID: 697 RVA: 0x000175CC File Offset: 0x000157CC
		// (set) Token: 0x060002BA RID: 698 RVA: 0x000175E3 File Offset: 0x000157E3
		public MouseButtonState RightButton { get; private set; }

		// Token: 0x17000090 RID: 144
		// (get) Token: 0x060002BB RID: 699 RVA: 0x000175EC File Offset: 0x000157EC
		// (set) Token: 0x060002BC RID: 700 RVA: 0x00017603 File Offset: 0x00015803
		public MouseButtonState MiddleButton { get; private set; }

		// Token: 0x17000091 RID: 145
		// (get) Token: 0x060002BD RID: 701 RVA: 0x0001760C File Offset: 0x0001580C
		// (set) Token: 0x060002BE RID: 702 RVA: 0x00017623 File Offset: 0x00015823
		public MouseButtonState X1Button { get; private set; }

		// Token: 0x17000092 RID: 146
		// (get) Token: 0x060002BF RID: 703 RVA: 0x0001762C File Offset: 0x0001582C
		// (set) Token: 0x060002C0 RID: 704 RVA: 0x00017643 File Offset: 0x00015843
		public MouseButtonState X2Button { get; private set; }

		// Token: 0x17000093 RID: 147
		// (get) Token: 0x060002C1 RID: 705 RVA: 0x0001764C File Offset: 0x0001584C
		// (set) Token: 0x060002C2 RID: 706 RVA: 0x00017663 File Offset: 0x00015863
		public MouseButton? DoubleClickButton { get; private set; }

		// Token: 0x17000094 RID: 148
		// (get) Token: 0x060002C3 RID: 707 RVA: 0x0001766C File Offset: 0x0001586C
		// (set) Token: 0x060002C4 RID: 708 RVA: 0x00017683 File Offset: 0x00015883
		public int WheelDelta { get; private set; }

		// Token: 0x17000095 RID: 149
		// (get) Token: 0x060002C5 RID: 709 RVA: 0x0001768C File Offset: 0x0001588C
		// (set) Token: 0x060002C6 RID: 710 RVA: 0x000176A3 File Offset: 0x000158A3
		public int HorizontalWheelDelta { get; private set; }

		// Token: 0x17000096 RID: 150
		// (get) Token: 0x060002C7 RID: 711 RVA: 0x000176AC File Offset: 0x000158AC
		// (set) Token: 0x060002C8 RID: 712 RVA: 0x000176C3 File Offset: 0x000158C3
		public Point Position { get; private set; }

		// Token: 0x17000097 RID: 151
		// (get) Token: 0x060002C9 RID: 713 RVA: 0x000176CC File Offset: 0x000158CC
		// (set) Token: 0x060002CA RID: 714 RVA: 0x000176E3 File Offset: 0x000158E3
		public Point PreviousPosition { get; private set; }

		// Token: 0x060002CB RID: 715 RVA: 0x000176EC File Offset: 0x000158EC
		public HwndMouseEventArgs(HwndMouseState state)
		{
			this.LeftButton = state.LeftButton;
			this.RightButton = state.RightButton;
			this.MiddleButton = state.MiddleButton;
			this.X1Button = state.X1Button;
			this.X2Button = state.X2Button;
			this.Position = state.Position;
			this.PreviousPosition = state.PreviousPosition;
		}

		// Token: 0x060002CC RID: 716 RVA: 0x0001775D File Offset: 0x0001595D
		public HwndMouseEventArgs(HwndMouseState state, int mouseWheelDelta, int mouseHWheelDelta)
			: this(state)
		{
			this.WheelDelta = mouseWheelDelta;
			this.HorizontalWheelDelta = mouseHWheelDelta;
		}

		// Token: 0x060002CD RID: 717 RVA: 0x00017779 File Offset: 0x00015979
		public HwndMouseEventArgs(HwndMouseState state, MouseButton doubleClickButton)
			: this(state)
		{
			this.DoubleClickButton = new MouseButton?(doubleClickButton);
		}
	}
}

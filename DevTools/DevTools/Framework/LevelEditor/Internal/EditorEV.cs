using System;
using Microsoft.Xna.Framework;

namespace BrawlerEditor
{
	// Token: 0x0200000D RID: 13
	public class EditorEV
	{
		// Token: 0x0400000E RID: 14
		public const int RoomWidth = 960;

		// Token: 0x0400000F RID: 15
		public const int RoomHeight = 540;

		// Token: 0x04000010 RID: 16
		public static Color GRID_LINE_COLOUR = new Color(50, 50, 50);

		// Token: 0x04000011 RID: 17
		public static Color COLLHULL_COLOUR = new Color(180, 0, 180);

		// Token: 0x04000012 RID: 18
		public static Color COLLHULL_JUMPABLE_COLOUR = new Color(240, 165, 60);

		// Token: 0x04000013 RID: 19
		public static Color COLLHULL_SHOOTTHROUGH = new Color(100, 180, 0);

		// Token: 0x04000014 RID: 20
		public static Color COLLHULL_SHOOTTHROUGH_AND_JUMP = new Color(150, 255, 10);

		// Token: 0x04000015 RID: 21
		public static Color TRIGGERHULL_COLOUR = new Color(25, 200, 220);

		// Token: 0x04000016 RID: 22
		public static Color TRANSITIONZONE_LEAVE_COLOUR = new Color(255, 255, 255);

		// Token: 0x04000017 RID: 23
		public static Color TRANSITIONZONE_ARRIVE_COLOUR = new Color(125, 125, 125);

		// Token: 0x04000018 RID: 24
		public static Color ROOMOBJ_COLOUR = new Color(0, 0, 255);

		// Token: 0x04000019 RID: 25
		public static Color ARENAOBJ_COLOUR = new Color(255, 255, 0);

		// Token: 0x0400001A RID: 26
		public static Color NAVMESH_COLOUR = Color.Yellow;

		// Token: 0x0400001B RID: 27
		public static float SELECTED_OBJ_OPACITY = 1f;

		// Token: 0x0400001C RID: 28
		public static float CURRENT_LAYER_OPACITY = 0.6f;

		// Token: 0x0400001D RID: 29
		public static float OTHER_LAYER_OPACITY = 0.15f;
	}
}

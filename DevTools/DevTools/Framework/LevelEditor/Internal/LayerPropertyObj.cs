using System;

namespace BrawlerEditor
{
	// Token: 0x0200001B RID: 27
	public class LayerPropertyObj
	{
		// Token: 0x1700002E RID: 46
		// (get) Token: 0x060000F2 RID: 242 RVA: 0x00006958 File Offset: 0x00004B58
		// (set) Token: 0x060000F3 RID: 243 RVA: 0x00006970 File Offset: 0x00004B70
		public bool applyGaussianBlur
		{
			get
			{
				return this.m_applyGaussianBlur;
			}
			set
			{
				this.m_applyGaussianBlur = value;
			}
		}

		// Token: 0x1700002F RID: 47
		// (get) Token: 0x060000F4 RID: 244 RVA: 0x0000697C File Offset: 0x00004B7C
		// (set) Token: 0x060000F5 RID: 245 RVA: 0x00006994 File Offset: 0x00004B94
		public string layerOverlay
		{
			get
			{
				return this.m_layerTint;
			}
			set
			{
				this.m_layerTint = value;
			}
		}

		// Token: 0x17000030 RID: 48
		// (get) Token: 0x060000F6 RID: 246 RVA: 0x000069A0 File Offset: 0x00004BA0
		// (set) Token: 0x060000F7 RID: 247 RVA: 0x000069B8 File Offset: 0x00004BB8
		public bool addToGameLayer
		{
			get
			{
				return this.m_addToGameLayer;
			}
			set
			{
				this.m_addToGameLayer = value;
			}
		}

		// Token: 0x17000031 RID: 49
		// (get) Token: 0x060000F8 RID: 248 RVA: 0x000069C4 File Offset: 0x00004BC4
		// (set) Token: 0x060000F9 RID: 249 RVA: 0x000069DC File Offset: 0x00004BDC
		public bool sortLayer
		{
			get
			{
				return this.m_sortLayer;
			}
			set
			{
				this.m_sortLayer = value;
			}
		}

		// Token: 0x17000032 RID: 50
		// (get) Token: 0x060000FA RID: 250 RVA: 0x000069E8 File Offset: 0x00004BE8
		// (set) Token: 0x060000FB RID: 251 RVA: 0x00006A00 File Offset: 0x00004C00
		public float parallaxX
		{
			get
			{
				return this.m_parallaxX;
			}
			set
			{
				this.m_parallaxX = value;
			}
		}

		// Token: 0x17000033 RID: 51
		// (get) Token: 0x060000FC RID: 252 RVA: 0x00006A0C File Offset: 0x00004C0C
		// (set) Token: 0x060000FD RID: 253 RVA: 0x00006A24 File Offset: 0x00004C24
		public float parallaxY
		{
			get
			{
				return this.m_parallaxY;
			}
			set
			{
				this.m_parallaxY = value;
			}
		}

		// Token: 0x17000034 RID: 52
		// (get) Token: 0x060000FE RID: 254 RVA: 0x00006A30 File Offset: 0x00004C30
		// (set) Token: 0x060000FF RID: 255 RVA: 0x00006A48 File Offset: 0x00004C48
		public float scrollSpeedX
		{
			get
			{
				return this.m_scrollSpeedX;
			}
			set
			{
				this.m_scrollSpeedX = value;
			}
		}

		// Token: 0x17000035 RID: 53
		// (get) Token: 0x06000100 RID: 256 RVA: 0x00006A54 File Offset: 0x00004C54
		// (set) Token: 0x06000101 RID: 257 RVA: 0x00006A6C File Offset: 0x00004C6C
		public float scrollSpeedY
		{
			get
			{
				return this.m_scrollSpeedY;
			}
			set
			{
				this.m_scrollSpeedY = value;
			}
		}

		// Token: 0x04000076 RID: 118
		private float m_parallaxX;

		// Token: 0x04000077 RID: 119
		private float m_parallaxY;

		// Token: 0x04000078 RID: 120
		private float m_scrollSpeedX;

		// Token: 0x04000079 RID: 121
		private float m_scrollSpeedY;

		// Token: 0x0400007A RID: 122
		private string m_layerTint = "255, 255, 255, 255";

		// Token: 0x0400007B RID: 123
		private bool m_sortLayer = true;

		// Token: 0x0400007C RID: 124
		private bool m_addToGameLayer;

		// Token: 0x0400007D RID: 125
		private bool m_applyGaussianBlur;
	}
}

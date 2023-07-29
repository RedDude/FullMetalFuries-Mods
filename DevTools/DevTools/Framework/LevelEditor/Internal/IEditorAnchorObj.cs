using System;
using Microsoft.Xna.Framework;

namespace BrawlerEditor
{
	// Token: 0x02000005 RID: 5
	public interface IEditorAnchorObj
	{
		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000010 RID: 16
		// (set) Token: 0x06000011 RID: 17
		Vector2 editorAnchor { get; set; }

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000012 RID: 18
		// (set) Token: 0x06000013 RID: 19
		float editorAnchorX { get; set; }

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000014 RID: 20
		// (set) Token: 0x06000015 RID: 21
		float editorAnchorY { get; set; }
	}
}

using System;

namespace BrawlerEditor
{
	// Token: 0x02000010 RID: 16
	public abstract class UndoAction
	{
		// Token: 0x06000050 RID: 80
		public abstract void ExecuteUndo();

		// Token: 0x06000051 RID: 81
		public abstract void ExecuteRedo();

		// Token: 0x06000052 RID: 82 RVA: 0x00003665 File Offset: 0x00001865
		public virtual void Dispose()
		{
			this.m_isDisposed = true;
		}

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x06000053 RID: 83 RVA: 0x00003670 File Offset: 0x00001870
		public bool IsDisposed
		{
			get
			{
				return this.m_isDisposed;
			}
		}

		// Token: 0x04000028 RID: 40
		public UndoAction NextNode = null;

		// Token: 0x04000029 RID: 41
		public UndoAction PreviousNode = null;

		// Token: 0x0400002A RID: 42
		private bool m_isDisposed = false;
	}
}

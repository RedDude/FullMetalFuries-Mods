using System;

namespace BrawlerEditor
{
	// Token: 0x02000043 RID: 67
	public class UndoManager
	{
		// Token: 0x17000082 RID: 130
		// (get) Token: 0x0600025D RID: 605 RVA: 0x00013AE8 File Offset: 0x00011CE8
		public MainWindow MainWindow
		{
			get
			{
				return this.m_mainWindow;
			}
		}

		// Token: 0x0600025E RID: 606 RVA: 0x00013B00 File Offset: 0x00011D00
		public UndoManager(MainWindow mainWindow)
		{
			this.m_mainWindow = mainWindow;
		}

		// Token: 0x0600025F RID: 607 RVA: 0x00013B14 File Offset: 0x00011D14
		public void AddAction(UndoAction action)
		{
			this.RemoveAllNodesPastCurrentNode();
			if (this.m_firstNode == null)
			{
				this.m_firstNode = action;
				this.m_lastNode = action;
			}
			else
			{
				action.PreviousNode = this.m_lastNode;
				this.m_lastNode.NextNode = action;
				this.m_lastNode = action;
			}
			this.m_currentNode = this.m_lastNode;
			this.MainWindow.ChangesMade = true;
		}

		// Token: 0x06000260 RID: 608 RVA: 0x00013B88 File Offset: 0x00011D88
		public void UndoLastAction()
		{
			if (this.m_currentNode != null)
			{
				this.m_currentNode.ExecuteUndo();
				this.m_currentNode = this.m_currentNode.PreviousNode;
				this.MainWindow.gameScreenControl.RefreshCachedSelectionBounds();
			}
		}

		// Token: 0x06000261 RID: 609 RVA: 0x00013BD4 File Offset: 0x00011DD4
		public void RedoLastAction()
		{
			bool flag = false;
			if (this.m_currentNode != null && this.m_currentNode.NextNode != null)
			{
				this.m_currentNode = this.m_currentNode.NextNode;
				this.m_currentNode.ExecuteRedo();
				flag = true;
			}
			else if (this.m_currentNode == null && this.m_firstNode != null)
			{
				this.m_currentNode = this.m_firstNode;
				this.m_currentNode.ExecuteRedo();
				flag = true;
			}
			if (flag)
			{
				this.MainWindow.gameScreenControl.RefreshCachedSelectionBounds();
			}
		}

		// Token: 0x06000262 RID: 610 RVA: 0x00013C78 File Offset: 0x00011E78
		private void RemoveAllNodesPastCurrentNode()
		{
			if (this.m_currentNode != null)
			{
				UndoAction undoAction = this.m_currentNode;
				while (undoAction.NextNode != null)
				{
					undoAction = undoAction.NextNode;
					undoAction.Dispose();
				}
				this.m_lastNode = this.m_currentNode;
			}
			else
			{
				this.m_firstNode = null;
			}
		}

		// Token: 0x06000263 RID: 611 RVA: 0x00013CD4 File Offset: 0x00011ED4
		public void ResetManager()
		{
			this.m_currentNode = this.m_firstNode;
			this.RemoveAllNodesPastCurrentNode();
			if (this.m_currentNode != null)
			{
				this.m_currentNode.Dispose();
			}
			this.m_currentNode = null;
			this.m_firstNode = null;
			this.m_lastNode = null;
		}

		// Token: 0x04000133 RID: 307
		private UndoAction m_firstNode;

		// Token: 0x04000134 RID: 308
		private UndoAction m_lastNode;

		// Token: 0x04000135 RID: 309
		private UndoAction m_currentNode;

		// Token: 0x04000136 RID: 310
		private MainWindow m_mainWindow;
	}
}

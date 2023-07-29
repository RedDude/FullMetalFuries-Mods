using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using CDGEngine;
using Microsoft.Xna.Framework;

namespace BrawlerEditor
{
	// Token: 0x02000029 RID: 41
	public class ScaleToolObj : ToolObj
	{
		// Token: 0x06000173 RID: 371 RVA: 0x0000A760 File Offset: 0x00008960
		public ScaleToolObj(GameScreenControl gameScreenControl)
			: base(gameScreenControl)
		{
			this.m_objsStartingPos = new List<Vector2>();
			this.m_objsStartingScale = new List<Vector2>();
			this.m_toolType = 3;
		}

		// Token: 0x06000174 RID: 372 RVA: 0x0000A78C File Offset: 0x0000898C
		public override void LeftMouseDown(object sender, HwndMouseEventArgs e)
		{
			this.m_selectionRect.X = 0f;
			this.m_selectionRect.Y = 0f;
			this.m_selectionRect.Width = 0f;
			this.m_selectionRect.Height = 0f;
			ObservableCollection<GameObj> selectedObjects = this.m_gameScreenControl.SelectedObjects;
			ObservableCollection<GameObj> layerObjList = this.m_gameScreenControl.LayerObjList;
			Camera2D camera = this.m_gameScreenControl.Camera;
			CDGRect b = new CDGRect((float)e.Position.X * 1f / camera.Zoom + camera.RelTopLeftCorner.X, (float)e.Position.Y * 1f / camera.Zoom + camera.RelTopLeftCorner.Y, 10f, 10f);
			if (this.m_gameScreenControl.MainWindow.ShiftHeld)
			{
				this.m_gameScreenControl.DeselectAllGameObjs();
				this.m_selectionRect.X = (float)(e.Position.X * 1.0 / (double)camera.Zoom + (double)camera.RelTopLeftCorner.X);
				this.m_selectionRect.Y = (float)(e.Position.Y * 1.0 / (double)camera.Zoom + (double)camera.RelTopLeftCorner.Y);
			}
			else
			{
				foreach (GameObj gameObj in layerObjList.Reverse<GameObj>())
				{
					if (CDGMath.Intersects(gameObj.AbsBounds, b))
					{
						bool flag = this.m_gameScreenControl.hitTestPoint(gameObj, new Vector2((float)e.Position.X, (float)e.Position.Y));
						if (flag)
						{
							if (!selectedObjects.Contains(gameObj))
							{
								if (!this.m_gameScreenControl.MainWindow.CtrlHeld)
								{
									this.m_gameScreenControl.DeselectAllGameObjs();
								}
								this.m_gameScreenControl.SelectGameObj(gameObj);
							}
							else if (selectedObjects.Contains(gameObj) && this.m_gameScreenControl.MainWindow.CtrlHeld)
							{
								this.m_gameScreenControl.DeselectGameObj(gameObj);
							}
							break;
						}
					}
				}
			}
			if (CDGMath.Intersects(this.m_gameScreenControl.SelectionBounds, b))
			{
				this.m_selectionRect.X = (float)(e.Position.X * 1.0 / (double)camera.Zoom + (double)camera.RelTopLeftCorner.X);
				this.m_selectionRect.Y = (float)(e.Position.Y * 1.0 / (double)camera.Zoom + (double)camera.RelTopLeftCorner.Y);
				if (this.m_gameScreenControl.SnapToGrid)
				{
					Vector2 vector = base.CalculateSnapTo((float)(e.Position.X * 1.0 / (double)camera.Zoom), (float)(e.Position.Y * 1.0 / (double)camera.Zoom));
					this.m_selectionRect.X = this.m_selectionRect.X + vector.X;
					this.m_selectionRect.Y = this.m_selectionRect.Y + vector.Y;
				}
				this.m_objsStartingScale.Clear();
				this.m_objsStartingPos.Clear();
				foreach (GameObj gameObj in this.m_gameScreenControl.SelectedObjects)
				{
					IScaleableObj scaleableObj = gameObj as IScaleableObj;
					if (scaleableObj != null)
					{
						this.m_objsStartingScale.Add(new Vector2(scaleableObj.Width, scaleableObj.Height));
					}
					else
					{
						this.m_objsStartingScale.Add(gameObj.Scale);
					}
					this.m_objsStartingPos.Add(gameObj.Position);
				}
			}
			else if (!this.m_gameScreenControl.MainWindow.CtrlHeld)
			{
				this.m_gameScreenControl.DeselectAllGameObjs();
			}
			if (this.m_gameScreenControl.SelectedObjects.Count == 1)
			{
				this.m_centrePosition = this.m_gameScreenControl.SelectedObjects[0].Position;
			}
			else
			{
				this.m_centrePosition = new Vector2(this.m_gameScreenControl.SelectionBounds.Center.X, this.m_gameScreenControl.SelectionBounds.Center.Y);
			}
		}

		// Token: 0x06000175 RID: 373 RVA: 0x0000ACBC File Offset: 0x00008EBC
		public override void MouseMove(object sender, HwndMouseEventArgs e)
		{
			Camera2D camera = this.m_gameScreenControl.Camera;
			float num = 0.01f;
			if (this.m_gameScreenControl.SelectedObjects.Count > 0 && e.LeftButton == MouseButtonState.Pressed)
			{
				this.m_selectionRect.Width = (float)Math.Round(e.Position.X * 1.0 / (double)camera.Zoom + (double)camera.RelTopLeftCorner.X - (double)this.m_selectionRect.X, MidpointRounding.AwayFromZero);
				this.m_selectionRect.Height = (float)Math.Round(e.Position.Y * 1.0 / (double)camera.Zoom + (double)camera.RelTopLeftCorner.Y - (double)this.m_selectionRect.Y, MidpointRounding.AwayFromZero);
				if (this.m_gameScreenControl.MainWindow.ShiftHeld)
				{
					this.m_selectionRect.Height = -this.m_selectionRect.Width;
				}
				for (int i = 0; i < this.m_gameScreenControl.SelectedObjects.Count; i++)
				{
					GameObj gameObj = this.m_gameScreenControl.SelectedObjects[i];
					if (gameObj is IScaleableObj)
					{
						if (this.m_gameScreenControl.SnapToGrid)
						{
							Vector2 vector = base.CalculateSnapTo(this.m_selectionRect.Width, this.m_selectionRect.Height);
							this.m_selectionRect.Width = this.m_selectionRect.Width + vector.X;
							this.m_selectionRect.Height = this.m_selectionRect.Height + vector.Y;
							if (this.m_gameScreenControl.MainWindow.ShiftHeld)
							{
								this.m_selectionRect.Height = this.m_selectionRect.Width;
							}
						}
						float num2 = this.m_objsStartingScale[i].X + this.m_selectionRect.Width;
						float num3 = this.m_objsStartingScale[i].Y + this.m_selectionRect.Height;
						if (num2 > 10f)
						{
							(gameObj as IScaleableObj).Width = this.m_objsStartingScale[i].X + this.m_selectionRect.Width;
						}
						if (num3 > 10f)
						{
							(gameObj as IScaleableObj).Height = this.m_objsStartingScale[i].Y + this.m_selectionRect.Height;
						}
					}
					else
					{
						gameObj.ScaleX = this.m_objsStartingScale[i].X + this.m_selectionRect.Width * num;
						gameObj.ScaleY = this.m_objsStartingScale[i].Y - this.m_selectionRect.Height * num;
					}
					float num4 = (this.m_objsStartingPos[i].X - this.m_centrePosition.X) * (1f / this.m_objsStartingScale[i].X);
					float num5 = (this.m_objsStartingPos[i].Y - this.m_centrePosition.Y) * (1f / this.m_objsStartingScale[i].Y);
					gameObj.X = this.m_objsStartingPos[i].X + this.m_selectionRect.Width * num * num4;
					gameObj.Y = this.m_objsStartingPos[i].Y - this.m_selectionRect.Height * num * num5;
				}
				this.m_gameScreenControl.RefreshCachedSelectionBounds();
			}
			else if (this.m_gameScreenControl.SelectedObjects.Count == 0 && e.LeftButton == MouseButtonState.Pressed && this.m_gameScreenControl.MainWindow.ShiftHeld)
			{
				this.m_selectionRect.Width = (float)(e.Position.X * 1.0 / (double)camera.Zoom - (double)this.m_selectionRect.X + (double)camera.RelTopLeftCorner.X);
				this.m_selectionRect.Height = (float)(e.Position.Y * 1.0 / (double)camera.Zoom - (double)this.m_selectionRect.Y + (double)camera.RelTopLeftCorner.Y);
			}
		}

		// Token: 0x06000176 RID: 374 RVA: 0x0000B160 File Offset: 0x00009360
		public override void LeftMouseUp(object sender, HwndMouseEventArgs e)
		{
			float x;
			float width;
			if (this.m_selectionRect.Width < 0f)
			{
				x = this.m_selectionRect.X + this.m_selectionRect.Width;
				width = this.m_selectionRect.Width * -1f;
			}
			else
			{
				x = this.m_selectionRect.X;
				width = this.m_selectionRect.Width;
			}
			float y;
			float height;
			if (this.m_selectionRect.Height < 0f)
			{
				y = this.m_selectionRect.Y + this.m_selectionRect.Height;
				height = this.m_selectionRect.Height * -1f;
			}
			else
			{
				y = this.m_selectionRect.Y;
				height = this.m_selectionRect.Height;
			}
			if (this.m_gameScreenControl.SelectedObjects.Count <= 0 && this.m_gameScreenControl.MainWindow.ShiftHeld)
			{
				this.m_gameScreenControl.DeselectAllGameObjs();
				CDGRect b = new CDGRect(x, y, width, height);
				ObservableCollection<GameObj> layerObjList = this.m_gameScreenControl.LayerObjList;
				foreach (GameObj gameObj in layerObjList)
				{
					if (CDGMath.Intersects(gameObj.AbsBounds, b))
					{
						this.m_gameScreenControl.SelectGameObj(gameObj);
					}
				}
			}
			else if (this.m_gameScreenControl.SelectedObjects.Count > 0)
			{
				IScaleableObj scaleableObj = this.m_gameScreenControl.SelectedObjects[0] as IScaleableObj;
				if ((scaleableObj != null && this.m_objsStartingScale[0].X != scaleableObj.Width && this.m_objsStartingScale[0].Y != scaleableObj.Height) || (scaleableObj == null && this.m_objsStartingScale[0] != this.m_gameScreenControl.SelectedObjects[0].Scale))
				{
					this.m_gameScreenControl.UndoManager.AddAction(new ScaleUndoAction(this.m_gameScreenControl.SelectedObjects, this.m_objsStartingScale, this.m_objsStartingPos));
				}
			}
			this.m_selectionRect = default(CDGRect);
		}

		// Token: 0x06000177 RID: 375 RVA: 0x0000B3F4 File Offset: 0x000095F4
		public override void KeyDown(KeyEventArgs e)
		{
			if (e.Key == Key.Delete || e.Key == Key.Back)
			{
				List<GameObj> list = new List<GameObj>();
				foreach (GameObj item in this.m_gameScreenControl.SelectedObjects)
				{
					list.Add(item);
				}
				if (list.Count > 0)
				{
					this.m_gameScreenControl.DeselectAllGameObjs();
					this.m_gameScreenControl.RemoveGameObjs(list, true);
				}
			}
			base.KeyDown(e);
		}

		// Token: 0x06000178 RID: 376 RVA: 0x0000B4B0 File Offset: 0x000096B0
		public override void Draw(Camera2D camera, float elapsedSeconds)
		{
			if (this.m_gameScreenControl.MainWindow.ShiftHeld && this.m_gameScreenControl.SelectedObjects.Count <= 0)
			{
				float num;
				float num2;
				if (this.m_selectionRect.Width < 0f)
				{
					num = this.m_selectionRect.X + this.m_selectionRect.Width;
					num2 = this.m_selectionRect.Width * -1f;
				}
				else
				{
					num = this.m_selectionRect.X;
					num2 = this.m_selectionRect.Width;
				}
				float num3;
				float num4;
				if (this.m_selectionRect.Height < 0f)
				{
					num3 = this.m_selectionRect.Y + this.m_selectionRect.Height;
					num4 = this.m_selectionRect.Height * -1f;
				}
				else
				{
					num3 = this.m_selectionRect.Y;
					num4 = this.m_selectionRect.Height;
				}
				camera.Draw(StaticTexture.SelectionBoxHorizontal, new Vector2(num, num3), new Rectangle?(new Rectangle(0, 0, (int)num2, 2)), Color.White);
				camera.Draw(StaticTexture.SelectionBoxHorizontal, new Vector2(num, num3 + num4 - 2f), new Rectangle?(new Rectangle(0, 0, (int)num2, 2)), Color.White);
				camera.Draw(StaticTexture.SelectionBoxVertical, new Vector2(num, num3), new Rectangle?(new Rectangle(0, 0, 2, (int)num4)), Color.White);
				camera.Draw(StaticTexture.SelectionBoxVertical, new Vector2(num + num2 - 2f, num3), new Rectangle?(new Rectangle(0, 0, 2, (int)num4)), Color.White);
			}
			base.Draw(camera, elapsedSeconds);
		}

		// Token: 0x040000A6 RID: 166
		private CDGRect m_selectionRect;

		// Token: 0x040000A7 RID: 167
		private List<Vector2> m_objsStartingScale;

		// Token: 0x040000A8 RID: 168
		private List<Vector2> m_objsStartingPos;

		// Token: 0x040000A9 RID: 169
		private Vector2 m_centrePosition;
	}
}

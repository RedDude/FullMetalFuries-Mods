using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using CDGEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BrawlerEditor
{
	// Token: 0x02000037 RID: 55
	public class SelectionToolObj : ToolObj
	{
		// Token: 0x060001F3 RID: 499 RVA: 0x0000E9AC File Offset: 0x0000CBAC
		public SelectionToolObj(GameScreenControl gameScreenControl)
			: base(gameScreenControl)
		{
			this.m_objsStartingPos = new List<Vector2>();
			this.m_toolType = 1;
		}

		// Token: 0x060001F4 RID: 500 RVA: 0x0000EA04 File Offset: 0x0000CC04
		public override void LeftMouseDown(object sender, HwndMouseEventArgs e)
		{
			this.m_horizontallyLocked = false;
			this.m_verticallyLocked = false;
			ObservableCollection<GameObj> selectedObjects = this.m_gameScreenControl.SelectedObjects;
			ObservableCollection<GameObj> layerObjList = this.m_gameScreenControl.LayerObjList;
			Camera2D camera = this.m_gameScreenControl.Camera;
			CDGRect b = new CDGRect((float)e.Position.X * 1f / camera.Zoom + camera.RelTopLeftCorner.X, (float)e.Position.Y * 1f / camera.Zoom + camera.RelTopLeftCorner.Y, 5f, 5f);
			this.m_startingMousePos = new Vector2(b.X, b.Y);
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
				this.m_selectionRect.X = (float)(e.Position.X * 1.0 / (double)camera.Zoom - (double)selectedObjects[0].X + (double)camera.RelTopLeftCorner.X);
				this.m_selectionRect.Y = (float)(e.Position.Y * 1.0 / (double)camera.Zoom - (double)selectedObjects[0].Y + (double)camera.RelTopLeftCorner.Y);
				if (this.m_gameScreenControl.SnapToGrid)
				{
					Vector2 vector = base.CalculateSnapTo((float)(e.Position.X * 1.0 / (double)camera.Zoom), (float)(e.Position.Y * 1.0 / (double)camera.Zoom));
					this.m_selectionRect.X = this.m_selectionRect.X + vector.X;
					this.m_selectionRect.Y = this.m_selectionRect.Y + vector.Y;
				}
				this.m_objsStartingPos.Clear();
				foreach (GameObj gameObj in this.m_gameScreenControl.SelectedObjects)
				{
					this.m_objsStartingPos.Add(gameObj.Position);
				}
			}
			else if (!this.m_gameScreenControl.MainWindow.CtrlHeld)
			{
				this.m_gameScreenControl.DeselectAllGameObjs();
			}
		}

		// Token: 0x060001F5 RID: 501 RVA: 0x0000EE6C File Offset: 0x0000D06C
		public override void MouseMove(object sender, HwndMouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				ObservableCollection<GameObj> selectedObjects = this.m_gameScreenControl.SelectedObjects;
				Camera2D camera = this.m_gameScreenControl.Camera;
				if (!this.m_gameScreenControl.MainWindow.ShiftHeld)
				{
					if (selectedObjects.Count > 0)
					{
						float num = (float)Math.Round(e.Position.X * 1.0 / (double)camera.Zoom + (double)camera.RelTopLeftCorner.X - (double)this.m_selectionRect.X, MidpointRounding.AwayFromZero);
						float num2 = (float)Math.Round(e.Position.Y * 1.0 / (double)camera.Zoom + (double)camera.RelTopLeftCorner.Y - (double)this.m_selectionRect.Y, MidpointRounding.AwayFromZero);
						if (this.m_gameScreenControl.SnapToGrid)
						{
							Vector2 vector = base.CalculateSnapTo(num, num2);
							num += vector.X;
							num2 += vector.Y;
							IEditorAnchorObj editorAnchorObj = selectedObjects[0] as IEditorAnchorObj;
							if (editorAnchorObj != null)
							{
								num -= editorAnchorObj.editorAnchorX;
								num2 -= editorAnchorObj.editorAnchorY;
							}
						}
						num -= selectedObjects[0].X;
						num2 -= selectedObjects[0].Y;
						float num3 = (float)Math.Abs(e.Position.X * 1.0 / (double)camera.Zoom - (double)this.m_startingMousePos.X + (double)camera.RelTopLeftCorner.X);
						float num4 = (float)Math.Abs(e.Position.Y * 1.0 / (double)camera.Zoom - (double)this.m_startingMousePos.Y + (double)camera.RelTopLeftCorner.Y);
						if (this.m_gameScreenControl.MainWindow.ZHeld && !this.m_horizontallyLocked && !this.m_verticallyLocked)
						{
							if (num3 > num4)
							{
								this.m_horizontallyLocked = true;
							}
							else
							{
								this.m_verticallyLocked = true;
							}
						}
						if (this.m_gameScreenControl.MainWindow.ZHeld)
						{
							if (this.m_horizontallyLocked)
							{
								num2 = 0f;
							}
							if (this.m_verticallyLocked)
							{
								num = 0f;
							}
						}
						else
						{
							this.m_horizontallyLocked = false;
							this.m_verticallyLocked = false;
						}
						foreach (GameObj gameObj in selectedObjects)
						{
							gameObj.X += num;
							gameObj.Y += num2;
						}
					}
				}
				else
				{
					this.m_selectionRect.Width = (float)(e.Position.X * 1.0 / (double)camera.Zoom - (double)this.m_selectionRect.X + (double)camera.RelTopLeftCorner.X);
					this.m_selectionRect.Height = (float)(e.Position.Y * 1.0 / (double)camera.Zoom - (double)this.m_selectionRect.Y + (double)camera.RelTopLeftCorner.Y);
				}
				this.m_gameScreenControl.RefreshCachedSelectionBounds();
			}
		}

		// Token: 0x060001F6 RID: 502 RVA: 0x0000F218 File Offset: 0x0000D418
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
			List<Vector2> list = new List<Vector2>();
			for (int i = 0; i < this.m_gameScreenControl.SelectedObjects.Count; i++)
			{
				list.Add(this.m_objsStartingPos[i]);
			}
			if (this.m_gameScreenControl.SelectedObjects.Count > 0 && this.m_gameScreenControl.SelectedObjects[0].Position != this.m_objsStartingPos[0])
			{
				this.m_gameScreenControl.UndoManager.AddAction(new MoveObjsUndoAction(this.m_gameScreenControl.SelectedObjects.ToList<GameObj>(), list));
			}
			if (this.m_gameScreenControl.MainWindow.ShiftHeld)
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
				this.m_objsStartingPos.Clear();
				foreach (GameObj gameObj in this.m_gameScreenControl.SelectedObjects)
				{
					this.m_objsStartingPos.Add(gameObj.Position);
				}
			}
			this.m_selectionRect = default(CDGRect);
		}

		// Token: 0x060001F7 RID: 503 RVA: 0x0000F4D0 File Offset: 0x0000D6D0
		public override void KeyDown(KeyEventArgs e)
		{
			if (e.Key == Key.Delete || e.Key == Key.Back)
			{
				List<GameObj> list = new List<GameObj>();
				foreach (GameObj gameObj in this.m_gameScreenControl.SelectedObjects)
				{
					list.Add(gameObj);
				}
				if (list.Count > 0)
				{
					this.m_gameScreenControl.DeselectAllGameObjs();
					this.m_gameScreenControl.RemoveGameObjs(list, true);
				}
			}
			if (e.Key == Key.I)
			{
				this.ApplyScale(true, e.IsRepeat);
			}
			else if (e.Key == Key.K)
			{
				this.ApplyScale(false, e.IsRepeat);
			}
			if (e.Key == Key.J)
			{
				this.ApplyRotation(false, e.IsRepeat);
			}
			else if (e.Key == Key.L)
			{
				this.ApplyRotation(true, e.IsRepeat);
			}
			if (e.Key == Key.U && !e.IsRepeat)
			{
				foreach (GameObj gameObj in this.m_gameScreenControl.SelectedObjects)
				{
					if (gameObj.Flip == SpriteEffects.None)
					{
						gameObj.Flip = SpriteEffects.FlipHorizontally;
					}
					else
					{
						gameObj.Flip = SpriteEffects.None;
					}
				}
				if (this.m_gameScreenControl.SelectedObjects.Count == 1)
				{
					this.m_gameScreenControl.MainWindow.propertiesStackPanel.ShowProperties(this.m_gameScreenControl.SelectedObjects[0]);
				}
			}
			base.KeyDown(e);
		}

		// Token: 0x060001F8 RID: 504 RVA: 0x0000F6D0 File Offset: 0x0000D8D0
		private void ApplyScale(bool increase, bool isRepeat)
		{
			Vector2 center = this.m_gameScreenControl.SelectionBounds.Center;
			if (!isRepeat)
			{
				this.m_previousPosList.Clear();
				this.m_previousScaleList.Clear();
			}
			float num = 1.01f;
			if (!increase)
			{
				num = 0.99f;
			}
			if (this.m_gameScreenControl.SelectedObjects.Count > 0)
			{
				foreach (GameObj gameObj in this.m_gameScreenControl.SelectedObjects)
				{
					if (!isRepeat)
					{
						this.m_previousScaleList.Add(gameObj.Scale);
						this.m_previousPosList.Add(gameObj.Position);
					}
					Vector2 value = center - gameObj.AbsPosition;
					gameObj.Scale *= num;
					gameObj.Position += value * (1f - num);
				}
				this.m_gameScreenControl.RefreshCachedSelectionBounds();
				if (this.m_gameScreenControl.SelectedObjects.Count == 1)
				{
					this.m_gameScreenControl.MainWindow.propertiesStackPanel.ShowProperties(this.m_gameScreenControl.SelectedObjects[0]);
				}
			}
		}

		// Token: 0x060001F9 RID: 505 RVA: 0x0000F85C File Offset: 0x0000DA5C
		private void ApplyRotation(bool rotateCW, bool isRepeat)
		{
			if (!isRepeat)
			{
				this.m_rotationCentre = this.m_gameScreenControl.SelectionBounds.Center;
				this.m_startingRotationList.Clear();
				this.m_previousPosList.Clear();
				this.m_rotationSpeed = 1f;
			}
			else
			{
				this.m_rotationSpeed *= 1.05f;
			}
			if (this.m_rotationSpeed > 10f)
			{
				this.m_rotationSpeed = 10f;
			}
			if (this.m_gameScreenControl.SelectedObjects.Count > 0)
			{
				int num = 0;
				foreach (GameObj gameObj in this.m_gameScreenControl.SelectedObjects)
				{
					if (!isRepeat)
					{
						this.m_startingRotationList.Add(gameObj.Rotation);
						this.m_previousPosList.Add(gameObj.Position);
					}
					if (rotateCW)
					{
						gameObj.Rotation += this.m_rotationSpeed;
					}
					else
					{
						gameObj.Rotation -= this.m_rotationSpeed;
					}
					gameObj.Rotation = MathHelper.ToRadians(gameObj.Rotation);
					gameObj.Rotation = MathHelper.WrapAngle(gameObj.Rotation);
					gameObj.Rotation = MathHelper.ToDegrees(gameObj.Rotation);
					Vector2 value = CDGMath.RotatedPoint(this.m_previousPosList[num] - this.m_rotationCentre, gameObj.Rotation - this.m_startingRotationList[num]);
					gameObj.Position = value + this.m_rotationCentre;
					num++;
				}
				this.m_gameScreenControl.RefreshCachedSelectionBounds();
				if (this.m_gameScreenControl.SelectedObjects.Count == 1)
				{
					this.m_gameScreenControl.MainWindow.propertiesStackPanel.ShowProperties(this.m_gameScreenControl.SelectedObjects[0]);
				}
			}
		}

		// Token: 0x060001FA RID: 506 RVA: 0x0000FA80 File Offset: 0x0000DC80
		public override void KeyUp(KeyEventArgs e)
		{
			if (e.Key == Key.I || e.Key == Key.K)
			{
				this.m_gameScreenControl.UndoManager.AddAction(new ScaleAllObjsUndoAction(this.m_gameScreenControl.SelectedObjects.ToList<GameObj>(), this.m_previousScaleList, this.m_previousPosList));
			}
			if (e.Key == Key.J || e.Key == Key.L)
			{
				this.m_gameScreenControl.UndoManager.AddAction(new RotateObjUndoAction(this.m_gameScreenControl.SelectedObjects.ToList<GameObj>(), this.m_previousPosList, this.m_startingRotationList));
			}
			base.KeyUp(e);
		}

		// Token: 0x060001FB RID: 507 RVA: 0x0000FB38 File Offset: 0x0000DD38
		public override void Draw(Camera2D camera, float elapsedSeconds)
		{
			if (this.m_gameScreenControl.MainWindow.ShiftHeld)
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

		// Token: 0x040000E9 RID: 233
		private CDGRect m_selectionRect;

		// Token: 0x040000EA RID: 234
		private List<Vector2> m_objsStartingPos;

		// Token: 0x040000EB RID: 235
		private bool m_verticallyLocked;

		// Token: 0x040000EC RID: 236
		private bool m_horizontallyLocked;

		// Token: 0x040000ED RID: 237
		private Vector2 m_startingMousePos;

		// Token: 0x040000EE RID: 238
		private List<Vector2> m_previousScaleList = new List<Vector2>();

		// Token: 0x040000EF RID: 239
		private List<Vector2> m_previousPosList = new List<Vector2>();

		// Token: 0x040000F0 RID: 240
		private Vector2 m_rotationCentre;

		// Token: 0x040000F1 RID: 241
		private float m_rotationSpeed = 1f;

		// Token: 0x040000F2 RID: 242
		private List<float> m_startingRotationList = new List<float>();
	}
}

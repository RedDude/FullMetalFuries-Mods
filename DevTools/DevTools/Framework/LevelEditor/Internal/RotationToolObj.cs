using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using CDGEngine;
using Microsoft.Xna.Framework;

namespace BrawlerEditor
{
	// Token: 0x0200000F RID: 15
	public class RotationToolObj : ToolObj
	{
		// Token: 0x0600004A RID: 74 RVA: 0x000029F7 File Offset: 0x00000BF7
		public RotationToolObj(GameScreenControl gameScreenControl)
			: base(gameScreenControl)
		{
			this.m_toolType = 5;
			this.m_objStartPositions = new List<Vector2>();
			this.m_objStartRotations = new List<float>();
		}

		// Token: 0x0600004B RID: 75 RVA: 0x00002A20 File Offset: 0x00000C20
		public override void LeftMouseDown(object sender, HwndMouseEventArgs e)
		{
			this.m_mouseDown = true;
			this.m_rotation = 0f;
			Camera2D camera = this.m_gameScreenControl.Camera;
			this.m_selectionRect.X = 0f;
			this.m_selectionRect.Y = 0f;
			this.m_selectionRect.Width = 0f;
			this.m_selectionRect.Height = 0f;
			ObservableCollection<GameObj> selectedObjects = this.m_gameScreenControl.SelectedObjects;
			ObservableCollection<GameObj> layerObjList = this.m_gameScreenControl.LayerObjList;
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
				if (this.m_gameScreenControl.SelectedObjects.Count == 1)
				{
					this.m_centrePosition = this.m_gameScreenControl.SelectedObjects[0].AbsPosition;
				}
				else
				{
					this.m_centrePosition = new Vector2(this.m_gameScreenControl.SelectionBounds.Center.X, this.m_gameScreenControl.SelectionBounds.Center.Y);
				}
				this.m_startMousePosition = new Vector2(this.m_selectionRect.X, this.m_selectionRect.Y);
				if (this.m_gameScreenControl.SnapToGrid)
				{
					Vector2 vector = base.CalculateSnapTo((float)(e.Position.X * 1.0 / (double)camera.Zoom), (float)(e.Position.Y * 1.0 / (double)camera.Zoom));
					this.m_selectionRect.X = this.m_selectionRect.X + vector.X;
					this.m_selectionRect.Y = this.m_selectionRect.Y + vector.Y;
				}
				this.m_objStartRotations.Clear();
				this.m_objStartPositions.Clear();
				foreach (GameObj gameObj in this.m_gameScreenControl.SelectedObjects)
				{
					this.m_objStartRotations.Add(gameObj.Rotation);
					this.m_objStartPositions.Add(gameObj.AbsPosition);
				}
			}
			else if (!this.m_gameScreenControl.MainWindow.CtrlHeld)
			{
				this.m_gameScreenControl.DeselectAllGameObjs();
			}
		}

		// Token: 0x0600004C RID: 76 RVA: 0x00002F50 File Offset: 0x00001150
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
			}
			else if (this.m_gameScreenControl.SelectedObjects.Count > 0 && this.m_objStartRotations.Count > 0 && this.m_gameScreenControl.SelectedObjects[0].Rotation != this.m_objStartRotations[0])
			{
				List<GameObj> list = new List<GameObj>();
				List<float> list2 = new List<float>();
				List<Vector2> list3 = new List<Vector2>();
				for (int i = 0; i < this.m_gameScreenControl.SelectedObjects.Count; i++)
				{
					list.Add(this.m_gameScreenControl.SelectedObjects[i]);
					list3.Add(this.m_objStartPositions[i]);
					list2.Add(this.m_objStartRotations[i]);
				}
				this.m_gameScreenControl.UndoManager.AddAction(new RotateObjUndoAction(list, list3, list2));
			}
			this.m_mouseDown = false;
			this.m_selectionRect = default(CDGRect);
		}

		// Token: 0x0600004D RID: 77 RVA: 0x000031E8 File Offset: 0x000013E8
		public override void MouseMove(object sender, HwndMouseEventArgs e)
		{
			Camera2D camera = this.m_gameScreenControl.Camera;
			if (this.m_gameScreenControl.SelectedObjects.Count > 0 && e.LeftButton == MouseButtonState.Pressed)
			{
				float num = (float)(e.Position.X * 1.0 / (double)camera.Zoom + (double)camera.RelTopLeftCorner.X) - this.m_startMousePosition.X;
				num = MathHelper.ToRadians(num / 1f);
				this.m_rotation = MathHelper.ToDegrees(MathHelper.WrapAngle(num));
				for (int i = 0; i < this.m_gameScreenControl.SelectedObjects.Count; i++)
				{
					this.m_gameScreenControl.SelectedObjects[i].Rotation = this.m_objStartRotations[i] + this.m_rotation;
					Vector2 value = CDGMath.RotatedPoint(this.m_objStartPositions[i] - this.m_centrePosition, this.m_rotation);
					this.m_gameScreenControl.SelectedObjects[i].Position = value + this.m_centrePosition;
				}
				this.m_gameScreenControl.RefreshCachedSelectionBounds();
			}
			else if (this.m_gameScreenControl.SelectedObjects.Count == 0 && e.LeftButton == MouseButtonState.Pressed && this.m_gameScreenControl.MainWindow.ShiftHeld)
			{
				this.m_selectionRect.Width = (float)(e.Position.X * 1.0 / (double)camera.Zoom - (double)this.m_selectionRect.X + (double)camera.RelTopLeftCorner.X);
				this.m_selectionRect.Height = (float)(e.Position.Y * 1.0 / (double)camera.Zoom - (double)this.m_selectionRect.Y + (double)camera.RelTopLeftCorner.Y);
			}
		}

		// Token: 0x0600004E RID: 78 RVA: 0x000033FC File Offset: 0x000015FC
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

		// Token: 0x0600004F RID: 79 RVA: 0x000034B8 File Offset: 0x000016B8
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

		// Token: 0x04000021 RID: 33
		private bool m_mouseDown;

		// Token: 0x04000022 RID: 34
		private CDGRect m_selectionRect;

		// Token: 0x04000023 RID: 35
		private Vector2 m_centrePosition;

		// Token: 0x04000024 RID: 36
		private List<Vector2> m_objStartPositions;

		// Token: 0x04000025 RID: 37
		private List<float> m_objStartRotations;

		// Token: 0x04000026 RID: 38
		private float m_rotation;

		// Token: 0x04000027 RID: 39
		private Vector2 m_startMousePosition;
	}
}

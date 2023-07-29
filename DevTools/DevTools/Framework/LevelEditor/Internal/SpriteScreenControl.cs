using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;
using CDGEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpriteSystem2;

namespace BrawlerEditor
{
	// Token: 0x0200002D RID: 45
	public class SpriteScreenControl : XnaControl
	{
		// Token: 0x06000184 RID: 388 RVA: 0x0000BD57 File Offset: 0x00009F57
		public SpriteScreenControl()
		{
			this.m_objList = new List<DisplayObj>();
		}

		// Token: 0x06000185 RID: 389 RVA: 0x0000BD70 File Offset: 0x00009F70
		protected override void loadContent(object sender, GraphicsDeviceEventArgs e)
		{
			base.loadContent(sender, e);
			base.Camera.Position -= base.Camera.RelTopLeftCorner;
			this.m_storedMinY = base.Camera.Y;
			this.LoadAllSpritesheets();
		}

		// Token: 0x06000186 RID: 390 RVA: 0x0000BDC4 File Offset: 0x00009FC4
		public void LoadAllSpritesheets()
		{
			string spritesheetDirectory = ConfigFile.Instance.SpritesheetDirectory;
			if (spritesheetDirectory != "")
			{
				string[] files = Directory.GetFiles(spritesheetDirectory, "*.png", SearchOption.AllDirectories);
				foreach (string spritesheetName in files)
				{
					this.LoadSpritesheet(spritesheetName, true);
				}
			}
		}

		// Token: 0x06000187 RID: 391 RVA: 0x0000BE28 File Offset: 0x0000A028
		public void LoadSpritesheet(string spritesheetName, bool useFullPath = false)
		{
			string text = ConfigFile.Instance.SpritesheetDirectory + spritesheetName;
			if (useFullPath)
			{
				text = spritesheetName;
			}
			string path = text.Substring(0, text.Length - 4) + ".xml";
			if (!File.Exists(path))
			{
				Console.WriteLine("Cannot load " + spritesheetName + " into spritesheet as XML file does not exist");
			}
			else
			{
				SpriteLibrary.LoadSpritesheet(this.GraphicsDevice, text, false);
			}
		}

		// Token: 0x06000188 RID: 392 RVA: 0x0000BE9C File Offset: 0x0000A09C
		public void LoadSprites(string spritesheetName, bool useFullPath = false)
		{
			string text = ConfigFile.Instance.SpritesheetDirectory + spritesheetName;
			if (useFullPath)
			{
				text = spritesheetName;
			}
			string path = text.Substring(0, text.Length - 4) + ".xml";
			if (!File.Exists(path))
			{
				Console.WriteLine("Cannot load " + spritesheetName + " sprite list as XML file does not exist");
			}
			else
			{
				List<string> allContainerNames = SpriteLibrary.GetAllContainerNames(text);
				List<string> allSpriteNames = SpriteLibrary.GetAllSpriteNames(text);
				this.m_objList.Clear();
				float num = 0f;
				float num2 = 0f;
				int num3 = 5;
				if (allContainerNames != null)
				{
					EditorContainerObj editorContainerObj = null;
					foreach (string spriteName in allContainerNames)
					{
						EditorContainerObj editorContainerObj2 = new EditorContainerObj(spriteName);
						this.m_objList.Add(editorContainerObj2);
						float num4;
						if (editorContainerObj2.SpriteBounds.Width > editorContainerObj2.SpriteBounds.Height)
						{
							num4 = 50f / editorContainerObj2.AbsSpriteBounds.Width;
						}
						else
						{
							num4 = 50f / editorContainerObj2.AbsSpriteBounds.Height;
						}
						editorContainerObj2.Scale = new Vector2(num4, num4);
						if (num4 <= 3.4028235E+38f && num4 >= -3.4028235E+38f)
						{
							if (editorContainerObj != null)
							{
								float x = editorContainerObj.AbsSpriteBounds.Right + (editorContainerObj2.X - editorContainerObj2.AbsSpriteBounds.Left) + (float)num3;
								editorContainerObj2.X = x;
								if ((double)editorContainerObj2.AbsSpriteBounds.Right > base.ActualWidth)
								{
									editorContainerObj2.X -= editorContainerObj2.AbsSpriteBounds.Left;
									editorContainerObj2.Y = num + (editorContainerObj2.Y - editorContainerObj2.AbsSpriteBounds.Top) + (float)num3;
									num2 = num;
								}
								else
								{
									editorContainerObj2.Y = num2 + (editorContainerObj2.Y - editorContainerObj2.AbsSpriteBounds.Top) + (float)num3;
								}
							}
							else
							{
								editorContainerObj2.X -= editorContainerObj2.AbsSpriteBounds.Left;
								editorContainerObj2.Y -= editorContainerObj2.AbsSpriteBounds.Top;
							}
							if (editorContainerObj2.AbsSpriteBounds.Bottom > num)
							{
								num = editorContainerObj2.AbsSpriteBounds.Bottom;
							}
							editorContainerObj = editorContainerObj2;
						}
					}
					this.m_containerSpriteDivider = new Rectangle(0, (int)num + 5, (int)base.ActualWidth, 2);
				}
				if (allSpriteNames != null)
				{
					EditorSpriteObj editorSpriteObj = null;
					num += 10f;
					num2 = num;
					foreach (string spriteName2 in allSpriteNames)
					{
						EditorSpriteObj editorSpriteObj2 = new EditorSpriteObj(spriteName2);
						this.m_objList.Add(editorSpriteObj2);
						float num4;
						if (editorSpriteObj2.SpriteBounds.Width > editorSpriteObj2.SpriteBounds.Height)
						{
							num4 = 50f / editorSpriteObj2.AbsSpriteBounds.Width;
						}
						else
						{
							num4 = 50f / editorSpriteObj2.AbsSpriteBounds.Height;
						}
						editorSpriteObj2.Scale = new Vector2(num4, num4);
						if (num4 <= 3.4028235E+38f && num4 >= -3.4028235E+38f)
						{
							if (editorSpriteObj != null)
							{
								float x = editorSpriteObj.AbsSpriteBounds.Right + (editorSpriteObj2.X - editorSpriteObj2.AbsSpriteBounds.Left) + (float)num3;
								editorSpriteObj2.X = x;
								if ((double)editorSpriteObj2.AbsSpriteBounds.Right > base.ActualWidth)
								{
									editorSpriteObj2.X -= editorSpriteObj2.AbsSpriteBounds.Left;
									editorSpriteObj2.Y = num + (editorSpriteObj2.Y - editorSpriteObj2.AbsSpriteBounds.Top) + (float)num3;
									num2 = num;
								}
								else
								{
									editorSpriteObj2.Y = num2 + (editorSpriteObj2.Y - editorSpriteObj2.AbsSpriteBounds.Top) + (float)num3;
								}
							}
							else
							{
								editorSpriteObj2.X -= editorSpriteObj2.AbsSpriteBounds.Left;
								editorSpriteObj2.Y = num2 + (editorSpriteObj2.Y - editorSpriteObj2.AbsSpriteBounds.Top);
							}
							if (editorSpriteObj2.AbsSpriteBounds.Bottom > num)
							{
								num = editorSpriteObj2.AbsSpriteBounds.Bottom;
							}
							editorSpriteObj = editorSpriteObj2;
						}
					}
				}
				this.ResetCameraPos();
			}
		}

		// Token: 0x06000189 RID: 393 RVA: 0x0000C424 File Offset: 0x0000A624
		protected override void LeftButton_MouseDown(object sender, HwndMouseEventArgs e)
		{
			foreach (DisplayObj displayObj in this.m_objList)
			{
				if (CDGMath.Intersects(displayObj.AbsBounds, new CDGRect((float)e.Position.X + base.Camera.RelTopLeftCorner.X, (float)e.Position.Y + base.Camera.RelTopLeftCorner.Y, 1f, 1f)))
				{
					DisplayObj displayObj2 = displayObj.CreateClone() as DisplayObj;
					displayObj2.Scale = Vector2.One;
					displayObj2.Position = base.MainWindow.gameScreenControl.Camera.Position;
					base.MainWindow.gameScreenControl.AddGameObj(displayObj2, true);
					break;
				}
			}
		}

		// Token: 0x0600018A RID: 394 RVA: 0x0000C530 File Offset: 0x0000A730
		protected override void LeftButton_MouseUp(object sender, HwndMouseEventArgs e)
		{
		}

		// Token: 0x0600018B RID: 395 RVA: 0x0000C534 File Offset: 0x0000A734
		protected override void RightButton_MouseDown(object sender, HwndMouseEventArgs e)
		{
			base.CaptureMouse();
			this.m_initialMousePos.X = (float)e.Position.X;
			this.m_initialMousePos.Y = (float)e.Position.Y;
			this.m_initialCameraPos = base.Camera.Position;
		}

		// Token: 0x0600018C RID: 396 RVA: 0x0000C58E File Offset: 0x0000A78E
		protected override void RightButton_MouseUp(object sender, HwndMouseEventArgs e)
		{
			base.ReleaseMouseCapture();
		}

		// Token: 0x0600018D RID: 397 RVA: 0x0000C598 File Offset: 0x0000A798
		protected override void MiddleButton_Scroll(object sender, HwndMouseEventArgs e)
		{
		}

		// Token: 0x0600018E RID: 398 RVA: 0x0000C59C File Offset: 0x0000A79C
		protected override void Mouse_MouseMove(object sender, HwndMouseEventArgs e)
		{
			if (e.RightButton == MouseButtonState.Pressed)
			{
				base.Camera.Y = (float)((double)this.m_initialCameraPos.Y - (e.Position.Y - (double)this.m_initialMousePos.Y) * 1.0 / (double)base.Camera.Zoom);
				if (base.Camera.Y < this.m_storedMinY)
				{
					base.Camera.Y = this.m_storedMinY;
				}
			}
			else
			{
				DisplayObj displayObj = null;
				foreach (DisplayObj displayObj2 in this.m_objList)
				{
					if (CDGMath.Intersects(displayObj2.AbsBounds, new CDGRect((float)e.Position.X + base.Camera.RelTopLeftCorner.X, (float)e.Position.Y + base.Camera.RelTopLeftCorner.Y, 1f, 1f)))
					{
						displayObj = displayObj2;
						break;
					}
				}
				if (displayObj != null)
				{
					base.MainWindow.SetSpriteNameBoxText(displayObj.spriteName);
				}
				else
				{
					base.MainWindow.ClearSpriteNameBox();
				}
			}
		}

		// Token: 0x0600018F RID: 399 RVA: 0x0000C714 File Offset: 0x0000A914
		protected override void Update(Stopwatch gameTime)
		{
		}

		// Token: 0x06000190 RID: 400 RVA: 0x0000C718 File Offset: 0x0000A918
		protected override void Draw(Stopwatch gameTime)
		{
			float elapsedSeconds = (float)gameTime.Elapsed.TotalSeconds;
			this.GraphicsDevice.Clear(Color.Black);
			base.Camera.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, base.Camera.TransformMatrix);
			base.Camera.Draw(StaticTexture.GenericTexture, this.m_containerSpriteDivider, Color.White);
			foreach (DisplayObj displayObj in this.m_objList)
			{
				displayObj.Draw(base.Camera, elapsedSeconds);
			}
			base.Camera.Draw(StaticTexture.GenericTexture, new Rectangle(-5, -5, 10, 10), Color.Red * 0.7f);
			base.Camera.End();
		}

		// Token: 0x06000191 RID: 401 RVA: 0x0000C814 File Offset: 0x0000AA14
		public void ResetCameraPos()
		{
			base.Camera.Y = this.m_storedMinY;
		}

		// Token: 0x040000AD RID: 173
		private const int SPRITE_PIXEL_WIDTH = 50;

		// Token: 0x040000AE RID: 174
		private List<DisplayObj> m_objList;

		// Token: 0x040000AF RID: 175
		private Rectangle m_containerSpriteDivider;

		// Token: 0x040000B0 RID: 176
		private Vector2 m_initialMousePos;

		// Token: 0x040000B1 RID: 177
		private Vector2 m_initialCameraPos;

		// Token: 0x040000B2 RID: 178
		private float m_storedMinY;
	}
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Input;
using Brawler2D;
using CDGEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BrawlerEditor
{
	// Token: 0x02000016 RID: 22
	public class EnemyScreenControl : XnaControl
	{
		// Token: 0x060000AF RID: 175 RVA: 0x0000585C File Offset: 0x00003A5C
		public EnemyScreenControl()
		{
			this.m_enemyList = new List<EditorEnemyObj>();
		}

		// Token: 0x060000B0 RID: 176 RVA: 0x00005874 File Offset: 0x00003A74
		protected override void loadContent(object sender, GraphicsDeviceEventArgs e)
		{
			base.loadContent(sender, e);
			base.Camera.Position -= base.Camera.RelTopLeftCorner;
			this.m_storedMinY = base.Camera.Y;
			this.LoadEnemies();
			this.OrganizeEnemies();
		}

		// Token: 0x060000B1 RID: 177 RVA: 0x000058CC File Offset: 0x00003ACC
		public void LoadEnemies()
		{
			if (ConfigFile.Instance.SpritesheetDirectory != "")
			{
				Assembly assembly = Assembly.GetAssembly(typeof(EnemyObj));
				GameController gameController = new GameController(null);
				gameController.EnemyManager = new EnemyManager(gameController);
				EnemyEV.Initialize();
				foreach (object obj in Enum.GetValues(typeof(EnemyType)))
				{
					Enum @enum = (Enum)obj;
					Type type = assembly.GetType("Brawler2D." + (EnemyType)@enum);
					if (type != null)
					{
						EnemyObj enemyObj = Activator.CreateInstance(type, new object[] { gameController }) as EnemyObj;
						enemyObj.Initialize();
						EditorEnemyObj editorEnemyObj = new EditorEnemyObj(enemyObj.spriteName);
						editorEnemyObj.forceDraw = true;
						editorEnemyObj.TextureColor = enemyObj.originalColour;
						editorEnemyObj.ingameScale = enemyObj.Scale;
						editorEnemyObj.enemyType = (EnemyType)@enum;
						this.m_enemyList.Add(editorEnemyObj);
					}
				}
			}
		}

		// Token: 0x060000B2 RID: 178 RVA: 0x00005A34 File Offset: 0x00003C34
		private void LoadEnemySpritesheets()
		{
			if (ConfigFile.Instance.SpritesheetDirectory != "")
			{
				string[] files = Directory.GetFiles(ConfigFile.Instance.SpritesheetDirectory + "\\Art\\Enemies", "*.png");
				foreach (string spritesheetName in files)
				{
					base.MainWindow.contentScreenControl.LoadSpritesheet(spritesheetName, true);
				}
			}
		}

		// Token: 0x060000B3 RID: 179 RVA: 0x00005AB0 File Offset: 0x00003CB0
		private void OrganizeEnemies()
		{
			float num = 0f;
			float num2 = 0f;
			int num3 = 5;
			float num4 = (float)base.Camera.GraphicsDevice.Viewport.Width;
			ContainerObj containerObj = null;
			foreach (ContainerObj containerObj2 in this.m_enemyList)
			{
				float x = 50f / containerObj2.AbsSpriteBounds.Width;
				float y = 50f / containerObj2.AbsSpriteBounds.Height;
				containerObj2.Scale = new Vector2(x, y);
				if (containerObj != null)
				{
					float x2 = containerObj.AbsSpriteBounds.Right + (containerObj2.X - containerObj2.AbsSpriteBounds.Left) + (float)num3;
					containerObj2.X = x2;
					if (containerObj2.AbsSpriteBounds.Right > num4)
					{
						containerObj2.X -= containerObj2.AbsSpriteBounds.Left;
						containerObj2.Y = num + (containerObj2.Y - containerObj2.AbsSpriteBounds.Top) + (float)num3;
						num2 = num;
					}
					else
					{
						containerObj2.Y = num2 + (containerObj2.Y - containerObj2.AbsSpriteBounds.Top) + (float)num3;
					}
				}
				else
				{
					containerObj2.X -= containerObj2.AbsSpriteBounds.Left;
					containerObj2.Y -= containerObj2.AbsSpriteBounds.Top;
				}
				if (containerObj2.AbsSpriteBounds.Bottom > num)
				{
					num = containerObj2.AbsSpriteBounds.Bottom;
				}
				containerObj = containerObj2;
			}
		}

		// Token: 0x060000B4 RID: 180 RVA: 0x00005CC8 File Offset: 0x00003EC8
		protected override void LeftButton_MouseDown(object sender, HwndMouseEventArgs e)
		{
			foreach (EditorEnemyObj editorEnemyObj in this.m_enemyList)
			{
				if (CDGMath.Intersects(editorEnemyObj.AbsSpriteBounds, new CDGRect((float)e.Position.X + base.Camera.RelTopLeftCorner.X, (float)e.Position.Y + base.Camera.RelTopLeftCorner.Y, 1f, 1f)))
				{
					EditorEnemyObj editorEnemyObj2 = editorEnemyObj.CreateClone() as EditorEnemyObj;
					editorEnemyObj2.Scale = editorEnemyObj2.ingameScale;
					editorEnemyObj2.Position = base.MainWindow.gameScreenControl.Camera.Position - editorEnemyObj2.editorAnchor;
					base.MainWindow.gameScreenControl.AddGameObj(editorEnemyObj2, true);
					break;
				}
			}
		}

		// Token: 0x060000B5 RID: 181 RVA: 0x00005DE0 File Offset: 0x00003FE0
		protected override void LeftButton_MouseUp(object sender, HwndMouseEventArgs e)
		{
		}

		// Token: 0x060000B6 RID: 182 RVA: 0x00005DE4 File Offset: 0x00003FE4
		protected override void RightButton_MouseDown(object sender, HwndMouseEventArgs e)
		{
			base.CaptureMouse();
			this.m_initialMousePos.X = (float)e.Position.X;
			this.m_initialMousePos.Y = (float)e.Position.Y;
			this.m_initialCameraPos = base.Camera.Position;
		}

		// Token: 0x060000B7 RID: 183 RVA: 0x00005E3E File Offset: 0x0000403E
		protected override void RightButton_MouseUp(object sender, HwndMouseEventArgs e)
		{
			base.ReleaseMouseCapture();
		}

		// Token: 0x060000B8 RID: 184 RVA: 0x00005E48 File Offset: 0x00004048
		protected override void MiddleButton_Scroll(object sender, HwndMouseEventArgs e)
		{
		}

		// Token: 0x060000B9 RID: 185 RVA: 0x00005E4C File Offset: 0x0000404C
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
		}

		// Token: 0x060000BA RID: 186 RVA: 0x00005EE4 File Offset: 0x000040E4
		protected override void Update(Stopwatch gameTime)
		{
		}

		// Token: 0x060000BB RID: 187 RVA: 0x00005EE8 File Offset: 0x000040E8
		protected override void Draw(Stopwatch gameTime)
		{
			float elapsedSeconds = (float)gameTime.Elapsed.TotalSeconds;
			this.GraphicsDevice.Clear(Color.White);
			base.Camera.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, base.Camera.TransformMatrix);
			foreach (EditorEnemyObj editorEnemyObj in this.m_enemyList)
			{
				editorEnemyObj.Draw(base.Camera, elapsedSeconds);
			}
			base.Camera.Draw(StaticTexture.GenericTexture, new Rectangle(-5, -5, 10, 10), Color.Red * 0.7f);
			base.Camera.End();
		}

		// Token: 0x04000065 RID: 101
		private const int SPRITE_PIXEL_WIDTH = 50;

		// Token: 0x04000066 RID: 102
		private List<EditorEnemyObj> m_enemyList;

		// Token: 0x04000067 RID: 103
		private Vector2 m_initialMousePos;

		// Token: 0x04000068 RID: 104
		private Vector2 m_initialCameraPos;

		// Token: 0x04000069 RID: 105
		private float m_storedMinY;
	}
}

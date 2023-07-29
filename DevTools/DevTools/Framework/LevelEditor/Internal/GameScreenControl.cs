using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Input;
using CDGEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpriteSystem2;

namespace BrawlerEditor
{
	// Token: 0x02000046 RID: 70
	public class GameScreenControl : XnaControl
	{
		// Token: 0x17000083 RID: 131
		// (get) Token: 0x0600026C RID: 620 RVA: 0x00013FFC File Offset: 0x000121FC
		// (set) Token: 0x0600026D RID: 621 RVA: 0x00014018 File Offset: 0x00012218
		public bool SnapToGrid
		{
			get
			{
				return ConfigFile.Instance.SnapToGrid;
			}
			set
			{
				ConfigFile.Instance.SnapToGrid = value;
			}
		}

		// Token: 0x17000084 RID: 132
		// (get) Token: 0x0600026E RID: 622 RVA: 0x00014028 File Offset: 0x00012228
		// (set) Token: 0x0600026F RID: 623 RVA: 0x00014044 File Offset: 0x00012244
		public int GridUnitSize
		{
			get
			{
				return ConfigFile.Instance.GridUnitSize;
			}
			set
			{
				ConfigFile.Instance.GridUnitSize = value;
			}
		}

		// Token: 0x17000085 RID: 133
		// (get) Token: 0x06000270 RID: 624 RVA: 0x00014054 File Offset: 0x00012254
		// (set) Token: 0x06000271 RID: 625 RVA: 0x00014070 File Offset: 0x00012270
		public bool GridVisible
		{
			get
			{
				return ConfigFile.Instance.GridVisible;
			}
			set
			{
				ConfigFile.Instance.GridVisible = value;
			}
		}

		// Token: 0x17000086 RID: 134
		// (get) Token: 0x06000272 RID: 626 RVA: 0x00014080 File Offset: 0x00012280
		public CDGRect SelectionBounds
		{
			get
			{
				if (!this.m_useCachedSelectionBounds)
				{
					this.m_useCachedSelectionBounds = true;
					if (this.SelectedObjects.Count < 1)
					{
						this.m_selectionBounds = default(CDGRect);
					}
					float num = float.MaxValue;
					float num2 = float.MaxValue;
					float num3 = float.MinValue;
					float num4 = float.MinValue;
					foreach (GameObj gameObj in this.SelectedObjects)
					{
						if (gameObj.AbsBounds.Left < num)
						{
							num = gameObj.AbsBounds.Left;
						}
						if (gameObj.AbsBounds.Right > num3)
						{
							num3 = gameObj.AbsBounds.Right;
						}
						if (gameObj.AbsBounds.Top < num2)
						{
							num2 = gameObj.AbsBounds.Top;
						}
						if (gameObj.AbsBounds.Bottom > num4)
						{
							num4 = gameObj.AbsBounds.Bottom;
						}
					}
					this.m_selectionBounds = new CDGRect(num, num2, num3 - num, num4 - num2);
				}
				return this.m_selectionBounds;
			}
		}

		// Token: 0x17000087 RID: 135
		// (get) Token: 0x06000273 RID: 627 RVA: 0x0001420C File Offset: 0x0001240C
		public ObservableCollection<GameObj> SelectedObjects
		{
			get
			{
				return this.m_selectedObjs;
			}
		}

		// Token: 0x17000088 RID: 136
		// (get) Token: 0x06000274 RID: 628 RVA: 0x00014224 File Offset: 0x00012424
		public ObservableCollection<GameObj> LayerObjList
		{
			get
			{
				int selectedIndex = base.MainWindow.layerTabControl.SelectedIndex;
				ObservableCollection<GameObj> result;
				if (selectedIndex > -1)
				{
					result = this.m_layerList[base.MainWindow.layerTabControl.SelectedIndex];
				}
				else
				{
					result = this.m_layerList[0];
				}
				return result;
			}
		}

		// Token: 0x17000089 RID: 137
		// (get) Token: 0x06000275 RID: 629 RVA: 0x0001427C File Offset: 0x0001247C
		public ObservableCollection<GameObj> GameLayerObjList
		{
			get
			{
				int gameLayerIndex = base.MainWindow.layerTabControl.GameLayerIndex;
				ObservableCollection<GameObj> result;
				if (gameLayerIndex < 0)
				{
					result = null;
				}
				else
				{
					result = this.m_layerList[gameLayerIndex];
				}
				return result;
			}
		}

		// Token: 0x1700008A RID: 138
		// (get) Token: 0x06000276 RID: 630 RVA: 0x000142BC File Offset: 0x000124BC
		public List<ObservableCollection<GameObj>> LayerList
		{
			get
			{
				return this.m_layerList;
			}
		}

		// Token: 0x1700008B RID: 139
		// (get) Token: 0x06000277 RID: 631 RVA: 0x000142D4 File Offset: 0x000124D4
		public UndoManager UndoManager
		{
			get
			{
				return this.m_undoManager;
			}
		}

		// Token: 0x06000278 RID: 632 RVA: 0x000142EC File Offset: 0x000124EC
		public GameScreenControl()
		{
			this.m_selectedObjs = new ObservableCollection<GameObj>();
			this.m_selectedObjs.CollectionChanged += this.SelectedObjs_CollectionChanged;
			this.m_gameLayer = new ObservableCollection<GameObj>();
			this.m_layerList = new List<ObservableCollection<GameObj>>();
			this.m_layerList.Add(this.m_gameLayer);
			this.m_objStartPos = new List<Vector2>();
		}

		// Token: 0x06000279 RID: 633 RVA: 0x00014364 File Offset: 0x00012564
		public void SelectedObjs_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			this.m_useCachedSelectionBounds = false;
			RoomObj roomObj = null;
			foreach (GameObj gameObj in this.SelectedObjects)
			{
				RoomObj roomObj2 = gameObj as RoomObj;
				if (roomObj2 != null)
				{
					roomObj = roomObj2;
					break;
				}
			}
			if (roomObj != null)
			{
				base.MainWindow.propertiesStackPanel.ShowProperties(roomObj);
				if (base.MainWindow.CodeDialog != null)
				{
					base.MainWindow.CodeDialog.codeScrollViewer.ShowCode(roomObj);
				}
			}
			else if (this.SelectedObjects.Count == 1)
			{
				base.MainWindow.propertiesStackPanel.ShowProperties(this.SelectedObjects[0]);
				if (base.MainWindow.CodeDialog != null)
				{
					base.MainWindow.CodeDialog.codeScrollViewer.ShowCode(this.SelectedObjects[0]);
				}
			}
			else if (this.SelectedObjects.Count > 1)
			{
				if (base.MainWindow.CodeDialog != null)
				{
					base.MainWindow.CodeDialog.codeScrollViewer.ClearPanel();
				}
				base.MainWindow.propertiesStackPanel.ShowProperties(this.SelectedObjects);
			}
			else
			{
				if (base.MainWindow.CodeDialog != null)
				{
					base.MainWindow.CodeDialog.codeScrollViewer.ClearPanel();
				}
				base.MainWindow.propertiesStackPanel.ClearPanel(true);
			}
		}

		// Token: 0x0600027A RID: 634 RVA: 0x00014534 File Offset: 0x00012734
		public void FocusCameraOnPlayerStart()
		{
			foreach (ObservableCollection<GameObj> observableCollection in this.m_layerList)
			{
				foreach (GameObj gameObj in observableCollection)
				{
					PlayerStartObj playerStartObj = gameObj as PlayerStartObj;
					if (playerStartObj != null && !playerStartObj.isDebugOnly)
					{
						base.Camera.Position = playerStartObj.Position;
						return;
					}
				}
			}
		}

		// Token: 0x0600027B RID: 635 RVA: 0x00014604 File Offset: 0x00012804
		public void RefreshCachedSelectionBounds()
		{
			this.m_useCachedSelectionBounds = false;
		}

		// Token: 0x0600027C RID: 636 RVA: 0x00014610 File Offset: 0x00012810
		protected override void loadContent(object sender, GraphicsDeviceEventArgs e)
		{
			base.loadContent(sender, e);
			this.m_premultiplyAlphaShader = this.Content.Load<Effect>("PremultiplyAlphaShader");
			StaticTexture.GenericTexture = new Texture2D(this.GraphicsDevice, 1, 1);
			StaticTexture.GenericTexture.SetData<Color>(new Color[] { Color.White });
			StaticTexture.GenericFont = this.Content.Load<SpriteFont>("Arial14");
			FileStream fileStream = new FileStream("Images/MarkerIconLarge.png", FileMode.Open);
			StaticTexture.MarkerTexture = Texture2D.FromStream(this.GraphicsDevice, fileStream);
			fileStream.Close();
			fileStream = new FileStream("Images/PlayerSpawnIcon.png", FileMode.Open);
			StaticTexture.PlayerSpawnTexture = Texture2D.FromStream(this.GraphicsDevice, fileStream);
			fileStream.Close();
			fileStream = new FileStream("Images/SelectionBorderWidth.gif", FileMode.Open);
			StaticTexture.SelectionBoxHorizontal = Texture2D.FromStream(this.GraphicsDevice, fileStream);
			fileStream.Close();
			fileStream = new FileStream("Images/SelectionBorderHeight.gif", FileMode.Open);
			StaticTexture.SelectionBoxVertical = Texture2D.FromStream(this.GraphicsDevice, fileStream);
			fileStream.Close();
			fileStream = new FileStream("Images/ArrowIcon.png", FileMode.Open);
			StaticTexture.ArrowTexture = Texture2D.FromStream(this.GraphicsDevice, fileStream);
			fileStream.Close();
			this.m_collHullToolObj = new CollHullToolObj(this);
			this.m_selectionToolObj = new SelectionToolObj(this);
			this.m_scaleToolObj = new ScaleToolObj(this);
			this.m_markerToolObj = new MarkerToolObj(this);
			this.m_roomToolObj = new RoomToolObj(this);
			this.m_playerStartToolObj = new PlayerStartToolObj(this);
			this.m_rotationToolObj = new RotationToolObj(this);
			this.m_undoManager = new UndoManager(base.MainWindow);
			this.m_viewportStartingWidth = (float)this.GraphicsDevice.Viewport.Width;
			this.m_viewportStartingHeight = (float)this.GraphicsDevice.Viewport.Height;
		}

		// Token: 0x0600027D RID: 637 RVA: 0x000147D8 File Offset: 0x000129D8
		protected override void MiddleButton_Scroll(object sender, HwndMouseEventArgs e)
		{
			if ((base.MainWindow.CodeDialog == null || !base.MainWindow.CodeDialog.codeScrollViewer.TextBoxHasFocus) && !base.MainWindow.outputScrollViewer.TextBoxHasFocus)
			{
				if (e.WheelDelta > 0)
				{
					base.Camera.Zoom += 0.1f;
				}
				else
				{
					base.Camera.Zoom -= 0.1f;
				}
				if (base.Camera.Zoom > 8f)
				{
					base.Camera.Zoom = 8f;
				}
				else if (base.Camera.Zoom < 0.1f)
				{
					base.Camera.Zoom = 0.1f;
				}
				base.MainWindow.zoomTextBlock.Text = "Zoom: " + base.Camera.Zoom.ToString("0.00");
			}
		}

		// Token: 0x0600027E RID: 638 RVA: 0x000148F4 File Offset: 0x00012AF4
		protected override void LeftButton_MouseDown(object sender, HwndMouseEventArgs e)
		{
			base.CaptureMouse();
			if (this.m_currentToolObj != null)
			{
				this.m_currentToolObj.LeftMouseDown(sender, e);
			}
		}

		// Token: 0x0600027F RID: 639 RVA: 0x00014924 File Offset: 0x00012B24
		protected override void LeftButton_MouseUp(object sender, HwndMouseEventArgs e)
		{
			base.ReleaseMouseCapture();
			if (this.m_currentToolObj != null)
			{
				this.m_currentToolObj.LeftMouseUp(sender, e);
			}
			if (this.SelectedObjects.Count > 0)
			{
				base.MainWindow.propertiesStackPanel.RefreshProperties();
			}
		}

		// Token: 0x06000280 RID: 640 RVA: 0x0001497C File Offset: 0x00012B7C
		protected override void RightButton_MouseDown(object sender, HwndMouseEventArgs e)
		{
			base.CaptureMouse();
			this.m_initialMousePos.X = (float)e.Position.X;
			this.m_initialMousePos.Y = (float)e.Position.Y;
			this.m_initialCameraPos = base.Camera.Position;
		}

		// Token: 0x06000281 RID: 641 RVA: 0x000149D6 File Offset: 0x00012BD6
		protected override void RightButton_MouseUp(object sender, HwndMouseEventArgs e)
		{
			base.ReleaseMouseCapture();
		}

		// Token: 0x06000282 RID: 642 RVA: 0x000149E0 File Offset: 0x00012BE0
		protected override void Mouse_MouseMove(object sender, HwndMouseEventArgs e)
		{
			if (e.RightButton == MouseButtonState.Pressed)
			{
				base.Camera.X = (float)((double)this.m_initialCameraPos.X - (e.Position.X - (double)this.m_initialMousePos.X) * 1.0 / (double)base.Camera.Zoom);
				base.Camera.Y = (float)((double)this.m_initialCameraPos.Y - (e.Position.Y - (double)this.m_initialMousePos.Y) * 1.0 / (double)base.Camera.Zoom);
				base.MainWindow.coordinatesTextBlock.Text = string.Concat(new object[]
				{
					"(",
					(int)base.Camera.X,
					", ",
					(int)base.Camera.Y,
					")"
				});
			}
			else if (this.m_currentToolObj != null)
			{
				this.m_currentToolObj.MouseMove(sender, e);
			}
		}

		// Token: 0x06000283 RID: 643 RVA: 0x00014B18 File Offset: 0x00012D18
		protected override void OnKeyDown(KeyEventArgs e)
		{
			if (this.m_currentToolObj != null)
			{
				this.m_currentToolObj.KeyDown(e);
			}
			base.OnKeyDown(e);
		}

		// Token: 0x06000284 RID: 644 RVA: 0x00014B48 File Offset: 0x00012D48
		protected override void OnKeyUp(KeyEventArgs e)
		{
			if (this.m_currentToolObj != null)
			{
				this.m_currentToolObj.KeyUp(e);
			}
			base.OnKeyUp(e);
		}

		// Token: 0x06000285 RID: 645 RVA: 0x00014B78 File Offset: 0x00012D78
		public void FlipAllSelectedObjects()
		{
			foreach (GameObj gameObj in this.SelectedObjects)
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
			base.MainWindow.propertiesStackPanel.RefreshProperties();
		}

		// Token: 0x06000286 RID: 646 RVA: 0x00014C3C File Offset: 0x00012E3C
		public void ShiftSelectedObjectsUp()
		{
			if (this.m_selectedObjs.Count > 0)
			{
				ObservableCollection<GameObj> activeLayer = this.LayerObjList;
				List<GameObj> list = new List<GameObj>();
				list.AddRange(this.SelectedObjects);
				list.Sort((GameObj obj1, GameObj obj2) => activeLayer.IndexOf(obj2).CompareTo(activeLayer.IndexOf(obj1)));
				foreach (GameObj item in list)
				{
					int num = activeLayer.IndexOf(item);
					if (num >= activeLayer.Count - 1)
					{
						break;
					}
					activeLayer.Remove(item);
					activeLayer.Insert(num + 1, item);
				}
				this.UndoManager.AddAction(new ShiftObjectsUpUndoAction(this.SelectedObjects, this.LayerObjList));
			}
		}

		// Token: 0x06000287 RID: 647 RVA: 0x00014D80 File Offset: 0x00012F80
		public void ShiftSelectedObjectsDown()
		{
			if (this.m_selectedObjs.Count > 0)
			{
				ObservableCollection<GameObj> activeLayer = this.LayerObjList;
				List<GameObj> list = new List<GameObj>();
				list.AddRange(this.SelectedObjects);
				list.Sort((GameObj obj1, GameObj obj2) => activeLayer.IndexOf(obj1).CompareTo(activeLayer.IndexOf(obj2)));
				foreach (GameObj item in list)
				{
					int num = activeLayer.IndexOf(item);
					if (num <= 0)
					{
						break;
					}
					activeLayer.Remove(item);
					activeLayer.Insert(num - 1, item);
				}
				this.UndoManager.AddAction(new ShiftObjectsDownUndoAction(this.SelectedObjects, this.LayerObjList));
			}
		}

		// Token: 0x06000288 RID: 648 RVA: 0x00014E7C File Offset: 0x0001307C
		public void ArrowKeysHandler(KeyEventArgs e)
		{
			if ((e.Key == Key.Left || e.Key == Key.Right || e.Key == Key.Up || e.Key == Key.Down) && this.SelectedObjects.Count > 0)
			{
				if (e.IsDown)
				{
					if (!e.IsRepeat)
					{
						this.m_objStartPos.Clear();
						foreach (GameObj gameObj in this.SelectedObjects)
						{
							this.m_objStartPos.Add(gameObj.Position);
						}
					}
					int num = 0;
					int num2 = 0;
					if (e.Key == Key.Left)
					{
						num = -1;
					}
					else if (e.Key == Key.Right)
					{
						num = 1;
					}
					if (e.Key == Key.Up)
					{
						num2 = -1;
					}
					else if (e.Key == Key.Down)
					{
						num2 = 1;
					}
					if (this.SnapToGrid)
					{
						num *= this.GridUnitSize;
						num2 *= this.GridUnitSize;
					}
					foreach (GameObj gameObj in this.SelectedObjects)
					{
						gameObj.X += (float)num;
						gameObj.Y += (float)num2;
					}
					this.RefreshCachedSelectionBounds();
					base.MainWindow.propertiesStackPanel.RefreshProperties();
				}
				else if (e.IsUp)
				{
					this.UndoManager.AddAction(new MoveObjsUndoAction(this.SelectedObjects.ToList<GameObj>(), this.m_objStartPos));
				}
			}
		}

		// Token: 0x06000289 RID: 649 RVA: 0x0001507C File Offset: 0x0001327C
		public void SetTool(byte toolType)
		{
			if (toolType != 1 && toolType != 3 && toolType != 5)
			{
				this.DeselectAllGameObjs();
			}
			switch (toolType)
			{
			case 0:
				this.m_currentToolObj = null;
				break;
			case 1:
				this.m_currentToolObj = this.m_selectionToolObj;
				break;
			case 2:
				this.m_currentToolObj = this.m_collHullToolObj;
				break;
			case 3:
				this.m_currentToolObj = this.m_scaleToolObj;
				break;
			case 4:
				this.m_currentToolObj = this.m_roomToolObj;
				break;
			case 5:
				this.m_currentToolObj = this.m_rotationToolObj;
				break;
			case 6:
				this.m_currentToolObj = this.m_markerToolObj;
				break;
			case 7:
				this.m_playerStartToolObj.setDebugOnly = false;
				this.m_currentToolObj = this.m_playerStartToolObj;
				break;
			case 8:
				this.m_playerStartToolObj.setDebugOnly = true;
				this.m_currentToolObj = this.m_playerStartToolObj;
				break;
			}
		}

		// Token: 0x0600028A RID: 650 RVA: 0x0001516C File Offset: 0x0001336C
		public void AddGameObj(GameObj obj, bool addUndoAction)
		{
			if (!this.LayerObjList.Contains(obj))
			{
				if (obj is IOpacityObj)
				{
					(obj as IOpacityObj).editorOpacity = EditorEV.CURRENT_LAYER_OPACITY;
				}
				else
				{
					obj.Opacity = EditorEV.CURRENT_LAYER_OPACITY;
				}
				if (obj is RoomObj)
				{
					int num = -1;
					for (int i = 0; i < this.LayerObjList.Count; i++)
					{
						RoomObj roomObj = this.LayerObjList[i] as RoomObj;
						if (roomObj != null && !roomObj.IsArenaZone)
						{
							num = i;
						}
					}
					if (num >= this.LayerObjList.Count)
					{
						this.LayerObjList.Add(obj);
					}
					else
					{
						this.LayerObjList.Insert(num + 1, obj);
					}
				}
				else if (obj is EditorCollHullObj)
				{
					int num = -1;
					for (int i = 0; i < this.LayerObjList.Count; i++)
					{
						EditorCollHullObj editorCollHullObj = this.LayerObjList[i] as EditorCollHullObj;
						RoomObj roomObj = this.LayerObjList[i] as RoomObj;
						if (editorCollHullObj != null || roomObj != null)
						{
							num = i;
						}
					}
					if (num >= this.LayerObjList.Count)
					{
						this.LayerObjList.Add(obj);
					}
					else
					{
						this.LayerObjList.Insert(num + 1, obj);
					}
				}
				else if (obj is PlayerStartObj)
				{
					bool isDebugOnly = (obj as PlayerStartObj).isDebugOnly;
					GameObj gameObj = null;
					int objRemovedLayer = 0;
					for (int i = 0; i < this.m_layerList.Count; i++)
					{
						ObservableCollection<GameObj> observableCollection = this.m_layerList[i];
						for (int j = 0; j < observableCollection.Count; j++)
						{
							PlayerStartObj playerStartObj = observableCollection[j] as PlayerStartObj;
							if (playerStartObj != null && playerStartObj.isDebugOnly == isDebugOnly)
							{
								gameObj = playerStartObj;
								objRemovedLayer = i;
								observableCollection.Remove(playerStartObj);
							}
							if (gameObj != null)
							{
								break;
							}
						}
						if (gameObj != null)
						{
							break;
						}
					}
					this.LayerObjList.Add(obj);
					if (addUndoAction)
					{
						this.UndoManager.AddAction(new AddPlayerStartUndoAction(obj, gameObj, objRemovedLayer, this));
					}
				}
				else
				{
					this.LayerObjList.Add(obj);
				}
				if (addUndoAction && !(obj is PlayerStartObj))
				{
					this.UndoManager.AddAction(new AddObjUndoAction(obj, this));
				}
			}
			else
			{
				Console.WriteLine("Cannot add object.  Already added");
			}
		}

		// Token: 0x0600028B RID: 651 RVA: 0x00015444 File Offset: 0x00013644
		public void AddGameObjs(List<GameObj> objList, bool addUndoAction)
		{
			foreach (GameObj gameObj in objList)
			{
				if (!this.LayerObjList.Contains(gameObj))
				{
					if (gameObj is IOpacityObj)
					{
						(gameObj as IOpacityObj).editorOpacity = EditorEV.CURRENT_LAYER_OPACITY;
					}
					else
					{
						gameObj.Opacity = EditorEV.CURRENT_LAYER_OPACITY;
					}
					if (gameObj is RoomObj)
					{
						int num = -1;
						for (int i = 0; i < this.LayerObjList.Count; i++)
						{
							RoomObj roomObj = this.LayerObjList[i] as RoomObj;
							if (roomObj != null && !roomObj.IsArenaZone)
							{
								num = i;
							}
						}
						if (num >= this.LayerObjList.Count)
						{
							this.LayerObjList.Add(gameObj);
						}
						else
						{
							this.LayerObjList.Insert(num + 1, gameObj);
						}
					}
					else if (gameObj is EditorCollHullObj)
					{
						int num = -1;
						for (int i = 0; i < this.LayerObjList.Count; i++)
						{
							EditorCollHullObj editorCollHullObj = this.LayerObjList[i] as EditorCollHullObj;
							RoomObj roomObj = this.LayerObjList[i] as RoomObj;
							if (editorCollHullObj != null || roomObj != null)
							{
								num = i;
							}
						}
						if (num >= this.LayerObjList.Count)
						{
							this.LayerObjList.Add(gameObj);
						}
						else
						{
							this.LayerObjList.Insert(num + 1, gameObj);
						}
					}
					else
					{
						this.LayerObjList.Add(gameObj);
					}
				}
				else
				{
					Console.WriteLine("Cannot add object.  Already added");
				}
			}
			if (addUndoAction)
			{
				this.UndoManager.AddAction(new AddObjUndoAction(objList, this));
			}
		}

		// Token: 0x0600028C RID: 652 RVA: 0x00015664 File Offset: 0x00013864
		public void AddGameObjInLayer(GameObj obj, int indexLayer, bool addUndoAction)
		{
			if (indexLayer >= 0 && indexLayer < this.LayerList.Count)
			{
				ObservableCollection<GameObj> observableCollection = this.m_layerList[indexLayer];
				if (!observableCollection.Contains(obj))
				{
					if (obj is IOpacityObj)
					{
						(obj as IOpacityObj).editorOpacity = EditorEV.CURRENT_LAYER_OPACITY;
					}
					else
					{
						obj.Opacity = EditorEV.CURRENT_LAYER_OPACITY;
					}
					if (obj is RoomObj)
					{
						int num = -1;
						for (int i = 0; i < observableCollection.Count; i++)
						{
							RoomObj roomObj = observableCollection[i] as RoomObj;
							if (roomObj != null && !roomObj.IsArenaZone)
							{
								num = i;
							}
						}
						if (num >= observableCollection.Count)
						{
							observableCollection.Add(obj);
						}
						else
						{
							observableCollection.Insert(num + 1, obj);
						}
					}
					else if (obj is EditorCollHullObj)
					{
						int num = -1;
						for (int i = 0; i < observableCollection.Count; i++)
						{
							EditorCollHullObj editorCollHullObj = observableCollection[i] as EditorCollHullObj;
							RoomObj roomObj = observableCollection[i] as RoomObj;
							if (editorCollHullObj != null || roomObj != null)
							{
								num = i;
							}
						}
						if (num >= observableCollection.Count)
						{
							observableCollection.Add(obj);
						}
						else
						{
							observableCollection.Insert(num + 1, obj);
						}
					}
					else
					{
						observableCollection.Add(obj);
					}
					if (addUndoAction)
					{
						this.UndoManager.AddAction(new AddObjUndoAction(obj, this));
					}
				}
				else
				{
					Console.WriteLine("Cannot add object.  Already added");
				}
			}
			else
			{
				Console.WriteLine("Cannot add object at index: " + indexLayer + " as it is out of bounds.");
			}
		}

		// Token: 0x0600028D RID: 653 RVA: 0x00015834 File Offset: 0x00013A34
		public void AddGameObjsInLayer(List<GameObj> objList, int indexLayer, bool addUndoAction)
		{
			if (indexLayer >= 0 && indexLayer < this.LayerList.Count)
			{
				ObservableCollection<GameObj> observableCollection = this.m_layerList[indexLayer];
				foreach (GameObj gameObj in objList)
				{
					if (!observableCollection.Contains(gameObj))
					{
						if (gameObj is IOpacityObj)
						{
							(gameObj as IOpacityObj).editorOpacity = EditorEV.CURRENT_LAYER_OPACITY;
						}
						else
						{
							gameObj.Opacity = EditorEV.CURRENT_LAYER_OPACITY;
						}
						if (gameObj is RoomObj)
						{
							int num = -1;
							for (int i = 0; i < observableCollection.Count; i++)
							{
								RoomObj roomObj = observableCollection[i] as RoomObj;
								if (roomObj != null && !roomObj.IsArenaZone)
								{
									num = i;
								}
							}
							if (num >= observableCollection.Count)
							{
								observableCollection.Add(gameObj);
							}
							else
							{
								observableCollection.Insert(num + 1, gameObj);
							}
						}
						else if (gameObj is EditorCollHullObj)
						{
							int num = -1;
							for (int i = 0; i < observableCollection.Count; i++)
							{
								EditorCollHullObj editorCollHullObj = observableCollection[i] as EditorCollHullObj;
								RoomObj roomObj = observableCollection[i] as RoomObj;
								if (editorCollHullObj != null || roomObj != null)
								{
									num = i;
								}
							}
							if (num >= observableCollection.Count)
							{
								observableCollection.Add(gameObj);
							}
							else
							{
								observableCollection.Insert(num + 1, gameObj);
							}
						}
						else
						{
							observableCollection.Add(gameObj);
						}
					}
					else
					{
						Console.WriteLine("Cannot add object.  Already added");
					}
				}
				if (addUndoAction)
				{
					this.UndoManager.AddAction(new AddObjUndoAction(objList, this));
				}
			}
			else
			{
				Console.WriteLine("Cannot add object at index: " + indexLayer + " as it is out of bounds.");
			}
		}

		// Token: 0x0600028E RID: 654 RVA: 0x00015A60 File Offset: 0x00013C60
		public void RemoveGameObj(GameObj obj, bool addUndoAction)
		{
			if (this.LayerObjList.Contains(obj))
			{
				this.LayerObjList.Remove(obj);
				if (addUndoAction)
				{
					this.UndoManager.AddAction(new DeleteObjUndoAction(obj, this));
				}
			}
			else
			{
				Console.WriteLine("Cannot remove obj. Does not exist in list");
			}
		}

		// Token: 0x0600028F RID: 655 RVA: 0x00015AB8 File Offset: 0x00013CB8
		public void RemoveGameObjs(List<GameObj> objList, bool addUndoAction)
		{
			foreach (GameObj item in objList)
			{
				if (this.LayerObjList.Contains(item))
				{
					this.LayerObjList.Remove(item);
				}
				else
				{
					Console.WriteLine("Cannot remove obj. Does not exist in list");
				}
			}
			if (addUndoAction)
			{
				this.UndoManager.AddAction(new DeleteObjUndoAction(objList, this));
			}
		}

		// Token: 0x06000290 RID: 656 RVA: 0x00015B50 File Offset: 0x00013D50
		public void RemoveGameObjInLayer(GameObj obj, int layerIndex, bool addUndoAction)
		{
			if (layerIndex >= 0 && layerIndex < this.LayerList.Count)
			{
				ObservableCollection<GameObj> observableCollection = this.m_layerList[layerIndex];
				if (observableCollection.Contains(obj))
				{
					observableCollection.Remove(obj);
					if (addUndoAction)
					{
						this.UndoManager.AddAction(new DeleteObjUndoAction(obj, this));
					}
				}
				else
				{
					Console.WriteLine("Cannot remove obj. Does not exist in list");
				}
			}
			else
			{
				Console.WriteLine("Cannot remove object at index: " + layerIndex + " as it is out of bounds.");
			}
		}

		// Token: 0x06000291 RID: 657 RVA: 0x00015BE8 File Offset: 0x00013DE8
		public void RemoveGameObjsInLayer(List<GameObj> objList, int layerIndex, bool addUndoAction)
		{
			if (layerIndex >= 0 && layerIndex < this.LayerList.Count)
			{
				ObservableCollection<GameObj> observableCollection = this.m_layerList[layerIndex];
				foreach (GameObj item in objList)
				{
					if (observableCollection.Contains(item))
					{
						observableCollection.Remove(item);
					}
					else
					{
						Console.WriteLine("Cannot remove obj. Does not exist in list");
					}
				}
				if (addUndoAction)
				{
					this.UndoManager.AddAction(new DeleteObjUndoAction(objList, this));
				}
			}
			else
			{
				Console.WriteLine("Cannot remove object at index: " + layerIndex + " as it is out of bounds.");
			}
		}

		// Token: 0x06000292 RID: 658 RVA: 0x00015CC4 File Offset: 0x00013EC4
		public void SwapLayerObjLists(int indexRemoved, int insertionIndex)
		{
			ObservableCollection<GameObj> item = this.m_layerList[indexRemoved];
			this.m_layerList.RemoveAt(indexRemoved);
			this.m_layerList.Insert(insertionIndex, item);
		}

		// Token: 0x06000293 RID: 659 RVA: 0x00015CFC File Offset: 0x00013EFC
		public void SelectGameObj(GameObj obj)
		{
			if (!this.m_selectedObjs.Contains(obj))
			{
				if (!(obj is EditorCollHullObj) || ConfigFile.Instance.selectCollHulls)
				{
					if ((!(obj is SpriteObj) && !(obj is ContainerObj)) || ConfigFile.Instance.selectSprites)
					{
						if (!(obj is RoomObj) || ConfigFile.Instance.selectRooms)
						{
							if (obj is IOpacityObj)
							{
								(obj as IOpacityObj).editorOpacity = EditorEV.SELECTED_OBJ_OPACITY;
							}
							else
							{
								obj.Opacity = EditorEV.SELECTED_OBJ_OPACITY;
							}
							this.m_selectedObjs.Add(obj);
							RoomObj roomObj = obj as RoomObj;
							if (roomObj != null)
							{
								foreach (ObservableCollection<GameObj> observableCollection in this.LayerList)
								{
									foreach (GameObj gameObj in observableCollection)
									{
										if (roomObj.SelectAllObjs && gameObj != roomObj)
										{
											if (CDGMath.Intersects(roomObj.AbsBounds, gameObj.AbsBounds))
											{
												this.SelectGameObj(gameObj);
											}
										}
									}
								}
							}
						}
					}
				}
			}
			else
			{
				Console.WriteLine("Cannot select obj. Already exists in list");
			}
		}

		// Token: 0x06000294 RID: 660 RVA: 0x00015EA0 File Offset: 0x000140A0
		public void SelectGameObjs(List<GameObj> objList)
		{
			foreach (GameObj gameObj in objList)
			{
				if (!this.m_selectedObjs.Contains(gameObj))
				{
					if (!(gameObj is EditorCollHullObj) || ConfigFile.Instance.selectCollHulls)
					{
						if ((!(gameObj is SpriteObj) && !(gameObj is ContainerObj)) || ConfigFile.Instance.selectSprites)
						{
							if (!(gameObj is RoomObj) || ConfigFile.Instance.selectRooms)
							{
								if (gameObj is IOpacityObj)
								{
									(gameObj as IOpacityObj).editorOpacity = EditorEV.SELECTED_OBJ_OPACITY;
								}
								else
								{
									gameObj.Opacity = EditorEV.SELECTED_OBJ_OPACITY;
								}
								this.m_selectedObjs.Add(gameObj);
								RoomObj roomObj = gameObj as RoomObj;
								if (roomObj != null)
								{
									foreach (ObservableCollection<GameObj> observableCollection in this.LayerList)
									{
										foreach (GameObj gameObj2 in observableCollection)
										{
											if (roomObj.SelectAllObjs && gameObj2 != roomObj)
											{
												if (CDGMath.Intersects(roomObj.AbsBounds, gameObj2.AbsBounds))
												{
													this.SelectGameObj(gameObj2);
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06000295 RID: 661 RVA: 0x000160BC File Offset: 0x000142BC
		public void DeselectGameObj(GameObj obj)
		{
			if (this.m_selectedObjs.Contains(obj))
			{
				if (obj is IOpacityObj)
				{
					(obj as IOpacityObj).editorOpacity = EditorEV.CURRENT_LAYER_OPACITY;
				}
				else
				{
					obj.Opacity = EditorEV.CURRENT_LAYER_OPACITY;
				}
				this.m_selectedObjs.Remove(obj);
			}
			else
			{
				Console.WriteLine("Cannot deselect obj. Does not exist in list");
			}
		}

		// Token: 0x06000296 RID: 662 RVA: 0x00016128 File Offset: 0x00014328
		public void DeselectGameObjs(List<GameObj> objList)
		{
			foreach (GameObj gameObj in objList)
			{
				if (this.m_selectedObjs.Contains(gameObj))
				{
					if (gameObj is IOpacityObj)
					{
						(gameObj as IOpacityObj).editorOpacity = EditorEV.CURRENT_LAYER_OPACITY;
					}
					else
					{
						gameObj.Opacity = EditorEV.CURRENT_LAYER_OPACITY;
					}
					this.m_selectedObjs.Remove(gameObj);
				}
			}
		}

		// Token: 0x06000297 RID: 663 RVA: 0x000161C8 File Offset: 0x000143C8
		public void DeselectAllGameObjs()
		{
			if (this.m_selectedObjs.Count > 0)
			{
				foreach (GameObj gameObj in this.m_selectedObjs)
				{
					if (gameObj is IOpacityObj)
					{
						(gameObj as IOpacityObj).editorOpacity = EditorEV.CURRENT_LAYER_OPACITY;
					}
					else
					{
						gameObj.Opacity = EditorEV.CURRENT_LAYER_OPACITY;
					}
				}
				this.m_selectedObjs.Clear();
			}
		}

		// Token: 0x06000298 RID: 664 RVA: 0x00016270 File Offset: 0x00014470
		public void AddLayer()
		{
			ObservableCollection<GameObj> item = new ObservableCollection<GameObj>();
			this.m_layerList.Add(item);
		}

		// Token: 0x06000299 RID: 665 RVA: 0x00016294 File Offset: 0x00014494
		public void AddLayerAt(int index)
		{
			ObservableCollection<GameObj> item = new ObservableCollection<GameObj>();
			this.m_layerList.Insert(index, item);
		}

		// Token: 0x0600029A RID: 666 RVA: 0x000162B8 File Offset: 0x000144B8
		public void RemoveLayerObjList(int index)
		{
			ObservableCollection<GameObj> observableCollection = this.m_layerList[index];
			if (observableCollection != null)
			{
				this.m_layerList.Remove(observableCollection);
			}
			else
			{
				Console.WriteLine("Cannot remove list at index: " + index);
			}
		}

		// Token: 0x0600029B RID: 667 RVA: 0x00016300 File Offset: 0x00014500
		public void CreateCornerHull(Key keyPressed)
		{
			if (this.SelectedObjects.Count == 2)
			{
				Vector2 vector = Vector2.Zero;
				Vector2 vector2 = Vector2.Zero;
				switch (keyPressed)
				{
				case Key.NumPad1:
					vector = CDGMath.LowerLeftCorner(this.SelectedObjects[0].AbsBounds, this.SelectedObjects[0].Rotation, Vector2.Zero);
					vector2 = CDGMath.LowerLeftCorner(this.SelectedObjects[1].AbsBounds, this.SelectedObjects[1].Rotation, Vector2.Zero);
					break;
				case Key.NumPad2:
					break;
				case Key.NumPad3:
					vector = CDGMath.LowerRightCorner(this.SelectedObjects[0].AbsBounds, this.SelectedObjects[0].Rotation, Vector2.Zero);
					vector2 = CDGMath.LowerRightCorner(this.SelectedObjects[1].AbsBounds, this.SelectedObjects[1].Rotation, Vector2.Zero);
					break;
				default:
					switch (keyPressed)
					{
					case Key.NumPad7:
						vector = CDGMath.UpperLeftCorner(this.SelectedObjects[0].AbsBounds, this.SelectedObjects[0].Rotation, Vector2.Zero);
						vector2 = CDGMath.UpperLeftCorner(this.SelectedObjects[1].AbsBounds, this.SelectedObjects[1].Rotation, Vector2.Zero);
						break;
					case Key.NumPad9:
						vector = CDGMath.UpperRightCorner(this.SelectedObjects[0].AbsBounds, this.SelectedObjects[0].Rotation, Vector2.Zero);
						vector2 = CDGMath.UpperRightCorner(this.SelectedObjects[1].AbsBounds, this.SelectedObjects[1].Rotation, Vector2.Zero);
						break;
					}
					break;
				}
				if ((vector2.X < vector.X && (keyPressed == Key.NumPad7 || keyPressed == Key.NumPad9)) || (vector.X < vector2.X && (keyPressed == Key.NumPad1 || keyPressed == Key.NumPad3)))
				{
					Vector2 vector3 = vector;
					vector = vector2;
					vector2 = vector3;
				}
				float x = vector.X;
				float y = vector.Y;
				float width = (float)Math.Sqrt((double)((vector2.X - vector.X) * (vector2.X - vector.X) + (vector2.Y - vector.Y) * (vector2.Y - vector.Y)));
				float height = (float)this.GridUnitSize;
				float rotation = CDGMath.VectorToAngle(vector2 - vector);
				this.AddGameObj(new EditorCollHullObj(x, y, width, height)
				{
					Rotation = rotation
				}, true);
			}
		}

		// Token: 0x0600029C RID: 668 RVA: 0x000165C4 File Offset: 0x000147C4
		public void UpdateAllObjOpacity(int index)
		{
			ObservableCollection<GameObj> observableCollection = this.m_layerList[index];
			foreach (ObservableCollection<GameObj> observableCollection2 in this.m_layerList)
			{
				if (observableCollection2 != observableCollection)
				{
					foreach (GameObj gameObj in observableCollection2)
					{
						if (gameObj is IOpacityObj)
						{
							(gameObj as IOpacityObj).editorOpacity = EditorEV.OTHER_LAYER_OPACITY;
						}
						else
						{
							gameObj.Opacity = EditorEV.OTHER_LAYER_OPACITY;
						}
					}
				}
				else
				{
					foreach (GameObj gameObj in observableCollection2)
					{
						if (gameObj is IOpacityObj)
						{
							(gameObj as IOpacityObj).editorOpacity = EditorEV.CURRENT_LAYER_OPACITY;
						}
						else
						{
							gameObj.Opacity = EditorEV.CURRENT_LAYER_OPACITY;
						}
					}
				}
			}
		}

		// Token: 0x0600029D RID: 669 RVA: 0x00016750 File Offset: 0x00014950
		protected override void Update(Stopwatch gameTime)
		{
		}

		// Token: 0x0600029E RID: 670 RVA: 0x00016754 File Offset: 0x00014954
		protected override void Draw(Stopwatch gameTime)
		{
			float elapsedSeconds = (float)gameTime.Elapsed.TotalSeconds;
			this.GraphicsDevice.Clear(Color.CornflowerBlue);
			this.DrawGrid();
			base.Camera.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, this.m_premultiplyAlphaShader, base.Camera.TransformMatrix);
			base.Camera.Draw(StaticTexture.GenericTexture, new Rectangle(-5, -5, 10, 10), Color.Red * 0.7f);
			ObservableCollection<GameObj> layerObjList = this.LayerObjList;
			foreach (ObservableCollection<GameObj> observableCollection in this.m_layerList)
			{
				if (observableCollection != layerObjList)
				{
					foreach (GameObj gameObj in observableCollection)
					{
						if (gameObj is IOpacityObj)
						{
							(gameObj as IOpacityObj).editorOpacity = EditorEV.OTHER_LAYER_OPACITY;
						}
						else
						{
							gameObj.Opacity = EditorEV.OTHER_LAYER_OPACITY;
						}
						gameObj.Draw(base.Camera, elapsedSeconds);
					}
				}
			}
			bool isChecked = base.MainWindow.ShowSpriteCollHulls.IsChecked;
			foreach (GameObj gameObj in layerObjList)
			{
				gameObj.Draw(base.Camera, elapsedSeconds);
				if (isChecked)
				{
					if (gameObj.HitboxesArray != null)
					{
						gameObj.HitboxesCalculated = false;
						foreach (Hitbox hitbox in gameObj.HitboxesArray)
						{
							if (hitbox.Type == HitboxType.Terrain)
							{
								base.Camera.Draw(StaticTexture.GenericTexture, new Rectangle((int)hitbox.X, (int)hitbox.Y, (int)hitbox.Width, (int)hitbox.Height), null, EditorEV.COLLHULL_COLOUR * gameObj.AbsOpacity, MathHelper.ToRadians(hitbox.Rotation), gameObj.AbsAnchor, gameObj.Flip, 1f);
							}
						}
					}
				}
			}
			if (this.SelectedObjects.Count > 0)
			{
				this.DrawSelectionBounds();
			}
			if (this.m_currentToolObj != null)
			{
				this.m_currentToolObj.Draw(base.Camera, elapsedSeconds);
			}
			base.Camera.End();
		}

		// Token: 0x0600029F RID: 671 RVA: 0x00016A8C File Offset: 0x00014C8C
		public bool hitTestPoint(GameObj obj, Vector2 screenGridPos)
		{
			bool result;
			if (obj is EditorCollHullObj)
			{
				result = true;
			}
			else if (obj is RoomObj)
			{
				result = true;
			}
			else
			{
				if (this.m_hitTestPointRT == null)
				{
					this.m_hitTestPointRT = new RenderTarget2D(base.Camera.GraphicsDevice, (int)this.m_viewportStartingWidth, (int)this.m_viewportStartingHeight);
				}
				base.Camera.GraphicsDevice.SetRenderTarget(this.m_hitTestPointRT);
				base.Camera.GraphicsDevice.Clear(Color.Transparent);
				base.Camera.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, null, null, null, base.Camera.TransformMatrix);
				obj.Draw(base.Camera, 0f);
				base.Camera.End();
				base.Camera.GraphicsDevice.SetRenderTarget(null);
				CDGRect cdgrect = new CDGRect(screenGridPos.X, screenGridPos.Y, 1f, 1f);
				Color[] array = new Color[1];
				this.m_hitTestPointRT.GetData<Color>(0, new Rectangle?(cdgrect.ToRectangle()), array, 0, 1);
				result = array[0].A != 0;
			}
			return result;
		}

		// Token: 0x060002A0 RID: 672 RVA: 0x00016BE0 File Offset: 0x00014DE0
		public void ResetControl()
		{
			this.DeselectAllGameObjs();
			foreach (ObservableCollection<GameObj> observableCollection in this.m_layerList)
			{
				foreach (GameObj gameObj in observableCollection)
				{
					gameObj.Dispose();
				}
				observableCollection.Clear();
			}
			this.m_layerList.Clear();
			this.m_layerList.Add(this.m_gameLayer);
			this.ResetZoom();
			this.ResetCameraPosition();
			this.UndoManager.ResetManager();
		}

		// Token: 0x060002A1 RID: 673 RVA: 0x00016CC4 File Offset: 0x00014EC4
		public void ResetZoom()
		{
			base.Camera.Zoom = 1f;
			base.MainWindow.zoomTextBlock.Text = "Zoom: " + base.Camera.Zoom.ToString("0.00");
		}

		// Token: 0x060002A2 RID: 674 RVA: 0x00016D18 File Offset: 0x00014F18
		public void ResetCameraPosition()
		{
			base.Camera.Position = Vector2.Zero;
			base.MainWindow.coordinatesTextBlock.Text = string.Concat(new object[]
			{
				"(",
				(int)base.Camera.X,
				", ",
				(int)base.Camera.Y,
				")"
			});
		}

		// Token: 0x060002A3 RID: 675 RVA: 0x00016D98 File Offset: 0x00014F98
		private void DrawGrid()
		{
			if (this.GridVisible)
			{
				Matrix transformMatrix = Matrix.CreateTranslation(new Vector3((float)this.m_camera.Width * 0.5f, (float)this.m_camera.Height * 0.5f, 0f));
				this.m_camera.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, transformMatrix);
				int gridUnitSize = this.GridUnitSize;
				int num = (int)(base.Camera.X / (float)gridUnitSize);
				num *= gridUnitSize;
				num = (int)(base.Camera.X - (float)num);
				int num2 = (int)(base.Camera.Y / (float)gridUnitSize);
				num2 *= gridUnitSize;
				num2 = (int)(base.Camera.Y - (float)num2);
				float num3 = (float)base.ActualWidth + (float)(gridUnitSize * 2);
				float num4 = (float)base.ActualHeight + (float)(gridUnitSize * 2);
				if (base.Camera.Zoom < 1f)
				{
					num3 = num3 * 1f / base.Camera.Zoom;
					num4 = num4 * 1f / base.Camera.Zoom;
				}
				int num5 = (int)Math.Round((double)(this.m_viewportStartingWidth / 2f / ((float)gridUnitSize * base.Camera.Zoom)), MidpointRounding.AwayFromZero);
				num5 = (int)((float)num5 * ((float)gridUnitSize * base.Camera.Zoom) - this.m_viewportStartingWidth / 2f);
				int num6 = (int)Math.Round((double)(this.m_viewportStartingHeight / 2f / ((float)gridUnitSize * base.Camera.Zoom)), MidpointRounding.AwayFromZero);
				num6 = (int)((float)num6 * ((float)gridUnitSize * base.Camera.Zoom) - this.m_viewportStartingHeight / 2f);
				int num7 = 0;
				while ((float)num7 < num3 / (float)gridUnitSize)
				{
					CDGRect cdgrect = new CDGRect((float)(num7 * gridUnitSize - num) * base.Camera.Zoom - (float)num5 - (float)base.Camera.Width / 2f, (float)(-(float)base.Camera.Height) / 2f, 1f, num4);
					base.Camera.Draw(this.m_genericTexture, cdgrect.ToRectangle(), EditorEV.GRID_LINE_COLOUR);
					num7++;
				}
				int num8 = 0;
				while ((float)num8 < num4 / (float)gridUnitSize)
				{
					CDGRect cdgrect2 = new CDGRect((float)(-(float)base.Camera.Width) / 2f, (float)(num8 * gridUnitSize - num2) * base.Camera.Zoom - (float)num6 - (float)base.Camera.Height / 2f, num3, 1f);
					base.Camera.Draw(this.m_genericTexture, cdgrect2.ToRectangle(), EditorEV.GRID_LINE_COLOUR);
					num8++;
				}
				base.Camera.End();
			}
		}

		// Token: 0x060002A4 RID: 676 RVA: 0x00017068 File Offset: 0x00015268
		private void DrawSelectionBounds()
		{
			Rectangle rectangle = this.SelectionBounds.ToRectangle();
			int num = 2;
			int num2 = num;
			num = (int)((float)num / base.Camera.Zoom);
			if (num < num2)
			{
				num = num2;
			}
			base.Camera.Draw(StaticTexture.GenericTexture, new Rectangle(rectangle.X, rectangle.Y, rectangle.Width, num), Color.Yellow);
			base.Camera.Draw(StaticTexture.GenericTexture, new Rectangle(rectangle.X, rectangle.Y, num, rectangle.Height), Color.Yellow);
			base.Camera.Draw(StaticTexture.GenericTexture, new Rectangle(rectangle.X + rectangle.Width, rectangle.Y, num, rectangle.Height), Color.Yellow);
			base.Camera.Draw(StaticTexture.GenericTexture, new Rectangle(rectangle.X, rectangle.Y + rectangle.Height, rectangle.Width, num), Color.Yellow);
		}

		// Token: 0x04000137 RID: 311
		private Vector2 m_initialMousePos;

		// Token: 0x04000138 RID: 312
		private Vector2 m_initialCameraPos;

		// Token: 0x04000139 RID: 313
		private ObservableCollection<GameObj> m_selectedObjs;

		// Token: 0x0400013A RID: 314
		private List<ObservableCollection<GameObj>> m_layerList;

		// Token: 0x0400013B RID: 315
		private ObservableCollection<GameObj> m_gameLayer;

		// Token: 0x0400013C RID: 316
		private ToolObj m_currentToolObj;

		// Token: 0x0400013D RID: 317
		private CollHullToolObj m_collHullToolObj;

		// Token: 0x0400013E RID: 318
		private SelectionToolObj m_selectionToolObj;

		// Token: 0x0400013F RID: 319
		private ScaleToolObj m_scaleToolObj;

		// Token: 0x04000140 RID: 320
		private MarkerToolObj m_markerToolObj;

		// Token: 0x04000141 RID: 321
		private RoomToolObj m_roomToolObj;

		// Token: 0x04000142 RID: 322
		private PlayerStartToolObj m_playerStartToolObj;

		// Token: 0x04000143 RID: 323
		private RotationToolObj m_rotationToolObj;

		// Token: 0x04000144 RID: 324
		private List<Vector2> m_objStartPos;

		// Token: 0x04000145 RID: 325
		private UndoManager m_undoManager;

		// Token: 0x04000146 RID: 326
		private bool m_useCachedSelectionBounds;

		// Token: 0x04000147 RID: 327
		private CDGRect m_selectionBounds = default(CDGRect);

		// Token: 0x04000148 RID: 328
		private float m_viewportStartingWidth;

		// Token: 0x04000149 RID: 329
		private float m_viewportStartingHeight;

		// Token: 0x0400014A RID: 330
		private Effect m_premultiplyAlphaShader;

		// Token: 0x0400014B RID: 331
		private RenderTarget2D m_hitTestPointRT;
	}
}

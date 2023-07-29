using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using Brawler2D;
using CDGEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpriteSystem2;

namespace BrawlerEditor
{
	// Token: 0x02000026 RID: 38
	public static class SaveHelper
	{
		// Token: 0x06000161 RID: 353 RVA: 0x00007FFC File Offset: 0x000061FC
		public static string SaveBEFFile(MainWindow mainWindow)
		{
			string text = "";
			mainWindow.outputScrollViewer.AddOutputText("Compiling Arena...");
			List<ObservableCollection<GameObj>> list = new List<ObservableCollection<GameObj>>();
			List<RoomObj> list2 = new List<RoomObj>();
			List<GameObj> list3 = new List<GameObj>();
			List<RoomObj> list4 = new List<RoomObj>();
			List<RoomObj> list5 = new List<RoomObj>();
			foreach (ObservableCollection<GameObj> observableCollection in mainWindow.gameScreenControl.LayerList)
			{
				foreach (GameObj gameObj in observableCollection)
				{
					RoomObj roomObj = gameObj as RoomObj;
					if (roomObj != null)
					{
						if (roomObj.IsArenaZone)
						{
							list4.Add(roomObj);
						}
						else
						{
							list5.Add(roomObj);
						}
					}
				}
			}
			list2.AddRange(list5);
			for (int i = 0; i < list4.Count; i++)
			{
				RoomObj roomObj2 = list4[i];
				foreach (RoomObj roomObj3 in list5)
				{
					if (CDGMath.Intersects(roomObj3.AbsBounds, roomObj2.AbsBounds) || roomObj3.AbsBounds.Contains(roomObj2.AbsBounds))
					{
						list2.Add(roomObj2);
						break;
					}
				}
			}
			foreach (ObservableCollection<GameObj> observableCollection in mainWindow.gameScreenControl.LayerList)
			{
				list.Add(new ObservableCollection<GameObj>());
			}
			for (int i = 0; i < mainWindow.gameScreenControl.LayerList.Count; i++)
			{
				ObservableCollection<GameObj> observableCollection2 = list[i];
				ObservableCollection<GameObj> observableCollection3 = mainWindow.gameScreenControl.LayerList[i];
				foreach (GameObj gameObj in observableCollection3)
				{
					foreach (RoomObj roomObj4 in list2)
					{
						if (gameObj != roomObj4 && !(gameObj is RoomObj))
						{
							bool flag = false;
							IEditorDrawableObj editorDrawableObj = gameObj as IEditorDrawableObj;
							if (editorDrawableObj != null)
							{
								flag = editorDrawableObj.forceAddToStage;
							}
							if (!list3.Contains(gameObj) && (flag || CDGMath.Intersects(roomObj4.AbsBounds, gameObj.AbsBounds)))
							{
								list3.Add(gameObj);
								observableCollection2.Add(gameObj);
							}
						}
					}
				}
			}
			if (list2.Count == 0)
			{
				throw new Exception("Cannot find any arenas.");
			}
			string str = SaveHelper.FilePath.Substring(0, SaveHelper.FilePath.LastIndexOf("\\") + 1);
			string text2 = SaveHelper.FilePath.Substring(SaveHelper.FilePath.LastIndexOf("\\") + 1);
			XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
			xmlWriterSettings.Indent = true;
			text2 = text2.Replace(".xml", ".bef");
			text = str + "\\Compiled\\" + text2;
			if (!Directory.Exists(str + "\\Compiled\\"))
			{
				Directory.CreateDirectory(str + "\\Compiled\\");
			}
			XmlWriter xmlWriter = XmlWriter.Create(text);
			try
			{
				xmlWriter.WriteStartDocument();
				xmlWriter.WriteStartElement("Map");
				List<string> list6 = new List<string>();
				foreach (GameObj gameObj in list3)
				{
					SaveHelper.AddSpritesheetNameToList(list6, gameObj);
				}
				foreach (string value in list6)
				{
					xmlWriter.WriteStartElement("Spritesheet");
					xmlWriter.WriteAttributeString("Name", value);
					xmlWriter.WriteEndElement();
				}
				SaveHelper.SaveLayerdata(xmlWriter, mainWindow.layerTabControl);
				foreach (RoomObj roomObj4 in list2)
				{
					SaveHelper.SaveGameObjData(xmlWriter, roomObj4, mainWindow.layerTabControl.GameLayerIndex, null);
				}
				int num = 0;
				foreach (ObservableCollection<GameObj> observableCollection in list)
				{
					foreach (GameObj gameObj in observableCollection)
					{
						SaveHelper.SaveGameObjData(xmlWriter, gameObj, num, null);
					}
					num++;
				}
				xmlWriter.WriteEndDocument();
				mainWindow.outputScrollViewer.AddOutputText("SUCCESS: Compile Successful");
			}
			catch (Exception ex)
			{
				MessageBox.Show("ERROR: Could not write to disk. Original error: " + ex.Message);
			}
			finally
			{
				if (xmlWriter != null)
				{
					xmlWriter.Flush();
					xmlWriter.Close();
				}
			}
			return text;
		}

		// Token: 0x06000162 RID: 354 RVA: 0x00008734 File Offset: 0x00006934
		public static void SaveXMLFile(MainWindow mainWindow, bool saveToClipboard)
		{
			XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
			xmlWriterSettings.Indent = true;
			XmlWriter xmlWriter = null;
			if (!saveToClipboard)
			{
				xmlWriter = XmlWriter.Create("tempsave.xml", xmlWriterSettings);
			}
			else
			{
				xmlWriter = XmlWriter.Create("Clipboard.ini", xmlWriterSettings);
			}
			try
			{
				xmlWriter.WriteStartDocument();
				xmlWriter.WriteStartElement("Map");
				if (!saveToClipboard)
				{
					List<string> list = new List<string>();
					foreach (ObservableCollection<GameObj> observableCollection in mainWindow.gameScreenControl.LayerList)
					{
						foreach (GameObj gameObj in observableCollection)
						{
							SaveHelper.AddSpritesheetNameToList(list, gameObj);
						}
					}
					foreach (string value in list)
					{
						xmlWriter.WriteStartElement("Spritesheet");
						xmlWriter.WriteAttributeString("Name", value);
						xmlWriter.WriteEndElement();
					}
					SaveHelper.SaveLayerdata(xmlWriter, mainWindow.layerTabControl);
					int num = 0;
					foreach (ObservableCollection<GameObj> observableCollection in mainWindow.gameScreenControl.LayerList)
					{
						foreach (GameObj gameObj in observableCollection)
						{
							SaveHelper.SaveGameObjData(xmlWriter, gameObj, num, null);
						}
						num++;
					}
				}
				else
				{
					int num = mainWindow.layerTabControl.SelectedIndex;
					foreach (GameObj gameObj in mainWindow.gameScreenControl.SelectedObjects)
					{
						if (gameObj is PlayerStartObj)
						{
							if ((gameObj as PlayerStartObj).isDebugOnly)
							{
								mainWindow.outputScrollViewer.AddOutputText("WARNING: Could not copy Debug PlayerStartObj as only one instance is allowed on-screen at all times.");
							}
							else
							{
								mainWindow.outputScrollViewer.AddOutputText("WARNING: Could not copy PlayerStartObj as only one instance is allowed on-screen at all times.");
							}
						}
						else
						{
							SaveHelper.SaveGameObjData(xmlWriter, gameObj, num, mainWindow.gameScreenControl.Camera);
						}
					}
				}
				xmlWriter.WriteEndDocument();
				xmlWriter.Flush();
				xmlWriter.Close();
				if (!saveToClipboard)
				{
					SaveHelper.CopyXmlDocument("tempsave.xml", SaveHelper.FilePath);
					mainWindow.outputScrollViewer.AddOutputText("SUCCESS: Save successful.");
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("ERROR: Could not write to disk. Original error: " + ex.Message);
				mainWindow.outputScrollViewer.AddOutputText("ERROR: Save failed. " + ex.Message);
			}
			finally
			{
				if (xmlWriter != null)
				{
					xmlWriter.Flush();
					xmlWriter.Close();
				}
				if (File.Exists("tempsave.xml"))
				{
					File.Delete("tempsave.xml");
				}
			}
		}

		// Token: 0x06000163 RID: 355 RVA: 0x00008B48 File Offset: 0x00006D48
		private static void CopyXmlDocument(string oldfile, string newfile)
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.Load(oldfile);
			if (File.Exists(newfile))
			{
				File.Delete(newfile);
			}
			xmlDocument.Save(newfile);
			if (File.Exists(oldfile))
			{
				File.Delete(oldfile);
			}
		}

		// Token: 0x06000164 RID: 356 RVA: 0x00008B94 File Offset: 0x00006D94
		private static void AddSpritesheetNameToList(List<string> spritesheetList, GameObj obj)
		{
			string text = "";
			SpriteObj spriteObj = obj as SpriteObj;
			if (spriteObj != null)
			{
				text = SpriteLibrary.GetSpritesheetObj(spriteObj.spriteName).SpritesheetPath;
			}
			ContainerObj containerObj = obj as ContainerObj;
			if (containerObj != null)
			{
				text = SpriteLibrary.GetSpritesheetObj((containerObj.GetChildAt(0) as DisplayObj).spriteName).SpritesheetPath;
			}
			if (text != "")
			{
				string text2 = text.ToLower().Replace(ConfigFile.Instance.SpritesheetDirectory.ToLower(), "");
				text2 = text2.Substring(1, text2.LastIndexOf(".") - 1);
				if (!spritesheetList.Contains(text2))
				{
					spritesheetList.Add(text2);
				}
			}
		}

		// Token: 0x06000165 RID: 357 RVA: 0x00008C58 File Offset: 0x00006E58
		private static void SaveLayerdata(XmlWriter writer, LayerTabControl layerTabControl)
		{
			for (int i = 0; i < layerTabControl.Items.Count; i++)
			{
				TabItem tabItem = layerTabControl.Items[i] as TabItem;
				LayerPropertyObj layerPropertyObj = layerTabControl.layerPropertyObjList[i];
				if (tabItem == null)
				{
					throw new Exception("Could not save layer data. Object in layerTabControl was somehow not a TabItem.");
				}
				string text = (tabItem.Header as TextBlock).Text;
				writer.WriteStartElement("Layer");
				writer.WriteAttributeString("Name", text);
				writer.WriteAttributeString("ParallaxX", layerPropertyObj.parallaxX.ToString());
				writer.WriteAttributeString("ParallaxY", layerPropertyObj.parallaxY.ToString());
				writer.WriteAttributeString("ScrollSpeedX", layerPropertyObj.scrollSpeedX.ToString());
				writer.WriteAttributeString("ScrollSpeedY", layerPropertyObj.scrollSpeedY.ToString());
				writer.WriteAttributeString("SortLayer", layerPropertyObj.sortLayer.ToString());
				writer.WriteAttributeString("AddToGameLayer", layerPropertyObj.addToGameLayer.ToString());
				writer.WriteAttributeString("LayerOverlay", layerPropertyObj.layerOverlay);
				writer.WriteAttributeString("ApplyGaussian", layerPropertyObj.applyGaussianBlur.ToString());
				writer.WriteEndElement();
			}
		}

		// Token: 0x06000166 RID: 358 RVA: 0x00008DC8 File Offset: 0x00006FC8
		private static void SaveGameObjData(XmlWriter writer, GameObj gameObj, int layerID, Camera2D camera)
		{
			bool flag = false;
			RoomObj roomObj = gameObj as RoomObj;
			if (roomObj != null && !flag)
			{
				flag = true;
				writer.WriteStartElement("Room");
				writer.WriteAttributeString("Width", roomObj.Width.ToString());
				writer.WriteAttributeString("Height", roomObj.Height.ToString());
				writer.WriteAttributeString("InnerZonePos", roomObj.innerZonePos.ToString());
				writer.WriteAttributeString("InnerZonePercent", roomObj.innerZonePercent.ToString());
				writer.WriteAttributeString("SelectAll", roomObj.SelectAllObjs.ToString());
				writer.WriteAttributeString("IsArenaZone", roomObj.IsArenaZone.ToString());
				writer.WriteAttributeString("ForceRed", roomObj.forceRed.ToString());
				writer.WriteAttributeString("ForceBlack", roomObj.forceBlack.ToString());
				writer.WriteAttributeString("ForceGreen", roomObj.forceGreen.ToString());
				writer.WriteAttributeString("HookNavMesh", roomObj.hookProjectilesToNavMesh.ToString());
				writer.WriteAttributeString("MaxCameraZoomOut", roomObj.maxCameraZoomOut.ToString());
			}
			EditorCollHullObj editorCollHullObj = gameObj as EditorCollHullObj;
			if (editorCollHullObj != null && !flag)
			{
				flag = true;
				if (editorCollHullObj.triggerType != 0)
				{
					writer.WriteStartElement("Trigger");
				}
				else
				{
					writer.WriteStartElement("CollHull");
				}
				writer.WriteAttributeString("Width", editorCollHullObj.Width.ToString());
				writer.WriteAttributeString("Height", editorCollHullObj.Height.ToString());
				writer.WriteAttributeString("TriggerType", editorCollHullObj.triggerType.ToString());
				writer.WriteAttributeString("TransitionType", editorCollHullObj.transitionZoneType.ToString());
				writer.WriteAttributeString("IsNavMesh", editorCollHullObj.isNavMesh.ToString());
				writer.WriteAttributeString("IsArenaTrigger", editorCollHullObj.isArenaTrigger.ToString());
				writer.WriteAttributeString("Jumpable", editorCollHullObj.jumpable.ToString());
				writer.WriteAttributeString("CanShoot", editorCollHullObj.canShootThrough.ToString());
				writer.WriteAttributeString("PlayerCanShoot", editorCollHullObj.playerCanShootThrough.ToString());
				writer.WriteAttributeString("PlayersCollide", editorCollHullObj.onlyCollidesWithPlayers.ToString());
				writer.WriteAttributeString("EnemiesCollide", editorCollHullObj.onlyCollidesWithEnemies.ToString());
				writer.WriteAttributeString("HidesEngineerUI", editorCollHullObj.hidesEngineerUI.ToString());
			}
			MarkerObj markerObj = gameObj as MarkerObj;
			if (markerObj != null && !flag)
			{
				flag = true;
				writer.WriteStartElement("Marker");
				writer.WriteAttributeString("ID", markerObj.ID.ToString());
			}
			PlayerStartObj playerStartObj = gameObj as PlayerStartObj;
			if (playerStartObj != null && !flag)
			{
				flag = true;
				writer.WriteStartElement("PlayerStart");
				writer.WriteAttributeString("DebugOnly", playerStartObj.isDebugOnly.ToString());
			}
			EditorSpriteObj editorSpriteObj = gameObj as EditorSpriteObj;
			if (editorSpriteObj != null && !flag)
			{
				flag = true;
				writer.WriteStartElement("Sprite");
				writer.WriteAttributeString("SpriteName", editorSpriteObj.spriteName);
				writer.WriteAttributeString("Jumpable", editorSpriteObj.jumpable.ToString());
				writer.WriteAttributeString("Opacity", editorSpriteObj.pureOpacity.ToString());
				writer.WriteAttributeString("AddPhysics", editorSpriteObj.addPhysics.ToString());
				writer.WriteAttributeString("ForceAdd", editorSpriteObj.forceAddToStage.ToString());
			}
			EditorEnemyObj editorEnemyObj = gameObj as EditorEnemyObj;
			if (editorEnemyObj != null && !flag)
			{
				flag = true;
				writer.WriteStartElement("Enemy");
				writer.WriteAttributeString("SpriteName", editorEnemyObj.spriteName);
				writer.WriteAttributeString("Type", editorEnemyObj.enemyType.ToString());
				writer.WriteAttributeString("R", editorEnemyObj.TextureColor.R.ToString());
				writer.WriteAttributeString("G", editorEnemyObj.TextureColor.G.ToString());
				writer.WriteAttributeString("B", editorEnemyObj.TextureColor.B.ToString());
			}
			EditorContainerObj editorContainerObj = gameObj as EditorContainerObj;
			if (editorContainerObj != null && !flag)
			{
				writer.WriteStartElement("Container");
				writer.WriteAttributeString("SpriteName", editorContainerObj.spriteName);
				writer.WriteAttributeString("Jumpable", editorContainerObj.jumpable.ToString());
				writer.WriteAttributeString("Opacity", editorContainerObj.pureOpacity.ToString());
				writer.WriteAttributeString("AddPhysics", editorContainerObj.addPhysics.ToString());
				writer.WriteAttributeString("ForceAdd", editorContainerObj.forceAddToStage.ToString());
			}
			SaveHelper.SaveGenericData(writer, gameObj, camera);
			writer.WriteAttributeString("Layer", layerID.ToString());
			writer.WriteEndElement();
		}

		// Token: 0x06000167 RID: 359 RVA: 0x0000937C File Offset: 0x0000757C
		private static void SaveGenericData(XmlWriter writer, GameObj obj, Camera2D camera)
		{
			writer.WriteAttributeString("Name", obj.Name);
			if (camera == null)
			{
				writer.WriteAttributeString("X", obj.X.ToString());
				writer.WriteAttributeString("Y", obj.Y.ToString());
			}
			else
			{
				writer.WriteAttributeString("X", (obj.X - camera.X).ToString());
				writer.WriteAttributeString("Y", (obj.Y - camera.Y).ToString());
			}
			writer.WriteAttributeString("ScaleX", obj.ScaleX.ToString());
			writer.WriteAttributeString("ScaleY", obj.ScaleY.ToString());
			writer.WriteAttributeString("Rotation", obj.Rotation.ToString());
			writer.WriteAttributeString("Flip", (obj.Flip == SpriteEffects.FlipHorizontally).ToString());
			writer.WriteAttributeString("Tag", obj.Tag);
			ICodeObj codeObj = obj as ICodeObj;
			if (codeObj != null)
			{
				if (codeObj.code != null && codeObj.code.Length > 1)
				{
					codeObj.code = SaveHelper.RefactorCode(codeObj.code);
				}
				writer.WriteAttributeString("Code", codeObj.code);
			}
		}

		// Token: 0x06000168 RID: 360 RVA: 0x000094F8 File Offset: 0x000076F8
		private static string RefactorCode(string code)
		{
			int num = 0;
			int length = Environment.NewLine.Length;
			int num2 = code.IndexOf(Environment.NewLine);
			int i = -1;
			if (num2 != -1)
			{
				i = num2 + length;
			}
			if (code.Length > 1 && code.Substring(code.Length - length) != Environment.NewLine)
			{
				code += Environment.NewLine;
			}
			List<string> list = new List<string>();
			while (i >= 0)
			{
				string text = code.Substring(num, i - num);
				text = text.Trim();
				if (text.Length > 2)
				{
					if (text[text.Length - 1] != ';')
					{
						text += ";";
					}
				}
				text += Environment.NewLine;
				list.Add(text);
				num = i;
				i = code.IndexOf(Environment.NewLine, num);
				if (i > -1)
				{
					i += length;
				}
			}
			string result;
			if (list.Count > 0)
			{
				string text2 = "";
				foreach (string str in list)
				{
					text2 += str;
				}
				result = text2;
			}
			else
			{
				result = code;
			}
			return result;
		}

		// Token: 0x06000169 RID: 361 RVA: 0x0000968C File Offset: 0x0000788C
		public static void LoadXMLFile(MainWindow mainWindow, bool loadToClipboard)
		{
			CultureInfo cultureInfo = (CultureInfo)CultureInfo.CurrentCulture.Clone();
			cultureInfo.NumberFormat.CurrencyDecimalSeparator = ".";
			XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
			xmlReaderSettings.IgnoreComments = true;
			xmlReaderSettings.IgnoreWhitespace = true;
			XmlReader xmlReader;
			if (!loadToClipboard)
			{
				xmlReader = XmlReader.Create(SaveHelper.FilePath, xmlReaderSettings);
			}
			else
			{
				xmlReader = XmlReader.Create("Clipboard.ini", xmlReaderSettings);
			}
			try
			{
				List<GameObj> list = new List<GameObj>();
				bool flag = false;
				while (xmlReader.Read())
				{
					if (xmlReader.NodeType == XmlNodeType.Element)
					{
						GameObj gameObj = null;
						string name = xmlReader.Name;
						switch (name)
						{
						case "Layer":
						{
							xmlReader.MoveToAttribute("Name");
							string value = xmlReader.Value;
							xmlReader.MoveToAttribute("ParallaxX");
							float parallaxX = float.Parse(xmlReader.Value, NumberStyles.Any, cultureInfo);
							xmlReader.MoveToAttribute("ParallaxY");
							float parallaxY = float.Parse(xmlReader.Value, NumberStyles.Any, cultureInfo);
							xmlReader.MoveToAttribute("ScrollSpeedX");
							float scrollSpeedX = float.Parse(xmlReader.Value, NumberStyles.Any, cultureInfo);
							xmlReader.MoveToAttribute("ScrollSpeedY");
							float scrollSpeedY = float.Parse(xmlReader.Value, NumberStyles.Any, cultureInfo);
							string layerOverlay = "255, 255, 255, 255";
							if (xmlReader.MoveToAttribute("LayerOverlay"))
							{
								layerOverlay = xmlReader.Value;
							}
							bool sortLayer = false;
							if (xmlReader.MoveToAttribute("SortLayer"))
							{
								sortLayer = bool.Parse(xmlReader.Value);
							}
							bool addToGameLayer = false;
							if (xmlReader.MoveToAttribute("AddToGameLayer"))
							{
								addToGameLayer = bool.Parse(xmlReader.Value);
							}
							bool applyGaussian = false;
							if (xmlReader.MoveToAttribute("ApplyGaussian"))
							{
								applyGaussian = bool.Parse(xmlReader.Value);
							}
							if (value == "Game Layer")
							{
								mainWindow.layerTabControl.ChangeLayerPropertiesAt(mainWindow.layerTabControl.GameLayerIndex, parallaxX, parallaxY, scrollSpeedX, scrollSpeedY, sortLayer, layerOverlay, applyGaussian, addToGameLayer);
								flag = true;
							}
							else if (!flag)
							{
								mainWindow.layerTabControl.AddLayerAt(value, mainWindow.layerTabControl.GameLayerIndex, false);
								mainWindow.layerTabControl.ChangeLayerPropertiesAt(mainWindow.layerTabControl.GameLayerIndex - 1, parallaxX, parallaxY, scrollSpeedX, scrollSpeedY, sortLayer, layerOverlay, applyGaussian, addToGameLayer);
							}
							else
							{
								mainWindow.layerTabControl.AddLayer(value, false);
								mainWindow.layerTabControl.ChangeLayerPropertiesAt(mainWindow.layerTabControl.Items.Count - 1, parallaxX, parallaxY, scrollSpeedX, scrollSpeedY, sortLayer, layerOverlay, applyGaussian, addToGameLayer);
							}
							break;
						}
						case "Room":
						{
							RoomObj roomObj = new RoomObj(0f, 0f, 0f, 0f);
							xmlReader.MoveToAttribute("Width");
							float width = float.Parse(xmlReader.Value, NumberStyles.Any, cultureInfo);
							xmlReader.MoveToAttribute("Height");
							float height = float.Parse(xmlReader.Value, NumberStyles.Any, cultureInfo);
							roomObj.Width = width;
							roomObj.Height = height;
							xmlReader.MoveToAttribute("InnerZonePos");
							byte innerZonePos = byte.Parse(xmlReader.Value, NumberStyles.Any, cultureInfo);
							roomObj.innerZonePos = innerZonePos;
							xmlReader.MoveToAttribute("InnerZonePercent");
							float innerZonePercent = float.Parse(xmlReader.Value, NumberStyles.Any, cultureInfo);
							roomObj.innerZonePercent = innerZonePercent;
							xmlReader.MoveToAttribute("SelectAll");
							bool selectAllObjs = bool.Parse(xmlReader.Value);
							roomObj.SelectAllObjs = selectAllObjs;
							xmlReader.MoveToAttribute("IsArenaZone");
							roomObj.IsArenaZone = bool.Parse(xmlReader.Value);
							if (xmlReader.MoveToAttribute("ForceRed"))
							{
								roomObj.forceRed = bool.Parse(xmlReader.Value);
							}
							if (xmlReader.MoveToAttribute("ForceBlack"))
							{
								roomObj.forceBlack = bool.Parse(xmlReader.Value);
							}
							if (xmlReader.MoveToAttribute("ForceGreen"))
							{
								roomObj.forceGreen = bool.Parse(xmlReader.Value);
							}
							if (xmlReader.MoveToAttribute("HookNavMesh"))
							{
								roomObj.hookProjectilesToNavMesh = bool.Parse(xmlReader.Value);
							}
							if (xmlReader.MoveToAttribute("MaxCameraZoomOut"))
							{
								roomObj.maxCameraZoomOut = float.Parse(xmlReader.Value, NumberStyles.Any, cultureInfo);
							}
							gameObj = roomObj;
							break;
						}
						case "Trigger":
						case "CollHull":
						{
							EditorCollHullObj editorCollHullObj = new EditorCollHullObj(0f, 0f, 0f, 0f);
							xmlReader.MoveToAttribute("Width");
							float width2 = float.Parse(xmlReader.Value, NumberStyles.Any, cultureInfo);
							xmlReader.MoveToAttribute("Height");
							float height2 = float.Parse(xmlReader.Value, NumberStyles.Any, cultureInfo);
							editorCollHullObj.Width = width2;
							editorCollHullObj.Height = height2;
							xmlReader.MoveToAttribute("TriggerType");
							byte triggerType = byte.Parse(xmlReader.Value, NumberStyles.Any, cultureInfo);
							editorCollHullObj.triggerType = triggerType;
							if (xmlReader.MoveToAttribute("TransitionType"))
							{
								editorCollHullObj.transitionZoneType = byte.Parse(xmlReader.Value, NumberStyles.Any, cultureInfo);
							}
							gameObj = editorCollHullObj;
							if (xmlReader.MoveToAttribute("IsNavMesh"))
							{
								editorCollHullObj.isNavMesh = bool.Parse(xmlReader.Value);
							}
							if (xmlReader.MoveToAttribute("IsArenaTrigger"))
							{
								editorCollHullObj.isArenaTrigger = bool.Parse(xmlReader.Value);
							}
							if (xmlReader.MoveToAttribute("Jumpable"))
							{
								editorCollHullObj.jumpable = bool.Parse(xmlReader.Value);
							}
							if (xmlReader.MoveToAttribute("CanShoot"))
							{
								editorCollHullObj.canShootThrough = bool.Parse(xmlReader.Value);
							}
							if (xmlReader.MoveToAttribute("PlayerCanShoot"))
							{
								editorCollHullObj.playerCanShootThrough = bool.Parse(xmlReader.Value);
							}
							if (xmlReader.MoveToAttribute("PlayersCollide"))
							{
								editorCollHullObj.onlyCollidesWithPlayers = bool.Parse(xmlReader.Value);
							}
							if (xmlReader.MoveToAttribute("EnemiesCollide"))
							{
								editorCollHullObj.onlyCollidesWithEnemies = bool.Parse(xmlReader.Value);
							}
							if (xmlReader.MoveToAttribute("HidesEngineerUI"))
							{
								editorCollHullObj.hidesEngineerUI = bool.Parse(xmlReader.Value);
							}
							break;
						}
						case "Marker":
						{
							MarkerObj markerObj = new MarkerObj();
							xmlReader.MoveToAttribute("ID");
							int id = int.Parse(xmlReader.Value, NumberStyles.Any, cultureInfo);
							markerObj.ID = id;
							gameObj = markerObj;
							break;
						}
						case "PlayerStart":
						{
							PlayerStartObj playerStartObj = new PlayerStartObj();
							if (xmlReader.MoveToAttribute("DebugOnly"))
							{
								playerStartObj.isDebugOnly = bool.Parse(xmlReader.Value);
							}
							gameObj = playerStartObj;
							break;
						}
						case "Sprite":
						{
							xmlReader.MoveToAttribute("SpriteName");
							string value2 = xmlReader.Value;
							EditorSpriteObj editorSpriteObj = new EditorSpriteObj(value2);
							if (xmlReader.MoveToAttribute("Jumpable"))
							{
								editorSpriteObj.jumpable = bool.Parse(xmlReader.Value);
							}
							if (xmlReader.MoveToAttribute("Opacity"))
							{
								editorSpriteObj.Opacity = float.Parse(xmlReader.Value, NumberStyles.Any, cultureInfo);
							}
							if (xmlReader.MoveToAttribute("AddPhysics"))
							{
								editorSpriteObj.addPhysics = bool.Parse(xmlReader.Value);
							}
							if (xmlReader.MoveToAttribute("ForceAdd"))
							{
								editorSpriteObj.forceAddToStage = bool.Parse(xmlReader.Value);
							}
							if (editorSpriteObj.spriteName == "" || editorSpriteObj.spriteName == null)
							{
								throw new Exception("Could not load Sprite: " + value2);
							}
							gameObj = editorSpriteObj;
							break;
						}
						case "Container":
						{
							xmlReader.MoveToAttribute("SpriteName");
							string value3 = xmlReader.Value;
							EditorContainerObj editorContainerObj = new EditorContainerObj(value3);
							if (xmlReader.MoveToAttribute("Jumpable"))
							{
								editorContainerObj.jumpable = bool.Parse(xmlReader.Value);
							}
							if (xmlReader.MoveToAttribute("Opacity"))
							{
								editorContainerObj.Opacity = float.Parse(xmlReader.Value, NumberStyles.Any, cultureInfo);
							}
							if (xmlReader.MoveToAttribute("AddPhysics"))
							{
								editorContainerObj.addPhysics = bool.Parse(xmlReader.Value);
							}
							if (xmlReader.MoveToAttribute("ForceAdd"))
							{
								editorContainerObj.forceAddToStage = bool.Parse(xmlReader.Value);
							}
							if (editorContainerObj.NumChildren == 0)
							{
								throw new Exception("Could not load Container: " + value3);
							}
							gameObj = editorContainerObj;
							break;
						}
						case "Enemy":
						{
							xmlReader.MoveToAttribute("Type");
							EnemyType enemyType = (EnemyType)Enum.Parse(typeof(EnemyType), xmlReader.Value);
							xmlReader.MoveToAttribute("SpriteName");
							string value4 = xmlReader.Value;
							EditorEnemyObj editorEnemyObj = new EditorEnemyObj(value4);
							editorEnemyObj.enemyType = enemyType;
							xmlReader.MoveToAttribute("R");
							byte r = byte.Parse(xmlReader.Value, NumberStyles.Any, cultureInfo);
							xmlReader.MoveToAttribute("G");
							byte g = byte.Parse(xmlReader.Value, NumberStyles.Any, cultureInfo);
							xmlReader.MoveToAttribute("B");
							byte b = byte.Parse(xmlReader.Value, NumberStyles.Any, cultureInfo);
							editorEnemyObj.TextureColor = new Color((int)r, (int)g, (int)b);
							gameObj = editorEnemyObj;
							break;
						}
						}
						if (gameObj != null)
						{
							if (!loadToClipboard)
							{
								SaveHelper.LoadGenericData(xmlReader, gameObj, null, cultureInfo);
								xmlReader.MoveToAttribute("Layer");
								int indexLayer = int.Parse(xmlReader.Value);
								mainWindow.gameScreenControl.AddGameObjInLayer(gameObj, indexLayer, false);
							}
							else
							{
								SaveHelper.LoadGenericData(xmlReader, gameObj, mainWindow.gameScreenControl.Camera, cultureInfo);
								list.Add(gameObj);
							}
						}
					}
				}
				if (loadToClipboard)
				{
					mainWindow.gameScreenControl.AddGameObjs(list, true);
					mainWindow.gameScreenControl.SelectGameObjs(list);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("ERROR: Could not load file. Original error: " + ex.Message + "  Reader value: " + xmlReader.Value);
			}
			finally
			{
				if (xmlReader != null)
				{
					xmlReader.Close();
				}
			}
		}

		// Token: 0x0600016A RID: 362 RVA: 0x0000A208 File Offset: 0x00008408
		private static void LoadGenericData(XmlReader reader, GameObj obj, Camera2D camera, CultureInfo ci)
		{
			reader.MoveToAttribute("Name");
			obj.Name = reader.Value;
			reader.MoveToAttribute("X");
			obj.X = float.Parse(reader.Value, NumberStyles.Any, ci);
			reader.MoveToAttribute("Y");
			obj.Y = float.Parse(reader.Value, NumberStyles.Any, ci);
			reader.MoveToAttribute("ScaleX");
			obj.ScaleX = float.Parse(reader.Value, NumberStyles.Any, ci);
			reader.MoveToAttribute("ScaleY");
			obj.ScaleY = float.Parse(reader.Value, NumberStyles.Any, ci);
			if (reader.MoveToAttribute("Rotation"))
			{
				obj.Rotation = float.Parse(reader.Value, NumberStyles.Any, ci);
			}
			reader.MoveToAttribute("Flip");
			bool flag = bool.Parse(reader.Value);
			if (flag)
			{
				obj.Flip = SpriteEffects.FlipHorizontally;
			}
			if (reader.MoveToAttribute("Tag"))
			{
				obj.Tag = reader.Value;
			}
			if (camera != null)
			{
				obj.X += camera.X;
				obj.Y += camera.Y;
			}
			if (reader.MoveToAttribute("Code"))
			{
				(obj as ICodeObj).code = reader.Value;
			}
		}

		// Token: 0x0400009E RID: 158
		public static string FilePath = "";

		// Token: 0x02000027 RID: 39
		private class RoomData
		{
			// Token: 0x0600016C RID: 364 RVA: 0x0000A392 File Offset: 0x00008592
			public RoomData()
			{
				this.objList = new List<GameObj>();
			}

			// Token: 0x0600016D RID: 365 RVA: 0x0000A3A8 File Offset: 0x000085A8
			public void Dispose()
			{
				this.objList.Clear();
				this.objList = null;
				this.room = null;
			}

			// Token: 0x0400009F RID: 159
			public RoomObj room;

			// Token: 0x040000A0 RID: 160
			public List<GameObj> objList;
		}
	}
}

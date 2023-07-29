using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CDGEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BrawlerEditor
{
	// Token: 0x02000042 RID: 66
	public class PropertiesStackPanel : StackPanel, IControl
	{
		// Token: 0x17000081 RID: 129
		// (get) Token: 0x0600024B RID: 587 RVA: 0x000119A0 File Offset: 0x0000FBA0
		// (set) Token: 0x0600024C RID: 588 RVA: 0x000119B7 File Offset: 0x0000FBB7
		public MainWindow MainWindow { get; set; }

		// Token: 0x0600024D RID: 589 RVA: 0x000119C0 File Offset: 0x0000FBC0
		public void ShowProperties(GameObj obj)
		{
			this.ClearPanel(false);
			if (obj != null)
			{
				if (obj is DisplayObj)
				{
					TextBlock textBlock = new TextBlock();
					textBlock.Margin = new Thickness(0.0, 10.0, 0.0, 0.0);
					if (obj is EditorEnemyObj)
					{
						textBlock.Text = (obj as EditorEnemyObj).enemyType.ToString();
					}
					else
					{
						textBlock.Text = (obj as DisplayObj).spriteName;
					}
					base.Children.Add(textBlock);
				}
				this.m_selectedObj = obj;
				this.AddTextBox("Name:", obj.Name, "Name");
				this.AddTextBox("X:", obj.X.ToString(), "X");
				this.AddTextBox("Y:", obj.Y.ToString(), "Y");
				IScaleableObj scaleableObj = obj as IScaleableObj;
				if (scaleableObj != null)
				{
					this.AddTextBox("Width:", scaleableObj.Width.ToString(), "Width");
					this.AddTextBox("Height:", scaleableObj.Height.ToString(), "Height");
				}
				this.AddTextBox("Scale X:", obj.ScaleX.ToString(), "ScaleX");
				this.AddTextBox("Scale Y:", obj.ScaleY.ToString(), "ScaleY");
				this.AddTextBox("Rotation:", obj.Rotation.ToString(), "Rotation");
				IOpacityObj opacityObj = obj as IOpacityObj;
				if (opacityObj != null)
				{
					this.AddTextBox("Opacity: ", opacityObj.pureOpacity.ToString(), "Opacity");
				}
				this.AddTextBox("Tag:", obj.Tag.ToString(), "Tag");
				if (obj is DisplayObj)
				{
					this.AddCheckBox("Flip Horizontally", "Flip", obj.Flip == SpriteEffects.FlipHorizontally);
				}
				IJumpOverObj jumpOverObj = obj as IJumpOverObj;
				if (jumpOverObj != null)
				{
					this.AddCheckBox("Jumpable", "Jumpable", jumpOverObj.jumpable);
				}
				IAddPhysicsObj addPhysicsObj = obj as IAddPhysicsObj;
				if (addPhysicsObj != null)
				{
					this.AddCheckBox("Add To Physics Manager", "AddPhysics", addPhysicsObj.addPhysics);
				}
				IEditorDrawableObj editorDrawableObj = obj as IEditorDrawableObj;
				if (editorDrawableObj != null)
				{
					this.AddCheckBox("Force Add to Stage", "ForceAdd", editorDrawableObj.forceAddToStage);
				}
				this.AddSpecialProperties();
			}
		}

		// Token: 0x0600024E RID: 590 RVA: 0x00011C94 File Offset: 0x0000FE94
		public void ShowProperties(ObservableCollection<GameObj> list)
		{
			this.ClearPanel(false);
			this.AddTextBox("Scale Selected Objects By:", this.m_layerScaleBy.ToString(), "LayerScaleBy");
			this.AddButton("Apply ScaleBy Change", "LayerScaleBy");
			this.AddTextBox("Scale Selected Objects To:", this.m_layerScaleTo.ToString(), "LayerScaleTo");
			this.AddButton("Apply ScaleTo Change", "LayerScaleTo");
			this.AddTextBox("Change Opacity to Selected Objs:", this.m_opacityTo.ToString(), "OpacityTo");
			this.AddButton("Apply Opacity Change", "OpacityTo");
		}

		// Token: 0x0600024F RID: 591 RVA: 0x00011D34 File Offset: 0x0000FF34
		public void RefreshProperties()
		{
			RoomObj roomObj = null;
			foreach (GameObj gameObj in this.MainWindow.gameScreenControl.SelectedObjects)
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
				this.ShowProperties(roomObj);
			}
			else if (this.MainWindow.gameScreenControl.SelectedObjects.Count == 1)
			{
				this.ShowProperties(this.m_selectedObj);
			}
			else if (this.MainWindow.gameScreenControl.SelectedObjects.Count > 1)
			{
				this.ShowProperties(this.MainWindow.gameScreenControl.SelectedObjects);
			}
		}

		// Token: 0x06000250 RID: 592 RVA: 0x00011E2C File Offset: 0x0001002C
		public void AddSpecialProperties()
		{
			EditorCollHullObj editorCollHullObj = this.m_selectedObj as EditorCollHullObj;
			if (this.m_selectedObj is MarkerObj)
			{
				this.AddTextBox("ID:", (this.m_selectedObj as MarkerObj).ID.ToString(), "ID");
			}
			else if (editorCollHullObj != null)
			{
				this.AddCheckBox("Is Nav Mesh", "NavMesh", editorCollHullObj.isNavMesh);
				this.AddCheckBox("Is Arena Trigger", "ArenaTrigger", editorCollHullObj.isArenaTrigger);
				if (!editorCollHullObj.isNavMesh && !editorCollHullObj.isArenaTrigger)
				{
					this.AddCheckBox("Can Shoot Through", "CanShoot", editorCollHullObj.canShootThrough);
					this.AddCheckBox("Only Player Can Shoot Through", "PlayerCanShoot", editorCollHullObj.playerCanShootThrough);
					this.AddCheckBox("Only Collides with Players", "PlayersCollide", editorCollHullObj.onlyCollidesWithPlayers);
					this.AddCheckBox("Only Collides with Enemies", "EnemiesCollide", editorCollHullObj.onlyCollidesWithEnemies);
					this.AddCheckBox("Hides Engineer UI", "HideEngineerUI", editorCollHullObj.hidesEngineerUI);
				}
				if (!editorCollHullObj.isNavMesh)
				{
					string[] radioButtonNames = new string[] { "Trigger: None", "Trigger: Normal", "Trigger: GoingLeft", "Trigger: GoingRight", "Trigger: GoingUp", "Trigger: GoingDown" };
					string[] radioButtonIDs = new string[] { "TriggerNone", "TriggerNormal", "TriggerGoingLeft", "TriggerGoingRight", "TriggerGoingUp", "TriggerGoingDown" };
					bool[] array = new bool[6];
					bool[] array2 = array;
					array2[(int)editorCollHullObj.triggerType] = true;
					this.AddRadioButtons(radioButtonNames, radioButtonIDs, array2, "TriggerType");
					if (!editorCollHullObj.isArenaTrigger)
					{
						string[] radioButtonNames2 = new string[] { "Transition: NONE", "Enters Moving: LEFT", "Exits Moving: LEFT", "Enters Moving: UP", "Exits Moving: UP", "Enters Moving: RIGHT", "Exits Moving: RIGHT", "Enters Moving: DOWN", "Exits Moving: DOWN" };
						string[] radioButtonIDs2 = new string[] { "TransitionNone", "ArrivesRight", "LeavesLeft", "ArrivesBottom", "LeavesTop", "ArrivesLeft", "LeavesRight", "ArrivesTop", "LeavesBottom" };
						array = new bool[9];
						bool[] array3 = array;
						array3[(int)editorCollHullObj.transitionZoneType] = true;
						this.AddRadioButtons(radioButtonNames2, radioButtonIDs2, array3, "TransitionZoneType");
					}
				}
			}
			else if (this.m_selectedObj is RoomObj)
			{
				RoomObj roomObj = this.m_selectedObj as RoomObj;
				this.AddTextBox("Inner Rect Percent:", roomObj.innerZonePercent.ToString(), "InnerRectPercent");
				this.AddTextBox("Max Camera Zoom Out", roomObj.maxCameraZoomOut.ToString(), "MaxZoomOut");
				string[] radioButtonNames = new string[] { "InnerZone: Centre", "InnerZone: Left", "InnerZone: Right", "InnerZone: Top", "InnerZone: Bottom" };
				string[] radioButtonIDs = new string[] { "InnerZoneCentre", "InnerZoneLeft", "InnerZoneRight", "InnerZoneTop", "InnerZoneBottom" };
				bool[] array = new bool[5];
				bool[] array2 = array;
				array2[(int)roomObj.innerZonePos] = true;
				this.AddRadioButtons(radioButtonNames, radioButtonIDs, array2, "InnerZoneType");
				this.AddCheckBox("Force Red", "ForceRed", roomObj.forceRed);
				this.AddCheckBox("Force Black", "ForceBlack", roomObj.forceBlack);
				this.AddCheckBox("Force Green", "ForceGreen", roomObj.forceGreen);
				this.AddCheckBox("Hook Projectiles to NavMesh", "HookNavMesh", roomObj.hookProjectilesToNavMesh);
				this.AddCheckBox("Is Arena Zone", "IsArenaZone", roomObj.IsArenaZone);
				if (!roomObj.IsArenaZone)
				{
					this.AddCheckBox("Select All Objects", "RoomSelectAll", roomObj.SelectAllObjs);
				}
			}
		}

		// Token: 0x06000251 RID: 593 RVA: 0x000122A4 File Offset: 0x000104A4
		public void AddButton(string buttonText, string buttonName)
		{
			Button button = new Button();
			button.Margin = new Thickness(0.0, 10.0, 0.0, 0.0);
			button.Content = buttonText;
			button.Tag = buttonName;
			base.Children.Add(button);
			button.Click += this.ButtonEventHandler;
		}

		// Token: 0x06000252 RID: 594 RVA: 0x00012318 File Offset: 0x00010518
		private void ButtonEventHandler(object sender, RoutedEventArgs e)
		{
			Button button = sender as Button;
			Vector2 center = this.MainWindow.gameScreenControl.SelectionBounds.Center;
			List<Vector2> list = new List<Vector2>();
			List<Vector2> list2 = new List<Vector2>();
			string text = (string)button.Tag;
			if (text != null)
			{
				if (!(text == "LayerScaleBy") && !(text == "LayerScaleTo"))
				{
					if (text == "OpacityTo")
					{
						foreach (GameObj gameObj in this.MainWindow.gameScreenControl.SelectedObjects)
						{
							gameObj.Opacity = this.m_opacityTo;
						}
					}
				}
				else
				{
					foreach (GameObj gameObj in this.MainWindow.gameScreenControl.SelectedObjects)
					{
						list.Add(gameObj.Scale);
						list2.Add(gameObj.Position);
						Vector2 value = center - gameObj.AbsPosition;
						if ((string)button.Tag == "LayerScaleBy")
						{
							gameObj.Scale *= this.m_layerScaleBy;
							gameObj.Position += value * (1f - this.m_layerScaleBy);
						}
						else
						{
							float num = this.m_layerScaleTo / gameObj.ScaleX;
							gameObj.Scale = new Vector2(this.m_layerScaleTo);
							gameObj.Position += value * (1f - num);
						}
					}
					this.MainWindow.gameScreenControl.UndoManager.AddAction(new ScaleAllObjsUndoAction(this.MainWindow.gameScreenControl.SelectedObjects.ToList<GameObj>(), list, list2));
				}
			}
			this.MainWindow.gameScreenControl.RefreshCachedSelectionBounds();
		}

		// Token: 0x06000253 RID: 595 RVA: 0x00012578 File Offset: 0x00010778
		public void AddCheckBox(string checkBoxText, string checkBoxName, bool isChecked)
		{
			CheckBox checkBox = new CheckBox();
			checkBox.Name = checkBoxName;
			checkBox.IsChecked = new bool?(isChecked);
			checkBox.Margin = new Thickness(0.0, 0.0, 0.0, 0.0);
			checkBox.Checked += this.CheckBoxEventHandler;
			checkBox.Unchecked += this.CheckBoxEventHandler;
			base.Children.Add(checkBox);
			checkBox.Content = new TextBlock
			{
				Margin = new Thickness(0.0, 5.0, 0.0, 0.0),
				Text = checkBoxText
			};
		}

		// Token: 0x06000254 RID: 596 RVA: 0x0001264C File Offset: 0x0001084C
		public void AddTextBox(string textBlockName, string textBoxText, string textBoxName)
		{
			TextBlock textBlock = new TextBlock();
			textBlock.Margin = new Thickness(0.0, 10.0, 0.0, 0.0);
			textBlock.Text = textBlockName;
			base.Children.Add(textBlock);
			TextBox textBox = new TextBox();
			textBox.Text = textBoxText;
			textBox.Name = textBoxName;
			textBox.TextChanged += this.TextBoxEventHandler;
			textBox.PreviewMouseDoubleClick += new MouseButtonEventHandler(this.HighlightAllText);
			base.Children.Add(textBox);
		}

		// Token: 0x06000255 RID: 597 RVA: 0x000126F0 File Offset: 0x000108F0
		public void AddRadioButtons(string[] radioButtonNames, string[] radioButtonIDs, bool[] isChecked, string groupName)
		{
			if (radioButtonNames.Length != radioButtonIDs.Length && radioButtonIDs.Length != isChecked.Length)
			{
				throw new Exception("Cannot create radio buttons as the parameter counts do not match");
			}
			for (int i = 0; i < radioButtonNames.Length; i++)
			{
				RadioButton radioButton = new RadioButton();
				if (i == 0)
				{
					radioButton.Margin = new Thickness(0.0, 20.0, 0.0, 0.0);
				}
				else
				{
					radioButton.Margin = new Thickness(0.0, 5.0, 0.0, 0.0);
				}
				radioButton.Name = radioButtonIDs[i];
				radioButton.GroupName = groupName;
				radioButton.IsChecked = new bool?(isChecked[i]);
				radioButton.Checked += this.RadioButtonEventHandler;
				radioButton.Content = radioButtonNames[i];
				base.Children.Add(radioButton);
			}
		}

		// Token: 0x06000256 RID: 598 RVA: 0x000127FC File Offset: 0x000109FC
		private void TextBoxEventHandler(object sender, TextChangedEventArgs args)
		{
			TextBox textBox = sender as TextBox;
			float num = 0f;
			int id = 0;
			string name = textBox.Name;
			switch (name)
			{
			case "Name":
				this.m_selectedObj.Name = textBox.Text;
				break;
			case "X":
			{
				float num3 = this.m_selectedObj.X;
				if (float.TryParse(textBox.Text, out num))
				{
					this.m_selectedObj.X = num;
				}
				if (num3 != num)
				{
					this.MainWindow.gameScreenControl.UndoManager.AddAction(new MoveObjsUndoAction(this.m_selectedObj, new Vector2(num3, this.m_selectedObj.Y)));
				}
				break;
			}
			case "Y":
			{
				float num3 = this.m_selectedObj.Y;
				if (float.TryParse(textBox.Text, out num))
				{
					this.m_selectedObj.Y = num;
				}
				if (num3 != num)
				{
					this.MainWindow.gameScreenControl.UndoManager.AddAction(new MoveObjsUndoAction(this.m_selectedObj, new Vector2(this.m_selectedObj.X, num3)));
				}
				break;
			}
			case "ScaleX":
			{
				float num3 = this.m_selectedObj.ScaleX;
				if (float.TryParse(textBox.Text, out num))
				{
					this.m_selectedObj.ScaleX = num;
				}
				if (num3 != num)
				{
					this.MainWindow.gameScreenControl.UndoManager.AddAction(new ScaleUndoAction(this.m_selectedObj, new Vector2(num3, this.m_selectedObj.ScaleY), this.m_selectedObj.Position));
				}
				break;
			}
			case "ScaleY":
			{
				float num3 = this.m_selectedObj.ScaleY;
				if (float.TryParse(textBox.Text, out num))
				{
					this.m_selectedObj.ScaleY = num;
				}
				if (num3 != num)
				{
					this.MainWindow.gameScreenControl.UndoManager.AddAction(new ScaleUndoAction(this.m_selectedObj, new Vector2(this.m_selectedObj.ScaleX, num3), this.m_selectedObj.Position));
				}
				break;
			}
			case "Rotation":
			{
				float num3 = this.m_selectedObj.Rotation;
				if (float.TryParse(textBox.Text, out num))
				{
					this.m_selectedObj.Rotation = num;
				}
				if (num3 != num)
				{
					this.MainWindow.gameScreenControl.UndoManager.AddAction(new RotateObjUndoAction(this.m_selectedObj, num3));
				}
				break;
			}
			case "Width":
			{
				float num3 = (this.m_selectedObj as IScaleableObj).Width;
				if (float.TryParse(textBox.Text, out num))
				{
					(this.m_selectedObj as IScaleableObj).Width = num;
				}
				else
				{
					(this.m_selectedObj as IScaleableObj).Width = this.TestForSpecialKey(textBox.Text, textBox);
					if ((this.m_selectedObj as IScaleableObj).Width != 0f)
					{
						textBox.Text = (this.m_selectedObj as IScaleableObj).Width.ToString();
					}
				}
				if (num3 != num)
				{
					this.MainWindow.gameScreenControl.UndoManager.AddAction(new ScaleUndoAction(this.m_selectedObj, new Vector2(num3, (this.m_selectedObj as IScaleableObj).Height), this.m_selectedObj.Position));
				}
				break;
			}
			case "Height":
			{
				float num3 = (this.m_selectedObj as IScaleableObj).Height;
				if (float.TryParse(textBox.Text, out num))
				{
					(this.m_selectedObj as IScaleableObj).Height = num;
				}
				else
				{
					(this.m_selectedObj as IScaleableObj).Height = this.TestForSpecialKey(textBox.Text, textBox);
					if ((this.m_selectedObj as IScaleableObj).Height != 0f)
					{
						textBox.Text = (this.m_selectedObj as IScaleableObj).Height.ToString();
					}
				}
				if (num3 != num)
				{
					this.MainWindow.gameScreenControl.UndoManager.AddAction(new ScaleUndoAction(this.m_selectedObj, new Vector2((this.m_selectedObj as IScaleableObj).Width, num3), this.m_selectedObj.Position));
				}
				break;
			}
			case "ID":
				if (int.TryParse(textBox.Text, out id))
				{
					(this.m_selectedObj as MarkerObj).ID = id;
				}
				break;
			case "InnerRectPercent":
				if (float.TryParse(textBox.Text, out num))
				{
					(this.m_selectedObj as RoomObj).innerZonePercent = num;
				}
				break;
			case "Opacity":
				if (float.TryParse(textBox.Text, out num))
				{
					this.m_selectedObj.Opacity = num;
				}
				break;
			case "Tag":
				this.m_selectedObj.Tag = textBox.Text;
				break;
			case "LayerScaleBy":
				if (float.TryParse(textBox.Text, out num))
				{
					this.m_layerScaleBy = num;
				}
				break;
			case "LayerScaleTo":
				if (float.TryParse(textBox.Text, out num))
				{
					this.m_layerScaleTo = num;
				}
				break;
			case "OpacityTo":
				if (float.TryParse(textBox.Text, out num))
				{
					this.m_opacityTo = num;
				}
				break;
			case "MaxZoomOut":
				if (float.TryParse(textBox.Text, out num))
				{
					(this.m_selectedObj as RoomObj).maxCameraZoomOut = num;
				}
				break;
			}
			this.MainWindow.ChangesMade = true;
			this.MainWindow.gameScreenControl.RefreshCachedSelectionBounds();
		}

		// Token: 0x06000257 RID: 599 RVA: 0x00012ED0 File Offset: 0x000110D0
		private void CheckBoxEventHandler(object sender, RoutedEventArgs args)
		{
			CheckBox checkBox = sender as CheckBox;
			string name = checkBox.Name;
			switch (name)
			{
			case "HookNavMesh":
				if (checkBox.IsChecked == true)
				{
					(this.m_selectedObj as RoomObj).hookProjectilesToNavMesh = true;
				}
				else
				{
					(this.m_selectedObj as RoomObj).hookProjectilesToNavMesh = false;
				}
				break;
			case "EnemiesCollide":
				if (checkBox.IsChecked == true)
				{
					(this.m_selectedObj as EditorCollHullObj).onlyCollidesWithEnemies = true;
				}
				else
				{
					(this.m_selectedObj as EditorCollHullObj).onlyCollidesWithEnemies = false;
				}
				break;
			case "PlayersCollide":
				if (checkBox.IsChecked == true)
				{
					(this.m_selectedObj as EditorCollHullObj).onlyCollidesWithPlayers = true;
				}
				else
				{
					(this.m_selectedObj as EditorCollHullObj).onlyCollidesWithPlayers = false;
				}
				break;
			case "HideEngineerUI":
				if (checkBox.IsChecked == true)
				{
					(this.m_selectedObj as EditorCollHullObj).hidesEngineerUI = true;
				}
				else
				{
					(this.m_selectedObj as EditorCollHullObj).hidesEngineerUI = false;
				}
				break;
			case "CanShoot":
				if (checkBox.IsChecked == true)
				{
					(this.m_selectedObj as EditorCollHullObj).canShootThrough = true;
				}
				else
				{
					(this.m_selectedObj as EditorCollHullObj).canShootThrough = false;
				}
				break;
			case "PlayerCanShoot":
				if (checkBox.IsChecked == true)
				{
					(this.m_selectedObj as EditorCollHullObj).playerCanShootThrough = true;
				}
				else
				{
					(this.m_selectedObj as EditorCollHullObj).playerCanShootThrough = false;
				}
				break;
			case "AddPhysics":
				if (checkBox.IsChecked == true)
				{
					(this.m_selectedObj as IAddPhysicsObj).addPhysics = true;
				}
				else
				{
					(this.m_selectedObj as IAddPhysicsObj).addPhysics = false;
				}
				break;
			case "RoomSelectAll":
				if (checkBox.IsChecked == true)
				{
					(this.m_selectedObj as RoomObj).SelectAllObjs = true;
				}
				else
				{
					(this.m_selectedObj as RoomObj).SelectAllObjs = false;
				}
				break;
			case "IsArenaZone":
				if (checkBox.IsChecked == true)
				{
					(this.m_selectedObj as RoomObj).IsArenaZone = true;
				}
				else
				{
					(this.m_selectedObj as RoomObj).IsArenaZone = false;
				}
				break;
			case "Flip":
				if (checkBox.IsChecked == true)
				{
					this.m_selectedObj.Flip = SpriteEffects.FlipHorizontally;
				}
				else
				{
					this.m_selectedObj.Flip = SpriteEffects.None;
				}
				break;
			case "Jumpable":
				if (checkBox.IsChecked == true)
				{
					(this.m_selectedObj as IJumpOverObj).jumpable = true;
				}
				else
				{
					(this.m_selectedObj as IJumpOverObj).jumpable = false;
				}
				break;
			case "NavMesh":
				if (checkBox.IsChecked == true)
				{
					(this.m_selectedObj as EditorCollHullObj).isNavMesh = true;
				}
				else
				{
					(this.m_selectedObj as EditorCollHullObj).isNavMesh = false;
				}
				break;
			case "ArenaTrigger":
				if (checkBox.IsChecked == true)
				{
					(this.m_selectedObj as EditorCollHullObj).isArenaTrigger = true;
				}
				else
				{
					(this.m_selectedObj as EditorCollHullObj).isArenaTrigger = false;
				}
				break;
			case "ForceAdd":
				if (checkBox.IsChecked == true)
				{
					(this.m_selectedObj as IEditorDrawableObj).forceAddToStage = true;
				}
				else
				{
					(this.m_selectedObj as IEditorDrawableObj).forceAddToStage = false;
				}
				break;
			case "ForceRed":
				if (checkBox.IsChecked == true)
				{
					(this.m_selectedObj as RoomObj).forceRed = true;
					(this.m_selectedObj as RoomObj).forceBlack = false;
					(this.m_selectedObj as RoomObj).forceGreen = false;
				}
				else
				{
					(this.m_selectedObj as RoomObj).forceRed = false;
				}
				break;
			case "ForceBlack":
				if (checkBox.IsChecked == true)
				{
					(this.m_selectedObj as RoomObj).forceBlack = true;
					(this.m_selectedObj as RoomObj).forceRed = false;
					(this.m_selectedObj as RoomObj).forceGreen = false;
				}
				else
				{
					(this.m_selectedObj as RoomObj).forceBlack = false;
				}
				break;
			case "ForceGreen":
				if (checkBox.IsChecked == true)
				{
					(this.m_selectedObj as RoomObj).forceGreen = true;
					(this.m_selectedObj as RoomObj).forceRed = false;
					(this.m_selectedObj as RoomObj).forceBlack = false;
				}
				else
				{
					(this.m_selectedObj as RoomObj).forceGreen = false;
				}
				break;
			}
			GameObj selectedObj = this.m_selectedObj;
			this.MainWindow.gameScreenControl.DeselectAllGameObjs();
			this.MainWindow.gameScreenControl.SelectGameObj(selectedObj);
		}

		// Token: 0x06000258 RID: 600 RVA: 0x00013620 File Offset: 0x00011820
		private void RadioButtonEventHandler(object sender, RoutedEventArgs args)
		{
			RadioButton radioButton = sender as RadioButton;
			string name = radioButton.Name;
			switch (name)
			{
			case "TriggerNone":
				(this.m_selectedObj as EditorCollHullObj).triggerType = 0;
				break;
			case "TriggerNormal":
				(this.m_selectedObj as EditorCollHullObj).triggerType = 1;
				break;
			case "TriggerGoingLeft":
				(this.m_selectedObj as EditorCollHullObj).triggerType = 2;
				break;
			case "TriggerGoingRight":
				(this.m_selectedObj as EditorCollHullObj).triggerType = 3;
				break;
			case "TriggerGoingUp":
				(this.m_selectedObj as EditorCollHullObj).triggerType = 4;
				break;
			case "TriggerGoingDown":
				(this.m_selectedObj as EditorCollHullObj).triggerType = 5;
				break;
			case "InnerZoneCentre":
				(this.m_selectedObj as RoomObj).innerZonePos = 0;
				break;
			case "InnerZoneLeft":
				(this.m_selectedObj as RoomObj).innerZonePos = 1;
				break;
			case "InnerZoneRight":
				(this.m_selectedObj as RoomObj).innerZonePos = 2;
				break;
			case "InnerZoneTop":
				(this.m_selectedObj as RoomObj).innerZonePos = 3;
				break;
			case "InnerZoneBottom":
				(this.m_selectedObj as RoomObj).innerZonePos = 4;
				break;
			case "TransitionNone":
				(this.m_selectedObj as EditorCollHullObj).transitionZoneType = 0;
				break;
			case "LeavesLeft":
				(this.m_selectedObj as EditorCollHullObj).transitionZoneType = 2;
				break;
			case "LeavesRight":
				(this.m_selectedObj as EditorCollHullObj).transitionZoneType = 6;
				break;
			case "LeavesTop":
				(this.m_selectedObj as EditorCollHullObj).transitionZoneType = 4;
				break;
			case "LeavesBottom":
				(this.m_selectedObj as EditorCollHullObj).transitionZoneType = 8;
				break;
			case "ArrivesLeft":
				(this.m_selectedObj as EditorCollHullObj).transitionZoneType = 5;
				break;
			case "ArrivesRight":
				(this.m_selectedObj as EditorCollHullObj).transitionZoneType = 1;
				break;
			case "ArrivesTop":
				(this.m_selectedObj as EditorCollHullObj).transitionZoneType = 7;
				break;
			case "ArrivesBottom":
				(this.m_selectedObj as EditorCollHullObj).transitionZoneType = 3;
				break;
			}
			this.MainWindow.gameScreenControl.RefreshCachedSelectionBounds();
		}

		// Token: 0x06000259 RID: 601 RVA: 0x00013990 File Offset: 0x00011B90
		private float TestForSpecialKey(string text, TextBox textBox)
		{
			if (text.Length > 1)
			{
				string text2 = text.Substring(text.Length - 1);
				string s = text.Substring(0, text.Length - 1);
				float num = 0f;
				string text3 = text2;
				if (text3 != null)
				{
					if (!(text3 == "r"))
					{
						if (text3 == "g")
						{
							if (float.TryParse(s, out num))
							{
								return num * (float)this.MainWindow.gameScreenControl.GridUnitSize;
							}
						}
					}
					else if (float.TryParse(s, out num))
					{
						if (textBox.Name == "Width")
						{
							return num * 960f;
						}
						return num * 540f;
					}
				}
			}
			return 0f;
		}

		// Token: 0x0600025A RID: 602 RVA: 0x00013A78 File Offset: 0x00011C78
		private void HighlightAllText(object sender, RoutedEventArgs e)
		{
			TextBox textBox = sender as TextBox;
			textBox.SelectAll();
		}

		// Token: 0x0600025B RID: 603 RVA: 0x00013A94 File Offset: 0x00011C94
		public void ClearPanel(bool clearSelectedObj)
		{
			if (clearSelectedObj)
			{
				this.m_selectedObj = null;
			}
			base.Children.Clear();
		}

		// Token: 0x0400012E RID: 302
		private GameObj m_selectedObj;

		// Token: 0x0400012F RID: 303
		private float m_layerScaleBy = 1f;

		// Token: 0x04000130 RID: 304
		private float m_layerScaleTo = 1f;

		// Token: 0x04000131 RID: 305
		private float m_opacityTo = 1f;
	}
}

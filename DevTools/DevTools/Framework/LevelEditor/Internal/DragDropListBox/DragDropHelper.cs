// using System;
// using System.Collections;
// using System.Windows;
// using System.Windows.Controls;
// using System.Windows.Documents;
// using System.Windows.Input;
// using System.Windows.Media;
// using BrawlerEditor;
//
// namespace DragDropListBox
// {
// 	// Token: 0x02000032 RID: 50
// 	public class DragDropHelper
// 	{
// 		// Token: 0x17000062 RID: 98
// 		// (get) Token: 0x060001BD RID: 445 RVA: 0x0000D6E4 File Offset: 0x0000B8E4
// 		private static DragDropHelper Instance
// 		{
// 			get
// 			{
// 				if (DragDropHelper.instance == null)
// 				{
// 					DragDropHelper.instance = new DragDropHelper();
// 				}
// 				return DragDropHelper.instance;
// 			}
// 		}
//
// 		// Token: 0x060001BE RID: 446 RVA: 0x0000D718 File Offset: 0x0000B918
// 		public static bool GetIsDragSource(DependencyObject obj)
// 		{
// 			return (bool)obj.GetValue(DragDropHelper.IsDragSourceProperty);
// 		}
//
// 		// Token: 0x060001BF RID: 447 RVA: 0x0000D73A File Offset: 0x0000B93A
// 		public static void SetIsDragSource(DependencyObject obj, bool value)
// 		{
// 			obj.SetValue(DragDropHelper.IsDragSourceProperty, value);
// 		}
//
// 		// Token: 0x060001C0 RID: 448 RVA: 0x0000D750 File Offset: 0x0000B950
// 		public static bool GetIsDropTarget(DependencyObject obj)
// 		{
// 			return (bool)obj.GetValue(DragDropHelper.IsDropTargetProperty);
// 		}
//
// 		// Token: 0x060001C1 RID: 449 RVA: 0x0000D772 File Offset: 0x0000B972
// 		public static void SetIsDropTarget(DependencyObject obj, bool value)
// 		{
// 			obj.SetValue(DragDropHelper.IsDropTargetProperty, value);
// 		}
//
// 		// Token: 0x060001C2 RID: 450 RVA: 0x0000D788 File Offset: 0x0000B988
// 		public static DataTemplate GetDragDropTemplate(DependencyObject obj)
// 		{
// 			return (DataTemplate)obj.GetValue(DragDropHelper.DragDropTemplateProperty);
// 		}
//
// 		// Token: 0x060001C3 RID: 451 RVA: 0x0000D7AA File Offset: 0x0000B9AA
// 		public static void SetDragDropTemplate(DependencyObject obj, DataTemplate value)
// 		{
// 			obj.SetValue(DragDropHelper.DragDropTemplateProperty, value);
// 		}
//
// 		// Token: 0x060001C4 RID: 452 RVA: 0x0000D7BC File Offset: 0x0000B9BC
// 		private static void IsDragSourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
// 		{
// 			ItemsControl itemsControl = obj as ItemsControl;
// 			if (itemsControl != null)
// 			{
// 				if (object.Equals(e.NewValue, true))
// 				{
// 					itemsControl.PreviewMouseLeftButtonDown += DragDropHelper.Instance.DragSource_PreviewMouseLeftButtonDown;
// 					itemsControl.PreviewMouseLeftButtonUp += DragDropHelper.Instance.DragSource_PreviewMouseLeftButtonUp;
// 					itemsControl.PreviewMouseMove += DragDropHelper.Instance.DragSource_PreviewMouseMove;
// 				}
// 				else
// 				{
// 					itemsControl.PreviewMouseLeftButtonDown -= DragDropHelper.Instance.DragSource_PreviewMouseLeftButtonDown;
// 					itemsControl.PreviewMouseLeftButtonUp -= DragDropHelper.Instance.DragSource_PreviewMouseLeftButtonUp;
// 					itemsControl.PreviewMouseMove -= DragDropHelper.Instance.DragSource_PreviewMouseMove;
// 				}
// 			}
// 		}
//
// 		// Token: 0x060001C5 RID: 453 RVA: 0x0000D888 File Offset: 0x0000BA88
// 		private static void IsDropTargetChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
// 		{
// 			ItemsControl itemsControl = obj as ItemsControl;
// 			if (itemsControl != null)
// 			{
// 				if (object.Equals(e.NewValue, true))
// 				{
// 					itemsControl.AllowDrop = true;
// 					itemsControl.PreviewDrop += DragDropHelper.Instance.DropTarget_PreviewDrop;
// 					itemsControl.PreviewDragEnter += DragDropHelper.Instance.DropTarget_PreviewDragEnter;
// 					itemsControl.PreviewDragOver += DragDropHelper.Instance.DropTarget_PreviewDragOver;
// 					itemsControl.PreviewDragLeave += DragDropHelper.Instance.DropTarget_PreviewDragLeave;
// 				}
// 				else
// 				{
// 					itemsControl.AllowDrop = false;
// 					itemsControl.PreviewDrop -= DragDropHelper.Instance.DropTarget_PreviewDrop;
// 					itemsControl.PreviewDragEnter -= DragDropHelper.Instance.DropTarget_PreviewDragEnter;
// 					itemsControl.PreviewDragOver -= DragDropHelper.Instance.DropTarget_PreviewDragOver;
// 					itemsControl.PreviewDragLeave -= DragDropHelper.Instance.DropTarget_PreviewDragLeave;
// 				}
// 			}
// 		}
//
// 		// Token: 0x060001C6 RID: 454 RVA: 0x0000D994 File Offset: 0x0000BB94
// 		private void DragSource_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
// 		{
// 			this.sourceItemsControl = (ItemsControl)sender;
// 			Visual element = e.OriginalSource as Visual;
// 			this.topWindow = Window.GetWindow(this.sourceItemsControl);
// 			this.initialMousePosition = e.GetPosition(this.topWindow);
// 			this.sourceItemContainer = this.sourceItemsControl.ContainerFromElement(element) as FrameworkElement;
// 			if (this.sourceItemContainer != null)
// 			{
// 				this.draggedData = this.sourceItemContainer.DataContext;
// 			}
// 		}
//
// 		// Token: 0x060001C7 RID: 455 RVA: 0x0000DA14 File Offset: 0x0000BC14
// 		private void DragSource_PreviewMouseMove(object sender, MouseEventArgs e)
// 		{
// 			if (this.draggedData != null)
// 			{
// 				if (Utilities.IsMovementBigEnough(this.initialMousePosition, e.GetPosition(this.topWindow)))
// 				{
// 					this.initialMouseOffset = this.initialMousePosition - this.sourceItemContainer.TranslatePoint(new Point(0.0, 0.0), this.topWindow);
// 					DataObject data = new DataObject(this.format.Name, this.draggedData);
// 					bool allowDrop = this.topWindow.AllowDrop;
// 					this.topWindow.AllowDrop = true;
// 					this.topWindow.DragEnter += this.TopWindow_DragEnter;
// 					this.topWindow.DragOver += this.TopWindow_DragOver;
// 					this.topWindow.DragLeave += this.TopWindow_DragLeave;
// 					DragDropEffects dragDropEffects = DragDrop.DoDragDrop((DependencyObject)sender, data, DragDropEffects.Move);
// 					this.RemoveDraggedAdorner();
// 					this.topWindow.AllowDrop = allowDrop;
// 					this.topWindow.DragEnter -= this.TopWindow_DragEnter;
// 					this.topWindow.DragOver -= this.TopWindow_DragOver;
// 					this.topWindow.DragLeave -= this.TopWindow_DragLeave;
// 					this.draggedData = null;
// 				}
// 			}
// 		}
//
// 		// Token: 0x060001C8 RID: 456 RVA: 0x0000DB79 File Offset: 0x0000BD79
// 		private void DragSource_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
// 		{
// 			this.draggedData = null;
// 		}
//
// 		// Token: 0x060001C9 RID: 457 RVA: 0x0000DB84 File Offset: 0x0000BD84
// 		private void DropTarget_PreviewDragEnter(object sender, DragEventArgs e)
// 		{
// 			this.targetItemsControl = (ItemsControl)sender;
// 			object data = e.Data.GetData(this.format.Name);
// 			this.DecideDropTarget(e);
// 			if (data != null)
// 			{
// 				this.ShowDraggedAdorner(e.GetPosition(this.topWindow));
// 				this.CreateInsertionAdorner();
// 			}
// 			e.Handled = true;
// 		}
//
// 		// Token: 0x060001CA RID: 458 RVA: 0x0000DBEC File Offset: 0x0000BDEC
// 		private void DropTarget_PreviewDragOver(object sender, DragEventArgs e)
// 		{
// 			object data = e.Data.GetData(this.format.Name);
// 			this.DecideDropTarget(e);
// 			if (data != null)
// 			{
// 				this.ShowDraggedAdorner(e.GetPosition(this.topWindow));
// 				this.UpdateInsertionAdornerPosition();
// 			}
// 			e.Handled = true;
// 		}
//
// 		// Token: 0x060001CB RID: 459 RVA: 0x0000DC48 File Offset: 0x0000BE48
// 		private void DropTarget_PreviewDrop(object sender, DragEventArgs e)
// 		{
// 			object data = e.Data.GetData(this.format.Name);
// 			int num = -1;
// 			if (data != null)
// 			{
// 				if ((e.Effects & DragDropEffects.Move) != DragDropEffects.None)
// 				{
// 					num = Utilities.RemoveItemFromItemsControl(this.sourceItemsControl, data);
// 				}
// 				if (num != -1 && this.sourceItemsControl == this.targetItemsControl && num < this.insertionIndex)
// 				{
// 					this.insertionIndex--;
// 				}
// 				Utilities.InsertItemInItemsControl(this.targetItemsControl, data, this.insertionIndex);
// 				if (this.targetItemsControl is LayerTabControl)
// 				{
// 					(this.targetItemsControl as LayerTabControl).TabsRearranged(num, this.insertionIndex, true);
// 				}
// 				this.RemoveDraggedAdorner();
// 				this.RemoveInsertionAdorner();
// 			}
// 			e.Handled = true;
// 		}
//
// 		// Token: 0x060001CC RID: 460 RVA: 0x0000DD28 File Offset: 0x0000BF28
// 		private void DropTarget_PreviewDragLeave(object sender, DragEventArgs e)
// 		{
// 			object data = e.Data.GetData(this.format.Name);
// 			if (data != null)
// 			{
// 				this.RemoveInsertionAdorner();
// 			}
// 			e.Handled = true;
// 		}
//
// 		// Token: 0x060001CD RID: 461 RVA: 0x0000DD68 File Offset: 0x0000BF68
// 		private void DecideDropTarget(DragEventArgs e)
// 		{
// 			int count = this.targetItemsControl.Items.Count;
// 			object data = e.Data.GetData(this.format.Name);
// 			if (this.IsDropDataTypeAllowed(data))
// 			{
// 				if (count > 0)
// 				{
// 					this.hasVerticalOrientation = Utilities.HasVerticalOrientation(this.targetItemsControl.ItemContainerGenerator.ContainerFromIndex(0) as FrameworkElement);
// 					this.targetItemContainer = this.targetItemsControl.ContainerFromElement((DependencyObject)e.OriginalSource) as FrameworkElement;
// 					if (this.targetItemContainer != null)
// 					{
// 						Point position = e.GetPosition(this.targetItemContainer);
// 						this.isInFirstHalf = Utilities.IsInFirstHalf(this.targetItemContainer, position, this.hasVerticalOrientation);
// 						this.insertionIndex = this.targetItemsControl.ItemContainerGenerator.IndexFromContainer(this.targetItemContainer);
// 						if (!this.isInFirstHalf)
// 						{
// 							this.insertionIndex++;
// 						}
// 					}
// 					else
// 					{
// 						this.targetItemContainer = this.targetItemsControl.ItemContainerGenerator.ContainerFromIndex(count - 1) as FrameworkElement;
// 						this.isInFirstHalf = false;
// 						this.insertionIndex = count;
// 					}
// 				}
// 				else
// 				{
// 					this.targetItemContainer = null;
// 					this.insertionIndex = 0;
// 				}
// 			}
// 			else
// 			{
// 				this.targetItemContainer = null;
// 				this.insertionIndex = -1;
// 				e.Effects = DragDropEffects.None;
// 			}
// 		}
//
// 		// Token: 0x060001CE RID: 462 RVA: 0x0000DECC File Offset: 0x0000C0CC
// 		private bool IsDropDataTypeAllowed(object draggedItem)
// 		{
// 			IEnumerable itemsSource = this.targetItemsControl.ItemsSource;
// 			bool result;
// 			if (draggedItem != null)
// 			{
// 				if (itemsSource != null)
// 				{
// 					Type type = draggedItem.GetType();
// 					Type type2 = itemsSource.GetType();
// 					Type @interface = type2.GetInterface("IList`1");
// 					if (@interface != null)
// 					{
// 						Type[] genericArguments = @interface.GetGenericArguments();
// 						result = genericArguments[0].IsAssignableFrom(type);
// 					}
// 					else
// 					{
// 						result = typeof(IList).IsAssignableFrom(type2);
// 					}
// 				}
// 				else
// 				{
// 					result = true;
// 				}
// 			}
// 			else
// 			{
// 				result = false;
// 			}
// 			return result;
// 		}
//
// 		// Token: 0x060001CF RID: 463 RVA: 0x0000DF78 File Offset: 0x0000C178
// 		private void TopWindow_DragEnter(object sender, DragEventArgs e)
// 		{
// 			this.ShowDraggedAdorner(e.GetPosition(this.topWindow));
// 			e.Effects = DragDropEffects.None;
// 			e.Handled = true;
// 		}
//
// 		// Token: 0x060001D0 RID: 464 RVA: 0x0000DF9E File Offset: 0x0000C19E
// 		private void TopWindow_DragOver(object sender, DragEventArgs e)
// 		{
// 			this.ShowDraggedAdorner(e.GetPosition(this.topWindow));
// 			e.Effects = DragDropEffects.None;
// 			e.Handled = true;
// 		}
//
// 		// Token: 0x060001D1 RID: 465 RVA: 0x0000DFC4 File Offset: 0x0000C1C4
// 		private void TopWindow_DragLeave(object sender, DragEventArgs e)
// 		{
// 			this.RemoveDraggedAdorner();
// 			e.Handled = true;
// 		}
//
// 		// Token: 0x060001D2 RID: 466 RVA: 0x0000DFD8 File Offset: 0x0000C1D8
// 		private void ShowDraggedAdorner(Point currentPosition)
// 		{
// 			if (this.draggedAdorner == null)
// 			{
// 				AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this.sourceItemsControl);
// 				this.draggedAdorner = new DraggedAdorner(this.draggedData, DragDropHelper.GetDragDropTemplate(this.sourceItemsControl), this.sourceItemContainer, adornerLayer);
// 			}
// 			this.draggedAdorner.SetPosition(currentPosition.X - this.initialMousePosition.X + this.initialMouseOffset.X, currentPosition.Y - this.initialMousePosition.Y + this.initialMouseOffset.Y);
// 		}
//
// 		// Token: 0x060001D3 RID: 467 RVA: 0x0000E074 File Offset: 0x0000C274
// 		private void RemoveDraggedAdorner()
// 		{
// 			if (this.draggedAdorner != null)
// 			{
// 				this.draggedAdorner.Detach();
// 				this.draggedAdorner = null;
// 			}
// 		}
//
// 		// Token: 0x060001D4 RID: 468 RVA: 0x0000E0A4 File Offset: 0x0000C2A4
// 		private void CreateInsertionAdorner()
// 		{
// 			if (this.targetItemContainer != null)
// 			{
// 				AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this.targetItemContainer);
// 				this.insertionAdorner = new InsertionAdorner(this.hasVerticalOrientation, this.isInFirstHalf, this.targetItemContainer, adornerLayer);
// 			}
// 		}
//
// 		// Token: 0x060001D5 RID: 469 RVA: 0x0000E0EC File Offset: 0x0000C2EC
// 		private void UpdateInsertionAdornerPosition()
// 		{
// 			if (this.insertionAdorner != null)
// 			{
// 				this.insertionAdorner.IsInFirstHalf = this.isInFirstHalf;
// 				this.insertionAdorner.InvalidateVisual();
// 			}
// 		}
//
// 		// Token: 0x060001D6 RID: 470 RVA: 0x0000E128 File Offset: 0x0000C328
// 		private void RemoveInsertionAdorner()
// 		{
// 			if (this.insertionAdorner != null)
// 			{
// 				this.insertionAdorner.Detach();
// 				this.insertionAdorner = null;
// 			}
// 		}
//
// 		// Token: 0x040000C8 RID: 200
// 		private DataFormat format = DataFormats.GetDataFormat("DragDropItemsControl");
//
// 		// Token: 0x040000C9 RID: 201
// 		private Point initialMousePosition;
//
// 		// Token: 0x040000CA RID: 202
// 		private Vector initialMouseOffset;
//
// 		// Token: 0x040000CB RID: 203
// 		private object draggedData;
//
// 		// Token: 0x040000CC RID: 204
// 		private DraggedAdorner draggedAdorner;
//
// 		// Token: 0x040000CD RID: 205
// 		private InsertionAdorner insertionAdorner;
//
// 		// Token: 0x040000CE RID: 206
// 		private Window topWindow;
//
// 		// Token: 0x040000CF RID: 207
// 		private ItemsControl sourceItemsControl;
//
// 		// Token: 0x040000D0 RID: 208
// 		private FrameworkElement sourceItemContainer;
//
// 		// Token: 0x040000D1 RID: 209
// 		private ItemsControl targetItemsControl;
//
// 		// Token: 0x040000D2 RID: 210
// 		private FrameworkElement targetItemContainer;
//
// 		// Token: 0x040000D3 RID: 211
// 		private bool hasVerticalOrientation;
//
// 		// Token: 0x040000D4 RID: 212
// 		private int insertionIndex;
//
// 		// Token: 0x040000D5 RID: 213
// 		private bool isInFirstHalf;
//
// 		// Token: 0x040000D6 RID: 214
// 		private static DragDropHelper instance;
//
// 		// Token: 0x040000D7 RID: 215
// 		public static readonly DependencyProperty IsDragSourceProperty = DependencyProperty.RegisterAttached("IsDragSource", typeof(bool), typeof(DragDropHelper), new UIPropertyMetadata(false, new PropertyChangedCallback(DragDropHelper.IsDragSourceChanged)));
//
// 		// Token: 0x040000D8 RID: 216
// 		public static readonly DependencyProperty IsDropTargetProperty = DependencyProperty.RegisterAttached("IsDropTarget", typeof(bool), typeof(DragDropHelper), new UIPropertyMetadata(false, new PropertyChangedCallback(DragDropHelper.IsDropTargetChanged)));
//
// 		// Token: 0x040000D9 RID: 217
// 		public static readonly DependencyProperty DragDropTemplateProperty = DependencyProperty.RegisterAttached("DragDropTemplate", typeof(DataTemplate), typeof(DragDropHelper), new UIPropertyMetadata(null));
// 	}
// }

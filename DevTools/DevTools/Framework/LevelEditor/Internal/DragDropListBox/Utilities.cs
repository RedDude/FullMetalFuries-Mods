// using System;
// using System.Collections;
// using System.Windows;
// using System.Windows.Controls;
// using System.Windows.Media;
//
// namespace DragDropListBox
// {
// 	// Token: 0x02000044 RID: 68
// 	public static class Utilities
// 	{
// 		// Token: 0x06000264 RID: 612 RVA: 0x00013D24 File Offset: 0x00011F24
// 		public static bool HasVerticalOrientation(FrameworkElement itemContainer)
// 		{
// 			bool result = true;
// 			if (itemContainer != null)
// 			{
// 				Panel panel = VisualTreeHelper.GetParent(itemContainer) as Panel;
// 				StackPanel stackPanel;
// 				WrapPanel wrapPanel;
// 				if ((stackPanel = panel as StackPanel) != null)
// 				{
// 					result = stackPanel.Orientation == Orientation.Vertical;
// 				}
// 				else if ((wrapPanel = panel as WrapPanel) != null)
// 				{
// 					result = wrapPanel.Orientation == Orientation.Vertical;
// 				}
// 			}
// 			return result;
// 		}
//
// 		// Token: 0x06000265 RID: 613 RVA: 0x00013D90 File Offset: 0x00011F90
// 		public static void InsertItemInItemsControl(ItemsControl itemsControl, object itemToInsert, int insertionIndex)
// 		{
// 			if (itemToInsert != null)
// 			{
// 				IEnumerable itemsSource = itemsControl.ItemsSource;
// 				if (itemsSource == null)
// 				{
// 					itemsControl.Items.Insert(insertionIndex, itemToInsert);
// 				}
// 				else if (itemsSource is IList)
// 				{
// 					((IList)itemsSource).Insert(insertionIndex, itemToInsert);
// 				}
// 				else
// 				{
// 					Type type = itemsSource.GetType();
// 					Type @interface = type.GetInterface("IList`1");
// 					if (@interface != null)
// 					{
// 						type.GetMethod("Insert").Invoke(itemsSource, new object[] { insertionIndex, itemToInsert });
// 					}
// 				}
// 			}
// 		}
//
// 		// Token: 0x06000266 RID: 614 RVA: 0x00013E44 File Offset: 0x00012044
// 		public static int RemoveItemFromItemsControl(ItemsControl itemsControl, object itemToRemove)
// 		{
// 			int num = -1;
// 			if (itemToRemove != null)
// 			{
// 				num = itemsControl.Items.IndexOf(itemToRemove);
// 				if (num != -1)
// 				{
// 					IEnumerable itemsSource = itemsControl.ItemsSource;
// 					if (itemsSource == null)
// 					{
// 						itemsControl.Items.RemoveAt(num);
// 					}
// 					else if (itemsSource is IList)
// 					{
// 						((IList)itemsSource).RemoveAt(num);
// 					}
// 					else
// 					{
// 						Type type = itemsSource.GetType();
// 						Type @interface = type.GetInterface("IList`1");
// 						if (@interface != null)
// 						{
// 							type.GetMethod("RemoveAt").Invoke(itemsSource, new object[] { num });
// 						}
// 					}
// 				}
// 			}
// 			return num;
// 		}
//
// 		// Token: 0x06000267 RID: 615 RVA: 0x00013F1C File Offset: 0x0001211C
// 		public static bool IsInFirstHalf(FrameworkElement container, Point clickedPoint, bool hasVerticalOrientation)
// 		{
// 			bool result;
// 			if (hasVerticalOrientation)
// 			{
// 				result = clickedPoint.Y < container.ActualHeight / 2.0;
// 			}
// 			else
// 			{
// 				result = clickedPoint.X < container.ActualWidth / 2.0;
// 			}
// 			return result;
// 		}
//
// 		// Token: 0x06000268 RID: 616 RVA: 0x00013F6C File Offset: 0x0001216C
// 		public static bool IsMovementBigEnough(Point initialMousePosition, Point currentPosition)
// 		{
// 			return Math.Abs(currentPosition.X - initialMousePosition.X) >= SystemParameters.MinimumHorizontalDragDistance || Math.Abs(currentPosition.Y - initialMousePosition.Y) >= SystemParameters.MinimumVerticalDragDistance;
// 		}
// 	}
// }

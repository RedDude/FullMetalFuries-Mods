// using System;
// using System.Windows;
// using System.Windows.Controls;
// using System.Windows.Documents;
// using System.Windows.Media;
//
// namespace DragDropListBox
// {
// 	// Token: 0x02000033 RID: 51
// 	public class DraggedAdorner : Adorner
// 	{
// 		// Token: 0x060001D9 RID: 473 RVA: 0x0000E21C File Offset: 0x0000C41C
// 		public DraggedAdorner(object dragDropData, DataTemplate dragDropTemplate, UIElement adornedElement, AdornerLayer adornerLayer)
// 			: base(adornedElement)
// 		{
// 			this.adornerLayer = adornerLayer;
// 			this.contentPresenter = new ContentPresenter();
// 			this.contentPresenter.Content = dragDropData;
// 			this.contentPresenter.ContentTemplate = dragDropTemplate;
// 			this.contentPresenter.Opacity = 0.7;
// 			this.adornerLayer.Add(this);
// 		}
//
// 		// Token: 0x060001DA RID: 474 RVA: 0x0000E284 File Offset: 0x0000C484
// 		public void SetPosition(double left, double top)
// 		{
// 			this.left = left - 30.0;
// 			this.top = top - 20.0;
// 			if (this.adornerLayer != null)
// 			{
// 				this.adornerLayer.Update(base.AdornedElement);
// 			}
// 		}
//
// 		// Token: 0x060001DB RID: 475 RVA: 0x0000E2D8 File Offset: 0x0000C4D8
// 		protected override Size MeasureOverride(Size constraint)
// 		{
// 			this.contentPresenter.Measure(constraint);
// 			return this.contentPresenter.DesiredSize;
// 		}
//
// 		// Token: 0x060001DC RID: 476 RVA: 0x0000E304 File Offset: 0x0000C504
// 		protected override Size ArrangeOverride(Size finalSize)
// 		{
// 			this.contentPresenter.Arrange(new Rect(finalSize));
// 			return finalSize;
// 		}
//
// 		// Token: 0x060001DD RID: 477 RVA: 0x0000E32C File Offset: 0x0000C52C
// 		protected override Visual GetVisualChild(int index)
// 		{
// 			return this.contentPresenter;
// 		}
//
// 		// Token: 0x17000063 RID: 99
// 		// (get) Token: 0x060001DE RID: 478 RVA: 0x0000E344 File Offset: 0x0000C544
// 		protected override int VisualChildrenCount
// 		{
// 			get
// 			{
// 				return 1;
// 			}
// 		}
//
// 		// Token: 0x060001DF RID: 479 RVA: 0x0000E358 File Offset: 0x0000C558
// 		public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
// 		{
// 			return new GeneralTransformGroup
// 			{
// 				Children =
// 				{
// 					base.GetDesiredTransform(transform),
// 					new TranslateTransform(this.left, this.top)
// 				}
// 			};
// 		}
//
// 		// Token: 0x060001E0 RID: 480 RVA: 0x0000E3A1 File Offset: 0x0000C5A1
// 		public void Detach()
// 		{
// 			this.adornerLayer.Remove(this);
// 		}
//
// 		// Token: 0x040000DA RID: 218
// 		private ContentPresenter contentPresenter;
//
// 		// Token: 0x040000DB RID: 219
// 		private double left;
//
// 		// Token: 0x040000DC RID: 220
// 		private double top;
//
// 		// Token: 0x040000DD RID: 221
// 		private AdornerLayer adornerLayer;
// 	}
// }

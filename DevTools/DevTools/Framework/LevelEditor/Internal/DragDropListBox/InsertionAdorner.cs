// using System;
// using System.Windows;
// using System.Windows.Documents;
// using System.Windows.Media;
//
// namespace DragDropListBox
// {
// 	// Token: 0x02000034 RID: 52
// 	public class InsertionAdorner : Adorner
// 	{
// 		// Token: 0x17000064 RID: 100
// 		// (get) Token: 0x060001E1 RID: 481 RVA: 0x0000E3B4 File Offset: 0x0000C5B4
// 		// (set) Token: 0x060001E2 RID: 482 RVA: 0x0000E3CB File Offset: 0x0000C5CB
// 		public bool IsInFirstHalf { get; set; }
//
// 		// Token: 0x060001E3 RID: 483 RVA: 0x0000E3D4 File Offset: 0x0000C5D4
// 		static InsertionAdorner()
// 		{
// 			InsertionAdorner.pen.Freeze();
// 			LineSegment lineSegment = new LineSegment(new Point(0.0, -5.0), false);
// 			lineSegment.Freeze();
// 			LineSegment lineSegment2 = new LineSegment(new Point(0.0, 5.0), false);
// 			lineSegment2.Freeze();
// 			PathFigure pathFigure = new PathFigure
// 			{
// 				StartPoint = new Point(5.0, 0.0)
// 			};
// 			pathFigure.Segments.Add(lineSegment);
// 			pathFigure.Segments.Add(lineSegment2);
// 			pathFigure.Freeze();
// 			InsertionAdorner.triangle = new PathGeometry();
// 			InsertionAdorner.triangle.Figures.Add(pathFigure);
// 			InsertionAdorner.triangle.Freeze();
// 		}
//
// 		// Token: 0x060001E4 RID: 484 RVA: 0x0000E4CF File Offset: 0x0000C6CF
// 		public InsertionAdorner(bool isSeparatorHorizontal, bool isInFirstHalf, UIElement adornedElement, AdornerLayer adornerLayer)
// 			: base(adornedElement)
// 		{
// 			this.isSeparatorHorizontal = false;
// 			this.IsInFirstHalf = isInFirstHalf;
// 			this.adornerLayer = adornerLayer;
// 			base.IsHitTestVisible = false;
// 			this.adornerLayer.Add(this);
// 		}
//
// 		// Token: 0x060001E5 RID: 485 RVA: 0x0000E508 File Offset: 0x0000C708
// 		protected override void OnRender(DrawingContext drawingContext)
// 		{
// 			Point point;
// 			Point point2;
// 			this.CalculateStartAndEndPoint(out point, out point2);
// 			drawingContext.DrawLine(InsertionAdorner.pen, point, point2);
// 			if (this.isSeparatorHorizontal)
// 			{
// 				this.DrawTriangle(drawingContext, point, 0.0);
// 				this.DrawTriangle(drawingContext, point2, 180.0);
// 			}
// 			else
// 			{
// 				this.DrawTriangle(drawingContext, point, 90.0);
// 				this.DrawTriangle(drawingContext, point2, -90.0);
// 			}
// 		}
//
// 		// Token: 0x060001E6 RID: 486 RVA: 0x0000E58C File Offset: 0x0000C78C
// 		private void DrawTriangle(DrawingContext drawingContext, Point origin, double angle)
// 		{
// 			drawingContext.PushTransform(new TranslateTransform(origin.X, origin.Y));
// 			drawingContext.PushTransform(new RotateTransform(angle));
// 			drawingContext.DrawGeometry(InsertionAdorner.pen.Brush, null, InsertionAdorner.triangle);
// 			drawingContext.Pop();
// 			drawingContext.Pop();
// 		}
//
// 		// Token: 0x060001E7 RID: 487 RVA: 0x0000E5E8 File Offset: 0x0000C7E8
// 		private void CalculateStartAndEndPoint(out Point startPoint, out Point endPoint)
// 		{
// 			startPoint = default(Point);
// 			endPoint = default(Point);
// 			double width = base.AdornedElement.RenderSize.Width;
// 			double height = base.AdornedElement.RenderSize.Height;
// 			if (this.isSeparatorHorizontal)
// 			{
// 				endPoint.X = width;
// 				if (!this.IsInFirstHalf)
// 				{
// 					startPoint.Y = height;
// 					endPoint.Y = height;
// 				}
// 			}
// 			else
// 			{
// 				endPoint.Y = height;
// 				if (!this.IsInFirstHalf)
// 				{
// 					startPoint.X = width;
// 					endPoint.X = width;
// 				}
// 			}
// 		}
//
// 		// Token: 0x060001E8 RID: 488 RVA: 0x0000E687 File Offset: 0x0000C887
// 		public void Detach()
// 		{
// 			this.adornerLayer.Remove(this);
// 		}
//
// 		// Token: 0x040000DE RID: 222
// 		private bool isSeparatorHorizontal;
//
// 		// Token: 0x040000DF RID: 223
// 		private AdornerLayer adornerLayer;
//
// 		// Token: 0x040000E0 RID: 224
// 		private static Pen pen = new Pen
// 		{
// 			Brush = Brushes.Gray,
// 			Thickness = 2.0
// 		};
//
// 		// Token: 0x040000E1 RID: 225
// 		private static PathGeometry triangle;
// 	}
// }

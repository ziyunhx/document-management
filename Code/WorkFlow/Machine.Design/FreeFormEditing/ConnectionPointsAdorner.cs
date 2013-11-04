//----------------------------------------------------------------

//----------------------------------------------------------------

namespace Machine.Design.FreeFormEditing
{
    using System.Activities.Presentation;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;    

    sealed internal class ConnectionPointsAdorner : Adorner
    {
        List<ConnectionPoint> connectionPoints;
        bool isParentShapeSelected = false;
        public ConnectionPointsAdorner(UIElement adornedElement, List<ConnectionPoint> connectionPointsToShow, bool isParentShapeSelected)
            : base(adornedElement)
        {
            Debug.Assert(adornedElement != null, "adornedElement is null");
            this.IsHitTestVisible = true;
            connectionPoints = connectionPointsToShow;
            this.isParentShapeSelected = isParentShapeSelected;
            if (adornedElement is StateDesigner)
            {
                this.ToolTip = SR.ConnectionPointTooltip;
            }
        }
        protected override void OnRender(DrawingContext drawingContext)
        {
            SolidColorBrush renderBrush;
            Pen renderPen;
            if (this.isParentShapeSelected)
            {
                renderBrush = new SolidColorBrush(WorkflowDesignerColors.WorkflowViewElementSelectedBackgroundColor);
                renderPen = new Pen(new SolidColorBrush(WorkflowDesignerColors.WorkflowViewElementSelectedBorderColor), 1.0);
            }
            else
            {
                renderBrush = new SolidColorBrush(WorkflowDesignerColors.WorkflowViewElementBackgroundColor);
                renderPen = new Pen(new SolidColorBrush(WorkflowDesignerColors.WorkflowViewElementBorderColor), 1.0);
            }

            Point actualPoint;
            Point origin = FreeFormPanel.GetLocation(AdornedElement);
            Thickness margin = ((FrameworkElement)AdornedElement).Margin;
            origin.X += margin.Left;
            origin.Y += margin.Top;

            foreach (ConnectionPoint connPoint in connectionPoints)
            {
                actualPoint = new Point(connPoint.Location.X - origin.X, connPoint.Location.Y - origin.Y);
                DrawConnectionPoint(connPoint, actualPoint, renderBrush, renderPen, drawingContext);
            }
            
            base.OnRender(drawingContext);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            AdornedElement.RaiseEvent(e);
        }

        static void DrawConnectionPoint(ConnectionPoint connPoint, Point actualLocation, Brush renderBrush, Pen renderPen, DrawingContext drawingContext)
        {
            // actualLocation is the point on the Edge with respect to the coordinate system defined by the top left corner of the adorned element
            // We will need this transparent rectangle to make sure OnMouseOver event can be triggered, for hit test.
            drawingContext.DrawRectangle(Brushes.Transparent, new Pen(Brushes.Transparent, 0),
                new Rect(actualLocation + connPoint.HitTestOffset, connPoint.HitTestSize));
            drawingContext.DrawRectangle(renderBrush, renderPen,
                new Rect(actualLocation + connPoint.DrawingOffset, connPoint.DrawingSize));
        }
    }
}
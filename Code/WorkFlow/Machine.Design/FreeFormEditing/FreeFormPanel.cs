//------------------------------------------------------------

//------------------------------------------------------------

namespace Machine.Design.FreeFormEditing
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Threading;

    internal sealed class FreeFormPanel : Panel
    {
        public static readonly DependencyProperty ChildSizeProperty = DependencyProperty.RegisterAttached("ChildSize", typeof(Size), typeof(FreeFormPanel), new FrameworkPropertyMetadata());
        public static readonly DependencyProperty LocationProperty = DependencyProperty.RegisterAttached("Location", typeof(Point), typeof(FreeFormPanel), new FrameworkPropertyMetadata());
        public static readonly DependencyProperty RequiredWidthProperty = DependencyProperty.Register("RequiredWidth", typeof(Double), typeof(FreeFormPanel), new FrameworkPropertyMetadata(double.NaN));
        public static readonly DependencyProperty RequiredHeightProperty = DependencyProperty.Register("RequiredHeight", typeof(Double), typeof(FreeFormPanel), new FrameworkPropertyMetadata(double.NaN));
        public static readonly DependencyProperty DestinationConnectionPointProperty = DependencyProperty.RegisterAttached("DestinationConnectionPoint", typeof(ConnectionPoint), typeof(FreeFormPanel), new FrameworkPropertyMetadata());
        public static readonly DependencyProperty SourceConnectionPointProperty = DependencyProperty.RegisterAttached("SourceConnectionPoint", typeof(ConnectionPoint), typeof(FreeFormPanel), new FrameworkPropertyMetadata());
        public static readonly DependencyProperty DisabledProperty = DependencyProperty.Register("Disabled", typeof(bool), typeof(FreeFormPanel), new UIPropertyMetadata(false));

        public const double ConnectorEditorOpacity = 1.0;
        public const double ConnectorEditorThickness = 1.5;
        public const double LeftStackingMargin = 50;
        public const double TopStackingMargin = 80;
        public const double VerticalStackingDistance = 50;
        public ConnectorEditor connectorEditor;
        double lastYPosition;
        bool measureConnectors = false;
        bool measureConnectorsPosted = false;

        public FreeFormPanel()
        {
            this.SnapsToDevicePixels = true;
            this.AllowDrop = true;
            connectorEditor = null;
            lastYPosition = FreeFormPanel.TopStackingMargin;
        }

        public event LocationChangedEventHandler LocationChanged;
        public event ConnectorMovedEventHandler ConnectorMoved;
        public event RequiredSizeChangedEventHandler RequiredSizeChanged;

        public static Size GetChildSize(DependencyObject obj)
        {
            return (Size)obj.GetValue(FreeFormPanel.ChildSizeProperty);
        }

        public double RequiredHeight
        {
            get { return (double)GetValue(FreeFormPanel.RequiredHeightProperty); }
            private set { SetValue(FreeFormPanel.RequiredHeightProperty, value); }
        }

        public double RequiredWidth
        {
            get { return (double)GetValue(FreeFormPanel.RequiredWidthProperty); }
            private set { SetValue(FreeFormPanel.RequiredWidthProperty, value); }
        }

        public bool Disabled
        {
            get { return (bool)GetValue(DisabledProperty); }
            set { SetValue(DisabledProperty, value); }
        }

        public static ConnectionPoint GetDestinationConnectionPoint(DependencyObject obj)
        {
            return (ConnectionPoint)obj.GetValue(FreeFormPanel.DestinationConnectionPointProperty);
        }

        public static ConnectionPoint GetSourceConnectionPoint(DependencyObject obj)
        {
            return (ConnectionPoint)obj.GetValue(FreeFormPanel.SourceConnectionPointProperty);
        }

        public static void SetDestinationConnectionPoint(DependencyObject obj, ConnectionPoint connectionPoint)
        {
            obj.SetValue(FreeFormPanel.DestinationConnectionPointProperty, connectionPoint);
        }

        public static void SetSourceConnectionPoint(DependencyObject obj, ConnectionPoint connectionPoint)
        {
            obj.SetValue(FreeFormPanel.SourceConnectionPointProperty, connectionPoint);
        }

        public static Point GetLocation(DependencyObject obj)
        {
            return (Point)obj.GetValue(FreeFormPanel.LocationProperty);
        }
        public static void SetChildSize(DependencyObject obj, Size size)
        {
            obj.SetValue(FreeFormPanel.ChildSizeProperty, size);
        }

        public static void SetLocation(DependencyObject obj, Point point)
        {
            obj.SetValue(FreeFormPanel.LocationProperty, point);
        }

        public void UpdateConnectorPoints(Connector connector, List<Point> points)
        {
            PointCollection pointCollection = new PointCollection();
            foreach (Point point in points)
            {
                pointCollection.Add(new Point(point.X < 0 ? 0 : point.X, point.Y < 0 ? 0 : point.Y));
            }
            connector.Points = pointCollection;
            OnLocationChanged(connector, null);
        }

        static public List<Point> GetEdgeRelativeToOutmostPanel(ConnectionPoint connectionPoint)
        {
            FreeFormPanel parentPanel = StateContainerEditor.GetVisualAncestor<FreeFormPanel>(connectionPoint.ParentDesigner);
            List<Point> edge = new List<Point>();
            foreach (Point point in connectionPoint.Edge)
            {
                edge.Add(parentPanel.GetLocationRelativeToOutmostPanel(point));
            }
            return edge;
        }

        static public Point GetLocationRelativeToOutmostPanel(ConnectionPoint connectionPoint)
        {
            FreeFormPanel parentPanel = StateContainerEditor.GetVisualAncestor<FreeFormPanel>(connectionPoint.ParentDesigner);
            return parentPanel.GetLocationRelativeToOutmostPanel(connectionPoint.Location);
        }

        public Point GetLocationRelativeToOutmostPanel(Point location)
        {
            StateContainerEditor designer = StateContainerEditor.GetVisualAncestor<StateContainerEditor>(this);
            StateContainerEditor outmostEditor = designer.GetOutmostStateContainerEditor();
            return this.TranslatePoint(location, outmostEditor.Panel);
        }

        internal bool IsOutmostPanel()
        {
            StateContainerEditor designer = StateContainerEditor.GetVisualAncestor<StateContainerEditor>(this);
            if (designer != null)
            {
                StateContainerEditor outmostEditor = designer.GetOutmostStateContainerEditor();
                return outmostEditor != null && this == outmostEditor.Panel;
            }
            return false;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double height = 0;
            double width = 0;
            for (int i = 0; i < Children.Count; i++)
            {
                Point pt = new Point(0, 0);
                Size size = Children[i].DesiredSize;
                if (Children[i].GetType() == typeof(Connector))
                {
                    ((UIElement)Children[i]).Arrange(new Rect(pt, size));
                }
                else
                {
                    pt = FreeFormPanel.GetLocation(Children[i]);
                    ((UIElement)Children[i]).Arrange(new Rect(pt, size));
                }
                if (width < (size.Width + pt.X))
                {
                    width = size.Width + pt.X;
                }
                if (height < (size.Height + pt.Y))
                {
                    height = size.Height + pt.Y;
                }
            }
            width = (width < this.MinWidth) ? this.MinWidth : width;
            width = (width < this.Width) ? (this.Width < Double.MaxValue ? this.Width : width) : width;

            height = (height < this.MinHeight) ? this.MinHeight : height;
            height = (height < this.Height) ? (this.Height < Double.MaxValue ? this.Height : height) : height;

            return new Size(width, height);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            base.MeasureOverride(availableSize);
            double height;
            double width;
            this.MeasureChildren(out height, out width);
            if (this.RequiredSizeChanged != null)
            {
                this.RequiredSizeChanged(this, new RequiredSizeChangedEventArgs(new Size(width, height)));
            }
            this.RequiredWidth = width;
            this.RequiredHeight = height;
            
            if (this.IsOutmostPanel())
            {
                Action MeasureConnectors = () =>
                {
                    //This action will execute at Input priority. 
                    //Enabling measuring on Connectors and forcing a MeasureOverride by calling InvalidateMeasure.
                    this.measureConnectors = true;
                    this.InvalidateMeasure();
                };
                if (!measureConnectorsPosted)
                {
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Input, MeasureConnectors);
                    measureConnectorsPosted = true;
                }
                if (measureConnectors)
                {
                    measureConnectors = false;
                    measureConnectorsPosted = false;
                }
            }
            width = (width < this.Width) ? (this.Width < Double.MaxValue ? this.Width : width) : width;
            height = (height < this.Height) ? (this.Height < Double.MaxValue ? this.Height : height) : height;
            return new Size(width, height);
        }

        private void MeasureChildren(out double height, out double width)
        {
            height = 0;
            width = 0;
            Point pt = new Point(0, 0);
            bool isOutmostPanel = this.IsOutmostPanel();
            foreach (UIElement child in Children)
            {
                Connector connectorChild = child as Connector;
                if (connectorChild != null && isOutmostPanel)
                {
                    pt = new Point(0, 0);

                    if (measureConnectors)
                    {
                        Point srcPoint = FreeFormPanel.GetLocationRelativeToOutmostPanel(FreeFormPanel.GetSourceConnectionPoint(connectorChild));
                        Point destPoint = FreeFormPanel.GetLocationRelativeToOutmostPanel(FreeFormPanel.GetDestinationConnectionPoint(connectorChild));
                        if (connectorChild.Points.Count == 0
                            || (DesignerGeometryHelper.DistanceBetweenPoints(connectorChild.Points[0], srcPoint) > 1)
                            || (DesignerGeometryHelper.DistanceBetweenPoints(connectorChild.Points[connectorChild.Points.Count - 1], destPoint) > 1))
                        {
                            connectorChild.Points = new PointCollection();
                            RoutePolyLine(connectorChild);
                        }
                        connectorChild.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
                    }
                    else
                    {
                        continue;
                    }
                }
                else //Measure non-connector elements.
                {
                    child.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
                    if (!child.DesiredSize.Equals(((Size)FreeFormPanel.GetChildSize(child))))
                    {
                        FreeFormPanel.SetChildSize(child, child.DesiredSize);
                    }
                    pt = FreeFormPanel.GetLocation(child);
                    if (pt.X == 0 && pt.Y == 0)
                    {
                        pt = new Point(LeftStackingMargin, lastYPosition);
                        OnLocationChanged(child, new LocationChangedEventArgs(pt));
                        FreeFormPanel.SetLocation(child, pt);
                        lastYPosition += child.DesiredSize.Height + VerticalStackingDistance;
                    }
                }
                if (height < child.DesiredSize.Height + pt.Y)
                {
                    height = child.DesiredSize.Height + pt.Y;
                }
                if (width < child.DesiredSize.Width + pt.X)
                {
                    width = child.DesiredSize.Width + pt.X;
                }
            }

            width = (width < this.MinWidth) ? this.MinWidth : width;
            height = (height < this.MinHeight) ? this.MinHeight : height;
        }


        void OnLocationChanged(Object sender, LocationChangedEventArgs e)
        {
            if (LocationChanged != null)
            {
                LocationChanged(sender, e);
            }
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            if (e != null && !this.Disabled && this.IsOutmostPanel())
            {
                if (connectorEditor != null && connectorEditor.BeingEdited
                    && !(Mouse.DirectlyOver is ConnectionPointsAdorner))
                {
                    SaveConnectorEditor(e.GetPosition(this));
                }
            }
            base.OnMouseLeave(e);
        }

        protected override void OnMouseMove(System.Windows.Input.MouseEventArgs e)
        {
            if (e != null && !this.Disabled && this.IsOutmostPanel())
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    if (connectorEditor != null && connectorEditor.BeingEdited)
                    {
                        connectorEditor.Update(e.GetPosition(this));
                        e.Handled = true;
                    }
                }
            }
            base.OnMouseMove(e);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (e != null && !this.Disabled && this.IsOutmostPanel())
            {
                if (connectorEditor != null && connectorEditor.BeingEdited)
                {
                    SaveConnectorEditor(e.GetPosition(this));
                }
            }
            base.OnMouseLeftButtonUp(e);
        }

        public void RemoveConnectorEditor()
        {
            if (connectorEditor != null)
            {
                connectorEditor.Remove();
                connectorEditor = null;
            }

        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e != null && !this.Disabled && this.IsOutmostPanel())
            {
                if (e.Key == Key.Escape)
                {
                    //If escape key is hit while dragging a connector, end dragging.
                    if (connectorEditor != null && connectorEditor.BeingEdited)
                    {
                        Connector affectedConnector = connectorEditor.Connector;
                        RemoveConnectorEditor();
                        this.connectorEditor = new ConnectorEditor(this, affectedConnector);
                        e.Handled = true;
                    }
                }
            }
            base.OnKeyDown(e);
        }

        static bool ShouldCreateNewConnectorEditor(MouseButtonEventArgs e)
        {
            Connector connector = e.Source as Connector;
            // Don't create new connector editor when clicking on the start dot.
            if (connector == null || connector.startDotGrid.IsAncestorOf(e.MouseDevice.DirectlyOver as DependencyObject))
            {
                return false;
            }
            return true;
        }

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (e != null && !this.Disabled && this.IsOutmostPanel())
            {
                //If one of the edit points is clicked, update the connector editor.
                if ((connectorEditor != null) && connectorEditor.EditPointsHitTest(e.GetPosition(this)))
                {
                    connectorEditor.Update(e.GetPosition(this));
                    e.Handled = true;
                }
                else if (ShouldCreateNewConnectorEditor(e))
                {
                    CreateNewConnectorEditor(e);
                }
            }
            base.OnPreviewMouseLeftButtonDown(e);
        }

        void CreateNewConnectorEditor(MouseButtonEventArgs e)
        {
            if (connectorEditor == null || !e.Source.Equals(connectorEditor.Connector))
            {
                //If user clicks anywhere other than the connector editor, destroy it.
                RemoveConnectorEditor();
                if (e.Source.GetType().IsAssignableFrom(typeof(Connector)))
                {
                    this.connectorEditor = new ConnectorEditor(this, e.Source as Connector);
                }
            }
        }

        //Calls the Line routing algorithm and populates the points collection of the connector.
        void RoutePolyLine(Connector connector)
        {
            Point[] pts = ConnectorRouter.Route(this, FreeFormPanel.GetSourceConnectionPoint(connector), FreeFormPanel.GetDestinationConnectionPoint(connector));
            List<Point> points = new List<Point>(pts);
            if (pts != null)
            {
                UpdateConnectorPoints(connector, points);
            }
        }


        //Connector editing is complete, save the final connectorEditor state into the connector.
        void SaveConnectorEditor(Point pt)
        {
            if (!connectorEditor.Persist(pt))
            {
                //Persist will return false, when the ConnectionEndPoint has been moved.
                if (this.ConnectorMoved != null)
                {
                    Connector connector = this.connectorEditor.Connector;
                    List<Point> points = this.connectorEditor.ConnectorEditorLocation;
                    RemoveConnectorEditor();
                    ConnectorMoved(connector, new ConnectorMovedEventArgs(points));
                }
                else
                {
                    RemoveConnectorEditor();
                }
            }
            else
            {
                this.InvalidateMeasure();
            }
        }
    }
}


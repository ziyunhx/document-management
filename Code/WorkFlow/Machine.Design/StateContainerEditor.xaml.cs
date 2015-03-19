//----------------------------------------------------------------
//ok
//----------------------------------------------------------------

namespace Machine.Design
{
    using System;
    using System.Activities.Presentation;
    using System.Activities.Presentation.Model;
    using System.Activities.Presentation.Services;
    using System.Activities.Presentation.View;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Threading;
    using Machine.Design.FreeFormEditing;
    
    using Machine.Design.ToolboxItems;

    // The StateContainerEditor contains a FreeFormPanel that implements free form editing behaviors
    // for States and Transitions among them. An instance of StateMachineDesigner and an instance of
    // StateDesigner each contains an instance of StateContainerEditor to edit its child States and 
    // Transitions.
   public partial class StateContainerEditor
    {
        // Flag to indicate whether the editor has been populated
        bool populated = false;

        // To keep track of all child state designer
        Dictionary<ModelItem, UIElement> modelItemToUIElement;

        // shapeLocations is useful to avoid pasting on existing shapes.
        HashSet<Point> shapeLocations = null;

        // To keep track of transition collections that the outmost editor listens to the CollectionChanged events.
        HashSet<ModelItem> listenedTransitionCollections;

        // Flag whether the view state change has already been committed in the UI.
        bool internalViewStateChange = false;

        // activeSrcConnectionPoint is required for connector creation gesture to store the source of the link.
        ConnectionPoint activeSrcConnectionPoint;

        // selectedConnector is a placeholder for the last connector selected. 
        Connector selectedConnector = null;

        // Used for connector creation
        UIElement lastConnectionPointMouseUpElement = null;

        // Only used by the outmost editor to keep track of transitions added when editing scope completes
        List<ModelItem> transitionModelItemsAdded;

        // The outmost editor when the designer is populated.
        // This is used to find the outmost editor when this editor has been removed from the visual tree.
        StateContainerEditor outmostStateContainerEditor = null;

        // To keep track of whether the initial state is changed during an EditingScope
        bool initialStateChanged = false;

        // The initial node symbol
        UIElement initialNode = null;

        // To register / unregister the editor as the default composite view on its parent
        ICompositeViewEvents compositeViewEvents = null;

        // Constants
        const double startSymbolTopMargin = 10.0;
        const double GridSize = 10;
        const double DefaultStateWidth = 100;
        const double DefaultStateHeight = 25;
        const double DefaultStateMachineWidth = 600;
        const double DefaultStateMachineHeight = 600;
        const double InitialFinalWidth = 60;
        const double InitialFinalHeight = 75;
        const double ChildViewMinWidth = 20;
        const string ShapeLocationViewStateKey = "ShapeLocation";
        const string ShapeSizeViewStateKey = "ShapeSize";
        internal const string ConnectorLocationViewStateKey = "ConnectorLocation";
        internal const string StateContainerWidthViewStateKey = "StateContainerWidth";
        internal const string StateContainerHeightViewStateKey = "StateContainerHeight";
        internal const string ChildStatesPropertyName = "States";

        public static readonly DependencyProperty StateContainerWidthProperty = DependencyProperty.Register(
            "StateContainerWidth",
            typeof(double),
            typeof(StateContainerEditor),
            new FrameworkPropertyMetadata(DefaultStateWidth));

        public static readonly DependencyProperty StateContainerHeightProperty = DependencyProperty.Register(
            "StateContainerHeight",
            typeof(double),
            typeof(StateContainerEditor),
            new FrameworkPropertyMetadata(DefaultStateHeight));

        public static readonly DependencyProperty PanelMinWidthProperty = DependencyProperty.Register(
            "PanelMinWidth",
            typeof(double),
            typeof(StateContainerEditor),
            new FrameworkPropertyMetadata());

        public static readonly DependencyProperty PanelMinHeightProperty = DependencyProperty.Register(
            "PanelMinHeight",
            typeof(double),
            typeof(StateContainerEditor),
            new FrameworkPropertyMetadata());

        public static readonly DependencyProperty ConnectorModelItemProperty = DependencyProperty.RegisterAttached(
            "ConnectorModelItem",
            typeof(ModelItem),
            typeof(StateContainerEditor),
            new FrameworkPropertyMetadata());

        public static readonly DependencyProperty ConnectionPointsProperty = DependencyProperty.RegisterAttached(
            "ConnectionPoints",
            typeof(List<ConnectionPoint>),
            typeof(StateContainerEditor),
            new FrameworkPropertyMetadata());

        public static readonly DependencyProperty ModelItemProperty = DependencyProperty.Register(
            "ModelItem",
            typeof(ModelItem),
            typeof(StateContainerEditor),
            new FrameworkPropertyMetadata());

        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register(
            "IsReadOnly",
            typeof(bool), typeof(StateContainerEditor),
            new FrameworkPropertyMetadata(false));

        public StateContainerEditor()
        {
            InitializeComponent();
            this.DataContext = this;
            this.modelItemToUIElement = new Dictionary<ModelItem, UIElement>();
            this.shapeLocations = new HashSet<Point>();
            this.listenedTransitionCollections = new HashSet<ModelItem>();
            this.transitionModelItemsAdded = new List<ModelItem>();

            Binding readOnlyBinding = new Binding();
            readOnlyBinding.RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(DesignerView), 1);
            readOnlyBinding.Path = new PropertyPath(DesignerView.IsReadOnlyProperty);
            readOnlyBinding.Mode = BindingMode.OneWay;
            this.SetBinding(IsReadOnlyProperty, readOnlyBinding);

            this.Loaded += (s, e) =>
            {
                if (this.ShouldInitialize())
                {
                    WorkflowViewElement parent = StateContainerEditor.GetVisualAncestor<WorkflowViewElement>(this);
                    this.ModelItem = parent.ModelItem;
                    this.StateMachineModelItem = StateContainerEditor.GetStateMachineModelItem(this.ModelItem);
                    this.Context = parent.Context;
                    this.compositeViewEvents = parent;
                    if (this.compositeViewEvents != null)
                    {
                        this.compositeViewEvents.RegisterDefaultCompositeView(this);
                    }
                    if (!this.populated)
                    {
                        this.Populate();
                        this.populated = true;
                    }
                }
            };

            this.Unloaded += (s, e) =>
            {
                if (this.compositeViewEvents != null)
                {
                    (compositeViewEvents).UnregisterDefaultCompositeView(this);
                    this.compositeViewEvents = null;
                }
                if (this.populated)
                {
                    this.Cleanup();
                    this.populated = false;
                }
            };
        }

        internal FreeFormPanel Panel
        {
            get
            {
                return this.panel;
            }
        }

        ViewStateService ViewStateService
        {
            get
            {
                ViewStateService viewStateService = this.Context.Services.GetService<ViewStateService>();
                return viewStateService;
            }
        }

        DesignerView DesignerView
        {
            get
            {
                return this.Context.Services.GetService<DesignerView>();
            }
        }

        public double StateContainerWidth
        {
            get { return (double)this.GetValue(StateContainerEditor.StateContainerWidthProperty); }
            set { this.SetValue(StateContainerEditor.StateContainerWidthProperty, value); }
        }

        public double StateContainerHeight
        {
            get { return (double)this.GetValue(StateContainerEditor.StateContainerHeightProperty); }
            set { this.SetValue(StateContainerEditor.StateContainerHeightProperty, value); }
        }

        public double PanelMinWidth
        {
            get { return (double)this.GetValue(StateContainerEditor.PanelMinWidthProperty); }
            set { this.SetValue(StateContainerEditor.PanelMinWidthProperty, value); }
        }

        public double PanelMinHeight
        {
            get { return (double)this.GetValue(StateContainerEditor.PanelMinHeightProperty); }
            set { this.SetValue(StateContainerEditor.PanelMinHeightProperty, value); }
        }

        public ModelItem ModelItem
        {
            get { return (ModelItem)GetValue(ModelItemProperty); }
            set { SetValue(ModelItemProperty, value); }
        }

        protected bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            private set { SetValue(IsReadOnlyProperty, value); }
        }

        public EditingContext Context
        {
            get;
            set;
        }

        ModelItem StateMachineModelItem
        {
            get;
            set;
        }

        #region PopulateCleanup

        void Populate()
        {
            // Keep track of the outmost editor, which may not be accessible by traversing the visual tree when the designer is deleted.
            this.outmostStateContainerEditor = this.GetOutmostStateContainerEditor();

            this.panel.LocationChanged += new LocationChangedEventHandler(OnFreeFormPanelLocationChanged);
            this.panel.ConnectorMoved += new ConnectorMovedEventHandler(OnFreeFormPanelConnectorMoved);
            this.panel.LayoutUpdated += new EventHandler(OnFreeFormPanelLayoutUpdated);
            this.panel.RequiredSizeChanged += new RequiredSizeChangedEventHandler(OnFreeFormPanelRequiredSizeChanged);

            this.ViewStateService.ViewStateChanged += new ViewStateChangedEventHandler(OnViewStateChanged);

            this.ModelItem.Properties[ChildStatesPropertyName].Collection.CollectionChanged += new NotifyCollectionChangedEventHandler(OnStateCollectionChanged);
            this.ModelItem.PropertyChanged += new PropertyChangedEventHandler(this.OnModelPropertyChanged);

            ModelTreeManager modelTreeManager = this.Context.Services.GetService<ModelTreeManager>();
            modelTreeManager.EditingScopeCompleted += new EventHandler<EditingScopeEventArgs>(this.outmostStateContainerEditor.OnEditingScopeCompleted);

            if (this.ModelItem.ItemType == typeof(State))
            {
                ModelItemCollection transitions = this.ModelItem.Properties[StateDesigner.TransitionsPropertyName].Collection;
                if (!this.outmostStateContainerEditor.listenedTransitionCollections.Contains(transitions))
                {
                    transitions.CollectionChanged += new NotifyCollectionChangedEventHandler(this.outmostStateContainerEditor.OnTransitionCollectionChanged);
                    this.outmostStateContainerEditor.listenedTransitionCollections.Add(transitions);
                }
            }

            object widthViewState = this.ViewStateService.RetrieveViewState(this.ModelItem, StateContainerWidthViewStateKey);
            if (widthViewState != null)
            {
                this.StateContainerWidth = (double)widthViewState;
            }

            object heightViewState = this.ViewStateService.RetrieveViewState(this.ModelItem, StateContainerHeightViewStateKey);
            if (heightViewState != null)
            {
                this.StateContainerHeight = (double)heightViewState;
            }


            panel.Children.Clear();
            this.modelItemToUIElement.Clear();
            this.shapeLocations.Clear();

            this.AddStateVisuals(this.ModelItem.Properties[ChildStatesPropertyName].Collection);

            if (this.ModelItem.ItemType == typeof(StateMachine))
            {
                this.AddInitialNode();
            }

            // We need to wait until after the state visuals are added and displayed.
            this.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(() =>
            {
                if (this.ModelItem.ItemType == typeof(State))
                {
                    this.AddChildTransitionVisualsToOutmostEditor();
                    ModelItem stateMachineModelItem = StateContainerEditor.GetStateMachineModelItem(this.ModelItem);
                    if (stateMachineModelItem.Properties[StateMachineDesigner.InitialStatePropertyName].Value == this.ModelItem)
                    {
                        this.outmostStateContainerEditor.AddInitialNodeConnector();
                    }
                }
            }));
        }

        void Cleanup()
        {
            this.panel.Children.Clear();
            // Cleaning up the designers as they might be re-used.
            foreach (UIElement element in this.modelItemToUIElement.Values)
            {
                element.MouseEnter -= new MouseEventHandler(OnChildElementMouseEnter);
                element.MouseLeave -= new MouseEventHandler(OnChildElementMouseLeave);
                ((FrameworkElement)element).SizeChanged -= new SizeChangedEventHandler(OnChildElementSizeChanged);
            }
            this.modelItemToUIElement.Clear();
            this.panel.LocationChanged -= new LocationChangedEventHandler(OnFreeFormPanelLocationChanged);
            this.panel.ConnectorMoved -= new ConnectorMovedEventHandler(OnFreeFormPanelConnectorMoved);
            this.panel.LayoutUpdated -= new EventHandler(OnFreeFormPanelLayoutUpdated);
            this.panel.RequiredSizeChanged -= new RequiredSizeChangedEventHandler(OnFreeFormPanelRequiredSizeChanged);
            this.ViewStateService.ViewStateChanged -= new ViewStateChangedEventHandler(OnViewStateChanged);
            this.ModelItem.Properties[ChildStatesPropertyName].Collection.CollectionChanged -= new NotifyCollectionChangedEventHandler(OnStateCollectionChanged);
            this.ModelItem.PropertyChanged -= new PropertyChangedEventHandler(this.OnModelPropertyChanged);

            ModelTreeManager modelTreeManager = this.Context.Services.GetService<ModelTreeManager>();
            modelTreeManager.EditingScopeCompleted -= new EventHandler<EditingScopeEventArgs>(this.outmostStateContainerEditor.OnEditingScopeCompleted);

            if (this.ModelItem.ItemType == typeof(State))
            {
                ModelItemCollection transitions = this.ModelItem.Properties[StateDesigner.TransitionsPropertyName].Collection;
                if (this.listenedTransitionCollections.Contains(transitions))
                {
                    transitions.CollectionChanged -= new NotifyCollectionChangedEventHandler(this.outmostStateContainerEditor.OnTransitionCollectionChanged);
                    this.outmostStateContainerEditor.listenedTransitionCollections.Remove(transitions);
                }
            }
        }

        #endregion

        #region InitialNode

        void AddInitialNode()
        {
            // Instantiate the initial node
            InitialNode initialNode = new InitialNode();
            SetInitialFinalViewSize(initialNode);
            this.initialNode = initialNode;
            this.PopulateConnectionPoints(this.initialNode);
            this.initialNode.MouseEnter += new MouseEventHandler(OnChildElementMouseEnter);
            this.initialNode.MouseLeave += new MouseEventHandler(OnChildElementMouseLeave);
            // Add the initial node and set its location and size.
            this.initialNode.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
            double startHeight = this.initialNode.DesiredSize.Height;
            double startWidth = this.initialNode.DesiredSize.Width;
            Point startPoint = new Point(panel.MinWidth / 2, startSymbolTopMargin + startHeight / 2);
            Point startLocation = SnapVisualToGrid(this.initialNode, startPoint, new Point(-1, -1));
            this.panel.Children.Add(this.initialNode);
            FreeFormPanel.SetLocation(this.initialNode, startLocation);
            FreeFormPanel.SetChildSize(this.initialNode, new Size(startWidth, startHeight));
           
        }

        void AddInitialNodeConnector()
        {
            Debug.Assert(this.ModelItem.ItemType == typeof(StateMachine), "Only StateMachine should have initial state.");
            List<Connector> attachedConnectors = StateContainerEditor.GetAttachedConnectors(this.initialNode);
            if (attachedConnectors.Count == 0)
            {
                this.AddConnector(this.initialNode, (UIElement)this.ModelItem.Properties[StateMachineDesigner.InitialStatePropertyName].Value.View, this.ModelItem);
            }
        }

        #endregion

        #region StateVisuals

        UIElement ProcessAndGetModelView(ModelItem model)
        {
            UIElement element;
            if (!this.modelItemToUIElement.TryGetValue(model, out element))
            {
                // Use VirtualizedContainerService for ModelItem.Focus to work.
                element = this.Context.Services.GetService<VirtualizedContainerService>().GetViewElement(model, this);

                SetChildViewSize((ContentControl)element, model);

                element.MouseEnter += new MouseEventHandler(OnChildElementMouseEnter);
                element.MouseLeave += new MouseEventHandler(OnChildElementMouseLeave);
                ((FrameworkElement)element).SizeChanged += new SizeChangedEventHandler(OnChildElementSizeChanged);

                this.modelItemToUIElement.Add(model, element);

                this.PopulateConnectionPoints(element);

                object locationOfShape = this.ViewStateService.RetrieveViewState(model, ShapeLocationViewStateKey);
                object sizeOfShape = this.ViewStateService.RetrieveViewState(model, ShapeSizeViewStateKey);
                if (locationOfShape != null)
                {
                    Point locationPt = (Point)locationOfShape;
                    FreeFormPanel.SetLocation(element, locationPt);
                    this.shapeLocations.Add(locationPt);
                }
                if (sizeOfShape != null)
                {
                    FreeFormPanel.SetChildSize(element, (Size)sizeOfShape);
                }
            }
            return element;
        }

        void AddStateVisuals(IList<ModelItem> modelItemCollection)
        {
            List<UIElement> viewsAdded = new List<UIElement>();
            foreach (ModelItem modelItem in modelItemCollection)
            {
                if (!this.modelItemToUIElement.ContainsKey(modelItem))
                {
                    viewsAdded.Add(ProcessAndGetModelView(modelItem));
                }
                else if (!this.panel.Children.Contains(this.modelItemToUIElement[modelItem]))
                {
                    viewsAdded.Add(this.modelItemToUIElement[modelItem]);
                }
            }
            foreach (UIElement view in viewsAdded)
            {
                this.panel.Children.Add(view);
            }
        }

        void RemoveStateVisual(WorkflowViewElement removedStateDesigner)
        {
            HashSet<Connector> connectorsToDelete = new HashSet<Connector>();
            ModelService modelService = this.Context.Services.GetService<ModelService>();
            List<UIElement> removedStateDesigners = new List<UIElement>();
            removedStateDesigners.Add(removedStateDesigner);
            removedStateDesigners.AddRange(GetAllChildStateDesigners(removedStateDesigner));

            StateContainerEditor outmostEditor = this.GetOutmostStateContainerEditor();
            foreach (UIElement designer in removedStateDesigners)
            {
                if (outmostEditor.activeSrcConnectionPoint != null)
                {
                    List<ConnectionPoint> connectionPoints = GetConnectionPoints(designer);
                    if (connectionPoints.Contains(outmostEditor.activeSrcConnectionPoint))
                    {
                        outmostEditor.activeSrcConnectionPoint = null;
                        RemoveAdorner(outmostEditor.panel, typeof(ConnectorCreationAdorner));
                    }
                }
                if (outmostEditor.lastConnectionPointMouseUpElement == designer)
                {
                    outmostEditor.lastConnectionPointMouseUpElement = null;
                }
                connectorsToDelete.UnionWith(GetAttachedConnectors(designer));
            }

            // Remove any connector visuals attached to this shape. This is required for the scenarios as follows:
            // Copy and paste two connected States into StateMachine and undo the paste. 
            // The Transition is not removed as a model change. Hence the connector visual will remain dangling on the designer.
            foreach (Connector connector in connectorsToDelete)
            {
                this.RemoveConnectorOnOutmostEditor(connector);
            }

            this.modelItemToUIElement.Remove(removedStateDesigner.ModelItem);
            removedStateDesigner.MouseEnter -= new MouseEventHandler(OnChildElementMouseEnter);
            removedStateDesigner.MouseLeave -= new MouseEventHandler(OnChildElementMouseLeave);
            ((FrameworkElement)removedStateDesigner).SizeChanged -= new SizeChangedEventHandler(OnChildElementSizeChanged);

            this.panel.Children.Remove(removedStateDesigner);

            // deselect removed item
            if (this.Context != null)
            {
                HashSet<ModelItem> selectedItems = new HashSet<ModelItem>(this.Context.Items.GetValue<Selection>().SelectedObjects);
                if (selectedItems.Contains(removedStateDesigner.ModelItem))
                {
                    Selection.Toggle(this.Context, removedStateDesigner.ModelItem);
                }
            }

            object locationOfShape = this.ViewStateService.RetrieveViewState(removedStateDesigner.ModelItem, StateContainerEditor.ShapeLocationViewStateKey);
            if (locationOfShape != null)
            {
                this.shapeLocations.Remove((Point)locationOfShape);
            }
        }

        #endregion

        #region TransitionVisualsAndConnector

        void AddTransitionVisual(ModelItem transitionModelItem)
        {
            Debug.Assert(this.IsOutmostStateContainerEditor(), "Should only be called by the outmost editor.");
            UIElement sourceDesigner = StateContainerEditor.GetParentStateModelItemForTransition(transitionModelItem).View as UIElement;
            UIElement destinationDesigner = transitionModelItem.Properties[TransitionDesigner.ToPropertyName].Value.View as UIElement;
            if (sourceDesigner.IsDescendantOf(this) && destinationDesigner.IsDescendantOf(this))
            {
                this.AddConnector(sourceDesigner, destinationDesigner, transitionModelItem);
            }
        }

        void AddTransitionVisuals(IList<ModelItem> transitionModelItemCollection)
        {
            Debug.Assert(this.IsOutmostStateContainerEditor(), "Should only be called by the outmost editor.");
            foreach (ModelItem transitionModelItem in transitionModelItemCollection)
            {
                this.AddTransitionVisual(transitionModelItem);
            }
        }

        void AddChildTransitionVisualsToOutmostEditor()
        {
            Debug.Assert(this.ModelItem.ItemType == typeof(State), "This should be a designer for State.");
            List<ModelItem> transitions = new List<ModelItem>();
            transitions.AddRange(this.ModelItem.Properties[StateDesigner.TransitionsPropertyName].Collection);
            this.GetOutmostStateContainerEditor().AddTransitionVisuals(transitions);
        }

        Connector CreateConnector(ConnectionPoint srcConnPoint, ConnectionPoint destConnPoint, PointCollection points, ModelItem connectorModelItem)
        {
            Debug.Assert(this.IsOutmostStateContainerEditor(), "Should only be called by the outmost editor.");
            Connector connector = new Connector();
            connector.FocusVisualStyle = null;
            connector.Focusable = true;
            DesignerView.SetCommandMenuMode(connector, CommandMenuMode.NoCommandMenu);
            if (connectorModelItem != null && connectorModelItem.ItemType == typeof(Transition))
            {
                SetConnectorLabel(connector, connectorModelItem);
                SetConnectorStartDotToolTip(connector.startDotGrid, connectorModelItem);
                connector.IsTransition = true;
                connector.ToolTip = SR.EditTransitionTooltip;
                connector.startDotGrid.MouseDown += new MouseButtonEventHandler(OnConnectorStartDotMouseDown);
                connector.startDotGrid.MouseUp += new MouseButtonEventHandler(OnConnectorStartDotMouseUp);
            }
            connector.GotKeyboardFocus += new KeyboardFocusChangedEventHandler(OnConnectorGotKeyboardFocus);
            connector.RequestBringIntoView += new RequestBringIntoViewEventHandler(OnConnectorRequestBringIntoView);
            connector.GotFocus += new RoutedEventHandler(OnConnectorGotFocus);
            connector.LostFocus += new RoutedEventHandler(OnConnectorLostFocus);
            connector.MouseDoubleClick += new MouseButtonEventHandler(OnConnectorMouseDoubleClick);
            connector.MouseDown += new MouseButtonEventHandler(OnConnectorMouseDown);
            connector.KeyDown += new KeyEventHandler(OnConnectorKeyDown);
            connector.ContextMenuOpening += new ContextMenuEventHandler(OnConnectorContextMenuOpening);
            SetConnectorSrcDestConnectionPoints(connector, srcConnPoint, destConnPoint);
            StateContainerEditor.SetConnectorModelItem(connector, connectorModelItem);
            connector.Unloaded += new RoutedEventHandler(OnConnectorUnloaded);
            connector.Points = points;
            return connector;
        }

        // Create a connector from the view state of the connector model item
        Connector CreateConnectorByConnectorModelItemViewState(UIElement source, UIElement dest, ModelItem connectorModelItem)
        {
            Debug.Assert(this.IsOutmostStateContainerEditor(), "Should only be called by the outmost editor.");
            Connector connector = null;
            object connectorLocation = this.ViewStateService.RetrieveViewState(connectorModelItem, ConnectorLocationViewStateKey);
            PointCollection locationPts = connectorLocation as PointCollection;
            if (locationPts != null)
            {
                ConnectionPoint srcConnPoint, destConnPoint;
                srcConnPoint = ConnectionPointHitTest(source, locationPts[0]);
                destConnPoint = ConnectionPointHitTest(dest, locationPts[locationPts.Count - 1]);
                if (srcConnPoint == null && destConnPoint == null)
                {
                    StateContainerEditor.GetEmptySrcDestConnectionPoints(source, dest, out srcConnPoint, out destConnPoint);
                }
                else if (srcConnPoint == null && destConnPoint != null)
                {
                    List<ConnectionPoint> srcConnectionPoints = GetEmptyConnectionPoints(source);
                    if (srcConnectionPoints.Count > 0)
                    {
                        srcConnPoint = StateContainerEditor.GetClosestConnectionPointNotOfType(destConnPoint, srcConnectionPoints, ConnectionPointType.Incoming);
                    }
                }
                else if (destConnPoint == null && srcConnPoint != null)
                {
                    List<ConnectionPoint> destConnectionPoints = GetEmptyConnectionPoints(dest);
                    if (destConnectionPoints.Count > 0)
                    {
                        destConnPoint = StateContainerEditor.GetClosestConnectionPointNotOfType(srcConnPoint, destConnectionPoints, ConnectionPointType.Outgoing);
                    }
                }
                if (srcConnPoint != null && destConnPoint != null)
                {
                    connector = this.CreateConnector(srcConnPoint, destConnPoint, locationPts, connectorModelItem);
                }
            }
            return connector;
        }

        // Create a connector by view state of the connector model item, and if failed, just create a connector using the connection points
        // of the source and destination designers. Then add the connector created to the free form panel.
        void AddConnector(UIElement sourceDesigner, UIElement destinationDesigner, ModelItem connectorModelItem)
        {
            Debug.Assert(this.IsOutmostStateContainerEditor(), "Should only be called by the outmost editor.");

            Connector connector = CreateConnectorByConnectorModelItemViewState(sourceDesigner, destinationDesigner, connectorModelItem);
            if (connector == null)
            {
                ConnectionPoint sourceConnectionPoint;
                ConnectionPoint destinationConnectionPoint;
                GetEmptySrcDestConnectionPoints(sourceDesigner, destinationDesigner, out sourceConnectionPoint, out destinationConnectionPoint);
                if (sourceConnectionPoint != null && destinationConnectionPoint != null)
                {
                    PointCollection connectorPoints = new PointCollection(ConnectorRouter.Route(this.panel, sourceConnectionPoint, destinationConnectionPoint));
                    this.ViewStateService.StoreViewState(connectorModelItem, ConnectorLocationViewStateKey, connectorPoints);
                    connector = CreateConnector(sourceConnectionPoint, destinationConnectionPoint, connectorPoints, connectorModelItem);
                }
            }
            if (connector != null)
            {
                this.panel.Children.Add(connector);
            }
        }

        void RemoveConnectorOnOutmostEditor(Connector connector)
        {
            ConnectionPoint srcConnectionPoint = FreeFormPanel.GetSourceConnectionPoint(connector);
            ConnectionPoint destConnectionPoint = FreeFormPanel.GetDestinationConnectionPoint(connector);
            // Update ConnectionPoints          
            srcConnectionPoint.AttachedConnectors.Remove(connector);
            destConnectionPoint.AttachedConnectors.Remove(connector);

            StateContainerEditor outmostContainer = this.GetOutmostStateContainerEditor();
            outmostContainer.panel.Children.Remove(connector);
            if (outmostContainer.selectedConnector == connector)
            {
                outmostContainer.ClearSelectedConnector();
            }
        }

        #endregion

        #region ConnectionPoint

        MultiBinding GetConnectionPointBinding(FrameworkElement element, double widthFraction, double heightFraction)
        {
            Debug.Assert(element != null, "FrameworkElement is null.");
            MultiBinding bindings = new MultiBinding();
            Binding sizeBinding = new Binding();
            sizeBinding.Source = element;
            sizeBinding.Path = new PropertyPath(FreeFormPanel.ChildSizeProperty);
            Binding locationBinding = new Binding();
            locationBinding.Source = element;
            locationBinding.Path = new PropertyPath(FreeFormPanel.LocationProperty);
            bindings.Bindings.Add(sizeBinding);
            bindings.Bindings.Add(locationBinding);
            bindings.Converter = new ConnectionPointConverter();
            bindings.ConverterParameter = new List<Object> { widthFraction, heightFraction, element.Margin };
            return bindings;
        }

        //widthFraction and heightFraction determine the location of the ConnectionPoint on the UIElement.
        ConnectionPoint CreateConnectionPoint(UIElement element, double widthFraction, double heightFraction, EdgeLocation location, ConnectionPointType type)
        {
            ConnectionPoint connectionPoint = new ConnectionPoint();
            connectionPoint.EdgeLocation = location;
            connectionPoint.PointType = type;
            connectionPoint.ParentDesigner = element;
            BindingOperations.SetBinding(connectionPoint, ConnectionPoint.LocationProperty, GetConnectionPointBinding(element as FrameworkElement, widthFraction, heightFraction));
            return connectionPoint;
        }

        void PopulateConnectionPoints(UIElement view)
        {
            view.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            List<ConnectionPoint> connectionPoints = new List<ConnectionPoint>();
            if (view is InitialNode)
            {
               
                connectionPoints.Add(CreateConnectionPoint(view, 0.5, 1.0, EdgeLocation.Bottom, ConnectionPointType.Outgoing));
                connectionPoints.Add(CreateConnectionPoint(view, 0, 0.5, EdgeLocation.Left, ConnectionPointType.Outgoing));
                connectionPoints.Add(CreateConnectionPoint(view, 1.0, 0.5, EdgeLocation.Right, ConnectionPointType.Outgoing));
            }
            else if (view is StateDesigner)
            {
                ConnectionPointType connectionPointType = ConnectionPointType.Default;
                double connectionPointNum = 3;
                double connectionPointRatio = 0.25;
                if (((StateDesigner)view).IsFinalState())
                {
                    connectionPointType = ConnectionPointType.Incoming;
                }

                
                for (int ii = 1; ii <= connectionPointNum; ii++)
                {
                    connectionPoints.Add(CreateConnectionPoint(view, 1, ii * connectionPointRatio, EdgeLocation.Right, connectionPointType));
                    connectionPoints.Add(CreateConnectionPoint(view, 0, ii * connectionPointRatio, EdgeLocation.Left, connectionPointType));
                }
              


                if (!((StateDesigner)view).IsFinalState())
                {
                    connectionPointNum = 5;  
               
                    connectionPointRatio = 0.167;
                }
                for (int ii = 1; ii <= connectionPointNum; ii++)
                {
                    connectionPoints.Add(CreateConnectionPoint(view, ii * connectionPointRatio, 0, EdgeLocation.Top, connectionPointType));
                    connectionPoints.Add(CreateConnectionPoint(view, ii * connectionPointRatio, 1, EdgeLocation.Bottom, connectionPointType));
                }
            }
            StateContainerEditor.SetConnectionPoints(view, connectionPoints);
        }

        List<ConnectionPoint> ConnectionPointsToShow(UIElement element)
        {
            bool isCreatingConnector = this.IsCreatingConnector();
            List<ConnectionPoint> connectionPointsToShow = new List<ConnectionPoint>();
            if (element is InitialNode)
            {
                // Don't allow moving the start of a transition to the initial node.
                if (isCreatingConnector || this.IsMovingStartOfConnectorForTransition())
                {
                    return connectionPointsToShow;
                }
                // Don't allow creating more than one connectors from the initial node.
                if ((StateContainerEditor.GetOutgoingConnectors(element).Count > 0) && !this.IsMovingStartOfConnectorFromInitialNode())
                {
                    return connectionPointsToShow;
                }
            }
            else if (element is StateDesigner)
            {
                StateDesigner designer = (StateDesigner)element;
                // Don't allow transitions to composite state
                if (isCreatingConnector && !designer.IsSimpleState())
                {
                    return connectionPointsToShow;
                }
                // Don't allow setting final state as the initial state
                if (designer.IsFinalState() && this.IsCreatingConnectorFromInitialNode())
                {
                    return connectionPointsToShow;
                }
                // Don't allow moving the start of the initial node connector to a state
                if (this.IsMovingStartOfConnectorFromInitialNode())
                {
                    return connectionPointsToShow;
                }
                // Don't allow creating transition from ancestor
                if (this.IsCreatingConnectorFromAncestorToDescendantStates(designer))
                {
                    return connectionPointsToShow;
                }
            }

            List<ConnectionPoint> connectionPoints = StateContainerEditor.GetConnectionPoints(element);
            if (isCreatingConnector)
            {
                connectionPointsToShow.AddRange(connectionPoints.Where<ConnectionPoint>(
                    (p) => { return p.PointType != ConnectionPointType.Outgoing && p.AttachedConnectors.Count == 0; }));
            }
            else
            {
                connectionPointsToShow.AddRange(connectionPoints.Where<ConnectionPoint>(
                    (p) => { return p.PointType != ConnectionPointType.Incoming && p.AttachedConnectors.Count == 0; }));
            }

            return connectionPointsToShow;
        }

        #endregion

        #region ChildElementEventHandlers

        void OnChildElementMouseEnter(object sender, MouseEventArgs e)
        {
            UIElement senderElement = sender as UIElement;
            if (senderElement != null && !this.IsReadOnly)
            {
                if (senderElement is StateDesigner)
                {
                    StateDesigner stateDesigner = StateContainerEditor.GetVisualAncestor<StateDesigner>(Mouse.DirectlyOver as DependencyObject);
                    // We don't want to show the connection points if the mouse is not directly over this state
                    if (stateDesigner != senderElement)
                    {
                        return;
                    }
                }

                AddConnectionPointsAdorner(senderElement);

                // Remove the adorner on the state designer when entering its child
                if (this.ModelItem.ItemType == typeof(State))
                {
                    StateDesigner parent = StateContainerEditor.GetVisualAncestor<StateDesigner>(this);
                    RemoveAdorner(parent, typeof(ConnectionPointsAdorner));
                }
            }
        }

        void AddConnectionPointsAdorner(UIElement element)
        {
            bool isSelected = false;
            if (element is WorkflowViewElement)
            {
                isSelected = (((Selection)this.Context.Items.GetValue<Selection>()).SelectedObjects as ICollection<ModelItem>).Contains(((WorkflowViewElement)element).ModelItem);
            }
            ConnectionPointsAdorner connectionPointsAdorner = new ConnectionPointsAdorner(element, ConnectionPointsToShow(element), isSelected);
            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(element);
            Debug.Assert(adornerLayer != null, "Cannot get AdornerLayer.");
            adornerLayer.Add(connectionPointsAdorner);
            // The outmostEditor should handle all the connection point related events for all its descendants
            StateContainerEditor outmostEditor = this.GetOutmostStateContainerEditor();
            connectionPointsAdorner.MouseDown += new MouseButtonEventHandler(outmostEditor.OnConnectionPointMouseDown);
            connectionPointsAdorner.MouseUp += new MouseButtonEventHandler(outmostEditor.OnConnectionPointMouseUp);
            connectionPointsAdorner.MouseLeave += new MouseEventHandler(outmostEditor.OnConnectionPointMouseLeave);
        }

        void OnChildElementMouseLeave(object sender, MouseEventArgs e)
        {
            bool removeConnectionPointsAdorner = true;
            if (Mouse.DirectlyOver != null)
            {
                removeConnectionPointsAdorner = !typeof(ConnectionPointsAdorner).IsAssignableFrom(Mouse.DirectlyOver.GetType());
            }
            if (removeConnectionPointsAdorner)
            {
                RemoveAdorner(sender as UIElement, typeof(ConnectionPointsAdorner));

                // Add connection points adorner to its containing state
                StateDesigner stateDesigner = StateContainerEditor.GetVisualAncestor<StateDesigner>(this);
                StateContainerEditor parentContainer = StateContainerEditor.GetVisualAncestor<StateContainerEditor>(stateDesigner);
                if (stateDesigner != null && parentContainer != null && !parentContainer.IsReadOnly)
                {
                    this.AddConnectionPointsAdorner(stateDesigner);
                }
            }
        }

        void OnChildElementSizeChanged(object sender, SizeChangedEventArgs e)
        {
            WorkflowViewElement view = sender as WorkflowViewElement;
            if (view != null)
            {
                // Such size changed events are a result of changes already committed in the UI. Hence we do not want to react to such view state changes.
                // Using internalViewStateChange flag for that purpose.
                this.internalViewStateChange = true;
                ModelItem storageModelItem = view.ModelItem;
                this.ViewStateService.StoreViewState(storageModelItem, ShapeSizeViewStateKey, ((UIElement)sender).DesiredSize);
                this.internalViewStateChange = false;
            }
        }

        #endregion

        #region ConnectorEventHandlers

        void OnConnectorStartDotMouseUp(object sender, MouseButtonEventArgs e)
        {
            this.activeSrcConnectionPoint = null;
            RemoveAdorner(this.panel, typeof(ConnectorCreationAdorner));
        }

        void OnConnectorStartDotMouseDown(object sender, MouseButtonEventArgs e)
        {
            DependencyObject startDot = (DependencyObject)sender;
            Connector connector = StateContainerEditor.GetVisualAncestor<Connector>(startDot);
            this.activeSrcConnectionPoint = FreeFormPanel.GetSourceConnectionPoint(connector);
            e.Handled = true;
        }

        void OnConnectorMouseDown(object sender, MouseButtonEventArgs e)
        {
            Connector connector = (Connector)sender;
            if (this.panel.Children.Contains(connector))
            {
                this.selectedConnector = connector;
            }
            // In order to not let WorkflowViewElement handle the event, which would cause the
            // ConnectorEditor to be removed.
            e.Handled = true;
        }

        void OnConnectorUnloaded(object sender, RoutedEventArgs e)
        {
            ModelItem primarySelection = this.Context.Items.GetValue<Selection>().PrimarySelection;
            if (object.Equals(primarySelection, StateContainerEditor.GetConnectorModelItem(sender as DependencyObject)))
            {
                if (primarySelection != null)
                {
                    Selection.Toggle(this.Context, primarySelection);
                }
            }
        }

        // Marking e.Handled = true to avoid scrolling in large workflows to bring the 
        // area of a connector in the center of the view region.
        void OnConnectorRequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
        {
            e.Handled = true;
        }

        // Marking e.Handled true for the case where a connector is clicked. 
        // This is to prevent WorkflowViewElement class from making StateMachine as the current selection.
        void OnConnectorGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            e.Handled = true;
        }

        void OnConnectorLostFocus(object sender, RoutedEventArgs e)
        {
            Connector connector = e.Source as Connector;

            if (this.panel.connectorEditor != null && connector.Equals(this.panel.connectorEditor.Connector))
            {
                this.panel.RemoveConnectorEditor();
            }
        }

        void OnConnectorGotFocus(object sender, RoutedEventArgs e)
        {
            Connector connector = e.Source as Connector;

            if (this.panel.connectorEditor == null || !connector.Equals(this.panel.connectorEditor.Connector))
            {
                this.panel.RemoveConnectorEditor();
                this.panel.connectorEditor = new ConnectorEditor(this.panel, connector);
            }

            if (this.panel.Children.Contains(connector))
            {
                ModelItem connectorModelItem = StateContainerEditor.GetConnectorModelItem(connector);
                Selection newSelection = new Selection();
                if (connectorModelItem != null && connectorModelItem.ItemType == typeof(Transition))
                {
                    newSelection = new Selection(connectorModelItem);
                }
                this.Context.Items.SetValue(newSelection);
                this.selectedConnector = connector;
                e.Handled = true;
            }
        }

        void OnConnectorKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                this.DesignerView.MakeRootDesigner(StateContainerEditor.GetConnectorModelItem(sender as DependencyObject));
                e.Handled = true;
            }
        }

        void OnConnectorMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ModelItem connectorModelItem = StateContainerEditor.GetConnectorModelItem(sender as DependencyObject);
            if (connectorModelItem != null && connectorModelItem.ItemType == typeof(Transition))
            {
                this.DesignerView.MakeRootDesigner(connectorModelItem);
                e.Handled = true;
            }
        }

        void OnConnectorContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            // Disable context menu
            e.Handled = true;
        }

        #endregion

        #region ConnectionPointEventHandlers

        void OnConnectionPointMouseDown(object sender, MouseButtonEventArgs e)
        {
            UIElement srcElement = ((Adorner)sender).AdornedElement as UIElement;
            this.activeSrcConnectionPoint = ConnectionPointHitTest(srcElement, e.GetPosition(this.panel));
            e.Handled = true;
        }

        void OnConnectionPointMouseLeave(object sender, MouseEventArgs e)
        {
            UIElement adornedElement = ((Adorner)sender).AdornedElement as UIElement;
            RemoveAdorner(adornedElement, typeof(ConnectionPointsAdorner));
        }

        void OnConnectionPointMouseUp(object sender, MouseButtonEventArgs e)
        {
            UIElement dest = ((Adorner)sender).AdornedElement as UIElement;
            if (this.activeSrcConnectionPoint != null)
            {
                ConnectionPoint destConnectionPoint = ConnectionPointHitTest(dest, e.GetPosition(this.panel));
                if (destConnectionPoint != null && !this.activeSrcConnectionPoint.Equals(destConnectionPoint))
                {
                    ConnectorCreationResult result = CreateConnectorGesture(this.activeSrcConnectionPoint, destConnectionPoint, null, false);
                    if (result != ConnectorCreationResult.Success)
                    {
                        StateContainerEditor.ReportConnectorCreationError(result);
                    }
                }
                this.activeSrcConnectionPoint = null;
                RemoveAdorner(this.panel, typeof(ConnectorCreationAdorner));
            }
            else
            {
                //This will cause the FreeFormPanel to handle the event and is useful while moving a connector end point.
                this.lastConnectionPointMouseUpElement = dest;
                dest.RaiseEvent(e);
            }
            RemoveAdorner(dest, typeof(ConnectionPointsAdorner));
        }

        #endregion

        #region FreeFormPanelEventHandlers

        void OnFreeFormPanelLocationChanged(object sender, LocationChangedEventArgs e)
        {
            Debug.Assert(sender is UIElement, "Sender should be of type UIElement");
            Connector movedConnector = sender as Connector;
            if (movedConnector != null)
            {
                //ViewState is undoable only when a user gesture moves a connector. If the FreeFormPanel routes a connector,
                //the change is not undoable.
                bool isUndoableViewState = false;
                ModelItem connectorModelItem = StateContainerEditor.GetConnectorModelItem(movedConnector);
                PointCollection existingViewState = this.ViewStateService.RetrieveViewState(connectorModelItem, ConnectorLocationViewStateKey) as PointCollection;
                if (existingViewState != null && existingViewState.Count > 0 && movedConnector.Points.Count > 0
                    && existingViewState[0].Equals(movedConnector.Points[0]) && existingViewState[existingViewState.Count - 1].Equals(movedConnector.Points[movedConnector.Points.Count - 1]))
                {
                    isUndoableViewState = true;
                }
                StoreConnectorLocationViewState(movedConnector, isUndoableViewState);
            }
            else
            {
                //This is called only when a shape without ViewState is auto-layout'd by the FreeFormPanel.
                WorkflowViewElement view = sender as WorkflowViewElement;
                if (view != null)
                {
                    StoreShapeLocationViewState(view, e.NewLocation);
                }
            }
        }

        void UpdateStateMachineOnConnectorMoved(ConnectionPoint knownConnectionPoint, Point newPoint, Connector movedConnector, bool isConnectorStartMoved)
        {
            HitTestResult hitTestResult = VisualTreeHelper.HitTest(this.panel, newPoint);
            if (hitTestResult == null)
            {
                return;
            }

            UIElement newViewElement = null;
            ConnectionPoint newConnectionPoint = null;

            //The case where the Connector is dropped on a ConnectionPoint.
            if (this.lastConnectionPointMouseUpElement != null)
            {
                newConnectionPoint = StateContainerEditor.ConnectionPointHitTest(this.lastConnectionPointMouseUpElement, newPoint);
                if (newConnectionPoint != null)
                {
                    newViewElement = this.lastConnectionPointMouseUpElement;
                }
                this.lastConnectionPointMouseUpElement = null;
            }

            //The case where the link is dropped on a shape.
            if (newViewElement == null)
            {
                newViewElement = StateContainerEditor.GetVisualAncestor<WorkflowViewElement>(hitTestResult.VisualHit);
            }

            if (newViewElement != null)
            {
                if (this.panel.IsAncestorOf(newViewElement))
                {
                    using (EditingScope es = (EditingScope)this.ModelItem.BeginEdit(SR.MoveLink))
                    {
                        // Remove the old connector ModelItem
                        this.DeleteConnectorModelItem(movedConnector);
                        // Create new connector
                        ConnectorCreationResult result = ConnectorCreationResult.OtherFailure;
                        if (!isConnectorStartMoved)
                        {
                            if (newConnectionPoint == null)
                            {
                                result = CreateConnectorGesture(knownConnectionPoint, newViewElement, movedConnector, false);
                            }
                            else
                            {
                                result = CreateConnectorGesture(knownConnectionPoint, newConnectionPoint, movedConnector, false);
                            }
                        }
                        else
                        {
                            // Don't allow moving the start of the initial node connector to a state
                            if (!(newViewElement is StateDesigner && StateContainerEditor.IsConnectorFromInitialNode(movedConnector)))
                            {
                                if (newConnectionPoint == null)
                                {
                                    result = CreateConnectorGesture(newViewElement, knownConnectionPoint, movedConnector, true);
                                }
                                else
                                {
                                    result = CreateConnectorGesture(newConnectionPoint, knownConnectionPoint, movedConnector, true);
                                }
                            }
                        }

                        if (result == ConnectorCreationResult.Success)
                        {
                            es.Complete();
                        }
                        else
                        {
                            StateContainerEditor.ReportConnectorCreationError(result);
                            es.Revert();
                        }
                    }
                }
            }
        }

        void OnFreeFormPanelConnectorMoved(object sender, ConnectorMovedEventArgs e)
        {
            Connector movedConnector = sender as Connector;
            if (movedConnector != null)
            {
                Debug.Assert(e.NewConnectorLocation.Count > 0, "Invalid connector editor");
                if (e.NewConnectorLocation[0].Equals(movedConnector.Points[0]))
                {
                    // destination moved
                    ConnectionPoint srcConnPoint = FreeFormPanel.GetSourceConnectionPoint(movedConnector);
                    Point destPoint = e.NewConnectorLocation[e.NewConnectorLocation.Count - 1];
                    UpdateStateMachineOnConnectorMoved(srcConnPoint, destPoint, movedConnector, false);
                }
                else
                {
                    // source moved
                    ConnectionPoint destConnPoint = FreeFormPanel.GetDestinationConnectionPoint(movedConnector);
                    UpdateStateMachineOnConnectorMoved(destConnPoint, e.NewConnectorLocation[0], movedConnector, true);
                }
            }
        }

        // This is to keep this.selectedConnector up to date.
        // Cases included: 1. create a connector, select it and undo, 2. move a connector from one shape to another.
        void OnFreeFormPanelLayoutUpdated(object sender, EventArgs e)
        {
            if (this.selectedConnector != null && !this.panel.Children.Contains(this.selectedConnector))
            {
                this.ClearSelectedConnector();
            }
        }

        void OnFreeFormPanelRequiredSizeChanged(object sender, RequiredSizeChangedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (e.NewRequiredSize.Width > this.StateContainerWidth)
                {
                    this.ViewStateService.StoreViewState(this.ModelItem, StateContainerEditor.StateContainerWidthViewStateKey, e.NewRequiredSize.Width);
                }
                if (e.NewRequiredSize.Height > this.StateContainerHeight)
                {
                    this.ViewStateService.StoreViewState(this.ModelItem, StateContainerEditor.StateContainerHeightViewStateKey, e.NewRequiredSize.Height);
                }
            }));
        }

        #endregion

        #region StateContainerGridEventHandlers

        void OnStateContainerGridMouseLeave(object sender, MouseEventArgs e)
        {
            if (!this.IsOutmostStateContainerEditor())
            {
                return;
            }
            bool endLinkCreation = !IsVisualHit(sender as UIElement, sender as UIElement, e.GetPosition(sender as IInputElement));
            if (endLinkCreation)
            {
                RemoveAdorner(this.panel, typeof(ConnectorCreationAdorner));
                this.activeSrcConnectionPoint = null;
            }
        }

        void OnStateContainerGridMouseMove(object sender, MouseEventArgs e)
        {
            if (!this.IsOutmostStateContainerEditor())
            {
                return;
            }
            if (this.activeSrcConnectionPoint != null)
            {
                Point[] points = ConnectorRouter.Route(this.panel, this.activeSrcConnectionPoint, e.GetPosition(this.panel));
                List<Point> segments = new List<Point>(points);
                // Remove the previous adorner.
                RemoveAdorner(this.panel, typeof(ConnectorCreationAdorner));
                // Add new adorner.
                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this.panel);
                Debug.Assert(adornerLayer != null, "Adorner Layer does not exist");
                ConnectorCreationAdorner newAdorner = new ConnectorCreationAdorner(this.panel, segments);
                adornerLayer.Add(newAdorner);
                e.Handled = true;
            }
        }

        void OnStateContainerGridPreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (!this.IsOutmostStateContainerEditor())
            {
                return;
            }
            // Creating connector
            if (this.activeSrcConnectionPoint != null)
            {
                AutoScrollHelper.AutoScroll(e, this);
            }
            // Reconnecting connector
            else if (this.panel.connectorEditor != null && (this.panel.connectorEditor.IsConnectorEndBeingMoved || this.panel.connectorEditor.IsConnectorStartBeingMoved))
            {
                AutoScrollHelper.AutoScroll(e, this);
            }
        }

        void OnStateContainerGridPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!this.IsOutmostStateContainerEditor())
            {
                return;
            }
            UIElement destElement = StateContainerEditor.GetVisualAncestor<WorkflowViewElement>(e.OriginalSource as DependencyObject);
            if (destElement != null && destElement is StateDesigner)
            {
                if (this.activeSrcConnectionPoint != null)
                {
                    ConnectorCreationResult result = CreateConnectorGesture(this.activeSrcConnectionPoint, destElement, null, false);
                    if (result != ConnectorCreationResult.Success)
                    {
                        StateContainerEditor.ReportConnectorCreationError(result);
                    }
                }
                RemoveAdorner(destElement, typeof(ConnectionPointsAdorner));
            }
            if (this.activeSrcConnectionPoint != null)
            {
                this.activeSrcConnectionPoint = null;
                RemoveAdorner(this.panel, typeof(ConnectorCreationAdorner));
            }
        }

        void OffsetConnectorViewState(ModelItem stateModelItem, Point oldLocation, Point newLocation)
        {
            this.OffsetLocationViewStates(
                new Vector(newLocation.X - oldLocation.X, newLocation.Y - oldLocation.Y), null,
                GetTransitionModelItems(new List<ModelItem> { stateModelItem }), true);
        }

        void OnStateContainerGridDrop(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.None;
            Object droppedObject = DragDropHelper.GetDroppedObject(this, e, Context);
            // Marking the event as being handled. In whichever case we want to route the event, it will be unmarked explicitly.
            e.Handled = true;
            if (droppedObject != null)
            {
                Point anchorPoint = DragDropHelper.GetDragDropAnchorPoint(e);
                ModelItem droppedModelItem = droppedObject as ModelItem;
                StateContainerEditor srcContainer = DragDropHelper.GetCompositeView(e) as StateContainerEditor;

                if (droppedModelItem != null && srcContainer != null && srcContainer.Equals(this))
                {
                    // Internal move
                    PerformInternalMove(this.modelItemToUIElement[droppedModelItem] as WorkflowViewElement, e.GetPosition(this.panel), anchorPoint);
                }
                else
                {
                    // External model Item drop
                    if (droppedModelItem != null)
                    {
                        this.ModelItem.Properties[ChildStatesPropertyName].Collection.Add(droppedModelItem);
                    }
                    // Toolbox drop.
                    else
                    {
                        if (droppedObject.GetType() == typeof(State))
                        {
                            ((State)droppedObject).DisplayName = SR.DefaultStateDisplayName;
                            droppedModelItem = this.ModelItem.Properties[ChildStatesPropertyName].Collection.Add(droppedObject);
                        }
                        else if (droppedObject.GetType() == typeof(FinalState))
                        {
                            droppedObject = new State() { DisplayName = SR.DefaultFinalStateDisplayName, IsFinal = true };
                            droppedModelItem = this.ModelItem.Properties[ChildStatesPropertyName].Collection.Add(droppedObject);
                        }
                    }
                    if (droppedModelItem != null)
                    {
                        WorkflowViewElement view = droppedModelItem.View as WorkflowViewElement;
                        Debug.Assert(view != null, "Designer for dropped ModelItem should already have been created.");
                        // If drag anchor point is beyond the size of the shape being dropped, 
                        if (anchorPoint.X > view.DesiredSize.Width || anchorPoint.Y > view.DesiredSize.Height)
                        {
                            anchorPoint = new Point(-1, -1);
                        }
                        Point shapeLocation = StateContainerEditor.SnapVisualToGrid(view, e.GetPosition(this.panel), anchorPoint);
                        object viewState = this.ViewStateService.RetrieveViewState(droppedModelItem, ShapeLocationViewStateKey);
                        if (viewState != null)
                        {
                            Point oldLocation = (Point)viewState;
                            oldLocation = srcContainer.panel.GetLocationRelativeToOutmostPanel(oldLocation);
                            Point newLocation = this.panel.GetLocationRelativeToOutmostPanel(shapeLocation);
                            // To make sure the connectors are still connected to the connection points
                            OffsetConnectorViewState(droppedModelItem, oldLocation, newLocation);
                        }
                        this.StoreShapeLocationViewState(droppedModelItem, shapeLocation);
                        DragDropHelper.SetDragDropCompletedEffects(e, DragDropEffects.Move);
                        this.Dispatcher.BeginInvoke(
                            new Action(() =>
                            {
                                Selection.SelectOnly(this.Context, droppedModelItem);
                                Keyboard.Focus((IInputElement)droppedModelItem.View);
                            }),
                            DispatcherPriority.ApplicationIdle);
                    }
                }
            }
        }

        void OnStateContainerGridDragEnter(object sender, DragEventArgs e)
        {
            OnStateContainerGridDrag(sender, e);
        }

        void OnStateContainerGridDragOver(object sender, DragEventArgs e)
        {
            OnStateContainerGridDrag(sender, e);
        }

        void OnStateContainerGridDrag(object sender, DragEventArgs e)
        {
            if (!e.Handled)
            {
                // Don't allow dropping on a connector. Otherwise the state will be dropped into the root state machine,
                // since all connectors belong to the root state container designer.
                if (GetVisualAncestor<Connector>(VisualTreeHelper.HitTest(this, e.GetPosition(this)).VisualHit) != null)
                {
                    e.Effects = DragDropEffects.None;
                    e.Handled = true;
                    return;
                }
                ModelItem modelItem = e.Data.GetData(DragDropHelper.ModelItemDataFormat) as ModelItem;
                if (modelItem != null && !StateContainerEditor.AreInSameStateMachine(modelItem, this.ModelItem))
                {
                    // Don't allow dragging a state into a different state machine.
                    e.Effects = DragDropEffects.None;
                }
                else if (modelItem != null && modelItem.ItemType == typeof(State) && this.ModelItem.ItemType == typeof(State) &&
                    (bool)modelItem.Properties[StateDesigner.IsFinalPropertyName].Value.GetCurrentValue())
                {
                    // Don't allow drag a final state into a state
                    e.Effects = DragDropEffects.None;
                }
                else if ((this.ModelItem.ItemType == typeof(State) && DragDropHelper.AllowDrop(e.Data, this.Context, typeof(State))) ||
                    (this.ModelItem.ItemType == typeof(StateMachine) && DragDropHelper.AllowDrop(e.Data, this.Context, typeof(State), typeof(FinalState))))
                {
                    e.Effects |= DragDropEffects.Move;
                }
                else
                {
                    e.Effects = DragDropEffects.None;
                }
                e.Handled = true;
            }
        }

        void KeyboardMove(Key key)
        {
            WorkflowViewElement shapeToMove = (WorkflowViewElement)this.modelItemToUIElement[this.Context.Items.GetValue<Selection>().PrimarySelection];
            Point currentLocation = FreeFormPanel.GetLocation(shapeToMove);
            switch (key)
            {
                case Key.Down:
                    PerformInternalMove(shapeToMove, new Point(currentLocation.X, currentLocation.Y + StateContainerEditor.GridSize), new Point(0, 0));
                    break;
                case Key.Up:
                    PerformInternalMove(shapeToMove, new Point(currentLocation.X, currentLocation.Y - StateContainerEditor.GridSize), new Point(0, 0));
                    break;
                case Key.Right:
                    PerformInternalMove(shapeToMove, new Point(currentLocation.X + StateContainerEditor.GridSize, currentLocation.Y), new Point(0, 0));
                    break;
                case Key.Left:
                    PerformInternalMove(shapeToMove, new Point(currentLocation.X - StateContainerEditor.GridSize, currentLocation.Y), new Point(0, 0));
                    break;
                default:
                    Debug.Assert(false, "Invalid case");
                    break;
            }
        }

        void OnStateContainerGridKeyDown(object sender, KeyEventArgs e)
        {
            Selection currentSelection = this.Context.Items.GetValue<Selection>();
            if (e.Key == Key.Delete && this.selectedConnector != null)
            {
                ModelItem primarySelection = currentSelection.PrimarySelection;
                //Delete connector
                ModelItem connectorModelItem = StateContainerEditor.GetConnectorModelItem(this.selectedConnector);
                if (object.Equals(primarySelection, connectorModelItem) ||
                    // Delete initial link
                    primarySelection == null && connectorModelItem != null && connectorModelItem.ItemType != typeof(Transition))
                {
                    this.DeleteConnectorModelItem(this.selectedConnector);
                    e.Handled = true;
                }
            }
            else if ((new List<Key> { Key.Left, Key.Right, Key.Up, Key.Down }).Contains(e.Key)
                && currentSelection.SelectionCount == 1
                && this.modelItemToUIElement.ContainsKey(currentSelection.PrimarySelection))
            {
                this.KeyboardMove(e.Key);
                e.Handled = true;
            }
        }

        void OnStateContainerGridPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.selectedConnector = null;
        }

        #endregion

        #region Misc

        bool IsOutmostStateContainerEditor()
        {
            return this == this.GetOutmostStateContainerEditor();
        }

        internal StateContainerEditor GetOutmostStateContainerEditor()
        {
            StateContainerEditor ret = this;
            StateContainerEditor parent = GetVisualAncestor<StateContainerEditor>(this);
            while (parent != null)
            {
                ret = parent;
                parent = GetVisualAncestor<StateContainerEditor>(parent);
            }
            return ret;
        }

        Connector GetConnectorOnOutmostEditor(ModelItem connectorModelItem)
        {
            StateContainerEditor outmostEditor = this.GetOutmostStateContainerEditor();
            foreach (UIElement element in outmostEditor.panel.Children)
            {
                Connector connector = element as Connector;
                if (connector != null)
                {
                    if (StateContainerEditor.GetConnectorModelItem(connector) == connectorModelItem)
                    {
                        return connector;
                    }
                }
            }
            return null;
        }

        bool IsCreatingConnector()
        {
            StateContainerEditor outmostEditor = this.GetOutmostStateContainerEditor();
            return (outmostEditor.activeSrcConnectionPoint != null || (outmostEditor.panel.connectorEditor != null && outmostEditor.panel.connectorEditor.IsConnectorEndBeingMoved));
        }

        bool IsCreatingConnectorFromInitialNode()
        {
            StateContainerEditor outmostEditor = this.GetOutmostStateContainerEditor();
            return (outmostEditor.activeSrcConnectionPoint != null && outmostEditor.activeSrcConnectionPoint.ParentDesigner is InitialNode) ||
                (outmostEditor.panel.connectorEditor != null && outmostEditor.panel.connectorEditor.IsConnectorEndBeingMoved &&
                outmostEditor.panel.connectorEditor.Connector != null &&
                IsConnectorFromInitialNode(outmostEditor.panel.connectorEditor.Connector));
        }

        bool IsMovingStartOfConnectorFromInitialNode()
        {
            StateContainerEditor outmostEditor = this.GetOutmostStateContainerEditor();
            return (outmostEditor.panel.connectorEditor != null && outmostEditor.panel.connectorEditor.IsConnectorStartBeingMoved &&
                outmostEditor.panel.connectorEditor.Connector != null &&
                IsConnectorFromInitialNode(outmostEditor.panel.connectorEditor.Connector));
        }

        bool IsMovingStartOfConnectorForTransition()
        {
            StateContainerEditor outmostEditor = this.GetOutmostStateContainerEditor();
            return (outmostEditor.panel.connectorEditor != null && outmostEditor.panel.connectorEditor.IsConnectorStartBeingMoved &&
                outmostEditor.panel.connectorEditor.Connector != null &&
                GetConnectorModelItem(outmostEditor.panel.connectorEditor.Connector).ItemType == typeof(Transition)); ;
        }

        bool IsCreatingConnectorFromAncestorToDescendantStates(StateDesigner designer)
        {
            StateContainerEditor outmostEditor = this.GetOutmostStateContainerEditor();
            // Creating a new connector
            if (outmostEditor.activeSrcConnectionPoint != null)
            {
                StateDesigner sourceDesigner = outmostEditor.activeSrcConnectionPoint.ParentDesigner as StateDesigner;
                if (sourceDesigner != null)
                {
                    if (StateContainerEditor.IsDescendantStateOf(designer.ModelItem, sourceDesigner.ModelItem))
                    {
                        return true;
                    }
                }
            }
            // Moving a connector
            else if (outmostEditor.panel.connectorEditor != null)
            {
                if (outmostEditor.panel.connectorEditor.IsConnectorEndBeingMoved)
                {
                    if (StateContainerEditor.IsDescendantStateOf(designer.ModelItem,
                        StateContainerEditor.GetParentStateModelItemForTransition(StateContainerEditor.GetConnectorModelItem(outmostEditor.panel.connectorEditor.Connector))))
                    {
                        return true;
                    }
                }
                else if (outmostEditor.panel.connectorEditor.IsConnectorStartBeingMoved)
                {
                    if (StateContainerEditor.IsDescendantStateOf(StateContainerEditor.GetConnectorModelItem(outmostEditor.panel.connectorEditor.Connector).Properties[TransitionDesigner.ToPropertyName].Value, designer.ModelItem))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        void InvalidateMeasureForOutmostPanel()
        {
            this.GetOutmostStateContainerEditor().panel.InvalidateMeasure();
        }

        void PerformInternalMove(WorkflowViewElement movedElement, Point newPoint, Point shapeAnchorPoint)
        {
            using (EditingScope es = (EditingScope)this.ModelItem.BeginEdit(SR.ItemMove))
            {
                RemoveAdorner(movedElement, typeof(ConnectionPointsAdorner));
                Point newLocation = SnapVisualToGrid(movedElement, newPoint, shapeAnchorPoint);
                object viewState = this.ViewStateService.RetrieveViewState(movedElement.ModelItem, ShapeLocationViewStateKey);
                if (viewState != null)
                {
                    Point oldLocation = (Point)viewState;
                    // To make sure the connectors are still connected to the connection points
                    this.OffsetConnectorViewState(movedElement.ModelItem, oldLocation, newLocation);
                }
                this.StoreShapeLocationViewState(movedElement, newLocation);
                // To make sure the connector changes are undoable
                this.StoreAttachedConnectorViewStates(movedElement);
                es.Complete();
            }
        }

        public void StoreAttachedConnectorViewStates(UIElement element)
        {
            foreach (Connector connector in GetAttachedConnectors(element))
            {
                StoreConnectorLocationViewState(connector, true);
            }
        }

        bool ShouldInitialize()
        {
            WorkflowViewElement parent = StateContainerEditor.GetVisualAncestor<WorkflowViewElement>(this);
            return parent != null && parent.ModelItem != null && (parent.ModelItem.ItemType == typeof(StateMachine) && parent.ShowExpanded ||
                   parent.ModelItem.ItemType == typeof(State) && !parent.IsRootDesigner);
        }

        void ClearSelectedConnector()
        {
            if (this.panel.connectorEditor != null && this.panel.connectorEditor.Connector == this.selectedConnector)
            {
                this.panel.RemoveConnectorEditor();
            }
            this.selectedConnector = null;
        }

        #endregion

        internal enum ConnectorCreationResult
        {
            Success,
            CannotCreateTransitionToCompositeState,
            CannotCreateTransitionFromAncestorToDescendant,
            CannotSetCompositeStateAsInitialState,
            CannotSetFinalStateAsInitialState,
            OtherFailure
        }
    }
}

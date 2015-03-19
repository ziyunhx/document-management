//-----------------------------------------------------------------------------

//-----------------------------------------------------------------------------

namespace Machine.Design
{
    using System;
    using System.Activities;
    using System.Activities.Presentation.Model;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Media;
    using Machine.Design.FreeFormEditing;
    

   public partial class StateContainerEditor
    {
        internal static ModelItem GetConnectorModelItem(DependencyObject obj)
        {
            return (ModelItem)obj.GetValue(StateContainerEditor.ConnectorModelItemProperty);
        }

        static void SetConnectorModelItem(DependencyObject obj, ModelItem modelItem)
        {
            obj.SetValue(StateContainerEditor.ConnectorModelItemProperty, modelItem);
        }

        internal static List<ConnectionPoint> GetConnectionPoints(DependencyObject obj)
        {
            return (List<ConnectionPoint>)obj.GetValue(StateContainerEditor.ConnectionPointsProperty);
        }

        static void SetConnectionPoints(DependencyObject obj, List<ConnectionPoint> connectionPoints)
        {
            obj.SetValue(StateContainerEditor.ConnectionPointsProperty, connectionPoints);
        }

        static void SetConnectorSrcDestConnectionPoints(Connector connector, ConnectionPoint srcConnectionPoint, ConnectionPoint destConnectionPoint)
        {
            FreeFormPanel.SetSourceConnectionPoint(connector, srcConnectionPoint);
            FreeFormPanel.SetDestinationConnectionPoint(connector, destConnectionPoint);
            srcConnectionPoint.AttachedConnectors.Add(connector);
            destConnectionPoint.AttachedConnectors.Add(connector);
        }

        static void SetConnectorLabel(Connector connector, ModelItem connectorModelItem)
        {
            connector.SetBinding(Connector.LabelTextProperty, new Binding() { 
                Source = connectorModelItem, 
                Path = new PropertyPath("DisplayName") });
        }

        static void SetConnectorStartDotToolTip(FrameworkElement startDot, ModelItem connectorModelItem)
        {           
            ModelItem triggerModelItem = connectorModelItem.Properties[TransitionDesigner.TriggerPropertyName].Value as ModelItem;
            string triggerName = null;
            if (triggerModelItem == null)
            {
                triggerName = "(null)";
            }
            else
            {
                ModelItem displayNameModelItem = triggerModelItem.Properties["DisplayName"].Value;
                if(displayNameModelItem != null)
                {
                    triggerName = displayNameModelItem.GetCurrentValue() as string;
                }
            }
            startDot.ToolTip = SR.TriggerNameToolTip + triggerName + Environment.NewLine + SR.SharedTriggerToolTip;
        }
        
        // Returns true if visual is on the visual tree for point p relative to the reference.
        static bool IsVisualHit(UIElement visual, UIElement reference, Point point)
        {
            bool visualIsHit = false;
            HitTestResult result = VisualTreeHelper.HitTest(reference, point);
            if (result != null)
            {
                DependencyObject obj = result.VisualHit;
                while (obj != null)
                {
                    if (visual.Equals(obj))
                    {
                        visualIsHit = true;
                        break;
                    }
                    obj = VisualTreeHelper.GetParent(obj);
                }
            }
            return visualIsHit;
        }
        
        //This snaps the center of the element to grid.
        //Wherever shapeAnchorPoint is valid, it is made co-incident with the drop location.
        static Point SnapVisualToGrid(UIElement element, Point location, Point shapeAnchorPoint)
        {
            Debug.Assert(element != null, "Input UIElement is null");
            element.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            Point oldCenter = location;
            if (shapeAnchorPoint.X < 0 && shapeAnchorPoint.Y < 0)
            {
                //shapeAnchorPoint is set to (-1, -1) in case where it does not make sense (eg. toolbox drop).
                location.X -= InitialFinalWidth/2;
                location.Y -= InitialFinalHeight/2;
            }
            else
            {
                location.X -= shapeAnchorPoint.X;
                location.Y -= shapeAnchorPoint.Y;
                oldCenter = new Point(location.X + element.DesiredSize.Width / 2, location.Y + element.DesiredSize.Height / 2);
            }

            Point newCenter = SnapPointToGrid(oldCenter);

            location.Offset(newCenter.X - oldCenter.X, newCenter.Y - oldCenter.Y);

            if (location.X < 0)
            {
                double correction = StateContainerEditor.GridSize - ((location.X * (-1)) % StateContainerEditor.GridSize);
                location.X = (correction == StateContainerEditor.GridSize) ? 0 : correction;
            }
            if (location.Y < 0)
            {
                double correction = StateContainerEditor.GridSize - ((location.Y * (-1)) % StateContainerEditor.GridSize);
                location.Y = (correction == StateContainerEditor.GridSize) ? 0 : correction;
            }
            return location;
        }

        static Point SnapPointToGrid(Point pt)
        {
            pt.X -= pt.X % StateContainerEditor.GridSize;
            pt.Y -= pt.Y % StateContainerEditor.GridSize;
            pt.X = pt.X < 0 ? 0 : pt.X;
            pt.Y = pt.Y < 0 ? 0 : pt.Y;
            return pt;
        }

        static void RemoveAdorner(UIElement adornedElement, Type adornerType)
        {
            Debug.Assert(adornedElement != null, "Invalid argument");
            Debug.Assert(typeof(Adorner).IsAssignableFrom(adornerType), "Invalid argument");
            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(adornedElement);
            if (adornerLayer != null)
            {
                Adorner[] adorners = adornerLayer.GetAdorners(adornedElement);
                if (adorners != null)
                {
                    foreach (Adorner adorner in adorners)
                    {
                        if (adornerType.IsAssignableFrom(adorner.GetType()))
                        {
                            adornerLayer.Remove(adorner);
                        }
                    }
                }
            }
        }

        internal static List<Connector> GetAttachedConnectors(UIElement shape)
        {
            HashSet<Connector> attachedConnectors = new HashSet<Connector>();
            List<ConnectionPoint> allConnectionPoints = GetConnectionPoints(shape);
            if (allConnectionPoints != null)
            {
                foreach (ConnectionPoint connPoint in allConnectionPoints)
                {
                    if (connPoint != null)
                    {
                        foreach (Connector connector in connPoint.AttachedConnectors)
                        {
                            attachedConnectors.Add(connector);
                        }
                    }
                }
            }
            return attachedConnectors.ToList<Connector>();
        }

        static List<Connector> GetOutgoingConnectors(UIElement shape)
        {
            List<Connector> outgoingConnectors = new List<Connector>();
            List<ConnectionPoint> allConnectionPoints = GetConnectionPoints(shape);
            foreach (ConnectionPoint connPoint in allConnectionPoints)
            {
                if (connPoint != null)
                {
                    outgoingConnectors.AddRange(connPoint.AttachedConnectors.Where(p => FreeFormPanel.GetSourceConnectionPoint(p).Equals(connPoint)));
                }
            }
            return outgoingConnectors;
        }

        static List<Connector> GetIncomingConnectors(UIElement shape)
        {
            List<Connector> incomingConnectors = new List<Connector>();
            List<ConnectionPoint> allConnectionPoints = GetConnectionPoints(shape);
            foreach (ConnectionPoint connPoint in allConnectionPoints)
            {
                if (connPoint != null)
                {
                    incomingConnectors.AddRange(connPoint.AttachedConnectors.Where(p => FreeFormPanel.GetDestinationConnectionPoint(p).Equals(connPoint)));
                }
            }
            return incomingConnectors;
        }

        static ConnectionPoint ConnectionPointHitTest(UIElement element, Point hitPoint)
        {
            ConnectionPoint hitConnectionPoint = null;
            List<ConnectionPoint> connectionPoints = StateContainerEditor.GetConnectionPoints(element);
            foreach (ConnectionPoint connPoint in connectionPoints)
            {
                if (connPoint != null)
                {
                    // We need to transform the connection point location to be relative to the outmost panel
                    FreeFormPanel panel = GetVisualAncestor<FreeFormPanel>(element);
                    if (new Rect(panel.GetLocationRelativeToOutmostPanel(connPoint.Location) + connPoint.HitTestOffset, connPoint.HitTestSize).Contains(hitPoint))
                    {
                        hitConnectionPoint = connPoint;
                        break;
                    }
                }
            }
            return hitConnectionPoint;
        }

        internal static T GetVisualAncestor<T>(DependencyObject child) where T : DependencyObject
        {
            if (child == null)
            {
                return null;
            }
            do
            {
                child = VisualTreeHelper.GetParent(child);
            }
            while (child != null && !typeof(T).IsAssignableFrom(child.GetType()));

            return child as T;
        }

        static internal ModelItem GetStateContainerModelItem(ModelItem modelItem)
        {
            ModelItem model = modelItem.Parent;
            while (model != null && model.ItemType != typeof(StateMachine) && model.ItemType != typeof(State))
            {
                if (typeof(Activity).IsAssignableFrom(modelItem.ItemType))
                {
                    return null;
                }
                model = model.Parent;
            }
            return model;
        }

        internal static ModelItem GetStateMachineModelItem(ModelItem modelItem)
        {
            ModelItem currentModelItem = modelItem;
            while(currentModelItem != null && currentModelItem.ItemType != typeof(StateMachine))
            {
                currentModelItem = currentModelItem.Parent;
            }
            return currentModelItem;
        }

        static bool AreInSameStateMachine(ModelItem modelItem1, ModelItem modelItem2)
        {
            return GetStateMachineModelItem(modelItem1) == GetStateMachineModelItem(modelItem2);
        }

        internal static ModelItem GetParentStateModelItemForTransition(ModelItem transitionModelItem)
        {
            ModelItem parent = transitionModelItem;
            while (parent != null && parent.ItemType != typeof(State))
            {
                parent = parent.Parent;
            }
            return parent;
        }

        static internal ConnectionPoint GetClosestConnectionPoint(ConnectionPoint srcConnPoint, List<ConnectionPoint> destConnPoints, out double minDist)
        {
            minDist = double.PositiveInfinity;
            double dist = 0;
            ConnectionPoint closestPoint = null;
            foreach (ConnectionPoint destConnPoint in destConnPoints)
            {
                if(srcConnPoint != destConnPoint)
                {
                    dist = DesignerGeometryHelper.DistanceBetweenPoints(srcConnPoint.Location, destConnPoint.Location);
                    if (dist < minDist)
                    {
                        minDist = dist;
                        closestPoint = destConnPoint;
                    }
                }
            }

            return closestPoint;
        }

        static ConnectionPoint GetClosestConnectionPointNotOfType(ConnectionPoint srcConnectionPoint, List<ConnectionPoint> targetConnectionPoints, ConnectionPointType illegalConnectionPointType)
        {
            double minDist;
            List<ConnectionPoint> filteredConnectionPoints = new List<ConnectionPoint>();
            foreach (ConnectionPoint connPoint in targetConnectionPoints)
            {
                if (connPoint.PointType != illegalConnectionPointType && !connPoint.Equals(srcConnectionPoint) && connPoint.AttachedConnectors.Count == 0)
                {
                    filteredConnectionPoints.Add(connPoint);
                }
            }
            return GetClosestConnectionPoint(srcConnectionPoint, filteredConnectionPoints, out minDist);
        }

        static void GetClosestConnectionPointPair(List<ConnectionPoint> srcConnPoints, List<ConnectionPoint> destConnPoints, out ConnectionPoint srcConnPoint, out ConnectionPoint destConnPoint)
        {
            double minDist = double.PositiveInfinity;
            double dist;
            ConnectionPoint tempConnPoint;
            srcConnPoint = null;
            destConnPoint = null;
            foreach (ConnectionPoint connPoint in srcConnPoints)
            {
                tempConnPoint = GetClosestConnectionPoint(connPoint, destConnPoints, out dist);
                if (dist < minDist)
                {
                    minDist = dist;
                    srcConnPoint = connPoint;
                    destConnPoint = tempConnPoint;
                }
            }
            Debug.Assert(srcConnPoint != null, "No ConnectionPoint found");
            Debug.Assert(destConnPoint != null, "No ConnectionPoint found");
        }

        static void GetEmptySrcDestConnectionPoints(UIElement source, UIElement dest, out ConnectionPoint srcConnPoint, out ConnectionPoint destConnPoint)
        {
            srcConnPoint = null;
            destConnPoint = null;
            List<ConnectionPoint> srcConnectionPoints = GetEmptyConnectionPoints(source);
            List<ConnectionPoint> destConnectionPoints = GetEmptyConnectionPoints(dest);
            if (srcConnectionPoints.Count > 0 && destConnectionPoints.Count > 0)
            {
                GetClosestConnectionPointPair(srcConnectionPoints, destConnectionPoints, out srcConnPoint, out destConnPoint);
            }
        }

        internal static List<ConnectionPoint> GetEmptyConnectionPoints(UIElement designer)
        {
            List<ConnectionPoint> connectionPoints = StateContainerEditor.GetConnectionPoints(designer);
            if (connectionPoints != null)
            {
                return new List<ConnectionPoint>(connectionPoints.Where<ConnectionPoint>(
                    (p) => { return p.AttachedConnectors == null || p.AttachedConnectors.Count == 0; }));
            }
            return new List<ConnectionPoint>();
        }

        //This returns the closest non-incoming connectionPoint on source. Return value will be different than destConnectionPoint.
        static ConnectionPoint GetClosestSrcConnectionPoint(UIElement src, ConnectionPoint destConnectionPoint)
        {
            ConnectionPoint srcConnectionPoint = null;
            if (destConnectionPoint.PointType != ConnectionPointType.Outgoing)
            {
                srcConnectionPoint = GetClosestConnectionPointNotOfType(destConnectionPoint, StateContainerEditor.GetConnectionPoints(src), ConnectionPointType.Incoming);
            }
            return srcConnectionPoint;
        }

        //This returns the closest non-outgoing connectionPoint on dest. Return value will be different than sourceConnectionPoint.
        static ConnectionPoint GetClosestDestConnectionPoint(ConnectionPoint sourceConnectionPoint, UIElement dest)
        {
            ConnectionPoint destConnectionPoint = null;
            if (sourceConnectionPoint.PointType != ConnectionPointType.Incoming)
            {
                destConnectionPoint = GetClosestConnectionPointNotOfType(sourceConnectionPoint, StateContainerEditor.GetConnectionPoints(dest), ConnectionPointType.Outgoing);
            }
            return destConnectionPoint;
        }

        internal static IEnumerable<ModelItem> GetAllChildStateModelItems(ModelItem stateModelItem)
        {
            Debug.Assert(stateModelItem.ItemType == typeof(State));
            List<ModelItem> children = new List<ModelItem>();
            ModelItemCollection childCollection = stateModelItem.Properties[ChildStatesPropertyName].Collection;
            children.AddRange(childCollection);
            foreach (ModelItem childState in childCollection)
            {
                children.AddRange(GetAllChildStateModelItems(childState));
            }
            return children;
        }

        static IEnumerable<UIElement> GetAllChildStateDesigners(UIElement stateDesigner)
        {
            List<UIElement> children = new List<UIElement>();
            StateDesigner designer = stateDesigner as StateDesigner;
            if(designer != null && designer.StateContainerEditor != null)
            {
                children.AddRange(designer.StateContainerEditor.modelItemToUIElement.Values);
                foreach(UIElement child in designer.StateContainerEditor.modelItemToUIElement.Values)
                {
                    children.AddRange(GetAllChildStateDesigners(child));
                }
            }
            return children;
        }

        static void SetChildViewSize(ContentControl control, ModelItem model)
        {
            control.MinWidth = ChildViewMinWidth;
            if ((bool)model.Properties[StateDesigner.IsFinalPropertyName].Value.GetCurrentValue())
            {
                SetInitialFinalViewSize(control);
            }
        }

        static void SetInitialFinalViewSize(ContentControl control)
        {
            control.Width = InitialFinalWidth;
            control.Height = InitialFinalHeight;
        }

        // Test if the transition is contained by any of the states or their descendants
        static bool IsTransitionModelItemContainedByStateModelItems(ModelItem transitionModelItem, IEnumerable<ModelItem> stateModelItems)
        {
            foreach(ModelItem stateModelItem in stateModelItems)
            {
                if(stateModelItem.Properties[StateDesigner.TransitionsPropertyName].Collection.Contains(transitionModelItem))
                {
                    return true;
                }
                else if (IsTransitionModelItemContainedByStateModelItems(transitionModelItem, stateModelItem.Properties[StateContainerEditor.ChildStatesPropertyName].Collection))
                {
                    return true;
                }
            }
            return false;
        }

        static bool IsTransitionDestinationWithinStates(Transition transition, IEnumerable<State> states)
        {
            foreach(State state in states)
            {
                if(transition.To == state)
                {
                    return true;
                }
                else if(IsTransitionDestinationWithinStates(transition, state.States))
                {
                    return true;
                }
            }
            return false;
        }

        // Remove dangling transitions that are not pointing to any of the input states or their descendants
        static void RemoveDanglingTransitions(IEnumerable<State> states)
        {
            Queue<State> statesToProcess = new Queue<State>(states);
            while(statesToProcess.Count > 0)
            {
                State state = statesToProcess.Dequeue();
                
                IEnumerable<Transition> toRemove = state.Transitions.Where<Transition>((p) =>
                    { return !IsTransitionDestinationWithinStates(p, states); }).Reverse();
                foreach (Transition transition in toRemove)
                {
                    state.Transitions.Remove(transition);
                }

                foreach(State childState in state.States)
                {
                    statesToProcess.Enqueue(childState);
                }
            }
        }

        static List<ModelItem> GetTransitionModelItems(IEnumerable<ModelItem> stateModelItems)
        {
            List<ModelItem> transitionModelItems = new List<ModelItem>();
            foreach(ModelItem stateModelItem in stateModelItems)
            {
                transitionModelItems.AddRange(stateModelItem.Properties[StateDesigner.TransitionsPropertyName].Collection);
                transitionModelItems.AddRange(GetTransitionModelItems(stateModelItem.Properties[ChildStatesPropertyName].Collection));
            }
            return transitionModelItems;
        }

        static bool IsDescendantStateOf(ModelItem descendant, ModelItem ancestor)
        {
            if (descendant != null && descendant.ItemType == typeof(State) && ancestor != null && ancestor.ItemType == typeof(State))
            {
                ModelItem current = descendant.Parent;
                while (current != null && current.ItemType != typeof(StateMachine))
                {
                    if (current == ancestor)
                    {
                        return true;
                    }
                    current = current.Parent;
                }
            }
            return false;
        }

        static void ShowMessageBox(string message)
        {
            MessageBox.Show(message, SR.ErrorMessageBoxTitle, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        static void ReportConnectorCreationError(ConnectorCreationResult result)
        {
            switch (result)
            {
                case ConnectorCreationResult.CannotCreateTransitionToCompositeState:
                    ShowMessageBox(SR.CannotCreateTransitionToCompositeState);
                    break;
                case ConnectorCreationResult.CannotCreateTransitionFromAncestorToDescendant:
                    ShowMessageBox(SR.CannotCreateTransitionFromAncestorToDescendant);
                    break;
                case ConnectorCreationResult.CannotSetCompositeStateAsInitialState:
                    ShowMessageBox(SR.CannotSetCompositeStateAsInitialState);
                    break;
                case ConnectorCreationResult.CannotSetFinalStateAsInitialState:
                    ShowMessageBox(SR.CannotSetFinalStateAsInitialState);
                    break;
                case ConnectorCreationResult.OtherFailure:
                    ShowMessageBox(SR.CannotCreateLink);
                    break;
            }
        }

        static bool IsConnectorFromInitialNode(Connector connector)
        {
            return GetConnectorModelItem(connector).ItemType == typeof(StateMachine);
        }
    }
}

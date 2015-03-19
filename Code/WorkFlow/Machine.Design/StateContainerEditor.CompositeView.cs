//-----------------------------------------------------------------------------

//-----------------------------------------------------------------------------

namespace Machine.Design
{
    using System;
    using System.Activities.Presentation;
    using System.Activities.Presentation.Model;
    using System.Activities.Presentation.View;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Windows;
    using System.Windows.Media;
   
    using Machine.Design.FreeFormEditing;
    using Machine.Design.ToolboxItems;

   public partial class StateContainerEditor : ICompositeView
    {
        public static readonly DependencyProperty DroppingTypeResolvingOptionsProperty =
            DependencyProperty.Register("DroppingTypeResolvingOptions", typeof(TypeResolvingOptions), typeof(StateContainerEditor));

        public TypeResolvingOptions DroppingTypeResolvingOptions
        {
            get { return (TypeResolvingOptions)GetValue(DroppingTypeResolvingOptionsProperty); }
            set { SetValue(DroppingTypeResolvingOptionsProperty, value); }
        }

        public bool IsDefaultContainer
        {
            get { return true; }
        }

        public void OnItemMoved(ModelItem modelItem)
        {
            Debug.Assert(this.modelItemToUIElement.ContainsKey(modelItem), "Moved item does not exist.");
            this.DoDeleteItems(new List<ModelItem> { modelItem }, false);
        }

        public object OnItemsCopied(List<ModelItem> itemsToCopy)
        {
            // Save the locations of copied items relative to the outmost editor to the metadata.
            // The metadata will be used to translate the location view states of pasted items to the pasting target.
            PointCollection metaData = new PointCollection();
            foreach (ModelItem modelItem in itemsToCopy)
            {
                object viewState = this.ViewStateService.RetrieveViewState(modelItem, ShapeLocationViewStateKey);
                Point location = (Point)viewState;
                StateContainerEditor parentDesigner = StateContainerEditor.GetVisualAncestor<StateContainerEditor>(modelItem.View);
                location = parentDesigner.panel.GetLocationRelativeToOutmostPanel(location);
                metaData.Add(location);
            }
            return metaData;
        }

        public object OnItemsCut(List<ModelItem> itemsToCut)
        {
            object metaData = OnItemsCopied(itemsToCut);
            this.OnItemsDelete(itemsToCut);
            return metaData;
        }

        public void OnItemsDelete(List<ModelItem> itemsToDelete)
        {
            DoDeleteItems(itemsToDelete, true);
        }

        void DoDeleteItems(List<ModelItem> itemsToDelete, bool removeIncomingConnectors)
        {
            HashSet<Connector> connectorsToDelete = new HashSet<Connector>();
            List<ModelItem> allStateModelItemsToDelete = new List<ModelItem>();
            IEnumerable<ModelItem> selectedStateModelItems = this.Context.Items.GetValue<Selection>().SelectedObjects.
                Where<ModelItem>((p) => { return p.ItemType == typeof(State); });

            foreach (ModelItem stateModelItem in itemsToDelete)
            {
                allStateModelItemsToDelete.Add(stateModelItem);
                allStateModelItemsToDelete.AddRange(GetAllChildStateModelItems(stateModelItem));
            }

            foreach (ModelItem modelItem in allStateModelItemsToDelete)
            {
                // We only need to delete incoming connectors to the states to be deleted; outgoing connectors will be deleted
                // automatically when the containing state is deleted.
                List<Connector> incomingConnectors = StateContainerEditor.GetIncomingConnectors((UIElement)modelItem.View);
                foreach (Connector connector in incomingConnectors)
                {
                    ModelItem transitionModelItem = StateContainerEditor.GetConnectorModelItem(connector);
                    // If the transition is contained by the states to delete, we don't bother to delete it separately.
                    if (!StateContainerEditor.IsTransitionModelItemContainedByStateModelItems(transitionModelItem, selectedStateModelItems))
                    {
                        connectorsToDelete.Add(connector);
                    }
                }
            }

            // If we don't need to remove incoming connectors, we still remove the transitions but then add them back later.
            // This is in order to create an undo unit that contains the change notifications needed to make undo/redo work correctly.
            foreach (Connector connector in connectorsToDelete)
            {
                ModelItem connectorModelItem = StateContainerEditor.GetConnectorModelItem(connector);
                if (removeIncomingConnectors || connectorModelItem.ItemType == typeof(Transition))
                {
                    this.DeleteConnectorModelItem(connector);
                }
            }
            if (!removeIncomingConnectors)
            {
                foreach (Connector connector in connectorsToDelete)
                {
                    ModelItem connectorModelItem = StateContainerEditor.GetConnectorModelItem(connector);
                    if (connectorModelItem.ItemType == typeof(Transition))
                    {
                        StateContainerEditor.GetParentStateModelItemForTransition(connectorModelItem).Properties[StateDesigner.TransitionsPropertyName].Collection.Add(connectorModelItem);
                    }
                }
            }

            if (null != itemsToDelete)
            {
                itemsToDelete.ForEach(p => this.DeleteState(p, removeIncomingConnectors));
            }
        }

        public bool CanPasteItems(List<object> itemsToPaste)
        {
            if (itemsToPaste != null)
            {
                return itemsToPaste.All(p =>
                    (typeof(State) == p.GetType() && !(((State)p).IsFinal && this.ModelItem.ItemType != typeof(StateMachine))) ||
                    (p is Type && typeof(State) == (Type)p) ||
                    (p is Type && typeof(FinalState) == (Type)p && this.ModelItem.ItemType == typeof(StateMachine)));
            }
            return false;
        }

        public void OnItemsPasted(List<object> itemsToPaste, List<object> metaData, Point pastePoint, WorkflowViewElement pastePointReference)
        {
            List<ModelItem> modelItemsPasted = new List<ModelItem>();
            List<State> states = new List<State>();
            foreach (object obj in itemsToPaste)
            {
                State state;
                if (obj is FinalState)
                {
                    state = new State() { DisplayName = SR.DefaultFinalStateDisplayName, IsFinal = true };
                }
                else
                {
                    state = (State)obj;
                    if (state.DisplayName == null)
                    {
                        state.DisplayName = SR.DefaultStateDisplayName;
                    }
                }
                states.Add(state);
            }

            RemoveDanglingTransitions(states);
            foreach (State state in states)
            {
                ModelItem stateModelItem = this.ModelItem.Properties[ChildStatesPropertyName].Collection.Add(state);
                modelItemsPasted.Add(stateModelItem);
            }
            if (modelItemsPasted.Count > 0)
            {
                // translate location view states to be in the coordinate system of the pasting target             
                this.UpdateLocationViewStatesByMetaData(modelItemsPasted, metaData);
                if (pastePoint.X > 0 && pastePoint.Y > 0)
                {
                    if (pastePointReference != null)
                    {
                        pastePoint = pastePointReference.TranslatePoint(pastePoint, this.panel);
                        pastePoint.X = pastePoint.X < 0 ? 0 : pastePoint.X;
                        pastePoint.Y = pastePoint.Y < 0 ? 0 : pastePoint.Y;
                    }
                    this.UpdateLocationViewStatesByPoint(modelItemsPasted, pastePoint);
                }
                // If paste point is not available, paste the items to the top left corner.
                else
                {
                    this.UpdateLocationViewStatesToTopLeft(modelItemsPasted);
                }
            }

            this.Context.Items.SetValue(new Selection(modelItemsPasted));
        }

        void UpdateLocationViewStatesByPoint(List<ModelItem> itemsPasted, Point point)
        {
            Point topLeft = new Point(Double.PositiveInfinity, Double.PositiveInfinity);
            foreach (ModelItem stateModelItem in itemsPasted)
            {
                object viewState = this.ViewStateService.RetrieveViewState(stateModelItem, ShapeLocationViewStateKey);
                if (viewState != null)
                {
                    Point location = (Point)viewState;
                    topLeft.X = topLeft.X > location.X ? location.X : topLeft.X;
                    topLeft.Y = topLeft.Y > location.Y ? location.Y : topLeft.Y;
                }
            }
            OffsetLocationViewStates(new Vector(point.X - topLeft.X, point.Y - topLeft.Y), itemsPasted, GetTransitionModelItems(itemsPasted), false);
        }

        void UpdateLocationViewStatesByMetaData(List<ModelItem> itemsPasted, List<object> metaData)
        {
            // If the states are not copied from state machine view (e.g., when the State designer is the breadcrumb root), 
            // there is no meta data
            if (metaData != null && metaData.Count > 0)
            {
                StateContainerEditor outmostEditor = this.GetOutmostStateContainerEditor();
                int ii = 0;
                foreach (object data in metaData)
                {
                    PointCollection points = (PointCollection)data;
                    foreach (Point point in points)
                    {
                        // translate location view states to be in the coordinate system of the pasting target
                        this.ViewStateService.StoreViewState(itemsPasted[ii], ShapeLocationViewStateKey, outmostEditor.panel.TranslatePoint(point, this.panel));
                        ++ii;
                    }
                }
                Debug.Assert(itemsPasted.Count == ii, "itemsCopied does not match the metaData.");
            }
        }

        void OffsetLocationViewStates(Vector offsetVector, IEnumerable<ModelItem> stateModelItems, IEnumerable<ModelItem> transitionModelItems, bool enableUndo)
        {
            // Offset view state for states
            if (stateModelItems != null)
            {
                foreach (ModelItem modelItem in stateModelItems)
                {
                    object viewState = this.ViewStateService.RetrieveViewState(modelItem, ShapeLocationViewStateKey);
                    if (viewState != null)
                    {
                        viewState = Point.Add((Point)viewState, offsetVector);
                        if (enableUndo)
                        {
                            this.ViewStateService.StoreViewStateWithUndo(modelItem, ShapeLocationViewStateKey, viewState);
                        }
                        else
                        {
                            this.ViewStateService.StoreViewState(modelItem, ShapeLocationViewStateKey, viewState);
                        }
                    }
                }
            }
            // Offset view state for transitions
            if (transitionModelItems != null)
            {
                foreach (ModelItem modelItem in transitionModelItems)
                {
                    object viewState = this.ViewStateService.RetrieveViewState(modelItem, ConnectorLocationViewStateKey);
                    if (viewState != null)
                    {
                        PointCollection locations = (PointCollection)viewState;
                        PointCollection newLocations = new PointCollection();
                        foreach (Point location in locations)
                        {
                            Point newLocation = Point.Add(location, offsetVector);
                            newLocation.X = newLocation.X < 0 ? 0 : newLocation.X;
                            newLocation.Y = newLocation.Y < 0 ? 0 : newLocation.Y;
                            newLocations.Add(newLocation);
                        }
                        if (enableUndo)
                        {
                            this.ViewStateService.StoreViewStateWithUndo(modelItem, ConnectorLocationViewStateKey, newLocations);
                        }
                        else
                        {
                            this.ViewStateService.StoreViewState(modelItem, ConnectorLocationViewStateKey, newLocations);
                        }
                    }
                }
            }
        }

        void UpdateLocationViewStatesToTopLeft(List<ModelItem> itemsPasted)
        {
            this.UpdateLocationViewStatesByPoint(itemsPasted, new Point(GridSize, GridSize));
            this.UpdateLocationViewStatesToAvoidOverlap(itemsPasted);
        }

        void UpdateLocationViewStatesToAvoidOverlap(List<ModelItem> itemsPasted)
        {
            int offset = 0;
            if (itemsPasted.Count > 0)
            {
                //Check to see if the first element in the input list needs offset. Generalize that information for all ModelItems in the input list.
                object location = this.ViewStateService.RetrieveViewState(itemsPasted[0], ShapeLocationViewStateKey);
                if (location != null)
                {
                    Point locationOfShape = (Point)location;
                    while (this.shapeLocations.Contains(locationOfShape))
                    {
                        offset++;
                        locationOfShape.Offset(StateContainerEditor.GridSize, StateContainerEditor.GridSize);
                    }
                }
            }
            //Update ViewState according to calculated offset.
            if (offset > 0)
            {
                double offsetValue = StateContainerEditor.GridSize * offset;
                OffsetLocationViewStates(new Vector(offsetValue, offsetValue), itemsPasted, GetTransitionModelItems(itemsPasted), false);
            }
        }
    }
}

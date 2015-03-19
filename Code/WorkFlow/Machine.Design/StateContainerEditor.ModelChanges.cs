//-----------------------------------------------------------------------------

//-----------------------------------------------------------------------------

namespace Machine.Design
{
    using System.Activities.Presentation;
    using System.Activities.Presentation.Model;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Media;
    using Machine.Design.FreeFormEditing;
    

  public  partial class StateContainerEditor
    {
        internal void DeleteConnectorModelItem(Connector connector)
        {
            ModelItem connectorModelItem = StateContainerEditor.GetConnectorModelItem(connector);
            if (connectorModelItem.ItemType == typeof(Transition))
            {
                StateContainerEditor.GetParentStateModelItemForTransition(connectorModelItem).Properties[StateDesigner.TransitionsPropertyName].Collection.Remove(connectorModelItem);
            }
            // Connector from initial node
            else if (connectorModelItem.ItemType == typeof(StateMachine))
            {
                using (EditingScope es = (EditingScope)this.ModelItem.BeginEdit(SR.ClearInitialState))
                {
                    connectorModelItem.Properties[StateMachineDesigner.InitialStatePropertyName].SetValue(null);
                    es.Complete();
                }
            }
        }

        void DeleteState(ModelItem stateModelItem, bool clearInitialState)
        {
            this.ModelItem.Properties[ChildStatesPropertyName].Collection.Remove(stateModelItem);
            if (clearInitialState && this.ModelItem.ItemType == typeof(StateMachine) && stateModelItem == this.ModelItem.Properties[StateMachineDesigner.InitialStatePropertyName].Value)
            {
                this.ModelItem.Properties[StateMachineDesigner.InitialStatePropertyName].SetValue(null);
            }
        }

        // referenceTransitionModelItem is used when a connector is re-linked.
        void CreateTransition(ConnectionPoint sourceConnPoint, ConnectionPoint destConnPoint, ModelItem referenceTransitionModelItem, bool isSourceMoved)
        {
            Debug.Assert(this.IsOutmostStateContainerEditor(), "Should only be called by the outmost editor.");

            WorkflowViewElement srcDesigner = sourceConnPoint.ParentDesigner as WorkflowViewElement;
            WorkflowViewElement destDesigner = destConnPoint.ParentDesigner as WorkflowViewElement;
            Debug.Assert(srcDesigner is StateDesigner && destDesigner is StateDesigner, "The source and destination designers should both be StateDesigner");

            ModelItem srcModelItem = srcDesigner.ModelItem;
            ModelItem destModelItem = destDesigner.ModelItem;
            ModelItem transitionModelItem = null;

            // We are moving the connector.
            if (referenceTransitionModelItem != null && referenceTransitionModelItem.ItemType == typeof(Transition))
            {
                transitionModelItem = referenceTransitionModelItem;
                // We are moving the start of the connector. We only preserve the trigger if it is not shared.
                if(isSourceMoved)
                {
                    Transition referenceTransition = referenceTransitionModelItem.GetCurrentValue() as Transition;
                    ModelItem stateModelItem = GetParentStateModelItemForTransition(referenceTransitionModelItem);
                    State state = stateModelItem.GetCurrentValue() as State;
                    bool isTriggerShared = false;
                    foreach (Transition transition in state.Transitions)
                    {
                        if (transition != referenceTransition && transition.Trigger == referenceTransition.Trigger)
                        {
                            isTriggerShared = true;
                            break;
                        }
                    }
                    if (isTriggerShared)
                    {
                        transitionModelItem.Properties[TransitionDesigner.TriggerPropertyName].SetValue(null);
                    }
                }
                transitionModelItem.Properties[TransitionDesigner.ToPropertyName].SetValue(destModelItem);
                srcModelItem.Properties[StateDesigner.TransitionsPropertyName].Collection.Add(transitionModelItem);
            }
            // We are creating a new connector. 
            else
            {
                Transition newTransition = new Transition() { DisplayName = string.Empty };
                newTransition.To = destModelItem.GetCurrentValue() as State;
                // Assign the shared trigger.
                if (sourceConnPoint.AttachedConnectors.Count > 0)
                {
                    Connector connector = sourceConnPoint.AttachedConnectors[0];
                    Transition existingTransition = StateContainerEditor.GetConnectorModelItem(connector).GetCurrentValue() as Transition;
                    newTransition.Trigger = existingTransition.Trigger;
                }
                transitionModelItem = srcModelItem.Properties[StateDesigner.TransitionsPropertyName].Collection.Add(newTransition);
            }
            if (transitionModelItem != null)
            {
                PointCollection connectorViewState = new PointCollection(ConnectorRouter.Route(this.panel, sourceConnPoint, destConnPoint));
                this.StoreConnectorLocationViewState(transitionModelItem, connectorViewState, true);
            }
        }

        // referenceConnector is used when we are re-linking the connector.
        internal ConnectorCreationResult CreateConnectorGesture(ConnectionPoint sourceConnectionPoint, ConnectionPoint destConnectionPoint, Connector referenceConnector, bool isConnectorStartMoved)
        {
            Debug.Assert(this.IsOutmostStateContainerEditor(), "Should only be called by the outmost editor.");
            Debug.Assert(sourceConnectionPoint != null, "sourceConnectionPoint is null.");
            Debug.Assert(destConnectionPoint != null, "destConnectionPoint is null.");
            ConnectorCreationResult result = ConnectorCreationResult.OtherFailure;
            if (destConnectionPoint.PointType != ConnectionPointType.Outgoing && sourceConnectionPoint.PointType != ConnectionPointType.Incoming)
            {
                if (sourceConnectionPoint.ParentDesigner is StateDesigner)
                {
                    bool sameDestination = false;
                    ModelItem refTransitionModelItem = null;
                    if(referenceConnector != null)
                    {
                        refTransitionModelItem = StateContainerEditor.GetConnectorModelItem(referenceConnector);
                        ModelItem destStateModelItem = ((StateDesigner)destConnectionPoint.ParentDesigner).ModelItem;
                        if (refTransitionModelItem != null && refTransitionModelItem.Properties[TransitionDesigner.ToPropertyName].Value == destStateModelItem)
                        {
                            sameDestination = true;
                        }
                    }

                    // We do not allow transitions to composite states unless we don't change the transition destination 
                    // (e.g., we are moving the start of a connector).
                    if (!sameDestination && !((StateDesigner)destConnectionPoint.ParentDesigner).IsSimpleState())
                    {
                        result = ConnectorCreationResult.CannotCreateTransitionToCompositeState;
                    }
                    // We do not allow transitions from an ancestor to its descendant
                    else if (StateContainerEditor.IsDescendantStateOf(((StateDesigner)destConnectionPoint.ParentDesigner).ModelItem, ((StateDesigner)sourceConnectionPoint.ParentDesigner).ModelItem))
                    {
                        result = ConnectorCreationResult.CannotCreateTransitionFromAncestorToDescendant;
                    }
                    else
                    {
                        using (EditingScope es = (EditingScope)this.ModelItem.BeginEdit(SR.CreateTransition))
                        {
                            if (refTransitionModelItem != null)
                            {
                                this.CreateTransition(sourceConnectionPoint, destConnectionPoint, refTransitionModelItem, isConnectorStartMoved);
                            }
                            else
                            {
                                this.CreateTransition(sourceConnectionPoint, destConnectionPoint, null, false);
                            }
                            result = ConnectorCreationResult.Success;
                            es.Complete();
                        }
                    }
                }
                else if (sourceConnectionPoint.ParentDesigner is InitialNode)
                {
                    StateDesigner destDesigner = (StateDesigner)destConnectionPoint.ParentDesigner;
                    // We only allow simple states to be set as the initial state
                    if (!destDesigner.IsSimpleState())
                    {
                        result = ConnectorCreationResult.CannotSetCompositeStateAsInitialState;
                    }
                    else if (destDesigner.IsFinalState())
                    {
                        result = ConnectorCreationResult.CannotSetFinalStateAsInitialState;
                    }
                    else
                    {
                        ModelItem destModelItem = destDesigner.ModelItem;
                        using (EditingScope es = (EditingScope)this.ModelItem.BeginEdit(SR.SetInitialState))
                        {
                            this.StateMachineModelItem.Properties[StateMachineDesigner.InitialStatePropertyName].SetValue(destModelItem);
                            PointCollection connectorViewState = new PointCollection(ConnectorRouter.Route(this.panel, sourceConnectionPoint, destConnectionPoint));
                            this.StoreConnectorLocationViewState(this.StateMachineModelItem, connectorViewState, true);
                            result = ConnectorCreationResult.Success;
                            es.Complete();
                        }
                    }
                }
            }
            return result;
        }

        internal ConnectorCreationResult CreateConnectorGesture(ConnectionPoint sourceConnectionPoint, UIElement dest, Connector referenceConnector, bool isConnectorStartMoved)
        {
            ConnectionPoint destConnectionPoint = GetClosestDestConnectionPoint(sourceConnectionPoint, dest);
            if (destConnectionPoint != null)
            {
                return CreateConnectorGesture(sourceConnectionPoint, destConnectionPoint, referenceConnector, isConnectorStartMoved);
            }
            return ConnectorCreationResult.OtherFailure;
        }

        internal ConnectorCreationResult CreateConnectorGesture(UIElement source, ConnectionPoint destConnectionPoint, Connector referenceConnector, bool isConnectorStartMoved)
        {
            ConnectionPoint sourceConnectionPoint = GetClosestSrcConnectionPoint(source, destConnectionPoint);
            if (sourceConnectionPoint != null)
            {
                return CreateConnectorGesture(sourceConnectionPoint, destConnectionPoint, referenceConnector, isConnectorStartMoved);
            }
            return ConnectorCreationResult.OtherFailure;
        }

        void StoreShapeLocationViewState(WorkflowViewElement view, Point newLocation)
        {
            StoreShapeLocationViewState(view.ModelItem, newLocation);
        }

        void StoreShapeLocationViewState(ModelItem storageModelItem, Point newLocation)
        {
            if (this.ViewStateService.RetrieveViewState(storageModelItem, ShapeLocationViewStateKey) != null)
            {
                this.ViewStateService.StoreViewStateWithUndo(storageModelItem, ShapeLocationViewStateKey, newLocation);
            }
            else
            {
                this.ViewStateService.StoreViewState(storageModelItem, ShapeLocationViewStateKey, newLocation);
            }
        }

        void StoreConnectorLocationViewState(ModelItem connectorModelItem, PointCollection viewState, bool isUndoableViewState)
        {
            if (isUndoableViewState)
            {
                this.ViewStateService.StoreViewStateWithUndo(connectorModelItem, ConnectorLocationViewStateKey, viewState);
            }
            else
            {
                this.ViewStateService.StoreViewState(connectorModelItem, ConnectorLocationViewStateKey, viewState);
            }
        }

        void StoreConnectorLocationViewState(Connector connector, bool isUndoableViewState)
        {
            //This method will be called whenever the FreeFormPanel raises a location changed event on a connector.
            //Such location changed events are a result of changes already committed in the UI. Hence we do not want to react to such view state changes.
            //Using internalViewStateChange flag for that purpose.
            this.internalViewStateChange = true;
            this.StoreConnectorLocationViewState(StateContainerEditor.GetConnectorModelItem(connector), connector.Points, isUndoableViewState);
            this.internalViewStateChange = false;
        }
    }
}

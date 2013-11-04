//-----------------------------------------------------------------------------

//-----------------------------------------------------------------------------

namespace Machine.Design
{
    using System;
    using System.Activities.Presentation;
    using System.Activities.Presentation.Model;
    using System.Activities.Presentation.View;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Threading;
    using Machine.Design.FreeFormEditing;
    

   public partial class StateContainerEditor
    {
        void OnStateCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            StateContainerEditor outmostEditor = this.GetOutmostStateContainerEditor();
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                if (e.OldItems != null)
                {
                    foreach (ModelItem deleted in e.OldItems)
                    {
                        if (deleted != null)
                        {
                            ModelItemCollection transitions = deleted.Properties[StateDesigner.TransitionsPropertyName].Collection;
                            if (outmostEditor.listenedTransitionCollections.Contains(transitions))
                            {
                                transitions.CollectionChanged -=
                                    new NotifyCollectionChangedEventHandler(outmostEditor.OnTransitionCollectionChanged);
                                outmostEditor.listenedTransitionCollections.Remove(transitions);
                            }
                            
                            if (this.modelItemToUIElement.ContainsKey(deleted))
                            {
                                this.RemoveStateVisual(this.modelItemToUIElement[deleted] as WorkflowViewElement);
                            }
                        }
                    }
                }
            }

            else if (e.Action == NotifyCollectionChangedAction.Add)
            {
                if (e.NewItems != null)
                {
                    foreach (ModelItem added in e.NewItems)
                    {
                        if (added != null)
                        {
                            ModelItemCollection transitions = added.Properties[StateDesigner.TransitionsPropertyName].Collection;
                            if (!outmostEditor.listenedTransitionCollections.Contains(transitions))
                            {
                                transitions.CollectionChanged +=
                                    new NotifyCollectionChangedEventHandler(outmostEditor.OnTransitionCollectionChanged);
                                outmostEditor.listenedTransitionCollections.Add(transitions);
                            }
                            this.AddStateVisuals(new List<ModelItem> { added });
                        }
                    }
                }
            }
        }

        void OnTransitionCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Debug.Assert(this.IsOutmostStateContainerEditor(), "Only the outmost editor should listen to the CollectionChanged events of transitions.");
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                if (e.OldItems != null)
                {
                    foreach (ModelItem deleted in e.OldItems)
                    {
                        if (deleted != null)
                        {
                            Connector connector = this.GetConnectorOnOutmostEditor(deleted);
                            if (connector != null)
                            {
                                this.RemoveConnectorOnOutmostEditor(connector);
                            }
                        }
                    }
                }
            }

            else if (e.Action == NotifyCollectionChangedAction.Add)
            {
                // We have to postpone updating the visual until the editing scope completes because 
                // the connector view state is not available at this moment
                foreach (ModelItem item in e.NewItems)
                {
                    if (!this.transitionModelItemsAdded.Contains(item))
                    {
                        this.transitionModelItemsAdded.Add(item);
                    }
                }
            }
        }

        void OnModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == StateMachineDesigner.InitialStatePropertyName)
            {
                Debug.Assert(this.ModelItem.ItemType == typeof(StateMachine), "Only StateMachine should have initial state");
                this.initialStateChanged = true;
            }
        }

        // All the connectors are directly contained by the outmost editor. This is because connectors can go across states.
        void OnEditingScopeCompleted(object sender, EditingScopeEventArgs e)
        {
            Debug.Assert(this.IsOutmostStateContainerEditor(), "Only the outmost editor should listen to the EditingScopeCompleted events of the model tree.");
            if (this.transitionModelItemsAdded.Count > 0)
            {
                // We need to wait until after the state visuals are updated
                this.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(() =>
                {
                    foreach (ModelItem transition in this.transitionModelItemsAdded)
                    {
                        ModelItem srcStateModelItem = StateContainerEditor.GetParentStateModelItemForTransition(transition);
                        this.AddTransitionVisual(transition);
                    }
                    this.transitionModelItemsAdded.Clear();
                }));
            }
            if (this.initialStateChanged)
            {
                Debug.Assert(this.ModelItem.ItemType == typeof(StateMachine), "Only StateMachine should have initial state");
                Debug.Assert(this.initialNode != null, "Initial node should not be null");

                // Remove the old link
                if (GetAttachedConnectors(this.initialNode).Count > 0)
                {
                    this.RemoveConnectorOnOutmostEditor(GetAttachedConnectors(this.initialNode)[0]);
                }
                // Add the new link if the new initial state is not null
                ModelItem initialStateModelItem = this.ModelItem.Properties[StateMachineDesigner.InitialStatePropertyName].Value;
                if (initialStateModelItem != null)
                {
                    // We need to wait until after the state visuals are updated
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(() =>
                    {
                        this.AddInitialNodeConnector();
                    }));
                }
                this.initialStateChanged = false;
            }
        }

        void OnViewStateChanged(object sender, ViewStateChangedEventArgs e)
        {
            Debug.Assert(e.ParentModelItem != null, "ViewState should be associated with some modelItem");

            if (!this.internalViewStateChange)
            {
                if (e.ParentModelItem == this.ModelItem)
                {
                    if (string.Equals(e.Key, StateContainerWidthViewStateKey, StringComparison.Ordinal))
                    {
                        double defaultWidth = ((this.ModelItem.ItemType == typeof(State)) ? DefaultStateWidth : DefaultStateMachineWidth);
                        object widthViewState = this.ViewStateService.RetrieveViewState(this.ModelItem, StateContainerWidthViewStateKey);
                        this.StateContainerWidth = (widthViewState != null) ? (double)widthViewState : defaultWidth;
                    }
                    else if (string.Equals(e.Key, StateContainerHeightViewStateKey, StringComparison.Ordinal))
                    {
                        double defaultHeight = ((this.ModelItem.ItemType == typeof(State)) ? DefaultStateHeight : DefaultStateMachineHeight);
                        object heightViewState = this.ViewStateService.RetrieveViewState(this.ModelItem, StateContainerHeightViewStateKey);
                        this.StateContainerHeight = (heightViewState != null) ? (double)heightViewState : defaultHeight;
                    }
                }
                
                if (e.ParentModelItem.ItemType == typeof(State) && e.Key.Equals(ShapeLocationViewStateKey))
                {
                    if (this.modelItemToUIElement.ContainsKey(e.ParentModelItem))
                    {
                        if (e.NewValue != null)
                        {
                            FreeFormPanel.SetLocation(this.modelItemToUIElement[e.ParentModelItem], (Point)e.NewValue);
                            this.panel.InvalidateMeasure();
                            if (e.OldValue != null)
                            {
                                this.shapeLocations.Remove((Point)e.OldValue);
                            }
                            this.shapeLocations.Add((Point)e.NewValue);
                            // To reroute the links
                            this.InvalidateMeasureForOutmostPanel();
                        }
                    }
                }

                else if(e.ParentModelItem.ItemType == typeof(State) && e.Key.Equals(ShapeSizeViewStateKey))
                {
                    // To reroute the links
                    this.InvalidateMeasureForOutmostPanel();
                }  

                // Only the outmost editor should respond to connector changes because all connectors are
                // only added to the outmost editor
                else if (e.Key.Equals(ConnectorLocationViewStateKey) && this.IsOutmostStateContainerEditor())
                {
                    Connector changedConnector = this.GetConnectorOnOutmostEditor(e.ParentModelItem);
                    if (changedConnector != null)
                    {
                        if (e.NewValue != null)
                        {
                            Debug.Assert(e.NewValue is PointCollection, "e.NewValue is not PointCollection");
                            changedConnector.Points = e.NewValue as PointCollection;
                            this.panel.RemoveConnectorEditor();
                            this.InvalidateMeasureForOutmostPanel();
                            if (IsConnectorFromInitialNode(changedConnector))
                            {
                                this.initialStateChanged = true;
                            }
                        }
                    }
                }
            }
        }
    }
}

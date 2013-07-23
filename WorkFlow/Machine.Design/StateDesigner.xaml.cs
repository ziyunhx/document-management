//----------------------------------------------------------------

//----------------------------------------------------------------

namespace Machine.Design
{
    using System.Activities.Presentation.Model;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    
    using Machine.Design.FreeFormEditing;
    using System.Collections.Generic;
    using System.Windows.Threading;
    using System;

    public partial class StateDesigner
    {
        ModelItem stateMachineModelItem = null;

        StateContainerEditor stateContainerEditor = null;

        internal const string EntryPropertyName = "Entry";
        internal const string ExitPropertyName = "Exit";
        internal const string IsFinalPropertyName = "IsFinal";
        internal const string TransitionsPropertyName = "Transitions";
        internal const string ChildStatesPropertyName = "States";

        public static readonly RoutedCommand SetAsInitialCommand = new RoutedCommand("SetAsInitial", typeof(StateDesigner));

        public StateDesigner()
        {
            InitializeComponent();
            this.PinAsExpanded();
        }

        protected override void OnShowExpandedChanged(bool newValue)
        {
            this.PinAsExpanded();
        }

        // Make sure StateDesigner is always expanded
        void PinAsExpanded()
        {
            this.ExpandState = true;
            this.PinState = true;
        }

        void OnFinalNameBoxLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            textBox.ScrollToHome();
        }

        internal StateContainerEditor StateContainerEditor
        {
            get { return this.stateContainerEditor; }
        }

        protected override void OnModelItemChanged(object newItem)
        {
            this.stateMachineModelItem = StateContainerEditor.GetStateMachineModelItem(this.ModelItem);
            base.OnModelItemChanged(newItem);
        }

        protected override void OnPreviewGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            if (!this.IsRootDesigner)
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(this.BringToFront));
            }
            base.OnPreviewGotKeyboardFocus(e);
        }

        void BringToFront()
        {
            FreeFormPanel panel = StateContainerEditor.GetVisualAncestor<FreeFormPanel>(this);
            // It is possible when BringToFront is executed, the state has already been deleted
            if (panel != null)
            {
                panel.Children.Remove(this);
                panel.Children.Add(this);

                // Bring to front all attached connectors including those attached to child states
                StateContainerEditor parent = StateContainerEditor.GetVisualAncestor<StateContainerEditor>(this);
                if (parent != null)
                {
                    FreeFormPanel outmostPanel = parent.GetOutmostStateContainerEditor().Panel;
                    HashSet<Connector> connectors = new HashSet<Connector>();
                    List<ModelItem> allStateModelItems = new List<ModelItem>();
                    allStateModelItems.Add(this.ModelItem);
                    allStateModelItems.AddRange(StateContainerEditor.GetAllChildStateModelItems(this.ModelItem));
                    foreach (ModelItem stateModelItem in allStateModelItems)
                    {
                        List<Connector> attachedConnectors = StateContainerEditor.GetAttachedConnectors((UIElement)stateModelItem.View);
                        foreach (Connector connector in attachedConnectors)
                        {
                            connectors.Add(connector);
                        }
                    }
                    foreach (Connector connector in connectors)
                    {
                        outmostPanel.Children.Remove(connector);
                        outmostPanel.Children.Add(connector);
                    }
                }
            }
        }

        void OnStateContainerLoaded(object sender, RoutedEventArgs e)
        {
            this.stateContainerEditor = sender as StateContainerEditor;
        }

        void OnSetAsInitialCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (this.ModelItem != this.stateMachineModelItem.Properties[StateMachineDesigner.InitialStatePropertyName].Value &&
                            !this.IsFinalState() && this.IsSimpleState() && 
                            !this.IsRootDesigner && StateContainerEditor.GetEmptyConnectionPoints(this).Count > 0);
            e.Handled = true;
        }

        void OnSetAsInitialExecute(object sender, ExecutedRoutedEventArgs e)
        {
            using (EditingScope es = (EditingScope)this.ModelItem.BeginEdit(SR.SetInitialState))
            {
                this.ViewStateService.RemoveViewState(this.stateMachineModelItem, StateContainerEditor.ConnectorLocationViewStateKey);
                this.stateMachineModelItem.Properties[StateMachineDesigner.InitialStatePropertyName].SetValue(this.ModelItem.GetCurrentValue());
                es.Complete();
            }
            e.Handled = true;
        }

        void OnStateSpecificMenuItemLoaded(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            if (null != item && this.IsFinalState())
            {
                item.Visibility = Visibility.Collapsed;
            }
            e.Handled = true;
        }

        internal bool IsSimpleState()
        {
            if (this.ModelItem.Properties[StateContainerEditor.ChildStatesPropertyName].Collection.Count == 0)
            {
                return true;
            }
            return false;
        }

        internal bool IsFinalState()
        {
            return (bool)this.ModelItem.Properties[StateDesigner.IsFinalPropertyName].Value.GetCurrentValue();
        }
    }
}
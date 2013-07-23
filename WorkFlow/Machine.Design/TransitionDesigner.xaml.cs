//-----------------------------------------------------------------------------

//-----------------------------------------------------------------------------

namespace Machine.Design
{
    using System.Activities.Presentation.Model;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Media;
    using Machine.Design.FreeFormEditing;

  public  partial class TransitionDesigner
    {
        List<ModelItem> transitionsSharingTrigger = new List<ModelItem>();

        internal const string TriggerPropertyName = "Trigger";
        internal const string ActionPropertyName = "Action";
        internal const string ToPropertyName = "To";

        public static readonly DependencyProperty IsTriggerSharedProperty =
            DependencyProperty.Register("IsTriggerShared",
            typeof(bool),
            typeof(TransitionDesigner),
            new FrameworkPropertyMetadata(false));

        public TransitionDesigner()
        {
            InitializeComponent();
            this.Loaded += (sender, e) =>
            {
                this.ModelItem.PropertyChanged += OnModelItemPropertyChanged;
                this.transitionsSharingTrigger.Clear();
                ModelItem parentStateModelItem = StateContainerEditor.GetParentStateModelItemForTransition(this.ModelItem);
                ModelItem triggerModelItem = this.ModelItem.Properties[TriggerPropertyName].Value;
                if (triggerModelItem != null)
                {
                    foreach (ModelItem transitionModelItem in parentStateModelItem.Properties[StateDesigner.TransitionsPropertyName].Collection)
                    {
                        if (transitionModelItem != this.ModelItem)
                        {
                            if (triggerModelItem == transitionModelItem.Properties[TriggerPropertyName].Value)
                            {
                                this.transitionsSharingTrigger.Add(transitionModelItem);
                            }
                        }
                    }
                }
                // Connectors starting from the same point should share the same trigger
                else
                {
                    PointCollection thisPointCollection = this.ViewStateService.RetrieveViewState(this.ModelItem, StateContainerEditor.ConnectorLocationViewStateKey) as PointCollection;
                    if (thisPointCollection != null && thisPointCollection.Count > 1)
                    {
                        foreach (ModelItem transitionModelItem in parentStateModelItem.Properties[StateDesigner.TransitionsPropertyName].Collection)
                        {
                            if (transitionModelItem != this.ModelItem)
                            {
                                PointCollection pointCollection = this.ViewStateService.RetrieveViewState(transitionModelItem, StateContainerEditor.ConnectorLocationViewStateKey) as PointCollection;
                                if (pointCollection != null && pointCollection.Count > 1)
                                {
                                    if (pointCollection[0].IsEqualTo(thisPointCollection[0]))
                                    {
                                        Debug.Assert(transitionModelItem.Properties[TriggerPropertyName].Value == null, "Transition trigger should be null.");
                                        this.transitionsSharingTrigger.Add(transitionModelItem);
                                    }
                                }
                            }
                        }

                    }
                }
                if (this.transitionsSharingTrigger.Count > 0)
                {
                    this.IsTriggerShared = true;
                }
            };
            this.Unloaded += (sender, e) =>
            {
                this.ModelItem.PropertyChanged -= OnModelItemPropertyChanged;
                this.transitionsSharingTrigger.Clear();
            };

      


        }

        public bool IsTriggerShared
        {
            get { return (bool)this.GetValue(IsTriggerSharedProperty); }
            set { this.SetValue(IsTriggerSharedProperty, value); }
        }

        void OnModelItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Update the Trigger property for all transitions that share the trigger
            if(e.PropertyName.Equals(TriggerPropertyName) && this.transitionsSharingTrigger.Count > 0)
            {
                foreach(ModelItem transitionModelItem in this.transitionsSharingTrigger)
                {
                    transitionModelItem.Properties[TriggerPropertyName].SetValue(this.ModelItem.Properties[TriggerPropertyName].Value);
                }
            }
        }
    }
}
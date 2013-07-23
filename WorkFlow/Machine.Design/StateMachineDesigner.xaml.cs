//----------------------------------------------------------------

//----------------------------------------------------------------

namespace Machine.Design
{
    using System.Activities.Presentation.Model;
    using System.Activities.Presentation.View;

   public  partial class StateMachineDesigner
    {
        const string ExpandViewStateKey = "IsExpanded";
        internal const string InitialStatePropertyName = "InitialState";
        internal const string VariablesPropertyName = "Variables";

        public StateMachineDesigner()
        {
            InitializeComponent();
        }

        protected override void OnModelItemChanged(object newItem)
        {
            ViewStateService viewStateService = this.Context.Services.GetService<ViewStateService>();
            if (viewStateService != null)
            {
                // Make StateMachine designer always collapsed by default, but only if the user didn't explicitly specify collapsed or expanded.
                bool? isExpanded = (bool?)viewStateService.RetrieveViewState((ModelItem)newItem, ExpandViewStateKey);
                if (isExpanded == null)
                {
                    viewStateService.StoreViewState((ModelItem)newItem, ExpandViewStateKey, false);
                }
            }
            base.OnModelItemChanged(newItem);
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var v = this.ModelItem.Properties["States"];

            System.Windows.MessageBox.Show(v.Value.ToString());

            dynamic list = v.Value ;
            list.Add(new State { DisplayName = "123" });

        }
    }
 }
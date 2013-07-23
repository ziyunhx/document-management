
namespace Machine.Design
{
    using System.Activities.Presentation.Model;
    using System.Activities.Presentation.View;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

  public  partial class InitialNode
    {
        public InitialNode()
        {
            InitializeComponent();
        }

        protected override void OnContextMenuOpening(ContextMenuEventArgs e)
        {
            e.Handled = true;
        }
    }
}

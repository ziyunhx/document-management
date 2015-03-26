using System.Windows.Controls;
using WorkFlow.Core;

namespace UserDesigner
{
    /// <summary>
    /// Interaction logic for nodeControl.xaml
    /// </summary>
    public partial class NodeControl : UserControl
    {
        public NodeControl()
        {
            InitializeComponent();
        }
        public NodeControl(WFNode node)
        {
            InitializeComponent();
            showFlowchar(node);
        }

        void showFlowchar(WFNode node)
        {
            this.Background = System.Windows.Media.Brushes.Red;
            Canvas.SetLeft(this, node.ShapeSize.x);
            Canvas.SetTop(this, node.ShapeSize.y);
            this.Width =node.ShapeSize.width;
            this.Height = node.ShapeSize.height;
            this.displayName.Text = node.DisplayName;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UserDesigner
{
    /// <summary>
    /// Interaction logic for nodeControl.xaml
    /// </summary>
    public partial class nodeControl : UserControl
    {
        public nodeControl()
        {
            InitializeComponent();
        }
        public nodeControl(WorkflowStruct.node node)
        {
            InitializeComponent();
            showFlowchar(node);
        }

        void showFlowchar(WorkflowStruct.node node)
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

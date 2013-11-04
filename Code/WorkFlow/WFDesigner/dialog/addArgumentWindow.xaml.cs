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
using System.Windows.Shapes;
using System.Activities.Presentation;
using System.Activities.Presentation.Services;
using System.Activities;
namespace WFDesigner.dialog
{
    /// <summary>
    /// Interaction logic for addArgumentWindow.xaml
    /// </summary>
    public partial class addArgumentWindow : Window
    {
        public addArgumentWindow()
        {
            InitializeComponent();
        }
        WorkflowDesigner designer;
        public addArgumentWindow(WorkflowDesigner designer)
        {
            InitializeComponent();
            this.designer = designer;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var v = designer.Context.Services.GetService<ModelService>().Root.Properties["Properties"].Collection;

            v.Add(new DynamicActivityProperty{ Name="wxdss",
                                               Type=typeof(InArgument<string>),
                                               Value=new InArgument<string>()
                                              });
           
        }
    }
}

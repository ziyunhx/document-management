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
using System.Activities.Core.Presentation;
using System.Activities.Presentation;
using System.Activities;
using Engine;

namespace WFDesigner.dialog
{
    /// <summary>
    /// Interaction logic for openChildWorkflowWindow.xaml
    /// </summary>
    public partial class openChildWorkflowWindow : Window
    {
        public openChildWorkflowWindow()
        {
            InitializeComponent();

            (new DesignerMetadata()).Register();

            this.DataContext = this;
        }

        WorkflowDesigner designer;

        void loadWorkflowFromFile(string workflowFilePathName)
        {

            desienerPanel.Content = null;
           
            designer = new WorkflowDesigner();

            try
            {
                designer.Load(workflowFilePathName);

                desienerPanel.Content = designer.View;

            }
            catch (SystemException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }  //end

        private void browserButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openDialog = new Microsoft.Win32.OpenFileDialog();
            if (openDialog.ShowDialog(this).Value)
            {
                loadWorkflowFromFile(openDialog.FileName);
             
            }
        }

        public  Activity activity = null;

        private void insertButton_Click(object sender, RoutedEventArgs e)
        {
            activity = tool.activityByXaml(designer.Text);
          
            this.Hide();
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            activity = null;
            this.Hide();
        }
    }
}

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
using Engine;

namespace WFDesigner.dialog
{
    /// <summary>
    /// Interaction logic for createWorkflowWindow.xaml
    /// </summary>
    public partial class createWorkflowWindow : Window
    {
        public createWorkflowWindow()
        {
            InitializeComponent();

            loadTemplate();
        }

        void loadTemplate()
        {
            templateListBox.Items.Add(@"template\activityBuilder.xaml");
            templateListBox.Items.Add(@"template\状态机.xaml");
            templateListBox.Items.Add(@"template\流程图.xaml");

            templateInfo.Text = tool.xamlFromFile(@"template\readme.txt");
        }

        public string templateName="";

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            if (button == null)
            {
                return;
            }

            string buttonValue = button.Content.ToString();

            switch (buttonValue)
            {
                case "取消":
                    templateName = "";
                    this.Visibility = System.Windows.Visibility.Hidden;
                    break;


                case "确定":
                    if(templateListBox.SelectedItem !=null)
                    {
                      templateName=templateListBox.SelectedItem.ToString();

                      this.Visibility = System.Windows.Visibility.Hidden;
                    }
                    else
                    {
                        MessageBox.Show("请选择模板");
                    }
                    break;

            }


        }

        private void templateListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (templateListBox.SelectedItem != null)
            {
             templateInfo.Text=   tool.xamlFromFile(templateListBox.SelectedItem.ToString() +".txt");
            }
        }



    }
}

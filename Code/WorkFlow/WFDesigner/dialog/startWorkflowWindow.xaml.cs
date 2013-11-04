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

namespace WFDesigner.dialog
{
    /// <summary>
    /// Interaction logic for startWorkflowWindow.xaml
    /// </summary>
    public partial class startWorkflowWindow : Window
    {
        public startWorkflowWindow()
        {
            InitializeComponent();
            argumentStartButton.IsEnabled = false;
        }
      
        public startWorkflowWindow(List<string> key)
        {
            InitializeComponent();
            this.key = key;
            loadUI();
        }

        void loadUI()
        {
            if (key == null)
            {
                argumentStartButton.IsEnabled = false;
                return;
               
            }

            foreach (string item in key)
            {
                TextBlock textBlock = new TextBlock() { Text = item };
                TextBox textBox = new TextBox() { Name = item, Width = 350, Height = 25 };
                body.Children.Add(textBlock);
                body.Children.Add(textBox);
            }
        }

        List<string> key;
        public string  selectButtonValue = "";
    public     System.Collections.Generic.Dictionary<string, object> dictionary = new Dictionary<string, object>();

        private void Button_Click(object sender, RoutedEventArgs e)
        {
             Button button = sender as Button;

            if (button == null)
            {
                return;
            }

            selectButtonValue = button.Content.ToString();

            switch (selectButtonValue)
            {
                case "参数启动":
                    foreach (var item in body.Children)
                    {
                        TextBox textBox = item as TextBox;
                        if (textBox != null)
                        {
                            dictionary.Add(textBox.Name, textBox.Text);
                        }

                    }

                    break;

                case "无参数启动":

                    break;

                case "取消":
                  
                    break;
            }
            this.Hide();
        }
    }
}

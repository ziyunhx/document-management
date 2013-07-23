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

namespace Engine
{
    /// <summary>
    /// Interaction logic for controlControl.xaml
    /// </summary>
    public partial class controlControl : UserControl
    {
        public controlControl()
        {
            InitializeComponent();


            if (s == null)
            {
                s = new StringBuilder();

                sw = new System.IO.StringWriter(s);

                System.Console.SetOut(sw);
            }

            this.Loaded += new RoutedEventHandler(controlControl_Loaded);
        }

        void controlControl_Loaded(object sender, RoutedEventArgs e)
        {
            infoTextBox.Text = value;
        }

        static System.IO.StringWriter sw;
        static StringBuilder s;

       public string value
       {
           get { return s.ToString(); }
       }
       public void clear()
       {
           s.Remove(0, s.Length);
       }

       private void refreshButton_Click(object sender, RoutedEventArgs e)
       {
           infoTextBox.Text = value;
       }

       private void clearButton_Click(object sender, RoutedEventArgs e)
       {
           clear();
       }
 
    }
}

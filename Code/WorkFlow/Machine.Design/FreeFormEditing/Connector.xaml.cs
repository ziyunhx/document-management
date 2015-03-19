//------------------------------------------------------------

//------------------------------------------------------------

namespace Machine.Design.FreeFormEditing
{
    using System;
    using System.Activities.Presentation.Model;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    partial class Connector : UserControl
    {
        //Label will be shown only if there is one segment in the connector whose length is greater than this.
        internal const int MinConnectorSegmentLengthForLabel = 30;
        
        public static readonly DependencyProperty PointsProperty = DependencyProperty.Register(
            "Points", 
            typeof(PointCollection), 
            typeof(Connector), 
            new FrameworkPropertyMetadata(new PointCollection()));
        
        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(
            "IsSelected", 
            typeof(bool), 
            typeof(Connector), 
            new FrameworkPropertyMetadata(false));
        
        public static readonly DependencyProperty LabelTextProperty = DependencyProperty.Register(
            "LabelText", 
            typeof(string), 
            typeof(Connector), 
            new FrameworkPropertyMetadata(null));

        public static readonly DependencyProperty IsTransitionProperty = DependencyProperty.Register(
            "IsTransition",
            typeof(bool),
            typeof(Connector),
            new FrameworkPropertyMetadata(false));
        
        public const double ArrowShapeWidth = 5;
        
        public Connector()
        {
            InitializeComponent();
        }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly",
            Justification = "PointCollection is a special WPF class and got special Clone logic, the setter of this property is used several places.")]
        public PointCollection Points
        {
            get { return (PointCollection)GetValue(Connector.PointsProperty); }
            set { SetValue(Connector.PointsProperty, value); }
        }

        public bool IsSelected
        {
            get { return (bool)GetValue(Connector.IsSelectedProperty); }
            set { SetValue(Connector.IsSelectedProperty, value); }
        }

        public string LabelText
        {
            get { return (string)GetValue(Connector.LabelTextProperty); }
            set { SetValue(Connector.LabelTextProperty, value); }
        }

        public bool IsTransition
        {
            get { return (bool)GetValue(Connector.IsTransitionProperty); }
            set { SetValue(Connector.IsTransitionProperty, value); }
        }
    }
}

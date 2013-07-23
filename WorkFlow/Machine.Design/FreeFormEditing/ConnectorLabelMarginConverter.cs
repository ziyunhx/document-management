//------------------------------------------------------------

//------------------------------------------------------------

namespace Machine.Design.FreeFormEditing
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Media;
    using System.Diagnostics.CodeAnalysis;

    [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification="The class is used in xaml.")]
    class ConnectorLabelMarginConverter : IMultiValueConverter
    {
        const double EPS = 1e-6;

        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification="The class is only used internally and not accessible externally.")]
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            Thickness margin = new Thickness(0);
            PointCollection connectorPoints = values[0] as PointCollection;
            double labelBorderWidth = (double)values[1];
            double labelBorderHeight = (double)values[2];
            if (connectorPoints != null)
            {
                int longestSegmentIndex;
                DesignerGeometryHelper.LongestSegmentLength(connectorPoints, out longestSegmentIndex);
                if (longestSegmentIndex >= 0)
                {
                    Point labelLocation = DesignerGeometryHelper.MidPointOfLineSegment(connectorPoints[longestSegmentIndex], connectorPoints[longestSegmentIndex + 1]);
                    labelLocation.X = (int)(labelLocation.X - labelBorderWidth / 2 + EPS);
                    labelLocation.Y = (int)(labelLocation.Y - labelBorderHeight / 2 + EPS);
                    margin.Top = labelLocation.Y;
                    margin.Left = labelLocation.X;
                }
            }
            return margin;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}

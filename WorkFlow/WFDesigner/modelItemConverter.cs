using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Activities.Presentation.Model;
using System.Activities;

namespace WFDesigner
{
    class modelItemConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            ModelItem mi = value as ModelItem;

            if (null != mi)
            {

                if (mi.ItemType.Equals(typeof(string)))
                {
                    return new List<object> { mi.GetCurrentValue() };
                }
                if (mi.ItemType.IsSubclassOf(typeof(Activity)) || mi.ItemType.IsSubclassOf(typeof(ActivityDelegate)))
                {
                    return new List<object> { mi };
                }
                return mi;
            }
            else
            {
                return value;
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

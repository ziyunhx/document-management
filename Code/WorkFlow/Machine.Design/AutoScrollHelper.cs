//----------------------------------------------------------------

//----------------------------------------------------------------

namespace Machine.Design
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;


    static class AutoScrollHelper
    {
        const int scrollBuffer = 30;
        const double scrollOnDragThresholdX = 25;
        const double scrollOnDragThresholdY = 25;
        const int scrollOnDragOffset = 1;

        public static void AutoScroll(MouseEventArgs e, DependencyObject element)
        {
            FrameworkElement logicalView = element as FrameworkElement;
            DependencyObject current = element;
            while (current != null)
            {
                current = VisualTreeHelper.GetParent(current);
                if (current != null && current.GetType() == typeof(ScrollViewer))
                {
                    break;
                }
            }
            ScrollViewer scrollViewer = current as ScrollViewer;
            if (scrollViewer != null)
            {
                AutoScroll(e.GetPosition(scrollViewer), scrollViewer, logicalView != null ? e.GetPosition(logicalView) : (Point?)null, logicalView,
                     scrollOnDragThresholdX, scrollOnDragThresholdY, scrollOnDragOffset);
            }
        }

        static void AutoScroll(Point positionInScrollViewer, ScrollViewer scrollViewer, Point? positionInLogicalView, FrameworkElement logicalView, double scrollOnDragThresholdX, double scrollOnDragThresholdY, int scrollOnDragOffset)
        {
            double scrollViewerWidth = scrollViewer.ActualWidth;
            double scrollViewerHeight = scrollViewer.ActualHeight;

            double logicalViewWidth = 0;
            double logicalViewHeight = 0;
            if (logicalView != null)
            {
                logicalViewWidth = logicalView.ActualWidth;
                logicalViewHeight = logicalView.ActualHeight;
            }

            int heightToScroll = 0;
            int widthToScroll = 0;

            if (positionInScrollViewer.X > (scrollViewerWidth - scrollOnDragThresholdX)
                && (positionInLogicalView == null
                   || positionInLogicalView.Value.X < (logicalViewWidth - scrollBuffer)))
            {
                widthToScroll = scrollOnDragOffset;
            }
            else if (positionInScrollViewer.X < scrollOnDragThresholdX
                && (positionInLogicalView == null
                   || positionInLogicalView.Value.X > scrollBuffer))
            {
                widthToScroll = -scrollOnDragOffset;
            }

            if (positionInScrollViewer.Y > (scrollViewerHeight - scrollOnDragThresholdY)
                && (positionInLogicalView == null
                    || positionInLogicalView.Value.Y < logicalViewHeight - scrollBuffer))
            {
                heightToScroll = scrollOnDragOffset;
            }
            else if (positionInScrollViewer.Y < scrollOnDragThresholdY
                && (positionInLogicalView == null
                   || positionInLogicalView.Value.Y > scrollBuffer))
            {
                heightToScroll = -scrollOnDragOffset;
            }

            if (widthToScroll != 0 || heightToScroll != 0)
            {
                scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset + heightToScroll);
                scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset + widthToScroll);
            }
        }
    }
}
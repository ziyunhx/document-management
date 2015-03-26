using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using WorkFlow.Core;

namespace UserDesigner
{
    /// <summary>
    /// Interaction logic for displayFlowcharControl.xaml
    /// </summary>
    public partial class displayFlowcharControl : UserControl
    {
        public displayFlowcharControl()
        {
            InitializeComponent();
        }

        public void showFlowchar(FlowcharStruct flowcharStruct)
        {
            body.Children.Clear();

            //(1)
            body.Children.Add(new NodeControl(flowcharStruct.beginNode));
            //(2)
            foreach (var nodeItem in flowcharStruct.nodeList)
            {
                body.Children.Add(new NodeControl(nodeItem));
            }

            //(3)
            foreach (var lineItem in flowcharStruct.lineList)
            {
                var v=lineItem.connectorPoint[0];
                lineItem.connectorPoint.RemoveAt(0);

                List<Point> ps=new List<Point>();
                foreach(var i in   lineItem.connectorPoint)
                {
                    ps.Add(new Point { X=i.x ,Y=i.y});
                }

                Path line = drawLines(new Point { X = v.x, Y = v.y }, ps);

                line.Stroke = new SolidColorBrush(Colors.Blue);
                line.StrokeThickness = 5;
                //line.StrokeStartLineCap = PenLineCap.Round;
                line.StrokeEndLineCap = PenLineCap.Triangle;
                line.StrokeLineJoin = PenLineJoin.Round;
                body.Children.Add(line);
            }
        }//end


         Path drawLines(Point startPoint, List<Point> points)
        {
            Path path = new Path();
            PathGeometry pathGeometry = new PathGeometry();
            PathFigure pathFigure = new PathFigure();
            pathFigure.StartPoint = startPoint;
            pathGeometry.Figures.Add(pathFigure);
            path.Data = pathGeometry;

            foreach (var point in points)
            {
                pathFigure.Segments.Add(new LineSegment(point, true));
            }

            return path;
        }
    }
}
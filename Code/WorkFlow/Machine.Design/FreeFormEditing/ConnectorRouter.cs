//------------------------------------------------------------

//------------------------------------------------------------

namespace Machine.Design.FreeFormEditing
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Controls;

    internal static class ConnectorRouter
    {
        const int connectorMargin = 30;

        [Flags]
        enum DesignerEdges
        {
            None = 0,
            Left = 1,
            Top = 2,
            Right = 4,
            Bottom = 8,
            All = 15
        }

        //This is used for link creation gesture to show the adorner.(In this case we know only the source connection point).
        internal static Point[] Route(FreeFormPanel panel, ConnectionPoint srcConnPoint, Point end)
        {
            Debug.Assert(panel.IsOutmostPanel(), "panel should be the outmost FreeFormPanel");
            return Route(panel, FreeFormPanel.GetLocationRelativeToOutmostPanel(srcConnPoint), end, FreeFormPanel.GetEdgeRelativeToOutmostPanel(srcConnPoint), null);
        }

        //This is used when we know both the source and destination connection points.
        internal static Point[] Route(FreeFormPanel panel, ConnectionPoint srcConnPoint , ConnectionPoint destConnPoint)
        {
            Debug.Assert(panel.IsOutmostPanel(), "panel should be the outmost FreeFormPanel");
            return Route(panel, FreeFormPanel.GetLocationRelativeToOutmostPanel(srcConnPoint),
                FreeFormPanel.GetLocationRelativeToOutmostPanel(destConnPoint), FreeFormPanel.GetEdgeRelativeToOutmostPanel(srcConnPoint), FreeFormPanel.GetEdgeRelativeToOutmostPanel(destConnPoint));
        }

        //This is used when we move the end connection point.
        internal static Point[] Route(FreeFormPanel panel, Point begin, Point end)
        {
            return Route(panel, begin, end, null, null);
        }

        static ConnectorSegment SrcEdge;
        static ConnectorSegment DestEdge;

        static void AddExcludedRects(FreeFormPanel outmostPanel, FreeFormPanel panel, Point begin, Point end, List<Rect> excludedRects)
        {
            foreach (UIElement child in panel.Children)
            {
                if (!(child is Connector))
                {
                    Thickness margin = new Thickness(0);
                    FrameworkElement frameworkChild = child as FrameworkElement;
                    if (frameworkChild != null)
                    {
                        margin = frameworkChild.Margin;
                    }
                    Size childSize = new Size(frameworkChild.DesiredSize.Width - margin.Left - margin.Right, frameworkChild.DesiredSize.Height - margin.Top - margin.Bottom);
                    Rect rect = new Rect(Point.Add(panel.TranslatePoint(FreeFormPanel.GetLocation(child), outmostPanel), new Vector(margin.Left, margin.Top)), childSize);
                    // We don't want to add containing rectangles to the exclusion list, otherwise the algorithm will fail to find a path
                    Rect shrunk = new Rect(rect.X + 0.1, rect.Y + 0.1, rect.Width - 0.2, rect.Height - 0.2);
                    if (!shrunk.Contains(begin) && !shrunk.Contains(end))
                    {
                        excludedRects.Add(rect);
                    }
                    StateDesigner stateDesigner = child as StateDesigner;
                    if (stateDesigner != null && stateDesigner.StateContainerEditor != null && stateDesigner.StateContainerEditor.Panel != null)
                    {
                        AddExcludedRects(outmostPanel, stateDesigner.StateContainerEditor.Panel, begin, end, excludedRects);
                    }
                }
            }
        }

        static Point[] Route(FreeFormPanel panel, Point begin, Point end, List<Point> srcEdge, List<Point> destEdge)
        {
            Point[] segments;
            if (panel == null)
            {
                throw new ArgumentNullException("panel");
            }
            List < Rect > excludedRects = new List < Rect >();
            List < Point > excludedLines = new List < Point >();
            foreach (UIElement child in panel.Children)
            {
                if (child.GetType() == typeof(Connector))
                {
                    Connector connector = (Connector)child;
                    for (int i = 0; i < connector.Points.Count - 1; i++)
                    {
                        excludedLines.Add(new Point(connector.Points[i].X, connector.Points[i].Y));
                        excludedLines.Add(new Point(connector.Points[i + 1].X, connector.Points[i + 1].Y));
                    }
                }
            }

            AddExcludedRects(panel, panel, begin, end, excludedRects);

            ConnectorRouter.SrcEdge = null;
            ConnectorRouter.DestEdge = null;
            if (srcEdge != null)
            {
                //ConnectorSegment should only be a segment from left to right or top to bottom.
                int smallerIndex = (srcEdge[0].X < srcEdge[1].X || srcEdge[0].Y < srcEdge[1].Y) ? 0 : 1;
                ConnectorRouter.SrcEdge = new ConnectorSegment(srcEdge[smallerIndex], srcEdge[1 - smallerIndex]);
            }
            if (destEdge != null)
            {
                int smallerIndex = (destEdge[0].X < destEdge[1].X || destEdge[0].Y < destEdge[1].Y) ? 0 : 1;
                ConnectorRouter.DestEdge = new ConnectorSegment(destEdge[smallerIndex], destEdge[1 - smallerIndex]);
            }

            segments = GetRoutedLineSegments(begin, end, new Size(connectorMargin, connectorMargin), excludedRects.ToArray(), excludedLines.ToArray());

            // If we failed to find a routed path, ignore all the lines and try again.
            if (!AreSegmentsValid(segments))
            {
                segments = GetRoutedLineSegments(begin, end, new Size(connectorMargin, connectorMargin), excludedRects.ToArray(), new Point[] { });
            }

            // If we still don't find a routed path, return the direct path.
            if (!AreSegmentsValid(segments))
            {
                double slope = DesignerGeometryHelper.SlopeOfLineSegment(begin, end);
                Point intermediatePoint = (slope < 1) ? new Point(end.X, begin.Y) : new Point(begin.X, end.Y);
                segments = new Point[] { begin, intermediatePoint, end };
            }
            segments = RemoveRedundantPoints(new List<Point>(segments));
            return segments;
        }

        //In a list of points specifying a connector, remove consecutive equivalent points.
        static Point[] RemoveRedundantPoints(List<Point> points)
        {
            for (int i = points.Count - 1; i > 0; i--)
            {
                if (points[i].IsEqualTo(points[i - 1]))
                {
                    points.RemoveAt(i);
                }
            }

            List<int> toRemove = new List<int>();
            int index1 = 0;
            int index2 = 1;
            int index3 = 2;
            while (index3 < points.Count)
            {
                if (points[index1].X.IsEqualTo(points[index3].X) ||
                    points[index1].Y.IsEqualTo(points[index3].Y))
                {
                    toRemove.Add(index2);
                }
                else
                {
                    index1 = index2;
                }
                ++index2;
                ++index3;
            }

            for (int i = points.Count - 1; i > 0; i--)
            {
                if (toRemove.Contains(i))
                {
                    points.RemoveAt(i);
                }
            }
            
            return points.ToArray();
        }


        static void AddBoundPoint(ref List < DistanceFromPoint > extremitiesList, Point p, ConnectorSegment segment, Point Z)
        {
            if (p.X != int.MinValue && p.X != int.MaxValue && p.Y != int.MinValue && p.Y != int.MaxValue)
            {
                extremitiesList.Add(new DistanceFromPoint(segment, Z, p));
            }
        }

        static bool AreSegmentsValid(Point[] segments)
        {
            if (segments == null || segments.Length == 0)
            {
                return false;
            }

            for (int i = 1; i < segments.Length; i++)
            {
                if (!segments[i - 1].X.IsEqualTo(segments[i].X) && !segments[i - 1].Y.IsEqualTo(segments[i].Y))
                {
                    return false;
                }
            }

            return true;
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification="This is a legacy algorithm.")]
        static Nullable < Point > EscapeAlgorithm(CoverSet coverSet, Point Z,
            ref List < Point > escapePointsA, ref List < ConnectorSegment > horizontalSegmentsA, ref List < ConnectorSegment > verticalSegmentsA, ref List < ConnectorSegment > horizontalSegmentsB, ref List < ConnectorSegment > verticalSegmentsB,
            ref Orientation orientationA, out ConnectorSegment intersectionSegmentA, out ConnectorSegment intersectionSegmentB, Size margin, ref bool noEscapeA)
        {
            Nullable < Point > intersection = null;
            intersectionSegmentA = null;
            intersectionSegmentB = null;

            ConnectorSegment leftCover = coverSet.GetCover(Z, DesignerEdges.Left);
            ConnectorSegment rightCover = coverSet.GetCover(Z, DesignerEdges.Right);
            ConnectorSegment bottomCover = coverSet.GetCover(Z, DesignerEdges.Bottom);
            ConnectorSegment topCover = coverSet.GetCover(Z, DesignerEdges.Top);

            ConnectorSegment h = ConnectorSegment.SegmentFromLeftToRightCover(coverSet, Z);
            // We do not want the routed line to coincide with the source or dest edge. 
            // Hence the edge should never be an escape line. 
            if (h.Overlaps(ConnectorRouter.SrcEdge) || h.Overlaps(ConnectorRouter.DestEdge))
            {
                h = null;
            }
            else
            {
                horizontalSegmentsA.Add(h);
            }

            ConnectorSegment v = ConnectorSegment.SegmentFromBottomToTopCover(coverSet, Z);
            if (v.Overlaps(ConnectorRouter.SrcEdge) || v.Overlaps(ConnectorRouter.DestEdge))
            {
                v = null;
            }
            else
            {
                verticalSegmentsA.Add(v);
            }


            // Check if the new escape line(s) intersect with the existing ones
            if (h != null)
            {
                for (int i = 0; i < verticalSegmentsB.Count; i++)
                {
                    ConnectorSegment segment = verticalSegmentsB[i];
                    intersection = h.Intersect(segment);
                    if (intersection != null)
                    {
                        intersectionSegmentA = h;
                        intersectionSegmentB = segment;
                        return intersection;
                    }
                }
            }

            if (v != null)
            {
                for (int i = 0; i < horizontalSegmentsB.Count; i++)
                {
                    ConnectorSegment segment = horizontalSegmentsB[i];
                    intersection = v.Intersect(segment);
                    if (intersection != null)
                    {
                        intersectionSegmentA = v;
                        intersectionSegmentB = segment;
                        return intersection;
                    }
                }
            }

            Nullable<Point> escapePoint = null;
            if (v != null)
            {
                escapePoint = EscapeProcessI(coverSet, Z, v, Orientation.Horizontal, margin);
                if (escapePoint != null)
                {
                    orientationA = Orientation.Vertical;
                    escapePointsA.Add((Point)escapePoint);
                    return null;
                }
            }

            if (h != null)
            {
                escapePoint = EscapeProcessI(coverSet, Z, h, Orientation.Vertical, margin);
                if (escapePoint != null)
                {
                    orientationA = Orientation.Horizontal;
                    escapePointsA.Add((Point)escapePoint);
                    return null;
                }
            }

            bool intersectionFlag = false;
            
            // Flags indicating if we can still continue in the given directions
            bool continue1, continue2, continue3, continue4;
            
            Point r1 = new Point(), r2 = new Point(), r3 = new Point(), r4 = new Point();

            if (topCover != null)
            {
                r1 = new Point(Z.X, topCover.A.Y);
            }
            if (rightCover != null)
            {
                r2 = new Point(rightCover.A.X, Z.Y);
            }
            if (bottomCover != null)
            {
                r3 = new Point(Z.X, bottomCover.A.Y);
            }
            if (leftCover != null)
            {
                r4 = new Point(leftCover.A.X, Z.Y);
            }
            do
            {
                continue1 = continue2 = continue3 = continue4 = false;
                if (topCover != null && v != null)
                {
                    r1.Y -= margin.Height;
                    if (r1.Y > Z.Y)
                    {
                        continue1 = true;
                        Nullable < Point > escape = EscapeProcessII(coverSet, Orientation.Vertical,
                            ref escapePointsA, ref horizontalSegmentsA, ref verticalSegmentsA, ref horizontalSegmentsB, ref verticalSegmentsB, r1, margin, out intersectionFlag, out intersectionSegmentA, out intersectionSegmentB);
                        if (escape != null)
                        {
                            verticalSegmentsA.Add(v);
                            if (intersectionFlag)
                            {
                                return escape;
                            }

                            orientationA = Orientation.Horizontal;
                            coverSet.AddUsedEscapeLine(new ConnectorSegment(Z, r1));
                            coverSet.AddUsedEscapeLine(new ConnectorSegment(r1, (Point)escape));
                            escapePointsA.Add((Point)escape);
                            return null;
                        }
                    }
                }

                if (rightCover != null && h != null)
                {
                    r2.X -= margin.Width;
                    if (r2.X > Z.X)
                    {
                        continue2 = true;
                        Nullable < Point > escape = EscapeProcessII(coverSet, Orientation.Horizontal,
                            ref escapePointsA, ref horizontalSegmentsA, ref verticalSegmentsA, ref horizontalSegmentsB, ref verticalSegmentsB, r2, margin, out intersectionFlag, out intersectionSegmentA, out intersectionSegmentB);
                        if (escape != null)
                        {
                            horizontalSegmentsA.Add(h);
                            if (intersectionFlag)
                            {
                                return escape;
                            }

                            orientationA = Orientation.Vertical;
                            coverSet.AddUsedEscapeLine(new ConnectorSegment(Z, r2));
                            coverSet.AddUsedEscapeLine(new ConnectorSegment(r2, (Point)escape));
                            escapePointsA.Add((Point)escape);
                            return null;
                        }
                    }
                }

                if (bottomCover != null && v != null)
                {
                    r3.Y += margin.Height;
                    if (r3.Y < Z.Y)
                    {
                        continue3 = true;
                        Nullable < Point > escape = EscapeProcessII(coverSet, Orientation.Vertical,
                            ref escapePointsA, ref horizontalSegmentsA, ref verticalSegmentsA, ref horizontalSegmentsB, ref verticalSegmentsB, r3, margin, out intersectionFlag, out intersectionSegmentA, out intersectionSegmentB);
                        if (escape != null)
                        {
                            verticalSegmentsA.Add(v);
                            if (intersectionFlag)
                            {
                                return escape;
                            }

                            orientationA = Orientation.Horizontal;
                            coverSet.AddUsedEscapeLine(new ConnectorSegment(Z, r3));
                            coverSet.AddUsedEscapeLine(new ConnectorSegment(r3, (Point)escape));
                            escapePointsA.Add((Point)escape);
                            return null;
                        }
                    }
                }

                if (leftCover != null && h!= null)
                {
                    r4.X += margin.Width;
                    if (r4.X < Z.X)
                    {
                        continue4 = true;
                        Nullable < Point > escape = EscapeProcessII(coverSet, Orientation.Horizontal,
                            ref escapePointsA, ref horizontalSegmentsA, ref verticalSegmentsA, ref horizontalSegmentsB, ref verticalSegmentsB, r4, margin, out intersectionFlag, out intersectionSegmentA, out intersectionSegmentB);
                        if (escape != null)
                        {
                            horizontalSegmentsA.Add(h);
                            if (intersectionFlag)
                            {
                                return escape;
                            }

                            orientationA = Orientation.Vertical;
                            coverSet.AddUsedEscapeLine(new ConnectorSegment(Z, r4));
                            coverSet.AddUsedEscapeLine(new ConnectorSegment(r4, (Point)escape));
                            escapePointsA.Add((Point)escape);
                            return null;
                        }
                    }
                }
            } while (continue1 || continue2 || continue3 || continue4);

            noEscapeA = true;
            return null;
        }

        static Nullable < Point > EscapeProcessI(CoverSet coverSet, Point Z,
            ConnectorSegment escapeLine, Orientation orientation, Size margin)
        {
            List < DistanceFromPoint > extremitiesList = new List < DistanceFromPoint >(4);

            ConnectorSegment lesserCover = coverSet.GetCover(Z, (orientation == Orientation.Horizontal) ? DesignerEdges.Left : DesignerEdges.Bottom);
            if (lesserCover != null)
            {
                AddBoundPoint(ref extremitiesList, lesserCover.A, lesserCover, Z);
                AddBoundPoint(ref extremitiesList, lesserCover.B, lesserCover, Z);
            }

            ConnectorSegment higherCover = coverSet.GetCover(Z, (orientation == Orientation.Horizontal) ? DesignerEdges.Right : DesignerEdges.Top);
            if (higherCover != null)
            {
                AddBoundPoint(ref extremitiesList, higherCover.A, higherCover, Z);
                AddBoundPoint(ref extremitiesList, higherCover.B, higherCover, Z);
            }

            if (extremitiesList.Count == 0)
            {
                return null;
            }

            DistanceSorter.Sort(ref extremitiesList);
            for (int i = 0; i < extremitiesList.Count; i++)
            {
                Point p = extremitiesList[i].P;
                Point direction = new Point(Math.Sign(p.X - Z.X), Math.Sign(p.Y - Z.Y));
                if (((orientation == Orientation.Vertical) ? direction.X : direction.Y).IsEqualTo(0))
                {
                    ConnectorSegment segment = extremitiesList[i].ConnectorSegment;
                    p = segment.ExtendPointOutwards(p);
                    direction = new Point(Math.Sign(p.X - Z.X), Math.Sign(p.Y - Z.Y));
                    p = extremitiesList[i].P;
                }

                DesignerEdges side;
                if (orientation == Orientation.Vertical)
                {
                    side = (direction.Y < 0) ? DesignerEdges.Bottom : DesignerEdges.Top;
                }
                else
                {
                    side = (direction.X < 0) ? DesignerEdges.Left : DesignerEdges.Right;
                }

                Point escapePoint;
                if ((orientation == Orientation.Vertical))
                {
                    escapePoint = new Point(p.X + direction.X * margin.Width, Z.Y);
                }
                else
                {
                    escapePoint = new Point(Z.X, p.Y + direction.Y * margin.Height);
                }

                ConnectorSegment newEscapeLine = new ConnectorSegment(Z, escapePoint);
                if (!coverSet.EscapeLineHasBeenUsed(escapePoint) &&
                    escapeLine.IsPointOnSegment(escapePoint) && !escapeLine.A.IsEqualTo(escapePoint) && !escapeLine.B.IsEqualTo(escapePoint) &&
                    coverSet.IsEscapePoint(Z, escapePoint, side))
                {
                    coverSet.AddUsedEscapeLine(newEscapeLine);
                    return escapePoint;
                }
            }

            return null;
        }

        static Nullable < Point > EscapeProcessII(CoverSet coverSet, Orientation orientation, ref List < Point > escapePointsA,
            ref List < ConnectorSegment > horizontalSegmentsA, ref List < ConnectorSegment > verticalSegmentsA, ref List < ConnectorSegment > horizontalSegmentsB, ref List < ConnectorSegment > verticalSegmentsB,
            Point R, Size margin, out bool intersectionFlag, out ConnectorSegment intersectionSegmentA, out ConnectorSegment intersectionSegmentB)
        {
            intersectionFlag = false;
            intersectionSegmentA = null;
            intersectionSegmentB = null;

            ConnectorSegment h = ConnectorSegment.SegmentFromLeftToRightCover(coverSet, R);
            ConnectorSegment v = ConnectorSegment.SegmentFromBottomToTopCover(coverSet, R);

            for (int i = 0; i < verticalSegmentsB.Count; i++)
            {
                ConnectorSegment segment = verticalSegmentsB[i];
                Nullable < Point > intersection = h.Intersect(segment);
                if (intersection != null)
                {
                    intersectionFlag = true;
                    intersectionSegmentA = h;
                    intersectionSegmentB = segment;
                    escapePointsA.Add(R);
                    return intersection;
                }
            }
            for (int i = 0; i < horizontalSegmentsB.Count; i++)
            {
                ConnectorSegment segment = horizontalSegmentsB[i];
                Nullable < Point > intersection = v.Intersect(segment);
                if (intersection != null)
                {
                    intersectionFlag = true;
                    intersectionSegmentA = v;
                    intersectionSegmentB = segment;
                    escapePointsA.Add(R);
                    return intersection;
                }
            }

            Nullable < Point > escapePointI = null;

            if (orientation == Orientation.Horizontal)
            {
                escapePointI = EscapeProcessI(coverSet, R, v, Orientation.Horizontal, margin);
                if (escapePointI != null)
                {
                    verticalSegmentsA.Add(v);
                    escapePointsA.Add(R);
                    return escapePointI;
                }

                escapePointI = EscapeProcessI(coverSet, R, h, Orientation.Vertical, margin);
                if (escapePointI != null)
                {
                    horizontalSegmentsA.Add(h);
                    escapePointsA.Add(R);
                    return escapePointI;
                }
            }
            else
            {
                escapePointI = EscapeProcessI(coverSet, R, h, Orientation.Vertical, margin);
                if (escapePointI != null)
                {
                    horizontalSegmentsA.Add(h);
                    escapePointsA.Add(R);
                    return escapePointI;
                }

                escapePointI = EscapeProcessI(coverSet, R, v, Orientation.Horizontal, margin);
                if (escapePointI != null)
                {
                    verticalSegmentsA.Add(v);
                    escapePointsA.Add(R);
                    return escapePointI;
                }
            }

            return null;
        }

        static List < Point > FirstRefinementAlgorithm(List < Point > points, ConnectorSegment intersectionSegment)
        {
            List < Point > refinedSet = new List < Point >();
            ConnectorSegment k = intersectionSegment;

            while (points.Count > 0)
            {
                Point point;
                int i = points.Count - 1;

                while (!k.PointLiesOnThisLine(points[i]) && i > 0)
                {
                    i--;
                }

                while (i > 0 && k.PointLiesOnThisLine(points[i - 1]))
                {
                    i--;
                }
                point = points[i];
                refinedSet.Add(point);

                while (points.Count > i)
                {
                    points.RemoveAt(i);
                }

                k = k.PerpendicularThroughPoint(point);
            }

            return refinedSet;
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "Catch all exceptions to prevent crash.")]
        [SuppressMessage("Reliability", "Reliability108:IsFatalRule",
            Justification = "Catch all exceptions to prevent crash.")]
        static Point[] GetRoutedLineSegments(Point begin, Point end, Size margin, Rect[] rectanglesToExclude, Point[] linesToExclude)
        {
            if (rectanglesToExclude == null)
            {
                throw new ArgumentNullException("rectanglesToExclude");
            }

            if (linesToExclude == null)
            {
                throw new ArgumentNullException("linesToExclude");
            }

            if ((linesToExclude.Length % 2) > 0)
            {
                throw new ArgumentException("Error");
            }


            CoverSet coverSet = new CoverSet(rectanglesToExclude, linesToExclude);
            coverSet.ClearUsedLines();

            Point A = begin;
            Point B = end;

            //escape points
            List < Point > escapePointsA = new List < Point >(); //escape points from begin to end
            List < Point > escapePointsB = new List < Point >(); //escape points from end to begin

            //horizontal/vertical escape segments from A
            List < ConnectorSegment > horizontalEscapeSegmentsA = new List < ConnectorSegment >();
            List < ConnectorSegment > verticalEscapeSegmentsA = new List < ConnectorSegment >();

            //horizontal/vertical escape segments from B
            List < ConnectorSegment > horizontalEscapeSegmentsB = new List < ConnectorSegment >();
            List < ConnectorSegment > verticalEscapeSegmentsB = new List < ConnectorSegment >();

            Orientation orientationA = Orientation.Horizontal;
            Orientation orientationB = Orientation.Horizontal;

            escapePointsA.Add(begin);
            escapePointsB.Add(end);

            bool noEscapeA = false;
            bool noEscapeB = false;

            Nullable < Point > intersection = null;
            ConnectorSegment intersectionSegmentA = null;
            ConnectorSegment intersectionSegmentB = null;

            try
            {
                do
                {
                    if (noEscapeA)
                    {
                        if (noEscapeB)
                        {
                            break;
                        }
                        else
                        {
                            List < Point > tempList = escapePointsA;
                            escapePointsA = escapePointsB;
                            escapePointsB = tempList;

                            Point tempPoint = A;
                            A = B;
                            B = tempPoint;

                            bool tempBool = noEscapeA;
                            noEscapeA = noEscapeB;
                            noEscapeB = tempBool;

                            Orientation tempOrientation = orientationA;
                            orientationA = orientationB;
                            orientationB = tempOrientation;

                            List < ConnectorSegment > tempListSegm = horizontalEscapeSegmentsA;
                            horizontalEscapeSegmentsA = horizontalEscapeSegmentsB;
                            horizontalEscapeSegmentsB = tempListSegm;

                            tempListSegm = verticalEscapeSegmentsA;
                            verticalEscapeSegmentsA = verticalEscapeSegmentsB;
                            verticalEscapeSegmentsB = tempListSegm;

                            continue;
                        }
                    }

                    Point objectPoint = escapePointsA[escapePointsA.Count - 1];

                    intersection = EscapeAlgorithm(coverSet, objectPoint,
                        ref escapePointsA, ref horizontalEscapeSegmentsA, ref verticalEscapeSegmentsA, ref horizontalEscapeSegmentsB, ref verticalEscapeSegmentsB, ref orientationA,
                        out intersectionSegmentA, out intersectionSegmentB, margin, ref noEscapeA);
                    if (intersection != null)
                    {
                        break;
                    }
                    else
                    {
                        List < Point > tempList = escapePointsA;
                        escapePointsA = escapePointsB;
                        escapePointsB = tempList;

                        Point tempPoint = A;
                        A = B;
                        B = tempPoint;

                        bool tempBool = noEscapeA;
                        noEscapeA = noEscapeB;
                        noEscapeB = tempBool;

                        Orientation tempOrientation = orientationA;
                        orientationA = orientationB;
                        orientationB = tempOrientation;

                        List < ConnectorSegment > tempListSegm = horizontalEscapeSegmentsA;
                        horizontalEscapeSegmentsA = horizontalEscapeSegmentsB;
                        horizontalEscapeSegmentsB = tempListSegm;

                        tempListSegm = verticalEscapeSegmentsA;
                        verticalEscapeSegmentsA = verticalEscapeSegmentsB;
                        verticalEscapeSegmentsB = tempListSegm;
                    }

                } while (true);

                if (intersection == null)
                {
                    return null;
                }

                List < Point > refinedPath = new List < Point >();

                escapePointsA = FirstRefinementAlgorithm(escapePointsA, intersectionSegmentA);
                escapePointsB = FirstRefinementAlgorithm(escapePointsB, intersectionSegmentB);

                for (int j = escapePointsA.Count - 1; j >= 0; j--)
                {
                    refinedPath.Add(escapePointsA[j]);
                }
                refinedPath.Add((Point)intersection);
                for (int j = 0; j < escapePointsB.Count; j++)
                {
                    refinedPath.Add(escapePointsB[j]);
                }

                SecondRefinementAlgorithm(coverSet, ref refinedPath, margin);

                if (refinedPath.Count > 1 && refinedPath[refinedPath.Count - 1].IsEqualTo(begin))
                {
                    refinedPath.Reverse();
                }

                return refinedPath.ToArray();
            }
            catch (Exception)
            {
                return null;
            }
        }

        static void SecondRefinementAlgorithm(CoverSet coverSet, ref List < Point > refinedPath, Size margin)
        {
            List < Point > newPath = new List < Point >();

            int currentSegment = 0;
            while (currentSegment < refinedPath.Count - 1)
            {
                Point a1 = refinedPath[currentSegment];
                Point a2 = refinedPath[currentSegment + 1];

                ConnectorSegment a = ConnectorSegment.ConstructBoundSegment(coverSet, a1, a2);

                int intersectingSegment = currentSegment + 2;
                while (intersectingSegment < refinedPath.Count - 1)
                {
                    Point b1 = refinedPath[intersectingSegment];
                    Point b2 = refinedPath[intersectingSegment + 1];
                    ConnectorSegment b = ConnectorSegment.ConstructBoundSegment(coverSet, b1, b2);

                    Nullable < Point > intersection = a.Intersect(b);
                    if (intersection != null)
                    {
                        newPath.Clear();
                        for (int i = 0; i <= currentSegment; i++)
                        {
                            newPath.Add(refinedPath[i]);
                        }
                        newPath.Add((Point)intersection);
                        for (int i = intersectingSegment + 1; i < refinedPath.Count; i++)
                        {
                            newPath.Add(refinedPath[i]);
                        }

                        List < Point > temp = refinedPath;
                        refinedPath = newPath;
                        newPath = temp;
                        newPath.Clear();

                        intersectingSegment = currentSegment + 2;
                    }
                    else
                    {
                        intersectingSegment++;
                    }
                }

                currentSegment++;
            }

            currentSegment = 0;
            while (currentSegment < refinedPath.Count - 1)
            {
                Point a1 = refinedPath[currentSegment];
                Point a2 = refinedPath[currentSegment + 1];

                bool intersected = false;
                ConnectorSegment a = ConnectorSegment.ConstructBoundSegment(coverSet, a1, a2);
                if (a != null)
                {
                    Point direction = new Point(a2.X - a1.X, a2.Y - a1.Y);

                    int steps = (int)Math.Max(Math.Abs(direction.X / margin.Width), Math.Abs(direction.Y / margin.Height)); //one of the values will be null
                    direction.X = (int)Math.Sign(direction.X);
                    direction.Y = (int)Math.Sign(direction.Y);

                    for (int i = 1; i <= steps; i++)
                    {
                        Point k = new Point(a1.X + i * margin.Width * direction.X, a1.Y + i * margin.Height * direction.Y);
                        if (k.IsEqualTo(a2))
                        {
                            break;
                        }

                        ConnectorSegment b = ConnectorSegment.ConstructBoundSegment(coverSet, k, (a.Orientation == Orientation.Horizontal) ? Orientation.Vertical : Orientation.Horizontal);
                        int intersectingSegment = currentSegment + 2;
                        while (intersectingSegment < refinedPath.Count - 1 && !intersected)
                        {
                            Point c1 = refinedPath[intersectingSegment];
                            Point c2 = refinedPath[intersectingSegment + 1];
                            ConnectorSegment c = new ConnectorSegment(c1, c2);

                            Nullable < Point > intersection = b.Intersect(c);
                            if (intersection != null && c.IsPointOnSegment((Point)intersection))
                            {
                                intersected = true;

                                newPath.Clear();
                                for (int j = 0; j <= currentSegment; j++)
                                {
                                    newPath.Add(refinedPath[j]);
                                }
                                newPath.Add(k);
                                newPath.Add((Point)intersection);
                                for (int j = intersectingSegment + 1; j < refinedPath.Count; j++)
                                {
                                    newPath.Add(refinedPath[j]);
                                }
                                List < Point > temp = refinedPath;
                                refinedPath = newPath;
                                newPath = temp;
                                newPath.Clear();
                                break;
                            }

                            intersectingSegment++;
                        }

                        if (intersected)
                        {
                            break;
                        }
                    }
                }

                if (!intersected)
                {
                    currentSegment++;
                }
            }
        }

        struct DistanceFromPoint
        {
            public ConnectorSegment ConnectorSegment;
            public double Distance;
            public Point P;

            public DistanceFromPoint(ConnectorSegment segment, Point z, Point p)
            {
                this.ConnectorSegment = segment;
                this.P = p;
                this.Distance = DesignerGeometryHelper.DistanceBetweenPoints(z, p);
            }
        }

        // Represents a segment - the main entity in the routing algorithm
        sealed class ConnectorSegment
        {
            Orientation orientation;
            Point point1;
            Point point2;

            public ConnectorSegment(Point point1, Point point2)
            {
                if (!point1.X.IsEqualTo(point2.X) && !point1.Y.IsEqualTo(point2.Y))
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, 
                        SR.CannotConstructConnectionSegment, point1.ToString(), point2.ToString()));
                }

                this.point1 = point1;
                this.point2 = point2;
                this.orientation = (this.point1.X.IsEqualTo(this.point2.X) ? Orientation.Vertical : Orientation.Horizontal);
            }

            public Point A
            {
                get
                {
                    return this.point1;
                }
            }

            public Point B
            {
                get
                {
                    return this.point2;
                }
            }

            public Orientation Orientation
            {
                get
                {
                    return this.orientation;
                }
            }

            public static ConnectorSegment ConstructBoundSegment(CoverSet coverSet, Point a, Point b)
            {
                if (!a.X.IsEqualTo(b.X) && !a.Y.IsEqualTo(b.Y))
                {
                    return null;
                }

                return ConstructBoundSegment(coverSet, a, a.X.IsEqualTo(b.X) ? Orientation.Vertical : Orientation.Horizontal);
            }

            public static ConnectorSegment ConstructBoundSegment(CoverSet coverSet, Point a, Orientation orientation)
            {
                return (orientation == Orientation.Horizontal) ? SegmentFromLeftToRightCover(coverSet, a) : SegmentFromBottomToTopCover(coverSet, a);
            }

            public static ConnectorSegment SegmentFromBottomToTopCover(CoverSet coverSet, Point p)
            {
                ConnectorSegment bottomCover = coverSet.GetCover(p, DesignerEdges.Bottom);
                ConnectorSegment topCover = coverSet.GetCover(p, DesignerEdges.Top);

                //construct vertical escape segment
                Point bottom = new Point(p.X, (bottomCover != null) ? bottomCover.A.Y : int.MinValue);
                Point top = new Point(p.X, (topCover != null) ? topCover.A.Y : int.MaxValue);
                ConnectorSegment v = new ConnectorSegment(bottom, top);
                return v;
            }

            public static ConnectorSegment SegmentFromLeftToRightCover(CoverSet coverSet, Point p)
            {
                ConnectorSegment leftCover = coverSet.GetCover(p, DesignerEdges.Left);
                ConnectorSegment rightCover = coverSet.GetCover(p, DesignerEdges.Right);

                //construct horizontal escape segment
                Point left = new Point((leftCover != null) ? leftCover.A.X : int.MinValue, p.Y);
                Point right = new Point((rightCover != null) ? rightCover.A.X : int.MaxValue, p.Y);
                ConnectorSegment h = new ConnectorSegment(left, right);
                return h;
            }

            public bool Covers(Point p)
            {
                return (this.orientation == Orientation.Horizontal) ? 
                    (p.X.IsNoLessThan(Math.Min(this.point1.X, this.point2.X)) && p.X.IsNoGreaterThan(Math.Max(this.point1.X, this.point2.X))) : 
                    (p.Y.IsNoLessThan(Math.Min(this.point1.Y, this.point2.Y)) && p.Y.IsNoGreaterThan(Math.Max(this.point1.Y, this.point2.Y)));
            }

            public override bool Equals(object obj)
            {
                ConnectorSegment segment = obj as ConnectorSegment;
                if (segment == null)
                {
                    return false;
                }
                return (this.point1.IsEqualTo(segment.A) && this.point2.IsEqualTo(segment.B) && Orientation == segment.Orientation);
            }

            public bool Overlaps(ConnectorSegment segment)
            {
                if (segment == null)
                {
                    return false;
                }
                if(this.Orientation == segment.Orientation)
                {
                    return this.IsPointOnSegment(segment.point1) || this.IsPointOnSegment(segment.point2) || segment.IsPointOnSegment(this.point1) || segment.IsPointOnSegment(this.point2);
                }
                return false;
            }

            public Point ExtendPointOutwards(Point p)
            {
                if (!p.IsEqualTo(this.point1) && !p.IsEqualTo(this.point2))
                {
                    return p;
                }

                int k = (int)((this.orientation == Orientation.Horizontal) ? p.X : p.Y);
                int k1 = (int)((this.orientation == Orientation.Horizontal) ? this.point1.X : this.point1.Y);
                int k2 = (int)((this.orientation == Orientation.Horizontal) ? this.point2.X : this.point2.Y);

                if (k == Math.Min(k1, k2))
                {
                    k--;
                }
                else
                {
                    k++;
                }

                return new Point((this.orientation == Orientation.Horizontal) ? k : p.X, (this.orientation == Orientation.Horizontal) ? p.Y : k);
            }

            public override int GetHashCode()
            {
                return this.point1.GetHashCode() ^ this.point2.GetHashCode() ^ Orientation.GetHashCode();
            }

            public Nullable < Point > Intersect(ConnectorSegment segment)
            {
                if (this.orientation == segment.Orientation)
                {
                    return null;
                }

                ConnectorSegment vertical = (this.orientation == Orientation.Vertical) ? this : segment;
                ConnectorSegment horizontal = (this.orientation == Orientation.Vertical) ? segment : this;

                if (vertical.A.X < Math.Min(horizontal.A.X, horizontal.B.X) || vertical.A.X > Math.Max(horizontal.A.X, horizontal.B.X))
                {
                    return null;
                }

                if (horizontal.A.Y < Math.Min(vertical.A.Y, vertical.B.Y) || horizontal.A.Y > Math.Max(vertical.A.Y, vertical.B.Y))
                {
                    return null;
                }

                return new Point(vertical.A.X, horizontal.A.Y);
            }

            public bool IsPointOnSegment(Point p)
            {
                if ((this.orientation == Orientation.Horizontal && !p.Y.IsEqualTo(this.point1.Y)) || (this.orientation == Orientation.Vertical && !p.X.IsEqualTo(this.point1.X)))
                {
                    return false;
                }

                double k = (this.orientation == Orientation.Horizontal) ? p.X : p.Y;
                double k1 = (this.orientation == Orientation.Horizontal) ? this.point1.X : this.point1.Y;
                double k2 = (this.orientation == Orientation.Horizontal) ? this.point2.X : this.point2.Y;
                return k.IsNoLessThan(Math.Min(k1, k2)) && k.IsNoGreaterThan(Math.Max(k1, k2));
            }

            public ConnectorSegment PerpendicularThroughPoint(Point p)
            {
                Orientation newOrientation = (this.orientation == Orientation.Horizontal) ? Orientation.Vertical : Orientation.Horizontal;
                Point newPoint = new Point(p.X, p.Y);
                if (newOrientation == Orientation.Horizontal)
                {
                    newPoint.X = int.MaxValue;
                }
                else
                {
                    newPoint.Y = int.MaxValue;
                }

                return new ConnectorSegment(p, newPoint);
            }

            // We consider the whole line to which this segment belongs for this test
            public bool PointLiesOnThisLine(Point p)
            {
                return (this.orientation == Orientation.Horizontal) ? p.Y.IsEqualTo(this.point1.Y) : p.X.IsEqualTo(this.point1.X);
            }
        }

        sealed class CoverSet
        {
            List < ConnectorSegment > horizontalCovers = new List < ConnectorSegment >();
            List < ConnectorSegment > usedEscapeLine = new List < ConnectorSegment >();
            List < ConnectorSegment > verticalCovers = new List < ConnectorSegment >();

            public CoverSet(Rect[] rectanglesToExclude, Point[] linesToExclude)
            {
                foreach (Rect rectangle in rectanglesToExclude)
                {
                    AddCover(new ConnectorSegment(new Point(rectangle.Left, rectangle.Top), new Point(rectangle.Left, rectangle.Bottom)));
                    AddCover(new ConnectorSegment(new Point(rectangle.Right, rectangle.Top), new Point(rectangle.Right, rectangle.Bottom)));
                    AddCover(new ConnectorSegment(new Point(rectangle.Left, rectangle.Top), new Point(rectangle.Right, rectangle.Top)));
                    AddCover(new ConnectorSegment(new Point(rectangle.Left, rectangle.Bottom), new Point(rectangle.Right, rectangle.Bottom)));
                }

                for (int i = 0; i < linesToExclude.Length / 2; i++)
                {
                    AddCover(new ConnectorSegment(linesToExclude[i * 2], linesToExclude[(i * 2) + 1]));
                }
            }

            public void AddCover(ConnectorSegment cover)
            {
                List < ConnectorSegment > covers = (cover.Orientation == Orientation.Vertical) ? this.verticalCovers : this.horizontalCovers;

                for (int i = 0; i < covers.Count; i++)
                {
                    ConnectorSegment existingCover = covers[i];
                    if (cover.IsPointOnSegment(existingCover.A) && cover.IsPointOnSegment(existingCover.B))
                    {
                        covers.RemoveAt(i);
                        break;
                    }
                    else if (existingCover.IsPointOnSegment(cover.A) && existingCover.IsPointOnSegment(cover.B))
                    {
                        return;
                    }
                }

                covers.Add(cover);
            }


            public void AddUsedEscapeLine(ConnectorSegment segment)
            {
                this.usedEscapeLine.Add(segment);
            }

            public void ClearUsedLines()
            {
                this.usedEscapeLine.Clear();
            }

            public bool EscapeLineHasBeenUsed(Point escapePoint)
            {
                for (int i = 0; i < this.usedEscapeLine.Count; i++)
                {
                    ConnectorSegment usedSegment = this.usedEscapeLine[i];
                    if (usedSegment.IsPointOnSegment(escapePoint))
                    {
                        return true;
                    }
                }
                return false;
            }

            // Gets the cover on the given side (closest cover to the given side) for the point out of all stored segments
            public ConnectorSegment GetCover(Point p, DesignerEdges side)
            {
                ConnectorSegment cover = null;
                int distance = 0;

                if (side == DesignerEdges.Left || side == DesignerEdges.Right)
                {
                    for (int i = 0; i < this.verticalCovers.Count; i++)
                    {
                        ConnectorSegment segment = this.verticalCovers[i];
                        int currentDistance = (int)((side == DesignerEdges.Left) ? p.X - segment.A.X : segment.A.X - p.X);
                        if (currentDistance > 0 && segment.Covers(p))
                        {
                            if (cover == null || distance > currentDistance)
                            {
                                cover = segment;
                                distance = currentDistance;
                            }
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < this.horizontalCovers.Count; i++)
                    {
                        ConnectorSegment segment = this.horizontalCovers[i];
                        int currentDistance = (int)((side == DesignerEdges.Bottom) ? p.Y - segment.A.Y : segment.A.Y - p.Y);
                        if (currentDistance > 0 && segment.Covers(p))
                        {
                            if (cover == null || distance > currentDistance)
                            {
                                cover = segment;
                                distance = currentDistance;
                            }
                        }
                    }
                }

                return cover;
            }

            public List < ConnectorSegment > GetCovers(Point p, DesignerEdges side)
            {
                List < ConnectorSegment > covers = new List < ConnectorSegment >();

                if (side == DesignerEdges.Left || side == DesignerEdges.Right)
                {
                    for (int i = 0; i < this.verticalCovers.Count; i++)
                    {
                        ConnectorSegment segment = this.verticalCovers[i];
                        int currentDistance = (int)((side == DesignerEdges.Left) ? p.X - segment.A.X : segment.A.X - p.X);
                        if (currentDistance > 0 && segment.Covers(p))
                        {
                            covers.Add(segment);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < this.horizontalCovers.Count; i++)
                    {
                        ConnectorSegment segment = this.horizontalCovers[i];
                        int currentDistance = (int)((side == DesignerEdges.Bottom) ? p.Y - segment.A.Y : segment.A.Y - p.Y);
                        if (currentDistance > 0 && segment.Covers(p))
                        {
                            covers.Add(segment);
                        }
                    }
                }

                return covers;
            }

            public bool IsEscapePoint(Point origin, Point escape, DesignerEdges side)
            {
                ConnectorSegment originalCover = this.GetCover(origin, side);
                int originalDistance;
                if (side == DesignerEdges.Left || side == DesignerEdges.Right)
                {
                    originalDistance = (int)(originalCover.A.X - escape.X);
                }
                else
                {
                    originalDistance = (int)(originalCover.A.Y - escape.Y);
                }

                if (originalCover.Covers(escape))
                {
                    return false;
                }

                List < ConnectorSegment > newCovers = this.GetCovers(escape, side);
                for (int i = 0; i < newCovers.Count; i++)
                {
                    ConnectorSegment newCover = newCovers[i];
                    if (newCover == originalCover)
                    {
                        return false;
                    }

                    int newDistance;
                    if (side == DesignerEdges.Left || side == DesignerEdges.Right)
                    {
                        newDistance = (int)Math.Abs(newCover.A.X - escape.X);
                    }
                    else
                    {
                        newDistance = (int)Math.Abs(newCover.A.Y - escape.Y);
                    }

                    if (Math.Sign(newDistance) == Math.Sign(originalDistance) && Math.Abs(newDistance) < Math.Abs(originalDistance))
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        sealed class DistanceSorter : IComparer < DistanceFromPoint >
        {
            DistanceSorter()
            {
            }

            public static void Sort(ref List < DistanceFromPoint > distances)
            {
                DistanceSorter sorter = new DistanceSorter();
                distances.Sort(sorter);
            }

            int IComparer < DistanceFromPoint >.Compare(DistanceFromPoint lhs, DistanceFromPoint rhs)
            {
                if (lhs.Distance.IsEqualTo(rhs.Distance))
                {
                    return 0;
                }
                else if (lhs.Distance > rhs.Distance)
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
            }
        }

    }
}

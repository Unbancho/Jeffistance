using Avalonia;
using Avalonia.Controls;
using System;

namespace Jeffistance.Client.ViewModels
{
    public class CircularPanel : Panel
    {

        // Override the default Measure method of Panel
        protected override Size MeasureOverride(Size availableSize)
        {
            var panelDesiredSize = new Size();

            // In our example, we just have one child. 
            // Report that our panel requires just the size of its only child.
            foreach (var child in Children)
            {
                child.Measure(availableSize);
                panelDesiredSize = child.DesiredSize;
            }

            return panelDesiredSize;
        }

        private Point GetPoint(Point circleCenter, int childIndex, int childCount, Control child)
        {
            double r = circleCenter.Y;
            double angle = (childIndex * 360) / childCount;
            double xp2 = circleCenter.X + r*Math.Sin(AnglesToRadians(angle));
            if(angle > 0 && 180 > angle)
            {
                xp2 = xp2 - child.DesiredSize.Width;
            }
            double yp2 = (0 + r*(1 - Math.Cos(AnglesToRadians(angle))));
            if(270 > angle && angle > 90 )
            {
                yp2 = yp2 - child.DesiredSize.Height;
            }
            return new Point(xp2, yp2);
        }

        private double AnglesToRadians(double angle)
        {
            return (Math.PI / 180) * angle;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            Point circleCenter = new Point(finalSize.Width/2, finalSize.Height/2);
            int childIndex = 0;
            foreach (Control child in Children)
            {
                Point childPoint = GetPoint(circleCenter, childIndex, Children.Count, child);
                //double x = circleCenter.X;
                //double y = circleCenter.Y;
                //Point childPoint = new Point(0,0);
                child.Arrange(new Rect(childPoint, child.DesiredSize));
                childIndex++;
            }
            
            return finalSize; // Returns the final Arranged size
        }
    }
}
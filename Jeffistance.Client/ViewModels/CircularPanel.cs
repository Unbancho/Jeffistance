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

        private Point GetPoint(Point circleCenter, int childIndex, int childCount, Control child, Size finalSize)
        {
            //Defining radius. Using half of the smallest side
            double r;
            if(finalSize.Height > finalSize.Width)
            {
                r = circleCenter.X;
            }
            else
            {
                r = circleCenter.Y;
            }
            //Dividing the angles for the number of players
            double angle = (childIndex * 360) / childCount;

            double ellipseX = finalSize.Width / (r + circleCenter.X);            
            double xp2 = (circleCenter.X + r*Math.Sin(AnglesToRadians(angle))) - child.DesiredSize.Width/2;
            double yp2 = (0 + r*(1 - Math.Cos(AnglesToRadians(angle)))) + child.DesiredSize.Height/2;

            
            if((angle < 180) && (xp2 + child.DesiredSize.Width >= finalSize.Width))
            {
                xp2 = xp2 - child.DesiredSize.Width - Math.Abs(finalSize.Width - xp2);
            }
            if((angle > 90) && (yp2 + child.DesiredSize.Height >= finalSize.Height))
            {
                yp2 = yp2 - child.DesiredSize.Height - Math.Abs(finalSize.Height - yp2);
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
                Point childPoint = GetPoint(circleCenter, childIndex, Children.Count, child, finalSize);
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
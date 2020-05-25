using Avalonia;
using Avalonia.Controls;
using System;
using Jeffistance.Common.ExtensionMethods;

namespace Jeffistance.Client.ViewModels
{
    public class CircularPanel : Panel
    {

        private Point GetPoint(Point circleCenter, int childIndex, int childCount, Control child)
        {
            (double rx, double ry) = (circleCenter.X - child.DesiredSize.Width, circleCenter.Y - child.DesiredSize.Height);

            //Dividing the angles for the number of players
            double angle = (childIndex * 360) / childCount;

            double xp2 = (circleCenter.X + rx * Math.Cos(angle.ToRadians()) - child.DesiredSize.Width/2);
            double yp2 = (circleCenter.Y + ry * Math.Sin(angle.ToRadians()) - child.DesiredSize.Height/2);
            
            return new Point(xp2, yp2);
        }


        protected override Size ArrangeOverride(Size finalSize)
        {
            Point circleCenter = new Point(finalSize.Width/2, finalSize.Height/2);
            int childIndex = 0;
            foreach (Control child in Children)
            {
                Point childPoint = GetPoint(circleCenter, childIndex, Children.Count, child);
                child.Arrange(new Rect(childPoint, child.DesiredSize));
                childIndex++;
            }
            return finalSize; // Returns the final Arranged size
        }
    }
}
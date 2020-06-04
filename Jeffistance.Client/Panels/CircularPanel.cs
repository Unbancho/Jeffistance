using Avalonia;
using Avalonia.Controls;
using System;
using System.Collections.Generic;
using Jeffistance.Common.JeffMath;
using System.Linq;


namespace Jeffistance.Client.ViewModels
{
    public class CircularPanel : Panel
    {
        protected override Size ArrangeOverride(Size finalSize)
        {
            Point circleCenter = new Point(finalSize.Width/2, finalSize.Height/2);
            List<Point> Placements = CalculatePlacements(Children.Count, 400, 150);
            foreach (var (placement, child) in Placements.Zip(Children, (placement, child) => (placement, child)))
            {
                child.Arrange(new Rect(circleCenter + new Point(placement.X, placement.Y) - new Point(child.DesiredSize.Width/2, child.DesiredSize.Height/2), child.DesiredSize));
            }
            return finalSize; // Returns the final Arranged size
        }

        private List<Point> CalculatePlacements(int nPlacements, double a=1, double b=1, double precision=3)
        {
            List<Point> placements = new List<Point>();
            double angle = 0;
            double deltaAngle = Math.Pow(10, -precision);
            double nIntegrals = Math.Round(2*Math.PI/deltaAngle);
            int nextPoint = 0;
            double run = 0;
            double circumference = JeffMath.CalculateEllipseCircumference(a, b);
            for( int i=0; i < nIntegrals; i++ ) {
                angle += deltaAngle;
                double subIntegral = nPlacements*run/circumference;
                if( (int) subIntegral >= nextPoint ) 
                {
                    placements.Add(new Point(a * Math.Cos(angle), b * Math.Sin(angle)));
                    nextPoint++;
                }
                run += Math.Sqrt(Math.Pow(a*Math.Sin(angle), 2) + Math.Pow(b*Math.Cos(angle), 2));
            }
            return placements;
        }
    }
}
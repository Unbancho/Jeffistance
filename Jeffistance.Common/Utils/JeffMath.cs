using System;

namespace Jeffistance.Common.JeffMath
{
    public static class JeffMath
    {
        public static double CalculateEllipseCircumference(double a=1, double b=2, int precision=3)
        {
            double circumference = 0;
            double theta = 0;
            double deltaTheta = Math.Pow(10, -precision);
            double nIntegrals = Math.Round(2*Math.PI/deltaTheta);
            for(int i=0; i<nIntegrals; i++) 
            {
                theta += i*deltaTheta;
                circumference += Math.Sqrt(Math.Pow(a*Math.Sin(theta), 2) + Math.Pow( b*Math.Cos(theta), 2));
            }
            return circumference;
        }
    }
}
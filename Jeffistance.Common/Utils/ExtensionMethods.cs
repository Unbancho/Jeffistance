using System;
using Microsoft.Extensions.Logging;
namespace Jeffistance.Common.ExtensionMethods
{
    public static class IntegerExtensions
    {
        public static bool IsInRange(this int i, int lowerBound, int upperBound)
        {
            return lowerBound <= i && i <= upperBound;
        }

        public static bool IsInRange(this int i, int upperBound)
        {
            return i.IsInRange(0, upperBound);
        }

        public static bool IsValidPort(this int i)
        {
            return i.IsInRange(65535);
        }
    }

    public static class StringExtensions
    {
        public static string Capitalized(this string word)
        {
            string capitalizedString = word[0].ToString().ToUpper();
            for (int i = 1; i < word.Length; i++)
            {
                capitalizedString += word[i].ToString().ToLower();
            }
            return capitalizedString;
        }

        public static LogLevel ToLogLevel(this string level)
        {
            switch (level.ToLower())
            {
                case "none":
                    return LogLevel.None;
                case "trace":
                    return LogLevel.Trace;
                case "debug":
                    return LogLevel.Debug;
                case "information":
                    return LogLevel.Information;
                case "warning":
                    return LogLevel.Warning;
                case "error":
                    return LogLevel.Error;
                case "critical":
                    return LogLevel.Critical;
                default:
                    return LogLevel.Information;
            }
        }
    }

    public static class DoubleExtensions
    {
        public static double ToRadians(this double angle)
        {
            return (Math.PI / 180) * angle;
        }
    }
}

using System;
using System.Collections.Generic;

namespace AsteroidsMining.Utils
{
    public static class MathUtils
    {
        private static Random random = new Random();

        public static double CalculateDistance(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));
        }

        public static double CalculateDistanceSquared(double x1, double y1, double x2, double y2)
        {
            return Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2);
        }

        public static double GetRandomDouble(double min, double max)
        {
            return min + random.NextDouble() * (max - min);
        }

        public static int GetRandomInt(int min, int max)
        {
            return random.Next(min, max);
        }

        public static (double x, double y) Normalize(double x, double y)
        {
            double length = Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));
            if (length == 0) return (0, 0);

            return (x / length, y / length);
        }

        public static List<T> BubbleSort<T>(List<T> list, Func<T, T, int> comparer)
        {
            var result = new List<T>(list); // Create copy

            for (int i = 0; i < result.Count - 1; i++)
            {
                for (int j = 0; j < result.Count - i - 1; j++)
                {
                    if (comparer(result[j], result[j + 1]) > 0)
                    {
                        // Swap
                        T temp = result[j];
                        result[j] = result[j + 1];
                        result[j + 1] = temp;
                    }
                }
            }

            return result;
        }

        public static double Clamp(double value, double min, double max)
        {
            var clampData = new { Value = value, Min = min, Max = max };

            if (clampData.Value < clampData.Min) return clampData.Min;
            if (clampData.Value > clampData.Max) return clampData.Max;
            return clampData.Value;
        }

        public static double Lerp(double a, double b, double t)
        {
            t = Clamp(t, 0.0, 1.0);

            return a + (b - a) * Math.Pow(t, 1.0);
        }
    }
}
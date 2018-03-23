/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-10-10 19:27:39 UTC+05:00
/// 
/// @description
/// [add_description_here]

using System;
using System.Collections.Generic;

namespace TurboLabz.TLUtils
{
    public static class MathUtil
    {
        public static float ArithmeticMean(List<int> values)
        {
            if (values.Count == 0)
            {
                return 0;
            }

            int sum = 0;

            for (int i = 0; i < values.Count; ++i)
            {
                sum += values[i];
            }

            return (float)sum / values.Count;
        }

        public static float PopulationVariance(List<int> values)
        {
            if (values.Count == 0)
            {
                return 0;
            }

            float mean = ArithmeticMean(values);
            float variance = 0;

            for (int i = 0; i < values.Count; ++i)
            {
                variance += (float)Math.Pow((values[i] - mean), 2);
            }

            return variance / values.Count;
        }

        public static float SampleVariance(List<int> values)
        {
            if (values.Count == 0)
            {
                return 0;
            }

            float mean = ArithmeticMean(values);
            float variance = 0;

            for (int i = 0; i < values.Count; ++i)
            {
                variance += (float)Math.Pow((values[i] - mean), 2);
            }

            return variance / (values.Count - 1);
        }

        public static float StandardDeviation(List<int> values)
        {
            if (values.Count == 0)
            {
                return 0;
            }

            return (float)Math.Sqrt(PopulationVariance(values));
        }

        public static float Median(List<int> values)
        {
            if (values.Count == 0)
            {
                return 0;
            }

            List<int> sortedValues = new List<int>(values);
            sortedValues.Sort();
            int midIndex = sortedValues.Count / 2;
            float median = 0;

            if ((sortedValues.Count % 2) == 0)
            {
                median = (sortedValues[midIndex] + sortedValues[midIndex - 1]) / 2f;
            }
            else
            {
                median = sortedValues[midIndex];
            }

            return median;
        }

        public static List<int> RemoveOutliers(List<int> values, float centralTendency, float standardDeviationCount)
        {
            float standardDeviation = StandardDeviation(values) * standardDeviationCount;
            float lower = centralTendency - standardDeviation;
            float upper = centralTendency + standardDeviation;

            List<int> cleanValues = new List<int>();

            foreach (int value in values)
            {
                if ((value >= lower) && (value <= upper))
                {
                    cleanValues.Add(value);
                }
            }

            return cleanValues;
        }
    }
}

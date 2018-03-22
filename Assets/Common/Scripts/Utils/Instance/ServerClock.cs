/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-10-17 08:57:37 UTC+05:00
/// 
/// @description
/// [add_description_here]

using System.Collections.Generic;

namespace TurboLabz.Common
{
    public class ServerClock : IServerClock
    {
        public int latency { get; private set; }
        public int latencySampleCount { get; set; }
        public int serverClockDelta { get; private set; }

        public long currentTimestamp
        {
            get
            {
                return TimeUtil.unixTimestampMilliseconds + serverClockDelta;
            }
        }

        private List<int> latencySamples = new List<int>();

        public void Reset()
        {
            latency = 0;
            serverClockDelta = 0;
            latencySamples.Clear();
        }

        public void CalculateLatency(long clientSendTimestamp,
                                     long serverReceiptTimestamp,
                                     long clientReceiptTimestamp)
        {
            int latency = (int)((clientReceiptTimestamp - clientSendTimestamp) / 2);
            SampleLatency(latency);
            this.latency = GetAverageLatency();
            serverClockDelta = (int)((serverReceiptTimestamp - clientReceiptTimestamp) + this.latency);
        }

        private void SampleLatency(int latency)
        {
            if (latencySamples.Count == latencySampleCount)
            {
                latencySamples.RemoveAt(0);
            }

            latencySamples.Add(latency);
        }

        private int GetAverageLatency()
        {   
            float median = MathUtil.Median(latencySamples);
            List<int> cleanSamples = MathUtil.RemoveOutliers(latencySamples, median, 1);
            return (int)MathUtil.ArithmeticMean(cleanSamples);
        }
    }
}

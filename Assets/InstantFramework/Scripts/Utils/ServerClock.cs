/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
using System.Collections.Generic;
using TurboLabz.InstantFramework;

namespace TurboLabz.TLUtils
{
    public class ServerClock : IServerClock
    {
        public int latency { get; private set; }
        public int serverClockDelta { get; private set; }

        public long currentTimestamp
        {
            get
            {
                return TimeUtil.unixTimestampMilliseconds;// + serverClockDelta;
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
            int currentLatency = (int)((clientReceiptTimestamp - clientSendTimestamp) / 2);
            SampleLatency(currentLatency);
            this.latency = GetAverageLatency();
            serverClockDelta = (int)((serverReceiptTimestamp - clientReceiptTimestamp) + this.latency);
        }

        private void SampleLatency(int latency)
        {
            if ((latencySamples.Count != 0) && 
                (latencySamples.Count == 6))
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

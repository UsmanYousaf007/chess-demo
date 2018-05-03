/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-10-17 17:39:18 UTC+05:00
/// 
/// @description
/// [add_description_here]

namespace TurboLabz.TLUtils
{
    public interface IServerClock
    {
        int latency { get; }
        int latencySampleCount { get; set; }
        int serverClockDelta { get; }
        long currentTimestamp { get; }

        void Reset();
        void CalculateLatency(long clientSendTimestamp,
                              long serverReceiptTimestamp,
                              long clientReceiptTimestamp);
    }
}

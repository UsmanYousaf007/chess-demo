/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-09-20 12:37:39 UTC+05:00
/// 
/// @description
/// When the session expires we keep references to all the requests in the
/// expiredRequests list so that if an expired request gets a response from the
/// backend the callbacks are not dangling.

using System.Collections.Generic;

namespace TurboLabz.Gamebet
{
    public class GSRequestSession 
    {
        public static GSRequestSession instance;

        private List<IGSRequest> currentRequests = new List<IGSRequest>();
        private List<IGSRequest> expiredRequests = new List<IGSRequest>();

        public GSRequestSession()
        {
            instance = this;
        }
 
        public void AddRequest(IGSRequest request)
        {
            currentRequests.Add(request);
        }

        public void RemoveRequest(IGSRequest request)
        {
            currentRequests.Remove(request);
        }

        public void EndSession()
        {
            foreach (IGSRequest request in currentRequests)
            {
                request.Expire();
                expiredRequests.Add(request);
            }

            currentRequests.Clear();
        }
    }
}

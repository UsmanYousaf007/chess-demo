/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-09-22 20:27:56 UTC+05:00
/// 
/// @description
/// [add_description_here]

using TurboLabz.Common;

namespace TurboLabz.Gamebet
{
    public abstract class GSRequest : IGSRequest
    {
        // When requests are added to the GSRequestSession, they will all expire
        // when the session ends. The session ends when we  have a backend
        // error. You are required to ignore responses that are received in
        // expired requests.
        protected bool isExpired;

        // Call base.Expire() at the top of your derived classes or else the
        // behavior will be unexpected.
        public virtual void Expire()
        {
            Assertions.Assert(isExpired == false, "The request must not already be expired!");

            isExpired = true;
        }
    }
}

/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-09-19 21:42:54 UTC+05:00
/// 
/// @description
/// [add_description_here]

using strange.extensions.promise.api;

namespace TurboLabz.Gamebet
{
    public partial class GSService
    {
        public IPromise<BackendResult> TestRequest(string eventKey)
        {
            return new GSTestRequest().Send(eventKey);
        }
    }
}


using System.Collections.Generic;
/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
namespace TurboLabz.InstantFramework
{
    public interface IAppsFlyerService
    {
        void Init();
        void TrackRichEvent(string eventName, Dictionary<string, string> eventValues);

    }
}

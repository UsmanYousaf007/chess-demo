/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using TurboLabz.TLUtils;

namespace TurboLabz.InstantFramework
{
    public interface IRoomSettingsModel
    {
        void Reset();
        // The keys of the dictionary are the IDs of the rooms.
        IOrderedDictionary<string, RoomSetting> settings { get; set; }
    }
}

/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

namespace TurboLabz.InstantFramework
{
    public interface ILevelSettingsModel
    {
        void Reset();
        int maxLevel { get; set; }
        LevelInfo currentLevelInfo { get; set; }
    }
}

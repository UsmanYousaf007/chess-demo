/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

namespace TurboLabz.InstantFramework
{
    public interface IPreferencesModel
    {
        bool isAudioOn { get; set; }
        int adSlotImpressions { get; set; }    
        long adSlotId { get; set; }
        bool hasRated { get; set; }
        bool isSafeMoveOn { get; set; }
        bool isFriendScreenVisited { get; set; }
        bool isCoachTooltipShown { get; set; }
        bool isStrengthTooltipShown { get; set; }
    }
}

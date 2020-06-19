/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
using strange.extensions.command.impl;

namespace TurboLabz.InstantFramework
{
    public class MatchAnalyticsCommand : Command
    {
        // Paramaters
        [Inject] public MatchAnalyticsVO matchAnalyticsVO { get; set; }

        // Services
        [Inject] public IAnalyticsService analyticsService { get; set; }

        public override void Execute()
        {
            analyticsService.Event($"{matchAnalyticsVO.matchType}_{matchAnalyticsVO.eventID}_{matchAnalyticsVO.friendType}", matchAnalyticsVO.context);
        }
    }
}

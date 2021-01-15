/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2020 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
///

using System;
using strange.extensions.command.impl;

namespace TurboLabz.InstantFramework
{
    public class GetAllStarLeaderboardCommand : Command
    {
        // dispatch signals
        [Inject] public UpdateAllStarLeaderboardSignal updateAllStarLeaderboardSignal { get; set; }

        // Models
        [Inject] public IMetaDataModel metaDataModel { get; set; }

        // services
        [Inject] public IBackendService backendService { get; set; }

        public override void Execute()
        {
            Retain();

            var lastFetchedTimeDelta = DateTimeOffset.UtcNow.ToUnixTimeSeconds() - metaDataModel.settingsModel.allStarLeaderboardLastFetchTime;
            if (lastFetchedTimeDelta >= GSSettings.LEADERBOARDS_FETCH_GAP_TIME)
            {
                backendService.GetAllStarLeaderboard().Then(OnGetComplete);
            }
            else
            {
                updateAllStarLeaderboardSignal.Dispatch();
            }
        }

        private void OnGetComplete(BackendResult result)
        {
            if (result == BackendResult.SUCCESS)
            {
                metaDataModel.settingsModel.allStarLeaderboardLastFetchTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

                updateAllStarLeaderboardSignal.Dispatch();
            }

            Release();
        }
    }
}

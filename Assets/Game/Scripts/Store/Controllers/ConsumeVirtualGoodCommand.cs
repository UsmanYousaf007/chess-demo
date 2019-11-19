/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System.Collections.Generic;
using GameSparks.Core;
using strange.extensions.command.impl;
using TurboLabz.TLUtils;

namespace TurboLabz.InstantFramework
{
    public class ConsumeVirtualGoodCommand : Command
    {
        // Command Params
        [Inject] public string key { get; set; }
        [Inject] public int quantity { get; set; }

        // Dispatch Signals
        [Inject] public UpdatePlayerInventorySignal updatePlayerInventorySignal { get; set; }

        // Models
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IMatchInfoModel matchInfoModel { get; set; }
        [Inject] public IAppInfoModel appInfoModel { get; set; }


        // Services
        [Inject] public IBackendService backendService { get; set; }
        [Inject] public IAnalyticsService analyticsService { get; set; }

        string challengeId = "";

        public override void Execute()
        {
            challengeId = matchInfoModel.activeChallengeId;

            if(appInfoModel.gameMode == GameMode.CPU)
            {
                challengeId = "";
            }

            LogUtil.Log("matchInfoModel ID " + challengeId);


            GSRequestData jsonData = new GSRequestData().AddString("shortCode", key).
                                                         AddNumber("quantity", quantity).
                                                         AddString("challengeId", challengeId);

            Retain();
            backendService.ConsumeVirtualGood(jsonData).Then(OnConsumeStoreItem);
        }

        private void OnConsumeStoreItem(BackendResult result)
        {
            if (result == BackendResult.SUCCESS)
            {
                updatePlayerInventorySignal.Dispatch(playerModel.GetPlayerInventory());
            }

            Release();
        }
    }
}
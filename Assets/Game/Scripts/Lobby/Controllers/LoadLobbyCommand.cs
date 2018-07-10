/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @description
/// [add_description_here]

using strange.extensions.command.impl;
using TurboLabz.Chess;
using System;
using TurboLabz.TLUtils;
using TurboLabz.InstantFramework;
using TurboLabz.CPU;

namespace TurboLabz.InstantGame
{
    public class LoadLobbyCommand : Command
    {
        // Dispatch Signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public UpdateMenuViewSignal updateMenuViewSignal { get; set; }
        [Inject] public UpdateAdsSignal updateAdsSignal { get; set; }
        [Inject] public SetSkinSignal setSkinSignal { get; set; }
        [Inject] public LoadGameSignal loadCPUGameDataSignal { get; set; }
        [Inject] public UpdatePlayerBucksDisplaySignal updatePlayerBucksDisplaySignal { get; set; }
        [Inject] public UpdateProfileSignal updateProfileSignal { get; set; }

        // Models
        [Inject] public ICPUGameModel cpuGameModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }
		[Inject] public IMetaDataModel metaDataModel { get; set; }

        // Services
        [Inject] public IFacebookService facebookService { get; set; }

        public override void Execute()
        {
            setSkinSignal.Dispatch(playerModel.activeSkinId);

            loadCPUGameDataSignal.Dispatch();
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_LOBBY);

			LobbyVO vo = new LobbyVO(cpuGameModel, playerModel, metaDataModel);

            updateMenuViewSignal.Dispatch(vo);
            updateAdsSignal.Dispatch();
            updatePlayerBucksDisplaySignal.Dispatch(playerModel.bucks);

            // If the social pic is not available yet
            ProfileVO pvo = new ProfileVO();
            pvo.playerPic = playerModel.socialPic;
            pvo.playerName = playerModel.name;
            pvo.eloScore = playerModel.eloScore;
            pvo.countryId = playerModel.countryId;

            if (facebookService.isLoggedIn())
            {
                pvo.isFacebookLoggedIn = true;

                if (pvo.playerPic == null)
                {
                    pvo.playerPic = facebookService.GetCachedPlayerPic();
                }
            }

            updateProfileSignal.Dispatch(pvo);
        }
    }
}

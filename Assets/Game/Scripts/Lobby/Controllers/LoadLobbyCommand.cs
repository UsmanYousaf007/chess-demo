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
        [Inject] public ApplySkinSignal applySkinSignal { get; set; }
        [Inject] public LoadCPUGameDataSignal loadCPUGameDataSignal { get; set; }

        // Models
        [Inject] public ICPUGameModel cpuGameModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }
		[Inject] public IMetaDataModel metaDataModel { get; set; }

        public override void Execute()
        {
            applySkinSignal.Dispatch(playerModel.activeSkinId);

            loadCPUGameDataSignal.Dispatch();

            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_LOBBY);

			LobbyVO vo = new LobbyVO(cpuGameModel, playerModel, metaDataModel);
            updateMenuViewSignal.Dispatch(vo);

            updateAdsSignal.Dispatch();
        }
    }
}

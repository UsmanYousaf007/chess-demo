/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
///
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-05-15 11:09:22 UTC+05:00
///
/// @description
/// [add_description_here]

using strange.extensions.command.impl;

namespace TurboLabz.Gamebet
{
    public class EndGameCommand : Command
    {
        // Dispatch signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public UpdateEndGameViewSignal updateEndGameViewSignal { get; set; }

        // Models
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IMatchInfoModel matchInfoModel { get; set; }
        [Inject] public IRoomSettingsModel roomSettingsModel { get; set; }
        [Inject] public IPromotionsModel promotionsModel { get; set; }

        public override void Execute()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_END_GAME);

            Promotions promotions;
            promotions.levelPromotions = promotionsModel.levelPromotions;
            promotions.leaguePromotion = promotionsModel.leaguePromotion;
            promotions.trophyPromotion = promotionsModel.trophyPromotion;
            promotions.roomTitlePromotion = promotionsModel.roomTitlePromotion;

            EndGameVO vo;
            vo.endGameResult = matchInfoModel.endGameResult;
            vo.currency1 = playerModel.currency1;
            vo.currency2 = playerModel.currency2;
            vo.roomInfo = roomSettingsModel.settings[matchInfoModel.roomId];
            vo.playerPublicProfile = playerModel.publicProfile;
            vo.opponentPublicProfile = matchInfoModel.opponentPublicProfile;
            vo.promotions = promotions;
            vo.playerModel = playerModel;

            updateEndGameViewSignal.Dispatch(vo);
        }
    }
}

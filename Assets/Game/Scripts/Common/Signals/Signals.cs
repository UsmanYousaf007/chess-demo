/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2018-01-11 18:33:07 UTC+05:00
/// 
/// @description
/// [add_description_here]
using strange.extensions.signal.impl;
using TurboLabz.InstantFramework;


namespace TurboLabz.InstantGame
{
    // CPU LOBBY
    public class UpdateMenuViewSignal : Signal<LobbyVO> {}
    public class AdjustStrengthSignal : Signal<bool> {}
    public class UpdateStrengthSignal : Signal<LobbyVO> {}
    public class UpdateDurationSignal : Signal<LobbyVO> {}
    public class UpdatePlayerColorSignal : Signal<LobbyVO> {}
	public class UpdateThemeSignal : Signal<LobbyVO> {}
    public class UpdateLobbyAdsSignal : Signal<AdsVO>{}
    public class UpdateAdsSignal : Signal {}
    public class ShowPromotionSignal : Signal<PromotionVO> {}
    public class LoadPromotionSingal : Signal {}
    public class ShowCoachTrainingDailogueSignal : Signal {}
    public class ShowStrengthTrainingDailogueSignal : Signal {}
    public class RemoveLobbyPromotionSignal : Signal<string> {}
    public class ReportLobbyPromotionAnalyticSingal : Signal<string, AnalyticsEventId> {}

    // CPU STATS
    public class LoadStatsSignal : Signal {}
    public class UpdateStatsDurationSignal : Signal<string> {}
    public class SaveStatsSignal : Signal<int> {}
    public class UpdateStatsSignal : Signal<StatsVO> {};

	// CPU STORE
	public class LoadStoreSignal : Signal {}
    public class LoadSpotPurchaseSignal : Signal<SpotPurchaseView.PowerUpSections> { }
    public class UpdateStoreSignal : Signal<StoreVO> {}
	public class UpdateStoreBuyDlgSignal : Signal<StoreItem> {}
	public class UpdateStoreNotEnoughBucksDlgSignal : Signal<StoreItem> {}
    public class ShowStoreTabSignal : Signal<StoreView.StoreTabs> {}
    public class UpdateSpotPurchaseSignal : Signal<StoreVO, SpotPurchaseView.PowerUpSections> {}
    public class ShowProcessingSignal : Signal<bool> {}

    // Top Inventory Bar
    public class UpdateTopInventoryBarSignal : Signal<PlayerInventoryVO> {}

    // Player Profile
    public class PlayerProfilePicTappedSignal : Signal {};
}

/// @license Propriety <http://license.url>
/// @copyright Copyright (C) DefaultCompany 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using TurboLabz.TLUtils;

namespace TurboLabz.InstantFramework
{
    public partial class GSService : IBackendService
    {
        // Dispatch signals
        [Inject] public BackendErrorSignal backendErrorSignal { get; set; }
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public ReceptionSignal receptionSignal { get; set; }
        [Inject] public NewFriendSignal newFriendSignal { get; set; }
        [Inject] public FindMatchCompleteSignal findMatchCompleteSignal { get; set; }
        [Inject] public FindRandomLongMatchCompleteSignal findRandomLongMatchCompleteSignal { get; set; }
        [Inject] public StartLongMatchSignal startLongMatchSignal { get; set; }
        [Inject] public RefreshFriendsSignal refreshFriendsSignal { get; set; }
        [Inject] public RefreshCommunitySignal refreshCommunitySignal { get; set; }
        [Inject] public RemoveFriendSignal removeFriendSignal { get; set; }
        [Inject] public UpdateFriendOnlineStatusSignal updtateFriendOnlineStatusSignal { get; set; }
        [Inject] public ClearFriendSignal clearFriendSignal { get; set; }
        [Inject] public ReceiveChatMessageSignal receiveChatMessageSignal { get; set; }
        [Inject] public UpdatePlayerInventorySignal updatePlayerInventorySignal { get; set; }
        [Inject] public ChallengeAcceptedSignal challengeAcceptedSignal { get; set; }
        [Inject] public OpponentPingedForConnectionSignal opponentPingedForConnectionSignal { get; set; }
        [Inject] public StoreAvailableSignal storeAvailableSignal { get; set; }
        [Inject] public SetDefaultSkinSignal setDefaultSkinSignal { get; set; }
        [Inject] public GetInitDataCompleteSignal getInitDataCompleteSignal { get; set; }
        [Inject] public GetInitDataFailedSignal getInitDataFailedSignal { get; set; }
        [Inject] public UpdatePurchasedStoreItemSignal updatePurchasedStoreItemSignal { get; set; }
        [Inject] public UpdateTournamentsViewSignal updateTournamentsViewSignal { get; set; }
        [Inject] public UpdateTournamentLeaderboardSignal updateTournamentLeaderboardSuccessSignal { get; set; }
        [Inject] public PlayerModelUpdatedSignal playerModelUpdatedSignal { get; set; }
        [Inject] public ClearInboxSignal clearInboxSignal { get; set; }
        [Inject] public UpdatePromotionBundleSignal updateBundleSignal { get; set; }
        [Inject] public LobbySequenceEndedSignal lobbySequenceEndedSignal { get; set; }
        [Inject] public UpdateRVTimerSignal updateRVTimer { get; set; }
        [Inject] public DailyRewardClaimFailedSignal dailyRewardClaimFailedSignal { get; set; }

        // Models
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IStoreSettingsModel storeSettingsModel { get; set; }
        [Inject] public IAppInfoModel appInfoModel { get; set; }
        [Inject] public IMetaDataModel metaDataModel { get; set; }
        [Inject] public IMatchInfoModel matchInfoModel { get; set; }
        [Inject] public IAdsSettingsModel adsSettingsModel { get; set; }
        [Inject] public IPicsModel picsModel { get; set; }
        [Inject] public IPreferencesModel preferencesModel { get; set; }
        [Inject] public IChatModel chatModel { get; set; }
        [Inject] public IRewardsSettingsModel rewardsSettingsModel { get; set; }
        [Inject] public ISettingsModel settingsModel { get; set; }
        [Inject] public INavigatorModel navigatorModel { get; set; }
        [Inject] public ILessonsModel lessonsModel { get; set; }
        [Inject] public ITournamentsModel tournamentsModel { get; set; }
        [Inject] public IInboxModel inboxModel { get; set; }
        [Inject] public IDownloadablesModel downloadablesModel { get; set; }
        [Inject] public ILeaguesModel leaguesModel { get; set; }
        [Inject] public ILeaderboardModel leaderboardModel { get; set; }
        [Inject] public INotificationsModel notificationsModel { get; set; }

        // Services
        [Inject] public IStoreService storeService { get; set; }
        [Inject] public IFacebookService facebookService { get; set; }
        [Inject] public IAppsFlyerService appsFlyerService { get; set; }
        [Inject] public IHAnalyticsService hAnalyticsService { get; set; }
        [Inject] public IAnalyticsService analyticsService { get; set; }
        [Inject] public ISignInWithAppleService signInWithAppleService { get; set; }
        [Inject] public IGameModesAnalyticsService gameModesAnalyticsService { get; set; }
        [Inject] public ILocalizationService localizationService { get; set; }
        [Inject] public IBackendService backendService { get; set; } // Temp fix for beta. Remove this later
        [Inject] public IPromotionsService promotionsService { get; set; }

        // Utils
        [Inject] public IRoutineRunner routineRunner { get; set; }
        [Inject] public IServerClock serverClock { get; set; }

        private GSFrameworkRequestContext GetRequestContext()
        {
            return new GSFrameworkRequestContext { currentViewId = navigatorModel.currentViewId };
        }
    }
}

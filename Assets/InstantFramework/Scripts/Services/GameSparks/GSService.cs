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
        [Inject] public StartLongMatchSignal startLongMatchSignal { get; set; }
        [Inject] public StoreAvailableSignal storeAvailableSignal { get; set; }
        [Inject] public RefreshFriendsSignal refreshFriendsSignal { get; set; }
        [Inject] public RemoveFriendSignal removeFriendSignal { get; set; }
        [Inject] public UpdateFriendOnlineStatusSignal updtateFriendOnlineStatusSignal { get; set; }
        [Inject] public ClearFriendSignal clearFriendSignal { get; set; }
        [Inject] public ReceiveChatMessageSignal receiveChatMessageSignal { get; set; }

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

        // Services
        [Inject] public IStoreService storeService { get; set; }
        [Inject] public IFacebookService facebookService { get; set; }

        // Utils
        [Inject] public IRoutineRunner routineRunner { get; set; }
        [Inject] public IServerClock serverClock { get; set; }
    }
}

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
 
        // Models
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public ILeagueSettingsModel leagueSettingsModel { get; set; }
        [Inject] public IRoomSettingsModel roomSettingsModel { get; set; }
        [Inject] public IStoreSettingsModel storeSettingsModel { get; set; }
        [Inject] public IAppInfoModel appInfoModel { get; set; }
        [Inject] public IMetaDataModel metaDataModel { get; set; }
        [Inject] public IPromotionsModel promotionsModel { get; set; }
        [Inject] public IMatchInfoModel matchInfoModel { get; set; }
        [Inject] public IAdsSettingsModel adsSettingsModel { get; set; }

        // Services
        [Inject] public IFacebookService facebookService { get; set; }
        [Inject] public IStoreService storeService { get; set; }

        // Utils
        [Inject] public IRoutineRunner routineRunner { get; set; }
        [Inject] public IServerClock serverClock { get; set; }
    }
}

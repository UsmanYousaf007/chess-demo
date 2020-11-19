/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine;
using TurboLabz.InstantGame;

namespace TurboLabz.InstantFramework
{
    public class RateAppService : IRateAppService
    {
        [Inject] public IPreferencesModel preferencesModel { get; set; }
        [Inject] public IMetaDataModel metaDataModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public ICPUStatsModel cpuStatsModel { get; set; }
        [Inject] public IMatchInfoModel matchInfoModel { get; set; }
        [Inject] public IAppInfoModel appInfoModel { get; set; }

        public void RateApp(bool goRate)
        {
            preferencesModel.hasRated = goRate;

            // Bail if not going to rate
            if (!goRate)
            {
                return;
            }

            #if UNITY_IOS
            //UnityEngine.iOS.Device.RequestStoreReview();
            Application.OpenURL(metaDataModel.appInfo.iosURL + "?action=write-review");
            #endif

            #if UNITY_ANDROID
            Application.OpenURL(metaDataModel.appInfo.androidURL);
            #endif

            #if UNITY_EDITOR
            Application.OpenURL("https://itunes.apple.com/us/app/chess/id1386718098?mt=8");
            Application.OpenURL("https://play.google.com/store/apps/details?id=com.turbolabz.instantchess.android.googleplay");
            #endif
        }

        public bool CanShowRateDialogue()
        {
            bool returnVal = false;

            int totalWins = playerModel.totalGamesWon + cpuStatsModel.GetStarsCount();

            if (!preferencesModel.isRateAppDialogueFirstTimeShown && totalWins > 1)
            {
                returnVal = true;
            }
            else if (!preferencesModel.hasRated && (totalWins == 1 || totalWins % metaDataModel.appInfo.nthWinsRateApp == 0) && (matchInfoModel.lastCompletedMatch != null && matchInfoModel.lastCompletedMatch.winnerId == playerModel.id))
            {
                returnVal = true;
            }
            else
            {
                returnVal = false;
            }

            return returnVal;
        }
    }
}
using TurboLabz.TLUtils;
using UnityEngine;

namespace TurboLabz.InstantFramework
{
    public class GameModesAnalyticsService : IGameModesAnalyticsService
    {
        //Models
        [Inject] public IPreferencesModel preferencesModel { get; set; }

        //Services
        [Inject] public IAnalyticsService analyticsService { get; set; }

        public void ProcessTimeSpent(float timeSpent, MatchInfo matchInfo = null)
        {
            if (matchInfo == null)
            {
                preferencesModel.timeSpentCpuMatch += timeSpent;
            }
            else if (matchInfo.isLongPlay)
            {
                preferencesModel.timeSpentLongMatch += timeSpent;
            }
            else if (matchInfo.gameTimeMode == GameTimeMode.TenMin)
            {
                preferencesModel.timeSpent10mMatch += timeSpent;
            }
            else if (matchInfo.gameTimeMode == GameTimeMode.OneMin)
            {
                preferencesModel.timeSpent1mMatch += timeSpent;
            }
            else if (matchInfo.gameTimeMode == GameTimeMode.ThreeMin)
            {
                preferencesModel.timeSpent3mMatch += timeSpent;
            }
            else if (matchInfo.gameTimeMode == GameTimeMode.ThirtyMin)
            {
                preferencesModel.timeSpent30mMatch += timeSpent;
            }
            else
            {
                preferencesModel.timeSpent5mMatch += timeSpent;
            }
        }

        public void LogTimeSpent()
        {
            analyticsService.Event("cpu_match", AnalyticsParameter.seconds, Mathf.RoundToInt(preferencesModel.timeSpentCpuMatch));
            analyticsService.Event("classic_match", AnalyticsParameter.seconds, Mathf.RoundToInt(preferencesModel.timeSpentLongMatch));
            analyticsService.Event("1m_match", AnalyticsParameter.seconds, Mathf.RoundToInt(preferencesModel.timeSpent1mMatch));
            analyticsService.Event("3m_match", AnalyticsParameter.seconds, Mathf.RoundToInt(preferencesModel.timeSpent3mMatch));
            analyticsService.Event("5m_match", AnalyticsParameter.seconds, Mathf.RoundToInt(preferencesModel.timeSpent5mMatch));
            analyticsService.Event("10m_match", AnalyticsParameter.seconds, Mathf.RoundToInt(preferencesModel.timeSpent10mMatch));
            analyticsService.Event("30m_match", AnalyticsParameter.seconds, Mathf.RoundToInt(preferencesModel.timeSpent30mMatch));
        }

        public void ProcessGameCount(MatchInfo matchInfo = null)
        {
            preferencesModel.gamesPlayedPerDay++;

            if (matchInfo == null)
            {
                preferencesModel.gameCountCPU++;
            }
            else if (matchInfo.isLongPlay)
            {
                preferencesModel.gameCountLong++;
            }
            else if (matchInfo.gameTimeMode == GameTimeMode.OneMin)
            {
                preferencesModel.gameCount1m++;
            }
            else if (matchInfo.gameTimeMode == GameTimeMode.ThreeMin)
            {
                preferencesModel.gameCount3m++;
            }
            else if (matchInfo.gameTimeMode == GameTimeMode.TenMin)
            {
                preferencesModel.gameCount10m++;
            }
            else if (matchInfo.gameTimeMode == GameTimeMode.ThirtyMin)
            {
                preferencesModel.gameCount30m++;
            }
            else
            {
                preferencesModel.gameCount5m++;
            }

            ProcessFavMode();
        }

        private void ProcessFavMode()
        {
            if (preferencesModel.favModeCount == 0)
            {
                preferencesModel.favModeCount = preferencesModel.gameCountCPU;
                preferencesModel.overallFavMode = "cpu";
            }

            if (preferencesModel.gameCountLong > preferencesModel.favModeCount)
            {
                preferencesModel.favModeCount = preferencesModel.gameCountLong;
                preferencesModel.overallFavMode = "48_hr";
            }
            else if (preferencesModel.gameCount1m > preferencesModel.favModeCount)
            {
                preferencesModel.favModeCount = preferencesModel.gameCount1m;
                preferencesModel.overallFavMode = "1_min";
            }
            else if (preferencesModel.gameCount3m > preferencesModel.favModeCount)
            {
                preferencesModel.favModeCount = preferencesModel.gameCount3m;
                preferencesModel.overallFavMode = "3_min";
            }
            else if (preferencesModel.gameCount5m > preferencesModel.favModeCount)
            {
                preferencesModel.favModeCount = preferencesModel.gameCount5m;
                preferencesModel.overallFavMode = "5_min";
            }
            else if (preferencesModel.gameCount10m > preferencesModel.favModeCount)
            {
                preferencesModel.favModeCount = preferencesModel.gameCount10m;
                preferencesModel.overallFavMode = "10_min";
            }
            else if (preferencesModel.gameCount30m > preferencesModel.favModeCount)
            {
                preferencesModel.favModeCount = preferencesModel.gameCount30m;
                preferencesModel.overallFavMode = "30_min";
            }
            else if (preferencesModel.gameCountCPU > preferencesModel.favModeCount)
            {
                preferencesModel.favModeCount = preferencesModel.gameCountCPU;
                preferencesModel.overallFavMode = "cpu";
            }
        }

        public void LogInstallDayData()
        {
            if (!preferencesModel.isInstallDayOver)
            {
                preferencesModel.isInstallDayOver = true;
                preferencesModel.installDayFavMode = preferencesModel.overallFavMode;
                preferencesModel.installDayGameCount = preferencesModel.gameFinishedCount;
            }

            analyticsService.Event(AnalyticsEventId.install_game_count, AnalyticsParameter.count, preferencesModel.installDayGameCount);
            analyticsService.Event(AnalyticsEventId.install_game_fav_mode, AnalyticsParameter.context, preferencesModel.installDayFavMode);
        }
    }
}

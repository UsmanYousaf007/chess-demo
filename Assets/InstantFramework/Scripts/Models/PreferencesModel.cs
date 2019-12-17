/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using TurboLabz.TLUtils;
using TurboLabz.InstantFramework;
using System;

namespace TurboLabz.InstantGame
{
    public class PreferencesModel : IPreferencesModel
    {
        // Services
        [Inject] public ILocalDataService localDataService { get; set; }
        [Inject] public IBackendService backendService { get; set; }

        // Listen to signals
        [Inject] public ModelsResetSignal modelsResetSignal { get; set; }
        [Inject] public ModelsLoadFromDiskSignal modelsLoadFromDiskSignal { get; set; }
        [Inject] public ModelsSaveToDiskSignal modelsSaveToDiskSignal { get; set; }

        public bool isAudioOn { get; set; }
        public long adSlotId { get; set; }
        public int adSlotImpressions { get; set; }
        public bool hasRated { get; set; }
        public bool isSafeMoveOn { get; set; }
        public bool isFriendScreenVisited { get; set; }
        public bool isCoachTooltipShown { get; set; }
        public bool isStrengthTooltipShown { get; set; }
        public bool isLobbyLoadedFirstTime { get; set; }
        public int coachUsedCount { get; set; }
        public int strengthUsedCount { get; set; }
        public int promotionCycleIndex { get; set; }
        public DateTime timeAtLobbyLoadedFirstTime { get; set; }
        public float timeSpentQuickMatch { get; set; }
        public float timeSpentLongMatch { get; set; }
        public float timeSpentCpuMatch { get; set; }
        public float timeSpentLobby { get; set; }
        public DateTime lastLaunchTime { get; set; }

        [PostConstruct]
        public void PostConstruct()
        {
            modelsResetSignal.AddListener(Reset);
            modelsLoadFromDiskSignal.AddListener(LoadFromDisk);
            modelsSaveToDiskSignal.AddListener(SaveToDisk);
        }

        private void Reset()
        {
            isAudioOn = true;
            adSlotId = 0;
            adSlotImpressions = 0;
            hasRated = false;
            isSafeMoveOn = false;
            isFriendScreenVisited = false;
            isCoachTooltipShown = false;
            isStrengthTooltipShown = false;
            isLobbyLoadedFirstTime = false;
            coachUsedCount = 0;
            strengthUsedCount = 0;
            promotionCycleIndex = 0;
            timeAtLobbyLoadedFirstTime = DateTime.Now;
            timeSpentQuickMatch = 0;
            timeSpentLongMatch = 0;
            timeSpentCpuMatch = 0;
            timeSpentLobby = 0;
            lastLaunchTime = TimeUtil.ToDateTime(backendService.serverClock.currentTimestamp);
        }

        private void LoadFromDisk()
        {
            if (!localDataService.FileExists(PrefKeys.PREFS_SAVE_FILENAME))
            {
                return;
            }

            try
            {
                ILocalDataReader reader = localDataService.OpenReader(PrefKeys.PREFS_SAVE_FILENAME);

                // Read preferences here
                isAudioOn = reader.Read<bool>(PrefKeys.AUDIO_STATE);

                if (reader.HasKey(PrefKeys.AD_SLOT_ID))
                {
                    adSlotId = reader.Read<long>(PrefKeys.AD_SLOT_ID);
                    adSlotImpressions = reader.Read<int>(PrefKeys.AD_SLOT_IMPRESSIONS);
                }

                if (reader.HasKey(PrefKeys.HAS_RATED))
                {
                    hasRated = reader.Read<bool>(PrefKeys.HAS_RATED);
                }

                if (reader.HasKey(PrefKeys.IS_SAFE_MOVE_ON))
                {
                    isSafeMoveOn = false;
                }

                if (reader.HasKey(PrefKeys.IS_FRIEND_SCREEN_VISITED))
                {
                    isFriendScreenVisited = reader.Read<bool>(PrefKeys.IS_FRIEND_SCREEN_VISITED);
                }

                if (reader.HasKey(PrefKeys.IS_COACH_TOOLTIP_SHOWN))
                {
                    isCoachTooltipShown = reader.Read<bool>(PrefKeys.IS_COACH_TOOLTIP_SHOWN);
                }

                if (reader.HasKey(PrefKeys.IS_STRENGTH_TOOLTIP_SHOWN))
                {
                    isStrengthTooltipShown = reader.Read<bool>(PrefKeys.IS_STRENGTH_TOOLTIP_SHOWN);
                }

                if (reader.HasKey(PrefKeys.IS_LOBBY_LOADED_FIRST_TIME))
                {
                    isLobbyLoadedFirstTime = reader.Read<bool>(PrefKeys.IS_LOBBY_LOADED_FIRST_TIME);
                }

                if (reader.HasKey(PrefKeys.COACH_USED_COUNT))
                {
                    coachUsedCount = reader.Read<int>(PrefKeys.COACH_USED_COUNT);
                }

                if (reader.HasKey(PrefKeys.STRENGTH_USED_COUNT))
                {
                    strengthUsedCount = reader.Read<int>(PrefKeys.STRENGTH_USED_COUNT);
                }

                if (reader.HasKey(PrefKeys.PROMOTION_CYCLE_INDEX))
                {
                    promotionCycleIndex = reader.Read<int>(PrefKeys.PROMOTION_CYCLE_INDEX);
                }

                if (reader.HasKey(PrefKeys.TIME_AT_LOBBY_LOADED_FIRST_TIME))
                {
                    timeAtLobbyLoadedFirstTime = DateTime.FromBinary(long.Parse(reader.Read<string>(PrefKeys.TIME_AT_LOBBY_LOADED_FIRST_TIME)));
                }

                if (reader.HasKey(PrefKeys.TIME_SPENT_CPU_MATCH))
                {
                    timeSpentCpuMatch = reader.Read<float>(PrefKeys.TIME_SPENT_CPU_MATCH);
                }

                if (reader.HasKey(PrefKeys.TIME_SPENT_LOBBY))
                {
                    timeSpentLobby = reader.Read<float>(PrefKeys.TIME_SPENT_LOBBY);
                }

                if (reader.HasKey(PrefKeys.TIME_SPENT_LONG_MATCH))
                {
                    timeSpentLongMatch = reader.Read<float>(PrefKeys.TIME_SPENT_LONG_MATCH);
                }

                if (reader.HasKey(PrefKeys.TIME_SPENT_QUICK_MATCH))
                {
                    timeSpentQuickMatch = reader.Read<float>(PrefKeys.TIME_SPENT_QUICK_MATCH);
                }

                if (reader.HasKey(PrefKeys.LAST_LAUNCH_TIME))
                {
                    lastLaunchTime = DateTime.FromBinary(long.Parse(reader.Read<string>(PrefKeys.LAST_LAUNCH_TIME)));
                }

                reader.Close();
            }
            catch (Exception e)
            {
                LogUtil.Log("Corrupt saved prefs! " + e, "red");
                localDataService.DeleteFile(PrefKeys.PREFS_SAVE_FILENAME);
                Reset();
            }
        }

        private void SaveToDisk()
        {
            try
            {
                ILocalDataWriter writer = localDataService.OpenWriter(PrefKeys.PREFS_SAVE_FILENAME);

                // Save preferences here
                writer.Write<bool>(PrefKeys.AUDIO_STATE, isAudioOn);
                writer.Write<long>(PrefKeys.AD_SLOT_ID, adSlotId);
                writer.Write<int>(PrefKeys.AD_SLOT_IMPRESSIONS, adSlotImpressions);
                writer.Write<bool>(PrefKeys.HAS_RATED, hasRated);
                writer.Write<bool>(PrefKeys.IS_SAFE_MOVE_ON, isSafeMoveOn);
                writer.Write<bool>(PrefKeys.IS_FRIEND_SCREEN_VISITED, isFriendScreenVisited);
                writer.Write<bool>(PrefKeys.IS_COACH_TOOLTIP_SHOWN, isCoachTooltipShown);
                writer.Write<bool>(PrefKeys.IS_STRENGTH_TOOLTIP_SHOWN, isStrengthTooltipShown);
                writer.Write<bool>(PrefKeys.IS_LOBBY_LOADED_FIRST_TIME, isLobbyLoadedFirstTime);
                writer.Write<int>(PrefKeys.COACH_USED_COUNT, coachUsedCount);
                writer.Write<int>(PrefKeys.STRENGTH_USED_COUNT, strengthUsedCount);
                writer.Write<int>(PrefKeys.PROMOTION_CYCLE_INDEX, promotionCycleIndex);
                writer.Write<string>(PrefKeys.TIME_AT_LOBBY_LOADED_FIRST_TIME, timeAtLobbyLoadedFirstTime.ToBinary().ToString());
                writer.Write<float>(PrefKeys.TIME_SPENT_QUICK_MATCH, timeSpentQuickMatch);
                writer.Write<float>(PrefKeys.TIME_SPENT_LONG_MATCH, timeSpentLongMatch);
                writer.Write<float>(PrefKeys.TIME_SPENT_LOBBY, timeSpentLobby);
                writer.Write<float>(PrefKeys.TIME_SPENT_CPU_MATCH, timeSpentCpuMatch);
                writer.Write<string>(PrefKeys.LAST_LAUNCH_TIME, lastLaunchTime.ToBinary().ToString());
                writer.Close();
            }
            catch (Exception e)
            {
                if (localDataService.FileExists(PrefKeys.PREFS_SAVE_FILENAME))
                {
                    localDataService.DeleteFile(PrefKeys.PREFS_SAVE_FILENAME);
                }

                LogUtil.Log("Critical error when saving prefs. File deleted. " + e, "red");
            }
        }

        public void UpdateTimeSpentAnalyticsData(AnalyticsEventId eventId, DateTime timeAtScreenShown)
        {
            var minutesSpent = (float)(TimeUtil.ToDateTime(backendService.serverClock.currentTimestamp) - timeAtScreenShown).TotalMinutes;

            LogUtil.Log("[eventId] : " + eventId + "   [minutesSpent] : "+ minutesSpent, "orange");

            if (eventId == AnalyticsEventId.time_spent_cpu_match)
            {
                timeSpentCpuMatch += minutesSpent;
                LogUtil.Log("timeSpentCpuMatch ###  " + timeSpentCpuMatch, "cyan");
            }
            else if (eventId == AnalyticsEventId.time_spent_long_match)
            {
                timeSpentLongMatch += minutesSpent;
                LogUtil.Log("timeSpentLongMatch ###  " + timeSpentLongMatch, "cyan");
            }
            else if (eventId == AnalyticsEventId.time_spent_quick_macth)
            {
                timeSpentQuickMatch += minutesSpent;
                LogUtil.Log("timeSpentQuickMatch ###  " + timeSpentQuickMatch, "cyan");
            }
            else if (eventId == AnalyticsEventId.time_spent_lobby)
            {
                timeSpentLobby += minutesSpent;
                LogUtil.Log("timeSpentLobby ###  " + timeSpentLobby, "cyan");
            }
        }

        public void ResetTimeSpentAnalyticsData()
        {
            lastLaunchTime = TimeUtil.ToDateTime(backendService.serverClock.currentTimestamp);

            timeSpentCpuMatch   = 0;
            timeSpentLongMatch  = 0;
            timeSpentQuickMatch = 0;
            timeSpentLobby      = 0;
        }
    }
}
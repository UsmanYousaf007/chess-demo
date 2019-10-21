﻿/// @license Propriety <http://license.url>
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
    }
}
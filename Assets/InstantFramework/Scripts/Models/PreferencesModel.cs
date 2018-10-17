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

        // Listen to signals
        [Inject] public AppEventSignal appEventSignal { get; set; }

        public bool isAudioOn { get; set; }
        public long adSlotId { get; set; }            
        public int adSlotImpressions { get; set; }   
        public bool hasRated { get; set; }  

        [PostConstruct]
        public void Load()
        {
            Reset();
            LoadFromFile();

            appEventSignal.AddListener(OnAppEvent);
        }

        private void LoadFromFile()
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
                

                reader.Close();
            }
            catch (Exception e)
            {
                LogUtil.Log("Corrupt saved prefs! " + e, "red");
                localDataService.DeleteFile(PrefKeys.PREFS_SAVE_FILENAME);
                Reset();
            }
        }

        private void SaveToFile()
        {
            try
            {
                ILocalDataWriter writer = localDataService.OpenWriter(PrefKeys.PREFS_SAVE_FILENAME);

                // Save preferences here
                writer.Write<bool>(PrefKeys.AUDIO_STATE, isAudioOn);
                writer.Write<long>(PrefKeys.AD_SLOT_ID, adSlotId);
                writer.Write<int>(PrefKeys.AD_SLOT_IMPRESSIONS, adSlotImpressions);
                writer.Write<bool>(PrefKeys.HAS_RATED, hasRated);

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

        private void Reset()
        {
            isAudioOn = true;
            adSlotId = 0;            
            adSlotImpressions = 0;   
            hasRated = false; 
        }

        private void OnAppEvent(AppEvent evt)
        {
            if (evt == AppEvent.QUIT || evt == AppEvent.PAUSED)
            {
                SaveToFile();
            }
        }
    }
}

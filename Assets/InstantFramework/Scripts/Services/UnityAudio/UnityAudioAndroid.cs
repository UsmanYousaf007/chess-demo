/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2018-01-10 10:52:49 UTC+05:00

using UnityEngine;
using System.Collections.Generic;

namespace TurboLabz.InstantFramework
{
    public class UnityAudioAndroid : IAudioService
    {
        [Inject] public IPreferencesModel preferencesModel { get; set; }

        [Inject] public AppEventSignal appEventSignal { get; set; }

        public AudioList sounds { get; set; }

        private const string OBJ_NAME = "AudioService";
        private const string FILE_EXT = ".wav";
        private Dictionary<string, int> streamFiles;

        public void Init()
        {
            appEventSignal.AddListener(OnAppEvent);
            sounds = GameObject.Find(OBJ_NAME).GetComponent<AudioList>();
            sounds.playStandardClickSignal.AddListener(PlayStandardClick);

            CreatePool(
                sounds.SFX_CAPTURE,
                sounds.SFX_CHECK,
                sounds.SFX_CLICK,
                sounds.SFX_DEFEAT,
                sounds.SFX_HINT,
                sounds.SFX_PLACE_PIECE,
                sounds.SFX_PROMO,
                sounds.SFX_STEP_CLICK,
                sounds.SFX_VICTORY,
                sounds.SFX_SHOP_PURCHASE_ITEM,
                sounds.SFX_TOOL_TIP,
                sounds.SFX_REWARD_UNLOCKED,
                sounds.SFX_CLOCK_WARNING,
                sounds.SFX_EFFECT_SLAM,
                sounds.SFX_EFFECT_CHEST_ACTIVATE,
                sounds.SFX_EFFECT_CHEST_SPEW,
                sounds.SFX_EFFECT_COIN_SPREAD,
                sounds.SFX_EFFECT_COIN_TRAVEL,
                sounds.SFX_EFFECT_COIN_FILL,
                sounds.SFX_EFFECT_GEM_SPREAD,
                sounds.SFX_EFFECT_GEM_TRAVEL,
                sounds.SFX_EFFECT_GEM_FILL
                //sounds.SFX_EFFECT_STAR_SPREAD,
                //sounds.SFX_EFFECT_STAR_TRAVEL,
                //sounds.SFX_EFFECT_STAR_FILL,
                //sounds.SFX_EFFECT_TROPHY_SPREAD,
                //sounds.SFX_EFFECT_TROPHY_TRAVEL,
                //sounds.SFX_EFFECT_TROPHY_FILL
            );
        }

        public void Play(AudioClip sound)
        {
            Play(sound,1.0f);
        }

        public void Play(AudioClip sound, float volume)
        {
            if (preferencesModel.isAudioOn)
            {
                AndroidNativeAudio.play(streamFiles[sound.name]);
            }
        }

        public void PlayStandardClick()
        {
            Play(sounds.SFX_CLICK);
        }

        public void ToggleAudio(bool state)
        {
            preferencesModel.isAudioOn = state;
        }
            
        public void OnAppEvent(AppEvent evt)
        {
            if (evt == AppEvent.QUIT)
            {
                foreach (KeyValuePair<string, int> entry in streamFiles)
                {
                    AndroidNativeAudio.unload(entry.Value);
                }

                AndroidNativeAudio.releasePool();
            }
        }

        public bool IsAudioOn()
        {
            return preferencesModel.isAudioOn;
        }

        private void CreatePool(params AudioClip[] clips)
        {
            AndroidNativeAudio.makePool(clips.Length);

            streamFiles = new Dictionary<string, int>();

            foreach (AudioClip clip in clips)
            {
                LoadSound(clip);
            }
        }

        private void LoadSound(AudioClip clip)
        {
            streamFiles.Add(clip.name, AndroidNativeAudio.load(clip.name + FILE_EXT));
        }
    }
}
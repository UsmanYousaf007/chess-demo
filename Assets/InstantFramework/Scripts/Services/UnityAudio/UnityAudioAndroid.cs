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
        [Inject] public AppEventSignal appEventSignal { get; set; }

        public AudioList sounds { get; set; }

        private bool audioOn;
        private const string OBJ_NAME = "AudioService";
        private const string FILE_EXT = ".wav";
        private Dictionary<string, int> streamFiles;

        [PostConstruct]
        public void PostConstruct()
        {
            appEventSignal.AddListener(OnAppEvent);
            sounds = GameObject.Find(OBJ_NAME).GetComponent<AudioList>();
            audioOn = true;
            sounds.playStandardClickSignal.AddListener(PlayStandardClick);

            AndroidNativeAudio.makePool(9); 
            streamFiles = new Dictionary<string, int>();
            LoadSound(sounds.SFX_CAPTURE);
            LoadSound(sounds.SFX_CHECK);
            LoadSound(sounds.SFX_CLICK);
            LoadSound(sounds.SFX_DEFEAT);
            LoadSound(sounds.SFX_HINT);
            LoadSound(sounds.SFX_PLACE_PIECE);
            LoadSound(sounds.SFX_PROMO);
            LoadSound(sounds.SFX_STEP_CLICK);
            LoadSound(sounds.SFX_VICTORY);
        }

        public void Play(AudioClip sound, float volume = 1.0f)
        {
            if (audioOn)
            {
                AndroidNativeAudio.play(streamFiles[sound.name]);
            }
        }

        public void PlayStandardClick()
        {
            Play(sounds.SFX_CLICK);
        }

        public void ToggleAudio(bool on)
        {
            audioOn = on;
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

        private void LoadSound(AudioClip clip)
        {
            streamFiles.Add(clip.name, AndroidNativeAudio.load(clip.name + FILE_EXT));
        }
    }
}
/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2018-01-10 10:52:49 UTC+05:00

using UnityEngine;

namespace TurboLabz.InstantFramework
{
    public class UnityAudio : IAudioService
    {
        public AudioList sounds { get; set; }

        private AudioSource audio;
        private const string OBJ_NAME = "AudioService";
        private bool audioOn;


        [PostConstruct]
        public void PostConstruct()
        {
            audio = GameObject.Find(OBJ_NAME).GetComponent<AudioSource>();
            sounds = audio.GetComponent<AudioList>();
            audioOn = true;

            sounds.playStandardClickSignal.AddListener(PlayStandardClick);
        }

        public void Play(AudioClip sound, float volume = 1.0f)
        {
            if (audioOn)
            {
                audio.PlayOneShot(sound, volume);
            }
        }

        public void PlayStandardClick()
        {
            Play(sounds.CLICK);
        }

        public void ToggleAudio(bool on)
        {
            audioOn = on;
        }
    }
}
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
        [Inject] public IPreferencesModel preferencesModel { get; set; }

        public AudioList sounds { get; set; }

        private AudioSource audio;
        private const string OBJ_NAME = "AudioService";

        public void Init()
        {
            sounds = GameObject.Find(OBJ_NAME).GetComponent<AudioList>();
            audio = sounds.audioSource;

            sounds.playStandardClickSignal.AddListener(PlayStandardClick);
        }

        public void Play(AudioClip sound)
        {
            Play(sound, 1.0f);
        }

        public void Play(AudioClip sound, float volume)
        {
            if (preferencesModel.isAudioOn)
            {
                audio.PlayOneShot(sound);
                //audio.Stop();
                //audio.clip = sound;
                //audio.Play();
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

        public bool IsAudioOn()
        {
            return preferencesModel.isAudioOn;
        }
    }
}
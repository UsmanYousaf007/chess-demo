/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2018-01-10 10:47:41 UTC+05:00
/// 
/// @description
/// [add_description_here]
using System.Collections.Generic;
using UnityEngine;

namespace TurboLabz.InstantFramework
{
    public interface IAudioService
    {
        AudioList sounds { get; set; }
        void Play(AudioClip sound, float volume = 1.0f);
        void ToggleAudio(bool on);
    }
}

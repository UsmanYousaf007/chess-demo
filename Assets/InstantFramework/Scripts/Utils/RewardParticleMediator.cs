/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2021 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System.Collections;
using System.Collections.Generic;
using strange.extensions.mediation.impl;
using TurboLabz.InstantFramework;
using UnityEngine;

public class RewardParticleMediator : Mediator
{
    //View Injection
    [Inject] public RewardParticle view {get; set;}

    //Services
    [Inject] public IAudioService audioService { get; set; }

    public override void OnRegister()
    {
        view.playSFXSignal.AddListener(OnPlaySFX);
    }

    private void OnPlaySFX(AudioClip clip)
    {
        audioService.PlayOneShot(clip);
    }
}

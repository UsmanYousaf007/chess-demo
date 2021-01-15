
/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2021 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine;
using DG.Tweening;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;

using System.Collections;
using System.Collections.Generic;
using strange.extensions.mediation.impl;
using TurboLabz.InstantFramework;
using UnityEngine;

public class RewardParticleEmitterMediator : Mediator
{
    //View Injection
    [Inject] public RewardParticleEmitter view { get; set; }

    //Services
    [Inject] public IAudioService audioService { get; set; }

    public override void OnRegister()
    {
        view.playSFXSignal.AddListener(OnPlaySFX);
    }

    private void OnPlaySFX(AudioClip clip)
    {
        audioService.Play(clip);
    }
}

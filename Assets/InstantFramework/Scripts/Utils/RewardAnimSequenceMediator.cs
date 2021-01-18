using System.Collections;
using System.Collections.Generic;
using strange.extensions.mediation.impl;
using TurboLabz.InstantFramework;
using UnityEngine;

public class RewardAnimSequenceMediator : Mediator
{
    //View Injection
    [Inject] public RewardAnimSequence view {get; set;}

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

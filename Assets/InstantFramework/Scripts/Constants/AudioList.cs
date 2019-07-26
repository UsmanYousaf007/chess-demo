/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2018-01-10 10:52:49 UTC+05:00
/// 
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using strange.extensions.signal.impl;

public class AudioList : MonoBehaviour
{
    public AudioSource audioSource;

    // Framework
    public AudioClip SFX_CLICK;
    public AudioClip SFX_STEP_CLICK;

    // Game
    public AudioClip SFX_CAPTURE;
    public AudioClip SFX_CHECK;
    public AudioClip SFX_DEFEAT;
    public AudioClip SFX_HINT;
    public AudioClip SFX_PLACE_PIECE;
    public AudioClip SFX_PROMO;
    public AudioClip SFX_VICTORY;
    public AudioClip SFX_SHOP_PURCHASE_ITEM;

    // Dispatch Signal
    public Signal playStandardClickSignal = new Signal();

    public void PlayStandardClick()
    {
        playStandardClickSignal.Dispatch();
    }
}

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
    public AudioClip SFX_TOOL_TIP;


    // Game
    public AudioClip SFX_CAPTURE;
    public AudioClip SFX_CHECK;
    public AudioClip SFX_DEFEAT;
    public AudioClip SFX_HINT;
    public AudioClip SFX_PLACE_PIECE;
    public AudioClip SFX_PROMO;
    public AudioClip SFX_VICTORY;
    public AudioClip SFX_SHOP_PURCHASE_ITEM;
    public AudioClip SFX_REWARD_UNLOCKED;
    public AudioClip SFX_CLOCK_WARNING;
    public AudioClip SFX_MOVE_DIAL_SELECTED;

    // Effects
    public AudioClip SFX_EFFECT_SLAM;
    public AudioClip SFX_EFFECT_CHEST_ACTIVATE;
    public AudioClip SFX_EFFECT_CHEST_SPEW;
    public AudioClip SFX_EFFECT_COIN_SPREAD;
    public AudioClip SFX_EFFECT_COIN_TRAVEL;
    public AudioClip SFX_EFFECT_COIN_FILL;
    public AudioClip SFX_EFFECT_GEM_SPREAD;
    public AudioClip SFX_EFFECT_GEM_TRAVEL;
    public AudioClip SFX_EFFECT_GEM_FILL;
    public AudioClip SFX_EFFECT_STAR_SPREAD;
    public AudioClip SFX_EFFECT_STAR_TRAVEL;
    public AudioClip SFX_EFFECT_STAR_FILL;
    public AudioClip SFX_EFFECT_TROPHY_SPREAD;
    public AudioClip SFX_EFFECT_TROPHY_TRAVEL;
    public AudioClip SFX_EFFECT_TROPHY_FILL;
    public AudioClip SFX_EFFECT_RING_SLAM;

    // Dispatch Signal
    public Signal playStandardClickSignal = new Signal();

    public void PlayStandardClick()
    {
        playStandardClickSignal.Dispatch();
    }
}

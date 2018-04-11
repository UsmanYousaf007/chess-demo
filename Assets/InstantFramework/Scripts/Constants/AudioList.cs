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

public class AudioList : MonoBehaviour {

    // Framework
    public AudioClip CLICK;

    // Game
    public AudioClip CAPTURE;
    public AudioClip CHECK;
    public AudioClip DEFEAT;
    public AudioClip HINT;
    public AudioClip PLACE_PIECE;
    public AudioClip PROMO;
    public AudioClip VICTORY;
    public AudioClip STEP_CLICK;

    // Dispatch Signal
    public Signal playStandardClickSignal = new Signal();

    public void PlayStandardClick()
    {
        playStandardClickSignal.Dispatch();
    }
}

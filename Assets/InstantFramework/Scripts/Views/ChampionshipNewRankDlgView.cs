/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2020 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine;
using UnityEngine.UI;

using TMPro;
using System;
using DG.Tweening;
using TurboLabz.InstantGame;
using System.Collections.Generic;
using strange.extensions.signal.impl;
using strange.extensions.mediation.impl;

namespace TurboLabz.InstantFramework
{
    [CLSCompliant(false)]
    public class ChampionshipNewRankDlgView : ChampionshipResultDlgView
    {
        // Set in Inspector
        [SerializeField] private TextMeshProUGUI timerText;

        public override void Init()
        {
            championshipBarsPool = new GameObjectsPool(championshipLeaderboardPlayerBarPrefab, 50);

            continueButton.onClick.AddListener(() => continueBtnClickedSignal.Dispatch());
        }
    }
}
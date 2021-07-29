/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
using UnityEngine;
using UnityEngine.UI;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using TurboLabz.TLUtils;
using System;
using TMPro;
using System.Collections.Generic;
using System.Text;
using TurboLabz.InstantGame;
using TurboLabz.CPU;
using System.Collections;

namespace TurboLabz.InstantFramework
{
    [CLSCompliant(false)]
    public class LessonCardMediator : Mediator
    {
        // View injection
        [Inject] public LessonCardView view { get; set; }

        //Dispatch signals
        [Inject] public LoadLessonsViewSignal loadLessonsViewSignal { get; set; }

        // Models
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IAppInfoModel appInfoModel { get; set; }

        //Services
        [Inject] public IPromotionsService promotionsService { get; set; }

        public override void OnRegister()
        {
            view.Init();
            view.viewAllButtonClickedSignal.AddListener(OnViewAllButtonClickedSignal);
        }

        private void OnViewAllButtonClickedSignal()
        {
            loadLessonsViewSignal.Dispatch();
        }
    }
}
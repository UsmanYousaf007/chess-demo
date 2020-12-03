﻿/// @license Propriety <http://license.url>
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
    public class LessonCardView : View
    {
        public TMP_Text subTitle;

        public Image currentLessonIcon;
        public TMP_Text currentLessonName;
        public TMP_Text currentTopic;

        public TMP_Text startButtonText;
        public Button startButton;

        //Services
        [Inject] public ILocalizationService localizationService { get; set; }
        [Inject] public IAudioService audioService { get; set; }

        public void Init()
        {
            
        }

        public void UpdateView(TopicsViewVO vo)
        {
            //lessonsCompletedSection.SetActive(vo.allLessonsWatched);
            /*lessonVO = vo.nextLesson;

            if (!vo.allLessonsWatched)
            {
                currentLessonIcon.sprite = vo.nextLesson.icon;
                currentLessonName.text = vo.nextLesson.name;
                nextLessonProgress.fillAmount = vo.nextLesson.progress;
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(categoryContainer.GetComponent<RectTransform>());*/
        }
    }
}

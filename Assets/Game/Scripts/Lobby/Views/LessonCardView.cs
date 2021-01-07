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
    public class LessonCardView : View
    {
        public TMP_Text subTitle;
        public TMP_Text currentLessonName;
        public TMP_Text startButtonText;
        public TMP_Text duration;
        public Button viewAllButton;
        public Button startButton;

        private VideoLessonVO lessonVO;
        public GameObject lessonsCompletedSection;
        public GameObject lessonsSection;
        public GameObject processing;
        public GameObject lessonLocked;
        public Text lessonLockedName;
        public Text lessonLockedDuration;
        public Text lessonLockedCost;
        public Button lessonLockedClose;
        public Button lessonLockedBuy;

        public string NextLessonId { get { return lessonVO != null ? lessonVO.videoId : string.Empty; } }
        public int LessonCost { get { return lessonVO.storeItem.currency3Cost; } }

        //Services
        [Inject] public ILocalizationService localizationService { get; set; }
        [Inject] public IAudioService audioService { get; set; }

        //Signals
        public Signal<VideoLessonVO> startButtonClickedSignal = new Signal<VideoLessonVO>();
        public Signal viewAllButtonClickedSignal = new Signal();
        public Signal<string> buyLessonClickedSingal = new Signal<string>();

        public void Init()
        {
            startButton.onClick.AddListener(OnStartButtonClicked);
            viewAllButton.onClick.AddListener(OnViewAllButtonClicked);
            lessonLockedClose.onClick.AddListener(() => lessonLocked.SetActive(false));
            lessonLockedBuy.onClick.AddListener(OnBuyLessonButtonClickedSignal);
            lessonLocked.SetActive(false);
        }

        public void UpdateView(VideoLessonVO vo, bool allLessonsWatched)
        {
            lessonsCompletedSection.SetActive(allLessonsWatched);
            lessonsSection.SetActive(!allLessonsWatched);
            lessonVO = vo;

            if (!allLessonsWatched)
            {
                currentLessonName.text = lessonVO.name;
                duration.text = TimeUtil.FormatSecondsToMinutes(lessonVO.duration);
                lessonLockedName.text = lessonVO.name;
                lessonLockedDuration.text = duration.text;
                lessonLockedCost.text = lessonVO.storeItem.currency3Cost.ToString();
            }
        }

        void OnStartButtonClicked()
        {
            audioService.PlayStandardClick();
            PlayNextLesson();
        }

        void OnViewAllButtonClicked()
        {
            audioService.PlayStandardClick();
            viewAllButtonClickedSignal.Dispatch();
        }

        void OnBuyLessonButtonClickedSignal()
        {
            audioService.PlayStandardClick();
            buyLessonClickedSingal.Dispatch(lessonVO.storeItem.key);
        }

        public void UnlockNextLesson()
        {
            if (lessonVO != null)
            {
                lessonVO.isLocked = false;
            }
        }

        public void PlayNextLesson()
        {
            startButtonClickedSignal.Dispatch(lessonVO);
        }
    }
}

/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;
using TurboLabz.TLUtils;
using TurboLabz.InstantFramework;
using strange.extensions.signal.impl;
using strange.extensions.mediation.impl;

namespace TurboLabz.InstantGame
{
    public class LessonsVideoPlayerView : View
    {
        #region UI
        public Image titleIconImage;
        public Text titleText;
        public Button _backButton;
        public Button nextVideoButton;
        public GameObject processing;
        public RectTransform layoutGroup;
        public GameObject buffering;

        [SerializeField] private RectTransform _videoScreen;
        [SerializeField] private Button _playPauseButton;
        [SerializeField] private RectTransform _playIconAndBg;
        [SerializeField] private TextMeshProUGUI _timeText;
        #endregion

        [Inject] public IVideoPlaybackService videoPlaybackService { get; set; }

        // Signals
        public Signal PlayVideoSignal = new Signal();
        public Signal PauseVideoSignal = new Signal();
        public Signal SeekStartedVideoSignal = new Signal();
        public Signal<float> SeekEndVideoSignal = new Signal<float>();

        private bool paused = false;
        private SeekSlider seekSlider;

        #region Monobehavior Methods
        void LateUpdate()
        {
            if (videoPlaybackService != null && videoPlaybackService.isPlaying)
            {
                seekSlider.UpdateSliderPosition(GetProgress());
            }
        }
        #endregion

        #region public methods
        public void Init()
        {
            SetTimeTextVisibility(false);
            _playIconAndBg.gameObject.SetActive(false);
            buffering.SetActive(true);

            seekSlider = GetComponentInChildren<SeekSlider>(true);
            seekSlider.Init(OnSliderSeekStartEvent, OnSliderSeekEndEvent);
            PositionSeekSlider();

            ResizePlayButtonBg();

            _playPauseButton.onClick.AddListener(PlayPauseButtonClickHandler);
        }

        public void Show()
        {
            gameObject.SetActive(true);
            UpdateView();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void SetTimeTextVisibility(bool show)
        {
            if (show)
            {
                UpdateTimeText();
            }

            _timeText.gameObject.SetActive(show);
        }

        public void UpdateTimeText()
        {
            TimeSpan currentTime = TimeSpan.FromMilliseconds(seekSlider.GetNormalizedValue() * videoPlaybackService.duration);
            TimeSpan totalTime = TimeSpan.FromMilliseconds(videoPlaybackService.duration);
            _timeText.text = TimeUtil.MillisecondsToMinutesAndSeconds(currentTime) + " / " + TimeUtil.MillisecondsToMinutesAndSeconds(totalTime);
        }

        public void Reset()
        {
            SetTimeTextVisibility(false);
            _playIconAndBg.gameObject.SetActive(false);
            buffering.SetActive(true);
            seekSlider.UpdateSliderPosition(0f);
            paused = false;
        }
        #endregion

        #region private methods
        private void ResizePlayButtonBg()
        {
            float newSize = _videoScreen.rect.width < _videoScreen.rect.height ? _videoScreen.rect.width : _videoScreen.rect.height;
            _playIconAndBg.sizeDelta = new Vector2(newSize, newSize);
        }

        private void PositionSeekSlider()
        {
            Vector3 sliderPosition = seekSlider.transform.localPosition;
            float newY = _videoScreen.rect.width < _videoScreen.rect.height ? _videoScreen.rect.width : _videoScreen.rect.height;
            seekSlider.transform.localPosition = new Vector3(sliderPosition.x, -newY/2f, sliderPosition.z);
        }

        private float GetProgress()
        {
            if (videoPlaybackService != null)
            {
                return videoPlaybackService.GetProgress();
            }

            return 0;
        }

        private void PlayPauseButtonClickHandler()
        {
            if (paused)
            {
                PlayVideoSignal.Dispatch();
            }
            else
            {
                PauseVideoSignal.Dispatch();
            }

            paused = !paused;

            SetTimeTextVisibility(paused);
            _playIconAndBg.gameObject.SetActive(paused);
        }
        #endregion

        #region Slider Events
        private void OnSliderSeekStartEvent()
        {
            SeekStartedVideoSignal.Dispatch();
        }

        private void OnSliderSeekEndEvent(float seek)
        {
            SeekEndVideoSignal.Dispatch(seek);
        }
        #endregion

        public void UpdateView()
        {
            if (isActiveAndEnabled)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroup);
            }
        }
    }
}

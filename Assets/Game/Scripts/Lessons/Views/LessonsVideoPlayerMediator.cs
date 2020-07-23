/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine;
using TurboLabz.InstantGame;
using strange.extensions.mediation.impl;

namespace TurboLabz.InstantFramework
{
    public class LessonsVideoPlayerMediator : Mediator
    {
        // View injection
        [Inject] public LessonsVideoPlayerView view { get; set; }

        // Models
        [Inject] public IPlayerModel playerModel { get; set; }

        // Services
        [Inject] public IVideoPlaybackService videoPlaybackService { get; set; }
        [Inject] public IAudioService audioService { get; set; }

        // Dispatch Signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public SavePlayerInventorySignal savePlayerInventorySignal { get; set; }
        [Inject] public SaveLastWatchedVideoSignal saveLastWatchedVideoSignal { get; set; }

        private bool videoPaused = false;
        private string videoId;

        public override void OnRegister()
        {
            view.Init();

            view.PlayVideoSignal.AddListener(PlayVideo);
            view.PauseVideoSignal.AddListener(PauseVideo);
            view.SeekStartedVideoSignal.AddListener(OnSeekStarted);
            view.SeekEndVideoSignal.AddListener(SeekTime);
            view._backButton.onClick.AddListener(OnBackButtonClicked);

        }

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.LESSON_VIDEO)
            {
                view.Show();
                PlayVideo();

                if (!string.IsNullOrEmpty(videoId) && playerModel.lastWatchedVideo != videoId)
                {
                    playerModel.lastWatchedVideo = videoId;
                    saveLastWatchedVideoSignal.Dispatch(videoId);
                }
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.LESSON_VIDEO)
            {
                videoPlaybackService.Stop();
                view.Reset();
                view.Hide();
            }
        }

        [ListensTo(typeof(UpdateVideoLessonViewSignal))]
        public void UpdateView(VideoLessonVO vo)
        {
            view.titleIconImage.sprite = vo.icon;
            view.titleText.text = vo.name;
            videoId = vo.videoId;
        }

        [ListensTo(typeof(VideoEventSignal))]
        public void VideoEventListener(VideoEvent videoEvent)
        {
            switch (videoEvent)
            {
                case VideoEvent.FinishedSeeking:
                    if (!videoPaused)
                    {
                        PlayVideo();
                    }
                    else
                    {
                        view.UpdateTimeText();
                    }

                    break;
                case VideoEvent.FinishedPlaying:
                    // Save video to active inventory
                    if (!string.IsNullOrEmpty(videoId))
                    {
                        VideoActiveInventoryItem videoInventoryItem = new VideoActiveInventoryItem(videoId, GSBackendKeys.ShopItem.VIDEO_LESSON_SHOP_TAG, 100f);
                        savePlayerInventorySignal.Dispatch(JsonUtility.ToJson(videoInventoryItem));

                        playerModel.UpdateVideoProgress(videoId, 100f);
                    }

                    break;
            }
        }

        private void OnSeekStarted()
        {
            if (!videoPaused)
            {
                videoPlaybackService.Pause();
            }
        }

        private void PlayVideo()
        {
            if (!videoPlaybackService.isPlaying)
            {
                videoPlaybackService.Play();
                videoPaused = false;
            }
        }

        private void PauseVideo()
        {
            if (videoPlaybackService.isPlaying)
            {
                videoPlaybackService.Pause();
                videoPaused = true;
            }
        }

        private void SeekTime(float nTime)
        {
            if (!videoPlaybackService.isPrepared) return;

            nTime = Mathf.Clamp(nTime, 0f, 1f);
            videoPlaybackService.Seek(nTime * videoPlaybackService.duration);
        }

        private void OnBackButtonClicked()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.ESCAPE);
            audioService.PlayStandardClick();
        }
    }
}

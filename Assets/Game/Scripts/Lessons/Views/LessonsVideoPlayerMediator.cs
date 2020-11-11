/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine;
using TurboLabz.TLUtils;
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
        [Inject] public IPreferencesModel preferencesModel { get; set; }
        [Inject] public IAppInfoModel appInfoModel { get; set; }

        // Services
        [Inject] public IVideoPlaybackService videoPlaybackService { get; set; }
        [Inject] public IAudioService audioService { get; set; }
        [Inject] public IAnalyticsService analyticsService { get; set; }
        [Inject] public IPromotionsService promotionsService { get; set; }

        // Dispatch Signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public SavePlayerInventorySignal savePlayerInventorySignal { get; set; }
        [Inject] public SaveLastWatchedVideoSignal saveLastWatchedVideoSignal { get; set; }
        [Inject] public LoadVideoSignal loadVideoSignal { get; set; }
        [Inject] public LoadTopicsViewSignal loadTopicsViewSignal { get; set; }
        [Inject] public SetSubscriptionContext setSubscriptionContext { get; set; }

        // Listeners
        [Inject] public SubscriptionDlgClosedSignal subscriptionDlgClosedSignal { get; set; }

        private bool videoPaused = false;
        private bool buffering = false;
        private string videoId = string.Empty;
        private int lessonIndex = 0;
        private VideoLessonVO nextVideo;

        public override void OnRegister()
        {
            view.Init();

            view.PlayVideoSignal.AddListener(PlayVideo);
            view.PauseVideoSignal.AddListener(PauseVideo);
            view.SeekStartedVideoSignal.AddListener(OnSeekStarted);
            view.SeekEndVideoSignal.AddListener(SeekTime);
            view._backButton.onClick.AddListener(OnBackButtonClicked);
            view.nextVideoButton.onClick.AddListener(OnNextVideoButtonClicked);
        }

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.LESSON_VIDEO)
            {
                view.Show();
                PlayVideo();
                analyticsService.ScreenVisit(AnalyticsScreen.lessons_play);
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.LESSON_VIDEO)
            {
                videoPlaybackService.Close();
                view.Reset();
                view.Hide();
            }
        }

        [ListensTo(typeof(UpdateVideoLessonViewSignal))]
        public void UpdateView(LessonPlayVO vo)
        {
            view.titleIconImage.sprite = vo.currentLesson.icon;
            view.titleIconImage.SetNativeSize();
            view.titleText.text = vo.currentLesson.name;
            videoId = vo.currentLesson.videoId;
            nextVideo = vo.nextLesson;
            lessonIndex = vo.currentLesson.overallIndex;
            view.UpdateView();
        }

        [ListensTo(typeof(VideoEventSignal))]
        public void VideoEventListener(VideoEvent videoEvent)
        {
            LogUtil.Log($"VideoEventListener {videoEvent}", "green");
            switch (videoEvent)
            {
                case VideoEvent.FirstFrameReady:
                    if (view.isActiveAndEnabled)
                    {
                        analyticsService.Event($"lesson_{lessonIndex}", AnalyticsContext.started);
                    }
                    break;

                case VideoEvent.ReadyToPlay:
                    if (view.isActiveAndEnabled)
                    {
                        PlayVideo();
                    }
                    break;

                case VideoEvent.Started:
                    if (view.isActiveAndEnabled)
                    {
                        view.buffering.SetActive(buffering);
                        view.processing.SetActive(false);
                        appInfoModel.isVideoLoading = false;
                    }
                    break;

                case VideoEvent.StartedSeeking:
                case VideoEvent.StartedBuffering:
                    buffering = true;
                    view.buffering.SetActive(true);
                    break;

                case VideoEvent.FinishedBuffering:
                case VideoEvent.FinishedSeeking:
                    buffering = false;
                    view.buffering.SetActive(false);
                    if (!videoPaused)
                    {
                        PlayVideo();
                    }
                    break;

                case VideoEvent.FinishedPlaying:
                    if (!string.IsNullOrEmpty(videoId))
                    {
                        //Analytics
                        analyticsService.Event($"lesson_{lessonIndex}", AnalyticsContext.completed);

                        //next video will be null if all videos are watched
                        if (nextVideo == null && !preferencesModel.isAllLessonsCompleted)
                        {
                            preferencesModel.isAllLessonsCompleted = true;
                            analyticsService.Event(AnalyticsEventId.all_lessons_complete);
                        }
                        //Analytics End

                        // Save video progress to active inventory
                        if (playerModel.GetVideoProgress(videoId) < 100f)
                        {
                            VideoActiveInventoryItem videoInventoryItem = new VideoActiveInventoryItem(videoId, GSBackendKeys.ShopItem.VIDEO_LESSON_SHOP_TAG, 100f);
                            savePlayerInventorySignal.Dispatch(JsonUtility.ToJson(videoInventoryItem));
                            playerModel.UpdateVideoProgress(videoId, 100f);
                        }

                        // Updating last watched video
                        if (playerModel.lastWatchedVideo != videoId)
                        {
                            saveLastWatchedVideoSignal.Dispatch(videoId);
                            playerModel.lastWatchedVideo = videoId;
                        }
                    }
                    break;
            }
        }

        [ListensTo(typeof(VideoLoadFailedSignal))]
        public void OnLessonLoadFailed()
        {
            if (view.isActiveAndEnabled)
            {
                view.processing.SetActive(false);
                appInfoModel.isVideoLoading = false;
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

            view.UpdateTimeText();

            if (!videoPlaybackService.isSeeking)
            {
                VideoEventListener(VideoEvent.FinishedSeeking);
            }
        }

        private void OnBackButtonClicked()
        {
            videoPlaybackService.Close();

            navigatorEventSignal.Dispatch(NavigatorEvent.ESCAPE);
            audioService.PlayStandardClick();
        }

        private void OnNextVideoButtonClicked()
        {
            audioService.PlayStandardClick();

            //next video will be null if all videos are watched
            if (nextVideo == null)
            {
                videoPlaybackService.Close();
                view.Reset();
                loadTopicsViewSignal.Dispatch();
            }
            else if (nextVideo.isLocked)
            {
                setSubscriptionContext.Dispatch($"lessons_{nextVideo.section.ToLower().Replace(' ', '_')}");
                videoPlaybackService.Pause();
                subscriptionDlgClosedSignal.AddOnce(OnSubscriptionDlgClosed);
                promotionsService.LoadSubscriptionPromotion();
            }
            else
            {
                view.processing.SetActive(true);
                appInfoModel.isVideoLoading = true;
                videoPlaybackService.Close();
                view.Reset();
                loadVideoSignal.Dispatch(nextVideo);
            }
        }

        [ListensTo(typeof(UpdatePurchasedStoreItemSignal))]
        public void OnSubscriptionPurchased(StoreItem item)
        {
            if (nextVideo != null &&
               (item.kind.Equals(GSBackendKeys.ShopItem.SUBSCRIPTION_TAG) || item.key.Equals(GSBackendKeys.ShopItem.ALL_LESSONS_PACK)))
            {
                nextVideo.isLocked = false;
            }
        }

        private void OnSubscriptionDlgClosed()
        {
            if (!videoPaused)
            {
                PlayVideo();
            }
        }
    }
}

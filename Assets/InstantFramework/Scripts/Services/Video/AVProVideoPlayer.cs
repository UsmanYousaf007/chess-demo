using System;
using UnityEngine;
using TurboLabz.TLUtils;
using RenderHeads.Media.AVProVideo;

namespace TurboLabz.InstantFramework
{
    public class AVProVideoPlayer : IVideoPlaybackService
    {
        private const string OBJ_NAME = "AVProVideoPlayer";

        // Dispatch Events
        [Inject] public VideoEventSignal videoEventSignal { get; set; }

        private MediaPlayer _avProPlayer;

        public bool isPlaying => _avProPlayer.Control.IsPlaying();
        public bool isPrepared => _avProPlayer.Control.CanPlay();
        public float duration => _avProPlayer.Info.GetDurationMs();
        public float playbackSpeed { get => _avProPlayer.m_PlaybackRate; set => _avProPlayer.Control.SetPlaybackRate(value); }
        public string url { get => _avProPlayer.m_VideoPath; set => _avProPlayer.m_VideoPath = value; }
        public double time => _avProPlayer.Control.GetCurrentTimeMs();

        public void Init()
        {
            try
            {
                _avProPlayer = GameObject.Find(OBJ_NAME).GetComponent<MediaPlayer>();
                _avProPlayer.Events.AddListener(OnVideoEvent);
            }
            catch (Exception e)
            {
                LogUtil.LogError(OBJ_NAME + " not found in the scene!");
            }
        }

        public void Prepare(string url)
        {
            this.url = url;
            _avProPlayer.OpenVideoFromFile(MediaPlayer.FileLocation.AbsolutePathOrURL, url, false);
        }

        public void Pause()
        {
            _avProPlayer.Pause();
        }

        public void Play()
        {
            _avProPlayer.Control.Play();
        }

        public void Stop()
        {
            _avProPlayer.Stop();
        }

        public void Seek(float time)
        {
            _avProPlayer.Control.Seek(time);
        }

        public float GetProgress()
        {
            return _avProPlayer.Control.GetCurrentTimeMs() / _avProPlayer.Info.GetDurationMs();
        }

        public void OnVideoEvent(MediaPlayer mediaPlayer, MediaPlayerEvent.EventType eventType, ErrorCode errorCode)
        {
            switch (eventType)
            {
                case MediaPlayerEvent.EventType.ReadyToPlay:
                    videoEventSignal.Dispatch(VideoEvent.ReadyToPlay);
                    break;
                case MediaPlayerEvent.EventType.FirstFrameReady:
                    videoEventSignal.Dispatch(VideoEvent.FirstFrameReady);
                    break;
                case MediaPlayerEvent.EventType.FinishedPlaying:
                    videoEventSignal.Dispatch(VideoEvent.FinishedPlaying);
                    break;
                case MediaPlayerEvent.EventType.FinishedSeeking:
                    videoEventSignal.Dispatch(VideoEvent.FinishedSeeking);
                    break;
            }
        }
    }
}
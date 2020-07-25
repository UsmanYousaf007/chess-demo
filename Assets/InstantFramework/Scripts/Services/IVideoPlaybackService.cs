/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System;
using UnityEngine;

namespace TurboLabz.InstantFramework
{
    public interface IVideoPlaybackService
    {
        bool isPrepared { get; }
        bool isPlaying { get; }
        bool isSeeking { get; }
        bool isBuffering { get; }
        float duration { get; }
        double time { get; }
        float playbackSpeed { get; set; }
        string url { get; set; }

        void Init();
        void Prepare(string url);
        void Stop();
        void Play();
        void Pause();
        void Seek(float time);
        float GetProgress();
    }
}


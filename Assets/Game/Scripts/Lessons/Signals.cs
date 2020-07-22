﻿/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using strange.extensions.signal.impl;
using TurboLabz.InstantFramework;

namespace TurboLabz.InstantGame
{
    public class VideoLoadFailedSignal : Signal { }
    public class UpdateVideoLessonViewSignal : Signal<VideoLessonVO> { }
    public class ShowVideoLessonSignal : Signal { }
}

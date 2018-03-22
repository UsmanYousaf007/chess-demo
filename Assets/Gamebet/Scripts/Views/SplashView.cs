/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-11-20 04:07:23 UTC+05:00
/// 
/// @description
/// [add_description_here]

using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;

namespace TurboLabz.Gamebet
{
    public class SplashView : View
    {
        public Signal splashAnimationCompletedSignal = new Signal();

        public void Init()
        {
        }

        public void OnSplashAnimationComplete()
        {
            splashAnimationCompletedSignal.Dispatch();
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}

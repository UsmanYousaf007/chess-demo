/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential


using UnityEngine;

using strange.extensions.signal.impl;
using TurboLabz.TLUtils;

namespace TurboLabz.Chess
{
    public class TapOff : MonoBehaviour
    {
        public Signal tapOffSignal = new Signal();
     
        void OnMouseDown()
        {
            tapOffSignal.Dispatch();
        }       
    }
}
/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-01-06 19:57:23 UTC+05:00
/// 
/// @description
/// [add_description_here]

using UnityEngine;

using strange.extensions.signal.impl;
using TurboLabz.TLUtils;

namespace TurboLabz.Chess
{
    public class ChessboardSquare : MonoBehaviour
    {
        public FileRank fileRank;
        public Signal<FileRank, Vector3> mouseDownSignal = new Signal<FileRank, Vector3>(); 
        public Signal<Vector3> mouseDragSignal = new Signal<Vector3>();
        public Signal<FileRank, Vector3> mouseEnterSignal = new Signal<FileRank, Vector3>();
        public Signal mouseUpSignal = new Signal();
        
        void OnMouseDown()
        {
            mouseDownSignal.Dispatch(fileRank, Input.mousePosition);
        }

        void OnMouseDrag()
        {
            mouseDragSignal.Dispatch(Input.mousePosition);
        }

        void OnMouseEnter()
        {
            mouseEnterSignal.Dispatch(fileRank, transform.position);
        }

        // Note: OnMouseUp will also get called in case a manual Sleep is performd while 
        // user is dragging on the mobile device screen. This means that a successful chess
        // piece move may occur right before the device goes into Sleep.
        void OnMouseUp()
        {
            mouseUpSignal.Dispatch();
        }
    }
}
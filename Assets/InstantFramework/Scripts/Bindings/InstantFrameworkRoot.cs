/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-09-06 23:15:44 UTC+05:00
/// 
/// @description
/// This is the root of the strange app.

using UnityEngine;

using strange.extensions.context.impl;

namespace TurboLabz.InstantFramework
{
    public class InstantFrameworkRoot : ContextView
    {
        [Inject] public SetErrorAndHaltSignal setErrorAndHaltSignal { get; set; }

        void Awake()
        {
            context = new InstantFrameworkContext(this);
        }

        void OnEnable()
        {
            Application.logMessageReceived += HandleLog;
        }

        void OnDisable()
        {
            Application.logMessageReceived -= HandleLog;
        }

        void HandleLog(string logString, string stackTrace, LogType type)
        { 
            if (type == LogType.Exception)
            {
                Debug.Log("GAME CRASHEDDDDDDDD @@@@@@@@@@@@@@@@@@@@@ ");

                string lString = logString.Replace('"', '\'').Replace('\n', ' ').Replace('\r', ' ');
                string sTrace = stackTrace.Replace('"', '\'').Replace('\n', ' ').Replace('\r', ' ');

                string _message = "<color=#ff0000>" + lString+"</color>" + " " + sTrace;
                if (_message.Length > 8192)
                {
                    _message = _message.Substring(8192);
                }

                setErrorAndHaltSignal.Dispatch(BackendResult.GAME_CRAHSED, _message);
            }
        }
    }

    
}

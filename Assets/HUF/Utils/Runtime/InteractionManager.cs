using System;
using HUF.Utils.Runtime.Extensions;
using JetBrains.Annotations;
using UnityEngine;

namespace HUF.Utils.Runtime
{
    public class InteractionManager : HSingleton<InteractionManager>
    {
#pragma warning disable 0067
        /// <summary>
        /// Raised when the back button is pressed on the device.
        /// </summary>
        [PublicAPI]
        public event Action OnBackButtonPress;
#pragma warning restore 0067

        void Update()
        {
#if UNITY_ANDROID || UNITY_EDITOR || UNITY_STANDALONE || UNITY_STANDALONE_OSX
            if ( Input.GetKeyDown( KeyCode.Escape ) )
                OnBackButtonPress.Dispatch();
#endif
        }
    }
}
using System;
using System.Collections;
using HUF.Utils;
using HUF.Utils.Runtime;
using HUF.Utils.Runtime.Extensions;
using JetBrains.Annotations;
using UnityEngine;

namespace HUF.GameServer.Runtime.Utils {
    internal class RequestBuffer<T>
    {
        const float OVERLOAD_TIMEOUT = 3f;
        const float OVERLOAD_MARGIN = 2f;
        const int OVERLOAD_THRESHOLD = 2;

        readonly Func<T, T, T> combiner;

        T callbacks;

        Action<T> action;

        bool hasTask;
        float lastQueue;
        int consecutiveOverloads;
        bool isTimedOut;

        IEnumerator timeoutRoutine;

        public RequestBuffer( Func<T, T, T> combiner )
        {
            this.combiner = combiner;
            timeoutRoutine = new WaitForSecondsRealtime( OVERLOAD_TIMEOUT );
        }

        public void Set( Action<T> action, [NotNull] T callback )
        {
            this.action = action;
            callbacks = combiner( callbacks, callback );

            if ( hasTask )
                return;

            QueueTask();
        }

        void QueueTask()
        {
            hasTask = true;
            OverloadCheck();
            lastQueue = Time.realtimeSinceStartup;
            CoroutineManager.StartCoroutine( BufferingRoutine() );
        }

        void OverloadCheck()
        {
            float now = Time.realtimeSinceStartup;

            if ( now - lastQueue < OVERLOAD_MARGIN )
            {
                consecutiveOverloads++;

                if ( consecutiveOverloads > OVERLOAD_THRESHOLD )
                    isTimedOut = true;
            }
            else
            {
                consecutiveOverloads = 0;
                isTimedOut = false;
            }
        }

        IEnumerator BufferingRoutine()
        {
            if ( isTimedOut )
                yield return timeoutRoutine;
            else
                yield return null;

            action.Dispatch( callbacks );
            callbacks = default;
            hasTask = false;
        }
    }
}
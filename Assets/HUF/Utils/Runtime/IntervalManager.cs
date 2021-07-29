using System;
using System.Collections.Generic;
using HUF.Utils.Runtime.Extensions;
using JetBrains.Annotations;
using UnityEngine;

namespace HUF.Utils.Runtime
{
    [DefaultExecutionOrder( -31800 )]
    public class IntervalManager : HSingleton<IntervalManager>
    {
        static float unscaledTime;
        bool everyFrameEventIsNull = true;
        bool everySecondEventIsNull = true;
        int secondCount = 0;
        List<DelayedAction> delayedActions = new List<DelayedAction>();

        /// <summary>
        /// Raised every frame.
        /// </summary>
        [PublicAPI]
        public event Action EveryFrame
        {
            add
            {
                everyFrame += value;
                everyFrameEventIsNull = everyFrame == null;
            }
            remove
            {
                everyFrame -= value;
                everyFrameEventIsNull = everyFrame == null;
            }
        }

        /// <summary>
        /// Raised every second.
        /// </summary>
        [PublicAPI]
        public event Action EverySecond
        {
            add
            {
                everySecond += value;
                everySecondEventIsNull = everySecond == null;
            }
            remove
            {
                everySecond -= value;
                everySecondEventIsNull = everySecond == null;
            }
        }

        event Action everyFrame;
        event Action everySecond;

        /// <summary>
        /// Runs an action with a time delay.
        /// </summary>
        [PublicAPI]
        public void RunWithDelay( Action action, float timeDelay )
        {
            delayedActions.Add( new DelayedAction( action, timeDelay ) );
        }

        void Awake()
        {
            unscaledTime = Time.unscaledTime;
        }

        void Update()
        {
            unscaledTime = Time.unscaledTime;

            if ( !everyFrameEventIsNull )
                everyFrame.Invoke();
            var newSecondCount = (int)unscaledTime;

            if ( !everySecondEventIsNull && newSecondCount != secondCount )
                everySecond.Invoke();
            secondCount = newSecondCount;

            for ( int i = delayedActions.Count - 1; i >= 0; i-- )
            {
                var delayedAction = delayedActions[i];

                if ( delayedAction.actionTime <= unscaledTime )
                {
                    delayedAction.action.Dispatch();
                    delayedActions.RemoveAt( i );
                }
            }
        }

        struct DelayedAction
        {
            public readonly Action action;
            public readonly float actionTime;

            public DelayedAction( Action action, float timeDelay )
            {
                this.action = action;
                actionTime = unscaledTime + timeDelay;
            }
        }
    }
}
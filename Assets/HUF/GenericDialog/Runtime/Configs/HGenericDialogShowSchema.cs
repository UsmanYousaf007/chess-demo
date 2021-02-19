using System;
using HUF.Utils.Runtime.Configs.API;
using HUF.Utils.Runtime.Logging;
using UnityEngine;

namespace HUF.GenericDialog.Runtime.Configs
{
    [CreateAssetMenu( menuName = HGenericDialogConfig.CONFIG_ROOT_PATH + nameof(HGenericDialogShowSchema) )]
    public class HGenericDialogShowSchema : AbstractConfig
    {
        new readonly HLogPrefix logPrefix = new HLogPrefix(nameof(HGenericDialogShowSchema));
        [SerializeField] bool showOnAndroid;
        [SerializeField] bool showOnIOS;
        [SerializeField] bool showOnOther;
        [SerializeField] int initialSessionsSkip;
        [SerializeField] int postponeSessionsSkip;
        [SerializeField] int showEveryNthSession = 1;

        bool CanShowOnPlatform
        {
            get
            {
#if UNITY_ANDROID
                return showOnAndroid;
#elif UNITY_IOS
                return showOnIOS;
#else
                return showOnOther;
#endif
            }
        }

        public bool CanShow( int session, int postponedSession = int.MinValue )
        {
            if(session < 1)
                HLog.LogError( logPrefix, "Session number must be greater than 0" );
            
            int adjustedSessionNumber = ( session - initialSessionsSkip ) - 1;
            
            return CanShowOnPlatform &&
                session > initialSessionsSkip &&
                adjustedSessionNumber % showEveryNthSession == 0 &&
                session > postponedSession + postponeSessionsSkip;
        }

        public override void ValidateConfig()
        {
            base.ValidateConfig();
            EnsureValueRanges();
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            EnsureValueRanges();
        }

        void EnsureValueRanges()
        {
            if ( showEveryNthSession <= 0 )
                showEveryNthSession = 1;
        }
    }
}
using HUF.Utils.Runtime.Configs.API;
using UnityEngine;

namespace HUF.GenericDialog.Runtime.Configs
{
    [CreateAssetMenu( menuName = HGenericDialogConfig.CONFIG_ROOT_PATH + nameof(HGenericDialogShowSchema) )]
    public class HGenericDialogShowSchema : AbstractConfig
    {
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
#endif
                return showOnOther;
            }
        }

        public bool CanShow( int session, int postponedSession = int.MinValue )
        {
            return CanShowOnPlatform &&
                session > initialSessionsSkip &&
                ( session - initialSessionsSkip ) % showEveryNthSession == 0 &&
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
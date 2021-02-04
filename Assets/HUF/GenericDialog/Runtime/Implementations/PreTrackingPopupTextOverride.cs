#if !HUF_TRANSLATION_SYSTEM
using HUF.GenericDialog.Runtime.Configs;
using HUF.Utils.Runtime.Configs.API;
using UnityEngine;

namespace HUF.GenericDialog.Runtime.Implementations
{
    [CreateAssetMenu( menuName = HGenericDialogConfig.CONFIG_ROOT_PATH + nameof(PreTrackingPopupTextOverride) )]
    public class PreTrackingPopupTextOverride : AbstractConfig
    {
        public string text;
    }
}
#endif
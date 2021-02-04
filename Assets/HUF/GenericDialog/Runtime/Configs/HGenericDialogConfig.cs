using HUF.GenericDialog.Runtime.API;
using HUF.Utils.Runtime.Configs.API;
using UnityEngine;

namespace HUF.GenericDialog.Runtime.Configs
{
    [CreateAssetMenu( menuName = CONFIG_ROOT_PATH + nameof(HGenericDialogConfig) )]
    public class HGenericDialogConfig : AbstractConfig
    {
        public const string CONFIG_ROOT_PATH = "HUF/GenericDialog/";

        public bool showSecondaryButton;
        public HGenericDialogInstance prefab;
        public HGenericDialogShowSchema showSchema;

        public string headerTranslation;
        public string contentTranslation;
        public string primaryButtonTranslation;
        public string secondaryButtonTranslation;
    }
}
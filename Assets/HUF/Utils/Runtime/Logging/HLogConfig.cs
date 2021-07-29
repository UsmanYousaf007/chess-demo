using HUF.Utils.Runtime.Configs.API;
using UnityEngine;

namespace HUF.Utils.Runtime.Logging
{
    [CreateAssetMenu(menuName = "HUF/Utils/LogConfig", fileName = "LogConfig.asset")]
    public class HLogConfig : AbstractConfig
    {
        [SerializeField] bool canLogOnProd = false;
        [SerializeField] bool isFilteringLogs = default;
        [SerializeField] string regexFilter = default;
        [SerializeField] bool ignoreCaseInRegex = default;
        [SerializeField] bool iOSNativeLogs = false;
        [SerializeField] bool showTimeInNativeLogs = false;
        [SerializeField] bool disableHLogsOnDebugBuilds = false;
        [SerializeField] bool invertFilter = false;

        public bool IsFilteringLogs => isFilteringLogs;
        public bool CanLogOnProd => canLogOnProd;
        public string RegexFilter => regexFilter;
        public bool IgnoreCaseInRegex => ignoreCaseInRegex;
        public bool IOSNativeLogs => iOSNativeLogs;
        public bool ShowTimeInNativeLogs => showTimeInNativeLogs;
        public bool DisableHLogsOnDebugBuilds => disableHLogsOnDebugBuilds;
        public bool InvertFilter => invertFilter;

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            HLog.RefreshConfig();
            base.OnValidate();
        }
#endif
    }
}
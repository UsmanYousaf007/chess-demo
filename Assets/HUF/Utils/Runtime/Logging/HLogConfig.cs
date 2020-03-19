using System.Collections;
using HUF.Utils.Configs.API;
using UnityEngine;

namespace HUF.Utils.Runtime.Logging
{
    [CreateAssetMenu(menuName = "HUF/Utils/LogConfig", fileName = "LogConfig.asset")]
    public class HLogConfig : AbstractConfig
    {
        [SerializeField] bool canLogOnProd = false;
        [SerializeField] bool isFilteringLogs = default;
        [SerializeField] string regexFilter;
        [SerializeField] bool ignoreCaseInRegex = default;

        public bool IsFilteringLogs => isFilteringLogs;
        public bool CanLogOnProd => canLogOnProd;
        public string RegexFilter => regexFilter;
        public bool IgnoreCaseInRegex => ignoreCaseInRegex;
    }
}
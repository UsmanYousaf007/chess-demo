using System.Collections.Generic;
using HUF.Auth.Runtime.API;
using HUF.AuthSIWA.Runtime.API;
using HUF.Utils.Editor.BuildSupport;
using HUF.Utils.Runtime.Logging;

namespace HUF.AuthSIWA.Editor
{
    public class AuthSIWADummyPreprocessBuild : BaseDummyPreprocessBuild
    {
#if HUF_AUTH_SIWA_DUMMY
        public override bool Enabled => true;
#else
        public override bool Enabled => false;
#endif
        public override IEnumerable<string> DirectoriesToHide => new[]
        {
            "HUF/AuthSIWA/Runtime/Implementation",
            "HUF/AuthSIWA/Plugins"
        };

        public override HLogPrefix LogPrefix => logPrefix;
        readonly HLogPrefix logPrefix = new HLogPrefix( HAuthSIWA.logPrefix, nameof(AuthSIWADummyPreprocessBuild) );
    }
}
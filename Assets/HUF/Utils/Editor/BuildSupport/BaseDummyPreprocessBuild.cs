
using System.Collections.Generic;

namespace HUF.Utils.Editor.BuildSupport
{
    public abstract class BaseDummyPreprocessBuild : HideFilesDuringBuild
    {
        public abstract IEnumerable<string> DirectoriesToHide { get; }
        public override IEnumerable<string> FilesAndDirectoriesToHide => DirectoriesToHide;
        public void HideFolders( bool hide ) => HideShowFilesAndDirectory( hide );
    }
}
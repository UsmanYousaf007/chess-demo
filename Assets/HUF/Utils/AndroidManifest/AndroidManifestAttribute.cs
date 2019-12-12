using System;

namespace HUF.Utils.AndroidManifest
{
    public class AndroidManifestAttribute : Attribute
    {
        public string Tag;
        public string Attribute;
        public string ValueToReplace;
    }
}
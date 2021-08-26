using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace HUFEXT.PackageManager.Editor.Views
{
    [Serializable]
    public class DeveloperView : PackageManagerView
    {
        public DeveloperView( PackageManagerWindow parent ) : base( parent ) { }

        public override Models.PackageManagerViewType Type => Models.PackageManagerViewType.DeveloperView;
    }
}
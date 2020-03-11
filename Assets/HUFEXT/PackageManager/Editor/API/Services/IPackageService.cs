using System.Collections.Generic;
using HUFEXT.PackageManager.Editor.API.Data;
using UnityEngine.Events;

namespace HUFEXT.PackageManager.Editor.API.Services
{
    public interface IPackageService
    {
        void RequestPackagesList( string channel, UnityAction<List<PackageManifest>> onComplete );
    }
}
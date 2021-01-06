using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace HUFEXT.PackageManager.Editor.Models
{
    [Serializable]
    public class Version
    {
        public string version = string.Empty;
        public string scope = string.Empty;
    }
}
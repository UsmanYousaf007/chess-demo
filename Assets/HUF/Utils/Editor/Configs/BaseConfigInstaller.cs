using System.Collections.Generic;
using System.Linq;
using HUF.Utils.Runtime.Configs.API;
using UnityEditor;

namespace HUF.Utils.Editor.Configs
{
    public class BaseConfigInstaller : AssetPostprocessor
    {
        protected static void PostprocessImport<T>(string featureName, string scriptFileName, 
            IEnumerable<string> imported, string folderStructure = null) where T : AbstractConfig
        {
            if (imported.Any(asset => asset.Contains(scriptFileName)))
            {
                HConfigsEditor.AddConfigToInstallation(featureName, typeof(T), folderStructure);
            }
        }
    }
}
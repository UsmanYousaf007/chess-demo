using HUF.StorageFirebase.Runtime.Implementation;
using HUF.Utils.Editor.Configs;

namespace HUF.StorageFirebase.Editor
{
    public class FirebaseStorageConfigInstaller : BaseConfigInstaller
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            PostprocessImport<FirebaseStorageConfig>("Firebase Storage", "FirebaseStorageConfigInstaller.cs",
                importedAssets);
        }
    }
}
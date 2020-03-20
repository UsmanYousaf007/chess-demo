using HUF.StorageFirebase.Implementation;
using HUF.Utils.Configs.API.Editor;

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
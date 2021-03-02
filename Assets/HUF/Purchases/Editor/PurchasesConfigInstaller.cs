using HUF.Purchases.Runtime.Implementation.Data;
using HUF.Utils.Editor.Configs;

namespace HUF.Purchases.Editor
{
    public class PurchasesConfigInstaller : BaseConfigInstaller
    {
        static void OnPostprocessAllAssets(string[] imported, string[] deleted, string[] moved, string[] movedFromPaths)
        {
            PostprocessImport<PurchasesConfig>("Purchases", "PurchasesConfigInstaller.cs", imported);
        }
    }
}
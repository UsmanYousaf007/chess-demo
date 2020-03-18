using HUFEXT.PackageManager.Editor.Views;

namespace HUFEXT.PackageManager.Editor.API.Views
{
    public interface IPackageManagerView
    {
        PackageManagerWindow Window { get; }
        void Initialize( PackageManagerWindow parent );
        void Repaint();
    }
}

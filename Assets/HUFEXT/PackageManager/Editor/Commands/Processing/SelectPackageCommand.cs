namespace HUFEXT.PackageManager.Editor.Commands.Processing
{
    public class SelectPackageCommand : Core.Command.Base
    {
        readonly string name;
        readonly Views.PackageManagerWindow window;

        public SelectPackageCommand( Views.PackageManagerWindow window, string name )
        {
            this.name = name;
            this.window = window;
        }
        
        public override void Execute()
        {
            var selectedPackage = Core.Packages.Data.Find( package => package.name == name );

            if ( selectedPackage == null )
            {
                window.state.selectedPackage = null;
                Complete( false );
                return;
            }

            window.state.selectedPackage = selectedPackage;
            
            Core.Command.Execute( new Connection.GetPackageVersionsCommand()
            {
                manifest = selectedPackage,
                OnComplete= ( success, serializedData ) =>
                {
                    if ( !success )
                    {
                        return;
                    }
                    
                    var manifest = Models.PackageManifest.ParseManifest( serializedData, true );
                    if ( manifest.name == window.state.selectedPackage.name )
                    {
                        window.state.selectedPackage = manifest;
                    }
                }
            });
            
            Complete( true );
        }
    }
}

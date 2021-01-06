using System.Collections.Generic;

namespace HUFEXT.PackageManager.Editor.Commands.Base
{
    public class AddGoogleScopedRegistryCommand : Core.Command.Base
    {
        public override void Execute()
        {
            Core.Command.Execute( new AddScopedRegistriesCommand
            {
                registries = new List<Models.ScopedRegistryWrapper>
                {
                    new Models.ScopedRegistryWrapper
                    {
                        name = "Game Package Registry by Google",
                        url = Models.Keys.GOOGLE_SCOPED_REGISTRY_KEY,
                        scopes = new List<string>
                        {
                            "com.google"
                        }
                    }
                }
            } );
            Complete( true );
        }
    }
}
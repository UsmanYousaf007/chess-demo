namespace HUFEXT.PackageManager.Editor.Commands.Connection
{
    public class AuthorizeTokenCommand : Core.Command.Base
    {
        public override void Execute()
        {
            var request = new Core.Request( Models.Keys.Routing.API.SCOPES,
                ( response ) =>
                {
                    if ( response.status == Core.RequestStatus.Failure )
                    {
                        Complete( false, "Validation failed. Response status is failure." );
                        return;
                    }

                    var scopes = Utils.Common.FromJsonToArray<Models.Scope>( response.text );


                    if ( scopes.Count > 0 && Models.Token.CreateSignedToken() )
                    {
                        Complete( true );
                        return;
                    }

                    Complete( false, "Validation failed. Unable to parse response or sign token." );
                } );
            request.Send();
        }
    }
}
using HUF.Utils.Runtime.NetworkRequests;

namespace HUF.GameServer.Runtime.Response
{
    public class ServerResponse : GameServerResponse<GameServerResponseStatus>
    {
        public static ServerResponse Uninitialized { get; }
            = new ServerResponse( GameServerResponseStatus.NotInitialized, GameServerUtils.UNINITIALIZED_CODE );

        public static ServerResponse GenericFail { get; }
            = new ServerResponse( GameServerResponseStatus.Failed, 0 );


        public ServerResponse( GameServerResponseStatus status, long responseCode ) : base( status, responseCode ) { }
        public ServerResponse( GameServerResponseStatus status ) : base( status ) { }
        public ServerResponse( GameServerResponse<GameServerResponseStatus> other ) : base( other ) { }
    }
}
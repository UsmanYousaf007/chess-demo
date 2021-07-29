using HUF.Utils.Runtime.NetworkRequests;

namespace HUF.Purchases.Runtime.API.Data
{
    public class PriceConversionResponse: GameServerResponse<GameServerResponseStatus, PriceConversionData>
    {
        public PriceConversionResponse( GameServerResponseStatus status ) : base( status ) { }
        public PriceConversionResponse( GameServerResponseStatus status, long responseCode ) : base( status, responseCode ) { }
        public PriceConversionResponse( GameServerResponseStatus status, PriceConversionData data, long responseCode ) : base( status, data, responseCode ) { }
        public PriceConversionResponse( GameServerResponse<GameServerResponseStatus> other ) : base( other ) { }
    }
}
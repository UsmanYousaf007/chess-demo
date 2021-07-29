using System;
using System.Collections.Generic;
using System.Net.Http;
using HUF.Analytics.Runtime.API;
using HUF.Analytics.Runtime.Implementation;
using HUF.AnalyticsHBI.Runtime.API;
using HUF.AnalyticsHBI.Runtime.Configs;
using HUF.AnalyticsHBI.Runtime.Implementation;
using HUF.Utils.Runtime;
using HUF.Utils.Runtime.Configs.API;
using HUF.Utils.Runtime.Extensions;
using HUF.Utils.Runtime.Logging;
using JetBrains.Annotations;
using UnityEngine;
using HBIAnalytics = huuuge.Analytics;
using HUF.Utils.Runtime.NetworkRequests;

namespace HUF.AnalyticsHBI.Runtime.Implementation
{
    public class PlayerAttributesResponse : GameServerResponse<GameServerResponseStatus, PlayerAttributes>
    {
        public PlayerAttributesResponse( GameServerResponseStatus status ) : base( status ) { }

        public PlayerAttributesResponse( GameServerResponseStatus status, long responseCode ) : base( status,
            responseCode ) { }

        public PlayerAttributesResponse( GameServerResponseStatus status,
            PlayerAttributes data,
            long responseCode ) :
            base( status, data, responseCode ) { }

        public PlayerAttributesResponse( GameServerResponse<GameServerResponseStatus> other ) : base( other ) { }
    }
}
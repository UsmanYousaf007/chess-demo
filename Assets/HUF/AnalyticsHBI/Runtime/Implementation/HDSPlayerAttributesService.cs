using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
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
    public static class HDSPlayerAttributesService
    {
        const string LAST_REVENUE_LEVEL = "HDSLastRevenueLevel";

        static readonly HLogPrefix logPrefix = new HLogPrefix( nameof(HDSPlayerAttributesService) );

        public static void GetPlayerAttributes( Action<PlayerAttributesResponse> response )
        {
            var config = HConfigs.GetConfig<HBIAnalyticsConfig>();

            if ( config.ValidationToken.IsNullOrEmpty() || config.ServerUrl.IsNullOrEmpty() )
                return;

            var route = GameServerUtils.CreateRoute( config.RoutingTable.playerAttributes )
                .Set( "{project_name}", config.ProjectName )
                .Set( "{user_id}", HAnalyticsHBI.UserId )
                .Value;
            byte[] bytesToEncode = Encoding.UTF8.GetBytes( $"{config.AuthorizationUsername}:{config.ValidationToken}" );
            var requestData = GameServerUtils.CreateRequestData( config, HttpMethod.Get, route );
            requestData.token = $"Basic {Convert.ToBase64String( bytesToEncode )}";

            GameServerUtils.AsyncRequest( requestData,
                www =>
                {
                    if ( www.isNetworkError )
                    {
                        response.Dispatch( new PlayerAttributesResponse( GameServerResponseStatus.Failed ) );
                        return;
                    }

                    if ( www.isHttpError )
                    {
                        response.Dispatch(
                            new PlayerAttributesResponse( GameServerResponseStatus.Failed, www.responseCode ) );
                        return;
                    }

                    try
                    {
                        var playerAttributes = new PlayerAttributes( www.downloadHandler.text );
                        PlayerPrefs.SetString( HAnalyticsHBI.HBI_LAST_PLAYER_ATTRIBUTES, www.downloadHandler.text );
                        NewPlayerAttributes( playerAttributes );

                        response.Dispatch( new PlayerAttributesResponse( GameServerResponseStatus.Success,
                            playerAttributes,
                            www.responseCode ) );
                    }
                    catch ( Exception e )
                    {
                        Console.WriteLine( e );
                        throw;
                    }
                } );
        }

        static void NewPlayerAttributes( PlayerAttributes playerAttributes )
        {
            int level = GetRevenueLevel( playerAttributes );

            if ( level != PlayerPrefs.GetInt( LAST_REVENUE_LEVEL, -1 ) )
            {
                PlayerPrefs.GetInt( LAST_REVENUE_LEVEL, level );
                HAnalyticsHBI.HandleRevenueLevelChanged( level );
            }
        }

        static int GetRevenueLevel( PlayerAttributes playerAttributes )
        {
            var config = HAnalyticsHBI.Config;
            int level = 0;

            float valueOverMaximumLevel = playerAttributes.RevenueSumInDollars -
                                          ( config.PlayerRevenueLevelsInDollars.Length > 0
                                              ? config.PlayerRevenueLevelsInDollars.Last()
                                              : 0 );

            if ( valueOverMaximumLevel < 0 )
            {
                for ( level = 0; level < config.PlayerRevenueLevelsInDollars.Length; level++ )
                {
                    float revenueLevel = config.PlayerRevenueLevelsInDollars[level];

                    if ( playerAttributes.RevenueSumInDollars < revenueLevel )
                    {
                        return config.MaximumRevenueLevel > 0 ? Math.Min( level, config.MaximumRevenueLevel ) : level;
                    }
                }
            }

            level = config.PlayerRevenueLevelsInDollars.Length +
                    (int)( valueOverMaximumLevel / config.RevenueLevelRangeInDollars );
            return config.MaximumRevenueLevel > 0 ? Math.Min( level, config.MaximumRevenueLevel ) : level;
        }
    }
}
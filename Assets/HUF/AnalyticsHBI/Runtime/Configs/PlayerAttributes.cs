using System;
using System.Collections.Generic;
using HUF.AnalyticsHBI.Runtime.API;
using HUF.Utils.Runtime;
using HUF.Utils.Runtime.Logging;
using UnityEngine;

namespace HUF.AnalyticsHBI.Runtime.Configs
{
    [Serializable]
    public class PlayerAttributes
    {
        static readonly HLogPrefix logPrefix = new HLogPrefix( HAnalyticsHBI.logPrefix, nameof(PlayerAttributes) );

        public PlayerAttributes( string jsonString )
        {
            AttributesDictionary = (Dictionary<string, object>)HUFJson.Deserialize( jsonString );
        }

        enum Attribute
        {
            player_id,
            last_login_time,
            first_purchase_time,
            first_purchase_value,
            last_purchase_time,
            last_purchase_value,
            count_purchase_time,
            sum_purchase_value,
            first_ha_iso,
            last_ha_iso,
            calc_prev_login_time,
            calc_sum_ads_value,
            calc_first_media_source
        }

        public static Dictionary<string, object> AttributesDictionary { private get; set; }

        public float RevenueSumInDollars => SumPurchaseValue / 100f + SumAdsValue / 100f;

        public long SumPurchaseValue => GetLongAttribute( Attribute.sum_purchase_value );
        public long SumAdsValue => GetLongAttribute( Attribute.calc_sum_ads_value );
        public long LifetimeTransactionCount => GetLongAttribute( Attribute.count_purchase_time );

        public Purchase? FirstPurchase => GetPurchase( Attribute.first_purchase_time, Attribute.first_purchase_value );

        public Purchase? LastPurchase => GetPurchase( Attribute.last_purchase_time, Attribute.last_purchase_value );

        public DateTime? LastLoginTime => GetDateTimeAttribute( Attribute.last_login_time );

        public DateTime? PreviousLoginTime => GetDateTimeAttribute( Attribute.calc_prev_login_time );

        public string PlayersFirstCountry => GetStringAttribute( Attribute.first_ha_iso );
        public string PlayersLastCountry => GetStringAttribute( Attribute.last_ha_iso );

        public string MediaSource => GetStringAttribute( Attribute.calc_first_media_source );
        public string PlayerId => GetStringAttribute( Attribute.player_id );

        public string ToString()
        {
            return $"RevenueSumInDollars: {RevenueSumInDollars}, LastLoginTime: {LastLoginTime}," +
                   $" FirstPurchase: {( FirstPurchase != null ? JsonUtility.ToJson( FirstPurchase ) : "null" )}, " +
                   $"LastPurchase: {( LastPurchase != null ? JsonUtility.ToJson( LastPurchase ) : "null" )}, " +
                   $"LifetimeTransactionCount: {LifetimeTransactionCount}, PreviousLoginTime: {PreviousLoginTime}, " +
                   $"PlayersFirstCountry: {PlayersFirstCountry}, PlayersLastCountry: {PlayersLastCountry}, " +
                   $"MediaSource: {MediaSource}, PlayerId {PlayerId}";
        }

        float GetFloatAttribute( Attribute attribute )
        {
            string attributeString = attribute.ToString();

            if ( AttributesDictionary.ContainsKey( attributeString ) )
            {
                return (float)( AttributesDictionary[attributeString] ?? 0f );
            }

            return 0f;
        }

        long GetLongAttribute( Attribute attribute )
        {
            string attributeString = attribute.ToString();

            if ( AttributesDictionary.ContainsKey( attributeString ) )
            {
                return (long)( AttributesDictionary[attributeString] ?? 0L );
            }

            return 0L;
        }

        string GetStringAttribute( Attribute attribute )
        {
            string attributeString = attribute.ToString();

            if ( AttributesDictionary.ContainsKey( attributeString ) )
            {
                return (string)( AttributesDictionary[attributeString] ?? "" );
            }

            return "";
        }

        DateTime? GetDateTimeAttribute( Attribute attribute )
        {
            long timestamp = GetLongAttribute( attribute );

            return timestamp > 0
                ? DateTimeUtils.FromTimestamp( timestamp / 1000 )
                : (DateTime?)null;
        }

        Purchase? GetPurchase( Attribute timeAttribute, Attribute valueAttribute ) =>
            GetLongAttribute( timeAttribute ) == 0 || GetLongAttribute( valueAttribute ) == 0
                ? (Purchase?)null
                : new Purchase( GetDateTimeAttribute( timeAttribute ) ?? DateTime.Now,
                    GetLongAttribute( valueAttribute ) / 100f );

        [Serializable]
        public struct Purchase
        {
            public DateTime dateTime;
            public float valueInDollars;

            public Purchase( DateTime dateTime, float valueInDollars )
            {
                this.dateTime = dateTime;
                this.valueInDollars = valueInDollars;
            }
        }
    }
}
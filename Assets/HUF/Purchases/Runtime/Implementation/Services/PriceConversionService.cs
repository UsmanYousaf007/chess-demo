#if UNITY_PURCHASING
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using HUF.Purchases.Runtime.API;
using HUF.Purchases.Runtime.API.Data;
using HUF.Purchases.Runtime.API.Services;
using HUF.Purchases.Runtime.Implementation.Data;
using HUF.Utils.Runtime;
using HUF.Utils.Runtime.Extensions;
using HUF.Utils.Runtime.Logging;
using HUF.Utils.Runtime.NetworkRequests;
using HUF.Utils.Runtime.PlayerPrefs;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.MiniJSON;

namespace HUF.Purchases.Runtime.Implementation.Services
{
    public class PriceConversionService : IPriceConversionService
    {
        const string CONVERTED_CURRENCY = "USD";
        const string INTERNAL_SERVER_BASE_CURRENCY_KEY = "baseCurrency";
        const string INTERNAL_SERVER_CURRENCY_KEY = "currency";
        const string INTERNAL_SERVER_VALUE_KEY = "value";
        const string INTERNAL_SERVER_URL = "{0}/Prod/api/convert?currency={1}&value=1";
        const int RETRIES = 3;
        const float ERROR_TIMEOUT = 2f;

        static readonly HLogPrefix logPrefix = new HLogPrefix( HPurchases.logPrefix, nameof(PriceConversionService) );
        static readonly PriceConversionData sameConversionData =
            new PriceConversionData( CONVERTED_CURRENCY, CONVERTED_CURRENCY, 1 );

        readonly PurchasesConfig purchasesConfig;

        PriceConversionData pricesConversionData;

        public event Action<Product> OnGetConversionEnd;

        public PriceConversionService( PurchasesConfig purchasesConfig )
        {
            this.purchasesConfig = purchasesConfig;
#if UNITY_EDITOR
            ForceLoadConversionDataForCurrency( purchasesConfig.PriceConversionCurrency );
#endif
        }

        public void LoadPriceData( IPurchasesService purchasesService )
        {
            LoadPriceData( purchasesService.AccountCurrency );
        }

#if UNITY_EDITOR
        void ForceLoadConversionDataForCurrency( string currencyISO )
        {
            if ( purchasesConfig.IsUsingInternalConversionServer )
            {
                CoroutineManager.StartCoroutine(
                    RequestForConversionInternalServer( currencyISO,
                        purchasesConfig.GetDownloadRequestTimeout,
                        null ) );
            }
            else
            {
                CoroutineManager.StartCoroutine(
                    RequestForConversionExternalServer( currencyISO,
                        purchasesConfig.GetDownloadRequestTimeout,
                        null ) );
            }
        }
#endif

        public void TryGetConversion( Product product )
        {
            if ( purchasesConfig.IsForceDownloadEnabled == false &&
                 pricesConversionData != null &&
                 pricesConversionData.saveTime + purchasesConfig.CacheTime * 60 > DateTime.Now.ToTimestamp() )
            {
                OnGetConversionEnd.Dispatch( product );
                HLog.Log( logPrefix, $"Price conversion got from cache" );
                return;
            }

            if ( product.metadata.isoCurrencyCode == CONVERTED_CURRENCY )
            {
                SavePriceData( sameConversionData );
                OnGetConversionEnd.Dispatch( product );
                return;
            }

            if ( purchasesConfig.IsUsingInternalConversionServer )
            {
                CoroutineManager.StartCoroutine(
                    RequestForConversionInternalServer( product.metadata.isoCurrencyCode,
                        purchasesConfig.GetDownloadRequestTimeout,
                        product ) );
            }
            else
            {
                CoroutineManager.StartCoroutine(
                    RequestForConversionExternalServer( product.metadata.isoCurrencyCode,
                        purchasesConfig.GetDownloadRequestTimeout,
                        product ) );
            }
        }

        IEnumerator RequestForConversionInternalServer( string currencyToConvert, int timeout, Product product )
        {
            var www = UnityWebRequest.Get( string.Format( INTERNAL_SERVER_URL,
                purchasesConfig.GetConversionApiInternalServerURL,
                currencyToConvert ) );

            www.timeout = timeout;
            yield return www.SendWebRequest();

            if ( www.isNetworkError || www.isHttpError )
            {
                HLog.LogError( logPrefix, $"Error during currency conversion request: {www.error}" );
            }
            else
            {
                HLog.Log( logPrefix, $"Price conversion downloaded: {www.downloadHandler.text}" );
                ConvertInternalData( www.downloadHandler.text );
            }

            OnGetConversionEnd.Dispatch( product );
        }

        bool ConvertInternalData( string json )
        {
            var hashtableFromJson = json.HashtableFromJson();

            if ( !hashtableFromJson.ContainsKey( INTERNAL_SERVER_BASE_CURRENCY_KEY ) ||
                 !hashtableFromJson.ContainsKey( INTERNAL_SERVER_CURRENCY_KEY ) ||
                 !hashtableFromJson.ContainsKey( INTERNAL_SERVER_VALUE_KEY ) )
                return false;

            var data = new PriceConversionData( (string)hashtableFromJson[INTERNAL_SERVER_CURRENCY_KEY],
                (string)hashtableFromJson[INTERNAL_SERVER_BASE_CURRENCY_KEY],
                (decimal)( (double)hashtableFromJson[INTERNAL_SERVER_VALUE_KEY] ) );
            SavePriceData( data );

            return true;
        }

        IEnumerator RequestForConversionExternalServer( string currencyToConvert, int timeout, Product product )
        {
            var www = UnityWebRequest.Get(
                $"{purchasesConfig.GetConversionApiExternalServerURL}/api/latest?access_key={purchasesConfig.GetExternalServerApiKey}" +
                $"&symbols={CONVERTED_CURRENCY},{currencyToConvert}" );
            www.timeout = timeout;
            yield return www.SendWebRequest();

            if ( www.isNetworkError || www.isHttpError )
            {
                HLog.LogError( logPrefix, $"Error during currency conversion request: {www.error}" );
            }
            else
            {
                HLog.Log( logPrefix, $"Price conversion downloaded: {www.downloadHandler.text}" );
                ConvertExternalData( currencyToConvert, www.downloadHandler.text );
            }

            OnGetConversionEnd.Dispatch( product );
        }

        bool ConvertExternalData( string currency, string json )
        {
            var hashtableFromJson = json.HashtableFromJson();

            if ( !hashtableFromJson.ContainsKey( "rates" ) )
                return false;

            var currencyDictionary = (Dictionary<string, object>)hashtableFromJson["rates"];

            if ( !currencyDictionary.ContainsKey( CONVERTED_CURRENCY )
                 || !currencyDictionary.ContainsKey( currency ) )
                return false;

            double value = (double)currencyDictionary[CONVERTED_CURRENCY] /
                           (double)currencyDictionary[currency];

            var data = new PriceConversionData( currency,
                CONVERTED_CURRENCY,
                (decimal)value );
            SavePriceData( data );

            return true;
        }

        void SavePriceData( PriceConversionData conversionData )
        {
            conversionData.saveTime = DateTime.Now.ToTimestamp();
            pricesConversionData = conversionData;

            HPlayerPrefs.SetString(
                $"priceData_{pricesConversionData.currencyToConvert}",
                JsonUtility.ToJson( pricesConversionData ) );
            HLog.Log( logPrefix, $"Data saved priceData_{pricesConversionData.currencyToConvert}" );
        }

        void LoadPriceData( string currency )
        {
            if ( HPlayerPrefs.HasKey( $"priceData_{currency}" ) == false )
            {
                return;
            }

            pricesConversionData = JsonUtility.FromJson<PriceConversionData>(
                HPlayerPrefs.GetString( $"priceData_{currency}" ) );

            HLog.Log( logPrefix,
                $"Data loaded priceData_{currency} " +
                $"conversion value: {pricesConversionData.convertedCurrencyValue}" );
        }

        public PriceConversionData GetConversionData()
        {
            return pricesConversionData;
        }

        public void TryGetPriceConversionData( Action<PriceConversionResponse> response, string currency )
        {
            if ( currency == CONVERTED_CURRENCY )
            {
                response.Dispatch( new PriceConversionResponse(
                    GameServerResponseStatus.Success,
                    sameConversionData,
                    (long)HttpStatusCode.OK ) );
                return;
            }

            if ( pricesConversionData == null || !string.Equals( pricesConversionData.currencyToConvert, currency ) )
                LoadPriceData( currency );

            if ( purchasesConfig.IsForceDownloadEnabled == false &&
                 pricesConversionData != null &&
                 pricesConversionData.currencyToConvert == currency &&
                 pricesConversionData.saveTime + purchasesConfig.CacheTime * 60 > DateTime.Now.ToTimestamp() )
            {
                HLog.Log( logPrefix, $"Got price conversion from cache." );
                response.Dispatch( new PriceConversionResponse( GameServerResponseStatus.Success, pricesConversionData, (long)HttpStatusCode.OK ) );
                return;
            }

            var requestData = purchasesConfig.IsUsingInternalConversionServer
            ? GetInternalRequest(currency)
            : GetExternalRequest(currency);

            GameServerUtils.AsyncRequest( requestData,
                www =>
                {
                    if ( www.isNetworkError )
                    {
                        response.Dispatch( new PriceConversionResponse( GameServerResponseStatus.Failed ) );
                        return;
                    }

                    if ( www.isHttpError )
                    {
                        response.Dispatch(
                            new PriceConversionResponse( GameServerResponseStatus.Failed,  www.responseCode ) );
                        return;
                    }

                    if( TryConvertRawData( currency, www.downloadHandler.text ) )
                        response.Dispatch( new PriceConversionResponse(
                            GameServerResponseStatus.Success,
                            pricesConversionData,
                            www.responseCode ) );
                    else
                        response.Dispatch( new PriceConversionResponse(
                            GameServerResponseStatus.Failed,
                            www.responseCode ) );

                } );
        }

        GameServerUtils.RequestData GetExternalRequest( string currency )
        {
            var request = new GameServerUtils.RequestData()
            {
                url = string.Empty,
                api = $"{purchasesConfig.GetConversionApiExternalServerURL}/api/latest?access_key={purchasesConfig.GetExternalServerApiKey}&symbols={CONVERTED_CURRENCY},{currency}",
                errorRetryLimit = RETRIES,
                errorRetryWait = ERROR_TIMEOUT,
                method = HttpMethod.Get.Method,
                timeout = purchasesConfig.GetDownloadRequestTimeout
            };
            return request;
        }

        GameServerUtils.RequestData GetInternalRequest( string currency )
        {
            var request = new GameServerUtils.RequestData()
            {
                url = string.Empty,
                api = string.Format( INTERNAL_SERVER_URL,
                    purchasesConfig.GetConversionApiInternalServerURL,
                    currency ),
                errorRetryLimit = RETRIES,
                errorRetryWait = ERROR_TIMEOUT,
                method = HttpMethod.Get.Method,
                timeout = purchasesConfig.GetDownloadRequestTimeout
            };
            return request;
        }

        bool TryConvertRawData( string currency, string data )
        {
            return purchasesConfig.IsUsingInternalConversionServer
                ? ConvertInternalData( data )
                : ConvertExternalData( currency, data );
        }
    }
}
#endif
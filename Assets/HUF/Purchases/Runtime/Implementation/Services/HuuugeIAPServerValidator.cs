using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using HUF.Purchases.Runtime.API;
using HUF.Purchases.Runtime.API.Data;
using HUF.Purchases.Runtime.Implementation.Data;
using HUF.Utils.Runtime;
using HUF.Utils.Runtime.Configs.API;
using HUF.Utils.Runtime.Extensions;
using HUF.Utils.Runtime.Logging;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;

namespace HUF.Purchases.Runtime.Implementation.Services
{
    public class HuuugeIAPServerValidator
    {
#if UNITY_IOS
        const string PLATFORM = "ios";
#else
        const string PLATFORM = "gp";
#endif

        const string CONTENT_TYPE = "Content-Type";
        const string CONTENT_TYPE_JSON = "application/json";
        const string REQUEST_TYPE_POST = "POST";
        const string TOKEN_HEADER = "X-PROJECT-TOKEN";
        const string STATUS_FIELD = "status";
        const string VALID_FIELD = "valid";
        const string REQUIRED_STATUS_VALUE = "OK";
        const float DELAY = 0.5f;

        public static event UnityAction<ValidatorResponse> OnHuuugeServerValidate;

        static readonly HLogPrefix logPrefix = new HLogPrefix( HPurchases.logPrefix, nameof(HuuugeIAPServerValidator) );
        readonly WaitForSeconds delayBetweenRequests = new WaitForSeconds( DELAY );

        int AttemptsAmount { get; }
        string BasePath { get; }
        string VerificationsEndpoint => $"{BasePath.TrimEnd( '/' )}/api/v1/iap/{PLATFORM}/verifications";
        string SubscriptionsEndpoint => $"{BasePath.TrimEnd( '/' )}/api/v1/iap/subscriptions/{PLATFORM}";
        string Token { get; }

        public HuuugeIAPServerValidator()
        {
            var purchasesConfig = HConfigs.GetConfig<PurchasesConfig>();
            Token = purchasesConfig.HuuugeIAPServerToken;
            BasePath = purchasesConfig.HuuugeIAPServerURL;
            AttemptsAmount = purchasesConfig.HuuugeIAPServerAttempts;
        }

        public IEnumerator ConsumeIapRequest( string iapId )
        {
            HLog.Log( logPrefix, $"ConsumeIapRequest: {iapId}" );

            for ( int attempt = 0; attempt < AttemptsAmount; attempt++ )
            {
                using ( var www = UnityWebRequest.Post( $"{VerificationsEndpoint}/{iapId}/consume", new WWWForm() ) )
                {
                    HLog.Log( logPrefix, $"URI: {www.uri}" );
                    www.SetRequestHeader( CONTENT_TYPE, CONTENT_TYPE_JSON );
                    www.SetRequestHeader( TOKEN_HEADER, Token );
                    yield return www.SendWebRequest();

                    if ( www.isNetworkError || www.isHttpError )
                    {
                        HLog.Log( logPrefix, www.error );
                    }
                    else
                    {
                        HLog.Log( logPrefix, $"Consume for {iapId} complete, responseCode {www.responseCode}" );
                        yield break;
                    }

                    yield return delayBetweenRequests;
                }
            }
        }

        public IEnumerator Verify( Product product, IPurchaseReceipt receipt )
        {
            bool isSubscription = product.definition.type == ProductType.Subscription;
            string address = isSubscription ? SubscriptionsEndpoint : VerificationsEndpoint;
            HLog.Log( logPrefix, $"Verify path {address}" );
            string json = GetVerifyJSON( product );
            HLog.Log( logPrefix, json );

            for ( int attempt = 0; attempt < AttemptsAmount; attempt++ )
            {
                bool isLastAttempt = attempt + 1 == AttemptsAmount;
                var request = new UnityWebRequest( address, REQUEST_TYPE_POST );
                var bodyRaw = Encoding.UTF8.GetBytes( json );
                request.uploadHandler = new UploadHandlerRaw( bodyRaw );
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader( CONTENT_TYPE, CONTENT_TYPE_JSON );
                request.SetRequestHeader( TOKEN_HEADER, Token );
                yield return request.SendWebRequest();

                bool failure = request.isNetworkError || request.isHttpError || request.responseCode != 200;
                HLog.Log( logPrefix, $"Status Code for transaction {product.definition.id}: {request.responseCode}" );
                HLog.Log( logPrefix, "Response: " + request.downloadHandler.text );

                if ( !failure )
                {
                    if ( isSubscription )
                    {
                        var validSubscription = GetFieldFromJson<bool>( request.downloadHandler.text, VALID_FIELD );
                        failure = !validSubscription;
                    }
                    else
                    {
                        var responseStatus = GetFieldFromJson<string>( request.downloadHandler.text, STATUS_FIELD );
                        failure = !responseStatus.Contains( REQUIRED_STATUS_VALUE );
                    }

                    if ( failure )
                    {
                        HLog.LogError( logPrefix,
                            $"Server returned that this transaction is not valid: {product.definition.id}" );
                    }
                }

                if ( failure )
                {
                    HLog.Log( logPrefix, "Something went wrong: " + request.downloadHandler.text );

                    if ( !isLastAttempt )
                    {
                        yield return delayBetweenRequests;

                        continue;
                    }
                }

                string requestId = failure || isSubscription
                    ? string.Empty
                    : GetFieldFromJson<string>( request.downloadHandler.text, "iapId" );

                var subscriptionResponse = failure || !isSubscription
                    ? null
                    : JsonUtility.FromJson<SubscriptionResponse>( request.downloadHandler.text );
                var responseError = failure ? request.downloadHandler.text : string.Empty;

                var response = new ValidatorResponse
                {
                    product = product,
                    receipt = receipt,
                    requestId = requestId,
                    responseCode = request.responseCode,
                    subscriptionResponse = subscriptionResponse,
                    responseError = responseError
                };
                OnHuuugeServerValidate.Dispatch( response );
                yield break;
            }
        }

#if UNITY_ANDROID
        string GetVerifyJSON( Product product )
        {
            var payloadData = new PurchaseReceiptData( product.receipt ).GetGooglePayloadData();
            var purchaseToken = GetFieldFromJson<string>( payloadData.json, "purchaseToken" );

            return JsonUtility.ToJson( new VerifyJsonGP
            {
                productId = product.definition.storeSpecificId,
                packageName = Application.identifier,
                purchaseToken = purchaseToken
            } );
        }
#elif UNITY_IOS
        string GetVerifyJSON( Product product )
        {
            string payloadData = new PurchaseReceiptData( product.receipt ).receipt.Payload;

            return JsonUtility.ToJson( new VerifyJsonIOS
            {
                productId = product.definition.storeSpecificId,
                receipt = payloadData
            } );
        }
#else
        string GetVerifyJSON( Product product )
        {
            return "";
        }
#endif

        static T GetFieldFromJson<T>( string json, string key )
        {
            if ( HUFJson.Deserialize( json ) is Dictionary<string, object> dict && dict.ContainsKey( key ) )
            {
                return (T)dict[key];
            }

            return default;
        }

#if UNITY_ANDROID
        struct VerifyJsonGP
        {
            public string productId;
            public string packageName;
            public string purchaseToken;
        }
#endif

#if UNITY_IOS
        struct VerifyJsonIOS
        {
            public string productId;
            public string receipt;
        }
#endif
    }
}
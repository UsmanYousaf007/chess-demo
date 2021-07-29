using System;
using HUF.Utils.Runtime.Logging;
using UnityEngine;

namespace HUF.Purchases.Runtime.API.Data
{
    public class PurchaseReceiptData
    {
        static readonly HLogPrefix logPrefix = new HLogPrefix( HPurchases.logPrefix, nameof(PurchaseReceiptData) );
        public readonly ReceiptData receipt;
        public readonly string rawReceipt;

        [Obsolete( "Use PurchaseReceiptData.receipt.Payload" )]
        // ReSharper disable once InconsistentNaming // backward compatibility
        public string payload => receipt.Payload;

        [Serializable]
        public class ReceiptData
        {
            // ReSharper disable InconsistentNaming
            public string Store = default;
            public string Payload = default;
            public string TransactionID = default;
            // ReSharper restore InconsistentNaming
        }

        [Serializable]
        public class GooglePurchasePayload
        {
            public string json = default;
            public string signature = default;
        }

        public PurchaseReceiptData( string receipt )
        {
            rawReceipt = receipt;
            this.receipt = JsonUtility.FromJson<ReceiptData>( receipt );
        }

        public GooglePurchasePayload GetGooglePayloadData()
        {
            bool payloadIsEmpty = string.IsNullOrEmpty( receipt.Payload );

            if ( !payloadIsEmpty )
            {
                var purchasePayload = JsonUtility.FromJson<GooglePurchasePayload>( receipt.Payload );

                if ( purchasePayload != null )
                    return purchasePayload;
            }

            HLog.LogError( logPrefix, $"Payload data is {( payloadIsEmpty ? "empty" : "broken" )}." );
            return null;
        }
    }
}

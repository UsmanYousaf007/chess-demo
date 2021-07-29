using System;
using UnityEngine;

namespace HUF.Purchases.Runtime.API.Data
{
    [Serializable]
    public class SubscriptionSaveData
    {
        [SerializeField] public bool isPaid = false;
        [SerializeField] public long expirationTimestamp = 0;
        [SerializeField] string id = string.Empty;

        public string Id => id;

        public SubscriptionSaveData() {}

        public SubscriptionSaveData(string id)
        {
            this.id = id;
        }
    }
}

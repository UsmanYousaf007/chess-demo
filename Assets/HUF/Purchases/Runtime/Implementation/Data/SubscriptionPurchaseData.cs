using HUF.Purchases.Runtime.API.Data;

namespace HUF.Purchases.Runtime.Implementation.Data
{
    public class SubscriptionPurchaseData : ISubscriptionPurchaseData
    {
        public SubscriptionPurchaseData(IProductInfo productInfo, bool isRenewed, bool isPaid)
        {
            this.ProductInfo = productInfo;
            this.IsRenewed = isRenewed;
            this.IsPaid = isPaid;
        }

        public IProductInfo ProductInfo { get; }
        public bool IsRenewed { get; }
        public bool IsPaid { get; }
    }
}
namespace HUF.Purchases.Runtime.API.Data
{
    public interface ISubscriptionPurchaseData
    {
        IProductInfo ProductInfo { get; }
        bool IsRenewed { get; }
        bool IsPaid { get; }
    }
}
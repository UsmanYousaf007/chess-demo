namespace HUF.Purchases.Runtime.API.Data
{
    public interface IProductInfo
    {
        IAPProductType Type { get; }
        
        string ProductId { get; }
        string ShopId { get; }
        int PriceInCents { get; }
        bool IsRestorable();
        int SubscriptionTrialPeriod { get; }
        int SubscriptionPeriod { get; }
    }
}
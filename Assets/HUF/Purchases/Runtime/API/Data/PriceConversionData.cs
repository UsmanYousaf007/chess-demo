using System;

namespace HUF.Purchases.Runtime.API.Data
{
    [Serializable]
    public class PriceConversionData
    {
        public string currencyToConvert;
        public string currencyConverted;
        public decimal convertedCurrencyValue;
        public double saveTime;

        public PriceConversionData( PriceConversionData conversionData, decimal notConvertedCurrency )
        {
            currencyToConvert = conversionData.currencyToConvert;
            currencyConverted = conversionData.currencyConverted;
            convertedCurrencyValue = conversionData.convertedCurrencyValue * notConvertedCurrency;
            saveTime = conversionData.saveTime;
        }

        public PriceConversionData( string currencyToConvert, string currencyConverted, decimal convertedCurrencyValue )
        {
            this.currencyToConvert = currencyToConvert;
            this.currencyConverted = currencyConverted;
            this.convertedCurrencyValue = convertedCurrencyValue;
        }
    }
}

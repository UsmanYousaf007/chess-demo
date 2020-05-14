using System.Collections.Generic;
using JetBrains.Annotations;

namespace HUF.Analytics.Runtime.API
{
    public class AnalyticsMonetizationEvent : AnalyticsEvent
    {
        public const string CENTS_KEY = "cents";
        
        /// <summary>
        /// Cents value
        /// </summary>
        [PublicAPI]
        public int Cents => EventData.ContainsKey(CENTS_KEY) ? (int) EventData[CENTS_KEY] : 0;

        AnalyticsMonetizationEvent(string name, int cents) : base(name)
        {
            SetCents(cents);
        }

        AnalyticsMonetizationEvent(Dictionary<string, object> eventData, int cents) : base(eventData)
        {
            SetCents(cents);
        }

        void SetCents(int cents)
        {
            EventData[CENTS_KEY] = cents;
        }

        /// <summary>
        /// Create analytics event with given name and specific cents value<para />
        /// </summary>
        [PublicAPI]
        public static AnalyticsMonetizationEvent Create(string eventName, int cents)
        {
            return IsNameValid(eventName) ? new AnalyticsMonetizationEvent(eventName, cents) : null;
        }

        /// <summary>
        /// Create monetization analytics event with wrapped parameters from passed dictionary. <para />
        /// Parameter with key "name" is required in dictionary, should store event name and can't be empty or null. <para />
        /// Parameter with key "cents" is required in dictionary and should be int type
        /// Supported types: bool, int, long, double or string
        /// </summary>
        /// <param name="eventData">Dictionary containing analytics event data</param>
        /// <param name="cents">Price of item described in analytics event</param>
        /// <returns>New wrapped AnalyticsEvent object or null if no
        /// event name value is present in passed event data</returns>
        [PublicAPI]
        public static AnalyticsMonetizationEvent Create(Dictionary<string, object> eventData, int cents)
        {
            if (!HasParameterInDictionary(eventData, EventConsts.EVENT_NAME_KEY))
            {
                return null;
            }

            var eventName = (string) eventData[EventConsts.EVENT_NAME_KEY];
            return IsNameValid(eventName) ? new AnalyticsMonetizationEvent(eventData, cents) : null;
        }
    }
}
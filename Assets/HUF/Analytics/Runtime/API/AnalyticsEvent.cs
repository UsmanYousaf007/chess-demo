using System;
using System.Collections.Generic;
using System.Text;
using HUF.Utils.Runtime.Extensions;
using JetBrains.Annotations;
using UnityEngine;

namespace HUF.Analytics.Runtime.API
{
    public class AnalyticsEvent
    {
        /// <summary>
        /// Name of current event
        /// </summary>
        [PublicAPI] public string EventName
        {
            get
            {
                if (EventData != null && EventData.ContainsKey(EventConsts.EVENT_NAME_KEY))
                {
                    return EventData[EventConsts.EVENT_NAME_KEY] as string;
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// Parameters added to current event
        /// </summary>
        [PublicAPI] public Dictionary<string, object> EventData { get; private set; }

        internal AnalyticsEvent(string name)
        {
            AddEventData(EventConsts.EVENT_NAME_KEY, name);
        }
        
        internal AnalyticsEvent(Dictionary<string, object> eventData)
        {
            EventData = eventData;
        }

        /// <summary>
        /// Create analytics event with given name. <para />
        /// <paramref name="eventName"/> can't be empty or null
        /// </summary>
        [PublicAPI]
        public static AnalyticsEvent Create(string eventName)
        {
            return IsNameValid(eventName) ? new AnalyticsEvent(eventName) : null;
        }

        protected static bool IsNameValid(string eventName)
        {
            if (eventName.IsNullOrEmpty())
            {
                Debug.LogWarning("[AnalyticsEvent] Event name can't be null or empty value");
                return false;
            }
            return true;
        }
        
        /// <summary>
        /// Create analytics event with wrapped parameters from passed dictionary. <para />
        /// Parameter with key "name" is required in dictionary, should store event name and can't be empty or null. <para />
        /// Supported types: bool, int, long, double or string
        /// </summary>
        /// <param name="eventData">Dictionary containing analytics event data</param>
        /// <returns>New wrapped AnalyticsEvent object or null if no
        /// event name value is present in passed event data</returns>
        [PublicAPI]
        public static AnalyticsEvent Create(Dictionary<string, object> eventData)
        {
            if (!HasParameterInDictionary(eventData, EventConsts.EVENT_NAME_KEY))
            {
                return null;
            }

            var eventName = (string) eventData[EventConsts.EVENT_NAME_KEY];
            return IsNameValid(eventName) ? new AnalyticsEvent(eventData) : null;
        }

        protected static bool HasParameterInDictionary(Dictionary<string, object> eventData, string name)
        {
            if (!eventData.ContainsKey(name))
            {
                Debug.LogWarning($"[AnalyticsEvent] Missing {name} parameter in dictionary.");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Add parameter with value <see cref="EventName"/> and value given in parameter.
        /// </summary>
        [PublicAPI]
        public AnalyticsEvent Value(int value)
        {
            AddEventData(EventConsts.VALUE_KEY, value);
            return this;
        }

        /// <summary>
        /// Add parameter with value <see cref="EventName"/> and value given in parameter.
        /// </summary>
        [PublicAPI]
        public AnalyticsEvent Value(long value)
        {
            AddEventData(EventConsts.VALUE_KEY, value);
            return this;
        }

        /// <summary>
        /// Add parameter to analytics event.
        /// </summary>
        /// <param name="name">Name of parameter</param>
        /// <param name="value">Value of parameter</param>
        [PublicAPI]
        public AnalyticsEvent Parameter(string name, bool value)
        {
            AddEventData(name, value);
            return this;
        }

        /// <summary>
        /// Add parameter to analytics event.
        /// </summary>
        /// <param name="name">Name of parameter</param>
        /// <param name="value">Value of parameter</param>
        [PublicAPI]
        public AnalyticsEvent Parameter(string name, int value)
        {
            AddEventData(name, value);
            return this;
        }

        /// <summary>
        /// Add parameter to analytics event.
        /// </summary>
        /// <param name="name">Name of parameter</param>
        /// <param name="value">Value of parameter</param>
        [PublicAPI]
        public AnalyticsEvent Parameter(string name, float value)
        {
            AddEventData(name, Convert.ToDouble(value));
            return this;
        }

        /// <summary>
        /// Add parameter to analytics event.
        /// </summary>
        /// <param name="name">Name of parameter</param>
        /// <param name="value">Value of parameter</param>
        [PublicAPI]
        public AnalyticsEvent Parameter(string name, long value)
        {
            AddEventData(name, Convert.ToInt64(value));
            return this;
        }

        /// <summary>
        /// Add parameter to analytics event.
        /// </summary>
        /// <param name="name">Name of parameter</param>
        /// <param name="value">Value of parameter</param>
        [PublicAPI]
        public AnalyticsEvent Parameter(string name, double value)
        {
            AddEventData(name, value);
            return this;
        }

        /// <summary>
        /// Add parameter to analytics event.
        /// </summary>
        /// <param name="name">Name of parameter</param>
        /// <param name="value">Value of parameter</param>
        [PublicAPI]
        public AnalyticsEvent Parameter(string name, string value)
        {
            AddEventData(name, value);
            return this;
        }

        void AddEventData(string name, object value)
        {
            if (value == null)
            {
                Debug.LogError($"[AnalyticsEvent] Parameter can't be null. Parameter {name} won't be added.");
                return;
            }

            if (value is string && ((string)value).IsNullOrEmpty() )
            {
                Debug.LogError($"[AnalyticsEvent] String parameter can't be empty. Parameter {name} won't be added.");
                return;
            }

            if (EventData == null)
                EventData = new Dictionary<string, object>();
            
            EventData[name] = value;
        }

        /// <summary>
        /// Add parameter with ST1 name to analytics event. <para />
        /// ST1 - Stage1 and is used by our analytics to structurize events into hierarchy.
        /// </summary>
        /// <param name="value">Value of parameter</param>
        [PublicAPI]
        public AnalyticsEvent ST1(string value)
        {
            return Parameter(EventConsts.ST1_KEY, value);
        }

        /// <summary>
        /// Add parameter with ST2 name to analytics event. <para />
        /// ST2 - Stage2 and is used by our analytics to structurize events into hierarchy.
        /// </summary>
        /// <param name="value">Value of parameter</param>
        [PublicAPI]
        public AnalyticsEvent ST2(string value)
        {
            return Parameter(EventConsts.ST2_KEY, value);
        }

        /// <summary>
        /// Add parameter with ST3 name to analytics event. <para />
        /// ST3 - Stage3 and is used by our analytics to structurize events into hierarchy.
        /// </summary>
        /// <param name="value">Value of parameter</param>
        [PublicAPI]
        public AnalyticsEvent ST3(string value)
        {
            return Parameter(EventConsts.ST3_KEY, value);
        }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("{");
            foreach (var pair in EventData)
            {
                stringBuilder.Append($"{pair.Key}: '{pair.Value}', ");
            }

            return stringBuilder.Remove(stringBuilder.Length - 2, 2).Append("}").ToString();
        }

        public static class EventConsts
        {
            public const string VALUE_KEY = "value";

            public const string EVENT_NAME_KEY = "name";
            public const string ST1_KEY = "st1";
            public const string ST2_KEY = "st2";
            public const string ST3_KEY = "st3";
        }
    }
}
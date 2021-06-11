/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2018-01-10 10:52:49 UTC+05:00

using UnityEngine;
using UnityEngine.Analytics;
using System.Collections.Generic;
using TurboLabz.TLUtils;
using System.Text;
using System;
using GameAnalyticsSDK;

namespace TurboLabz.InstantFramework
{
    public class UnityAnalyticsService : IAnalyticsService
    {
        public void ScreenVisit(AnalyticsScreen screen)
        {
            AnalyticsEvent.ScreenVisit(screen.ToString());
            GameAnalytics.NewDesignEvent($"screen_visit:{screen}");
            Print($"screen_visit:{screen}");
        }

        public void ScreenVisit(AnalyticsScreen screen, bool fb_logged_in)
        {
            Dictionary<string, object> p = new Dictionary<string, object>
            {
                { AnalyticsParameter.fb_logged_in.ToString(), fb_logged_in }
            };

            AnalyticsEvent.ScreenVisit(screen.ToString(), p);
            GameAnalytics.NewDesignEvent($"screen_visit:{screen}:fb_logged_in_{fb_logged_in}");
            Print($"screen_visit:{screen}:fb_logged_in_{fb_logged_in}");
        }

        public void ScreenVisit(AnalyticsScreen screen, bool fb_logged_in, bool is_bot) 
        {
            Dictionary<string, object> p = new Dictionary<string, object>
            {
                { AnalyticsParameter.fb_logged_in.ToString(), fb_logged_in },
                { AnalyticsParameter.is_bot.ToString(), is_bot },
            };

            AnalyticsEvent.ScreenVisit(screen.ToString(), p);
            GameAnalytics.NewDesignEvent($"screen_visit:{screen}:fb_logged_in_{fb_logged_in}:is_bot{is_bot}");
            Print("ScreenVisit:" + screen, p);
        }

        public void ScreenVisit(AnalyticsScreen screen, AnalyticsContext context)
        {
            Dictionary<string, object> p = new Dictionary<string, object>
            {
                { AnalyticsParameter.context.ToString(), context.ToString() }
            };

            AnalyticsEvent.ScreenVisit(screen.ToString(), p);
            GameAnalytics.NewDesignEvent($"screen_visit:{screen}:{context}");
            Print("ScreenVisit:" + screen, p);
        }

        public void Event(AnalyticsEventId evt)
        {
            Analytics.CustomEvent(evt.ToString());
            GameAnalytics.NewDesignEvent(evt.ToString());
            Print(evt.ToString());
        }

        public void Event(AnalyticsEventId evt, AnalyticsParameter param, object val)
        {
            Event(evt.ToString(), param, val);
        }

        public void Event(string evt, AnalyticsParameter param, object val)
        {
            if (param == AnalyticsParameter.elo)
            {
                int rating = (int)val;
                val = rating - (rating % 200);
            }
            else if (param == AnalyticsParameter.internal_matchmaking_elo)
            {
                int rating = (int)val;
                val = rating - (rating % 100);
            }
            else if (param == AnalyticsParameter.bot_difficulty)
            {
                val = Math.Round((float)val, 1);
            }
            else if (param == AnalyticsParameter.duration)
            {
                val = Math.Round((double)val, 1);
            }

            Dictionary<string, object> p = new Dictionary<string, object>
            {
                { param.ToString(), val }
            };

            Analytics.CustomEvent(evt, p);
            GameAnalytics.NewDesignEvent($"{evt}:{param}:{val}");
            Print(evt, p);
        }

        public void Event(AnalyticsEventId evt, AnalyticsContext context)
        {
            Event(evt.ToString(), context);
        }

        public void Event(string evt, AnalyticsContext context)
        {
            Dictionary<string, object> p = new Dictionary<string, object>
            {
                { AnalyticsParameter.context.ToString(), context.ToString() }
            };

            Analytics.CustomEvent(evt, p);
            GameAnalytics.NewDesignEvent($"{evt}:{context}");
            Print(evt, p);
        }

        public void LevelComplete(int difficulty)
        {
            AnalyticsEvent.LevelComplete(difficulty);
            GameAnalytics.NewDesignEvent($"level_complete:difficulty:{difficulty}");
            Print("LevelComplete:" + difficulty);
        }

        public void LevelFail(int difficulty)
        {
            AnalyticsEvent.LevelFail(difficulty);
            GameAnalytics.NewDesignEvent($"level_failed:difficulty:{difficulty}");
            Print("LevelFail:" + difficulty);
        }

        void Print(string name, Dictionary<string, object> parameters = null)
        {
            #if DEVELOPMENT_BUILD || UNITY_EDITOR
            StringBuilder builder = new StringBuilder();
            builder.Append("[TLANALYTICS] ");
            builder.Append(name);

            if (parameters != null)
            {
                builder.Append(" [PARAMS] ");

                foreach (KeyValuePair<string, object> kvp in parameters)
                {
                    builder.Append(kvp.Key + ":" + kvp.Value);
                    builder.Append(" , ");
                }
            }

            LogUtil.Log(builder, "yellow");
            #endif
        }

        public void DesignEvent(AnalyticsEventId evt)
        {
            var eventStr = evt.ToString();
            GameAnalytics.NewDesignEvent(eventStr);
            Print(eventStr);
        }

        public void DesignEvent(AnalyticsEventId evt, AnalyticsParameter param, object val, AnalyticsEventId subEvt)
        {
            var eventStr = string.Format("{0}:{1}{2}:{3}", evt, param, val, subEvt);
            GameAnalytics.NewDesignEvent(eventStr);
            Print(eventStr);
        }

        public void DesignEvent(AnalyticsEventId evt, params string[] contexts)
        {
            var eventStr = evt.ToString();

            foreach (var context in contexts)
            {
                eventStr += $":{context}";
            }

            GameAnalytics.NewDesignEvent(eventStr);
            Print(eventStr);
        }

        public void HEvent(string evt, params string[] param)
        {
            var evtStr = evt;
            if (param != null && param.Length > 0)
            {
                var paramDict = new Dictionary<string, object>();
                for (int i = 0; i < param.Length; i++)
                {
                    param[i] = string.IsNullOrEmpty(param[i]) ? "null" : param[i];
                    paramDict.Add($"ST{i + 1}", param[i]);
                    evtStr += $":{param[i]}";
                }

                Analytics.CustomEvent(evt, paramDict);
            }
            else
            {
                Analytics.CustomEvent(evt);
            }
            GameAnalytics.NewDesignEvent(evtStr);
        }

        public void ResourceEvent(GAResourceFlowType flowType, string currency, int amount, string itemType, string itemId)
        {
            GameAnalytics.NewResourceEvent(flowType, currency, (float)amount, itemType, itemId);

            var paramDict = new Dictionary<string, object>();
            paramDict.Add("currency", currency);
            paramDict.Add("amount", amount);
            paramDict.Add("itemType", itemType);
            paramDict.Add("itemId", itemId);

            Analytics.CustomEvent(flowType.ToString(), paramDict);
            Print(flowType.ToString(), paramDict);
        }

        public void ValueEvent(AnalyticsEventId evt, string context, float val)
        {
            GameAnalytics.NewDesignEvent($"{evt}:{context}", val);
            Print($"{evt}:{context}:{val}");
        }

        public void ValueEvent(string evt, AnalyticsContext context, float val)
        {
            GameAnalytics.NewDesignEvent($"{evt}:{context}", val);
            Print($"{evt}:{context}:{val}");
        }
    }
}
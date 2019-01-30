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

namespace TurboLabz.InstantFramework
{
    public class UnityAnalyticsService : IAnalyticsService
    {
        public void ScreenVisit(AnalyticsScreen screen)
        {
            AnalyticsEvent.ScreenVisit(screen.ToString());

            Print("ScreenVisit:" + screen);
        }

        public void ScreenVisit(AnalyticsScreen screen, bool fb_logged_in)
        {
            Dictionary<string, object> p = new Dictionary<string, object>
            {
                { AnalyticsParameter.fb_logged_in.ToString(), fb_logged_in }
            };

            AnalyticsEvent.ScreenVisit(screen.ToString(), p);

            Print("ScreenVisit:" + screen, p);
        }

        public void ScreenVisit(AnalyticsScreen screen, bool fb_logged_in, bool is_bot) 
        {
            Dictionary<string, object> p = new Dictionary<string, object>
            {
                { AnalyticsParameter.fb_logged_in.ToString(), fb_logged_in },
                { AnalyticsParameter.is_bot.ToString(), is_bot },
            };

            AnalyticsEvent.ScreenVisit(screen.ToString(), p);

            Print("ScreenVisit:" + screen, p);
        }

        public void ScreenVisit(AnalyticsScreen screen, AnalyticsContext context)
        {
            Dictionary<string, object> p = new Dictionary<string, object>
            {
                { AnalyticsParameter.context.ToString(), context.ToString() }
            };

            AnalyticsEvent.ScreenVisit(screen.ToString(), p);

            Print("ScreenVisit:" + screen, p);
        }

        public void Event(AnalyticsEventId evt)
        {
            Analytics.CustomEvent(evt.ToString());

            Print(evt.ToString());
        }

        public void Event(AnalyticsEventId evt, AnalyticsParameter param, object val)
        {
            if (param == AnalyticsParameter.elo)
            {
                int rating = (int)val;
                val = rating - (rating % 500);
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

            Analytics.CustomEvent(evt.ToString(), p);

            Print(evt.ToString(), p);
        }

        public void Event(AnalyticsEventId evt, AnalyticsContext context)
        {
            Dictionary<string, object> p = new Dictionary<string, object>
            {
                { AnalyticsParameter.context.ToString(), context.ToString() }
            };

            Analytics.CustomEvent(evt.ToString(), p);

            Print(evt.ToString(), p);
        }

        public void LevelComplete(int difficulty)
        {
            AnalyticsEvent.LevelComplete(difficulty);

            Print("LevelComplete:" + difficulty);
        }

        public void LevelFail(int difficulty)
        {
            AnalyticsEvent.LevelFail(difficulty);

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
    }
}
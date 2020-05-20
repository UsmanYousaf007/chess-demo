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
    public class UnityAnalyticsServiceEditor : IAnalyticsService
    {
        public void ScreenVisit(AnalyticsScreen screen)
        {
            Print("ScreenVisit:" + screen);
        }

        public void ScreenVisit(AnalyticsScreen screen, bool fb_logged_in)
        {
            Dictionary<string, object> p = new Dictionary<string, object>
            {
                { AnalyticsParameter.fb_logged_in.ToString(), fb_logged_in }
            };

            Print("ScreenVisit:" + screen, p);
        }

        public void ScreenVisit(AnalyticsScreen screen, bool fb_logged_in, bool is_bot)
        {
            Dictionary<string, object> p = new Dictionary<string, object>
            {
                { AnalyticsParameter.fb_logged_in.ToString(), fb_logged_in },
                { AnalyticsParameter.is_bot.ToString(), is_bot },
            };

            Print("ScreenVisit:" + screen, p);
        }

        public void ScreenVisit(AnalyticsScreen screen, AnalyticsContext context)
        {
            Dictionary<string, object> p = new Dictionary<string, object>
            {
                { AnalyticsParameter.context.ToString(), context.ToString() }
            };

            Print("ScreenVisit:" + screen, p);
        }

        public void Event(AnalyticsEventId evt)
        {
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

            Print(evt.ToString(), p);
        }

        public void Event(AnalyticsEventId evt, AnalyticsContext context)
        {
            Dictionary<string, object> p = new Dictionary<string, object>
            {
                { AnalyticsParameter.context.ToString(), context.ToString() }
            };

            Print(evt.ToString(), p);
        }

        public void Event(string evt, AnalyticsContext context)
        {
            Dictionary<string, object> p = new Dictionary<string, object>
            {
                { AnalyticsParameter.context.ToString(), context.ToString() }
            };

            Print(evt, p);
        }

        public void LevelComplete(int difficulty)
        {
            Print("LevelComplete:" + difficulty);
        }

        public void LevelFail(int difficulty)
        {
            Print("LevelFail:" + difficulty);
        }

        void Print(string name, Dictionary<string, object> parameters = null)
        {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            StringBuilder builder = new StringBuilder();
            builder.Append("[EDITOR_ANALYTICS] ");
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

        public void DesignEvent(AnalyticsEventId evt, AnalyticsParameter param, object val, AnalyticsEventId subEvt)
        {
            var eventStr = string.Format("{0}:{1}{2}:{3}", evt, param, val, subEvt);
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
                    paramDict.Add($"ST{i + 1}", param[i]);
                    evtStr += $":{param[i]}";
                }

                Print(evt, paramDict);
            }
            else
            {
                Print(evt);
            }
            Print(evtStr);
        }
    }
}
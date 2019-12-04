﻿using System.Collections.Generic;
using strange.extensions.command.impl;
using UnityEngine;
using UnityEngine.Networking;

namespace TurboLabz.InstantFramework
{
    public class ContactSupportCommand : Command
    {
        //Services
        [Inject] public IAnalyticsService analyticsService { get; set; }

        //Models
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IAppInfoModel appInfoModel { get; set; }

        public override void Execute()
        {
            string email = Settings.SUPPORT_EMAIL;
            string subject = MyEscapeURL("Feeback");
            string body = MyEscapeURL("\r\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n ***** DO NOT REMOVE THE TEXT BELOW *******" + AddPlayerData());

            Application.OpenURL("mailto:" + email + "?subject=" + subject + "&body=" + body);

            analyticsService.Event(AnalyticsEventId.tap_support);
        }

        private string MyEscapeURL(string URL)
        {
            return UnityWebRequest.EscapeURL(URL).Replace("+", "%20");
        }

        private string AddPlayerData()
        {
            Dictionary<string, string> data = new Dictionary<string, string>();

            data.Add("DisplayName", playerModel.name);
            data.Add("ClientVersion", appInfoModel.clientVersion);
            data.Add("EditedName", playerModel.editedName);
            data.Add("DeviceModel", SystemInfo.deviceModel);
            data.Add("OsVersion", SystemInfo.operatingSystem);
            data.Add("Memory", SystemInfo.systemMemorySize + " MB");

            if (playerModel.isPremium)
            {
                data.Add("PlayerTag-P", playerModel.tag);
            }
            else
            {
                data.Add("PlayerTag-F", playerModel.tag);
            }

            string playerData = "\n";

            foreach (KeyValuePair<string, string> entry in data)
            {
                playerData += "\n" + entry.Key + " : " + entry.Value.ToString();
            }

            return playerData;

        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocialEdge.Requests;
using SocialEdge.Communication;
using System;
using SocialEdge.Configuration;
using PlayFab.ClientModels;
using MongoDB.Driver;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SocialEdge.Utils;
using PlayFab;

public class SocialEdgeInit
{
    public CommunicationHub communicationHub;
    private MongoClient _client;
    public void Init()
    {
        LoginPlayer();
        DatabaseInit();
        CommunicationHubInit();
        GameInit();
        PlayerInit();
    }

    private void DatabaseInit()
    {
        var connectionString = Configurations.MongoConnectionString;
        var database = _client.GetDatabase(Configurations.MongoDatabase);
    }


    private void PlayerInit()
    {
        new SocialEdgeGetPlayerDataRequest().Send();
    }

    private void GameInit()
    {
        new SocialEdgeGetTitleDataRequest().SetSuccessCallback(GetTitleDataSuccessCallBack)
                                           .SetFailureCallback(GetTitleDataFailureCallBack)
                                           .Send();

        new SocialEdgeGetStoreItemsRequest(SocialEdge.Configuration.GameSettings.Store, SocialEdge.Configuration.GameSettings.CatalogueVersion)
                                            .SetSuccessCallback(this.GetStoreItemsSuccessCallBack)
                                            .SetFailureCallback(this.GetStoreItemsFailureCallBack)
                                            .Send();

        new SocialEdgeCloudScriptRequest("getTitleObjects").SetSuccessCallback(GetGameSettingsSuccessCallBack)
                                            .SetFailureCallback(GetGameSettingsFailureCallBack).Send();

       // PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
       // {
       //     FunctionName = "getTitleObjects",
       //     GeneratePlayStreamEvent = true
       // },
       //(objs) =>
       //{
       //    dynamic scriptResult = objs.FunctionResult;
       //    if (scriptResult != null)
       //    {
       //        Dictionary<string, string> settings = new Dictionary<string, string>();
       //        SettingsView settingsView = new SettingsView();
       //        var objects = scriptResult[0][1];

       //        foreach (var item in objects)
       //        {
       //            settings[item.Value["ObjectName"]] = item.Value["DataObject"];
       //        }

       //        Task<GameSettings> gameSettings = ParseGameSettings(settings["GameSettings"]);
       //        Task<GameEconomySettings> gameEconomySettings = ParseGameEconSettings(settings["GameEconomySettings"]);

       //        Task.WaitAll(gameSettings, gameEconomySettings);
       //        settingsView.gameEconomySettings = gameEconomySettings.Result;
       //        settingsView.gameSettings = gameSettings.Result;
       //        Debug.Log("we at end");
       //    }
       //},
       //OnFailed);

    }

    public void GetGameSettingsSuccessCallBack(SocialEdgeCloudScriptResponse resp)
    {

        //dynamic scriptResult = objs.FunctionResult;
        //if (scriptResult != null)
        //{
        //    Dictionary<string, string> settings = new Dictionary<string, string>();
        //    SettingsView settingsView = new SettingsView();
        //    var objects = scriptResult[0][1];

        //    foreach (var item in objects)
        //    {
        //        settings[item.Value["ObjectName"]] = item.Value["DataObject"];
        //    }

        //    Task<GameSettings> gameSettings = ParseGameSettings(settings["GameSettings"]);
        //    Task<GameEconomySettings> gameEconomySettings = ParseGameEconSettings(settings["GameEconomySettings"]);

        //    Task.WaitAll(gameSettings, gameEconomySettings);
        //    settingsView.gameEconomySettings = gameEconomySettings.Result;
        //    settingsView.gameSettings = gameSettings.Result;
        //}
    }
    public void GetGameSettingsFailureCallBack(SocialEdgeCloudScriptResponse resp)
    { }

    public void GetStoreItemsSuccessCallBack(SocialEdgeGetStoreItemsResponse resp)
    {
        /*Populate some data structure*/
    }

    public void GetStoreItemsFailureCallBack(SocialEdgeGetStoreItemsResponse resp)
    { }

    public void GetTitleDataSuccessCallBack(SocialEdgeGetTitleDataResponse resp)
    {
        /*Populate some data structure*/
    }

    public void GetTitleDataFailureCallBack(SocialEdgeGetTitleDataResponse resp)
    { }

    private void CommunicationHubInit()
    {
        communicationHub = new CommunicationHub();
        communicationHub.Setup();
    }

    private void LoginPlayer()
    {
        new SocialEdgeBackendLoginRequest().SetSuccessCallback(LoginSuccessCallBack)
                                           .SetFailureCallback(LoginFailureCallBack)
                                           .GetBasicInfo()
                                           .Send();
    }


    public void LoginSuccessCallBack(SocialEdgeBackendLoginResponse resp)
    {
        Debug.Log("login callback");
        CommunicationHubInit();
        GameInit();
        PlayerInit();

    }

    public void LoginFailureCallBack(SocialEdgeBackendLoginResponse resp)
    {
        Debug.Log("login callback");

    }

    private async Task<GameEconomySettings> ParseGameEconSettings(dynamic gameEconomySettings)
    {
        Debug.Log("we at econ");
        string decompressedJson = UtilityMethods.Decompress(gameEconomySettings);
        dynamic decompressedObject = JsonConvert.DeserializeObject(decompressedJson);
        GameEconomySettings settings = JsonConvert.DeserializeObject<GameEconomySettings>(decompressedObject);
        return settings;
    }

    private async Task<GameSettings> ParseGameSettings(dynamic gameSettings)
    {
        Debug.Log("we at g settings");
        string decompressedJson = UtilityMethods.Decompress(gameSettings);
        dynamic decompressedObject = JsonConvert.DeserializeObject(decompressedJson);
        GameSettings settings = JsonConvert.DeserializeObject<GameSettings>(decompressedObject);
        return settings;
    }
}

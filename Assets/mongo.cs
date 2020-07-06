using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text.RegularExpressions;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using PlayFab;
using PlayFab.AuthenticationModels;
using PlayFab.ClientModels;
using PlayFab.DataModels;
using SocialEdge.Modules;
using Newtonsoft.Json;
using SocialEdge.Types;
using SocialEdge.Utils;
using UnityEngine;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

public class mongo : MonoBehaviour
{
    private MongoClient _client;
    private IMongoCollection<BsonDocument> _collection;
    string entityId;
    string entityType;
    // Start is called before the first frame update
    void Start()
    {
        var req = new PlayFab.ClientModels.LoginWithCustomIDRequest
        {
            CustomId = "user0",
            CreateAccount = false
        };
        PlayFabClientAPI.LoginWithCustomID(req, OnSuccess, OnFailed);

        //var req2= new PlayFabApiSettings

        var data = new Dictionary<string, object>()
        {
            {"Health", 100},
            {"Mana", 10000}
        };
        var dataList = new List<SetObject>()
        {
            new SetObject()
            {
                ObjectName = "PlayerData",
                DataObject = data
            },
            // A free-tier customer may store up to 3 objects on each entity
        };

        

        //PlayFabDataAPI.SetObjects(new SetObjectsRequest()
        //{
        //    Entity = new EntityKey { Id = entityId, Type = entityType }, // Saved from GetEntityToken, or a specified key created from a titlePlayerId, CharacterId, etc
        //    Objects = dataList,
        //}, (setResult) => {
        //    Debug.Log(setResult.ProfileVersion);
        //}, OnFailed);

        //PlayFabAuthenticationAPI.GetEntityToken(new GetEntityTokenRequest()
        //{
           
        //}, (entityResult) =>
        //{
        //    entityId = entityResult.Entity.Id;
        //    entityType = entityResult.Entity.Type;

            //PlayFabDataAPI.SetObjects(new SetObjectsRequest
            //{
            //    Entity = new PlayFab.DataModels.EntityKey { Id = entityId, Type = entityType },
            //    Objects = dataList
            //},


        //},
        //    OnFailed
        //) ;

        
            _client = new MongoClient("mongodb+srv://MyMongoDBUser:mongodbpassword123@testcluster-hsxfp.mongodb.net/test?retryWrites=true&w=majority");
        var database = _client.GetDatabase("TestDatabase");
        _collection = database.GetCollection<BsonDocument>("Players");

        var results = Search("Player", 0, 10, new PlayerModel(), "abc");
    }

    public void OnEntityTokenSuccess(GetEntityTokenResponse resp)
    {
        entityId = resp.Entity.Id;
        entityType = resp.Entity.Type;
    }

    public void OnAddFriendSuccess(GetFriendsListResult res)
    { }

    public void OnSuccess(LoginResult result)
    {

        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
        {
            FunctionName = "getTitleObjects", 
            GeneratePlayStreamEvent = true 
        },
        (objs) =>
                {   dynamic scriptResult = objs.FunctionResult;
                    if (scriptResult != null)
                    {
                        Dictionary<string, string> settings = new Dictionary<string, string>();
                        SettingsView settingsView = new SettingsView();
                        var objects = scriptResult[0][1];

                        foreach (var item in objects){
                            settings[item.Value["ObjectName"]] = item.Value["DataObject"];
                        }

                        Task<GameSettings> gameSettings= ParseGameSettings(settings["GameSettings"]);
                        Task<GameEconomySettings> gameEconomySettings = ParseGameEconSettings(settings["GameEconomySettings"]);

                        Task.WaitAll(gameSettings, gameEconomySettings);
                        settingsView.gameEconomySettings = gameEconomySettings.Result;
                        settingsView.gameSettings = gameSettings.Result;
                        Debug.Log("we at end");
                    }
                },
        OnFailed);


        var req2 = new PlayFab.ClientModels.GetFriendsListRequest
        {
        };

        PlayFabClientAPI.GetFriendsList(req2, OnAddFriendSuccess, OnFailed);

        var req = new GetStoreItemsRequest {
            StoreId = "StoreId1"            

        };
        PlayFabClientAPI.GetStoreItems(req,OnCatSuccess,OnFailed);
    }

    private async Task<GameEconomySettings> ParseGameEconSettings(dynamic gameEconomySettings)
    {
        Debug.Log("we at econ");
        string decompressedJson = UtilityMethods.Decompress(gameEconomySettings);
        dynamic decompressedObject = JsonConvert.DeserializeObject(decompressedJson);
        GameEconomySettings settings =  JsonConvert.DeserializeObject<GameEconomySettings>(decompressedObject);        
        return settings;
    }

    private async Task<GameSettings> ParseGameSettings(dynamic gameSettings)
    {
        Debug.Log("we at g settings");
        string decompressedJson = UtilityMethods.Decompress(gameSettings);
        dynamic decompressedObject = JsonConvert.DeserializeObject(decompressedJson);
        GameSettings settings =  JsonConvert.DeserializeObject<GameSettings>(decompressedObject);
        return settings;
    }

    public void OnCatSuccess(GetStoreItemsResult res)
    {
        var req = new PlayFab.ClientModels.PurchaseItemRequest{
            ItemId="One",
            Price=20,
            VirtualCurrency="GD",
            StoreId="StoreId1"
        };
        Console.WriteLine("Cato get");

        //PlayFabClientAPI.PurchaseItem(req, OnPurchaseSuccess, OnFailed);


        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), OnInventorySucces, OnFailed);

    }

    public void OnPurchaseSuccess(PurchaseItemResult result)
    {
        Console.WriteLine("Purchase successful woohoo money spent");
    }

    public void OnInventorySucces(GetUserInventoryResult result)
    {
        Console.WriteLine("Inventory gotten phrrrrrr");
    }
    public void OnFailed(PlayFabError result)
    {
        Console.WriteLine("Phaiiiilllll");
    }

    public FriendModel Create(string playerId)
    {
        FriendModel friend = new FriendModel();
        friend.publicProfile = new PlayerModel().GetPublicProfile(playerId);
        friend.friendType = FriendTypes.FRIEND_TYPE_SOCIAL;
        return friend;
    }

    public Dictionary<string, FriendModel> Search(string matchString, int skipDocs, int limitDocs, PlayerModel playerData, string playerId)
    {
        Dictionary<string, FriendModel> result = null;
        List<PlayerModel> players = null;
        try
        {
            string pattern = "^" + matchString + ".*";
            var filter = Builders<BsonDocument>.Filter.Regex("Name", pattern);
            var projection = Builders<BsonDocument>.Projection.Include("_id");
            var bsonDocs = _collection.Find(filter).Project(projection).Skip(skipDocs).Limit(limitDocs).ToList();

            var list = new List<PlayerModel>();

            foreach (var doc in bsonDocs)
            {
                var a = BsonSerializer.Deserialize<PlayerModel>(doc);
                list.Add(a);
            }

            if (list != null && list.Count > 0)
            {
                players = SearchFilter(list, playerData, playerId);

                if (players != null && players.Count > 0)
                {
                    result = new Dictionary<string, FriendModel>();
                    foreach (var player in players)
                    {
                        var friend = Create(player.id);
                        friend.friendType = GetFriendType(playerData, player.id);
                        result[player.id] = friend;
                    }
                }
            }
        }
        catch (Exception e)
        {
            throw new Exception("Exception thrown from Friend.Search. " + e.Message, e.InnerException);
        }
        return result;
    }

    private string GetFriendType(PlayerModel player, string friendId)
    {
        string type = null;
        try
        {
            type = FriendTypes.FRIEND_TYPE_COMMUNITY;
            var friend = player.privateProfile.friends.ContainsKey(friendId) ? player.privateProfile.friends[friendId] : null;
            if (friend != null)
            {
                type = friend.friendType;
            }
        }

        catch (Exception e)
        {
            throw new Exception("Exception thrown from Friend.GetFriendType. " + e.Message, e.InnerException);
        }
        return type;
    }

    private List<PlayerModel> SearchFilter(List<PlayerModel> result, PlayerModel playerData, string playerId)
    {
        List<PlayerModel> players = new List<PlayerModel>();
        try
        {

            foreach (var i in result)
            {
                if (i.id != playerId && !playerData.privateProfile.blocked.ContainsKey(i.id))
                {
                    players.Add(i);
                }
            }
        }
        catch (Exception e)
        {
            throw new Exception("Exception thrown from Friend.SearchFilter. " + e.Message, e.InnerException);
        }

        return players;
    }

    /*Search by tag text and case insensitive*/
    public Dictionary<string, FriendModel> SearchTag(string tag, PlayerModel playerData, string playerId)
    {
        Dictionary<string, FriendModel> result = null;

        try
        {
            var filter = Builders<BsonDocument>.Filter.Regex("tag", tag);
            var projection = Builders<BsonDocument>.Projection.Include("_id");
            var bsonDocs = _collection.Find(filter).Project(projection).Limit(1).ToList();
            var searchedPlayer = new List<PlayerModel>();

            foreach (var doc in bsonDocs)
            {
                var a = BsonSerializer.Deserialize<PlayerModel>(doc);
                searchedPlayer.Add(a);
            }
            if (searchedPlayer != null && searchedPlayer.Count > 0)
            {
                var filteredPlayer = SearchFilter(searchedPlayer, playerData, playerId).FirstOrDefault();
                if (filteredPlayer != null)
                {
                    result = new Dictionary<string, FriendModel>();
                    var friend = Create(filteredPlayer.id);
                    friend.friendType = GetFriendType(playerData, filteredPlayer.id);
                    result[filteredPlayer.id] = friend;
                }
            }
        }
        catch (Exception e)
        {
            throw new Exception("Exception thrown from Friend.SearchTag. " + e.Message, e.InnerException);
        }
        return result;
    }

    
}

public class SettingsView{
    public GameSettings gameSettings;
    public GameEconomySettings gameEconomySettings;
}

public class GameSettings {

    [JsonProperty("Meta")]
    MetaSettings Meta { get; set; }
    [JsonProperty("Community")]
    CommunitySettings Community { get; set; }
    [JsonProperty("Friends")]
    FriendsSettings Friends { get; set; }
    [JsonProperty("Common")]
    CommonSettings Common { get; set; }

}

public class MetaSettings
{

    public string androidURL { get; set; }
    public string iosURL { get; set; }
    public int rateAppThreshold { get; set; }
    public int backendAppVersion { get; set; }
    public string contactSupportURL { get; set; }
}

public class CommunitySettings
{
    public int expireAfterSeconds { get; set; }
    public int maxSuggests { get; set; }
    public int minSuggests {get;set;}
    public int domesticPct { get; set; }
}

public class FriendsSettings {

    public int searchExpireAfterSeconds { get; set; }
    public int searchPageMax { get; set; }
}

public class CommonSettings {
    public bool maintenanceFlag { get; set; }
    public bool maintenanceWarningFlag { get; set; }
    public string nmaintenanceWarningMessage { get; set; }
    public string maintenanceMessage { get; set; }
    public string updateMessage { get; set; }
    public int maxLongMatchCount { get; set; }
    public int maxFriendsCount { get; set; }
    public int maxCommunityMatches { get; set; }
    public int maxRecentlyCompletedMatchCount { get; set; }
    public string maintenanceWarningBgColor { get; set; }

    [JsonProperty("ios")]
    public MobileVersionSettings ios { get; set; }
    [JsonProperty("android")]
    public MobileVersionSettings android { get; set; }
    [JsonProperty("premium")]
    public PremiumSettings premium { get; set; }
}

public class MobileVersionSettings
{
    public string minimumClientVersion { get; set; }
    public string gameUpdateBannerMsg { get; set; }
    public string manageSubscriptionURL { get; set; }
}

public class PremiumSettings {
    public int maxLongMatchCount { get; set; }
    public int maxFriendsCount { get; set; }
}

public class GameEconomySettings {
    [JsonProperty("PlayerDefaults")]
    PlayerDefaultsSettings playerDefaults;
    [JsonProperty("PlayerDefaultOwnedItems")]
    List<PlayerDefaultOwnedItemsSettings> playerDefaultOwnedItems;
    [JsonProperty("Rewards")]
    RewardsSettings rewards;
    [JsonProperty("Ads")]
    AdsSettings ads;
}

public class PlayerDefaultsSettings {
    public int CURRENCY2 { get; set; }
}

public class PlayerDefaultOwnedItemsSettings {
    public string shopItemKey;
    public int quantity;
}

public class RewardsSettings
{
    public decimal matchWinReward;
    public decimal matchWinAdReward;
    public decimal matchRunnerUpReward;
    public decimal matchRunnerUpAdReward;
    public decimal rewardMatchPromotional;
    public int failSafe;
    public decimal facebookConnectReward;
}

public class AdsSettings
{
    public int adsGlobalCap;
    public int adsInterstitialCap;
    public int adsRewardedVideoCap;
    public int resignCap;
    public int ADS_SLOT_HOUR;
    public int ADS_FREE_NO_ADS_PERIOD;
    public int minutesForVictoryInteralAd;
    public int autoSubscriptionDlgThreshold;
    public int daysPerAutoSubscriptionDlgThreshold;
    public int sessionsBeforePregameAd;
    public int maxPregameAdsPerDay;
    public int intervalsBetweenPregameAds;
    public decimal waitForPregameAdLoadSeconds;
    public bool showPregameOneMinute;
}

//{
//  "PlayerDefaults": {
//    "CURRENCY2": 50
//  },
//  "PlayerDefaultOwnedItems": [
//    {
//      "shopItemKey": "SkinSlate",
//      "quantity": 1
//    },
//    {
//      "shopItemKey": "PowerUpHindsight",
//      "quantity": 6
//    },
//    {
//      "shopItemKey": "PowerUpHint",
//      "quantity": 6
//    }
//  ],
//  "Rewards": {
//    "matchWinReward": 0.5,
//    "matchWinAdReward": 1,
//    "matchRunnerUpReward": 0.5,
//    "matchRunnerUpAdReward": 1,
//    "rewardMatchPromotional": 0.25,
//    "failSafe": 20,
//    "facebookConnectReward": 50
//  },
//  "Ads": {
//    "adsGlobalCap": 0,
//    "adsInterstitialCap": 0,
//    "adsRewardedVideoCap": 10,
//    "resignCap": 6,
//    "ADS_SLOT_HOUR": 19,
//    "ADS_FREE_NO_ADS_PERIOD": 0,
//    "minutesForVictoryInteralAd": 0,
//    "autoSubscriptionDlgThreshold": 2,
//    "daysPerAutoSubscriptionDlgThreshold": 1,
//    "sessionsBeforePregameAd": 1,
//    "maxPregameAdsPerDay": 100,
//    "intervalsBetweenPregameAds": 1,
//    "waitForPregameAdLoadSeconds": 0,
//    "showPregameOneMinute": false
//  }
//}
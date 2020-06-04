using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using SocialEdge.Modules;
using UnityEngine;

public class mongo : MonoBehaviour
{
    Friend _friend;
    private MongoClient _client;
    private IMongoCollection<BsonDocument> _collection;
    // Start is called before the first frame update
    void Start()
    {
        _friend = new Friend();
        _client = new MongoClient("mongodb+srv://MyMongoDBUser:mongodbpassword123@testcluster-hsxfp.mongodb.net/test?retryWrites=true&w=majority");
        var database = _client.GetDatabase("TestDatabase");
        _collection = database.GetCollection<BsonDocument>("Players");

        var results = Search("Player", 0, 10, new PlayerModel(), "abc");
    }

    public Dictionary<string, Friend> Search(string matchString, int skipDocs, int limitDocs, PlayerModel playerData, string playerId)
    {
        Dictionary<string, Friend> result = null;
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
                    result = new Dictionary<string, Friend>();
                    foreach (var player in players)
                    {
                        var friend = _friend.Create(player.id);
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
    public Dictionary<string, Friend> SearchTag(string tag, PlayerModel playerData, string playerId)
    {
        Dictionary<string, Friend> result = null;

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
                    result = new Dictionary<string, Friend>();
                    var friend = _friend.Create(filteredPlayer.id);
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

    // Update is called once per frame
    void Update()
    {
        
    }
}

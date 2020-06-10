using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using SocialEdge.Types;
using SocialEdge.Utils;

namespace SocialEdge.Modules
{
    public class Friends
    {
        private MongoClient _client;
        private IMongoCollection<BsonDocument> _collection;

        /*basic setup for now*/
        public Friends()
        {
            _client = new MongoClient("mongodb+srv://MyMongoDBUser:mongodbpassword123@testcluster-hsxfp.mongodb.net/test?retryWrites=true&w=majority");
            var database = _client.GetDatabase("TestDatabase");
            _collection = database.GetCollection<BsonDocument>("Players");
        }

        public FriendModel Create(string playerId)
        {
            FriendModel friend = new FriendModel();
            friend.publicProfile = new PlayerModel().GetPublicProfile(playerId);
            friend.friendType = FriendTypes.FRIEND_TYPE_SOCIAL;
            return friend;
        }

        public FriendModel Add(PlayerModel player, string friendId)
        {
            var friend = Create(friendId);

            if (friend.publicProfile.eloScore != 0)
            {
                player.privateProfile.friends[friendId] = friend;
                return friend;
            }
            return null;
        }

        public bool Remove(PlayerModel player, string friendId)
        {
            if (player.privateProfile.friends.ContainsKey(friendId))
            {
                player.privateProfile.friends.Remove(friendId);
                return true;
            }
            return false;
        }


        //        var block = function(sparkPlayer, friendId) {
        //        var publicProfile = PlayerModel.getPublicProfile(friendId);
        //        playerData.priv.blocked[friendId] = {name: publicProfile.name
        //    };
        //    delete playerData.priv.friends[friendId];
        //}
        ///* -------------------------------------------------------------------------------- */
        //var attachPublicProfiles = function(sparkPlayer) {
        //        var playerData = PlayerModel.get(sparkPlayer);
        //        for (var id in playerData.priv.friends) {
        //            playerData.priv.friends[id].publicProfile = PlayerModel.getPublicProfile(id);
        //        }
        //    };

        public void Block(PlayerModel player, string friendId)
        {
            var friend = new PlayerModel().GetPublicProfile(friendId);
            player.privateProfile.blocked[friendId] = friend.name;
            player.privateProfile.friends.Remove(friendId);

        }

        public void AttachPublicProfiles(PlayerModel player)
        {
            //TBD
            //foreach (var id in player.privateProfile.friends)
            //{
            //    player.privateProfile.friends[id].publicProfile = new PlayerModel().GetPublicProfile(id);
            //}
        }

        public Dictionary<string, FriendModel> Search(string matchString, int skipDocs, int limitDocs, PlayerModel playerData, string playerId)
        {
            Dictionary<string, FriendModel> result = null;
            List<PlayerModel> list, players = null;
            
            try
            {
                string pattern = "^" + matchString + ".*";
                var filter = Builders<BsonDocument>.Filter.Regex("Name", pattern);
                var projection = Builders<BsonDocument>.Projection.Include("_id");
                var bsonDocs = _collection.Find(filter).Project(projection).Skip(skipDocs).Limit(limitDocs).ToList();

                if (!bsonDocs.Empty())
                {
                    list = new List<PlayerModel>();
                    foreach (var doc in bsonDocs)
                    {
                        var deserializedDoc = BsonSerializer.Deserialize<PlayerModel>(doc);
                        list.Add(deserializedDoc);
                    }
                    
                    players = SearchFilter(list, playerData, playerId);

                    if (!players.Empty())
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
            string type = FriendTypes.FRIEND_TYPE_COMMUNITY;
            try
            {
                var friend = player.privateProfile.friends.ContainsKey(friendId) ? player.privateProfile.friends[friendId] : null;
                if (friend != null) type = friend.friendType;
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
                        players.Add(i);
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
                if (!searchedPlayer.Empty())
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
}
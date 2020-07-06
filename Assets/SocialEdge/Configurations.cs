using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SocialEdge.Configuration
{
    public static class Configurations
    {
        private const string _mongoConnectionString = "mongodb+srv://MyMongoDBUser123:mongodbpassword123@testcluster-hsxfp.mongodb.net/test?retryWrites=true&w=majority";
        private const string _mongoDatabase = "TestDatabase";
        private const string _azureUrl = "https://chessstars.azurewebsites.net/api";

        public static string MongoConnectionString
        {
            get { return _mongoConnectionString; }
        }

        public static string MongoDatabase
        {
            get { return _mongoDatabase; }
        }

        public static string AzuerUrl
        {
            get { return _azureUrl; }
        }
    }
}
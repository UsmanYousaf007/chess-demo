using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SocialEdge.Configuration
{
    public static class GameSettings
    {
        private static string _storeId = "StoreId1";
        private static string _catalogueVersion = "";

        public static string Store
        {
            get { return _storeId; }
        }

        public static string CatalogueVersion
        {
            get { return _catalogueVersion; }
        }
    }
}
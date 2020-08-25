
using UnityEngine;
using System.Collections.Generic;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TurboLabz.InstantFramework
{
    // Class name to match the script file name
    [System.Serializable]
    public class TournamentAssetsContainer : ScriptableObject
    {
        const string ASSET_NAME = "TournamentAssets";
        public TournamentAsset[] assets;

        private Dictionary<string, TournamentAsset> assetsDict;

        public static TournamentAssetsContainer Load()
        {
            TournamentAssetsContainer assetsContainer = Resources.Load(ASSET_NAME) as TournamentAssetsContainer;
            assetsContainer.PopulateDict();
            return assetsContainer;
        }

        public Sprite GetSticker(string tournamentType)
        {
            if (assetsDict.ContainsKey(tournamentType))
            {
                return assetsDict[tournamentType].stickerSprite;
            }

            return null;
        }

        public Sprite GetTile(string tournamentType)
        {
            if (assetsDict.ContainsKey(tournamentType))
            {
                return assetsDict[tournamentType].tileSprite;
            }

            return null;
        }

        private void PopulateDict ()
        {
            assetsDict = new Dictionary<string, TournamentAsset>(assets.Length + 1);
            for (int i = 0; i < assets.Length; i++)
            {
                assetsDict.Add(assets[i].type, assets[i]);
            }
        }

        [System.Serializable]
        public class TournamentAsset
        {
            [Header("Type must correspond to the tournament type values in TournamentConstants.cs")]
            public string type;
            public Sprite stickerSprite;
            public Sprite tileSprite;
        }

#if UNITY_EDITOR
        const string ASSET_PATH = "Assets/Game/Images/Resources/";
        
        [MenuItem("Assets/Create/Turbolabz/Tournament Assets")]
        public static void CreateAsset()
        {
            ScriptableObject.CreateInstance<TournamentAssetsContainer>().Build();
        }

        public void Build()
        {
            AssetBuilder.Build(this, ASSET_NAME, ASSET_PATH);
        }
#endif
    }
}








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
    public class LeagueTierIconsContainer : ScriptableObject
    {
        const string ASSET_NAME = "LeagueTierIcons";
        public LeagueAsset[] assets;

        private Dictionary<string, LeagueAsset> assetsDict;

        public static LeagueTierIconsContainer Load()
        {
            LeagueTierIconsContainer assetsContainer = Resources.Load(ASSET_NAME) as LeagueTierIconsContainer;
            assetsContainer.PopulateDict();
            return assetsContainer;
        }

        public LeagueAsset GetAssets(string leagueType)
        {
            if (assetsDict.ContainsKey(leagueType))
            {
                return assetsDict[leagueType];
            }

            return null;
        }

        private void PopulateDict()
        {
            assetsDict = new Dictionary<string, LeagueAsset>(assets.Length + 1);
            for (int i = 0; i < assets.Length; i++)
            {
                assetsDict.Add(assets[i].typeID, assets[i]);
            }
        }

        [System.Serializable]
        public class LeagueAsset
        {
            [Header("Type must correspond to the league type values in TournamentConstants.cs")]
            public string typeName;
            public string typeID;
            public Sprite chestSprite;
            public Sprite trophySprite;
            public Sprite ringSprite;
            public Sprite bgSprite;
            public Sprite textUnderlaySprite;
            public Color borderColor;
        }

#if UNITY_EDITOR
        const string ASSET_PATH = "Assets/Game/Images/Resources/";

        [MenuItem("Assets/Create/Turbolabz/League Tier Icons")]
        public static void CreateAsset()
        {
            ScriptableObject.CreateInstance<LeagueTierIconsContainer>().Build();
        }

        public void Build()
        {
            AssetBuilder.Build(this, ASSET_NAME, ASSET_PATH);
        }
#endif
    }
}
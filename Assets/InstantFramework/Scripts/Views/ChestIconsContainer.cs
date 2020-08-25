
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
    public class ChestIconsContainer : ScriptableObject
    {
        const string ASSET_NAME = "ChestIcons";
        public ChestIcon [] icons;

        private Dictionary<string, Sprite> iconsDict;

        public static ChestIconsContainer Load()
        {
            ChestIconsContainer chestIconsContainer = Resources.Load(ASSET_NAME) as ChestIconsContainer;
            chestIconsContainer.PopulateDict();
            return chestIconsContainer;
        }

        public Sprite GetChest(string chestType)
        {
            if (iconsDict.ContainsKey(chestType))
            {
                return iconsDict[chestType];
            }

            return null;
        }

        private void PopulateDict ()
        {
            iconsDict = new Dictionary<string, Sprite>(icons.Length + 1);
            for (int i = 0; i < icons.Length; i++)
            {
                iconsDict.Add(icons[i].chestType, icons[i].iconSprite);
            }
        }

        [System.Serializable]
        public class ChestIcon
        {
            [Header("Chest Type must correspond to the chest type values in TournamentConstants.cs")]
            public string chestType;
            public Sprite iconSprite;
        }

#if UNITY_EDITOR
        const string ASSET_PATH = "Assets/Game/Images/Resources/";
        
        [MenuItem("Assets/Create/Turbolabz/Chest Icons")]
        public static void CreateAsset()
        {
            ScriptableObject.CreateInstance<ChestIconsContainer>().Build();
        }

        public void Build()
        {
            AssetBuilder.Build(this, ASSET_NAME, ASSET_PATH);
        }
#endif
    }
}








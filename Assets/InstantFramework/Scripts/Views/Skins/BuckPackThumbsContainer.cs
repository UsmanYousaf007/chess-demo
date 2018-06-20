
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using TurboLabz.TLUtils;
using TurboLabz.Chess;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TurboLabz.InstantFramework
{
	// Class name to match the script file name
	[System.Serializable]
	public class BuckPackThumbsContainer : ScriptableObject 
	{
		const string ASSET_NAME = "BuckPackThumbs";
		public List<Sprite> sprites = new List<Sprite>();

		public static BuckPackThumbsContainer Load()
		{
			return Resources.Load(ASSET_NAME) as BuckPackThumbsContainer;
		}

		public Sprite GetSprite(string key)
		{
			foreach (Sprite sprite in sprites)
			{
				if (sprite.name == key)
				{
					return sprite;
				}
			}

			return null;
		}

		#if UNITY_EDITOR
		const string ASSET_PATH = "Assets/Game/Images/Resources/";
		const string INPUT_PATH = "Assets/Game/Images/BuckPackThumbs";

		[MenuItem("Assets/Create/Turbolabz/BuckPack Thumbs")]
		public static void CreateAsset() 
		{
			ScriptableObject.CreateInstance<BuckPackThumbsContainer>().Build();
		}

		public void Build()
		{
			string sourcePath = INPUT_PATH;
			string outFileName = sourcePath.Substring(sourcePath.LastIndexOf("/") + 1);

			string[] files = Directory.GetFiles(INPUT_PATH , "*.png");

			foreach(string filePath in files)
			{
				Sprite sprite = new Sprite();
				sprite = AssetDatabase.LoadAssetAtPath<Sprite>(filePath);
				sprites.Add(sprite);
			}

			AssetBuilder.Build(this, outFileName, ASSET_PATH);
		}
		#endif
	}
}








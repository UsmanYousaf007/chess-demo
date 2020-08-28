
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
	public class SpritesContainer : ScriptableObject 
	{
		public List<Sprite> sprites = new List<Sprite>();
		public Dictionary<string, Sprite> dict = new Dictionary<string, Sprite>();

		public static SpritesContainer Load(string assetName)
		{
			SpritesContainer container = Resources.Load(assetName) as SpritesContainer;
			container.Init();
			return container;
		}

		private void Init()
        {
			foreach(Sprite sprite in sprites)
            {
				if (!dict.ContainsKey(sprite.name))
				{
					dict.Add(sprite.name, sprite);
				}
            }
        }

		public Sprite GetSprite(string key)
		{
			return dict.ContainsKey(key) ? dict[key] : null;
		}

		#if UNITY_EDITOR
		const string ASSET_PATH = "Assets/Game/Images/Resources/";
		const string INPUT_PATH = "Assets/InstantFramework/Images/SpriteBank/";

		[MenuItem("Assets/Create/Turbolabz/Sprite Container")]
		public static void CreateAsset() 
		{
			SpritesContainer container = ScriptableObject.CreateInstance<SpritesContainer> ();
			container.name = "Sprite Cointainer";
			container.Build();
		}

		public void Build()
		{
			string sourcePath = EditorUtility.OpenFolderPanel("Select Folder", INPUT_PATH, "");
			string outFileName = sourcePath.Substring(sourcePath.LastIndexOf("/") + 1);

			if (sourcePath == "")
			{
				Debug.Log("Asset creation operation cancelled.");
				return;
			}

			string[] files = Directory.GetFiles(sourcePath , "*.png");
			sourcePath = sourcePath.Substring(sourcePath.IndexOf("Assets"));

			foreach(string filePath in files)
			{
				string filepath = sourcePath + Path.DirectorySeparatorChar + Path.GetFileName(filePath);

				Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(filepath);
				sprites.Add(sprite);
			}
				
			AssetBuilder.Build(this, outFileName, ASSET_PATH);
		}
		#endif
	}
}








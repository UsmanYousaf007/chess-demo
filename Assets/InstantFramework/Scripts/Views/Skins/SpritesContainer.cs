
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

		public static SpritesContainer Load(string assetName)
		{
			return Resources.Load(assetName) as SpritesContainer;
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
		const string INPUT_PATH = "Assets/Game/Images/";

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

				Sprite sprite = new Sprite();
				sprite = AssetDatabase.LoadAssetAtPath<Sprite>(filepath);
				sprites.Add(sprite);
			}
				
			AssetBuilder.Build(this, outFileName, ASSET_PATH);
		}
		#endif
	}
}








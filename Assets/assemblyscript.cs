using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
//using ClassLibrary;
public class assemblyscript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //testclass t = new testclass();
        //Load texture from disk
        //TextAsset dataAsset = (TextAsset)Resources.Load("ClassLibrary", typeof(TextAsset));
        
        //var assembly = System.Reflection.Assembly.Load(dataAsset.bytes);
        //var type = assembly.GetType("ClassLibrary.testclass");
        //var fields = type.GetFields();
        //var types = assembly.GetTypes();


        var myLoadedAssetBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "dllbundle"));
        Debug.Log("Failed to load AssetBundle!");
        if (myLoadedAssetBundle == null)
        {
            Debug.Log("Failed to load AssetBundle!");
            return;
        }
        var abc = myLoadedAssetBundle.GetAllAssetNames();
        //var txt = myLoadedAssetBundle.LoadAsset("ClassLibrary", typeof(TextAsset)) as TextAsset;
        var txt = myLoadedAssetBundle.LoadAsset<TextAsset>("assets/classlibrary.bytes");
        // Load the TextAsset object


        // Load the assembly and get a type (class) from it
        var assembly = System.Reflection.Assembly.Load(txt.bytes);
        //var type = assembly.GetType("ClassLibrary.testclass");
        var prefab = myLoadedAssetBundle.LoadAsset<GameObject>("assets/instantframework/prefabs/testexternalgameobj.prefab");

        //prefab.AddComponent(type);
        Instantiate(prefab);

        myLoadedAssetBundle.Unload(false);

        //type.
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

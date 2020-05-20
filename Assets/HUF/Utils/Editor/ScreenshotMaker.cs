using System.Collections;
using System.IO;
using System.Linq;
using HUF.Utils.Runtime;
using UnityEditor;
using UnityEngine;

namespace HUF.Utils.Editor
{
    public class ScreenshotMaker : EditorWindow
    {
        const string WINDOW_TITLE = "Screenshot Maker";
        const string SCREENS_DIR = "Screenshots";
        const string OPEN_BUTTON_LABEL = "Open \"Screenshots\" directory";

        static readonly Vector2Int iPhoneRes = new Vector2Int(1242, 2208);
        static readonly Vector2Int iPadRes = new Vector2Int(2048, 2732);
        static readonly Vector2Int iPhoneXRes = new Vector2Int(1125, 2436);
        static readonly Vector2Int iPhoneXsRes = new Vector2Int(1242, 2688);
        static readonly Vector2Int nexus7Res = new Vector2Int(1200, 1920);
        static readonly Vector2Int nexus10Res = new Vector2Int(1600, 2560);

        enum ScreenshotType
        {
            iPhone,
            iPad,
            iPhoneX,
            iPhoneXs,
            Nexus7,
            Nexus10
        }

        struct ScreenshotData
        {
            public readonly ScreenshotType Type;
            public readonly Vector2Int Resolution;
            public readonly KeyCode KeyCode;

            public ScreenshotData(ScreenshotType type, Vector2Int resolution, KeyCode keyCode)
            {
                Type = type;
                Resolution = resolution;
                KeyCode = keyCode;
            }
        }

        static readonly ScreenshotData[] screensList =
        {
            new ScreenshotData(ScreenshotType.iPhone, iPhoneRes, KeyCode.Alpha1),
            new ScreenshotData(ScreenshotType.iPad, iPadRes, KeyCode.Alpha2),
            new ScreenshotData(ScreenshotType.iPhoneX, iPhoneXRes, KeyCode.Alpha3),
            new ScreenshotData(ScreenshotType.iPhoneXs, iPhoneXsRes, KeyCode.Alpha4),
            new ScreenshotData(ScreenshotType.Nexus7, nexus7Res, KeyCode.Alpha5),
            new ScreenshotData(ScreenshotType.Nexus10, nexus10Res, KeyCode.Alpha6)
        };
        
        static string ScreenPath => Path.Combine(Directory.GetParent(Application.dataPath).FullName, SCREENS_DIR);

        static IOrderedEnumerable<Camera> AllCameras
        {
            get { return Camera.allCameras.OrderBy(x => x.depth); }
        }

        static ScreenshotMaker()
        {
            EditorApplication.playModeStateChanged -= OnPlaymodeStateChanged;
            EditorApplication.playModeStateChanged += OnPlaymodeStateChanged;
        }


        [MenuItem("HUF/Windows/Screenshot Maker")]
        static void Init()
        {
            ScreenshotMaker window = (ScreenshotMaker) GetWindow(typeof(ScreenshotMaker), false, WINDOW_TITLE);
            window.Show();
        }

        void OnGUI()
        {
            DrawTutorial();
            DrawDirectoryButton();
        }

        static void DrawTutorial()
        {
            GUILayout.Label("Press \"K\" to make All screenshots");
            foreach (var data in screensList)
            {
                GUILayout.Label($"Press \"{data.KeyCode.ToString()}\" to make {data.Type.ToString()} screenshot");
            }
        }

        static void DrawDirectoryButton()
        {
            GUILayout.Space(15);
            UnityEngine.GUI.backgroundColor = Color.green;

            if (GUILayout.Button(OPEN_BUTTON_LABEL, GUILayout.Height(40f)))
            {
                EditorUtility.RevealInFinder(ScreenPath);
            }
        }

        static void OnPlaymodeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredPlayMode)
            {
                CoroutineManager.StartCoroutine(ForceUpdateRoutine());
            }
        }

        static IEnumerator ForceUpdateRoutine()
        {
            while (true)
            {
                yield return null;
                ForceUpdate();
            }
        }

        static void ForceUpdate()
        {
            if (!EditorApplication.isPlaying)
                return;

            if (Input.GetKeyDown(KeyCode.K))
                CreateAllScreenshots();

            foreach (var data in screensList)
            {
                if (Input.GetKeyDown(data.KeyCode))
                    CreateScreenshot(data);
            }
        }

        static void CreateAllScreenshots()
        {
            foreach (var screenData in screensList)
            {
                CreateScreenshot(screenData);
            }
        }

        static void CreateScreenshot(ScreenshotData screenData)
        {
            var screenshotDirectory = GetDirectoryName(screenData.Type);
            var directory = Directory.CreateDirectory(screenshotDirectory);
            var index = GetNextScreenIndex(directory);
            var screenshotName = GetName(screenshotDirectory, index);
            var screenshotData = GetScreenshot(screenData.Resolution);

            SaveScreenshot(screenshotName, screenshotData);
        }

        static string GetDirectoryName(ScreenshotType screenshotType)
        {
            return Path.Combine(ScreenPath, screenshotType.ToString());
        }

        static int GetNextScreenIndex(DirectoryInfo directory)
        {
            return directory.GetFiles().Length + 1;
        }

        static string GetName(string screenDirectory, int index)
        {
            var fileName = string.Format("Screen_{0:00}.png", index);
            return Path.Combine(screenDirectory, fileName);
        }

        static Texture2D GetScreenshot(Vector2Int resolution)
        {
            var renderTexture = new RenderTexture(resolution.x, resolution.y, 24);
            var screenShot = new Texture2D(resolution.x, resolution.y, TextureFormat.RGB24, false);

            RenderTexture.active = renderTexture;
            foreach (var camera in AllCameras)
            {
                camera.targetTexture = renderTexture;
                camera.Render();
            }

            screenShot.ReadPixels(new Rect(0, 0, resolution.x, resolution.y), 0, 0);

            Clear(renderTexture);
            return screenShot;
        }

        static void Clear(RenderTexture renderTexture)
        {
            foreach (var camera in AllCameras)
            {
                camera.targetTexture = null;
            }

            RenderTexture.active = null;
            Destroy(renderTexture);
        }

        static void SaveScreenshot(string filename, Texture2D screenShot)
        {
            var bytes = screenShot.EncodeToPNG();
            File.WriteAllBytes(filename, bytes);
            Debug.LogFormat("Screenshot created: {0}", filename);
        }
    }
}
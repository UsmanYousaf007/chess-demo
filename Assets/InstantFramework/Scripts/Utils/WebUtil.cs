using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace TurboLabz.TLUtils
{
    public static class WebUtil
    {



        public static UnityWebRequest Post(string url, Dictionary<string, string> post)
        {
            WWWForm form = new WWWForm();
            foreach (KeyValuePair<string, string> post_arg in post)
            {
                form.AddField(post_arg.Key, post_arg.Value);
            }
            using (UnityWebRequest www = UnityWebRequest.Post(url, form))
            {
                www.SendWebRequest();
                return www;
            }
        }

        public static UnityWebRequest Post(string url, byte[] stream, string filename, string mimeType)
        {
            WWWForm form = new WWWForm();
            form.AddBinaryData("file", stream, filename, mimeType);
            using (UnityWebRequest www = UnityWebRequest.Post(url, form))
            {
                www.SendWebRequest();
                return www;
            }

        }

        public static Texture2D GetImage(string url, int width, int height)
        {
            var www = new WWW(url);
            var downloadedImage = new Texture2D(width, height);
            www.LoadImageIntoTexture(downloadedImage);
            return downloadedImage;
        }

        public static UnityWebRequest Get(string url)
        {
            using (UnityWebRequest www = UnityWebRequest.Get(url))
            {
                www.SendWebRequest();
                return www;
            }
        }

        public static byte[] GetBinary(string url)
        {
            using (UnityWebRequest www = UnityWebRequest.Get(url))
            {
                www.SendWebRequest();
                byte[] stream = www.downloadHandler.data;
                return stream;
            }

        }
    }

}

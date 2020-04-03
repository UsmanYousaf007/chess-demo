using HUF.Utils.Extensions;
using UnityEngine;
using UnityEngine.Events;

namespace HUF.Ads.Implementation.EditorAds
{
    public class EditorAdsGUI : MonoBehaviour
    {
        const int MARGIN = 10;

        public event UnityAction<AdResult> AdResult;

        static int ButtonWidth => Screen.width / 2;
        static int ButtonHeight => Screen.height / 12;

        void OnGUI()
        {
            GUI.Box(new Rect(10, 10, Screen.width - 20, Screen.height - 20), "Editor Ads");

            var buttonPost = new Vector2(Screen.width / 2 - ButtonWidth / 2 + MARGIN,
                Screen.height / 2 - ButtonHeight - MARGIN);
            DrawResultButton(Implementation.AdResult.Failed, ref buttonPost);
            DrawResultButton(Implementation.AdResult.Skipped, ref buttonPost);
            DrawResultButton(Implementation.AdResult.Completed, ref buttonPost);
        }

        static Rect GetButtonRect(Vector2 position)
        {
            return new Rect(position, new Vector2(ButtonWidth, ButtonHeight));
        }

        void DrawResultButton(AdResult result, ref Vector2 position)
        {
            if (GUI.Button(GetButtonRect(position), $"Result: {result}"))
                AdResult.Dispatch(result);

            position.y += MARGIN * 2 + ButtonHeight;
        }
    }
}
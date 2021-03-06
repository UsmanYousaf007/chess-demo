using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using strange.extensions.command.impl;
using System.IO;
using TurboLabz.TLUtils;
namespace TurboLabz.InstantFramework
{
    public class ShowShareDialogCommand : Command
    {
        //Navigation
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public UpdateShareDialogSignal updateShareDialogSignal { get; set; }

        public override void Execute()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_SHARE_SCREEN_DLG);
            string path = ScreenShotPath.GetScreenCapturePath();
            updateShareDialogSignal.Dispatch(LoadSprite(path));

        }

        private Sprite LoadSprite(string path)
        {
            if (string.IsNullOrEmpty(path)) return null;
            if (System.IO.File.Exists(path))
            {
                byte[] bytes = System.IO.File.ReadAllBytes(path);

                Texture2D texture = new Texture2D(1, 1);
                texture.LoadImage(bytes);
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                return sprite;
            }
            return null;
        }
    }
}
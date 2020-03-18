using HUF.Storage.API;
using HUF.Storage.API.Structs;
using HUF.Utils.Configs.API;
using HUFEXT.CrossPromo.Implementation.Model;
using UnityEngine;

namespace HUFEXT.CrossPromo.Implementation.View.CrossPromoTile
{
    public class TileViewModel
    {
        readonly string logPrefix;
        readonly Transform parent;

        TileModel model;
        TileContainer container;

        public TileViewModel(TileModel model, Transform parent)
        {
            logPrefix = $"[{GetType().Name}]";
            this.model = model;
            this.parent = parent;
            LoadSprite(model.IsRemote);
        }

        public void DestroyView()
        {
            if (container != null)
            {
                Object.Destroy(container.gameObject);
            }
        }

        public void UpdateView(TileModel model)
        {
            this.model = model;
            LoadSprite(model.IsRemote);
        }


        public void UpdateTexts()
        {
            var isInstalled = AppLauncher.AppLauncher.IsAppInstalled(model.PackageName);
            UpdateTexts(isInstalled);
        }

        void UpdateTexts(bool isInstalled)
        {
            if ( container == null )
                return;

            container.SetButtonText(
                isInstalled
                    ? CrossPromoService.InstalledStateButtonText
                    : CrossPromoService.NotInstalledStateButtonText);
            container.SetLabelText(CrossPromoService.InstalledStateTileLabelText);
        }

        void HandleRemoteSpriteCreation(string filePath)
        {
            HStorage.Texture.Get(filePath, HandleTextureFetched);
        }

        void HandleTextureFetched(ObjectResultContainer<Texture2D> resultContainer)
        {
            if (resultContainer.IsSuccess)
            {
                var tex = resultContainer.Result;
                var rect = new Rect(0.0f, 0.0f, tex.width, tex.height);
                var pivot = new Vector2(0.5f, 0.5f);
                var sprite = Sprite.Create(tex, rect, pivot);
                Debug.Assert(sprite != null, $"{logPrefix} Failed to create sprite from remote texture");
                InternalViewUpdate(sprite);
            }
        }

        void HandleLocalSpriteCreation(string filePath)
        {
            var sprite = Resources.Load<Sprite>(filePath);
            Debug.Assert(sprite != null, $"{logPrefix} Failed to load sprite at path {filePath}");
            InternalViewUpdate(sprite);
        }

        void LoadSprite(bool isRemote)
        {
            if (isRemote)
            {
                HandleRemoteSpriteCreation(model.SpritePath);
            }
            else
            {
                HandleLocalSpriteCreation(model.SpritePath);
            }
        }

        void InternalViewUpdate(Sprite sprite)
        {
            if (container == null)
            {
                CreateView();
                UpdateTexts();
            }

            var isInstalled = AppLauncher.AppLauncher.IsAppInstalled(model.PackageName);
            var config = HConfigs.GetConfig<CrossPromoRemoteConfig>();

            container.SetTitle(model.Title);
            container.SetSprite(sprite);
            container.SetButtonColor(isInstalled ? config.NotInstalledStateButtonColor : config.InstalledStateButtonColor);

            if (isInstalled)
            {
                container.SetButtonAction(OpenGame);
                container.SetImageAction(OpenGame);
            }
            else
            {
                container.SetButtonAction(OpenStoreLink);
                container.SetImageAction(OpenStoreLink);
            }

            container.SetLabelActive(isInstalled);
            container.SetLabelColor(config.InstalledStateButtonColor);
            container.SetInteractive(model.IsInteractive);
            container.SetButtonActive(model.IsButtonActive);

            UpdateTexts();
        }

        void CreateView()
        {
            var config = HConfigs.GetConfig<CrossPromoLocalConfig>();
            container = Object.Instantiate(config.TileContainerPrefab, parent);
        }

        void OpenGame()
        {
            AppLauncher.AppLauncher.LaunchApp(model.PackageName);
        }

        void OpenStoreLink()
        {
            Application.OpenURL(model.StoreLink);
        }
    }
}
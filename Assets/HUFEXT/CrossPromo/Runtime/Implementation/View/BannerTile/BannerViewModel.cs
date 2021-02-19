using HUF.Storage.Runtime.API;
using HUF.Storage.Runtime.Implementation.Structs;
using HUF.Utils.Runtime.Configs.API;
using HUF.Utils.Runtime.Extensions;
using HUFEXT.CrossPromo.Runtime.Implementation.Model;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace HUFEXT.CrossPromo.Runtime.Implementation.View.BannerTile
{
    public class BannerViewModel
    {
        readonly string logPrefix;
        readonly RectTransform parent;
        readonly RectTransform sizeArchetype;

        public event UnityAction<RotationDirection> OnBannerRotate;

        TileModel model;
        BannerTileContainer container;

        public BannerViewModel( TileModel model, RectTransform parent, RectTransform sizeArchetype )
        {
            logPrefix = $"[{GetType().Name}]";
            this.model = model;
            this.parent = parent;
            this.sizeArchetype = sizeArchetype;
            LoadSprite( model.IsRemote );
        }

        public void DestroyView()
        {
            if ( container != null )
            {
                container.OnBannerRotation -= HandleBannerRotation;
                Object.Destroy( container.gameObject );
            }
        }

        public void UpdateView( TileModel model )
        {
            this.model = model;

            if ( SpriteBank.Sprites.ContainsKey( model.SpritePath ) )
            {
                InternalViewUpdate( SpriteBank.Sprites[model.SpritePath] );
            }
            else
            {
                LoadSprite( model.IsRemote );
            }
        }

        void HandleTextureFetched( ObjectResultContainer<Texture2D> resultContainer )
        {
            if ( resultContainer.IsSuccess )
            {
                var name = resultContainer.StorageResultContainer.PathToFile;

                if ( !SpriteBank.Sprites.ContainsKey( name ) )
                {
                    var tex = resultContainer.Result;
                    var rect = new Rect( 0.0f, 0.0f, tex.width, tex.height );
                    var pivot = new Vector2( 0.5f, 0.5f );
                    var sprite = Sprite.Create( tex, rect, pivot );

                    if ( sprite == null )
                    {
                        Debug.LogError( $"{logPrefix} Failed to create sprite from remote texture {name}" );
                        return;
                    }

                    sprite.name = name;
                    SpriteBank.Sprites.Add( name, sprite );
                }

                InternalViewUpdate( SpriteBank.Sprites[name] );
            }
            else
            {
                Debug.LogWarning(
                    $"{logPrefix} Failed to download texture from remote texture {resultContainer.StorageResultContainer.PathToFile}" );
            }
        }

        void HandleRemoteSpriteCreation( string filePath )
        {
            HStorage.Texture.Get( filePath, HandleTextureFetched );
        }

        void HandleLocalSpriteCreation( string filePath )
        {
            var sprite = Resources.Load<Sprite>( filePath );

            if ( sprite == null )
            {
                Debug.LogError( $"{logPrefix} Failed to load sprite at path {filePath}" );
                HandleRemoteSpriteCreation( filePath );
                return;
            }

            if ( !SpriteBank.Sprites.ContainsKey( filePath ) )
            {
                sprite.name = filePath;
                SpriteBank.Sprites.Add( filePath, sprite );
            }

            InternalViewUpdate( SpriteBank.Sprites[filePath] );
        }

        void LoadSprite( bool isRemote )
        {
            if ( isRemote )
            {
                HandleRemoteSpriteCreation( model.SpritePath );
            }
            else
            {
                HandleLocalSpriteCreation( model.SpritePath );
            }
        }

        void InternalViewUpdate( Sprite sprite )
        {
            if ( container == null )
            {
                CreateView();
                UpdateTexts();
            }

            var isInstalled = AppLauncher.AppLauncher.IsAppInstalled( model.PackageName );
            var config = HConfigs.GetConfig<CrossPromoRemoteConfig>();
            container.SetSprite( sprite );

            container.SetButtonColor( isInstalled
                ? config.NotInstalledStateButtonColor
                : config.InstalledStateButtonColor );

            if ( isInstalled )
            {
                container.SetButtonAction( OpenGame );
                container.SetImageClickAction( OpenGame );
            }
            else
            {
                container.SetButtonAction( OpenStoreLink );
                container.SetImageClickAction( OpenStoreLink );
            }

            UpdateTexts( isInstalled );
            container.SetInteractive( model.IsInteractive );
            container.SetButtonActive( model.IsButtonActive );
        }

        void CreateView()
        {
            var config = HConfigs.GetConfig<CrossPromoLocalConfig>();
            container = Object.Instantiate( config.BannerTileContainerPrefab, parent );
            container.transform.name = "BannerContainer" + model.Uuid + model.SpritePath;
            container.OnBannerRotation += HandleBannerRotation;
            container.SetSizeArchetype( sizeArchetype );
        }

        void HandleBannerRotation( RotationDirection rotationDirection )
        {
            OnBannerRotate?.Invoke( rotationDirection );
        }

        void OpenStoreLink()
        {
            Application.OpenURL( model.StoreLink );
        }

        void OpenGame()
        {
            AppLauncher.AppLauncher.LaunchApp( model.PackageName );
        }

        public void UpdateTexts()
        {
            var isInstalled = AppLauncher.AppLauncher.IsAppInstalled( model.PackageName );

            if ( isInstalled == false && model.URLScheme.IsNullOrEmpty() == false )
            {
                isInstalled = AppLauncher.AppLauncher.IsAppInstalled( model.URLScheme );
            }

            UpdateTexts( isInstalled );
        }

        void UpdateTexts( bool isInstalled )
        {
            if ( container == null )
                return;

            container.SetButtonText( isInstalled
                ? CrossPromoService.InstalledStateButtonText
                : CrossPromoService.NotInstalledStateButtonText );
        }
    }
}
using HUF.Storage.Runtime.API;
using HUF.Storage.Runtime.Implementation.Structs;
using HUF.Utils.Runtime.Configs.API;
using HUF.Utils.Runtime.Extensions;
using HUF.Utils.Runtime.Logging;
using HUFEXT.CrossPromo.Runtime.API;
using HUFEXT.CrossPromo.Runtime.Implementation.Model;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace HUFEXT.CrossPromo.Runtime.Implementation.View.BannerTile
{
    public class BannerViewModel
    {
        static readonly HLogPrefix logPrefix = new HLogPrefix( HCrossPromo.logPrefix, nameof(BannerViewModel) );
        readonly Vector2 spriteCreationPivot = new Vector2( 0.5f, 0.5f );
        readonly RectTransform parent;
        readonly RectTransform sizeArchetype;

        TileModel model;
        BannerTileContainer container;

        public event UnityAction<RotationDirection> OnBannerRotate;

        public BannerViewModel( TileModel model, RectTransform parent, RectTransform sizeArchetype )
        {
            this.parent = parent;
            this.sizeArchetype = sizeArchetype;
            UpdateView( model );
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
            if ( this.model != model )
            {
                this.model = model;
                LoadSprite();
            }
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

        void LoadSprite()
        {
            if ( SpriteBank.Sprites.ContainsKey( model.SpritePath ) )
            {
                InternalViewUpdate( SpriteBank.Sprites[model.SpritePath] );
            }
            else
            {
                if ( model.IsRemote )
                {
                    HandleRemoteSpriteCreation( model.SpritePath );
                }
                else
                {
                    HandleLocalSpriteCreation( model.SpritePath );
                }
            }
        }

        void HandleRemoteSpriteCreation( string filePath )
        {
            HStorage.Texture.Get( filePath, HandleTextureFetched );
        }

        void HandleTextureFetched( ObjectResultContainer<Texture2D> resultContainer )
        {
            if ( resultContainer.IsSuccess )
            {
                var name = resultContainer.StorageResultContainer.PathToFile;

                if ( SpriteBank.Sprites.ContainsKey( name ) )
                    HLog.LogError( logPrefix, $"SpriteBank already contains sprite: {name}" );
                else
                {
                    var tex = resultContainer.Result;
                    var rect = new Rect( 0, 0, tex.width, tex.height );
                    var sprite = Sprite.Create( tex, rect, spriteCreationPivot );

                    if ( sprite == null )
                    {
                        HLog.LogError( logPrefix, $"Failed to create sprite from remote texture: {name}" );
                        return;
                    }

                    sprite.name = name;
                    SpriteBank.Sprites.Add( name, sprite );
                }

                InternalViewUpdate( SpriteBank.Sprites[name] );
            }
            else
            {
                HLog.LogWarning( logPrefix,
                    $"Failed to download texture from remote texture: {resultContainer.StorageResultContainer.PathToFile}, message: {resultContainer.StorageResultContainer.ErrorMessage}" );
            }
        }

        void HandleLocalSpriteCreation( string filePath )
        {
            var sprite = Resources.Load<Sprite>( filePath );

            if ( sprite == null )
            {
                HLog.LogError( logPrefix, $"Failed to load sprite at path: {filePath}" );
                HandleRemoteSpriteCreation( filePath );
                return;
            }

            sprite.name = filePath;
            SpriteBank.Sprites.Add( filePath, sprite );
            InternalViewUpdate( sprite );
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
            container.transform.name = $"BannerContainer {model.Uuid}{model.SpritePath}";
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
using HUF.Storage.Runtime.API;
using HUF.Storage.Runtime.Implementation.Structs;
using HUF.Utils.Runtime.Configs.API;
using HUF.Utils.Runtime.Logging;
using HUFEXT.CrossPromo.Runtime.API;
using HUFEXT.CrossPromo.Runtime.Implementation.Model;
using UnityEngine;

namespace HUFEXT.CrossPromo.Runtime.Implementation.View.CrossPromoTile
{
    public class TileViewModel
    {
        readonly HLogPrefix logPrefix = new HLogPrefix( HCrossPromo.logPrefix, nameof(TileViewModel) );
        readonly Vector2 spriteCreationPivot = new Vector2( 0.5f, 0.5f );
        readonly Transform parent;

        TileModel model;
        TileContainer container;

        public TileViewModel( TileModel model, Transform parent )
        {
            this.parent = parent;
            UpdateView( model );
        }

        public void DestroyView()
        {
            if ( container != null )
            {
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

            if ( container == null )
                return;

            container.SetButtonText(
                isInstalled
                    ? CrossPromoService.InstalledStateButtonText
                    : CrossPromoService.NotInstalledStateButtonText );
            container.SetLabelText( CrossPromoService.InstalledStateTileLabelText );
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
                    InternalViewUpdate( sprite );
                }
            }
        }

        void HandleLocalSpriteCreation( string filePath )
        {
            var sprite = Resources.Load<Sprite>( filePath );

            if ( sprite == null )
            {
                HLog.LogError( logPrefix, $"Failed to load sprite at path {filePath}" );
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
            container.SetTitle( model.Title );
            container.SetSprite( sprite );

            container.SetButtonColor( isInstalled
                ? config.NotInstalledStateButtonColor
                : config.InstalledStateButtonColor );

            if ( isInstalled )
            {
                container.SetButtonAction( OpenGame );
                container.SetImageAction( OpenGame );
            }
            else
            {
                container.SetButtonAction( OpenStoreLink );
                container.SetImageAction( OpenStoreLink );
            }

            container.SetLabelActive( isInstalled );
            container.SetLabelColor( config.InstalledStateButtonColor );
            container.SetInteractive( model.IsInteractive );
            container.SetButtonActive( model.IsButtonActive );
            UpdateTexts();
        }

        void CreateView()
        {
            var config = HConfigs.GetConfig<CrossPromoLocalConfig>();
            container = Object.Instantiate( config.TileContainerPrefab, parent );
        }

        void OpenGame()
        {
            AppLauncher.AppLauncher.LaunchApp( model.PackageName );
        }

        void OpenStoreLink()
        {
            Application.OpenURL( model.StoreLink );
        }
    }
}
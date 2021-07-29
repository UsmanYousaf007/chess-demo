using System;
using System.Collections;
using System.IO;
using System.Threading.Tasks;
using Firebase.Extensions;
using Firebase.Storage;
using HUF.InitFirebase.Runtime;
using HUF.Storage.Runtime.Implementation.ActionHandlers;
using HUF.Storage.Runtime.Implementation.Structs;
using HUF.Utils.Runtime;
using UnityEngine;
using UnityEngine.Networking;

namespace HUF.StorageFirebase.Runtime.Implementation.ActionHandlers
{
    public class AssetBundleDownloadHandler : AssetBundleLocalHandler
    {
        public AssetBundleDownloadHandler( string fileId, Action<ObjectResultContainer<AssetBundle>> completeHandler )
            : base( fileId, completeHandler ) { }

        public void DownloadFile( StorageReference storageReference )
        {
            TryGetAssetBundle( Path.GetFileName( FileId ) )?.Unload( false );

            if ( storageReference == null )
                SendHandlerFail( "Storage Reference is not set" );
            storageReference.GetDownloadUrlAsync().ContinueWithOnMainThread( HandleUriDownloadComplete );
        }

        void HandleUriDownloadComplete( Task<Uri> task )
        {
            if ( task.IsFaulted || task.IsCanceled )
            {
                SendHandlerFail( task.Exception.GetFullErrorMessage() );
                return;
            }

            CoroutineManager.StartCoroutine( DownloadFile( task.Result ) );
        }

        IEnumerator DownloadFile( Uri uri )
        {
            var request = UnityWebRequest.Get( uri );
            yield return request.SendWebRequest();

            if ( request.isHttpError || request.isNetworkError )
            {
                SendHandlerFail( request.error );
                yield break;
            }

            var assetBundle = AssetBundle.LoadFromMemory( request.downloadHandler.data );
            SendHandlerSuccess( assetBundle, request.downloadHandler.data );
        }
    }
}
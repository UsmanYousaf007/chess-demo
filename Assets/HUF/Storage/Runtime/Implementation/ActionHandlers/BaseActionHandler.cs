using System;
using HUF.Storage.Runtime.API;
using HUF.Storage.Runtime.Implementation.Structs;
using HUF.Utils.Runtime.Extensions;

namespace HUF.Storage.Runtime.Implementation.ActionHandlers
{
    public abstract class BaseActionHandler<T> where T : class
    {
        protected Action<ObjectResultContainer<T>> completeHandler;
        protected string FileId { get; }
        protected string FilePath;

        public abstract void DownloadFile();

        protected BaseActionHandler(string fileId, Action<ObjectResultContainer<T>> completeHandler)
        {
            FileId = fileId;
            this.completeHandler = completeHandler;
        }

        protected virtual void SendHandlerFail(string error)
        {
            SendResult(new StorageResultContainer(FileId, error));
        }

        protected virtual void SendHandlerSuccess(T result)
        {
            SendResult(new StorageResultContainer(FileId), result);
        }
        
        protected void SendHandlerSuccess(T result, byte[] byteDataToSave)
        {
            var file = FileId;

            if ( !FilePath.IsNullOrEmpty() )
            {
                file = FilePath;
            }

            if (byteDataToSave != null && !StorageUtils.TrySaveObject(byteDataToSave, file))
            {
                SendHandlerFail($"{StorageErrorMessages.ERROR_DURING_SAVING}. ({FileId})");
                return;
            }
            SendResult(new StorageResultContainer(FileId), result);
            completeHandler = null;
        }

        void SendResult(StorageResultContainer resultContainer, T result = null)
        {
            completeHandler.Dispatch( new ObjectResultContainer<T>(resultContainer, result));
            completeHandler = null;
        }
    }
}
using HUF.Storage.Runtime.API;
using HUF.Storage.Runtime.API.Structs;
using HUF.Utils.Runtime.Extensions;
using UnityEngine.Events;

namespace HUF.Storage.Runtime.Implementation.ActionHandlers
{
    public abstract class BaseActionHandler<T> where T : class
    {
        protected UnityAction<ObjectResultContainer<T>> completeHandler;
        protected string FilePath { get; }

        public abstract void ReadLocalFile();

        protected BaseActionHandler(string filePath, UnityAction<ObjectResultContainer<T>> completeHandler)
        {
            FilePath = filePath;
            this.completeHandler = completeHandler;
        }

        protected void SendHandlerFail(string error)
        {
            completeHandler.Dispatch(
                new ObjectResultContainer<T>(new StorageResultContainer(FilePath, error)));
            completeHandler = null;
        }

        protected void SendHandlerSuccess(T result)
        {
            completeHandler.Dispatch(
                new ObjectResultContainer<T>(new StorageResultContainer(FilePath), result));
            completeHandler = null;
        }
        
        protected void SendHandlerSuccess(T result, byte[] byteDataToSave)
        {
            if (byteDataToSave != null && !StorageUtils.TrySaveObject(byteDataToSave, FilePath))
            {
                SendHandlerFail($"{StorageErrorMessages.ERROR_DURING_SAVING}. ({FilePath})");
                return;
            }
            completeHandler.Dispatch(
                new ObjectResultContainer<T>(new StorageResultContainer(FilePath), result));
            completeHandler = null;
        }
    }
}
using System.Text;
using System.Threading.Tasks;
using Firebase.Extensions;
using Firebase.Storage;
using HUF.Storage.Runtime.API.Structs;
using HUF.Utils.Runtime.Extensions;
using UnityEngine.Events;

namespace HUF.StorageFirebase.Runtime.Implementation.ActionHandlers
{
    public class StorageRemoveHandler
    {
        public string PathToFile { get; }
        readonly UnityAction<StorageResultContainer> onCompleteHandler;

        public StorageRemoveHandler(string filePath, UnityAction<StorageResultContainer> onCompleteHandler)
        {
            PathToFile = filePath;
            this.onCompleteHandler = onCompleteHandler;
        }

        public void StartRemove(StorageReference storageReference)
        {
            storageReference.DeleteAsync().ContinueWithOnMainThread(HandleRemoveComplete);
        }

        void HandleRemoveComplete(Task task)
        {
            if (task.IsFaulted || task.IsCompleted)
            {
                var exceptionMessage = GetException(task);
                onCompleteHandler.Dispatch(new StorageResultContainer(PathToFile, exceptionMessage));
                return;
            }

            HandleRemoveComplete();
        }

        void HandleRemoveComplete()
        {
            onCompleteHandler.Dispatch(new StorageResultContainer(PathToFile));
        }

        static string GetException(Task task)
        {
            if (task.Exception == null)
                return string.Empty;
            var exceptionMessage = new StringBuilder(task.Exception?.Message);
            if (task.Exception.InnerExceptions.Count > 0)
            {
                foreach (var innerException in task.Exception.InnerExceptions)
                    exceptionMessage.Append(" / ").Append(innerException.Message);
            }

            return exceptionMessage.ToString();
        }
    }
}
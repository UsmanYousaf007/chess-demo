using System;

namespace HUF.Storage.API.Models
{
    public class StorageModel : IDisposable
    {
        IDownloadService downloadService;
        IUploadService uploadService;
        IRemoveService removeService;

        internal IDownloadService DownloadService => downloadService;
        internal IUploadService UploadService => uploadService;
        internal IRemoveService RemoveService => removeService;
        
        public bool TryRegisterService(IDownloadService serviceToRegister)
        {
            if (downloadService == null)
            {
                downloadService = serviceToRegister;
                return true;
            }
            
            return false;
        }

        public bool TryRegisterService(IUploadService serviceToRegister)
        {
            if (uploadService == null)
            {
                uploadService = serviceToRegister;
                return true;
            }
            
            return false;
        }

        public bool TryRegisterService(IRemoveService serviceToRegister)
        {
            if (removeService == null)
            {
                removeService = serviceToRegister;
                return true;
            }
            
            return false;
        }

        public void Dispose()
        {
            downloadService.Dispose();
            uploadService.Dispose();
            removeService.Dispose();
        }
    }
}
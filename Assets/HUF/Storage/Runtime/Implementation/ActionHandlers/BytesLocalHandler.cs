using System;
using System.IO;
using HUF.Storage.Runtime.API;
using HUF.Storage.Runtime.Implementation.Structs;

namespace HUF.Storage.Runtime.Implementation.ActionHandlers
{
    public class BytesLocalHandler : BaseActionHandler<byte[]>
    {
        public BytesLocalHandler(
            string fileId,
            Action<ObjectResultContainer<byte[]>> completeHandler)
            : base(fileId, completeHandler)
        {
        }

        public BytesLocalHandler(
            string fileId,
            Action<ObjectResultContainer<byte[]>> completeHandler,
            string filePath)
            : base(fileId, completeHandler)
        {
            FilePath = filePath;
        }

        public override void DownloadFile()
        {
            LoadBytesFile(StorageUtils.GetLocalFilePath(FilePath));
        }

        void LoadBytesFile(string localFilePath)
        {
            try
            {
                var bytes = File.ReadAllBytes(localFilePath);
                SendHandlerSuccess(bytes);
            }
            catch (Exception ex)
            {
                SendHandlerFail(ex.Message);
            }
        }
    }
}
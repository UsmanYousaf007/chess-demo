using System;
using System.IO;
using HUF.Storage.API;
using HUF.Storage.API.Structs;
using UnityEngine.Events;

namespace HUF.Storage.Implementation.ActionHandlers
{
    public class BytesLocalHandler : BaseActionHandler<byte[]>
    {
        public BytesLocalHandler(
            string filePath,
            UnityAction<ObjectResultContainer<byte[]>> completeHandler) 
            : base(filePath, completeHandler)
        {
        }

        public override void ReadLocalFile()
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
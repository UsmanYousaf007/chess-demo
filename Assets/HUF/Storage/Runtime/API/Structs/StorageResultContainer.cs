using HUF.Utils.Runtime.Extensions;

namespace HUF.Storage.Runtime.API.Structs
{
    public class StorageResultContainer
    {
        public string PathToFile { get; }
        public string ErrorMessage { get; }

        public bool IsSuccess => !PathToFile.IsNullOrEmpty() && ErrorMessage.IsNullOrEmpty();

        public StorageResultContainer(string pathToFile, string errorMessage = null)
        {
            PathToFile = pathToFile;
            ErrorMessage = errorMessage;
        }
    }
}
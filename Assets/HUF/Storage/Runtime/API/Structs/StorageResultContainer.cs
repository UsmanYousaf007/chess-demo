using HUF.Utils.Extensions;

namespace HUF.Storage.API.Structs
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
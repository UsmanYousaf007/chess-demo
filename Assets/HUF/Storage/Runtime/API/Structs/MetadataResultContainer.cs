namespace HUF.Storage.Runtime.Implementation.Structs
{
    public class MetadataResultContainer
    {
        public StorageResultContainer StorageResultContainer { get; }
        public bool IsUpdateAvailable { get; }

        public MetadataResultContainer(StorageResultContainer resultContainer, bool isUpdateAvailable = false)
        {
            StorageResultContainer = resultContainer;
            IsUpdateAvailable = isUpdateAvailable;
        }
    }
}
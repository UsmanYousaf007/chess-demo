namespace HUF.Storage.Runtime.API.Structs
{
    public class ObjectResultContainer<T> where T : class
    {
        public StorageResultContainer StorageResultContainer;
        public T Result { get; }
        public bool IsSuccess => StorageResultContainer.IsSuccess && Result != null;

        public ObjectResultContainer(StorageResultContainer resultContainer, T result = null)
        {
            StorageResultContainer = resultContainer;
            Result = result;
        }
    }
}
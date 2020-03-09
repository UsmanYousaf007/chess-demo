namespace HUF.Storage.API
{
    public static class StorageErrorMessages
    {
        public const string STORAGE_NOT_INITIALIZED = "Storage services not initialized. Please initialize before use.";
        public const string ERROR_DURING_SAVING = "Error occured during saving bytes to local storage";
        public const string DOWNLOAD_ALREADY_IN_PROGRESS = "Download of given file is already in progress. " + 
                                                           "Before requesting another download wait for previous one to finish.";
    }
}
namespace HUF.StorageFirebase.Runtime.API
{
    public static class FirebaseErrorMessages
    {
        public const string FIREBASE_NOT_SIGNED_IN = "Firebase auth is not signed in. Cannot perform operation.";
        public const string UPLOAD_FAILED_OR_CANCELED = "Upload to Firebase failed or was canceled. Details: ";
        public const string CONFIG_MISSING_ERROR = "Config for Firebase Storage is missing. Fix before continue.";
        public const string FIREBASE_NOT_INITIALIZED = "Firebase is not initialized.";
        public const string FIREBASE_STORAGE_MISSING = "Firebase storage reference missing.";
        public const string STORAGE_ALREADY_INITIALIZED = "Storage already initialized";
    }
}
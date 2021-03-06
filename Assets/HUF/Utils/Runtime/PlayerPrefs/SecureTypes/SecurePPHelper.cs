using HUF.Utils.Runtime._3rdParty.Blowfish;

namespace HUF.Utils.Runtime.PlayerPrefs.SecureTypes
{
    public static class SecurePPHelper
    {
        public static string EncryptString(string stringToEncrypt, BlowFish encryption)
        {
            if (encryption == null)
            {
                return stringToEncrypt;
            }
            return encryption.Encrypt_ECB(stringToEncrypt);
        }
        
        public static string DecryptString(string stringToDecrypt, BlowFish encryption)
        {
            if (encryption == null)
            {
                return stringToDecrypt;
            }
            return encryption.Decrypt_ECB(stringToDecrypt);
        }
        
    }
}
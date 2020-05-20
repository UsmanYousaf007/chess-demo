using UnityEngine;

namespace HUF.Utils.Runtime.BuildSupport
{
    [System.Serializable]
    public class PurposeStringData
    {
        [SerializeField] string purposeName;
        [SerializeField] string purposeReason;

        public PurposeStringData(string purposeName, string purposeReason)
        {
            this.purposeName = purposeName;
            this.purposeReason = purposeReason;
        }

        public string PurposeName => purposeName;

        public string PurposeReason => purposeReason;
    }
}
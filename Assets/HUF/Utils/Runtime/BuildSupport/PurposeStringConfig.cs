using System.Collections.Generic;
using UnityEngine;

namespace HUF.Utils.Runtime.BuildSupport
{
    [CreateAssetMenu(menuName = "HUF/BuildSupport/iOS/PurposeStringConfig",
        fileName = "PurposeStringConfig.asset")]
    public class PurposeStringConfig : ScriptableObject
    {
        [SerializeField] List<PurposeStringData> purposeStringData = new List<PurposeStringData>();

        public IEnumerable<PurposeStringData> PurposeStringData => purposeStringData;
    }
}
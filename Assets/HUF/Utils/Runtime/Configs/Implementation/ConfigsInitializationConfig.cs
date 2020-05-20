using UnityEngine;

namespace HUF.Utils.Runtime.Configs.Implementation
{
    [CreateAssetMenu(fileName = NAME, menuName = "HUF/" + NAME)]
    public class ConfigsInitializationConfig : ScriptableObject
    {
        public const string NAME = "ConfigsInitializationConfig";
        
        [SerializeField] bool autoInit = true;

        public bool AutoInit => autoInit;
    }
}
using UnityEngine.Events;

namespace HUFEXT.PackageManager.Editor.API.Models
{
    public interface IAuthModel
    {
        string DeveloperID { get; }
        bool IsPolicyAccepted { get; }
        bool IsValid { get; }
        bool Initialize();
        void Validate( UnityAction<bool> response );
        void Invalidate();
    }
}

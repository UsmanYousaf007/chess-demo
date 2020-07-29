/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential


namespace TurboLabz.InstantFramework
{
    public interface IPhotoService
    {
        void PickPhoto(int width, int height, string format="jpeg");
        void TakePhoto(int widht, int height, string format="jpeg");
        bool HasCameraPermission();
        bool HasGalleryPermission();
        void OpenCameraSettings();
        void OpenGallerySettings();
    }
}

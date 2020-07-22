/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System;
using System.Collections.Generic;
using strange.extensions.promise.api;
using UnityEngine;

namespace TurboLabz.InstantFramework
{
    public interface IPhotoService
    {
        Photo PickPhoto(int size, string format);
        Photo TakePhoto(int size, string format);
    }
}

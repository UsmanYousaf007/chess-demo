/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System;
using UnityEngine;

namespace TurboLabz.InstantFramework
{
	public class PhotoPickerService : IPhotoService
	{

		public byte[] PickPhoto(int maxSize, string format)
		{
			byte[] imageStream = null;
			try
			{
				NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
				{
					if (path != null)
					{
						Texture2D photoTexture = NativeGallery.LoadImageAtPath(path, maxSize, true);
						if (photoTexture != null)
						{
							imageStream = photoTexture.EncodeToJPG();
							Texture2D.Destroy(photoTexture);
						}
					}
				});
				return imageStream;
			}
			catch (Exception e)
			{ throw new Exception("Exception thrown from TakePhoto", e.InnerException); }


		}

		public byte[] TakePhoto(int maxSize, string format = "jpeg")
		{
			try
			{
				byte[] imageStream = null;
				NativeCamera.Permission permission = NativeCamera.TakePicture((path) =>
				{
					if (path != null)
					{
						Texture2D photoTexture = NativeCamera.LoadImageAtPath(path, maxSize, true);
						if (photoTexture != null)
						{
							imageStream = photoTexture.EncodeToJPG();
							Texture2D.Destroy(photoTexture);
						}
					}
				});
				return imageStream;
			}
			catch (Exception e)
			{ throw new Exception("Exception thrown from TakePhoto", e.InnerException); }
		}
	}
}

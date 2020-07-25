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
		[Inject] public PhotoPickerCompleteSignal photoPickerCompletedSignal { get; set; }

		public void PickPhoto(int maxSize, string format= "jpeg")
		{
			Photo photo = null;
			try
			{
				NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
				{
					if (path != null)
					{
						Texture2D photoTexture = NativeGallery.LoadImageAtPath(path, maxSize, false);
						if (photoTexture != null)
                        {
                            photo = CreatePhotoView(photoTexture);
                            Texture2D.Destroy(photoTexture);

							photoPickerCompletedSignal.Dispatch(photo);
						}
					}
				});
			}
			catch (Exception e)
			{
                throw new Exception("Exception thrown from TakePhoto", e.InnerException);
            }
		}

        public void TakePhoto(int maxSize, string format = "jpeg")
		{
			try
			{
				Photo photo = null;
				NativeCamera.Permission permission = NativeCamera.TakePicture((path) =>
				{
					if (path != null)
					{
						Texture2D photoTexture = NativeGallery.LoadImageAtPath(path, maxSize, false);
						if (photoTexture != null)
						{
							photo = CreatePhotoView(photoTexture);
							Texture2D.Destroy(photoTexture);

							photoPickerCompletedSignal.Dispatch(photo);
						}
					}
				});
			}
			catch (Exception e)
			{
                throw new Exception("Exception thrown from TakePhoto", e.InnerException);
            }
		}

		private static Photo CreatePhotoView(Texture2D photoTexture)
		{
			Sprite image = Sprite.Create(photoTexture,
											 new Rect(0, 0, photoTexture.width, photoTexture.height),
											 new Vector2(0.5f, 0.5f));
			image.name = photoTexture.name;

			byte[] imageStream = photoTexture.EncodeToJPG();

			Photo photo = new Photo(image, imageStream);
			return photo;
		}
	}
}

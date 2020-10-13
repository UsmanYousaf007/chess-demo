/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System;
using System.Collections.Generic;
using UnityEngine;

namespace TurboLabz.InstantFramework
{
	public class PhotoPickerService : IPhotoService
	{
		[Inject] public PhotoPickerCompleteSignal photoPickerCompletedSignal { get; set; }

		#region Public Methods
		public void PickPhoto(int width, int height, string format= "jpeg")
		{
			PhotoVO photo = null;
			int maxSize = width > height ? width : height;
			try
			{
				NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
				{
					if (path != null)
					{
						Texture2D photoTexture = NativeGallery.LoadImageAtPath(path, maxSize, false);
						if (photoTexture != null)
                        {
							var dimensionSize = photoTexture.width < photoTexture.height ? photoTexture.width : photoTexture.height;
							ResizeTextureCanvas(photoTexture, dimensionSize, dimensionSize);
							photo = CreatePhotoView(photoTexture);        
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

        public void TakePhoto(int width, int height, string format = "jpeg")
		{
			try
			{
				int maxSize = width > height ? width : height;
				PhotoVO photo = null;
				NativeCamera.Permission permission = NativeCamera.TakePicture((path) =>
				{
					if (path != null)
					{
						Texture2D photoTexture = NativeGallery.LoadImageAtPath(path, maxSize, false);
						if (photoTexture != null)
						{
							var dimensionSize = photoTexture.width < photoTexture.height ? photoTexture.width : photoTexture.height;
							ResizeTextureCanvas(photoTexture, dimensionSize, dimensionSize);
							photo = CreatePhotoView(photoTexture);
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

		public bool HasCameraPermission()
		{
			var permission = NativeCamera.CheckPermission();
			Debug.Log("HasCameraPermission: " + permission.ToString());
			if (permission == NativeCamera.Permission.Granted || permission == NativeCamera.Permission.ShouldAsk)
			{
				return true;
            }

			return false;
        }

		public bool HasGalleryPermission()
		{
			var permission = NativeGallery.CheckPermission(NativeGallery.PermissionType.Read);
			Debug.Log("HasGalleryPermission: " + permission.ToString());
			if (permission == NativeGallery.Permission.Granted || permission == NativeGallery.Permission.ShouldAsk || permission == NativeGallery.Permission.LimitedAccess)
			{
				return true;
			}

			return false;
		}

		public void OpenCameraSettings()
		{
			NativeCamera.OpenSettings();
        }

		public void OpenGallerySettings()
		{
			NativeGallery.OpenSettings();
		}

        #endregion

        #region Private Methods
        private PhotoVO CreatePhotoView(Texture2D photoTexture)
		{
			Sprite image = Sprite.Create(photoTexture,
											 new Rect(0, 0, photoTexture.width, photoTexture.height),
											 new Vector2(0.5f, 0.5f));
			image.name = photoTexture.name;

			byte[] imageStream = photoTexture.EncodeToJPG();

			PhotoVO photo = new PhotoVO(image, imageStream);
			return photo;
		}

		private Color32[] ResizeTextureCanvas(Texture2D texture, int width, int height)
		{
			var newPixels = ResizeTextureCanvas(texture.GetPixels32(), texture.width, texture.height, width, height);
			texture.Resize(width, height);
			texture.SetPixels32(newPixels);
			texture.Apply();
			return newPixels;
		}

		private Color32[] ResizeTextureCanvas(IList<Color32> pixels, int oldWidth, int oldHeight, int width, int height)
		{
			var newPixels = new Color32[(width * height)];
			Color32 fillColor = new Color32(0, 0, 0, 255);

			for (int i = 0; i < (width * height); i++)
				newPixels[i] = fillColor;

			var wBorder = (width - oldWidth) / 2;
			var hBorder = (height - oldHeight) / 2;

			for (int r = 0; r < height; r++)
			{
				var oldR = r - hBorder;
				if (oldR < 0) { continue; }
				if (oldR >= oldHeight) { break; }

				for (int c = 0; c < width; c++)
				{
					var oldC = c - wBorder;
					if (oldC < 0) { continue; }
					if (oldC >= oldWidth) { break; }

					var oldI = oldR * oldWidth + oldC;
					var i = r * width + c;
					newPixels[i] = pixels[oldI];
				}
			}

			return newPixels;
		}

        #endregion
    }
}

using UnityEngine;
using System.Collections;

public static class CameraDevice
{
	public static GameObject cameraPlane;
	private static WebCamTexture cameraDevice;
	public static Texture2D Photo;

	public static void ShowCameraImage()
	{
		cameraPlane.GetComponent<Renderer>().material.mainTexture = null;

		if (cameraDevice != null)
			cameraDevice.Stop();

		cameraDevice = new WebCamTexture();
		cameraPlane.GetComponent<Renderer>().material.mainTexture = cameraDevice;

		if (HaveCamera())
			cameraDevice.Play();
	}

	public static void StopCameraImage()
	{
		cameraPlane.GetComponent<Renderer>().material.mainTexture = null;

		if (cameraDevice != null)
			cameraDevice.Stop();
	}

	public static void RecordPhoto()
	{
		new WaitForEndOfFrame(); 

		Photo = new Texture2D(cameraDevice.width, cameraDevice.height);
		Photo.SetPixels(cameraDevice.GetPixels());
		Photo.Apply();

		cameraPlane.GetComponent<Renderer>().material.mainTexture = Photo;

		UnityAndroidExtras.instance.makeToast("Foto capturada", 1);

		//ShowCameraImage();
	}

	public static bool HaveCamera() 
	{ 
		return (WebCamTexture.devices.Length > 0) ? true : false;
	}

}

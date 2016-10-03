using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;

public static class GPS
{
	private static double[] _location;
	public static double[] location { get { return _location; } }

	public static void StartGPS()
	{
		Input.location.Start();
	}

	public static void StopGPS()
	{
		Input.location.Stop();
	}

	public static bool ReceivePlayerLocation()
	{
		if (Application.platform != RuntimePlatform.Android) 
		{
			UnityAndroidExtras.instance.makeToast("Dispositivo sem serviço de localização", 1);
			return false;
		}

		if (!Input.location.isEnabledByUser) 
		{
			UnityAndroidExtras.instance.makeToast("Verifique o serviço de localização do celular", 1);
			return false;
		}

		if (Input.location.status == LocationServiceStatus.Failed)
		{
			UnityAndroidExtras.instance.makeToast("Problema no serviço de localização do celular", 1);
			return false;
		}
		else
		{
			_location = new double[2];

			_location[0] = System.Convert.ToDouble(Input.location.lastData.latitude);
			_location[1] = System.Convert.ToDouble(Input.location.lastData.longitude);
		}
		
		return true;
	}

}

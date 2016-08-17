using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;

public static class GPS 
{
	private static double[] _location;
	public static double[] location { get { return _location; } }

	public static bool ReceivePlayerLocation()
	{
		_location = null;

		if (Application.platform != RuntimePlatform.Android) 
		{
			UnityAndroidExtras.instance.makeToast("Dispositivo sem serviço de localização", 1);
			return false;
		}

		if (!Input.location.isEnabledByUser) 
		{
			UnityAndroidExtras.instance.makeToast("Ative o serviço de localização do celular", 1);
			return false;
		}

		Input.location.Start();

		if (Input.location.status == LocationServiceStatus.Failed)
		{
			UnityAndroidExtras.instance.makeToast("Falha ao obter sua localização", 1);
			return false;
		}
		else
		{
			_location = new double[2];

			_location[0] = System.Convert.ToDouble(Input.location.lastData.latitude);
			_location[1] = System.Convert.ToDouble(Input.location.lastData.longitude);

			UnityAndroidExtras.instance.makeToast("Localização obtida", 1);
		}
			
		Input.location.Stop();
		return true;
	}
}

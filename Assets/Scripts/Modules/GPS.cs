using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;

public class GPS 
{
	private static double[] _location;
	public static double[] location { get { return _location; } }

	public static bool ReceivePlayerLocation()
	{
		_location = null;

		if (Application.platform != RuntimePlatform.Android) 
		{
			AndroidToast.ShowMessage ("Dispositivo sem serviço de localização");
			return false;
		}

		if (!Input.location.isEnabledByUser) 
		{
			AndroidToast.ShowMessage ("Ative o serviço de localização do celular");
			return false;
		}

		Input.location.Start();

		int maxWait = 20;
		while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
		{
			new WaitForSeconds(1);
			maxWait--;
		}
			
		if (maxWait < 1)
		{
			AndroidToast.ShowMessage ("O tempo de busca de seu local esgotou");
			return false;
		}

		if (Input.location.status == LocationServiceStatus.Failed)
		{
			AndroidToast.ShowMessage ("Falha ao obter sua localização");
			return false;
		}
		else
		{
			_location = new double[2];

			_location[0] = System.Convert.ToDouble(Input.location.lastData.latitude);
			_location[1] = System.Convert.ToDouble(Input.location.lastData.longitude);

			Debug.Log("Localização obtida");
		}
			
		Input.location.Stop();
		return true;
	}
}

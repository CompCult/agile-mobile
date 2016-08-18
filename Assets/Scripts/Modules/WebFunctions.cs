using UnityEngine;
using System.Collections.Generic;

public static class WebFunctions
{
	// http://agile-admin-dev.herokuapp.com/api
	// http://arenademetis.com.br/api
	private static string _url = "http://arenademetis.com.br/api",
	_pvtKey = "c64620ce5b6ef7c901c947cd38314e279421d489",
	_apiPlace = "/";

	public static string url { get { return _url; } }
	public static string pvtKey { get { return _pvtKey; } }
	public static string apiPlace { get { return _apiPlace; } set { _apiPlace = value; } }
		
	public static WWW Get(string url)
	{
		WWW www = new WWW (url);

		WaitForSeconds w;
		while (!www.isDone)
			w = new WaitForSeconds(0.1f);

		return www; 
	}

	public static WWW Post(string url, WWWForm form)
	{
		WWW www = new WWW(url, form);

		WaitForSeconds w;
		while (!www.isDone)
			w = new WaitForSeconds(0.1f);

		return www; 
	}

	public static bool haveError(WWW response)
	{
		if (response == null) 
		{
			UnityAndroidExtras.instance.makeToast("Sem conexão", 1);
			return true;
		}

		if (response.text != null) 
		{
			if (response.text.Equals("")) 
			{
				UnityAndroidExtras.instance.makeToast("Falha no servidor", 1);
				return true;
			}

			if (response.text != null && response.text.Contains("invalid ")) 
			{
				UnityAndroidExtras.instance.makeToast("PIN inválido", 1);
				return true;
			}
		}

		if (response.error != null) 
		{
			if (response.error.Contains ("404")) {
				UnityAndroidExtras.instance.makeToast("Não encontrado", 1);
				return true;
			}
			if (response.error.Contains ("500")) {
				UnityAndroidExtras.instance.makeToast("Houve um problema na requisitação", 1);
				return true;
			}
		}

		return false;
	}
}
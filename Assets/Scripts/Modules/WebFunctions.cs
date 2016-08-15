using UnityEngine;
using System.Collections.Generic;

public class WebFunctions
{
	protected static string _url = "http://agile-admin-dev.herokuapp.com/api",
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
		Debug.Log (response.text);

		if (response == null) 
		{
			AndroidToast.ShowMessage ("Sem conexão");
			return true;
		}

		if (response.text != null) 
		{
			if (response.text != null && response.text.Contains("invalid ")) {
				AndroidToast.ShowMessage ("PIN inválido");
				return true;
			}
		}

		if (response.error != null) 
		{
			if (response.error.Contains ("404")) {
				AndroidToast.ShowMessage ("Não encontrado");
				return true;
			}
			if (response.error.Contains ("500")) {
				AndroidToast.ShowMessage ("Houve um problema na requisitação");
				return true;
			}
		}

		return false;
	}
}
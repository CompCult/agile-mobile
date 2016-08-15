using UnityEngine;
using UnityEngine.UI;
using System;

public class Login : Screen 
{
	[Header("Screen elements")]
	public Dropdown teamSelector;
	public InputField pinField;

	public void Start() 
	{
		nextScene = "Home";
		backScene = null;
	}

	public void SignIn()
	{
		string pin = pinField.text,
		team = teamSelector.captionText.text;

		AndroidToast.ShowMessage ("Conectando...");

		WWW loginRequest = Authenticator.RequestTeam (pin, team);

		processLogin (loginRequest);
	}

	public void processLogin (WWW loginRequest)
	{
		if (!WebFunctions.haveError(loginRequest)) 
		{
			Debug.Log ("Recebido: " + loginRequest.text);

			UsrManager.UpdateTeam (loginRequest.text);
			base.LoadNextScene ();
		} 
	}
}

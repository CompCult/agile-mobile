using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class Login : Screen {

	public Dropdown TeamSelector;
	public InputField PINField;

	public void Start() 
	{
		Controller = GameObject.Find("Controller").GetComponent<Controller>();
		
		APIPlace = "/auth/";
		NextScene = "Home";
		BackScene = null;
	}

	public void CreateLoginForm() 
	{
		string PIN = PINField.text,
			   Team = TransformInSlug(TeamSelector.captionText.text);

		if (PIN.Length < 1)
			PIN = "1";

		WWWForm form = new WWWForm ();
		form.AddField ("slug", Team);
		form.AddField ("pin", PIN);
		WWW www = new WWW (Controller.GetURL() + APIPlace + Controller.GetKey(), form);

		Debug.Log("Trying to connect...");
		ShowToastMessage("Conectando...");

		StartCoroutine(SendLoginForm(www));
	}
 
    private IEnumerator SendLoginForm(WWW www)
    {
        yield return www;
        
        string JSON = www.text;
        string Error = www.error;

        if (Error == null)
        {
	        Debug.Log("Response: " + JSON);

	       	Controller.UpdateTeam(JSON);
	       	LoadScene(NextScene);
        }
        else
        {
        	Debug.Log("Error: " + Error);

        	if (Error.Contains("invalid pin"))
        		ShowToastMessage("PIN inválido");
        }
     } 
}

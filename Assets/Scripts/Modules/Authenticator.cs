using UnityEngine;
using System;
using System.Collections;

public static class Authenticator
{
	public static WWW RequestTeam (string pin, string team) 
	{
		if (pin.Length < 1)
			pin = "-1";

		WWWForm loginForm = new WWWForm ();
		loginForm.AddField ("slug", TransformInSlug(team));
		loginForm.AddField ("pin", pin);

		WebFunctions.apiPlace = "/auth/";
		string apiLink = WebFunctions.url + WebFunctions.apiPlace + WebFunctions.pvtKey;

		return WebFunctions.Post(apiLink, loginForm);
	}

	private static string TransformInSlug(string team)
	{
		team = team.ToLower().Replace(" ", "-");
		return team; 
	}
}

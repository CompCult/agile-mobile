using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Home : Screen 
{
	public Text teamNameField, goldCoinsField, silverCoinsField;
	public Image teamEmblem;
	private IEnumerator UpdateTeamLoop;

	public void Start() 
	{
		backScene = "Login";
		nextScene = "Home";

		UpdateTeamLoop = UpdateTeamInformation ();
		UpdateFieldsOnScreen ();
	}

	private void UpdateFieldsOnScreen()
	{
		string teamName = UsrManager.team.name;
		int goldCoins = UsrManager.team.gold_coins,
		silverCoins = UsrManager.team.silver_coins;

		goldCoinsField.text = goldCoins.ToString();
		silverCoinsField.text = silverCoins.ToString();
		teamNameField.text = teamName;

		SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();
        sr.sprite = Resources.Load<Sprite>("Emblem/" + teamName);

        teamEmblem.sprite = sr.sprite;

		StopCoroutine (UpdateTeamLoop);
		StartCoroutine (UpdateTeamLoop);
	}

	private IEnumerator UpdateTeamInformation()
	{
		string pin = UsrManager.team.pin.ToString(),
		team = UsrManager.team.slug;

		yield return new WaitForSeconds (30);
		WWW updateRequest = Authenticator.RequestTeam (pin, team);

		if (!WebFunctions.haveError (updateRequest)) 
		{
			Debug.Log ("Updated team info.");

			UsrManager.UpdateTeam (updateRequest.text);
			LoadNextScene (); // Reload current scene
		} 
		else 
		{
			Debug.Log ("Failed to update team info.");
			UpdateFieldsOnScreen ();
		}
	}

}

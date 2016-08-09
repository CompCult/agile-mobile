using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Home : Screen {

	public Text TeamNameField, GoldCoinsField, SilverCoinsField;
	public Image TeamEmblem;

	public void Start() 
	{
		Controller = GameObject.Find("Controller").GetComponent<Controller>();
		APIPlace = "/auth/";
		BackScene = "Login";

		UpdateFieldsOnScreen();
		UpdateTeamInfo();
	}

	private void UpdateFieldsOnScreen()
	{
		int GoldCoins = Controller.GetTeam().gold_coins,
			SilverCoins = Controller.GetTeam().silver_coins;

		string TeamName = Controller.GetTeam().name;

		GoldCoinsField.text = GoldCoins.ToString();
		SilverCoinsField.text = SilverCoins.ToString();
		TeamNameField.text = TeamName;

		SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();
        sr.sprite = Resources.Load<Sprite>("Emblem/" + TeamName);

        TeamEmblem.sprite = sr.sprite;
	}

	public void UpdateTeamInfo() 
	{
		string teamNameInSlug = TransformInSlug(Controller.GetTeam().name);
		int teamPIN = Controller.GetTeam().pin;

		WWWForm form = new WWWForm ();
		form.AddField ("slug", teamNameInSlug);
		form.AddField ("pin", teamPIN);
		WWW www = new WWW (Controller.GetURL() + APIPlace + Controller.GetKey(), form);

		Debug.Log("Updating team info...");
		StartCoroutine(SendUpdateTeamInfoRequest(www));
	}
 
    private IEnumerator SendUpdateTeamInfoRequest(WWW www)
    {
        yield return www;
        
        string JSON = www.text,
        	   Error = www.error;

        if (Error == null)
        {
	        Debug.Log("Team info updated");
	        
	        Controller.UpdateTeam(JSON);
	        UpdateFieldsOnScreen();
        }
        else
        {
        	Debug.Log("Error on updating team info: " + Error);
        }
     } 
}

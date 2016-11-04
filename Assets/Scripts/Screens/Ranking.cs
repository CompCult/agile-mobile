using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class Ranking : GenericScreen 
{
	public GameObject teamCard, updatingText;
	public Image teamEmblem, teamMedal, teamBackground;
	public Text teamName, goldCoins, silverCoins;

	public List<Team> teamList;

	public void Start () 
	{
		nextScene = "Ranking";
		backScene = "Home";
		UnityAndroidExtras.instance.Init();

		RequestRanking();
	}

	public void RequestRanking() 
	{
		WebFunctions.apiPlace = "/group/get/all/";
		string rankingURL = WebFunctions.url + WebFunctions.apiPlace + WebFunctions.pvtKey;

		WWW rankingRequest = WebFunctions.Get (rankingURL);
		if (!WebFunctions.haveError (rankingRequest))
			OrderTeams (rankingRequest.text);
		else
			UnityAndroidExtras.instance.makeToast("Tente atualizar o ranking novamente", 1);
	}
 
     private void OrderTeams(string ranking)
     {
		string[] rankingJSON = ranking.Replace ("[", "").Replace ("]", "").Replace ("},{", "}%{").Split ('%');
     	teamList = new List<Team>();

		foreach (string teamJSON in rankingJSON)
        {
			Team team = UsrManager.CreateTeamFromJSON(teamJSON);
        	team.total_points = team.silver_coins + (10 * team.gold_coins);

        	teamList.Add(team);
        }

        teamList.Sort((x, y) => y.total_points.CompareTo(x.total_points));

        CreateTeamsCard();
     }

     private void CreateTeamsCard()
     {
     	Vector3 Position = teamCard.transform.position;

     	foreach (Team team in teamList)
        {
        	teamName.text = team.name;
			goldCoins.text = team.gold_coins.ToString();
			silverCoins.text = team.silver_coins.ToString();

            Position = new Vector3(Position.x, Position.y - 100, Position.z);
            
            SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();
            sr.sprite = Resources.Load<Sprite>("Emblem/" + team.name);

            teamEmblem.sprite = sr.sprite;

            GameObject Card = (GameObject) Instantiate(teamCard, Position, Quaternion.identity);
            Card.transform.SetParent(GameObject.Find("Teams").transform, false);

            Debug.Log(team.ToString());
        }

        updatingText.SetActive(false);
        Destroy(teamCard);

		StartCoroutine (UpdateRanking ());
     }

	private IEnumerator UpdateRanking()
	{
		yield return new WaitForSeconds (60);
		LoadNextScene ();

		Debug.Log ("Ranking updated");
	}
}

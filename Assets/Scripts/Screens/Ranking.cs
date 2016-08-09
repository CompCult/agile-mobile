using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Ranking : Screen {

	public GameObject MyTeamCard;
	public Image MyTeamEmblem, MyTeamMedal, MyTeamBG;
	public Text MyTeamName, MyGoldCoins, MySilverCoins;

	public GameObject TeamCard;
	public Image TeamEmblem, TeamMedal, TeamBG;
	public Text TeamName, GoldCoins, SilverCoins;

	public List<Team> teamList;

	public void Start () 
	{
		Controller = GameObject.Find("Controller").GetComponent<Controller>();
		BackScene = "Home";
		APIPlace = "/group/get/all/";

		RequestRanking();
	}

	public void RequestRanking() 
	{
		WWW www = new WWW (Controller.GetURL() + APIPlace + Controller.GetKey());

		Debug.Log("Requesting ranking...");
		StartCoroutine(SendRankingRequest(www));
	}
 
    private IEnumerator SendRankingRequest(WWW www)
    {
        yield return www;
        
        string JSON = www.text,
        	   Error = www.error;

       	string[] TeamsInJSON = JSON.Replace("[","").Replace("]","").Replace("},{", "}%{").Split('%');

       	if (Error == null)
       	{
       		Debug.Log("Teams received");
        	OrderTeams(TeamsInJSON);
        }
        else
        {
        	Debug.Log("Error on getting ranking: " + Error);
        }
     } 

     private void OrderTeams(string[] TeamsInJSON)
     {
     	teamList = new List<Team>();

        foreach(string teamJSON in TeamsInJSON)
        {
        	Team team = Controller.CreateTeam(teamJSON);
        	team.total_points = team.silver_coins + (10 * team.gold_coins);

        	teamList.Add(team);
        }

        teamList.Sort((x, y) => x.total_points.CompareTo(y.total_points));

        foreach (Team team in teamList)
        	Debug.Log(team.ToString());
     }

}

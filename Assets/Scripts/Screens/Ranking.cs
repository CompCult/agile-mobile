using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Ranking : Screen {

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

        CreateTeamsCard();
     }

     private void CreateTeamsCard()
     {
     	Vector3 Position = TeamCard.transform.position;

     	foreach (Team team in teamList)
        {
        	TeamName.text = team.name;
        	GoldCoins.text = "" + team.gold_coins;
        	SilverCoins.text = "" + team.silver_coins;

            Position = new Vector3(Position.x, Position.y - 100, Position.z);
            
            SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();
            sr.sprite = Resources.Load<Sprite>("Emblem/" + team.name);

            TeamEmblem.sprite = sr.sprite;

            GameObject Card = (GameObject) Instantiate(TeamCard, Position, Quaternion.identity);
            Card.transform.SetParent(GameObject.Find("Teams").transform, false);

            Debug.Log(team.ToString());
        }

        Destroy(TeamCard);
     }

}

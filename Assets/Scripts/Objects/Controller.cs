using UnityEngine;
using System;
using System.Collections;

public class Controller : MonoBehaviour {

	private string URL = "http://agile-admin-dev.herokuapp.com/api",
				   Key = "c64620ce5b6ef7c901c947cd38314e279421d489";

	private Team Team;
	private double[] Location;

	public void Awake() 
	{
        DontDestroyOnLoad(transform.gameObject);
    }

	public void UpdateTeam(string JSON)
	{
		Team = JsonUtility.FromJson<Team>(JSON);

	 	Debug.Log(Team.ToString());
	}

    public Team CreateTeam(string JSON)
    {
        return JsonUtility.FromJson<Team>(JSON);
    }

	public IEnumerator ReceivePlayerLocation()
    {
        Location = null;

        // First, check if user has location service enabled
        if (!Input.location.isEnabledByUser)
            yield break;

        // Start service before querying location
        Input.location.Start();

        // Wait until service initializes
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // Service didn't initialize in 20 seconds
        if (maxWait < 1)
        {
            Debug.Log("Timed out while trying to get device location");
            yield break;
        }

        // Connection has failed
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.Log("Unable to determine device location");
            yield break;
        }
        else
        {
            Location = new double[2];
            
            Location[0] = System.Convert.ToDouble(Input.location.lastData.latitude);
            Location[1] = System.Convert.ToDouble(Input.location.lastData.longitude);

            Debug.Log("Localization registered!");
        }

        // Stop service if there is no need to query location updates continuously
        Input.location.Stop();
    }

     public Team GetTeam() { return Team; }
     public string GetURL() { return URL; }
     public string GetKey() { return Key; }
     public double[] GetLocation() { return Location; }
}

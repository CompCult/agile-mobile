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

	private IEnumerator ReceiveCurrentLocationFromGPS()
    {
    	Location = null;
        Screen ScreenSystem = GameObject.Find("ScreenSystem").GetComponent<Screen>();

        Debug.Log("Coletando localização...");

        // First, check if user has location service enabled
        if (!Input.location.isEnabledByUser)
        {
            ScreenSystem.ShowToastMessage("Ative o serviço de localização");
            return false;
        }

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
            yield break;

        // Connection has failed
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            ScreenSystem.ShowToastMessage("Falha ao receber sua localização");
            yield break;
        }
        else
        {
        	Location = new double[2];
        	
            Location[0] = System.Convert.ToDouble(Input.location.lastData.latitude);
            Location[1] = System.Convert.ToDouble(Input.location.lastData.longitude);

            ScreenSystem.ShowToastMessage("Localização coletada");
            Debug.Log("Localization registered!");
        }

        // Stop service if there is no need to query location updates continuously
        Input.location.Stop();
    }

     public Team GetTeam() { return Team; }
     public string GetURL() { return URL; }
     public string GetKey() { return Key; }
     public double[] GetLocation() { ReceiveCurrentLocationFromGPS(); return Location; }
}

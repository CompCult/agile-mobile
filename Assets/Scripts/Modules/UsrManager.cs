using UnityEngine;
using System.Collections;

public class UsrManager
{
	private static Team _team;
	public static Team team { get { return _team; } }

	public static void UpdateTeam(string JSON)
	{
		_team = JsonUtility.FromJson<Team>(JSON);
	}

	public static Team CreateTeamFromJSON(string JSON)
	{
		return JsonUtility.FromJson<Team>(JSON);
	}

}

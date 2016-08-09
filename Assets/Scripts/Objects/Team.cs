using UnityEngine;
using System.Collections;

public class Team {

	public string name,
				  slug,
				  participants,
				  created_at,
				  updated_at;

	public int id,
			   pin,
			   gold_coins,
			   silver_coins,
			   total_points;

	public override string ToString()
	{
		return "id " + id + " - " 
				+ name + " - " 
				+ gold_coins + "G e " 
				+ silver_coins + "S e " 
				+ total_points + "TP";
	}
}

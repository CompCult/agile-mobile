using UnityEngine;
using System.Collections;

public class Modules : MonoBehaviour 
{
	public void Awake()
	{
		DontDestroyOnLoad (transform.gameObject);
	}
}

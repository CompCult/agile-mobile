using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GenericScreen : MonoBehaviour 
{
	protected static string backScene,
	nextScene;

	public virtual void Update()
	{
		if (Input.GetKeyUp(KeyCode.Escape)) 
			LoadBackScene();
	}

	public void LoadNextScene()
	{
		LoadScene(nextScene);
	}

	public void LoadBackScene()
	{
		LoadScene(backScene);
	}

	public void LoadScene(string Scene) 
	{
		if (Scene != null) 
			SceneManager.LoadScene(Scene);
		else
			Application.Quit();
	}
}



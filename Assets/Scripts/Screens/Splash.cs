using UnityEngine;
using System.Collections;

public class Splash : GenericScreen 
{
	public void Start () 
	{
		nextScene = "Login";

		StartCoroutine(SplashTime());
	}

	private IEnumerator SplashTime () 
	{
        yield return new WaitForSeconds(4);

        // Disables Android Status Bar
		ApplicationChrome.statusBarState = ApplicationChrome.States.Hidden;
		// Enables Android Navigation Bar
		ApplicationChrome.navigationBarState = ApplicationChrome.States.Visible;

        base.LoadNextScene();
    }

}

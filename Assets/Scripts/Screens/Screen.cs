using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Screen : MonoBehaviour {

	protected string BackScene, NextScene, APIPlace, toastString;
	protected Controller Controller;
	AndroidJavaObject currentActivity;

	public virtual void Update()
	{
		if (Input.GetKeyUp(KeyCode.Escape)) 
    		LoadScene();
	}

	public void LoadScene()
	{
		LoadScene(BackScene);
	}

	public void LoadScene(string Scene) 
	{
		if (Scene != null) 
			SceneManager.LoadScene(Scene);
		else
			Application.Quit();
	}

	protected string TransformInSlug(string Text)
    {
    	// Transforms ex.: 'Name Surname' in 'name-surname'
    	Text = Text.ToLower().Replace(" ", "-");
    	return Text; 
    }

    public void ShowToastMessage(string toastString)
    {
    	if (Application.platform != RuntimePlatform.Android)
    		return;

		AndroidJavaClass UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		 
		currentActivity = UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
		this.toastString = toastString;
		 
		currentActivity.Call ("runOnUiThread", new AndroidJavaRunnable (ShowToast));
	}
	 
	public void ShowToast()
	{
		AndroidJavaObject context = currentActivity.Call<AndroidJavaObject>("getApplicationContext");
		AndroidJavaClass Toast = new AndroidJavaClass("android.widget.Toast");
		AndroidJavaObject javaString=new AndroidJavaObject("java.lang.String", toastString);
		AndroidJavaObject toast = Toast.CallStatic<AndroidJavaObject> ("makeText", context, javaString, Toast.GetStatic<int>("LENGTH_SHORT"));
		
		toast.Call("show");
	}
}



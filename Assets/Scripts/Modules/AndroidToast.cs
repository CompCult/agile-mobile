using UnityEngine;
using System.Collections;

public static class AndroidToast
{
	private static AndroidJavaObject AndroidObjectActivity;
	private static string toastMessage;

	public static void ShowMessage(string toastString)
	{
		Debug.Log("Toast: " + toastString);

		if (Application.platform != RuntimePlatform.Android)
			return;

		AndroidJavaClass UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");

		AndroidObjectActivity = UnityPlayer.GetStatic<AndroidJavaObject>("AndroidObjectActivity");
		toastMessage = toastString;

		AndroidObjectActivity.Call ("runOnUiThread", new AndroidJavaRunnable (ShowToast));
	}

	private static void ShowToast()
	{
		AndroidJavaObject context = AndroidObjectActivity.Call<AndroidJavaObject>("getApplicationContext");
		AndroidJavaClass Toast = new AndroidJavaClass("android.widget.Toast");
		AndroidJavaObject javaString = new AndroidJavaObject("java.lang.String", toastMessage);
		AndroidJavaObject toast = Toast.CallStatic<AndroidJavaObject> ("makeText", context, javaString, Toast.GetStatic<int>("LENGTHSHORT"));

		toast.Call("show");
	}
}

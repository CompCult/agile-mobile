using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ActivityQuiz : Screen {

	public GameObject Home, 
			 		  SearchActScreen,
			 		  SearchQuizScreen, 
			 		  QuestHome, 
			 		  QuizHome,
			 		  GPSScreen, 
			 		  MediaScreen,
			 		  EndScreen,
			 		  CameraField;

	public InputField ActivityID,
					  QuizID;

	//---------- QUEST START SCREEN --------------------
	public Text QuestHomeName, QuestHomeDescription, QuestHomePlace;
	//----------- GPS SCREEN  --------------------------
	public Text GPSScreenName;
	//----------- MEDIA SCREEN -------------------------
	public Text MediaScrenName;
	//----------- QUIZ SCREEN --------------------------
	public Text QuizName, QuizDescription, Alt1, Alt2, Alt3, Alt4;
	//--------------------------------------------------

	private Activity Activity;
	private Quiz Quiz;
	private WebCamTexture MobileCamera;

	public void Start () 
	{
		Controller = GameObject.Find("Controller").GetComponent<Controller>();
		
		BackScene = "Home";
		OpenScreen("Home");

		ShowCameraImage();
	}
	
	public override void Update()
	{
		if (Input.GetKeyUp(KeyCode.Escape)) 
		{
			if (Home.activeSelf)
				LoadScene();
			else
				OpenScreen("Home");
		}
	}

	public void OpenScreen(string Screen)
	{
		GameObject[] Screens = new GameObject[] {Home, SearchActScreen, SearchQuizScreen, QuestHome, QuizHome, GPSScreen, MediaScreen, EndScreen};

		foreach (GameObject ScreenChild in Screens)
		{
			if (ScreenChild.name.Equals(Screen))
				ScreenChild.SetActive(true);
			else
				ScreenChild.SetActive(false);
		}
	}

	public void CreateActivityOrQuizForm(string Type) 
	{
		string ID = null;

		if (Type.Equals("Activity"))
		{
			ID = ActivityID.text;
			APIPlace = "/activity/get/";
		}
		else
		{
			ID = QuizID.text;
			APIPlace = "/quiz/get/";
		}

		if (ID.Length < 1)
			ID += "1";

		Debug.Log("Requesting Quiz/Activity with ID " + ID + "...");

		WWW www = new WWW (Controller.GetURL() + APIPlace + ID + "/" + Controller.GetKey());

		StartCoroutine(SendActivityOrQuizForm(www, Type));
	}

	private IEnumerator SendActivityOrQuizForm(WWW www, string Type)
    {
        yield return www;
        
        string JSON = www.text;
        string Error = www.error;

        if (Error == null)
        {
	        Debug.Log("Response: " + JSON);

	        if (Type.Equals("Activity"))
	        {
	        	Activity = JsonUtility.FromJson<Activity>(JSON);
	        	StartActivity();
	        }
	        else
	        {
	        	Quiz = JsonUtility.FromJson<Quiz>(JSON);
	        	StartQuiz();
	        }
        }
        else
        {
        	Debug.Log("Error: " + Error);
        }
     } 

     private void StartActivity()
     {
     	QuestHomeName.text = Activity.name;
     	QuestHomeDescription.text = Activity.description;
     	QuestHomePlace.text = Activity.location;

     	GPSScreenName.text = Activity.name;
     	MediaScrenName.text = Activity.name;

     	OpenScreen("Quest Home");
     }

     private void StartQuiz()
     {
     	QuizName.text = Quiz.name;
     	QuizDescription.text = Quiz.question;
     	Alt1.text = Quiz.option_1;
     	Alt2.text = Quiz.option_2;
     	Alt3.text = Quiz.option_3;
     	Alt4.text = Quiz.option_4;

     	OpenScreen("Quiz Home");
     }

     public void ContinueActivity()
     {
     	if (QuizHome.activeSelf)
     		OpenScreen("End");
     	else 
     	{
	     	if (Activity.is_gps_enabled && !(GPSScreen.activeSelf) && !(MediaScreen.activeSelf))
	     		OpenScreen("GPS Screen");
	     	else if (Activity.is_media_enabled && !(MediaScreen.activeSelf))
	     		OpenScreen("Media Screen");
	     	else
	     		OpenScreen("End");
	    }
     }

    private void ShowCameraImage()
	{
		MobileCamera = new WebCamTexture();
        CameraField.GetComponent<Renderer>().material.mainTexture = MobileCamera;

        if (HaveCamera())
            MobileCamera.Play();
	}


    public void TrySendPhoto()
    {
        if (HaveCamera())
            StartCoroutine(RecordPhoto());
        else
        	ContinueActivity();
    }

    private IEnumerator RecordPhoto()
    {
        yield return new WaitForEndOfFrame(); 

		// Creates a texture to hold the photo
        Texture2D photo = new Texture2D(MobileCamera.width, MobileCamera.height);
        photo.SetPixels(MobileCamera.GetPixels());
        photo.Apply();

        CameraField.GetComponent<Renderer>().material.mainTexture = photo;

        yield return new WaitForSeconds(4);
        ShowCameraImage();
        ContinueActivity();
    }

    public bool HaveCamera() { return (WebCamTexture.devices.Length > 0) ? true : false; }
}

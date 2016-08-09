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
	public Text MediaScreenName;
	//----------- QUIZ SCREEN --------------------------
	public Text QuizName, QuizDescription, Alt1, Alt2, Alt3, Alt4;
	//--------------------------------------------------

	private Activity Activity;
	private Quiz Quiz;
	private WebCamTexture MobileCamera;

	//------------ QUEST RESPONSES --------------------
	private Texture2D Photo;
	private bool IsQuizCorrect, QuestSent;

	public void Start () 
	{
		Controller = GameObject.Find("Controller").GetComponent<Controller>();
		
		BackScene = "Home";
		OpenScreen("Home");

		ShowCameraImage();
		Controller.GetLocation();
	}
	
	public override void Update()
	{
		if (Input.GetKeyUp(KeyCode.Escape)) 
		{
			if (Home.activeSelf)
			{
				CameraField.GetComponent<Renderer>().material.mainTexture = null;
            	MobileCamera.Stop();

				LoadScene();
			}
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

	public void PrepareQuestForm(string Type) 
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
			ID = "-1";

		ShowToastMessage("Buscando missão...");
		Debug.Log("Requesting Quiz/Activity with ID " + ID + "...");

		WWW www = new WWW (Controller.GetURL() + APIPlace + ID + "/" + Controller.GetKey());

		StartCoroutine(ReadQuestForm(www, Type));
	}

	private IEnumerator ReadQuestForm(WWW www, string Type)
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

        	if (Error.Contains("404"))
        		ShowToastMessage("Missão não encontrada");
        	else
        		ShowToastMessage("Falha no servidor");
        }
     } 

     private void StartActivity()
     {
     	QuestHomeName.text = Activity.name;
     	QuestHomeDescription.text = Activity.description;
     	QuestHomePlace.text = Activity.location;

     	GPSScreenName.text = Activity.name;
     	MediaScreenName.text = Activity.name;

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
     	if (QuizHome.activeSelf) // Se a tela de quiz está aberta, então acabou a missão.
     		OpenScreen("End");
     	else 
     	{
     		// Se a tela a missão tem GPS e tela de GPS e de Media estão Offline, ative a tela de GPS.
	     	if (Activity.is_gps_enabled && !(GPSScreen.activeSelf) && !(MediaScreen.activeSelf))
	     		OpenScreen("GPS Screen");
	     	// Se tem media e a tela de Media não está ativa, ative a tela de Media.
	     	else if (Activity.is_media_enabled && !(MediaScreen.activeSelf))
	     		OpenScreen("Media Screen");
	     	else
	     		OpenScreen("End");
	    }
     }

    private void ShowCameraImage()
	{
		CameraField.GetComponent<Renderer>().material.mainTexture = null;
			
		if (MobileCamera != null)
			MobileCamera.Stop();
		
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
        Photo = new Texture2D(MobileCamera.width, MobileCamera.height);
        Photo.SetPixels(MobileCamera.GetPixels());
        Photo.Apply();

        CameraField.GetComponent<Renderer>().material.mainTexture = Photo;

        ShowToastMessage("Foto capturada");

        ShowCameraImage();
        ContinueActivity();
    }

    public void AnswerQuiz(Text Answer)
    {
    	IsQuizCorrect = false;

    	if (Quiz.correct.Equals(Answer.text))
    		IsQuizCorrect = true;

    	OpenScreen("End");
    }

    public void RegisterPlayerCoordinates(string Step)
    {
    	// FALAR COM CHICO
    	string PlayerLocation = "" + Controller.GetLocation();

    	if (PlayerLocation == null)
    		return;

    	if (Step.Equals("coord_start"))
    		Activity.coord_start = PlayerLocation;
    	else if (Step.Equals("coord_mid"))
    		Activity.coord_mid = PlayerLocation;
    	else
    		Activity.coord_end = PlayerLocation;
    }

    public void SendActivity()
    {
    	if (QuestSent)
    		return;

    	QuestSent = true;

    	// FORM DE ENVIO
    	ShowToastMessage("Missão enviada");
    	OpenScreen("Home");
    }

    public bool HaveCamera() { return (WebCamTexture.devices.Length > 0) ? true : false; }
}

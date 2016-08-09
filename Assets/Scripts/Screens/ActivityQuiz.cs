using UnityEngine;
using UnityEngine.UI;
using System;
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
			 		  VoiceScreen,
			 		  EndScreen,
			 		  CameraField;

	public InputField ActivityID,
					  QuizID;

	[Header("Quest screen elements")]
	public Text QuestHomeName, QuestHomeDescription, QuestHomePlace;
	[Header("GPS screen elements")]
	public Text GPSScreenName;
	[Header("Photo screen elements")]
	public Text MediaScreenName;
	[Header("Quiz screen elements")]
	public Text QuizName, QuizDescription, Alt1, Alt2, Alt3, Alt4;
	[Header("Audio screen elements")]
    private bool micConnected = false;
    private int minFreq;
    private int maxFreq;
    public AudioSource SampleAudioSource, RecordedAudio;

	[Header("Quest objects")]
	private Activity Activity;
	private Quiz Quiz;
	private WebCamTexture MobileCamera;

	[Header("Responses")]
	private Texture2D Photo;
	private byte[] AudioSample;
	private bool QuestSent, isQuiz;
	private string QuizStatus, CoordStart, CoordMid, CoordEnd;

	public void Start () 
	{
		Controller = GameObject.Find("Controller").GetComponent<Controller>();
		
		BackScene = "Home";
		OpenScreen("Home");

		ShowCameraImage();
		RegisterPlayerCoordinates("Start");
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

	// Sequence: Home - GPS - Media - Voice - End
	public void OpenScreen(string Screen)
	{
		GameObject[] Screens = new GameObject[] {Home, SearchActScreen, 
												SearchQuizScreen, QuestHome, 
												QuizHome, GPSScreen, MediaScreen, 
												VoiceScreen, EndScreen};

		if (Screen.Equals("GPS Screen") && !Activity.gps_enabled)
			OpenScreen("Media Screen");
		else if (Screen.Equals("Media Screen") && !Activity.photo_enabled)
			OpenScreen("Voice Screen");
		else if (Screen.Equals("Voice Screen") && !Activity.audio_enabled)
			OpenScreen("End");
		else 
		{
			foreach (GameObject ScreenChild in Screens)
			{
				if (ScreenChild.name.Equals(Screen))
				{
					ScreenChild.SetActive(true);
				}
				else
					ScreenChild.SetActive(false);
			}
		}
	}

	public void PrepareQuestForm(string Type) 
	{
		string ID = null;

		if (Type.Equals("Activity"))
		{
			ID = ActivityID.text;
			APIPlace = "/activity/get/";
			isQuiz = false;
		}
		else
		{
			ID = QuizID.text;
			APIPlace = "/quiz/get/";
			isQuiz = true;
		}

		if (ID.Length < 1)
			ID = "-1";

		Debug.Log("Requesting Quiz/Activity with ID " + ID + "...");
		Debug.Log("At " + Controller.GetURL() + APIPlace + ID + "/" + Controller.GetKey());

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

     private void ClearPreviousQuests()
     {
     	Photo = null;
		AudioSample = null;
		QuestSent = false;
		QuizStatus = null;
		CoordStart = null;
		CoordMid = null;
		CoordEnd = null;

		Debug.Log("Atividades anteriores foram limpas");
     }

     private void StartActivity()
     {
     	ClearPreviousQuests();

     	QuestHomeName.text = Activity.name;
     	QuestHomeDescription.text = Activity.description;
     	QuestHomePlace.text = Activity.location;

     	GPSScreenName.text = Activity.name;
     	MediaScreenName.text = Activity.name;

     	OpenScreen("Quest Home");
     }

     private void StartQuiz()
     {
     	ClearPreviousQuests();

     	QuizName.text = Quiz.name;
     	QuizDescription.text = Quiz.question;

     	Alt1.text = Quiz.option_1;
     	Alt2.text = Quiz.option_2;
     	Alt3.text = Quiz.option_3;
     	Alt4.text = Quiz.option_4;

     	OpenScreen("Quiz Home");
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
        	OpenScreen("Voice Screen");
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
        OpenScreen("Voice Screen");
    }

    public void AnswerQuiz(string Answer)
    {
    	QuizStatus = "Errou";

    	if (Quiz.correct.Equals(Answer))
    		QuizStatus = "Acertou";

    	OpenScreen("End");
    }

     public void CheckCoordsAndContinue()
     {
     	if (CoordStart == null || CoordMid == null || CoordEnd == null)
     	{
     		ShowToastMessage("Você não marcou as três localizações");
     		Debug.Log("As três localizações não foram marcadas");
     	}
     	else
     	{
     		OpenScreen("Media Screen");
     	}
     }

    public IEnumerator RegisterPlayerCoordinates(string Step)
    {
    	yield return StartCoroutine(Controller.UpdatePlayerLocation());

    	// Retrieve the user location
    	if (Controller.GetLocation() == null)
		{
			ShowToastMessage("Verifique o serviço de localização do celular");
			Debug.Log("Falha ao obter sua localização!");

			yield return StartCoroutine(Controller.UpdatePlayerLocation());
		}

    	string PlayerLocation = Controller.GetLocation()[0] + " | " + Controller.GetLocation()[1];

    	if (Step.Equals("coord_start"))
    		CoordStart = PlayerLocation;
    	else if (Step.Equals("coord_mid"))
    		CoordMid = PlayerLocation;
    	else if (Step.Equals("coord_end"))
    		CoordEnd = PlayerLocation;

    	ShowToastMessage("Local registrado");
    }

    public void RecordMicrophone() 
    {
    	if(Microphone.devices.Length <= 0)
        {
            Debug.Log("Microphone not connected!");
            ShowToastMessage("Nenhum microfone encontrado");
        }
        else 
        {
            micConnected = true;
            Microphone.GetDeviceCaps(null, out minFreq, out maxFreq);
                        
            if(minFreq == 0 && maxFreq == 0)
                maxFreq = 44100;
        }

        if(micConnected)
        {
            if(!Microphone.IsRecording(null))
            {
            	Debug.Log("Clique novamente para parar a gravação");
            	ShowToastMessage("Clique novamente para parar a gravação");
                SampleAudioSource.clip = Microphone.Start(null, true, 20, maxFreq);
            }
            else //Recording is in progress
            {
                Microphone.End(null); //Stop the audio recording
                AudioClip source = SampleAudioSource.clip;
                float[] samples = new float[SampleAudioSource.clip.samples * SampleAudioSource.clip.channels];
                source.GetData(samples, 0);

                // Copy the float data into a byte array
                AudioSample = new byte[samples.Length * 4];
				Buffer.BlockCopy(samples, 0, AudioSample, 0, AudioSample.Length);

				ShowToastMessage("Voz gravada com sucesso");
            }            
        }
    }

    public void PlayRecordedSound()
    {
    	if (AudioSample == null)
    		ShowToastMessage("Nenhum som gravado");

		float[] f = ConvertByteToFloat(AudioSample);
		AudioClip audioClip = AudioClip.Create("testSound", f.Length, 1, 44100, false, false);
		audioClip.SetData(f, 0);

		RecordedAudio.clip = audioClip;
		if (Application.platform != RuntimePlatform.Android)
			RecordedAudio.pitch = 2.2f;
		else
			RecordedAudio.pitch = 1;

		RecordedAudio.Play();
    }

    public float[] ConvertByteToFloat(byte[] array)
	{
		float[] data = new float[array.Length / 4];
		Buffer.BlockCopy(array, 0, data, 0, array.Length);

		return data;
	}

    public void PrepareSendQuestForm()
    {
    	if (QuestSent)
    		return;

    	Debug.Log("Sending activity...");
		ShowToastMessage("Enviando pergaminho...");

    	QuestSent = true;
    	APIPlace = "/activity/post/";

    	WWWForm form = new WWWForm ();
    	form.AddField("group_id", Controller.GetTeam().id);
    	
    	if (isQuiz)
    	{
			form.AddField ("quiz_id", Quiz.id);
			Debug.Log("Quiz ID: " + Quiz.id);
			form.AddField("quiz_correct", QuizStatus);
			Debug.Log("Quiz Status: " + QuizStatus);
    	}
		else
		{
			form.AddField ("activity_id", Activity.id);
			Debug.Log("Atividade: " + Activity.id);
			
			form.AddField ("location", Activity.location);
			Debug.Log("Local: " + Activity.location);

			form.AddField ("coord_start", CoordStart);
			form.AddField ("coord_mid", CoordMid);
			form.AddField ("coord_end", CoordEnd);
		}

		if (Photo != null)
		{
			form.AddBinaryData("photo", Photo.EncodeToPNG(), "Photo.png", "image/png");
			Debug.Log("Foto registrada");
		}
		else
		{
			Debug.Log("Foto não registrada");
		}

		if (AudioSample != null)
		{
			form.AddBinaryData("audio", AudioSample, "audio.wav", "sound/wav");
		}
		else
		{
			Debug.Log("Voz não registrada");
		}

		WWW www = new WWW (Controller.GetURL() + APIPlace + Controller.GetKey(), form);

		StartCoroutine(SendQuestForm(www));
    }

    private IEnumerator SendQuestForm(WWW www)
    {
    	yield return www;
        
        string JSON = www.text;
        string Error = www.error;

        if (Error == null)
        {
	        Debug.Log("Response: " + JSON);

	        ShowToastMessage("Pergaminho enviado!");
    		OpenScreen("Home");
        }
        else
        {
        	Debug.Log("Error: " + Error);
        	
        	ShowToastMessage("Falha ao enviar pergaminho");
        	QuestSent = false;
        }
    }

    public bool HaveCamera() { return (WebCamTexture.devices.Length > 0) ? true : false; }
}

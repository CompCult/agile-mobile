using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class ActivityQuiz : Screen 
{
	[Header("Screens")]
	public GameObject[] internalScreens;
	public GameObject currentScreen;

	[Header("Inputs")]
	public InputField activityID,
	quizID;

	[Header("Activity Screen Elements")]
	public Text activityHomeName,
	activityHomeDescription,
	activityHomeLocation;

	[Header("Quiz Screen Elements")]
	public Text quizName,
	quizDescription,
	alt1, alt2, alt3, alt4;

	[Header("Voice Screen Elements")]
	public AudioSource audioSource;

	[Header("Camera Scren Elements")]
	public GameObject cameraField;

	public void Start () 
	{
		nextScene = "ActivityQuiz";
		backScene = "Home";
		UnityAndroidExtras.instance.Init();

		ClearPreviousVoice ();

		StartCamera ();
		OpenScreen("Home");
	}
	
	public override void Update()
	{
		if (Input.GetKeyUp(KeyCode.Escape)) 
		{
			if (currentScreen.name.Equals ("Home")) 
			{
				CameraDevice.StopCameraImage ();
				LoadBackScene ();
			}
				else
				OpenScreen("Home");
		}
	}

	private void ClearPreviousVoice()
	{
		var filepath = Path.Combine(Application.persistentDataPath, "voice.wav");

		if (System.IO.File.Exists(filepath))
			System.IO.File.Delete (filepath); // Deletes previous voice files loaded
	}

	// Sequence: Home - GPS - Media - Voice - End
	public void OpenScreen(string screen)
	{
		if (screen.Equals("GPS Screen") && !QuestManager.activity.gps_enabled)
			OpenScreen("Media Screen");
		else if (screen.Equals("Media Screen") && !QuestManager.activity.photo_enabled)
			OpenScreen("Voice Screen");
		else if (screen.Equals("Voice Screen") && !QuestManager.activity.audio_enabled)
			OpenScreen("End");
		else 
		{
			foreach (GameObject screenChild in internalScreens) 
			{
				if (screenChild.name.Equals (screen))
				{
					currentScreen = screenChild;
					screenChild.SetActive (true);

					if (screen.Equals ("Media Screen"))
						CameraDevice.ShowCameraImage ();
					else
						CameraDevice.StopCameraImage ();
				} 
				else
					screenChild.SetActive (false);
			}

		}
	}

	public void ReceiveQuest(string type)
	{
		string ID, url;

		if (type.Equals("Activity"))
		{
			ID = activityID.text;
			WebFunctions.apiPlace = "/activity/get/";
		}
		else 
		{
			ID = quizID.text;
			WebFunctions.apiPlace = "/quiz/get/";
		}

		if (ID.Length < 1)
			ID = "-1";
				
		url =  WebFunctions.url + WebFunctions.apiPlace + ID + "/" + WebFunctions.pvtKey;

		WWW questForm = WebFunctions.Get (url);
		if (!WebFunctions.haveError (questForm)) 
		{
			ClearPreviousVoice ();

			if (type.Equals ("Activity")) 
			{
				QuestManager.UpdateActivity (questForm.text);
				QuestManager.activityResponse.activity_id = QuestManager.activity.id;
				QuestManager.activityResponse.group_id = UsrManager.team.id;

				StartActivity ();
			} 
			else 
			{
				QuestManager.UpdateQuiz (questForm.text);
				QuestManager.quizResponse.quiz_id = QuestManager.quiz.id;
				QuestManager.quizResponse.group_id = UsrManager.team.id;

				StartQuiz ();
			}
		}
	}

	private void StartActivity()
	{
		activityHomeName.text = QuestManager.activity.name;
		activityHomeDescription.text = QuestManager.activity.description;
		activityHomeLocation.text = QuestManager.activity.location;

		OpenScreen ("Activity Home");
	}

	private void StartQuiz()
	{
		quizName.text = QuestManager.quiz.name;
		quizDescription.text = QuestManager.quiz.question;

		alt1.text = QuestManager.quiz.option_1;
		alt2.text = QuestManager.quiz.option_2;
		alt3.text = QuestManager.quiz.option_3;
		alt4.text = QuestManager.quiz.option_4;

		OpenScreen("Quiz Home");
	}

	public void RequestCoordinates(string step)
	{
		UnityAndroidExtras.instance.makeToast("Obtendo localização...", 1);
		GPS.ReceivePlayerLocation ();

		if (GPS.location == null)
			return;

		string playerLocation = GPS.location[0] + " | " + GPS.location[1];

		switch (step) 
		{
		case "coord_start":
			QuestManager.activityResponse.coord_start = playerLocation;
			break;
		case "coord_mid":
			QuestManager.activityResponse.coord_mid = playerLocation;
			break;
		case "coord_end":
			QuestManager.activityResponse.coord_end = playerLocation;
			break;
		}
	}

	public void CheckCoordsAndContinue()
	{
		if (QuestManager.AreCoordsFilled ()) 
		{
			UnityAndroidExtras.instance.makeToast("Coordenadas preenchidas", 1);
			OpenScreen ("Media Screen");
		}
		else
			UnityAndroidExtras.instance.makeToast("Coordenadas não preenchidas", 1);
	}

	public void StartCamera()
	{
		CameraDevice.cameraPlane = cameraField;
		CameraDevice.ShowCameraImage ();
	}

	public void CapturePhoto()
	{
		CameraDevice.RecordPhoto ();
		QuestManager.activityResponse.photo = CameraDevice.Photo.EncodeToPNG ();

		new WaitForSeconds (4);

		OpenScreen ("Voice Screen");
	}

	public void RegisterQuizAnswer(string answer)
	{
		QuestManager.RegisterQuizResponse (answer);

		OpenScreen ("End");
	}

	public void RecordMicrophone()
	{
		AudioRec.audioSource = audioSource;
		AudioRec.RecordAudio ();
	}

	public void ListenAudio()
	{
		AudioRec.ListenAudio ();
	}

	public void CheckVoiceAndContinue()
	{
		if (AudioRec.audioSource.clip != null)
			OpenScreen ("End");
		else
			UnityAndroidExtras.instance.makeToast ("Nenhum áudio gravado", 1);

		if (!AudioRec.isRecorded())
			UnityAndroidExtras.instance.makeToast("Nenhuma voz gravada", 1);
		else
		{
			var filepath = Path.Combine(Application.persistentDataPath, "voice.wav");
			QuestManager.activityResponse.audio = System.IO.File.ReadAllBytes(filepath);

			OpenScreen("End");
		}
	}

	public void SendQuestForm()
	{
		WWW response;

		if (QuestManager.activity != null)
			response = QuestManager.SendActivity ();
		else
			response = QuestManager.SendQuiz ();

		if (!WebFunctions.haveError (response)) 
		{
			Debug.Log ("Resposta: " + response.text);
			UnityAndroidExtras.instance.makeToast ("Enviado com sucesso", 1);
		}

		OpenScreen ("Home");
	}
}

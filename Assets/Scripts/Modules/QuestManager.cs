using UnityEngine;
using System.Collections;

public static class QuestManager
{
	private static Activity _activity;
	private static ActivityResponse _activityResponse;

	public static Activity activity { get { return _activity; } set { _activity = value; } }
	public static ActivityResponse activityResponse { get { return _activityResponse; } set { _activityResponse = value; } }
		
	private static Quiz _quiz;
	private static QuizResponse _quizResponse;

	public static Quiz quiz { get { return _quiz; } set { _quiz = value; } }
	public static QuizResponse quizResponse { get { return _quizResponse; } set { _quizResponse = value; } }

	public static void UpdateActivity(string JSON)
	{
		_quiz = null;
		_activity = JsonUtility.FromJson<Activity>(JSON);
		_activityResponse = new ActivityResponse ();
	}

	public static void UpdateQuiz(string JSON)
	{
		_activity = null;
		_quiz = JsonUtility.FromJson<Quiz>(JSON);
		_quizResponse = new QuizResponse ();
	}

	public static WWW SendQuiz()
	{
		WebFunctions.apiPlace = "/activity/post/";
		string url = WebFunctions.url + WebFunctions.apiPlace + WebFunctions.pvtKey;

		WWWForm responseForm = new WWWForm ();

		responseForm.AddField("group_id", quizResponse.group_id);
		responseForm.AddField ("activity_id", quizResponse.quiz_id);
		responseForm.AddField ("quiz_correct", quizResponse.quiz_correct);

		Debug.Log ("Enviando para " + url);
		WWW response =  WebFunctions.Post(url, responseForm);

		return response;
	}

	public static WWW SendActivity()
	{
		WebFunctions.apiPlace = "/activity/post/";
		string url = WebFunctions.url + WebFunctions.apiPlace + WebFunctions.pvtKey;

		WWWForm responseForm = new WWWForm ();

		responseForm.AddField("group_id", activityResponse.group_id);
		responseForm.AddField ("activity_id", activityResponse.activity_id);

		if (AreCoordsFilled () && (Application.platform == RuntimePlatform.Android)) 
		{
			responseForm.AddField ("coord_start", activityResponse.coord_start);
			responseForm.AddField ("coord_mid", activityResponse.coord_mid);
			responseForm.AddField ("coord_end", activityResponse.coord_end);

			Debug.Log ("Coordenadas registradas");
		}
		else 
		{
			Debug.Log ("Coordenadas não registradas");
		}

		if (activityResponse.photo != null)
		{
			responseForm.AddBinaryData("photo", activityResponse.photo, "Photo.png", "image/png");
			Debug.Log("Foto registrada");
		}
		else
		{
			Debug.Log("Foto não registrada");
		}

		if (activityResponse.audio != null)
		{
			responseForm.AddBinaryData("audio", activityResponse.audio, "voice.wav", "audio/wav");
			Debug.Log("Voz registrada");
		}
		else
		{
			Debug.Log("Voz não registrada");
		}

		Debug.Log (activityResponse.ToString ());
		Debug.Log ("Enviando para " + url);
		WWW response =  WebFunctions.Post(url, responseForm);

		return response;
	}

	public static bool AreCoordsFilled()
	{
		if (Application.platform != RuntimePlatform.Android) 
			return true;

		return (activityResponse.coord_start != null &&
		activityResponse.coord_mid != null &&
		activityResponse.coord_end != null);
	}

	public static bool RegisterQuizResponse(string response)
	{
		if (response.Equals (quiz.correct)) 
		{
			quizResponse.quiz_correct = "Acertou";
			return true;
		}

		quizResponse.quiz_correct = "Errou";
		return false;
	}
}

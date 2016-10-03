using System;
using System.IO;
using UnityEngine;
using System.Collections.Generic;

public static class AudioRec
{
	public static AudioSource audioSource;
	private static bool micConnected = false;
	private static int minFreq, maxFreq;

	public static void RecordAudio()
	{
		SavWav.instance.Init();

		if (Microphone.devices.Length <= 0)
			UnityAndroidExtras.instance.makeToast("Nenhum microfone encontrado", 1);
		else 
		{
			micConnected = true;
			Microphone.GetDeviceCaps(null, out minFreq, out maxFreq);

			if(minFreq == 0 && maxFreq == 0)
				maxFreq = 44100;
		}
		if (micConnected)
		{
			if (!Microphone.IsRecording(null))
			{
				UnityAndroidExtras.instance.makeToast("Clique novamente para parar a gravação", 1);
				audioSource.clip = Microphone.Start(null, true, 600, maxFreq);
			}
			else //Recording is in progress, then stop it.
			{
				var position = Microphone.GetPosition(null);

				Microphone.End(null); //Stop the audio recording

				var soundData = new float[audioSource.clip.samples * audioSource.clip.channels];
				audioSource.clip.GetData (soundData, 0);
				 
				//Create shortened array for the data that was used for recording
				var newData = new float[position * audioSource.clip.channels];
				 
				//Copy the used samples to a new array
				for (int i = 0; i < newData.Length; i++) 
				    newData[i] = soundData[i];

				//Creates a newClip with the correct length
				var newClip = AudioClip.Create ("voice",
				                                 position,
				                                 audioSource.clip.channels,
				                                 audioSource.clip.frequency,
				                                 true, false);
				 
				newClip.SetData (newData, 0); //Give it the data from the old clip
				 
			 	//Replace the old clip
				AudioClip.Destroy (audioSource.clip);
				audioSource.clip = newClip;  

				SavWav.instance.Save ("voice", audioSource.clip);
				UnityAndroidExtras.instance.makeToast("Voz gravada com sucesso", 1);
			}            
		}
	}

	public static void ListenAudio()
	{
		if (audioSource.clip == null)
			UnityAndroidExtras.instance.makeToast("Nenhum áudio gravado", 1);

		else if (isRecorded()) // If recorded 
		{
			audioSource.Play ();
			UnityAndroidExtras.instance.makeToast("Reproduzindo", 1);
		} 
	}

	public static bool isRecorded()
	{
		var filepath = Path.Combine(Application.persistentDataPath, "voice.wav");

		if (System.IO.File.Exists (filepath))
			return true;

		return false;
	}
}

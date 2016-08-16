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
		if(micConnected)
		{
			if(!Microphone.IsRecording(null))
			{
				UnityAndroidExtras.instance.makeToast("Clique novamente para parar a gravação", 1);
				audioSource.clip = Microphone.Start(null, true, 20, maxFreq);
			}
			else //Recording is in progress
			{
				Microphone.End(null); //Stop the audio recording

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

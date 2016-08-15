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
		if (Microphone.devices.Length <= 0)
			AndroidToast.ShowMessage ("Nenhum microfone encontrado");
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
				AndroidToast.ShowMessage("Clique novamente para parar a gravação");
				audioSource.clip = Microphone.Start(null, true, 20, maxFreq);
			}
			else //Recording is in progress
			{
				Microphone.End(null); //Stop the audio recording

				SavWav.Save ("voice", audioSource.clip);
				AndroidToast.ShowMessage("Voz gravada com sucesso");
			}            
		}
	}

	public static void ListenAudio()
	{
		if (audioSource.clip == null)
			AndroidToast.ShowMessage ("Nenhum áudio gravado");

		else if (isRecorded()) // If recorded 
		{
			audioSource.Play ();
			AndroidToast.ShowMessage ("Reproduzindo");
		} 
		else 
		{
			AndroidToast.ShowMessage ("Nenhum áudio gravado");
		}
	}

	public static bool isRecorded()
	{
		var filepath = Path.Combine(Application.dataPath, "voice.wav");

		if (System.IO.File.Exists (filepath))
			return true;

		return false;
	}
}

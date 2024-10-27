using System;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;



public static class WavUtility
{
    public static AudioClip ToAudioClip(byte[] fileBytes, string clipName = "wavClip")
    {

        string tempPath = Path.Combine(Application.temporaryCachePath, "temp.wav");
        File.WriteAllBytes(tempPath, fileBytes);
        return LoadClip(tempPath, clipName);
    }

    public static AudioClip LoadClip(string filepath, string clipName)
    {
       
        UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + filepath, AudioType.WAV);

        var operation = www.SendWebRequest();
        while (!operation.isDone) { }

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError($"Error loading AudioClip from path: {filepath}, Error: {www.error}");
            return null;
        }

        return DownloadHandlerAudioClip.GetContent(www);
    }
}

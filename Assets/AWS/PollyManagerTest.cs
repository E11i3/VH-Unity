using Amazon.Polly;
using Amazon.Polly.Model;
using Amazon.Runtime;
using System.IO;
using UnityEngine;
using System;
using System.Collections;

public class PollyManagerTest : MonoBehaviour
{
    private AmazonPollyClient pollyClient;
    public AudioSource audioSource;  
    public ChatHandlerTest chatHandlerTest;

    void Start()
    {
        try
        {
           
           var awsCredentials = new BasicAWSCredentials("", "");
            pollyClient = new AmazonPollyClient(awsCredentials, Amazon.RegionEndpoint.USEast1);
        
            TriggerPollySpeech("Would you like some general advice or type 1 to stretch?");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error initializing AWS Polly: {e.Message}");
        }
    }

    public void SynthesizeTextToSpeech(string customText)
    {
        try
        {
            if (customText.Length > 3000)
            {
                Debug.LogError("Text too long for Polly request (must be 3000 characters or fewer).");
                return;
            }
            
            string ssmlText = "<speak><prosody rate=\"slow\">" + customText + "</prosody></speak>";

            SynthesizeSpeechRequest synthReq = new SynthesizeSpeechRequest()
            {
                Text = ssmlText, 
                VoiceId = VoiceId.Joanna,  
                OutputFormat = OutputFormat.Pcm,  
                TextType = TextType.Ssml 
            };

            SynthesizeSpeechResponse synthRes = pollyClient.SynthesizeSpeechAsync(synthReq).Result;

            using (MemoryStream memoryStream = new MemoryStream())
            {
                synthRes.AudioStream.CopyTo(memoryStream);
                byte[] audioData = memoryStream.ToArray();
                PlayAudio(audioData);
            }
        }
        catch (AmazonPollyException e)
        {
            Debug.LogError($"Error making Polly request: {e.Message}");
        }
        catch (Exception e)
        {
            Debug.LogError($"General error occurred: {e.Message}");
        }
    }

    private void PlayAudio(byte[] audioData)
    {
        int sampleRate = 22050;  
        int channels = 1;  

       
        float[] floatArray = new float[audioData.Length / 2]; 
        for (int i = 0; i < floatArray.Length; i++)
        {
            short sample = BitConverter.ToInt16(audioData, i * 2);  
            floatArray[i] = sample / 32768.0f;  
        }

        
        AudioClip clip = AudioClip.Create("PollyAudioClip", floatArray.Length, channels, sampleRate, false);
        clip.SetData(floatArray, 0);

      
        audioSource.clip = clip;
        audioSource.Play();
    }

    
    public void TriggerPollySpeech(string customText)
    {
        
        SynthesizeTextToSpeech(customText);
    }

        public void TriggerPollySpeechFromChatHandler()
    {
        if (chatHandlerTest != null)
        {
            string customText = chatHandlerTest.GetLatestChatResponse();
            SynthesizeTextToSpeech(customText);
        }
        else
        {
            Debug.LogError("ChatHandler is not assigned.");
        }
    }
}

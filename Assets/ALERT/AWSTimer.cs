using Amazon.Polly;
using Amazon.Polly.Model;
using Amazon.Runtime;
using System.IO;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using Newtonsoft.Json.Linq;

public class AWSTimer : MonoBehaviour
{
    public PollyManager pollyManager; 
    public ChatHandlerTest chatHandlerTest;

        public float interval = 47f; 
        private float timer;
         public float delay = 240f;
        private bool toDelay;
        public Button submitButton;

    void Start()
    {
        timer = interval;

         if (submitButton !=null){
           submitButton.onClick.AddListener(ResetTimer);

        }
    }

    private void ResetTimer()
    {
        StartCoroutine(DelayCoroutine());
    }

    private IEnumerator DelayCoroutine()
    {
        toDelay = true;
        yield return new WaitForSeconds(delay); 
        toDelay = false;
        timer = interval; 
    }

     void Update()
    {
       

        if(!toDelay){
            timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            if (pollyManager != null)
            {
                
                pollyManager.SynthesizeTextToSpeech("You got this!");
            }
            timer = interval; 
        }

        }
    
        
    }
}

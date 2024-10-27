using Amazon.Polly;
using Amazon.Polly.Model;
using Amazon.Runtime;
using System.IO;
using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class AnimationSwitch : MonoBehaviour
{
    public Animator animator; 
    public TMP_InputField inputField; 
    public Button submitButton; 

    void Start()
    {
        submitButton.onClick.AddListener(OnSubmitButtonClicked);
    }

    private void OnSubmitButtonClicked()
    {
        int animationIndex;

        if (int.TryParse(inputField.text, out animationIndex))
        {
            StartCoroutine(TriggerAnimation(animationIndex,15f)); 
            inputField.text = ""; 
        }
    }


    private IEnumerator TriggerAnimation(int index, float delay)
    {
        yield return new WaitForSeconds(delay);
        
        switch (index)
        {
            case 1:
                animator.SetTrigger("Stretching");
                break;
            case 2:
                animator.SetTrigger("Joke");
                break;
        }
    }
}

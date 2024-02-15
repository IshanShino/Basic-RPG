using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RPG.SceneManagement
{
    public class Fader : MonoBehaviour
    {   
        CanvasGroup canvasGroup;
        Coroutine currentActiveFade = null;
        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        public void FadeOutImmediate()
        {
            canvasGroup.alpha = 1;
        }

        // public IEnumerator FadeOutIn()
        // {   
        //     yield return FadeOut(2f);
        //     print("Faded Out.");
        //     yield return FadeIn(1f);
        //     print("Faded In.");
        // }
        public Coroutine FadeOut(float time)
        {       
            return Fade(1, time);
        }
        public Coroutine FadeIn(float time)
        {
           return Fade(0, time);
        }

        private Coroutine Fade(float target, float time)
        {   
            if (currentActiveFade != null)   // cancel current coroutine 
            {
                StopCoroutine(currentActiveFade);  
            }
                 
            currentActiveFade = StartCoroutine(FadeRoutine(target, time));  // run fadeout coroutine
            
            return currentActiveFade;
        }

        private IEnumerator FadeRoutine(float target, float time)
        {
            while(!Mathf.Approximately(canvasGroup.alpha, target)) 
            {   
                canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, target, Time.deltaTime / time);
                yield return null;
            }
        }

    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.JsonSaving;

namespace RPG.SceneManagement
{
    public class SavingWrapper : MonoBehaviour
    {   
        const string defaultSaveFile = "save";

        private void Awake()
        {
            StartCoroutine(LoadLastScene());
        }
        IEnumerator LoadLastScene()
        {   
            //yield return GetComponent<JsonSavingSystem>().LoadLastScene(defaultSaveFile);
            Fader fader = FindObjectOfType<Fader>();
            fader.FadeOutImmediate();  
            yield return GetComponent<JsonSavingSystem>().LoadLastScene(defaultSaveFile);
            yield return fader.FadeIn(2f);
        }
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                Load();
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                Save();
            }
        }

        public void Save()
        {
            GetComponent<JsonSavingSystem>().Save(defaultSaveFile);
        }

        public void Load()
        {
            GetComponent<JsonSavingSystem>().Load(defaultSaveFile);
        }
    }

}

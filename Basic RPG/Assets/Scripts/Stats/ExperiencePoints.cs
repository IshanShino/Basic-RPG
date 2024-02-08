using System;
using Newtonsoft.Json.Linq;
using RPG.JsonSaving;
using UnityEngine;

namespace RPG.Stats
{
    public class ExperiencePoints : MonoBehaviour, IJsonSaveable
    {
        [SerializeField] float expPoints = 0f;

        public event Action onExperienceChanged;
        public void GainExp(float expGained)
        {
            expPoints = expGained + expPoints;
            onExperienceChanged();
        }
        public void UpdateExpAfterLevelUp(float xPUsed)
        {
            expPoints -= xPUsed;
        }

        public float GetExpPoints()
        {
            return expPoints;
        }

        public JToken CaptureAsJToken()
        {
            return JToken.FromObject(expPoints);
        }
        public void RestoreFromJToken(JToken state)
        {
            expPoints = state.ToObject<float>();
            onExperienceChanged();
        }
    }    
}


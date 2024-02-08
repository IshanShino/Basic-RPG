using System;
using Newtonsoft.Json.Linq;
using RPG.JsonSaving;
using RPG.Utils;
using TMPro;
using UnityEngine;

namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour, IJsonSaveable
    {   
        [Range(1, 50)]
        [SerializeField] int startingLevel = 1;
        LazyValue<int> currentLevel;
        [SerializeField] CharacterClass characterClass;
        [SerializeField] ProgressionStats progressionStats = null;
        [SerializeField] GameObject levelUpEffect = null;

        public event Action onLevelUp;
        ExperiencePoints experiencePoints;

        void Awake() 
        {
            experiencePoints = GetComponent<ExperiencePoints>();
            currentLevel = new LazyValue<int>(CalculateLevel);
        }
        void Start()
        {   
            currentLevel.ForceInit();
        }

        void OnEnable() 
        {
            if (experiencePoints != null) 
            {
                experiencePoints.onExperienceChanged += UpdateLevel;
            }
        }

        void OnDisable()
        {
            if (experiencePoints != null) 
            {
                experiencePoints.onExperienceChanged -= UpdateLevel;
            }
        }
        void UpdateLevel()
        {         
            int newLevel = CalculateLevel();
            if (newLevel > currentLevel.value)
            {   
                currentLevel.value = newLevel;
                Instantiate(levelUpEffect, transform);
                onLevelUp();
            }        
        }
        public float GetStat(Stat stat)
        {
            return Mathf.RoundToInt(GetBaseStat(stat) + GetAdditiveModifier(stat) * (1 + GetPercentageModifier(stat)/100));
        }

        private float GetBaseStat(Stat stat)
        {
            return progressionStats.GetStat(stat, characterClass, GetLevel());
        }

        private float GetAdditiveModifier(Stat stat)
        {   
            float total = 0;
            foreach(IModifierProvider modifierProvider in GetComponents<IModifierProvider>())
            {
                foreach(float modifier in modifierProvider.GetAdditiveModifiers(stat))
                {
                    total += modifier;
                }
            }
            return total;
        }

        private float GetPercentageModifier(Stat stat)
        {
            float percentageTotal = 0;
            foreach(IModifierProvider modifierProvider in GetComponents<IModifierProvider>())
            {
                foreach(float modifier in modifierProvider.GetPercentageModifiers(stat))
                {
                    percentageTotal += modifier;
                }
            }
            return percentageTotal;
        }

        public int GetLevel()
        {
            if (currentLevel.value < 1)
            {
                currentLevel.value = CalculateLevel();
            }
            return currentLevel.value;
        }

        private int CalculateLevel()
        {       

            if (experiencePoints == null) return startingLevel;
            float currentXP = experiencePoints.GetExpPoints();
            int maxLevel = progressionStats.GetMaxLevel(characterClass, Stat.ExpRequiredToLevelUp);
            for (int level = 1; level <= maxLevel; level++)
            {
                float xPToLevelUp = progressionStats.GetStat(Stat.ExpRequiredToLevelUp, characterClass, level + 1);
                if (xPToLevelUp > currentXP)
                {   
                    return level;
                }  
            }
            //experiencePoints.UpdateExpAfterLevelUp(xPToLevelUp); (only cumulative exp for now :(
            // try to find a solution for non-cumulative approach later
            return maxLevel;
        }
        //     if (experiencePoints == null) return startingLevel;
        //     float currentXP = experiencePoints.GetExpPoints();
        //     int maxLevel = progressionStats.GetMaxLevel(characterClass, Stat.ExpRequiredToLevelUp);
        //     int level = startingLevel;
        //     float xPToLevelUp = progressionStats.GetStat(Stat.ExpRequiredToLevelUp, characterClass, level + 1);
        //     if (level <= maxLevel)
        //     {   
        //         if (currentXP >= xPToLevelUp)
        //         {
        //             level++;
        //             experiencePoints.UpdateExpAfterLevelUp(xPToLevelUp);
        //             return level;
        //         }
        //     }
        //     return level;
        // }

        public JToken CaptureAsJToken()
        {
            return JToken.FromObject(currentLevel);
        }
        public void RestoreFromJToken(JToken state)
        {
            currentLevel.value = state.ToObject<int>();
        }
    } 
}


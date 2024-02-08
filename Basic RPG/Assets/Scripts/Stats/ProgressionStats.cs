using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Stats
{
    [CreateAssetMenu(fileName = "ProgressionStats", menuName = "Stats/ProgressionStats", order = 0)]
    public class ProgressionStats : ScriptableObject 
    {   
        [SerializeField] ProgressionCharacterClass[] progressionCharacterClasses = null;
        Dictionary<CharacterClass, Dictionary<Stat, float[]>> lookupTable = null;

        [Serializable]
        class ProgressionCharacterClass
        {   
            public CharacterClass characterClass;   
            public ProgressionStat[] progressionStats;

            public CharacterClass GetCC()
            {
                return characterClass;
            }
        }

        [Serializable]
        class ProgressionStat
        {
            public Stat stats;
            public float[] levels;
        }

        public float GetStat(Stat stat, CharacterClass characterClass, int level)
        {   
            BuildLookup();
            
            if (!lookupTable.ContainsKey(characterClass)) return 0;

            Dictionary<Stat, float[]> statDict =  lookupTable[characterClass];

            if (!statDict.ContainsKey(stat)) return 0;

            float[] levels = statDict[stat];

            if (levels.Length < level) return 0;

            return levels[level - 1];
        }

        public int GetMaxLevel(CharacterClass characterClass, Stat stat)
        {
            BuildLookup();
            float[] levels = lookupTable[characterClass][stat];
            return levels.Length;
        }

        private void BuildLookup()
        {
            if (lookupTable != null) return;

            lookupTable = new Dictionary<CharacterClass, Dictionary<Stat, float[]>>();

            foreach(ProgressionCharacterClass pCC in progressionCharacterClasses)
            {
                var statLookupTable =  new Dictionary<Stat, float[]>();

                foreach (ProgressionStat progressionStat in pCC.progressionStats)
                {
                    statLookupTable[progressionStat.stats] = progressionStat.levels;
                }

                lookupTable[pCC.GetCC()] = statLookupTable;
            }
        }
    }
}

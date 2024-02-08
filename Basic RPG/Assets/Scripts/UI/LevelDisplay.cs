using RPG.Stats;
using TMPro;
using UnityEngine;

namespace RPG.UI
{
    public class LevelDisplay : MonoBehaviour
    {   
        BaseStats baseStats;
        void Awake()
        {
            baseStats = GameObject.FindGameObjectWithTag("Player").GetComponent<BaseStats>();
        } 

        void Update()
        {
            GetComponent<TextMeshProUGUI>().SetText($"Level: {baseStats.GetLevel()}");
        }
    }

}

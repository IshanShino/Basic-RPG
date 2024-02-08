using TMPro;
using UnityEngine;
using RPG.Stats;

namespace RPG.UI
{
    public class XPDisplay : MonoBehaviour
    {   
        ExperiencePoints XP;   
        void Awake()
        {
            XP =  GameObject.FindGameObjectWithTag("Player").GetComponent<ExperiencePoints>();
        }

        void Update()
        {
            GetComponent<TextMeshProUGUI>().SetText($"XP: {XP.GetExpPoints()}");
        }
    }

}

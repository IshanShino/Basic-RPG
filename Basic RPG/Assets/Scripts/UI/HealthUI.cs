using RPG.Attributes;
using TMPro;
using UnityEngine;

namespace RPG.UI
{
    public class HealthUI : MonoBehaviour
    {
        Health health;

        void Awake() 
        {
            health = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
        }

        void Update() 
        {   
            GetComponent<TextMeshProUGUI>().SetText($"Health:  {health.HealthPoints}/{health.GetMaxHP()}");
        } 
    }
}

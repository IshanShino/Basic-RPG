using TMPro;
using UnityEngine;
using RPG.Attributes;
using RPG.Combat;

namespace RPG.UI
{
    public class EnemyHealthUI : MonoBehaviour
    {
        Fighter player;
        Health target;

        void Awake() 
        {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<Fighter>();
        }

        private void Update()
        {
            target = player.GetTarget();
            DisplayHealth();
        }

        void DisplayHealth()
        {
            if (target == null || target.IsDead())
            {
                GetComponent<TextMeshProUGUI>().SetText($"Enemy:  N/A");
                return;
            }
            else
            {   
                GetComponent<TextMeshProUGUI>().SetText($"Enemy:  {target.HealthPoints}/{target.GetMaxHP()}");
            }
        }
    }
}

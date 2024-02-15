using TMPro;
using UnityEngine;

namespace RPG.UI
{
    public class DamageSpawner : MonoBehaviour 
    {
        [SerializeField] DamageUI damageUIPrefab = null;
        public void Spawn(float damage)
        {   
            DamageUI instance = Instantiate(damageUIPrefab, transform);
            instance.GetComponentInChildren<TextMeshProUGUI>().SetText($"{damage}");
        }
    }
}
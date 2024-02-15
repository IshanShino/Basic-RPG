using UnityEngine;

namespace RPG.UI
{
    public class DamageUI : MonoBehaviour
    {   
        void Start()
        {
            DestroyText();
        }
    
        public void DestroyText()
        {
            float animationLength = GetComponentInChildren<Animation>().clip.length;
            Destroy(gameObject, animationLength);
        }
    }

}
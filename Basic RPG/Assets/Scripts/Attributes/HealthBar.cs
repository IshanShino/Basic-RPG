using UnityEngine;

namespace RPG.Attributes
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] RectTransform foreground = null;
        [SerializeField] Health healthComponent = null;
        [SerializeField] Canvas rootCanvas = null;
        
        void Start()
        {
            rootCanvas.enabled = false;
        }
        public void HandleHealthChange(float value)
        {   
            rootCanvas.enabled = true;

            if (Mathf.Approximately(healthComponent.GetFraction(), 0f) || Mathf.Approximately(healthComponent.GetFraction(), 1f))
            { 
                rootCanvas.enabled = false;
            }

            foreground.localScale = new Vector3(healthComponent.GetFraction(), 1, 1);
        }
    }
}

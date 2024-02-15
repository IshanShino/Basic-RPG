using UnityEngine;

namespace RPG.UI
{
    public class CameraFacing : MonoBehaviour 
    {
        void LateUpdate() 
        {
            transform.forward = Camera.main.transform.forward;
        }
    }
}

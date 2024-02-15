using UnityEngine;
using RPG.Attributes;
using RPG.Core;
using RPG.Control;

namespace RPG.Combat
{   
    [RequireComponent(typeof(Health))]
    public class CombatTarget : MonoBehaviour, IRaycastable
    {   
        
        public bool HandleRaycast(PlayerController controller)
        {
            if (!controller.GetComponent<Fighter>().CanAttack(gameObject)) 
            {
                return false; 
            }           
            if (Input.GetMouseButton(0))
            {
                controller.GetComponent<Fighter>().Attack(gameObject);
            }
            return true;         
        }

        public CursorType GetCursorType(PlayerController controller)
        {   
            return controller.CombatCursor;
        }
    }
}
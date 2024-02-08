using UnityEngine;
using RPG.Movement;
using RPG.Combat;
using RPG.Attributes;
using UnityEngine.EventSystems;

namespace RPG.Control
{   
    public class PlayerController : MonoBehaviour
    {   
        [SerializeField] CursorType movementCursor;
        [SerializeField] CursorType combatCursor;
        [SerializeField] CursorType noneCursor;
        [SerializeField] CursorType uICursor;
        void Update()
        {   
            if(InteractWithUI()) return;
        
            if (GetComponent<Health>().IsDead()) 
            {   
                noneCursor.SetCursor();
                return;
            }

            if (InteractWithCombat()) return;
            if (InteractWithMovement()) return;

            noneCursor.SetCursor();
        }

        private bool InteractWithUI()
        {
            if (EventSystem.current.IsPointerOverGameObject()) // checks if the pointer is over an UI game object. bool return.
            {
                uICursor.SetCursor();
                return true;
            }
            return false;
        }

        private bool InteractWithCombat()
        {   
            RaycastHit[] hits  = Physics.RaycastAll(GetMouseRay());
            foreach (RaycastHit hit in hits)
            {   
                CombatTarget target = hit.transform.GetComponent<CombatTarget>();

                if (target == null) continue;

                if (!GetComponent<Fighter>().CanAttack(target.gameObject)) 
                {
                    continue; // Here continue means to continue the loop, which means to check the next thing in the loop.
                // So basically it means that if we *can't attack* a target, just move along the loop and check the next target.
                }
            
                if (Input.GetMouseButton(0))
                {
                    GetComponent<Fighter>().Attack(target.gameObject);
                }
                combatCursor.SetCursor();
                return true;
            }
            return false;
        }

        private bool InteractWithMovement()
        {          
            RaycastHit hit;
            bool hasHit = Physics.Raycast(GetMouseRay(), out hit);

            if (hasHit == true)
            {
                if (Input.GetMouseButton(0))
                {
                    GetComponent<Mover>().StartMoveAction(hit.point, 1f);
                }
                movementCursor.SetCursor();
                return true;
            }
            return false;
        }

        private static Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using RPG.Movement;
using RPG.Combat;
using System;

namespace RPG.Control
{
    public class PlayerController : MonoBehaviour
    {   
        void Update()
        {   
            if (InteractWithCombat()) return;
            if (InteractWithMovement()) return;
            print("Nothing to do.");
        }

        private bool InteractWithCombat()
        {
            RaycastHit[] hits  = Physics.RaycastAll(GetMouseRay());
            foreach (RaycastHit hit in hits)
            {   
                CombatTarget target = hit.transform.GetComponent<CombatTarget>();
                
                if (!GetComponent<Fighter>().CanAttack(target)) 
                {
                    continue; // Here continue means to continue the loop, which means to check the next thing in the loop.
                // So basically it means that if we *can't attack* a target, just move along the loop and check the next target.
                }
            
                if (Input.GetMouseButtonDown(0))
                {
                    GetComponent<Fighter>().Attack(target);
                }
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
                    GetComponent<Mover>().StartMoveAction(hit.point);
                }
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

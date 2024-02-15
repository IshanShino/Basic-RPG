using UnityEngine;
using RPG.Movement;
using RPG.Attributes;
using UnityEngine.EventSystems;
using RPG.Core;
using System;
using UnityEngine.AI;

namespace RPG.Control
{   
    public class PlayerController : MonoBehaviour
    {   
        [SerializeField] CursorType movementCursor;
        [SerializeField] CursorType noneCursor;
        [SerializeField] CursorType uICursor;
        [SerializeField] CursorType combatCursor;
        public CursorType CombatCursor { get {return combatCursor; } }
        [SerializeField] CursorType pickupCursor;
        public CursorType PickupCursor { get {return pickupCursor; } }

        [SerializeField] float sphereCastRadius = 0.7f;

        void Update()
        {   
            if(InteractWithUI()) return;
        
            if (GetComponent<Health>().IsDead()) 
            {   
                GetCursor(noneCursor);
                return;
            }
            
            if (InteractWithComponents()) return;
            if (InteractWithMovement()) return;

            GetCursor(noneCursor);
        }

        private bool InteractWithUI()
        {
            if (EventSystem.current.IsPointerOverGameObject()) // checks if the pointer is over an UI game object. bool return.
            {
                GetCursor(uICursor);
                return true;
            }
            return false;
        }
        private bool InteractWithComponents()
        {   
            RaycastHit[] hits  = RaycastAllSorted();
            foreach (RaycastHit hit in hits)
            {
                IRaycastable[] raycastables = hit.transform.GetComponents<IRaycastable>();

                foreach (IRaycastable raycastable in raycastables)
                {
                    if (raycastable.HandleRaycast(this))
                    {   
                        GetCursor(raycastable.GetCursorType(this));
                        return true;
                    }
                }
            }
            return false;
        }

        private RaycastHit[] RaycastAllSorted()
        {
            RaycastHit[] hits = Physics.SphereCastAll(GetMouseRay(), sphereCastRadius);    // Get all hits
            float[] distances = new float[hits.Length]; // build distance array 
            for (int i = 0; i < hits.Length; i++) 
            {   
                distances[i] = hits[i].distance;  //The distance from the ray's origin to the impact point.
            }
            Array.Sort(distances, hits);                // sort the hits based on the distances
            return hits;
        }

        private bool InteractWithMovement()
        {          
            Vector3 target;
            bool hasHit = RaycastNavmesh(out target);
            if (hasHit)
            {   
                if (!GetComponent<Mover>().CanMoveTo(target)) return false;

                if (Input.GetMouseButton(0))
                {
                    GetComponent<Mover>().StartMoveAction(target, 1f);
                }
                GetCursor(movementCursor);
                return true;
            }
            return false;
        }

        private bool RaycastNavmesh(out Vector3 target)
        {   
            target = new Vector3();

            RaycastHit hit;
            bool hasHit = Physics.Raycast(GetMouseRay(), out hit);
            
            if (!hasHit) return false;       

            NavMeshHit navMeshHit;
            bool hasCastToNavmesh = NavMesh.SamplePosition(hit.point, out navMeshHit, 1f, NavMesh.AllAreas);

            if (!hasCastToNavmesh) return false;

            target = navMeshHit.position;  

            return true;
        }

        public void GetCursor(CursorType cursorType)
        {
            cursorType.SetCursor();
        }
        private static Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
    }
}

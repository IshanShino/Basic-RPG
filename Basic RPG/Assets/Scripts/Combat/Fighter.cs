using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using RPG.Movement;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour
    {   
        [SerializeField] float WeaponRange = 2f;
        Transform target;
        Mover mover;
        void Start()
        {
            mover = GetComponent<Mover>();   
        }
        void Update()
        {   
            bool isInRange = Vector3.Distance(transform.position, target.position) < WeaponRange;
            if (target != null && !isInRange)
            {   
                mover.MoveTo(target.position);
            }
            else
            {
                mover.Stop();
            }
        }
        public void Attack(CombatTarget combatTarget)
        {
            target = combatTarget.transform;
        }
        
    }

}
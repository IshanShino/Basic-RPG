using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using RPG.Movement;
using RPG.Core;
using Unity.VisualScripting;
using RPG.JsonSaving;
using Newtonsoft.Json.Linq;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction, IJsonSaveable
    {        
        [SerializeField] float timeBetweenAttacks = 1.5f;
        [SerializeField] Transform rightHand = null;
        [SerializeField] Transform leftHand = null;
        [SerializeField] Weapon defaultWeapon = null;
        Health target;
        float timeSinceLastAttack = Mathf.Infinity;
        Mover mover;
        Weapon currentWeapon = null;
        GameObject equippedWeapon = null;

        void Awake()
        {
            if (currentWeapon ==  null)
            {
                EquipWeapon(defaultWeapon);
            }  
        } 
        void Start()
        {              
            mover = GetComponent<Mover>();   
        }
        void Update()
        {   
            timeSinceLastAttack += Time.deltaTime;
            if (target == null) return;
            if (target.IsDead()) return;

            if (!GetIsInRange())
            {
                mover.MoveTo(target.transform.position, 1f);
            }
            else
            {   
                mover.Cancel();
                AttackBehaviour();
            }
        }
        public bool CanAttack(GameObject combatTarget)
        {   
            if (combatTarget == null) { return false; }

            Health targetToTest =  combatTarget.GetComponent<Health>();
            return targetToTest != null && !targetToTest.IsDead();
        }
        public void Attack(GameObject combatTarget)
        {   
            GetComponent<ActionScheduler>().StartAction(this);
            target = combatTarget.GetComponent<Health>();
        }

        public void AttackBehaviour()
        {   
            transform.LookAt(target.transform);
            if (timeSinceLastAttack > timeBetweenAttacks)
            {
                // This will trigger the Hit() event.
                TriggerAttack();
                timeSinceLastAttack = 0f;
            }
        }

        private void TriggerAttack()
        {
            GetComponent<Animator>().ResetTrigger("stopAttack");
            GetComponent<Animator>().SetTrigger("attack");
        }

        // Animation Event
        void Hit()
        {   
            if (target ==  null) return;
            if (currentWeapon.HasProjectile())
            {
                currentWeapon.LaunchProjectile(rightHand, leftHand, target);
            }
            else
            {
                target.TakeDamage(currentWeapon.WeaponDamage);
            }
        }

        // Animation Event
        void Shoot()
        {
            Hit();
        }

        private bool GetIsInRange()
        {
            return Vector3.Distance(transform.position, target.transform.position) < currentWeapon.WeaponRange;
        }
        public void Cancel()
        {
            StopAttack();
            target = null;
            mover.Cancel();
        }

        private void StopAttack()
        {
            GetComponent<Animator>().ResetTrigger("attack");
            GetComponent<Animator>().SetTrigger("stopAttack");
        }

        public void EquipWeapon(Weapon weapon)
        {   
            if (weapon != null)
            {   
                currentWeapon = weapon;
                if (equippedWeapon != null)
                {
                    Destroy(equippedWeapon);
                }
                Animator animator = GetComponent<Animator>();
                equippedWeapon = weapon.Spawn(rightHand, leftHand, animator);
            }
        }

        public JToken CaptureAsJToken()
        {
            return JToken.FromObject(currentWeapon.name);
        }

        public void RestoreFromJToken(JToken state)
        {
            string weaponName = state.ToObject<string>();
            Weapon weapon = Resources.Load<Weapon>(weaponName);
            EquipWeapon(weapon);
        }
    }
}
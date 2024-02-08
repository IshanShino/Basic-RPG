using UnityEngine;
using RPG.Movement;
using RPG.Core;
using RPG.JsonSaving;
using Newtonsoft.Json.Linq;
using RPG.Attributes;
using RPG.Stats;
using System.Collections.Generic;
using RPG.Utils;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction, IJsonSaveable, IModifierProvider
    {        
        [SerializeField] float timeBetweenAttacks = 1.5f;
        [SerializeField] Transform rightHand = null;
        [SerializeField] Transform leftHand = null;
        [SerializeField] Weapon defaultWeapon = null;
        Health target;
        float timeSinceLastAttack = Mathf.Infinity;
        Mover mover;
        LazyValue<Weapon> currentWeapon;
        GameObject equippedWeapon = null;

        void Awake()
        {   
            mover = GetComponent<Mover>();  
            currentWeapon = new LazyValue<Weapon>(SetDefaultWeapon);
           
        } 
        void Start()
        {              
            currentWeapon.ForceInit();
        }

        Weapon SetDefaultWeapon()
        {
            AttachWeapon(defaultWeapon);
            return defaultWeapon;
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

            float damage = GetComponent<BaseStats>().GetStat(Stat.Damage);
            if (currentWeapon.value.HasProjectile())
            {
                currentWeapon.value.LaunchProjectile(rightHand, leftHand, target, damage);
            }
            else
            {
                target.TakeDamage(damage);
            }
        }

        // Animation Event
        void Shoot()
        {
            Hit();
        }

        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            if(stat == Stat.Damage)
            {
                yield return currentWeapon.value.WeaponDamage;
            }
        }
        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            if(stat == Stat.Damage)
            {
                yield return currentWeapon.value.PercentageBonus;
            }
        }

        private bool GetIsInRange()
        {
            return Vector3.Distance(transform.position, target.transform.position) < currentWeapon.value.WeaponRange;
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
                currentWeapon.value = weapon;
                if (equippedWeapon != null)
                {
                    Destroy(equippedWeapon);
                }
                AttachWeapon(weapon);
            }
        }

        private void AttachWeapon(Weapon weapon)
        {
            Animator animator = GetComponent<Animator>();
            equippedWeapon = weapon.Spawn(rightHand, leftHand, animator);
        }

        public Health GetTarget()
        {
            return target;
        }

        public JToken CaptureAsJToken()
        {
            return JToken.FromObject(currentWeapon.value.name);
        }

        public void RestoreFromJToken(JToken state)
        {
            string weaponName = state.ToObject<string>();
            Weapon weapon = Resources.Load<Weapon>(weaponName);
            EquipWeapon(weapon);
        }
    }
}
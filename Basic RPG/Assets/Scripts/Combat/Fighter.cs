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
        [SerializeField] WeaponConfig defaultWeapon = null;
        Health target;
        float timeSinceLastAttack = Mathf.Infinity;
        Mover mover;
        WeaponConfig currentWeaponConfig;
        Weapon equippedWeapon = null;
        LazyValue<Weapon> currentWeapon;

        void Awake()
        {   
            mover = GetComponent<Mover>();  
            currentWeaponConfig = defaultWeapon;
            currentWeapon = new LazyValue<Weapon>(SetDefaultWeapon);
           
        } 
        void Start()
        {              
            currentWeapon.ForceInit();
        }

        private Weapon SetDefaultWeapon()
        {
            return AttachWeapon(defaultWeapon);
        }

        void Update()
        {   
            timeSinceLastAttack += Time.deltaTime;
            if (target == null) return;
            if (target.IsDead()) return;

            if (!GetIsInRange(target.transform))
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

            if (!GetComponent<Mover>().CanMoveTo(combatTarget.transform.position) && !GetIsInRange(combatTarget.transform)) 
            { 
                return false; 
            } 

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

            if (currentWeapon.value != null)
            {   
                currentWeapon.value.OnHit();
            }

            float damage = GetComponent<BaseStats>().GetStat(Stat.Damage);
            if (currentWeaponConfig.HasProjectile())
            {
                currentWeaponConfig.LaunchProjectile(rightHand, leftHand, target, damage);
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
                yield return currentWeaponConfig.WeaponDamage;
            }
        }
        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            if(stat == Stat.Damage)
            {
                yield return currentWeaponConfig.PercentageBonus;
            }
        }

        private bool GetIsInRange(Transform targetTransform)
        {
            return Vector3.Distance(transform.position, targetTransform.position) < currentWeaponConfig.WeaponRange;
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

        public void EquipWeapon(WeaponConfig weapon)
        {   
            if (weapon != null)
            {
                currentWeaponConfig = weapon;
                if (equippedWeapon != null)
                {
                    Destroy(equippedWeapon.gameObject);
                }
                currentWeapon.value = AttachWeapon(weapon);
            }
        }

        private Weapon AttachWeapon(WeaponConfig weapon)
        {   
            Animator animator = GetComponent<Animator>();
            equippedWeapon = weapon.Spawn(rightHand, leftHand, animator);
            return equippedWeapon;
        }

        public Health GetTarget()
        {
            return target;
        }

        public JToken CaptureAsJToken()
        {
            return JToken.FromObject(currentWeaponConfig.name);
        }

        public void RestoreFromJToken(JToken state)
        {
            string weaponName = state.ToObject<string>();
            WeaponConfig weapon = Resources.Load<WeaponConfig>(weaponName);
            EquipWeapon(weapon);
        }
    }
}
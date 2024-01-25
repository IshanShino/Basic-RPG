using UnityEngine;
using RPG.JsonSaving;
using Unity.VisualScripting;
using System;
using Newtonsoft.Json.Linq;

namespace RPG.Core
{
    public class Health : MonoBehaviour, IJsonSaveable
    {
        [SerializeField] float healthPoints = 100f;
        public float HealthPoints { get {return healthPoints; } }
        bool isDead = false;      

        Animator animator;

        void Awake()
        {
            animator = GetComponent<Animator>();
        }
        public bool IsDead()
        {
            return isDead;
        }
        
        public void TakeDamage(float damage)
        {   
            healthPoints = Mathf.Max(healthPoints - damage, 0);
            print(healthPoints);
            if (healthPoints == 0)
            {
                Die();
            }
        }

        private void Die()
        {   
            if (isDead) return;

            isDead = true;
            GetComponent<Animator>().SetTrigger("die");
            GetComponent<ActionScheduler>().CancelCurrentAction();
            GetComponent<Collider>().enabled = false;
        }

        //private void Ressurect()
        //{
            //if (!isDead) return;
            //isDead = false;
            //animator.enabled = false;
            //animator.enabled = true; // resetting the animator
        //}

        public JToken CaptureAsJToken()
        {
            return JToken.FromObject(healthPoints);
        }

        public void RestoreFromJToken(JToken state)
        {
            healthPoints = state.ToObject<float>();
            if (healthPoints == 0)
            {
                Die();
            }
            else
            {   
                isDead = false;
                animator.Rebind();
            }
        }
    }
}

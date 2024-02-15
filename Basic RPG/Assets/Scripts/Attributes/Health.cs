using UnityEngine;
using RPG.JsonSaving;
using Newtonsoft.Json.Linq;
using RPG.Core;
using RPG.Stats;
using RPG.Utils;
using UnityEngine.Events;

namespace RPG.Attributes
{
    public class Health : MonoBehaviour, IJsonSaveable
    {
        LazyValue<float> healthPoints;
        public float HealthPoints { get {return healthPoints.value; } }
        [SerializeField] float regenPercentage = 80f;
        [SerializeField] UnityEvent<float> takeDamage;
        [SerializeField] UnityEvent onDie;
        bool isDead = false;      

        BaseStats baseStats;
        Animator animator;

        void Awake()
        {   
            baseStats = GetComponent<BaseStats>();
            animator = GetComponent<Animator>();          
            healthPoints = new LazyValue<float>(GetInitialHealth);
        }
        void Start()
        {   
            healthPoints.ForceInit(); //force initialization if it hasn't initialised already. 
        }
        void OnEnable() 
        {
            if (baseStats != null)
            {
                baseStats.onLevelUp += RegenHealth;
            } 
        }

        void OnDisable()
        {
            if (baseStats != null)
            {
                baseStats.onLevelUp -= RegenHealth;
            } 
        }
        float GetInitialHealth()
        {
            return baseStats.GetStat(Stat.Health);
        }
        public bool IsDead()
        {
            return isDead;
        }
        
        public void TakeDamage(float damage)
        {   
            healthPoints.value = Mathf.Max(healthPoints.value - damage, 0); 

            if (healthPoints.value == 0)
            {   
                onDie.Invoke();
                Die();
                AwardExpPoints();
            }
            else
            {
                takeDamage.Invoke(damage);
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

        public float GetFraction()
        {
            return healthPoints.value/GetMaxHP();
        }
        public float GetMaxHP()
        {
            return baseStats.GetStat(Stat.Health);
        }
        void RegenHealth()
        {
            float regenHealthPoints = baseStats.GetStat(Stat.Health) * (regenPercentage / 100);
            healthPoints.value = Mathf.Max(healthPoints.value, regenHealthPoints);
        }
        void AwardExpPoints()
        {
            ExperiencePoints experiencePoints = GameObject.FindGameObjectWithTag("Player").GetComponent<ExperiencePoints>();

            if (experiencePoints ==  null) return;
            experiencePoints.GainExp(GetComponent<BaseStats>().GetStat(Stat.ExperienceRewards));
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
            return JToken.FromObject(healthPoints.value);
        }

        public void RestoreFromJToken(JToken state)
        {
            healthPoints.value = state.ToObject<float>();
            if (healthPoints.value == 0)
            {
                Die();
            }
            else
            {   
                isDead = false;
                GetComponent<Collider>().enabled = true;
                animator.Rebind();
            }
        }
    }
}

using RPG.Attributes;
using UnityEngine;

namespace RPG.Combat
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] float projectileSpeed = 5f;
        [SerializeField] bool isHoming = false;
        [SerializeField] GameObject hitEffect = null;
        [SerializeField] GameObject[] destroyAfterImpact;
        [SerializeField] float lifeAfterImpact = 1f;
        Health target = null;
        float damage = 0f;

        void Start()
        {   
            transform.LookAt(GetAimLocation());          
        }
        void Update()
        {   
            if (target == null) 
            {
                Destroy(gameObject);
                return;
            }
            if (isHoming && !target.IsDead())
            {
                transform.LookAt(GetAimLocation());
            }
            transform.Translate(Vector3.forward * projectileSpeed * Time.deltaTime); 
            Destroy(gameObject, 8f);         
        }

        public void SetTarget(Health target, float damage)
        {
            this.target = target;
            this.damage = damage;
        }


        private Vector3 GetAimLocation()
        {
            Collider targetCollider = target.GetComponent<Collider>();
            if (targetCollider == null) return target.transform.position;
            return targetCollider.bounds.center + Vector3.up * 0.5f;
        }

        private void OnTriggerEnter(Collider other) 
        {
            if (other.GetComponent<Health>() != target) return;
            target.TakeDamage(damage);

            if (hitEffect !=null)
            {
                Instantiate(hitEffect, GetAimLocation(), transform.rotation);
            }

            foreach (GameObject toDestroy in destroyAfterImpact)
            {
                Destroy(toDestroy);
            }

            Destroy(gameObject, lifeAfterImpact);      
        }
    }

}

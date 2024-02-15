using UnityEngine;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using System;
using RPG.Attributes;
using RPG.Utils;
using Unity.VisualScripting;

namespace RPG.Control
{
    public class AIController : MonoBehaviour
    {   
        [SerializeField] float chaseDistance = 5f;
        [SerializeField] float suspicionTime = 3f;
        [SerializeField] float waypointDwellTime = 3f;
        [SerializeField] PatrolPath patrolPath;
        [SerializeField] float waypointTolerance = 2.2f;
        [Range(0, 1)]
        [SerializeField] float patrolSpeedFraction = 0.2f;
        [SerializeField] float aggroTimeCooldown = 3f;
        [SerializeField] float alertRadius = 5f;

        Fighter fighter;
        GameObject player;
        Health health;
        Mover mover;
        LazyValue<Vector3> guardPosition;
        float timeSinceLastSawPlayer = Mathf.Infinity;
        float timeSpentOnCurrentWaypoint = Mathf.Infinity;
        float timeSinceAggravated = Mathf.Infinity;
        int currentWaypointIndex = 0;

        bool hasBeenAggrodRecently = false;

        void Awake()
        {
            fighter = GetComponent<Fighter>();
            mover = GetComponent<Mover>();
            health = GetComponent<Health>();
            player = GameObject.FindWithTag("Player");
            guardPosition = new LazyValue<Vector3>(GetInitialPosition);
        }
        void Start()
        {
            guardPosition.ForceInit();
        }

        Vector3 GetInitialPosition()
        {
            return transform.position;
        }

        void Update()
        {
            if (health.IsDead()) return;

            if (IsAggravated() && fighter.CanAttack(player))
            {
                AttackBehaviour();
            }
            else if (timeSinceLastSawPlayer < suspicionTime)
            {
                SuspicionBehaviour();
            }
            else
            {
                PatrolBehaviour();
            }
            
            UpdateTimers();
        }

        private void AttackBehaviour()
        {   
            timeSinceLastSawPlayer = 0f;
            fighter.Attack(player);

            AggravateNearbyEnemies();
        }

        private void AggravateNearbyEnemies()
        {
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, alertRadius, Vector3.up, 0);
            foreach (RaycastHit hit in hits)
            {
                AIController enemy =  hit.transform.GetComponent<AIController>();
                if (enemy == null) continue;

                enemy.AggroAllies();
            }
        }

        private void SuspicionBehaviour()
        {
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }
        private void PatrolBehaviour()
        {   
            Vector3 nextPosition = guardPosition.value;
            if (patrolPath != null)
            {   
                if (AtWaypoint())
                {                      
                    timeSpentOnCurrentWaypoint = 0f;
                    CycleWaypoint();                      
                }
                nextPosition = GetCurrentWaypoint();
            }

            if (timeSpentOnCurrentWaypoint > waypointDwellTime)
            {
                mover.StartMoveAction(nextPosition, patrolSpeedFraction);
            }
        }

        public void Aggravate()
        {
            timeSinceAggravated = 0f;
            timeSinceLastSawPlayer = 0f;
        }

        public void AggroAllies()
        {   
            if (hasBeenAggrodRecently == true) return;

            if (hasBeenAggrodRecently == false)
            {
                timeSinceAggravated = 0f;
                timeSinceLastSawPlayer = 0f;
                hasBeenAggrodRecently = true;
            }
            
        }

        private bool AtWaypoint()
        {
            float distanceToWaypoint = Vector3.Distance(transform.position, GetCurrentWaypoint());
            return distanceToWaypoint < waypointTolerance;
        }

        private void CycleWaypoint()
        {   
            currentWaypointIndex = patrolPath.GetNextIndex(currentWaypointIndex);         
        }

        private Vector3 GetCurrentWaypoint()
        {
            return patrolPath.GetWaypoint(currentWaypointIndex);
        }

        private bool IsAggravated()
        {
            float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
            return distanceToPlayer <= chaseDistance || timeSinceAggravated < aggroTimeCooldown; 
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
        }

        private void UpdateTimers()
        {
            timeSinceLastSawPlayer += Time.deltaTime;
            timeSpentOnCurrentWaypoint += Time.deltaTime;
            timeSinceAggravated += Time.deltaTime;

            if (timeSinceAggravated >= aggroTimeCooldown && timeSinceLastSawPlayer >= suspicionTime)
            {
                hasBeenAggrodRecently = false;
            }
        }

    }
}

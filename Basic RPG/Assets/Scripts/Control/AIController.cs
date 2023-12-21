using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using RPG.Combat;
using RPG.Core;
using UnityEditor;
using RPG.Movement;
using System;
using UnityEngine.AI;

namespace RPG.Control
{
    public class AIController : MonoBehaviour
    {   
        [SerializeField] float chaseDistance = 5f;
        [SerializeField] float suspicionTime = 3f;
        [SerializeField] float waypointDwellTime = 3f;
        [SerializeField] PatrolPath patrolPath;
        [SerializeField] float waypointTolerance = 2.2f;

        Fighter fighter;
        GameObject player;
        Health health;
        Vector3 guardPosition;
        float timeSinceLastSawPlayer = Mathf.Infinity;
        float timeSpentOnCurrentWaypoint = Mathf.Infinity;
        int currentWaypointIndex = 0;

        void Start()
        {   
            fighter = GetComponent<Fighter>();
            player = GameObject.FindWithTag("Player");
            health = GetComponent<Health>();
            guardPosition = transform.position;
        }
        void Update()
        {
            if (health.IsDead()) return;

            if (InAttackRangeOfPlayer() && fighter.CanAttack(player))
            {
                AttackBehaviour();
            }
            else if (timeSinceLastSawPlayer < suspicionTime)
            {
                SuspicionBehaviour();
            }
            else
            {
                GetComponent<NavMeshAgent>().speed = 3f;
                PatrolBehaviour();
            }
            
            UpdateTimers();
        }

        private void AttackBehaviour()
        {   
            timeSinceLastSawPlayer = 0f;
            fighter.Attack(player);
        }

        private void SuspicionBehaviour()
        {
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }
        private void PatrolBehaviour()
        {   
            Vector3 nextPosition = guardPosition;
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
                GetComponent<Mover>().StartMoveAction(nextPosition);
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

        private bool InAttackRangeOfPlayer()
        {
            float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
            return distanceToPlayer <= chaseDistance; 
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
        }

    }
}

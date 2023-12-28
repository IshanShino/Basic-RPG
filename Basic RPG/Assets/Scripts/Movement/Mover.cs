using UnityEngine;
using UnityEngine.AI;
using RPG.Core;
using RPG.Saving;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using System.Collections.Generic;
using Unity.VisualScripting;

namespace RPG.Movement
{
    public class Mover : MonoBehaviour, IAction, ISaveable
    {   
        [SerializeField] float maxSpeed = 6f;
        NavMeshAgent navMeshAgent;
        Health health;

        void Awake() 
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
        }
        void Start()
        {
            health = GetComponent<Health>();
        }
        void Update()
        {   
            navMeshAgent.enabled = !health.IsDead();           
            UpdateAnimator();
        }

        public void StartMoveAction(Vector3 destination, float speedFraction)
        {   
            GetComponent<ActionScheduler>().StartAction(this);
            MoveTo(destination, speedFraction);
        }

        public void MoveTo(Vector3 destination, float speedFraction)
        {   
            navMeshAgent.destination = destination;
            navMeshAgent.speed = maxSpeed * Mathf.Clamp01(speedFraction);
            navMeshAgent.isStopped = false;
        }

        public void Cancel()
        {   
            navMeshAgent.isStopped = true;
        }
        void UpdateAnimator()
        {
            Vector3 velocity = GetComponent<NavMeshAgent>().velocity;
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);
            float speed = localVelocity.z;
            GetComponent<Animator>().SetFloat("forwardSpeed", speed);
        }

        public object CaptureState()
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data["position"] = new SerializableVector3(transform.position);
            data["rotation"] = new SerializableVector3(transform.eulerAngles);
            return data;
        }

        public void RestoreState(object state)
        {
            Dictionary<string, object> data = (Dictionary<string, object>)state;
            navMeshAgent.enabled = false;
            transform.position = ((SerializableVector3)data["position"]).ToVector();
            transform.eulerAngles = ((SerializableVector3)data["rotation"]).ToVector();
            navMeshAgent.enabled = true;
        }
    }
}

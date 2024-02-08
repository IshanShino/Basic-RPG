using UnityEngine;
using UnityEngine.AI;
using RPG.Core;
using RPG.JsonSaving;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using System.Collections.Generic;
using Unity.VisualScripting;
using Newtonsoft.Json.Linq;
using RPG.Attributes;

namespace RPG.Movement
{
    public class Mover : MonoBehaviour, IAction, IJsonSaveable
    {   
        [SerializeField] float maxSpeed = 6f;
        NavMeshAgent navMeshAgent;
        Health health;
        private struct MoverSaveData
        {
            public JToken position;
            public JToken rotation;
        }
        void Awake() 
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
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

        public JToken CaptureAsJToken()
        {
            return JToken.FromObject(new MoverSaveData()
            {
                position = transform.position.ToToken(),
                rotation = transform.eulerAngles.ToToken()
            });
        }

        public void RestoreFromJToken(JToken state)
        {
            navMeshAgent.enabled = false;
            MoverSaveData data = state.ToObject<MoverSaveData>();
            navMeshAgent.Warp(data.position.ToVector3());
            transform.eulerAngles = data.rotation.ToVector3();
            navMeshAgent.enabled = true;
        }
    }

}

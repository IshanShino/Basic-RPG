using UnityEngine;
using UnityEngine.AI;
using RPG.Core;
using RPG.JsonSaving;
using Newtonsoft.Json.Linq;
using RPG.Attributes;

namespace RPG.Movement
{
    public class Mover : MonoBehaviour, IAction, IJsonSaveable
    {   
        [SerializeField] float maxSpeed = 6f;
        [SerializeField] float maxPathLength = 30f;
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

        public bool CanMoveTo(Vector3 destination)
        {
            NavMeshPath path = new NavMeshPath();
            bool hasPath = NavMesh.CalculatePath(transform.position, destination, NavMesh.AllAreas, path);
            if (!hasPath) return false;
            if (path.status != NavMeshPathStatus.PathComplete) return false;
            if (GetPathLength(path) > maxPathLength) return false;

            return true;
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

        private float GetPathLength(NavMeshPath path)
        {   
            float totalPathLength = 0f;

            if (path.corners.Length < 2) return totalPathLength;
            
            for (int i = 0; i < path.corners.Length - 1; i++)
            {
                totalPathLength += Vector3.Distance(path.corners[i], path.corners[i + 1]);
            }
            return totalPathLength;
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

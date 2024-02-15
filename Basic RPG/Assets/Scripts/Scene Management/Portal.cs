using System.Collections;
using RPG.Control;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace RPG.SceneManagement
{
    public class Portal : MonoBehaviour
    {   
        enum DestinationIdentifier
        {
            A, B, C, D, E
        }
        [SerializeField] int sceneToLoad = -1;
        [SerializeField] Transform spawnPoint;
        [SerializeField] DestinationIdentifier destination;
        [SerializeField] DestinationIdentifier destinationToGo;
        [SerializeField] float fadeOutTime = 1f;
        [SerializeField] float fadeInTime = 2f;
        [SerializeField] float fadeWaitTime = 0.5f;

        private void OnTriggerEnter(Collider other) 
        {
            if (other.tag == "Player")
            {   
                StartCoroutine(Transition());
            }
        }

        private IEnumerator Transition()
        {   
            if (sceneToLoad < 0)
            {   
                Debug.LogError("Scene not found!");
                yield break;
            }
            DontDestroyOnLoad(gameObject);

            Fader fader = FindObjectOfType<Fader>();
            
            PlayerController playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
            playerController.enabled = false;

            yield return fader.FadeOut(fadeOutTime);

            SavingWrapper wrapper =  FindObjectOfType<SavingWrapper>();
            wrapper.Save();
            
            yield return SceneManager.LoadSceneAsync(sceneToLoad);

            PlayerController newPlayerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
            newPlayerController.enabled = false;
           
            wrapper.Load();

            Portal otherPortal = GetOtherPortal();
            UpdatePlayer(otherPortal);

            wrapper.Save();

            yield return new WaitForSeconds(fadeWaitTime);
            yield return fader.FadeIn(fadeInTime); // for immediate fade in and control enabled, just remove yield return

            newPlayerController.GetComponent<PlayerController>().enabled = true;
            Destroy(gameObject);   
        }

        private void UpdatePlayer(Portal otherPortal)
        {   
            GameObject player = GameObject.FindWithTag("Player");
            player.GetComponent<NavMeshAgent>().Warp(otherPortal.spawnPoint.position);
            player.transform.rotation = otherPortal.spawnPoint.rotation;
        }

        private Portal GetOtherPortal()
        {
            foreach (Portal portal in FindObjectsOfType<Portal>())
            {
                if (portal ==  this) continue;
                if (portal.destination == this.destinationToGo)
                {
                    return portal;
                }
            }
            return null;
        }
    }
}
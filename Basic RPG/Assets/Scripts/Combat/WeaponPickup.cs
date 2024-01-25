using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Combat
{
    public class WeaponPickup : MonoBehaviour
    {   
        [SerializeField] Weapon weaponToPickup;
        [SerializeField] float respawnTime = 5f;
        private void OnTriggerEnter(Collider other) 
        {
            if (other.gameObject.tag == "Player")
            {
                other.GetComponent<Fighter>().EquipWeapon(weaponToPickup);
                StartCoroutine(PickupRespawner(respawnTime));
            }
        }

        private IEnumerator PickupRespawner(float waitTime)
        {   
            ShowPickup(false);
            yield return new WaitForSeconds(waitTime);
            ShowPickup(true);
        }

        private void ShowPickup(bool shouldShow)
        {   
            GetComponent<Collider>().enabled = shouldShow;
            foreach(Transform child in transform)
            {
                child.gameObject.SetActive(shouldShow);
            }
        }
    }
}

using System.Collections;
using RPG.Combat;
using RPG.Control;
using RPG.Core;
using RPG.Movement;
using UnityEngine;

namespace RPG.Pickups
{
    public class WeaponPickup : MonoBehaviour, IRaycastable
    {   
        [SerializeField] WeaponConfig weaponToPickup;
        [SerializeField] float respawnTime = 5f;
        private void OnTriggerEnter(Collider other) 
        {
            if (other.gameObject.tag == "Player")
            {
                Pickup(other.GetComponent<Fighter>());
            }
        }
        private void Pickup(Fighter fighter)
        {
            fighter.EquipWeapon(weaponToPickup);
            StartCoroutine(PickupRespawner(respawnTime));
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

        public bool HandleRaycast(PlayerController controller)
        {
            if (Input.GetMouseButton(0))
            {
                controller.GetComponent<Mover>().StartMoveAction(transform.position, 1f);
            }
            return true;
        }

        public CursorType GetCursorType(PlayerController controller)
        {
            return controller.PickupCursor;
        }
    }
}

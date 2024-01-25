using System;
using RPG.Combat;
using RPG.Core;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make New Weapon", order = 0)]
public class Weapon : ScriptableObject 
{   
    [SerializeField] float weaponRange = 3f;
    public float WeaponRange { get {return weaponRange;} } 
    [SerializeField] float weaponDamage = 30f;
    public float WeaponDamage { get {return weaponDamage;} }

    [SerializeField] GameObject equippedWeapon = null;
    [SerializeField] Projectile projectile = null;
    [SerializeField] AnimatorOverrideController animatorOverride = null;
    [SerializeField] bool isRightHanded = true;

    public GameObject Spawn(Transform rightHand, Transform leftHand, Animator animator)
        {   
            GameObject weapon = null;          
            if (equippedWeapon != null)
            {
                Transform handTransform = GetTransform(rightHand, leftHand);
                weapon = Instantiate(equippedWeapon, handTransform);
            }

            // Extra but not necessary logic for override controller, basically resetting the default controller if there is no
            // override in the editor put in. Easily avoidable using a debug.log warning in the else statement or creating a
            // override controller for the fist "weapon". Lecture 125

            var overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;
            if (animatorOverride != null)
            {
                animator.runtimeAnimatorController = animatorOverride; 
            }
            else if (overrideController != null)
            {
                animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;
            }  
            return weapon;
        }
        
    private Transform GetTransform(Transform rightHand, Transform leftHand)
    {
        Transform handTransform;
        if (isRightHanded) handTransform = rightHand;
        else handTransform = leftHand;
        return handTransform;
    }

    public bool HasProjectile()
    {
        return projectile != null;
    }

    public void LaunchProjectile(Transform rightHand, Transform leftHand, Health target)
    {
        if (projectile != null)
        {   
            Transform handTransform = GetTransform(rightHand, leftHand);
            Projectile projectileInstance = Instantiate(projectile, handTransform.position, Quaternion.identity);
            projectileInstance.SetTarget(target, weaponDamage);
        }
    }
}

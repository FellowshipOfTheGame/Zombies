using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static InterfacesMNG;

public class Bullet : MonoBehaviour
{
    private Transform playerCamera;
    private delegate void DamageDelegate();
    private DamageDelegate damageDelegate;
    
    public void Initialize(WeaponStruct data, Transform playerCameraTransform, Color color) // WeaponStruct.Special special)
    {
        playerCamera = playerCameraTransform;
        
        gameObject.GetComponent<Rigidbody>().AddRelativeForce(20*Vector3.forward, ForceMode.Impulse);
        
        // StartCoroutine(Timer(fuseTime, ()=> Explode(range, damage)));
        // "The ()=> Explode(range,damage) part is a parameter‑less delegate that, when invoked, runs Explode with the values you captured (range and damage)."
        
        StartCoroutine(Timer(1f, () => Explode(data.damage, data.sFloat, color)) );
        
        // switch (special)
        // {
        //     case WeaponStruct.Special.Explosive: damageDelegate = 
        //         break;
        //     case WeaponStruct.Special.LowHealth:
        //         break;
        //     case WeaponStruct.Special.HighHealth:
        //         break;
        //     case WeaponStruct.Special.Echo:
        //         break;
        //     case WeaponStruct.Special.True:
        //         break;
        //     case WeaponStruct.Special.Poison:
        //         break;
        //     case WeaponStruct.Special.Bleed:
        //         break;
        //     case WeaponStruct.Special.Hemorrhage:
        //         break;
        //     case WeaponStruct.Special.DoT:
        //         break;
        //     case WeaponStruct.Special.Healing:
        //         break;
        //     case WeaponStruct.Special.HoT:
        //         break;
        //     default:
        //     case WeaponStruct.Special.None:
        //         break;
        // }
    }
    
    // private void OnCollisionEnter(Collision other)
    // {
    //     ICombat iCombat = other.gameObject.GetComponent<ICombat>();
    // }
    
    private static IEnumerator Timer(float time, Action callback)
    {
        yield return new WaitForSeconds(time);
        callback.Invoke();
    }
    
    private void Explode(int damage, float range, Color color)
    {
        Collider playerCollider = playerCamera.GetComponent<Collider>();
        var colliders = new List<Collider>(Physics.OverlapSphere(transform.position, range));
        
        if (colliders.Contains(playerCollider))
        {
            colliders.Remove(playerCollider);
            DamageCollider(playerCollider, damage/2, range, color); // halve self-damage
        }
        
        foreach (Collider col in colliders) DamageCollider(col, damage, range, color);
        Destroy(gameObject);
    }
    
    // private void Implode(float range, int damage, Color color)
    // {
    //     Collider[] colliders = Physics.OverlapSphere(transform.position, range);
    //     foreach (Collider col in colliders)
    //     {
    //         Vector3 direction = (transform.position - col.transform.position).normalized;
    //         // float falloff = Mathf.Pow(1f - (direction.magnitude / range), 2f); // decai exponencialmente
    //         
    //         col.GetComponent<Health>()?.TakeDamage(damage, col.transform.position, playerCamera, color);
    //         col.attachedRigidbody?.AddForce(2f * direction,  ForceMode.Impulse);
    //     }
    //     Destroy(gameObject);
    // }
    
    private void DamageCollider(Collider col, int damage, float range, Color color)
    {
        Vector3 direction = col.transform.position - transform.position;
        float fallOff = direction.magnitude/range; // decai linearmente
        // float falloff = Mathf.Pow(1f - (direction.magnitude / range), 2f); // decai exponencialmente
            
        col.GetComponent<ICombat>()?.TakeDamage(Mathf.FloorToInt(damage * fallOff), col.transform.position, playerCamera, color);
        
        if (col.attachedRigidbody)
        {
            if (col.attachedRigidbody.linearDamping > 0) col.attachedRigidbody.AddForce(100 * damage * fallOff * direction);
            else col.attachedRigidbody.AddForce(10 * damage * fallOff * direction);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    public AiHealth health;

    public void OnRaycastHit(Weapon weapon, Vector3 direction)
    {
        if (this.gameObject.CompareTag("Head")) 
        {
            health.TakeDamage(weapon.headShotDamage, direction);
        }
        else
        {
            health.TakeDamage(weapon.damage, direction);
        }
    }

    public void Damage(int damage, Vector3 direction)
    {
        health.TakeDamage(damage, direction);
    }
}
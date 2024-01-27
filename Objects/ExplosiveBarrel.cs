using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveBarrel : MonoBehaviour
{
    [SerializeField] private GameObject barrel, explosion;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private float explosionRadius = 5f;
    [SerializeField] private int damage = 100;
    private Vector3 forceDirection;
    private CapsuleCollider capsuleCollider;

    private void Awake()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();
        explosion.SetActive(false);
    }

    public void Explode()
    {
        barrel.SetActive(false);
        explosion.SetActive(true);
        capsuleCollider.enabled = !capsuleCollider.enabled;
        audioSource.Play();

        // Generate a random force direction
        forceDirection = Random.insideUnitSphere * -1; // Multiply by -1 to make it negative

        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider hit in colliders)
        {
            if (hit.CompareTag("Player"))
            {
                PHealth.instance.TakeDamage(damage);
            }
            if (hit.CompareTag("Enemy"))
            {
                HitBox hitBox = hit.GetComponent<HitBox>();
                hitBox.Damage(damage, forceDirection);
            }
        }
    
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}

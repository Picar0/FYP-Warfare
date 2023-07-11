using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiWeapon : MonoBehaviour
{
    public GameObject weapon;
    public float fireRate = 1.0f;
    public float inaccuracy = 0.17f;
    private bool canShoot = true;
    private float shootTimer = 0f;
    AiWeaponIK weaponIK;
    Transform currentTarget;
    public ParticleSystem muzzleFlash;
    public Transform barrel;
    public TrailRenderer tracerEffect;
    public LayerMask layerMask;
    public int damageAmount = 10;

    Ray ray;
    RaycastHit hitInfo;

    // Sound variables
    public AudioSource audioSource;
    public AudioClip gunshotSound;

    private void Start()
    {
      weaponIK = GetComponent<AiWeaponIK>();
    }

    public void SetTarget(Transform target)
    {
        weaponIK.SetTargetTransform(target);
        currentTarget = target;
    }

    public void DropWeapon()
    {
        if (weapon)
        {
            weapon.gameObject.GetComponent<BoxCollider>().enabled = true;
            weapon.gameObject.AddComponent<Rigidbody>();
            weapon = null;
        }
    }

    public void Shoot()
    {
        if (!canShoot || !weapon)
            return;

        muzzleFlash.Emit(1);

        ray.origin = barrel.position;
        ray.direction = (currentTarget.position + new Vector3(0f, 1.4f, 0f)) - barrel.position;

        ray.direction += Random.insideUnitSphere * inaccuracy;

        var tracer = Instantiate(tracerEffect, ray.origin, Quaternion.identity);
        tracer.AddPosition(ray.origin);

        if (Physics.Raycast(ray, out hitInfo, layerMask))
        {
            tracer.transform.position = hitInfo.point;

            // Check if the ray hit the player
            PlayerHealth playerHealth = hitInfo.collider.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount);
            }
        }

        // Play gunshot sound
        audioSource.PlayOneShot(gunshotSound);

        canShoot = false;
        shootTimer = 1f / fireRate;
    }

    private void Update()
    {
        if (!canShoot)
        {
            shootTimer -= Time.deltaTime; // Decrement shootTimer correctly

            if (shootTimer <= 0f)
            {
                canShoot = true;
                shootTimer = 0f; // Reset the shoot timer
            }
        }
    }
}

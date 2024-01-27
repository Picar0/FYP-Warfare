using System.Collections;
using UnityEngine;

public class AiWeapon : MonoBehaviour
{
    [SerializeField] private GameObject weapon;
    [SerializeField] private int damage = 10;
    [SerializeField] private float inaccuracy = 0.17f;
    [SerializeField] private float distanceForInAccuracy = 20f;
    [SerializeField] private float fireRate; // Add fire rate specific to AI
    [SerializeField] private Transform barrel;
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private ParticleSystem hitEffect;
    [SerializeField] private ParticleSystem bloodHitEffect;
    [SerializeField] private TrailRenderer tracerEffect;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip firingSound;
    [SerializeField] private Vector3 targetOffest = new Vector3(0f, 1f, 0f);
    [SerializeField] private LayerMask layerMask;
    private Ray ray;
    private AiWeaponIK weaponIK;
    private Transform currentTarget;
    public bool isFiring = false;


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
            weapon.gameObject.GetComponent<MeshCollider>().enabled = true;
            weapon.gameObject.AddComponent<Rigidbody>();
            weapon = null;
        }
    }

    public void StartFiring()
    {
        if (isFiring) return; //If already firing, do nothing
        isFiring = true;
        StartCoroutine(FireBullets());
    }

    public void StopFiring()
    {
        isFiring = false;
    }


    private IEnumerator FireBullets()
    {
        while (isFiring)
        {
            FireBullet();
            yield return new WaitForSeconds(1f / fireRate);
        }

        isFiring = false; // Reset flag when done firing
    }


    private void FireBullet()
    {
        muzzleFlash.Emit(1);
        audioSource.PlayOneShot(firingSound);
        RaycastHit hitInfo;
        ray.origin = barrel.position;

        // Calculate direction based on distance to target
        float distanceToTarget = Vector3.Distance(currentTarget.position, transform.position);
        float currentInaccuracy = distanceToTarget > distanceForInAccuracy ? inaccuracy : 0f;

        ray.direction = (currentTarget.position + targetOffest) - barrel.position;
        ray.direction += Random.insideUnitSphere * currentInaccuracy;

        var tracer = Instantiate(tracerEffect, ray.origin, Quaternion.identity);
        tracer.AddPosition(ray.origin);

        //int layerMask = ~LayerMask.GetMask("Enemy");

        if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, layerMask))
        {
            if (hitInfo.collider.CompareTag("Player"))
            {
                bloodHitEffect.transform.position = hitInfo.point;
                bloodHitEffect.transform.forward = hitInfo.normal;
                bloodHitEffect.Emit(1);
                PHealth.instance.TakeDamage(damage);
            }
            else
            {
                hitEffect.transform.position = hitInfo.point;
                hitEffect.transform.forward = hitInfo.normal;
                hitEffect.Emit(1);
            }
            tracer.transform.position = hitInfo.point;

            ExplosiveBarrel explosiveBarrel = hitInfo.collider.GetComponent<ExplosiveBarrel>();
            if (explosiveBarrel)
            {
                explosiveBarrel.Explode();
            }
        }
        else
        {
            tracer.transform.position = ray.origin + ray.direction * 100.0f;
        }
    }
}

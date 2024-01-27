using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Cinemachine;

public class Weapon : MonoBehaviour
{
    public static Weapon instance;
    public float damage = 20;
    public float headShotDamage = 100;
    public int totalAmmoCount = 360;
    [SerializeField] private WeaponAnimationEvents weaponAnimationEvents;
    [SerializeField] private CinemachineImpulseSource cameraShake;
    [SerializeField] private Animator rigAnimator;
    [SerializeField] private Transform leftHand;
    [SerializeField] private GameObject magazine;
    [SerializeField] private Transform barrel;
    [SerializeField] private Transform RayCastDestination;
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private ParticleSystem hitEffect;
    [SerializeField] private ParticleSystem bloodHitEffect;
    [SerializeField] private ParticleSystem bulletCaseEjectingEffect;
    [SerializeField] private TrailRenderer tracerEffect;
    [SerializeField] private TextMeshProUGUI ammoCountText;
    [SerializeField] private int fireRate = 8;
    [SerializeField] private int maxAmmoCount = 30;
    [SerializeField] private float reloadTime = 1.5f;
    [SerializeField] private bool isFiring = false;
    [SerializeField] private bool Reloading = false; 
    private GameObject magHand;
    private GameObject droppedMag;
    private float delay = 2f;
    private int currentAmmoCount;
    private int ammoToAdd;
    private bool isReloading = false;
    private IEnumerator firingCoroutine;
    private Coroutine textDisplayCoroutine;
    private Ray ray;




    private void Awake()
    {
        instance = this;
        weaponAnimationEvents.WeaponAnimationEvent.AddListener(OnAnimationEvents);
        // Set the initial values for the ammo count and UI text elements
        currentAmmoCount = maxAmmoCount;
        UpdateAmmoCountText();
    }

    private void Update()
    {
        if (currentAmmoCount == 0)
        {
            Reload();
        }
    }

    private void Recoil()
    {
        cameraShake.GenerateImpulse(Camera.main.transform.forward);
    }

    //Animation events cases for reload animation
    private void OnAnimationEvents(string eventName)
    {
        switch (eventName)
        {
            case "detach_magazine":
                DetachMagazine();
                break;
            case "drop_magazine":
                DropMagazine();
                break;
            case "refill_magazine":
                RefillMagazine();
                break;
            case "attach_magazine":
                AttachMagazine();
                break;
        }
    }

    private void DetachMagazine()
    {
        magHand = Instantiate(magazine, leftHand, true);
        magazine.SetActive(false);
    }

    private void DropMagazine()
    {
        droppedMag = Instantiate(magHand, magHand.transform.position, magHand.transform.rotation);
        droppedMag.AddComponent<Rigidbody>();
        droppedMag.AddComponent<BoxCollider>();
        StartCoroutine(WaitAndDestroy());
        magHand.SetActive(false);
    }

    private void RefillMagazine()
    {
        magHand.SetActive(true);
    }

    private void AttachMagazine()
    {
        magazine.SetActive(true);
        Destroy(magHand);
    }


    private IEnumerator WaitAndDestroy()
    {
        yield return new WaitForSeconds(delay);
        Destroy(droppedMag);
    }


    public void UpdateAmmoCountText()
    {
        string ammoCountString = currentAmmoCount.ToString() + "/" + totalAmmoCount.ToString();
        if (ammoCountString != ammoCountText.text)
        {
            ammoCountText.text = ammoCountString;
        }
    }



    public void Reload()
    {
        Reloading = true;
        isFiring = false;
        if (isReloading) return;
        if (totalAmmoCount == 0) return;
        if (totalAmmoCount == 0 && currentAmmoCount == 0) return; // Check if total ammo count and current ammo count are both 0

        if (currentAmmoCount == maxAmmoCount)
        {
            Reloading = false;
            return;
        }

        isReloading = true;

        // Subtract the amount of ammo being reloaded from the total ammo count
        ammoToAdd = Mathf.Min(maxAmmoCount - currentAmmoCount, totalAmmoCount);
        totalAmmoCount -= ammoToAdd;

        // play reload animation/sound, etc.
        rigAnimator.CrossFade("Reloading", 0.15f);
        AudioManager.instance.PlayPlayerSFX("Reload");
        // Wait for the reload animation/sound to finish
        StartCoroutine(WaitForReloadAnimation());
    }



    private IEnumerator WaitForReloadAnimation()
    {
        // Wait for reload animation/sound to finish
        yield return new WaitForSeconds(reloadTime);
        currentAmmoCount += ammoToAdd;
        isReloading = false;
        Reloading = false;
        UpdateAmmoCountText();

    }


    public void StartFiring()
    {
        if (isReloading)
        {
            // Stop firing immediately if reloading
            StopFiring();
            return;
        }
        if (isFiring) return;
        if (totalAmmoCount == 0 && currentAmmoCount == 0)
        {
            AudioManager.instance.PlayPlayerSFX("Empty");
            return;
        }
        isFiring = true;
        firingCoroutine = FireBullets();
        StartCoroutine(firingCoroutine);
    }

    private IEnumerator FireBullets()
    {
        while (isFiring)
        {
            FireBullet();
            yield return new WaitForSeconds(1.0f / fireRate);
        }
    }

    private void FireBullet()
    {
        currentAmmoCount--;

        muzzleFlash.Emit(1);
        
        bulletCaseEjectingEffect.Emit(1);
        AudioManager.instance.PlayPlayerSFX("Weapon Firing");
        RaycastHit hitInfo;
        ray.origin = barrel.position;
        ray.direction = RayCastDestination.position - barrel.position;

        var tracer = Instantiate(tracerEffect, ray.origin, Quaternion.identity);
        tracer.AddPosition(ray.origin);

        int layerMask = ~LayerMask.GetMask("PickUps");

        if (Physics.Raycast(ray, out hitInfo,Mathf.Infinity,layerMask))
        {
            //Debug.DrawLine(ray.origin, hitInfo.point, Color.red, 1.0f);

            if (hitInfo.collider.CompareTag("Enemy") || hitInfo.collider.CompareTag("Head"))
            {
                bloodHitEffect.transform.position = hitInfo.point;
                bloodHitEffect.transform.forward = hitInfo.normal;
                bloodHitEffect.Emit(1);
            }
            else
            {
                hitEffect.transform.position = hitInfo.point;
                hitEffect.transform.forward = hitInfo.normal;
                hitEffect.Emit(1);
            }
            tracer.transform.position = hitInfo.point;


            var hitBox = hitInfo.collider.GetComponent<HitBox>();
            if (hitBox)
            {
                hitBox.OnRaycastHit(this, ray.direction);
            }

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

        // Update the ammo count text after firing a bullet
        UpdateAmmoCountText();
        Recoil();

    }

    public void StopFiring()
    {
        if (!isFiring) return;
        isFiring = false;
        StopCoroutine(firingCoroutine);
    }
}

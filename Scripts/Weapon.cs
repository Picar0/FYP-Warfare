using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using TMPro;
using System;
using Cinemachine;

public class Weapon : MonoBehaviour
{
    [HideInInspector] public CinemachineImpulseSource cameraShake;
    public AudioSource gunfireSound;
    public CinemachineVirtualCamera camera1;
    public CinemachineVirtualCamera camera2;
    public Animator animator;
    public PlayerController playerController;
    public Transform barrel;
    public Transform RayCastDestination;
    public ParticleSystem muzzleFlash;
    public TrailRenderer tracerEffect;
    public TextMeshProUGUI ammoCountText;
    public LayerMask layerMask;
    public int fireRate = 8;
    public int totalAmmoCount = 500;
    public int maxAmmoCount = 30;
    public int currentAmmoCount = 30;
    public float damage = 20;
    public float recoilDuration;
    public float recoilAmount = 0.5f;
    public float reloadTime = 1.5f;
    public bool isFiring = false;
    public bool Reloading = false;
    private bool isReloading = false;
    private GameObject hitEffect;
    private PlayerInput playerInput;
    private InputAction shootAction;
    private InputAction reloadAction;



    
    float time;
    Ray ray;
    RaycastHit hitInfo;


    private IEnumerator firingCoroutine;

    private void Start()
    {
        cameraShake = GetComponent<CinemachineImpulseSource>();
        animator = playerController.animator;
        // Set the initial values for the ammo count and UI text elements
        currentAmmoCount = maxAmmoCount;
        totalAmmoCount = 500; // Set the total ammo count to the desired value
        UpdateAmmoCountText();


        hitEffect = ParticlePool.instance.GetObject();
        // Getting the PlayerInput component attached to the Player GameObject
        playerInput = GameObject.FindWithTag("Player").GetComponent<PlayerInput>();
        // Getting the "Shoot" action from the PlayerInput component
        shootAction = playerInput.actions["Shoot"];
        //Getting the "Reload" action from the PlayerInput component.
        reloadAction = playerInput.actions["Reload"];
        // Subscribing to the "performed" and "canceled" events of the "Shoot" action
        shootAction.performed += _ => StartFiring();
        shootAction.canceled += _ => StopFiring();

        reloadAction.performed += _ => Reloading = true;
    }

    private void Update()
    {
        //setting verticle recoil by modifying camera verticle axis over multiple frames
        if (time > 0)
        {
            camera1.GetCinemachineComponent<CinemachinePOV>().m_VerticalAxis.Value -= recoilAmount * Time.deltaTime;
            camera2.GetCinemachineComponent<CinemachinePOV>().m_VerticalAxis.Value -= recoilAmount * Time.deltaTime;
            time -= Time.deltaTime;
        }

        // Check if the player has pressed the reload button and can currently reload
        if (Reloading && !isReloading)
        {
            Reload();
        }
    }


    private void Recoil()
    {
        time = recoilDuration;
        cameraShake.GenerateImpulse(Camera.main.transform.forward);
    }


    public void UpdateAmmoCountText()
    {
        string ammoCountString = currentAmmoCount.ToString() + "/" + totalAmmoCount.ToString();
        if (ammoCountString != ammoCountText.text)
        {
            ammoCountText.text = ammoCountString;
        }
    }


 
    private void Reload()
    {
        if (!Reloading) return;

        if (isReloading) return;

        if (totalAmmoCount == 0 && currentAmmoCount == 0) return; // Check if total ammo count and current ammo count are both 0

        if (currentAmmoCount == maxAmmoCount) {
            Reloading = false;
            return; }

        isReloading = true;

        // Subtract the amount of ammo being reloaded from the total ammo count
        int ammoToAdd = Mathf.Min(maxAmmoCount - currentAmmoCount, totalAmmoCount);
        totalAmmoCount -= ammoToAdd;

        // play reload animation/sound, etc.
        //animator.CrossFade("Reloading", 0.15f);

        // Wait for the reload animation/sound to finish
        StartCoroutine(WaitForReloadAnimation(() => {
            currentAmmoCount += ammoToAdd;
            isReloading = false;
            Reloading = false;
            UpdateAmmoCountText();
        }));
    }



    private IEnumerator WaitForReloadAnimation(Action onReloadFinished)
    {
        // Wait for reload animation/sound to finish
        yield return new WaitForSeconds(reloadTime);

        onReloadFinished?.Invoke();
    }


    public void StartFiring()
    {
        if (isFiring) return;
        if (totalAmmoCount == 0 && currentAmmoCount == 0) return; // Check if total ammo count and current ammo count are both 0
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
        if (currentAmmoCount == 0)
        {
            Reload();
            return;
        }
        currentAmmoCount--;

        muzzleFlash.Emit(1);
        gunfireSound.Play();

        ray.origin = barrel.position;
        ray.direction = RayCastDestination.position - barrel.position;

        var tracer = Instantiate(tracerEffect, ray.origin, Quaternion.identity);
        tracer.AddPosition(ray.origin);

        if (Physics.Raycast(ray, out hitInfo, layerMask))
        {
            //Debug.DrawLine(ray.origin, hitInfo.point, Color.red, 1.0f);
            GameObject obj = ParticlePool.instance.GetObject();
            obj.transform.position = hitInfo.point;
            obj.transform.forward = hitInfo.normal;
            obj.SetActive(true);
            tracer.transform.position = hitInfo.point;

            var hitBox = hitInfo.collider.GetComponent<HitBox>();
            if (hitBox)
            {
                hitBox.OnRaycastHit(this, ray.direction);
            }
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
        ParticlePool.instance.ReturnObject(hitEffect);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;


public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 200;
    public Animator animator;
    public PlayerInput playerInput;
    public int currentHealth;
    public RigBuilder rigBuilder;
    public PlayerController playerController;
    public GameObject playerWeapon;
    DeathCamManager deathCamManager;

    private void Start()
    {
        currentHealth = maxHealth;
        playerInput = GetComponent<PlayerInput>();
        rigBuilder = GetComponent<RigBuilder>();
        playerController = GetComponent<PlayerController>();
        deathCamManager = FindObjectOfType<DeathCamManager>();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }
    public int GetCurrentHealth()
    {
        return currentHealth;
    }


    private void Die()
    {
        
        if (rigBuilder != null)
        {
            rigBuilder.enabled = false;
        }

        if (playerInput != null)
        {
            playerInput.enabled = false;
        }

        if (playerController != null)
        {
            playerController.enabled = false;
        }
        
        if (playerWeapon)
        {
            playerWeapon.gameObject.GetComponent<BoxCollider>().enabled = true;
            playerWeapon.gameObject.AddComponent<Rigidbody>();
            playerWeapon.transform.parent = null;
        }

        deathCamManager.EnableDeathCam();

        animator.SetTrigger("Die");

        Collider collider = GetComponent<Collider>();
        if (collider != null)
        {
            collider.enabled = false;
        }
    }
}

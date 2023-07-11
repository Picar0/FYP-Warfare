using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiHealth : MonoBehaviour
{
    AiAgent agent;
    public float maxHealth;
    [HideInInspector]
    public float currentHealth;

    UIHealthBar healthBar;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<AiAgent>();
        healthBar = GetComponentInChildren<UIHealthBar>();
        currentHealth = maxHealth;

        var rigidBodies = GetComponentsInChildren<Rigidbody>();
        foreach (var rigidBody in rigidBodies)
        {
            HitBox hitBox = rigidBody.gameObject.AddComponent<HitBox>();
            hitBox.health = this;
            if (hitBox.gameObject != gameObject)
            {
                hitBox.gameObject.layer = LayerMask.NameToLayer("Hitbox");
            }
        }
    }

    // Update is called once per frame
    public void TakeDamage(float amount, Vector3 direction)
    {
        currentHealth -= amount;
        healthBar.SetHealthBarPercentage(currentHealth / maxHealth);
        if (currentHealth <= 0.0f)
        {
            Die(direction);
        }
    }

    private void Die(Vector3 direction)
    {
        AiDeathState deathState = agent.stateMachine.GetState(AIStateId.Death) as AiDeathState;
        deathState.direction = direction;
        agent.stateMachine.ChangeState(AIStateId.Death);

    }
}

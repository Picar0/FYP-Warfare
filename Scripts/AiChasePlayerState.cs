using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AiChasePlayerState : AIState
{
    float timer = 0.0f;

    public AIStateId GetId()
    {
        return AIStateId.ChasePlayer;
    }

    public void Enter(AiAgent agent)
    {
        // Set the player transform as the target for the AI weapon
        agent.aiWeapon.SetTarget(agent.playerTransform);
    }

    public void Update(AiAgent agent)
    {
        if (!agent.enabled)
        {
            return;
        }

        timer -= Time.deltaTime;

        // Update the destination to the player's position
        agent.navMeshAgent.destination = agent.playerTransform.position;

        if (timer < 0.0f)
        {
            Vector3 direction = (agent.playerTransform.position - agent.transform.position);
            direction.y = 0;
            if (direction.sqrMagnitude > agent.config.maxDistance * agent.config.maxDistance)
            {
                if (agent.navMeshAgent.pathStatus != NavMeshPathStatus.PathPartial)
                {
                    agent.navMeshAgent.destination = agent.playerTransform.position;
                }
            }
            timer = agent.config.maxTime;

            if (agent.playerHealth.GetCurrentHealth() > 0)
            {
                agent.aiWeapon.Shoot();
            }

        }
    }

    public void Exit(AiAgent agent)
    {

    }
}

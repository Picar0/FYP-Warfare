using UnityEngine;
using UnityEngine.AI;


public class AiChasePlayerState : AIState
{
    private int agentsInChase;
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
        if (agent.aiHealth.currentHealth > 0)
        {
            // No targets found, transition to search state
            if (!agent.aiSensor.visibleTargets.Exists(target => target.CompareTag("Player")))
            {
                agent.stateMachine.ChangeState(AIStateId.Searching);
                agent.aiWeapon.SetTarget(null);
                return;
            }

            //Ai chasing the player
            Vector3 direction = (agent.playerTransform.position - agent.transform.position);
            direction.y = 0;
            if (direction.sqrMagnitude > agent.config.maxDistance * agent.config.maxDistance)
            {
                if (agent.navMeshAgent.pathStatus != NavMeshPathStatus.PathPartial)
                {
                    agent.navMeshAgent.destination = agent.playerTransform.position;
                }
            }


            //Shooting at player and checking player is dead if it is then we stop shooting
            if (PHealth.instance.currentHealth > 0)
            {
                agent.aiWeapon.StartFiring();
            }
            else
            {
                agent.aiWeapon.StopFiring();
            }
        }
    }
    public void Exit(AiAgent agent)
    {
        agent.aiWeapon.StopFiring();
    }
}
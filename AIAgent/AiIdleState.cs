using System.Linq;
using UnityEngine;

public class AiIdleState : AIState
{
    private bool TargetSet { get; set; } = false;
    public AIStateId GetId()
    {
        return AIStateId.Idle;
    }

    public void Enter(AiAgent agent)
    {
        // Reset the TargetSet flag and IsPlayerDetectedOnce flag when entering the idle state
        TargetSet = false;
    }

    public void Update(AiAgent agent)
    {
        // Switching state based on player detection and cover detection
        bool playerDetected = agent.aiSensor.visibleTargets.Any(target => target.CompareTag("Player"));

        if (playerDetected)
        {

            // Check if the chase state count is less than two
            if (AiStateManager.Instance.ChaseStateCount < 2)
            {
                AiStateManager.Instance.ChaseStateCount++;
                agent.stateMachine.ChangeState(AIStateId.ChasePlayer);
            }

            // Set the player transform as the target for the AI weapon only if not already set
            if (!TargetSet)
            {
                agent.aiWeapon.SetTarget(agent.playerTransform);
                TargetSet = true; // Mark the target as set
            }

            // Shooting at the player
            if (PHealth.instance?.currentHealth > 0)
            {
                agent.aiWeapon.StartFiring();
            }
            else
            {
                agent.aiWeapon.StopFiring();
            }

        }
        else
        {
            agent.aiWeapon.StopFiring();
            // Clear the target
            agent.aiWeapon.SetTarget(null);
            // Reset the TargetSet flag when the player is not detected
            TargetSet = false;
        }

        if (agent.aiHealth.currentHealth < agent.aiHealth.maxHealth)
        {
            agent.stateMachine.ChangeState(AIStateId.Searching);
        }
    }

    public void Exit(AiAgent agent)
    {
        agent.aiWeapon.StopFiring();
    }
}

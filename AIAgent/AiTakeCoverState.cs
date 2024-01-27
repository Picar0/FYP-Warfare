using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; // Make sure to include this namespace

public class AiTakeCoverState : AIState
{
    private float originalStoppingDistance;
    private Transform coverPoint;

    public AIStateId GetId()
    {
        return AIStateId.Cover;
    }

    public void Enter(AiAgent agent)
    {
        // Set the player transform as the target for the AI weapon
        agent.aiWeapon.SetTarget(agent.playerTransform);

        // Store the original stopping distance of the agent
        originalStoppingDistance = agent.navMeshAgent.stoppingDistance;

        // Set the stopping distance to a value appropriate for reaching cover
        agent.navMeshAgent.stoppingDistance = 1.5f;


        if (agent.aiSensor.availableCoverPoints.Count > 0)
        {
            //looping through all available cover points to check if they are occupied or not
            bool foundCover = false;
            for (int i = 0; i < agent.aiSensor.availableCoverPoints.Count; i++)
            {
                coverPoint = agent.aiSensor.availableCoverPoints[i];

                if (!CoverPointsManager.instance.occupiedCoverPoints.Contains(coverPoint))
                {
                    // If not occupied, move to that point and occupy it
                    agent.navMeshAgent.SetDestination(coverPoint.position);
                    CoverPointsManager.instance.OccupyCoverPoint(coverPoint);
                    foundCover = true; // Set flag to true
                    break;
                }
            }
            // if no cover found, change state
            if (!foundCover)
            {
                agent.stateMachine.ChangeState(AIStateId.ChasePlayer);
            }

        }
        else
        {
            //if no cover point found then switch state
            agent.stateMachine.ChangeState(AIStateId.ChasePlayer);
        }

    }

    public void Update(AiAgent agent)
    {
        // Check if agent has reached cover point
        if (!agent.navMeshAgent.pathPending && agent.navMeshAgent.remainingDistance <= agent.navMeshAgent.stoppingDistance)
        {
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

        // No targets found, transition to search state
        if (!agent.aiSensor.visibleTargets.Exists(target => target.CompareTag("Player")))
        {
            agent.stateMachine.ChangeState(AIStateId.Searching);
            agent.aiWeapon.SetTarget(null);
            return;
        }
    }

    public void Exit(AiAgent agent)
    {
        // Restore the original stopping distance
        agent.navMeshAgent.stoppingDistance = originalStoppingDistance;
        // Releasing the coverpoint occupied by agent
        CoverPointsManager.instance.ReleaseCoverPoint(coverPoint);
        // Stop firing
        agent.aiWeapon.StopFiring();
    }
}

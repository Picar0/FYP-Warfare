using System.Linq;
using UnityEngine;

public class AiPatrollingState : AIState
{
    private int waypointIndex = 0;
    private Vector3 target;
    public AIStateId GetId()
    {
        return AIStateId.Patrolling;
    }

    public void Enter(AiAgent agent)
    {
        agent.navMeshAgent.stoppingDistance = 0;
        target = agent.patrolPoints[waypointIndex].position;
        agent.navMeshAgent.SetDestination(target);
    }

    public void Update(AiAgent agent)
    {
        // Switching state based on player detection and cover detection
        bool playerDetected = agent.aiSensor.visibleTargets.Any(target => target.CompareTag("Player"));

        if (playerDetected)
        {
            agent.stateMachine.ChangeState(AIStateId.ChasePlayer);
        }

        //Switch to Search State
        if (agent.aiHealth.currentHealth < agent.aiHealth.maxHealth)
        {
            agent.stateMachine.ChangeState(AIStateId.Searching);
        }

        if (Vector3.Distance(agent.transform.position, target) < 1)
        {
            waypointIndex++;
            if (waypointIndex == agent.patrolPoints.Length)
            {
                waypointIndex = 0;
            }
            target = agent.patrolPoints[waypointIndex].position;
            agent.navMeshAgent.SetDestination(target);
        }
    }

    public void Exit(AiAgent agent)
    {
        agent.navMeshAgent.stoppingDistance = 10;
    }
}

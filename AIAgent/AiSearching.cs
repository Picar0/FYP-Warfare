using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class AiSearching : AIState
{
    private float timeToSearch = 40f;
    private Coroutine returnToIdleCoroutine;
    private AiAgent agent;

    public AIStateId GetId()
    {
        return AIStateId.Searching;
    }

    public void Enter(AiAgent agent)
    {
        this.agent = agent;
        returnToIdleCoroutine = agent.StartCoroutine(ReturnToInitialState());
    }

    public void Update(AiAgent agent)
    {
        // Switching state based on player detection 
        bool playerDetected = agent.aiSensor.visibleTargets.Any(target => target != null && target.CompareTag("Player"));

        if (playerDetected)
        {
            // Check if the chase state count is less than two
            if (AiStateManager.Instance.ChaseStateCount < 2)
            {
                agent.stateMachine.ChangeState(AIStateId.ChasePlayer);
            }
            else
            {
                // If there are already two agents in chase state, stay in idle state
                agent.stateMachine.ChangeState(AIStateId.Idle);
            }

        }

        //Setting centre point of Search area
        Transform centrePoint = agent.playerTransform;

        if (agent.navMeshAgent.remainingDistance <= agent.navMeshAgent.stoppingDistance)
        {
            Vector3 point;
            if (RandomPoint(centrePoint.position, agent.config.searchRange, out point))
            {
                Debug.DrawRay(point, Vector3.up, Color.blue, 1.0f);
                agent.navMeshAgent.SetDestination(point);
            }
        }
    }

    public void Exit(AiAgent agent)
    {
        if (returnToIdleCoroutine != null)
        {
            agent.StopCoroutine(returnToIdleCoroutine);
        }
    }

    private IEnumerator ReturnToInitialState()
    {
        yield return new WaitForSeconds(timeToSearch);
        agent.stateMachine.ChangeState(agent.initialState);
    }

    bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {
        Vector3 randomPoint = center + Random.insideUnitSphere * range;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
        {
            result = hit.position;
            return true;
        }

        result = Vector3.zero;
        return false;
    }
}

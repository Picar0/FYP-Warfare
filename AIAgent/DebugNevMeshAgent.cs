using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DebugNevMeshAgent : MonoBehaviour
{
    [SerializeField] private bool velocity;
    [SerializeField] private bool desiredVelocity;
    [SerializeField] private bool path;

    NavMeshAgent agent;


    // Start is called before the first frame update
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    private void OnDrawGizmos()
    {
        if (velocity)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, transform.position + agent.velocity);
        }

        if (desiredVelocity)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + agent.desiredVelocity);
        }

        if (path)
        {
            Gizmos.color = Color.black;
            var agentPath = agent.path;
            Vector3 prevCorner = transform.position;
            foreach (var corner in agentPath.corners)
            {
                Gizmos.DrawLine(prevCorner, corner);
                Gizmos.DrawSphere(corner, 0.1f);
                prevCorner = corner;
            }
        }
    }
}
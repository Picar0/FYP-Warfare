using System.Collections.Generic;
using UnityEngine;

public class AiStateManager : MonoBehaviour
{
    public static AiStateManager Instance { get; private set; }

    [SerializeField] private string stateName = "ChasePlayer";
    [SerializeField] private int chaseStateCount;

    private List<AiAgent> aiAgents = new List<AiAgent>();

    // Property to get the current chase state count
    public int ChaseStateCount
    {
        get { return chaseStateCount; }
        set { chaseStateCount = value; }
    }

    // Start is called before the first frame update
    private void Start()
    {
        Instance = this;
        // Cache references to AiAgent components during initialization for performance purposes
        CacheAiAgents();
    }

    private void Update()
    {
        UpdateChaseStateCount();
    }

    // Method to cache references to AiAgent components
    private void CacheAiAgents()
    {
        aiAgents.Clear();

        for (int i = 0; i < transform.childCount; i++)
        {
            AiAgent aiAgent = transform.GetChild(i).GetComponent<AiAgent>();
            if (aiAgent != null)
            {
                aiAgents.Add(aiAgent);
            }
        }
    }

    // Method to update chase state count based on the current state of all AiAgents
    private void UpdateChaseStateCount()
    {
        ChaseStateCount = 0; // Reset the count before updating

        int agentCount = aiAgents.Count;
        for (int i = 0; i < agentCount; i++)
        {
            AiAgent aiAgent = aiAgents[i];

            // Consider only active agents
            if (aiAgent.isActiveAndEnabled)
            {
                string aiAgentState = aiAgent.currentState;

                // Increase chase state count if the agent is in the chase state
                if (aiAgentState == stateName)
                {
                    ChaseStateCount++;
                }
            }
        }
    }
}

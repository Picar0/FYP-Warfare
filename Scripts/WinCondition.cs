using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinCondition : MonoBehaviour
{
    public GameObject panel;
    public Canvas camCanvas;
    private List<AiAgent> aiAgents = new List<AiAgent>();

    private void Start()
    {
        AiAgent[] agents = GetComponentsInChildren<AiAgent>();
        aiAgents.AddRange(agents);
    }

    private void Update()
    {
        bool allDisabled = true;
        foreach (AiAgent agent in aiAgents)
        {
            if (agent.gameObject.activeSelf)
            {
                allDisabled = false;
                break;
            }
        }

        // Show the win screen if all AI agents are disabled
        if (allDisabled)
        {
            panel.SetActive(true);
            camCanvas.enabled = false;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}

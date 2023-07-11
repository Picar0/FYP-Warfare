using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiDeathState : AIState
{
    public Vector3 direction;

    private float disappearTime = 5f; 

    public AIStateId GetId()
    {
        return AIStateId.Death;
    }

    public void Enter(AiAgent agent)
    {
        if (agent.weaponIK != null)
        {
            agent.weaponIK.weight = 0f;
        }
        agent.ragdoll.ActivateRagdoll();
        direction.y = 1;
        agent.ragdoll.ApplyForce(direction * agent.config.dieForce);
        agent.ui.gameObject.SetActive(false);
        agent.CallDropWeapon();
        agent.StartCoroutine(DisappearAfterTime(agent));
    }

    public void Update(AiAgent agent)
    {
       
    }

    public void Exit(AiAgent agent)
    {
        // Stop the coroutine if needed
        //agent.StopCoroutine(DisappearAfterTime(agent));
    }

    private IEnumerator DisappearAfterTime(AiAgent agent)
    {
        yield return new WaitForSeconds(disappearTime);

        agent.gameObject.SetActive(false);
    }
}

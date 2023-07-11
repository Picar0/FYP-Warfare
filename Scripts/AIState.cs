using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AIStateId
{
    ChasePlayer,
    Death,
    Idle
}

public interface AIState
{
    AIStateId GetId(); 
    void Enter(AiAgent agent);
    void Update(AiAgent agent);
    void Exit(AiAgent agent);
}

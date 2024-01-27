public enum AIStateId
{
    ChasePlayer,
    Searching,
    Patrolling,
    Death,
    Cover,
    Idle
}

public interface AIState
{
    AIStateId GetId();
    void Enter(AiAgent agent);
    void Update(AiAgent agent);
    void Exit(AiAgent agent);
}
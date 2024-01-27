public class AIStateMachine
{
    public AIState[] states;
    public AiAgent agent;
    public AIStateId currentState;


    public AIStateMachine(AiAgent agent)
    {
        this.agent = agent;
        int numStates = System.Enum.GetNames(typeof(AIStateId)).Length;
        states = new AIState[numStates];
    }

    public void RegisterState(AIState state)
    {
        int index = (int)state.GetId();
        states[index] = state;
    }

    public AIState GetState(AIStateId stateId)
    {
        int index = (int)stateId;
        return states[index];
    }

    public AIStateId GetCurrentStateId()
    {
        return currentState;
    }

    public void Update()
    {
        GetState(currentState)?.Update(agent);
    }

    public void ChangeState(AIStateId newState)
    {
        GetState(currentState)?.Exit(agent);
        currentState = newState;
        GetState(currentState)?.Enter(agent);
    }

}
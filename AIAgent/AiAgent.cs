using UnityEngine;
using UnityEngine.AI;

public class AiAgent : MonoBehaviour
{
    public AIStateId initialState;
    public string currentState;
    public AiAgentConfig config;
    public Transform[] patrolPoints; //waypoints for patrolling
    [HideInInspector] public AIStateMachine stateMachine;
    [HideInInspector] public NavMeshAgent navMeshAgent;
    [HideInInspector] public Ragdoll ragdoll;
    [HideInInspector] public UIHealthBar ui;
    [HideInInspector] public Transform playerTransform;
    [HideInInspector] public AiHealth aiHealth;
    [HideInInspector] public AiWeapon aiWeapon;
    [HideInInspector] public AiWeaponIK weaponIK;
    [HideInInspector] public AiSensor aiSensor;

    // Start is called before the first frame update
    void Start()
    {
        ui = GetComponentInChildren<UIHealthBar>();
        ragdoll = GetComponent<Ragdoll>();
        aiHealth = GetComponent<AiHealth>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        weaponIK = GetComponent<AiWeaponIK>();
        aiWeapon = GetComponent<AiWeapon>();
        aiSensor = GetComponent<AiSensor>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        stateMachine = new AIStateMachine(this);
        stateMachine.RegisterState(new AiChasePlayerState());
        stateMachine.RegisterState(new AiDeathState());
        stateMachine.RegisterState(new AiSearching());
        stateMachine.RegisterState(new AiIdleState());
        stateMachine.RegisterState(new AiPatrollingState());
        stateMachine.RegisterState(new AiTakeCoverState());
        stateMachine.ChangeState(initialState);
    }

    // Update is called once per frame
    void Update()
    {
        stateMachine.Update();
        currentState = stateMachine.GetCurrentStateId().ToString();
    }


    public void CallDropWeapon()
    {
        aiWeapon.DropWeapon();
    }
}

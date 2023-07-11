using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AiAgent : MonoBehaviour
{
    public AIStateMachine stateMachine;
    public AIStateId initialState;
    public NavMeshAgent navMeshAgent;
    public AiAgentConfig config;
    public Ragdoll ragdoll;
    public UIHealthBar ui;
    public Transform playerTransform;
    public AiWeapon aiWeapon;
    public AiWeaponIK weaponIK;
    public PlayerHealth playerHealth;

    // Start is called before the first frame update
    void Start()
    {
        ui = GetComponentInChildren<UIHealthBar>();
        ragdoll = GetComponent<Ragdoll>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        stateMachine = new AIStateMachine(this);
        stateMachine.RegisterState(new AiChasePlayerState());
        stateMachine.RegisterState(new AiDeathState());
        stateMachine.RegisterState(new AiIdleState());
        stateMachine.ChangeState(initialState);
        aiWeapon = GetComponent<AiWeapon>();
        weaponIK = GetComponent<AiWeaponIK>();
    }

    // Update is called once per frame
    void Update()
    {
        stateMachine.Update();
    }

    public void CallDropWeapon()
    {
        aiWeapon.DropWeapon();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Footsteps : MonoBehaviour
{
    public GameObject footstep;
    private PlayerController playerController;

    void Start()
    {
        playerController = GetComponent<PlayerController>();
        footstep.SetActive(false);
    }

    void Update()
    {
        if (playerController.IsMoving())
        {
            footstep.SetActive(true);  
        }
        else
        {
            footstep.SetActive(false);
        }
    }
}

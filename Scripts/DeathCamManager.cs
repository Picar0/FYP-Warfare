using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathCamManager : MonoBehaviour
{
    public Cinemachine.CinemachineVirtualCameraBase deathCam;
    public GameObject panel;
    public Canvas camCanvas;

    public void EnableDeathCam()
    {
        deathCam.Priority = 20;
        panel.SetActive(true);
        camCanvas.enabled = false;
        Cursor.lockState = CursorLockMode.None;
    }
}

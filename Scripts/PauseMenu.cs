using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;
    public Canvas camCanvas;
    public PlayerInput playerInput;
    private InputAction pauseAction;
    private InputAction aimAction;
    private InputAction lookAction;
    private InputAction shootAction;

    private void Awake()
    {
        pauseAction = playerInput.actions["PauseMenu"];
        aimAction = playerInput.actions["Aim"];
        lookAction = playerInput.actions["Look"];
        shootAction = playerInput.actions["Shoot"];
    }

    private void OnEnable()
    {
        pauseAction.Enable();
    }

    private void OnDisable()
    {
        pauseAction.Disable();
    }

    private void Start()
    {
        pauseAction.performed += PauseAction;
    }

    private void PauseAction(InputAction.CallbackContext context)
    {
        if (pauseMenu.activeSelf)
            ResumeGame();
        else
            PauseGame();
    }

    private void PauseGame()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
      
        aimAction.Disable(); 
        lookAction.Disable(); 
        shootAction.Disable();
        camCanvas.enabled = false;
        Cursor.lockState = CursorLockMode.None;
        
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        aimAction.Enable(); 
        lookAction.Enable(); 
        shootAction.Enable();
        camCanvas.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void Quit()
    {
        Application.Quit();
    }
}

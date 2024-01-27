using UnityEngine;
using Cinemachine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Animations.Rigging;

public class PlayerController : MonoBehaviour
{
    //Player Settings
    [Header("Player Settings")]
    [SerializeField] private float _playerSpeed = 2.0f;
    [SerializeField] private float _jumpHeight = 1.0f;
    [SerializeField] private float _gravityValue = -9.81f;
    [SerializeField] private float _rotationSpeed = 5f;
    [SerializeField] private GameObject playerWeapon;
    [SerializeField] private bool _groundedPlayer;
    [SerializeField] private float footstepDelay = 0.5f;
    private Player _playerInput;
    private CharacterController _controller;
    private RigBuilder rigBuilder;
    private Vector3 _playerVelocity;
    private bool hasDied = false;
    private bool wasInAir = false;
    private float timeSinceLastFootstep = 0f;
    private bool isPlayingLandingSound = false;
    private float landingSoundDelay = 0.05f; 
    private float timeSinceLanding = 0f;

    //Animator Settings
    [Header("Animator Settings")]
    [SerializeField] private float _animationTimeForTransistion = 0.15f;
    [SerializeField] private float _animationSmoothTime = 0.05f;
    private Animator _animator;
    private Vector2 _animationVelocity;
    private Vector2 _currentAnimationBlendVector;
    private int _moveZAnimParameterID;
    private int _moveXAnimParameterID;

    //Camera Settings
    [Header("Camera Settings")]
    [SerializeField] private float _mouseSensitivityX = 1.0f;
    [SerializeField] private float _mouseSensitivityY = 1.0f;
    [SerializeField] private float _touchSensitivityX = 1.0f;
    [SerializeField] private float _touchSensitivityY = 1.0f;
    [SerializeField] private GameObject _crosshair1, _crosshair2;
    [SerializeField] private CinemachineVirtualCamera _cam1;
    [SerializeField] private CinemachineVirtualCamera _cam2;
    [SerializeField] private GameObject _cam3;
    private Transform _cameraMain;
    private CinemachinePOV _aimPOV1;
    private CinemachinePOV _aimPOV2;
    private float _minVerticalCamBoundary = -35f;
    private float _maxVerticleCamBoundary = 35f;

    //UI Setting
    [Header("UI Canvas")]
    [SerializeField] private GameObject uiButtons;
    [SerializeField] private GameObject uiHealthBar;
    [SerializeField] private GameObject uiText;
    [SerializeField] private GameObject uiCrosshair;


    private void Awake()
    {
        //Getting new input system
        _playerInput = new Player();
        _controller = GetComponent<CharacterController>();
        rigBuilder = GetComponent<RigBuilder>();
        //Animator Related properties
        _animator = GetComponent<Animator>();
        _moveXAnimParameterID = Animator.StringToHash("MoveX");
        _moveZAnimParameterID = Animator.StringToHash("MoveZ");

    }

    private void OnEnable()
    {
        _playerInput.Enable();
    }

    private void OnDisable()
    {
        _playerInput.Disable();
    }

    private void Start()
    {
        // Cams Stuff
        _cameraMain = Camera.main.transform;
        _aimPOV1 = _cam1.GetCinemachineComponent<CinemachinePOV>();
        _aimPOV2 = _cam2.GetCinemachineComponent<CinemachinePOV>();
        // Cursor.lockState = CursorLockMode.Locked;

        //Firing input
        _playerInput.PlayerMain.Shoot.performed += _ => Weapon.instance.StartFiring();
        _playerInput.PlayerMain.Shoot.canceled += _ => Weapon.instance.StopFiring();
        //Reloading input
        _playerInput.PlayerMain.Reload.performed += _ => Weapon.instance.Reload();
    }

    private void Update()
    {
        if (PHealth.instance.currentHealth > 0)
        {
            CheckIfPlayerOnGround();
            PlayerMovement();
            Jump();
            CameraSwitch();
            RotateCamera();
        }
        else
        {
            Die();
        }
    }


    private void CheckIfPlayerOnGround()
    {
        _groundedPlayer = _controller.isGrounded;
        if (_groundedPlayer && _playerVelocity.y < 0)
        {
            _playerVelocity.y = 0f;
            _animator.SetBool("isJumping", false);

            if (wasInAir && !isPlayingLandingSound)
            {
                timeSinceLanding = 0f; // Reset the timer
                isPlayingLandingSound = true;
            }

            wasInAir = false;
        }
        else
        {
            wasInAir = true;
            isPlayingLandingSound = false; // Reset flag if player is not grounded
        }

        if (_controller.velocity.y < -3f)
        {
            _animator.SetBool("isJumping", true);
        }

        if (isPlayingLandingSound)
        {
            timeSinceLanding += Time.deltaTime;
            if (timeSinceLanding >= landingSoundDelay)
            {
                AudioManager.instance.PlayPlayerSFX("Land");
                isPlayingLandingSound = false;
            }
        }
    }



    private void Jump()
    {
        if (_playerInput.PlayerMain.Jump.triggered && _groundedPlayer)
        {
            _playerVelocity.y += Mathf.Sqrt(_jumpHeight * -3.0f * _gravityValue);
            _animator.SetBool("isJumping", true);
            AudioManager.instance.PlayPlayerSFX("Jump");
        }
        _playerVelocity.y += _gravityValue * Time.deltaTime;
        _controller.Move(_playerVelocity * Time.deltaTime);
    }


    private void PlayerMovement()
    {
        //Player Movement with smooth transistion between animation blend tree
        Vector2 movementInput = _playerInput.PlayerMain.Move.ReadValue<Vector2>();
        _currentAnimationBlendVector = Vector2.SmoothDamp(_currentAnimationBlendVector, movementInput, ref _animationVelocity, _animationSmoothTime);
        Vector3 move = new Vector3(_currentAnimationBlendVector.x, 0, _currentAnimationBlendVector.y);
        move = move.x * _cameraMain.right.normalized + move.z * _cameraMain.forward.normalized;
        move.y = 0f;
        _controller.Move(move * Time.deltaTime * _playerSpeed);
        _animator.SetFloat(_moveXAnimParameterID, _currentAnimationBlendVector.x);
        _animator.SetFloat(_moveZAnimParameterID, _currentAnimationBlendVector.y);
        if (_currentAnimationBlendVector.magnitude > 0.6f && _groundedPlayer)
        {
            timeSinceLastFootstep += Time.deltaTime;

            if (timeSinceLastFootstep > footstepDelay)
            {
                AudioManager.instance.PlayFootStepsSFX();
                timeSinceLastFootstep = 0f; // Reset the timer
            }
        }

        // Player rotate with camera
        Quaternion targetRotation = Quaternion.Euler(0, _cameraMain.transform.eulerAngles.y, 0);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);


    }


    private void CameraSwitch()
    {
        if (_cam2.gameObject.activeSelf == false && _playerInput.PlayerMain.Aim.triggered)
        {
            _cam1.gameObject.SetActive(false);
            _cam2.gameObject.SetActive(true);
            _crosshair1.SetActive(false);
            _crosshair2.SetActive(true);

        }
        else if (_cam2.gameObject.activeSelf == true && _playerInput.PlayerMain.Aim.triggered)
        {
            _cam1.gameObject.SetActive(true);
            _cam2.gameObject.SetActive(false);
            _crosshair1.SetActive(true);
            _crosshair2.SetActive(false);
        }
    }

    private void RotateCamera()
    {
        Vector2 lookInput = _playerInput.PlayerMain.Look.ReadValue<Vector2>();

        // Check if the game is running in the Unity Editor
        bool isEditor = Application.isEditor;

        if (isEditor)
        {
            // Mouse Input
            if (_cam1.gameObject.activeSelf == true)
            {
                _aimPOV1.m_VerticalAxis.Value = Mathf.Clamp(_aimPOV1.m_VerticalAxis.Value - lookInput.y * _mouseSensitivityY * Time.deltaTime, _minVerticalCamBoundary, _maxVerticleCamBoundary);
                _aimPOV1.m_HorizontalAxis.Value += lookInput.x * _mouseSensitivityX * Time.deltaTime;
            }
            else
            {
                _aimPOV2.m_VerticalAxis.Value = Mathf.Clamp(_aimPOV2.m_VerticalAxis.Value - lookInput.y * _mouseSensitivityY * Time.deltaTime, _minVerticalCamBoundary, _maxVerticleCamBoundary);
                _aimPOV2.m_HorizontalAxis.Value += lookInput.x * _mouseSensitivityX * Time.deltaTime;
            }
        }
        else
        {
            // Touch Input
            if (Touchscreen.current.touches.Count == 0)
                return;

            if (EventSystem.current.IsPointerOverGameObject(Touchscreen.current.touches[0].touchId.ReadValue()))
            {
                if (Touchscreen.current.touches.Count > 1 && Touchscreen.current.touches[1].isInProgress)
                {
                    if (EventSystem.current.IsPointerOverGameObject(Touchscreen.current.touches[1].touchId.ReadValue()))
                        return;

                    Vector2 touchDeltaPosition = Touchscreen.current.touches[1].delta.ReadValue();
                    if (_cam1.gameObject.activeSelf == true)
                    {
                        _aimPOV1.m_VerticalAxis.Value = Mathf.Clamp(_aimPOV1.m_VerticalAxis.Value - touchDeltaPosition.y * _touchSensitivityY * Time.deltaTime, _minVerticalCamBoundary, _maxVerticleCamBoundary);
                        _aimPOV1.m_HorizontalAxis.Value += touchDeltaPosition.x * _touchSensitivityX * Time.deltaTime;
                    }
                    else
                    {
                        _aimPOV2.m_VerticalAxis.Value = Mathf.Clamp(_aimPOV2.m_VerticalAxis.Value - touchDeltaPosition.y * _touchSensitivityY * Time.deltaTime, _minVerticalCamBoundary, _maxVerticleCamBoundary);
                        _aimPOV2.m_HorizontalAxis.Value += touchDeltaPosition.x * _touchSensitivityX * Time.deltaTime;
                    }
                }
            }
            else
            {
                if (Touchscreen.current.touches.Count > 0 && Touchscreen.current.touches[0].isInProgress)
                {
                    if (EventSystem.current.IsPointerOverGameObject(Touchscreen.current.touches[0].touchId.ReadValue()))
                        return;

                    Vector2 touchDeltaPosition = Touchscreen.current.touches[0].delta.ReadValue();
                    if (_cam1.gameObject.activeSelf == true)
                    {
                        _aimPOV1.m_VerticalAxis.Value = Mathf.Clamp(_aimPOV1.m_VerticalAxis.Value - touchDeltaPosition.y * _touchSensitivityY * Time.deltaTime, _minVerticalCamBoundary, _maxVerticleCamBoundary);
                        _aimPOV1.m_HorizontalAxis.Value += touchDeltaPosition.x * _touchSensitivityX * Time.deltaTime;
                    }
                    else
                    {
                        _aimPOV2.m_VerticalAxis.Value = Mathf.Clamp(_aimPOV2.m_VerticalAxis.Value - touchDeltaPosition.y * _touchSensitivityY * Time.deltaTime, _minVerticalCamBoundary, _maxVerticleCamBoundary);
                        _aimPOV2.m_HorizontalAxis.Value += touchDeltaPosition.x * _touchSensitivityX * Time.deltaTime;
                    }
                }
            }
        }
    }

    private void Die()
    {
        if (!hasDied)
        {
            hasDied = true;

            _playerInput.Disable();

            if (rigBuilder != null)
            {
                rigBuilder.enabled = false;
            }
            if (playerWeapon != null)
            {
                playerWeapon.gameObject.GetComponent<MeshCollider>().enabled = true;
                playerWeapon.gameObject.AddComponent<Rigidbody>();
                playerWeapon.transform.parent = null;
            }

            // Disable all inputs and UI 
            uiButtons.gameObject.SetActive(false);
            uiHealthBar.gameObject.SetActive(false);
            uiText.gameObject.SetActive(false);
            uiCrosshair.gameObject.SetActive(false);
            _cam1.gameObject.SetActive(false);
            _cam2.gameObject.SetActive(false);
            _cam3.gameObject.SetActive(true);

            //Forcing player to be at ground when he dies in air
            while (!_controller.isGrounded)
            {
                _playerVelocity.y += _gravityValue * Time.deltaTime;
                _controller.Move(_playerVelocity * Time.deltaTime);
            }

            // Trigger dying animation
            _animator.SetTrigger("Die");
            AudioManager.instance.PlayPlayerSFX("Death");
        }
    }




}

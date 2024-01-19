using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FirstPersonController : MonoBehaviour
{
    public bool CanMove { 
        get;
        private set;
        } = true;
    
    private bool isSprinting => canSprint && Input.GetKey(sprintKey);

    [Header("Functional Options")]
    [SerializeField] private bool canSprint = true;
    [SerializeField] private bool isHeadBobbing = true;
    [SerializeField] private bool useStamina = true;


    [Header("Controls")]
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;

    [Header("Movement Parameters")]
    [SerializeField] private float walkSpeed = 3.0f;
    [SerializeField] private float sprintSpeed = 6.0f;
    [SerializeField] private float gravity = 30.0f; // MB make ragdolls instead of this construction

    [Header("Look Parameters")]
    [SerializeField, Range(1, 10)] private float lookSpeedX = 2.0f; // Must be accessible from options
    [SerializeField, Range(1, 10)] private float lookSpeedY = 2.0f; // Must be accessible from options
    [SerializeField, Range(1, 180)] private float upLookLimit = 80.0f;
    [SerializeField, Range(1, 180)] private float downLookLimit = 80.0f;

    [Header("Headbob Parameters")]
    [SerializeField] private float walkBobSpeed = 14f;
    [SerializeField] private float walkBobAmount = 0.05f;
    [SerializeField] private float sprintBobSpeed = 18f;
    [SerializeField] private float sprintBobAmount = 0.1f;

    [Header("Stamina Parameters")]
    [SerializeField] private float maxStamina = 100;
    [SerializeField] private float staminaUseMultiplier = 5;
    [SerializeField] private float timeBeforeStaminaRegenStarts = 4.0f;
    [SerializeField] private float staminaValueIncrement = 2;
    [SerializeField] private float staminaTimeIncrement = 0.1f;
    [SerializeField] private float minStaminaToSprint = 5;
    private float currentStamina = 100;
    private Coroutine regeneratingStamina;
    public static Action<float> OnStaminaChange;

    private float defaultYPos = 0;
    private float timer;


    private Camera playerCamera;
    private CharacterController characterController;

    private Vector3 moveDirection;
    private Vector2 currentInput;

    private float rotationX = 0;

    void Start()
    {
        playerCamera = GetComponentInChildren<Camera>();
        characterController = GetComponent<CharacterController>();        
        defaultYPos = playerCamera.transform.localPosition.y;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (CanMove) 
        {
            HandleMovementInput();
            HandleMouseLock();

            AplyFinalMovements();
        }

        if (isHeadBobbing) 
        {
            HandleHeadBob();
        }

        if (useStamina)
        {
            HandleStamina();
        }
    }

    private void HandleMovementInput()
    {
        currentInput = new Vector2((isSprinting ? sprintSpeed : walkSpeed) * Input.GetAxis("Vertical"), 
        (isSprinting ? sprintSpeed : walkSpeed) * Input.GetAxis("Horizontal"));

        float moveDirectionY = moveDirection.y;
        moveDirection = (transform.TransformDirection(Vector3.forward) * currentInput.x)
        + (transform.TransformDirection(Vector3.right) * currentInput.y);
        
        moveDirection.y = moveDirectionY;

    }
    private void HandleMouseLock()
    {
        rotationX -= Input.GetAxis("Mouse Y") * lookSpeedY;
        rotationX = Mathf.Clamp(rotationX, -upLookLimit, downLookLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeedX, 0);
    }

    private void HandleHeadBob() 
    {
        if (!characterController.isGrounded) return;

        if (Mathf.Abs(moveDirection.x) > 0.1f || Mathf.Abs(moveDirection.z) > 0.1f) 
        {
            timer += Time.deltaTime * (isSprinting ? sprintBobSpeed : walkBobSpeed);
            playerCamera.transform.localPosition = new Vector3(
                playerCamera.transform.localPosition.x,
                defaultYPos + Mathf.Sin(timer) * (isSprinting ? sprintBobAmount : walkBobAmount),
                playerCamera.transform.localPosition.z
            );
        }
    }

    private void HandleStamina()
    {
        if (isSprinting && currentInput != Vector2.zero) // Vector2.zero for check if player  moves
        {
            if (regeneratingStamina != null)
            {
                StopCoroutine(regeneratingStamina);
                regeneratingStamina = null;
            }
            currentStamina -= staminaUseMultiplier * Time.deltaTime; // deltaTime for every second

            if (currentStamina < 0) 
            {
                currentStamina = 0;
            }

            OnStaminaChange?.Invoke(currentStamina);

            if (currentStamina <= 0)
            {
                canSprint = false;
            }
        }
        if (!isSprinting && currentStamina < maxStamina && regeneratingStamina == null)
        {
            regeneratingStamina = StartCoroutine(RegenerateStamina());
        }
    }

    private void AplyFinalMovements()
    {
        if (!characterController.isGrounded) 
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }
        characterController.Move(moveDirection * Time.deltaTime);
    }


    private IEnumerator RegenerateStamina()
    {
        yield return new WaitForSeconds(timeBeforeStaminaRegenStarts);
        WaitForSeconds timeToWait = new WaitForSeconds(staminaTimeIncrement);

        while (currentStamina < maxStamina)
        {
            if (currentStamina >= minStaminaToSprint)
            {
                canSprint = true;
            }
            currentStamina += staminaValueIncrement;

            if (currentStamina > maxStamina)
            {
                currentStamina = maxStamina;
            }

            OnStaminaChange?.Invoke(currentStamina);

            yield return timeToWait;
        }
        regeneratingStamina = null;
    }
}

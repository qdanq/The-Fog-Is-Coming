using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FirstPersonController : MonoBehaviour
{
    private float defaultYPos = 0;
    private float timer;


    private Camera playerCamera;
    private CharacterController characterController;

    private Vector3 moveDirection;
    private Vector2 currentInput;

    private float rotationX = 0;

    public bool CanMove { 
        get;
        private set;
        } = true;
    
    private bool isSprinting => canSprint && Input.GetKey(sprintKey) && Input.GetAxis("Vertical") > 0;
    private bool AbleCrouch => Input.GetKeyDown(crouchKey) && !duringCrouch 
        && characterController.isGrounded;

    [Header("Functional Options")]
    [SerializeField] private bool canSprint = true;
    [SerializeField] private bool canCrouch = true;
    [SerializeField] private bool isHeadBobbing = true;
    [SerializeField] private bool useStamina = true;
    [SerializeField] private bool canInteract = true;
    float moveMode;


    [Header("Controls")]
    //[SerializeField] private KeyCode moveForward = KeyCode.W;
    //[SerializeField] private KeyCode moveBackward = KeyCode.S;
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode crouchKey = KeyCode.LeftControl;
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    [Header("Movement Parameters")]
    [SerializeField] private float walkSpeed = 3.0f;
    [SerializeField] private float sprintSpeed = 6.0f;
    [SerializeField] private float crouchSpeed = 1.5f;
    [SerializeField] private float gravity = 30.0f; // MB make ragdolls instead of this construction

    [Header("Look Parameters")]
    [SerializeField, Range(1, 10)] private float lookSpeedX = 2.0f; // Must be accessible from options
    [SerializeField, Range(1, 10)] private float lookSpeedY = 2.0f; // Must be accessible from options
    [SerializeField, Range(1, 180)] private float upLookLimit = 80.0f;
    [SerializeField, Range(1, 180)] private float downLookLimit = 80.0f;

    [Header("Crouch Parameters")]
    private float standHeight;
    [SerializeField] private float crouchHeight = 0.5f;
    [SerializeField] private float timeToCrouch = 0.25f;
    [SerializeField] private Vector3 crouchCenter = new Vector3(0, 0.5f, 0);
    [SerializeField] private Vector3 standCenter = new Vector3(0, 0, 0);

    private bool isCrouching = false;
    private bool duringCrouch = false;

    [Header("Health Parameters")]
    [SerializeField] private float maxHealth = 100;
    [SerializeField] private float timeBeforeRegenStarts = 6;
    [SerializeField] private float healthValueIncrement = 1;
    [SerializeField] private float healthTimeIncrement = 0.1f;
    private float currentHealth = 100;
    private Coroutine regeneratingHealth;
    public static Action<float> OnTakeDamage;
    public static Action<float> OnDamage;
    public static Action<float> OnHeal;

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

    [Header("Interaction")]
    [SerializeField] private Vector3 interactionRayPoint = default;
    [SerializeField] private float interactionDist = default;
    [SerializeField] private LayerMask interactionLayer = default;
    private Interaction currentInteraction; 

    

    void Start()
    {
        playerCamera = GetComponentInChildren<Camera>();
        characterController = GetComponent<CharacterController>();        
        defaultYPos = playerCamera.transform.localPosition.y;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        standHeight = characterController.height;
    }

    private void OnEnable() {
        OnTakeDamage += ApplyDamage;
    }
    private void OnDisable() {
        OnTakeDamage -= ApplyDamage;
    }

    void Update()
    {
        if (CanMove) 
        {
            HandleMovementInput();
            HandleMouseLock();

        }

        if (isHeadBobbing) 
        {
            HandleHeadBob();
        }

        if (useStamina)
        {
            HandleStamina();
        }

        if (canCrouch)
        {
            HandleCrouch();
        }
        if (canInteract)
        {
            HandleInteractionCheck();
            HandleInteractionInput();
        }
        
        AplyFinalMovements();
    }

    private void HandleMovementInput()
    {
        moveMode = isCrouching ? crouchSpeed : isSprinting ? sprintSpeed : walkSpeed;

        currentInput = new Vector2(moveMode * Input.GetAxis("Vertical"), moveMode
         * Input.GetAxis("Horizontal"));

        float moveDirectionY = moveDirection.y;
        moveDirection = (transform.TransformDirection(Vector3.forward) * currentInput.x)
        + (transform.TransformDirection(Vector3.right) * currentInput.y);
        moveDirection = moveDirection.normalized * Mathf.Clamp(moveDirection.magnitude, 0, moveMode);
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

    private void HandleCrouch()
    {
        if (AbleCrouch)
            StartCoroutine(CrouchStand());
    }

    private void HandleInteractionCheck()
    {
        if (Physics.Raycast(playerCamera.ViewportPointToRay(interactionRayPoint), out RaycastHit hit, interactionDist))
        {
            if (hit.collider.gameObject.layer == 3 && currentInteraction == null 
            || hit.collider.gameObject.GetInstanceID() != currentInteraction.GetInstanceID())
            {
                hit.collider.TryGetComponent(out currentInteraction);

                if (currentInteraction)
                    currentInteraction.OnFocus();
            }
        }
        else if (currentInteraction)
        {
            currentInteraction.OnLoseFocus();
            currentInteraction = null;
        }
    }

    private void HandleInteractionInput()
    {
        if (Input.GetKeyDown(interactKey) && currentInteraction != null && Physics.Raycast(playerCamera.ViewportPointToRay(interactionRayPoint), out RaycastHit hit, interactionDist, interactionLayer))
        {
            currentInteraction.OnInteract();
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

    private void ApplyDamage(float dmg)
    {
        currentHealth -= dmg;
        OnDamage?.Invoke(currentHealth);
        if (currentHealth <= 0)
        {
            KillPlayer();
        } 
        else if (regeneratingHealth != null)
        {
            StopCoroutine(regeneratingHealth);
        }
        regeneratingHealth = StartCoroutine(RegenerateHealth());
    }

    private void KillPlayer()
    {
        currentHealth = 0;

        if (regeneratingHealth != null)
        {
            StopCoroutine(regeneratingHealth);
        }

        print("Dead");
        //gameObject.SetActive(false);
        
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

    private IEnumerator RegenerateHealth()
    {
        yield return new WaitForSeconds(timeBeforeRegenStarts);
        WaitForSeconds timeToWait = new WaitForSeconds(healthTimeIncrement);

        while (currentHealth < maxHealth)
        {
            currentHealth += healthValueIncrement;

            if (currentHealth > maxHealth)
            {
                currentHealth = maxHealth;
            }
            OnHeal?.Invoke(currentHealth);
            yield return timeToWait;
        }

        regeneratingHealth = null;
    }

    private IEnumerator CrouchStand()
    {
        if (isCrouching && Physics.Raycast(playerCamera.transform.position, Vector3.up, 2f)) // 2f is difference between crouchHeight and standHeight
        {
            yield break;
        }
        duringCrouch = true;

        float timeElapsed = 0;
        float targetHeight = isCrouching ? standHeight : crouchHeight;
        float currentHeight = characterController.height;
        Vector3 targetCenter = isCrouching ? standCenter : crouchCenter;
        Vector3 currentCenter = characterController.center;

        while (timeToCrouch > timeElapsed)
        {
            characterController.height = Mathf.Lerp(currentHeight,  targetHeight, timeElapsed/timeToCrouch);
            characterController.center = Vector3.Lerp(currentCenter, targetCenter, timeElapsed/timeToCrouch);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        characterController.height = targetHeight;
        characterController.center = targetCenter;
        
        isCrouching = !isCrouching;
        canSprint = !canSprint;

        duringCrouch = false;
    }
}

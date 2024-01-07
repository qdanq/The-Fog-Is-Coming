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
    [SerializeField] private float walkBobAmount = 0.5f;
    [SerializeField] private float sprintBobSpeed = 18f;
    [SerializeField] private float sprintBobAmount = 1f;
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
        
    }
    
    private void AplyFinalMovements()
    {
        if (!characterController.isGrounded) 
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }
        characterController.Move(moveDirection * Time.deltaTime);
    }

}

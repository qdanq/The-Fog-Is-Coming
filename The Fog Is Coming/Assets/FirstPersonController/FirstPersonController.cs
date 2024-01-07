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
    
    [Header("Movement Parameters")]
    [SerializeField] private float walkSpeed = 3.0f;
    [SerializeField] private float gravity = 30.0f; // MB make ragdolls instead of this construction

    [Header("Look Parameters")]
    [SerializeField, Range(1, 10)] private float lookSpeedX = 2.0f; // Must be accessible from options
    [SerializeField, Range(1, 10)] private float lookSpeedY = 2.0f; // Must be accessible from options
    [SerializeField, Range(1, 180)] private float upLookLimit = 80.0f;
    [SerializeField, Range(1, 180)] private float downLookLimit = 80.0f;

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
    }

    private void HandleMovementInput()
    {
        currentInput = new Vector2(walkSpeed * Input.GetAxis("Vertical"), 
        walkSpeed * Input.GetAxis("Horizontal"));

        float moveDirectionY = moveDirection.y;
        moveDirection = (transform.TransformDirection(Vector3.forward) * currentInput.x)
        + (transform.TransformDirection(Vector3.right) * currentInput.y);
        
        moveDirection.y = moveDirectionY;

    }
    private void HandleMouseLock()
    {
        
    }
    private void AplyFinalMovements()
    {
        
    }
}

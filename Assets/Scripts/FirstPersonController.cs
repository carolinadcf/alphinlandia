
using UnityEngine;
using System;
using System.Collections;

public class FirstPersonController : MonoBehaviour
{
    public bool CanMove { get; private set; } = true;

    [Header("Functional Options")]
    [SerializeField] private bool canInteract = true;
    [SerializeField] private bool useStamina = true;

    [Header("Controls")]
    [SerializeField] private KeyCode interactKey = KeyCode.Mouse0; // left mouse button

    [Header("Movement Parameters")]
    [SerializeField] private float walkSpeed = 3.0f;
    [SerializeField] private float gravity = 30.0f;

    [Header("Look Parameters")]
    [SerializeField, Range(1, 10)] private float lookSpeedX = 2.0f;
    [SerializeField, Range(1, 10)] private float lookSpeedY = 2.0f;
    [SerializeField, Range(1, 180)] private float upperLookLimit = 80.0f;
    [SerializeField, Range(1, 180)] private float lowerLookLimit = 80.0f;

    [Header("Stamina Parameters")]
    [SerializeField] private float maxStamina = 100;
    [SerializeField] private float staminaUseMultiplier = 5;
    [SerializeField] private float timeBeforeStaminaRegenStarts = 5; // seconds
    [SerializeField] private float staminaValueIncrement = 2;
    [SerializeField] private float staminaTimeIncrement = 0.1f;
    private float currentStamina;
    private Coroutine regeneratingStamina;
    public static Action<float> OnStaminaChange;

    [Header("Interaction")]
    [SerializeField] private Vector3 interactionRayPoint = default;
    [SerializeField] private float interactionDistance = default;
    [SerializeField] private LayerMask interactionLayer = default;
    private Interactable currentInteractable;

    private Camera playerCamera;
    private CharacterController characterController;

    private Vector3 moveDirection;
    private Vector2 currentInput;

    private float rotationX = 0;

    void Awake()
    {
        playerCamera = GetComponentInChildren<Camera>();
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;       
        
        currentStamina = maxStamina;
    }

    // Update is called once per frame
    void Update()
    {
        if (CanMove) {
            HandleMovementInput();
            HandleMouseLook();

            if (canInteract)
            {
                HandleInteractionCheck();
                HandleInteractionInput();
            }

            if(useStamina) {
                HandleStamina();
            }

            ApplyFinalMovements();
        }
    }

    private void HandleMovementInput()
    {
        currentInput = new Vector2(walkSpeed * Input.GetAxis("Vertical"), walkSpeed * Input.GetAxis("Horizontal"));

        float moveDirectionY = moveDirection.y;
        moveDirection = (transform.TransformDirection(Vector3.forward) * currentInput.x) + (transform.TransformDirection(Vector3.right) * currentInput.y);
        moveDirection.y = moveDirectionY;
    }

    private void HandleMouseLook()
    {
        rotationX -= Input.GetAxis("Mouse Y") * lookSpeedY;
        rotationX = Mathf.Clamp(rotationX, -upperLookLimit, lowerLookLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeedX, 0);
    }

    private void HandleInteractionCheck()
    {
        if(Physics.Raycast(playerCamera.ViewportPointToRay(interactionRayPoint), out RaycastHit hit, interactionDistance))
        {
            if(hit.collider.gameObject.layer == 6 && (currentInteractable == null || hit.collider.gameObject.GetInstanceID() != currentInteractable.gameObject.GetInstanceID()))
            {
                hit.collider.TryGetComponent(out currentInteractable);

                if (currentInteractable)
                {
                    currentInteractable.OnFocus();
                }
            }
        }
        else if (currentInteractable)
        {
            currentInteractable.OnLoseFocus();
            currentInteractable = null;
        }
    }

    private void HandleInteractionInput()
    {
        if(Input.GetKeyDown(interactKey) && currentInteractable != null && Physics.Raycast(playerCamera.ViewportPointToRay(interactionRayPoint), out RaycastHit hit, interactionDistance, interactionLayer))
        {
            currentInteractable.OnInteract();
        } 
    }  

    private void HandleStamina()
    {
        // moving
        if (currentInput != Vector2.zero)
        {
            if (regeneratingStamina != null)
            {
                StopCoroutine(regeneratingStamina);
                regeneratingStamina = null;
            }

            currentStamina -= staminaUseMultiplier * Time.deltaTime;
            
            if (currentStamina < 0)
            {
                currentStamina = 0;
            }
            
            if (currentStamina < 50) 
                walkSpeed = 1.0f;

            OnStaminaChange?.Invoke(currentStamina);
        }

        if (currentStamina < maxStamina && regeneratingStamina == null)
        {
            regeneratingStamina = StartCoroutine(RegenerateStamina());
        }
    }

    private void ApplyFinalMovements()
    {
        if (!characterController.isGrounded)
            moveDirection.y -= gravity * Time.deltaTime;

        characterController.Move(moveDirection * Time.deltaTime);
    }

    private IEnumerator RegenerateStamina()
    {
        yield return new WaitForSeconds(timeBeforeStaminaRegenStarts);

        WaitForSeconds timeToWait = new WaitForSeconds(staminaTimeIncrement);

        while (currentStamina < maxStamina)
        {
            currentStamina += staminaValueIncrement;

            if (currentStamina > maxStamina)
            {
                currentStamina = maxStamina;
                walkSpeed = 3.0f;
            }

            OnStaminaChange?.Invoke(currentStamina);
            
            yield return timeToWait;
        }

        regeneratingStamina = null;

    }
}

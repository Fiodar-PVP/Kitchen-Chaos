using System;
using UnityEngine;

public class Player : MonoBehaviour, IKitchenObjectParent
{
    public static Player Instance { get; private set; }

    public event EventHandler<OnSelectedCounterChangedEventArg> OnSelectedCounterChanged;
    public class OnSelectedCounterChangedEventArg : EventArgs
    {
        public BaseCounter selectedCounter;
    }

    [Header("Movement variables")]
    [SerializeField] private float playerSize = 0.7f;
    [SerializeField] private float playerHeight = 2f;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotateSpeed = 10f;

    [Header("Interaction variables")]
    [SerializeField] private float interactionDistance = 2f;
    [SerializeField] private LayerMask countersLayerMask;

    [SerializeField] private GameInput gameInput;
    [SerializeField] private Transform kitchenObjectHoldPoint;

    private Vector3 lastInteractionDirection;
    private BaseCounter selectedCounter;
    private KitchenObject kitchenObject;

    public bool IsWalking { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There are more than one Player Instance!");
        }

        Instance = this;
    }

    private void Start()
    {
        gameInput.OnInteractAction += GameInput_OnInteractAction;
    }

    private void Update()
    {
        HandleMovement();
        HandleInteraction();
    }

    /// <summary>
    /// Handles the interaction action triggered by the player. If a counter is currently selected, 
    /// it calls the Interact method on the selected counter.
    /// </summary>
    private void GameInput_OnInteractAction(object sender, EventArgs e)
    {
        if (selectedCounter != null)
        {
            selectedCounter.Interact(this);
        }
    }

    /// <summary>
    /// Handles player interactions with nearby objects by performing a Raycast in the direction of movement.
    /// If the player is moving, updates the last interaction direction. If the Raycast hits an interactable
    /// object, triggers interaction with that object.
    /// </summary>
    private void HandleInteraction()
    {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();

        Vector3 moveDirection = new Vector3(inputVector.x, 0f, inputVector.y);

        if(moveDirection != Vector3.zero)
        {
            lastInteractionDirection = moveDirection;
        }

        if (Physics.Raycast(transform.position, lastInteractionDirection, out RaycastHit raycastHit, interactionDistance, countersLayerMask))
        {
            if (raycastHit.transform.TryGetComponent(out BaseCounter baseCounter))
            {
                //Raycast hit object with attached BaseCounter component
                if(baseCounter != selectedCounter)
                {
                    SetSelectedCounter(baseCounter);
                }
            }
            else
            {
                //Raycast hit something but it's not type of BaseCounter
                SetSelectedCounter(null);
            }
        }
        else
        {
            //Raycast did not hit anything
            SetSelectedCounter(null);
        }
    }

    /// <summary>
    /// Sets the currently selected counter and triggers the OnSelectedCounterChanged event to notify listeners
    /// that the selected counter has changed.
    /// </summary>
    /// <param name="selectedCounter">The counter to be set as the currently selected counter.</param>
    private void SetSelectedCounter(BaseCounter selectedCounter)
    {
        this.selectedCounter = selectedCounter;

        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArg { selectedCounter = selectedCounter });
    }

    /// <summary>
    /// Handles the player's movement by checking input direction, determining if movement is possible,
    /// and moving the player accordingly. If movement in the initial direction is blocked,
    /// attempts are made to move along the X or Z axis individually.
    /// </summary>
    private void HandleMovement()
    {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();

        Vector3 moveDirection = new Vector3(inputVector.x, 0f, inputVector.y);

        float moveDistance = moveSpeed * Time.deltaTime;

        bool canMove = CheckIfCanMoveInDirection(moveDistance, moveDirection);

        //If cannot move towards moveDirection
        if (!canMove)
        {
            //Attempt only X movement
            Vector3 moveDirectionX = new Vector3(moveDirection.x, 0, 0).normalized;
            canMove = moveDirection.x != 0 && CheckIfCanMoveInDirection(moveDistance, moveDirectionX);

            if (canMove)
                //Can move only on X
                moveDirection = moveDirectionX;
            else
            {
                //Cannot move only on X

                //Attempt only Z movement
                Vector3 moveDirectionZ = new Vector3(0, 0, moveDirection.z).normalized;
                canMove = moveDirection.z != 0 && CheckIfCanMoveInDirection(moveDistance, moveDirectionZ);

                if (canMove)
                    //Can move only on Z
                    moveDirection = moveDirectionZ;
                else
                {
                    //Cannot move in any direction
                }
            }
        }

        if (canMove)
            transform.position += moveDirection * moveDistance;

        IsWalking = moveDirection != Vector3.zero;

        //Rotates the player to match the current moveDirection
        if (moveDirection != Vector3.zero)
            transform.forward = Vector3.Slerp(transform.forward, moveDirection, rotateSpeed * Time.deltaTime);

    }

    /// <summary>
    /// Checks if the player can move in the given direction by performing a CapsuleCast.
    /// </summary>
    /// <param name="moveDistance">The distance the player intends to move in the given direction.</param>
    /// <param name="moveDirection">The direction the player intends to move in.</param>
    /// <returns>
    /// Returns true if there are no obstacles within the given move distance in the specified direction;
    /// otherwise, returns false.
    /// </returns>
    private bool CheckIfCanMoveInDirection(float moveDistance, Vector3 moveDirection)
    {
        return !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerSize, moveDirection, moveDistance);
    }

    public Transform GetKitchenObjectFollowTransform()
    {
        return kitchenObjectHoldPoint;
    }

    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        this.kitchenObject = kitchenObject;
    }

    public KitchenObject GetKitchenObject()
    {
        return kitchenObject;
    }

    public void ClearKitchenObject()
    {
        kitchenObject = null;
    }

    public bool HasKitchenObject()
    {
        return kitchenObject != null;
    }
}

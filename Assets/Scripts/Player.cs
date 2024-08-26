using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Movement variables")]
    [SerializeField] private float playerSize = 0.7f;
    [SerializeField] private float playerHeight = 2f;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotateSpeed = 10f;

    [Header("Interaction variables")]
    [SerializeField] private float interactionDistance = 2f;
    [SerializeField] private LayerMask countersLayerMask;

    [SerializeField] private GameInput gameInput;

    private Vector3 lastInteractionDirection;

    public bool IsWalking { get; private set; }

    private void Update()
    {
        HandleMovement();
        HandleInteraction();
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
            if (raycastHit.transform.TryGetComponent(out ClearCounter clearCounter))
            {
                clearCounter.Interact();
            }
        }
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
            canMove = CheckIfCanMoveInDirection(moveDistance, moveDirectionX);

            if (canMove)
                //Can move only on X
                moveDirection = moveDirectionX;
            else
            {
                //Cannot move only on X

                //Attempt only Z movement
                Vector3 moveDirectionZ = new Vector3(0, 0, moveDirection.z).normalized;
                canMove = CheckIfCanMoveInDirection(moveDistance, moveDirectionZ);

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
}

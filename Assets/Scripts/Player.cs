using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float playerSize = 0.7f;
    [SerializeField] private float playerHeight = 2f;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotateSpeed = 10f;
    [SerializeField] private GameInput gameInput;

    public bool IsWalking { get; private set; }

    private void Update()
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

        //Rotates the player to match the current moveDirection
        if(moveDirection != Vector3.zero)
            transform.forward = Vector3.Slerp(transform.forward, moveDirection, rotateSpeed * Time.deltaTime);
        
        IsWalking = moveDirection != Vector3.zero;
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

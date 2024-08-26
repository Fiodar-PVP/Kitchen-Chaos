using UnityEngine;

public class GameInput : MonoBehaviour
{
    private PlayerInputActions playerInputActions;

    private void Awake()
    {
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable(); 
    }

    /// <summary>
    /// Retrieves the player's movement input as a normalized <see cref="Vector2"/>.
    /// </summary>
    /// <returns>
    /// A <see cref="Vector2"/> representing the normalized movement input from the player.
    /// The returned vector will have a magnitude of 1 if there is movement input, or 0 if no input is detected.
    /// </returns>
    public Vector2 GetMovementVectorNormalized()
    {
        Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();

        inputVector = inputVector.normalized;

        return inputVector;
    }
}

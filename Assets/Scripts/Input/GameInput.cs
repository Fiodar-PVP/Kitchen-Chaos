using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    private const string PLAYER_PREFS_BINDINGS = "INPUT_BINDINGS";
    public static GameInput Instance { get; private set; }

    public event EventHandler OnInteractAction;
    public event EventHandler OnInteractAlternateAction;
    public event EventHandler OnPauseAction;

    public enum Binding
    {
        MoveUp,
        MoveDown,
        MoveLeft,
        MoveRight,
        Interact,
        InteractAlternate,
        Pause,
        Gamepad_Interact,
        Gamepad_InteractAlternate,
        Gamepad_Pause
    }

    private PlayerInputActions playerInputActions;

    private void Awake()
    {
        Instance = this;

        playerInputActions = new PlayerInputActions();
        if(PlayerPrefs.HasKey(PLAYER_PREFS_BINDINGS))
        {
            playerInputActions.LoadBindingOverridesFromJson(PlayerPrefs.GetString(PLAYER_PREFS_BINDINGS));
        }

        playerInputActions.Player.Enable();

        playerInputActions.Player.Interact.performed += Interact_performed;
        playerInputActions.Player.InteractAlternate.performed += InteractAlternate_performed;
        playerInputActions.Player.Pause.performed += Pause_performed;
    }

    private void OnDestroy()
    {
        playerInputActions.Player.Interact.performed -= Interact_performed;
        playerInputActions.Player.InteractAlternate.performed -= InteractAlternate_performed;
        playerInputActions.Player.Pause.performed -= Pause_performed;

        playerInputActions.Dispose();
    }

    private void Pause_performed(InputAction.CallbackContext obj)
    {
        OnPauseAction?.Invoke(this, EventArgs.Empty);
    }

    private void Interact_performed(InputAction.CallbackContext obj)
    {
        OnInteractAction?.Invoke(this, EventArgs.Empty);
    }

    private void InteractAlternate_performed(InputAction.CallbackContext obj)
    {
        OnInteractAlternateAction?.Invoke(this, EventArgs.Empty);
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

    public void RebindBinding(Binding binding, Action OnActionRebound)
    {
        playerInputActions.Player.Disable();

        InputAction inputAction;
        int bindingIndex;

        switch (binding)
        {
            default:
            case Binding.MoveUp:
                {
                    inputAction = playerInputActions.Player.Move;
                    bindingIndex = 1;
                    break;
                }
            case Binding.MoveDown:
                {
                    inputAction = playerInputActions.Player.Move;
                    bindingIndex = 2;
                    break;
                }
            case Binding.MoveLeft:
                {
                    inputAction = playerInputActions.Player.Move;
                    bindingIndex = 3;
                    break;
                }
            case Binding.MoveRight:
                {
                    inputAction = playerInputActions.Player.Move;
                    bindingIndex = 4;
                    break;
                }
            case Binding.Interact:
                {
                    inputAction = playerInputActions.Player.Interact;
                    bindingIndex = 0;
                    break;
                }
            case Binding.InteractAlternate:
                {
                    inputAction = playerInputActions.Player.InteractAlternate;
                    bindingIndex = 0;
                    break;
                }
            case Binding.Pause:
                {
                    inputAction = playerInputActions.Player.Pause;
                    bindingIndex = 0;
                    break;
                }
            case Binding.Gamepad_Interact:
                {
                    inputAction = playerInputActions.Player.Interact;
                    bindingIndex = 1;
                    break;
                }
            case Binding.Gamepad_InteractAlternate:
                {
                    inputAction = playerInputActions.Player.Pause;
                    bindingIndex = 1;
                    break;
                }
            case Binding.Gamepad_Pause:
                {
                    inputAction = playerInputActions.Player.Pause;
                    bindingIndex = 1;
                    break;
                }
        }

        inputAction.PerformInteractiveRebinding(bindingIndex)
            .OnComplete(callback =>
                {
                    callback.Dispose();
                    OnActionRebound();
                    playerInputActions.Player.Enable();
                    
                    PlayerPrefs.SetString(PLAYER_PREFS_BINDINGS, playerInputActions.SaveBindingOverridesAsJson());
                    PlayerPrefs.Save();
                })
            .Start();

    }

    public string GetBindingText(Binding binding)
    {
        switch (binding)
        {
            default:
            case Binding.MoveUp:
                return playerInputActions.Player.Move.bindings[1].ToDisplayString();
            case Binding.MoveDown:
                return playerInputActions.Player.Move.bindings[2].ToDisplayString();
            case Binding.MoveLeft:
                return playerInputActions.Player.Move.bindings[3].ToDisplayString();
            case Binding.MoveRight:
                return playerInputActions.Player.Move.bindings[4].ToDisplayString();
            case Binding.Interact:
                return playerInputActions.Player.Interact.bindings[0].ToDisplayString();
            case Binding.InteractAlternate:
                return playerInputActions.Player.InteractAlternate.bindings[0].ToDisplayString();
            case Binding.Pause:
                return playerInputActions.Player.Pause.bindings[0].ToDisplayString();
            case Binding.Gamepad_Interact:
                return playerInputActions.Player.Interact.bindings[1].ToDisplayString();
            case Binding.Gamepad_InteractAlternate:
                return playerInputActions.Player.InteractAlternate.bindings[1].ToDisplayString();
            case Binding.Gamepad_Pause:
                return playerInputActions.Player.Pause.bindings[1].ToDisplayString();
        }
    }
}

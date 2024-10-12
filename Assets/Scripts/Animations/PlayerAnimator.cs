using Unity.Netcode;
using UnityEngine;

public class PlayerAnimator : NetworkBehaviour
{
    private const string IS_WALKING = "IsWalking";

    private Animator playerAnimator;
    private Player player;

    private void Awake()
    {
        player = GetComponentInParent<Player>();

        playerAnimator = GetComponent<Animator>();
        playerAnimator.SetBool(IS_WALKING, false);
    }
    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;

        playerAnimator.SetBool(IS_WALKING, player.IsWalking);   
    }
}

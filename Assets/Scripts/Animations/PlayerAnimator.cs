using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
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
        playerAnimator.SetBool(IS_WALKING, player.IsWalking);   
    }
}

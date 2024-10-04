using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStartCountdownUIAnimator : MonoBehaviour
{
    private static string IS_COUNTDOWN = "NumberPopup";
    private Animator animator;

    private int previousCountDownNumber;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        int countdownNumber = Mathf.CeilToInt(KitchenGameManager.Instance.GetCountdownToStartTimer());
        
        if(previousCountDownNumber != countdownNumber)
        {
            previousCountDownNumber = countdownNumber;
            animator.SetTrigger(IS_COUNTDOWN);

            SoundManager.Instance.PlayCountdownSound();
        }
    }
}

using UnityEditor;
using UnityEngine;

public class StoveBurnFlashingBarAnimator : MonoBehaviour
{
    private const string IS_FLASHING = "IsFlashing";

    [SerializeField] private StoveCounter stoveCounter;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        stoveCounter.OnProgressChanged += StoveCounter_OnProgressChanged;

        animator.SetBool(IS_FLASHING, false);
    }

    private void StoveCounter_OnProgressChanged(object sender, IHasProgress.OnProgressChangedEventArgs e)
    {
        float burnFlashProgressAmount = 0.5f;
        bool flash = stoveCounter.IsFried() && e.progressNormalized > burnFlashProgressAmount;

        if (flash)
        {
            animator.SetBool(IS_FLASHING, flash);    
        }
    }
}
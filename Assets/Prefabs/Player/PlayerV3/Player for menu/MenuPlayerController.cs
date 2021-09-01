using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuPlayerController : MonoBehaviour
{
    public int numberOfUpgradeAnimations;
    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void PlayRandomUpdateAnimation()
    {
        if (animator != null)
        {
            animator.SetBool("is idle", false);
            animator.SetFloat("upgrade anim index", Random.Range(0, numberOfUpgradeAnimations));
        }
    }
}

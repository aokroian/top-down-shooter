using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Security.Cryptography;

public class MenuPlayerController : MonoBehaviour
{
    public int maxAnimIndex;
    private Animator animator;
    private int lastRandom;

    private bool animationBlock = false;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        lastRandom = Random.Range(0, maxAnimIndex + 1);

        if (animator != null)
        {
            animator.SetBool("is idle", true);
        }
    }

    public void PlayRandomUpdateAnimation()
    {
        if (animator != null)
        {
            int random = Random.Range(0, maxAnimIndex + 1);

            while(random == lastRandom)
            {
                random = Random.Range(0, maxAnimIndex + 1);
            }
            animator.SetInteger("upgrade anim index", random);
            lastRandom = random;

            if (!animationBlock)
            {          
                animator.SetBool("is idle", false);
                animationBlock = true;
                Invoke(nameof(EnableIdleAnimation), 0.2f);
            } 
        }
    }

    private void EnableIdleAnimation()
    {
        animator.SetBool("is idle", true);
        animationBlock = false;
    }
}

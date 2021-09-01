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
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        lastRandom = Random.Range(0, maxAnimIndex);
    }



    public void PlayRandomUpdateAnimation()
    {
        if (animator != null)
        {
            int random = Random.Range(0, maxAnimIndex);

            while(random == lastRandom)
            {
                random = Random.Range(0, maxAnimIndex);
            }
            animator.SetInteger("upgrade anim index", random);
            lastRandom = random;

            animator.SetBool("is idle", false);
            Invoke(nameof(EnableIdleAnimation), 0.2f);
        }
    }

    private void EnableIdleAnimation()
    {
        animator.SetBool("is idle", true);
    }
}

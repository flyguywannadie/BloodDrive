using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableAnimatorOnEnd : MonoBehaviour
{
    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        animator.enabled = false;
    }

	public void DisableAnimator()
	{
        animator.enabled = false;
	}
}

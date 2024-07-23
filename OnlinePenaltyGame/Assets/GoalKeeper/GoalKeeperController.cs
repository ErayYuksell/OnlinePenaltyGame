using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalKeeperController : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] AnimationClip bodyBlockAnim;
    private Vector3 finalPosition;
    private Quaternion finalRotation;
    private bool animationFinished = false;

    private void Update()
    {
        if (animationFinished)
        {
            transform.position = finalPosition;
            transform.rotation = finalRotation; 
        }
    }
    public void PlayBodyBlock()
    {
        animator.Play(bodyBlockAnim.name);
    }

    public void OnAnimationComplete()
    {
        finalPosition = transform.position;
        finalRotation = transform.rotation;
        animationFinished = true;
    }
}

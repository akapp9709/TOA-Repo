using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class LookAtTarget : StateMachineBehaviour
{
    public float lookTime;
    public float moveSpeed, stepDistance, interactingDelay;
    private float _time;
    private Transform enemy, parent;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _time = 0;
        parent = animator.transform.parent;
        enemy = parent.GetComponent<AttackHandler2>().GetTargetEnemy();

        if (enemy == null)
        {
            return;
        }

        parent.DOLookAt(enemy.position, lookTime);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _time += Time.deltaTime;
        if (_time >= interactingDelay)
        {
            animator.SetBool("IsInteracting", true);
        }

        if (enemy == null)
        {
            return;
        }


        var dir = enemy.position - parent.position;
        var rigidBody = parent.GetComponent<Rigidbody>();

        if (Vector3.Distance(enemy.position, parent.position) > stepDistance)
            rigidBody.velocity = dir * moveSpeed;
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("IsInteracting", false);
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}

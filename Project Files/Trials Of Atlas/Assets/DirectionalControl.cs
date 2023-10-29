using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class DirectionalControl : StateMachineBehaviour
{
    public float tweenTime;
    public float stoppingDistance;
    public float interactingDelay;
    private float _time;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var pos = animator.transform.parent.position;
        var target = animator.GetComponentInParent<AttackHandler2>().GetTargetEnemy();
        if (target == null)
        {
            return;
        }

        var dir = target.position - pos;
        dir.Normalize();
        var dist = Vector3.Distance(pos, target.position);
        animator.transform.parent.DOMove(pos + dir * (dist - stoppingDistance), tweenTime, false);
        animator.transform.parent.DOLookAt(pos + dir, tweenTime / 3);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _time += Time.deltaTime;
        if (_time >= interactingDelay)
        {
            animator.SetBool("IsInteracting", true);
        }
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

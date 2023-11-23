using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

public class BruteJumpHandler : StateMachineBehaviour
{
    public float moveSpeed = 6f;
    public float moveDelay = 0.2f, endOffset;
    private Transform _transform;
    private Vector3 targetPosition;
    private Timer _timer;
    private AnimatorStateInfo StateInfo;
    private BruteBehavior _brute;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(targetPosition, 1);
    }

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponentInParent<NavMeshAgent>().enabled = false;
        _brute = animator.GetComponentInParent<BruteBehavior>();

        StateInfo = stateInfo;
        _transform = _brute.transform;

        targetPosition = SetTargetPosition(animator);
        Debug.Log("Player Position: " + targetPosition);
        _transform.DOLookAt(targetPosition, moveDelay);
        _timer = new Timer(moveDelay, MoveToward, (float x) =>
        {
            targetPosition = SetTargetPosition(animator);
            _brute.SetDecalPosition(targetPosition);
        });
        _brute.ActivateDecal(targetPosition);
    }

    private Vector3 SetTargetPosition(Animator anim)
    {
        var targetPos = _brute.GetTargetPosition();
        if (targetPos == Vector3.zero)
        {
            return _transform.position;
        }
        else
        {
            return targetPos;
        }
    }

    private void MoveToward()
    {
        Debug.Log("Jumping forward");
        _brute.DecoupleDecal();
        _transform.DOMove(targetPosition, StateInfo.length - moveDelay - endOffset);
        _timer = null;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _timer.Tick(Time.deltaTime);
        _brute.SetDecalPosition(targetPosition);
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponentInParent<NavMeshAgent>().enabled = true;
        _brute.DeactivateDecal();
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

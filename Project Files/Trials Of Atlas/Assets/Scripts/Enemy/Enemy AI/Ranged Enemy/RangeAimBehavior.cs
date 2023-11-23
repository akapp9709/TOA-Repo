using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeAimBehavior : StateMachineBehaviour
{
    private Timer _timer, _timer2;
    public float aimTime, fireDelay;
    private RangedBehavior _range;
    private Transform _player, _trans;
    private bool _followPlayer = true;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _followPlayer = true;
        animator.SetBool("isAiming", true);
        _range = animator.GetComponentInParent<RangedBehavior>();
        _trans = _range.transform;
        _timer = new Timer(aimTime, () =>
        {
            _followPlayer = false;
        });
        _timer2 = new Timer(aimTime + fireDelay, () =>
        {
            animator.SetBool("isAiming", false);
        });
        _player = GameObject.FindGameObjectWithTag("Player").transform;

        _range.ActivateAimingLine();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _timer.Tick(Time.deltaTime);
        _timer2.Tick(Time.deltaTime);

        if (!_followPlayer)
            return;

        var targetPos = _player.position;
        var targetDir = targetPos - _trans.position;
        targetDir.y = 0f;
        var targetRot = Quaternion.LookRotation(targetDir);

        _trans.rotation = Quaternion.Slerp(_trans.rotation, targetRot, 0.1f);
        _range.SetAimingLineDirection();

    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _timer = null;
        _timer2 = null;
        _range.DeactivateAimingLine();
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

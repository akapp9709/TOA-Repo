using System.Collections;
using System.Collections.Generic;
using AJK;
using UnityEngine;
using UnityEngine.InputSystem;

public class AttackHandler : MonoBehaviour
{
    [SerializeField] private Transform _cameraTransform;

    public int attackVal;
    public bool isAttacking;
    private AnimationHandler animHandler;
    private PlayerControls _controls;
    private InputHandler _input;
    [SerializeField] private Animator anim;
    public static AttackHandler singleton;

    private void Start()
    {
        singleton = this;
        animHandler = GetComponentInChildren<AnimationHandler>();
        _controls = new PlayerControls();
        _controls.PlayerActions.Enable();

        _controls.PlayerActions.MainAttack.started += Attack;

        _input = GetComponent<InputHandler>();
    }

    private void LateUpdate()
    {
        if (isAttacking)
        {
            var targetDirection = _cameraTransform.forward * _input.vertical;
            targetDirection += _cameraTransform.right * _input.horizontal;

            var targetRot = Quaternion.LookRotation(targetDirection);
            transform.rotation = targetRot;
        }
    }

    private void Attack(InputAction.CallbackContext context)
    {
        isAttacking = anim.GetBool("isAttacking");
        attackVal++;
        attackVal = Mathf.Clamp(attackVal, 0, 3);
        if (attackVal == 1)
        {
            animHandler.TriggerTargetAnimation("Attack", true);
            anim.SetInteger("Attack Index", attackVal);
        }
        else if (isAttacking)
        {
            anim.SetInteger("Attack Index", attackVal);
        }
    }
}

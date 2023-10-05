using System;
using System.Collections;
using System.Collections.Generic;
using AJK;
using UnityEngine;
using UnityEngine.InputSystem;

public class AttackHandler : MonoBehaviour
{
    [SerializeField] private Transform _cameraTransform;
    public float attackRadius, minimumStepDistance = 1f;
    private string enemyTag = "Enemy";
    private Transform _closestEnemy;
    private float _distanceTo;

    public int attackVal;
    public float angle;
    public float rotationSpeed;
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

    private void Update()
    {
        isAttacking = anim.GetBool("isAttacking");

        FindClosestEnemy();
        if (_closestEnemy != null)
        {
            Debug.DrawLine(transform.position + Vector3.up, _closestEnemy.position + Vector3.up);
            if (isAttacking && !animHandler.rootMotion)
            {
                RotateTowardsEnemy();
                if (_distanceTo > minimumStepDistance)
                {
                    GetComponent<Rigidbody>().velocity = (_closestEnemy.position - transform.position).normalized * 6f;
                }
            }
        }

    }

    private void Attack(InputAction.CallbackContext context)
    {

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

    private void FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        float closestDistance = Mathf.Infinity;
        Transform newClosestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);

            if (distanceToEnemy < closestDistance)
            {
                closestDistance = distanceToEnemy;
                newClosestEnemy = enemy.transform;
            }
        }

        if (closestDistance > attackRadius)
        {
            _closestEnemy = null;
            return;
        }

        _closestEnemy = newClosestEnemy;
        _distanceTo = closestDistance;
    }

    private void RotateTowardsEnemy()
    {
        if (_closestEnemy != null)
        {
            Vector3 directionToEnemy = _closestEnemy.position - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(directionToEnemy);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }
}

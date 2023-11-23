using System.Collections;
using System.Collections.Generic;
using AJK;
using UnityEngine;
using UnityEngine.InputSystem;

public class AttackHandler2 : MonoBehaviour
{
    private PlayerInputCLass _input;
    private Animator _anim;
    public PlayerSO playerStats;
    private HitBox _hitBox;
    public Transform cameraTransform;
    private float inX, inY;
    private Vector3 _inputDirection;
    private Transform targetEnemy;
    private int _attackInd = 0;
    private float _lastAttackTime;
    public float maxDelay;
    private Vector2 _inVec;

    private void OnDrawGizmos()
    {
        var pos = transform.position;

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(pos + Vector3.up * 2.3f, pos + _inputDirection + Vector3.up * 2.3f);
        if (targetEnemy != null)
            Gizmos.DrawSphere(targetEnemy.position + Vector3.up * 2.3f, 0.5f);
        Gizmos.DrawWireSphere(pos, playerStats.attackRange);
    }

    private void Awake()
    {
        _anim = null;
    }

    // Start is called before the first frame update
    void Start()
    {
        _anim = GetComponentInChildren<Animator>();

        _input = GetComponentInChildren<PlayerManager>().inputHandler;
        _input.AttackAction = Attack;
    }

    // Update is called once per frame
    void Update()
    {
        inX = _input.inputVector.x;
        inY = _input.inputVector.z;

        _inputDirection = inX * cameraTransform.right + inY * cameraTransform.forward;
        _inputDirection.y = 0;

        if (_anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.7f && _anim.GetCurrentAnimatorStateInfo(0).IsName("Attack1"))
            _anim.SetBool("Attack1", false);
        if (_anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.7f && _anim.GetCurrentAnimatorStateInfo(0).IsName("Attack2"))
            _anim.SetBool("Attack2", false);
        if (_anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.7f && _anim.GetCurrentAnimatorStateInfo(0).IsName("Attack3"))
        {
            _anim.SetBool("Attack3", false);
            _attackInd = 0;
        }

        if (Time.time - _lastAttackTime > maxDelay)
        {
            _attackInd = 0;
        }
    }

    public Transform GetTargetEnemy()
    {
        if (Mathf.Clamp01(Mathf.Abs(_inVec.x) + Mathf.Abs(_inVec.y)) == 0)
        {
            _inputDirection = cameraTransform.forward;
        }

        var arr = FindObjectsOfType<Enemy>();
        var list = new List<Enemy>(arr);
        Enemy target = null;

        float maxDot = -1f;
        float minDist = Mathf.Infinity;

        foreach (var enemy in list)
        {
            var dist = Vector3.Distance(transform.position, enemy.transform.position);
            var dot = Vector3.Dot(_inputDirection, enemy.transform.position - transform.position);
            var hit = new RaycastHit();
            var ray = Physics.Raycast(transform.position + Vector3.up,
                                    (enemy.transform.position - transform.position).normalized,
                                    out hit);

            if ((dot > maxDot) && dist < playerStats.attackRange
                    && hit.collider.tag == enemy.tag)
            {
                maxDot = dot;
                minDist = dist;
                target = enemy;
            }
        }

        if (target == null)
        {
            return null;
        }

        targetEnemy = target.transform;
        return target.transform;
    }

    private void Attack()
    {
        _lastAttackTime = Time.time;
        _attackInd++;
        if (_attackInd == 1)
        {
            _anim.SetTrigger("Attack");
            _anim.SetBool("Attack1", true);
        }

        _attackInd = Mathf.Clamp(_attackInd, 0, 3);

        if (_attackInd >= 2 && _anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.7f && _anim.GetCurrentAnimatorStateInfo(0).IsName("Attack1"))
        {
            _anim.SetBool("Attack1", false);
            _anim.SetBool("Attack2", true);
        }
        if (_attackInd >= 3 && _anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.7f && _anim.GetCurrentAnimatorStateInfo(0).IsName("Attack2"))
        {
            _anim.SetBool("Attack2", false);
            _anim.SetBool("Attack3", true);
        }
    }

    private void SetAttack()
    {
        _anim.SetBool("isAttacking", true);
    }
}

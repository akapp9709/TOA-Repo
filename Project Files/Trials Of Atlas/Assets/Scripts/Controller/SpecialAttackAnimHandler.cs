using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting.ReorderableList;
using UnityEngine;

public class SpecialAttackAnimHandler : MonoBehaviour
{
    private SpecialAttackHandler _handler;
    private Transform _trans;
    public int numEnemies;
    private int _enemyIndex;
    private List<Transform> _enemies;
    public float stopDistance = 1;
    private Animator _anim;

    private void OnDrawGizmos()
    {

    }

    private void Start()
    {
        _handler = GetComponentInParent<SpecialAttackHandler>();
        _trans = _handler.transform;
        _anim = GetComponent<Animator>();

    }

    public void StartSpecialAttack()
    {
        _enemies = _handler.enemiesInRange;

        numEnemies = _enemies.Count;
        _enemyIndex = -1;

        _anim.SetTrigger("Special Attack");
        _anim.SetBool("SPInProgress", true);
    }

    public void MoveToNextTarget(float tweenTime)
    {
        _enemyIndex++;
        if (_enemyIndex >= numEnemies)
        {
            MoveToCenter(tweenTime);
            return;
        }

        var targetPos = _enemies[_enemyIndex].position;
        var targetDir = targetPos - _trans.position;
        targetDir.Normalize();
        var dist = Vector3.Distance(targetPos, _trans.position);
        _trans.LookAt(targetPos);
        // _trans.position + targetDir * (dist - stopDistance)
        _trans.DOMove(targetPos, tweenTime);
        Debug.Log("Teleporting");
    }

    public void MoveToCenter(float tweenTime)
    {
        _trans.DOMove(_handler.centerPos, tweenTime);

        _anim.SetBool("SPInProgress", false);

    }
}

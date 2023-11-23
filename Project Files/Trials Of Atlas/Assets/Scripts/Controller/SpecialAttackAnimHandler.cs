using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class SpecialAttackAnimHandler : MonoBehaviour
{
    private MultiSpecialHandler _handler;
    private Transform _trans;
    public int numEnemies;
    private int _enemyIndex;
    private List<Transform> _enemies;
    public float stopDistance = 1;
    private Animator _anim;

    private Timer _timer1;

    private void OnDrawGizmos()
    {

    }

    private void Start()
    {
        _handler = GetComponentInParent<MultiSpecialHandler>();
        _trans = _handler.transform;
        _anim = GetComponent<Animator>();


    }

    private void Update()
    {
        TickTimer(_portTimer, Time.deltaTime);
        TickTimer(_timer1, Time.deltaTime);
    }

    private void TickTimer(Timer timer, float delta)
    {
        if (timer != null)
            timer.Tick(delta);
    }

    public void StartSpecialAttack()
    {

    }

    public void MoveToNextTarget(float tweenTime)
    {
        var targetPos = _handler.FindEnemyInRange(8);
        if (targetPos == Vector3.zero)
        {
            targetPos = _trans.position;
            _anim.SetBool("SPInProgress", false);
        }

        var dir = targetPos - _trans.position;
        dir.Normalize();
        var dist = Vector3.Distance(targetPos, _trans.position) - 1;


        _trans.DOLookAt(targetPos, tweenTime / 3);
        _trans.DOMove(_trans.position + dir * dist, tweenTime).SetEase(Ease.InCirc);
    }

    private Timer _portTimer;
    public void TeleportToTarget(float tweenTime)
    {
        var target = _handler.FindCentreTarget();

        if (target == null) return;

        var dir = target.position - _trans.position;
        dir.Normalize();
        var dist = Vector3.Distance(target.position, _trans.position) - 1;

        _trans.LookAt(target);
        _trans.DOMove(_trans.position + dir * dist, tweenTime).SetEase(Ease.InQuart).OnComplete(() => EndPort());
        _anim.speed = 0;
    }

    public void EndPort()
    {
        _anim.speed = 1;
    }

    public void Endattack()
    {
        _anim.SetBool("SPInProgress", false);
    }

    public void StartTimer(float duration)
    {
        _timer1 = new Timer(duration, () => { _anim.SetBool("SPInProgress", false); });
    }
}

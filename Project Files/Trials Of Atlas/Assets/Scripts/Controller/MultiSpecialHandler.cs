using System;
using System.Collections;
using System.Collections.Generic;
using AJK;
using UnityEngine;

public class MultiSpecialHandler : MonoBehaviour
{
    public bool primed = false;
    public float targetResetTime = 0.5f;
    private PlayerInputCLass _input;
    private Animator _anim;
    public List<SpecialAttack> specialAttacks = new List<SpecialAttack>();
    private Camera cam;

    private Timer _timer1, _timer2, _timer3, _targetResetTimer;
    public bool sp1 = true, sp2 = true, sp3 = true;
    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<PlayerLocomotion>().cameraTransform.GetComponent<Camera>();

        _input = GetComponent<PlayerManager>().inputHandler;
        _input.Special1 = Special1;
        _input.Special2 = Special2;
        _input.Special3 = Special3;
        _input.SpecialAction = Special1;

        _anim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        primed = _input.specialAttackPrimed;

        float delta = Time.deltaTime;
        TickTimer(_timer1, delta);
        TickTimer(_timer2, delta);
        TickTimer(_timer3, delta);
        TickTimer(_targetResetTimer, delta);
    }

    private void TickTimer(Timer timer, float delta)
    {
        if (timer != null)
            timer.Tick(delta);
    }

    private void Special1()
    {
        if (!sp1)
            return;

        Debug.Log(specialAttacks[0].triggerName + " initialized");
        _timer1 = new Timer(specialAttacks[0].CoolDown, EndSpecial1);
        _anim.SetTrigger(specialAttacks[0].triggerName);
        _anim.SetBool("SPInProgress", true);

        sp1 = false;
    }

    private void EndSpecial1()
    {
        _timer1 = null;
        sp1 = true;
        Debug.Log("SpecialAttack1 Cooled off");
    }

    private void Special2()
    {
        if (!sp2)
            return;
        Debug.Log(specialAttacks[1].triggerName + " initialized");
        _timer2 = new Timer(specialAttacks[1].CoolDown, EndSpecial2);
        _anim.SetTrigger(specialAttacks[1].triggerName);
        _anim.SetBool("SPInProgress", true);

        sp2 = false;
    }
    private void EndSpecial2()
    {
        _timer2 = null;
        sp2 = true;
        Debug.Log("SpecialAttack2 Cooled off");
    }


    private void Special3()
    {
        if (!sp3)
            return;
        Debug.Log(specialAttacks[2].triggerName + " initialized");
        _timer3 = new Timer(specialAttacks[2].CoolDown, EndSpecial3);
        _anim.SetTrigger(specialAttacks[2].triggerName);
        _anim.SetBool("SPInProgress", true);

        sp3 = false;
    }
    private void EndSpecial3()
    {
        _timer3 = null;
        sp3 = true;
        Debug.Log("SpecialAttack3 Cooled off");
    }

    private Transform previousEnemy = null;
    public Vector3 FindEnemyInRange(float range)
    {
        var arr = Physics.OverlapSphere(transform.position, range);
        List<Transform> enemies = new List<Transform>();

        foreach (var obj in arr)
        {
            var dir = obj.transform.position - transform.position;
            dir.Normalize();
            var dist = Vector3.Distance(transform.position, obj.transform.position);
            RaycastHit hit = new RaycastHit();
            var rayCast = Physics.Raycast(transform.position + Vector3.up, dir, out hit);
            if (hit.collider == null) continue;
            if (obj.CompareTag("Enemy") && hit.collider.CompareTag("Enemy") && hit.collider.transform != previousEnemy)
                enemies.Add(obj.transform);
        }

        if (enemies.Count == 0)
        {
            return Vector3.zero;
        }

        float minDist = Mathf.Infinity;
        Transform close = null;
        foreach (var enemy in enemies)
        {
            var dist = Vector3.Distance(transform.position, enemy.position);
            if (dist < minDist && enemy != previousEnemy)
            {
                minDist = dist;
                close = enemy;
            }
        }

        previousEnemy = close;
        _targetResetTimer = new Timer(targetResetTime, () => { previousEnemy = null; });
        return close.position;
    }

    public Transform FindCentreTarget()
    {
        var arr = Physics.OverlapSphere(transform.position, 20);
        List<Transform> enemies = new List<Transform>();

        foreach (var obj in arr)
        {
            var dir = obj.transform.position - transform.position;
            dir.Normalize();
            var dist = Vector3.Distance(transform.position, obj.transform.position);
            RaycastHit hit = new RaycastHit();
            var rayCast = Physics.Raycast(transform.position + Vector3.up, dir, out hit);
            if (obj.CompareTag("Enemy") && hit.collider.CompareTag("Enemy"))
                enemies.Add(obj.transform);
        }

        if (enemies.Count == 0)
        {
            return null;
        }

        float minDist = Mathf.Infinity;
        Transform close = null;
        foreach (var enemy in enemies)
        {
            var dist = Vector2.Distance(cam.WorldToScreenPoint(enemy.position + Vector3.up),
                                            new Vector2(Screen.width / 2f, Screen.height / 2f));
            if (dist < minDist)
            {
                minDist = dist;
                close = enemy;
            }
        }

        return close;
    }
}

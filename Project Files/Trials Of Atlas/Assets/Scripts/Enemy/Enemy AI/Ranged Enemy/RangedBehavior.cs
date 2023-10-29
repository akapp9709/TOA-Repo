using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedBehavior : EnemyBehavior
{
    public Transform rightHand;
    public GameObject projectilePrefab;
    public float projectileSpeed = 20;
    public float projectileLifeTime = 5;

    private int _actionCount;
    private GameObject _projectileObj;

    public GameObject aimingLine;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        OnTick = MakeADecision;
        _brain = new RangedBrain();
        _brain.AddToDictionary("target", GameObject.FindGameObjectWithTag("Player"));
        _brain.enemyStats = enemySO;

        _brain.StartFSM("Idle", this);

        Action = FireProjectile;
    }

    // Update is called once per frame
    protected override void Update()
    {
        _brain.UpdateFSM(this);

        if (aimingLine != null)
        {
            aimingLine.GetComponent<LineRenderer>().SetPosition(1, transform.position + transform.forward * 20f + Vector3.up);
        }
    }

    private void MakeADecision()
    {
        if (!_brain.isActive)
        {
            return;
        }
    }

    private void FireProjectile()
    {
        if (_actionCount == 0)
        {
            _projectileObj = Instantiate(projectilePrefab, rightHand.position, Quaternion.identity, transform);
            _projectileObj.GetComponent<ProjectileHandler>().SetupValues(projectileLifeTime, rightHand, "Player", enemySO.strength);
            _actionCount++;
        }
        else if (_actionCount == 1)
        {
            var prRB = _projectileObj.GetComponent<Rigidbody>();
            _projectileObj.GetComponent<ProjectileHandler>().isLaunched = true;
            prRB.velocity = transform.forward.normalized * projectileSpeed;
            _actionCount = 0;
        }
    }

    public void ActivateAimingLine()
    {
        aimingLine.SetActive(true);
        aimingLine.GetComponent<LineRenderer>().SetPosition(0, transform.position + Vector3.up);
    }

    public void DeactivateAimingLine()
    {
        aimingLine.SetActive(false);
    }

}

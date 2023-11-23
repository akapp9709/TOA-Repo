using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class BruteBehavior : EnemyBehavior
{
    [SerializeField] private float staggerTime = 3f;
    [SerializeField] private float maxChaseDist, maxChaseTime = 5f, delayTime;

    public Transform warningDecal;
    private Transform _playerTrans;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        _playerTrans = GameObject.FindGameObjectWithTag("Player").transform;

        _brain = new BruteBrain();
        _brain.AddToDictionary("target", _playerTrans.gameObject);
        _brain.AddToDictionary("Stagger Time", staggerTime);
        _brain.AddToDictionary("Max Chase Time", maxChaseTime);
        _brain.AddToDictionary("Max Chase Distance", maxChaseDist);
        _brain.AddToDictionary("Delay Time", delayTime);
        _brain.enemyStats = enemySO;

        var arr = GetComponentsInChildren<HitBox>();
        foreach (var comp in arr)
        {
            comp.SetupValues("Player", enemySO.strength);
        }

        _brain.StartFSM("Idle", this);
    }

    // Update is called once per frame
    protected override void Update()
    {

        base.Update();
        _brain.UpdateFSM(this);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position + Vector3.up, maxChaseDist);
    }

    public void ActivateDecal(Vector3 position)
    {
        warningDecal.gameObject.SetActive(true);
        warningDecal.position = position + Vector3.up * 0.1f;
    }

    public void DeactivateDecal()
    {
        warningDecal.transform.parent = transform;
        warningDecal.gameObject.SetActive(false);
        warningDecal.localPosition = Vector3.zero;
    }

    public void SetDecalPosition(Vector3 position)
    {
        warningDecal.position = position + Vector3.up * 0.1f;
    }
    public void DecoupleDecal()
    {
        warningDecal.parent = null;
    }

    public Vector3 GetTargetPosition()
    {
        Vector3 outVec;

        var navHit = new NavMeshHit();
        if (NavMesh.SamplePosition(_playerTrans.position, out navHit, 0.5f, NavMesh.AllAreas))
        {
            outVec = _playerTrans.position;
        }
        else
            outVec = Vector3.zero;

        return outVec;
    }
}

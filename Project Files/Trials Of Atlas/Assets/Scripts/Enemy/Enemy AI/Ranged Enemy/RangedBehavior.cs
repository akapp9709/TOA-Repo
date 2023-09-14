using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedBehavior : EnemyBehavior
{
    // private EnemyBrain _brain;

    

    public delegate void BrainTick();

    public BrainTick LogicTick;
    
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        OnTick = MakeADecision;
        Debug.Log("Ranged Enemy Initialized");
        _brain = new RangedBrain();
        _brain.AddToDictionary("target", GameObject.FindGameObjectWithTag("Player"));
        _brain.enemyStats = enemySO;
        
        _brain.StartFSM("Idle", this);
    }

    // Update is called once per frame
    protected override void Update()
    {
        _brain.UpdateFSM(this);
    }

    private void MakeADecision()
    {
        if (!_brain.isActive)
        {
            return;
        }
        
        LogicTick?.Invoke();
    }
}

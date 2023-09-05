using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedBehavior : EnemyBehavior
{
    private EnemyBrain _brain;

    public EnemyStats enemySO;
    
    
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        OnTick = MakeADecision;
        Debug.Log("Ranged Enemy Initialized");
        _brain = new RangedBrain();
        _brain.AddToDictionary("target", GameObject.FindGameObjectWithTag("Player"));
        _brain.enemyStats = enemySO;
    }

    // Update is called once per frame
    protected override void Update()
    {
        _brain.UpdateFSM();
    }

    private void MakeADecision()
    {
        Debug.Log("A Second has passed and I have nothing to dwell");
    }
}

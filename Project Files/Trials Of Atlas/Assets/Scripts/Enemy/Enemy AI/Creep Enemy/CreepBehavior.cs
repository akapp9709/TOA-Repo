using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreepBehavior : EnemyBehavior
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        _brain = new CreepBrain();
        
        _brain.StartFSM("Idle", this);
    }

    // Update is called once per frame
    protected override void Update()
    {
        _brain.UpdateFSM(this);
    }
}

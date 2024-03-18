using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractibleDoor : InteractibleBehaivor
{
    private Animator _animator;
    public Action DoorOpen;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        _animator = GetComponent<Animator>();
        _animator.SetBool("isOpen", false);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    public override void Interact()
    {
        if (!isActive)
            return;

        _animator.SetBool("isOpen", true);
        DoorOpen?.Invoke();
    }
}

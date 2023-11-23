using System.Collections;
using System.Collections.Generic;
using AJK;
using UnityEngine;

public class Reward : InteractibleBehaivor
{
    public enum RewardType
    {
        Heal,
        DamageUp,
        DefenseUp
    }
    public RewardType type;

    public float amount;

    protected override void Start()
    {
        base.Start();


        GetComponentInChildren<Canvas>().worldCamera = Camera.current;
    }

    protected override void Update()
    {
        base.Update();

        foreach (var obj in inputImage)
        {
            obj.transform.LookAt(playerTrans.GetComponent<PlayerLocomotion>().cameraTransform);
        }
    }

    public override void Interact()
    {
        if (!isActive)
        {
            return;
        }

        playerTrans.GetComponent<PlayerManager>().GainEffect(this);
    }

    public void PickUp()
    {
        Destroy(this.gameObject);
    }
}

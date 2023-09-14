using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AITest : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        BroadcastMessage("InitializeBehavior");
    }
}

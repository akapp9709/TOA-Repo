using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationHandler : MonoBehaviour
{
    public void FireProjectile()
    {
        GetComponentInParent<EnemyBehavior>().Action?.Invoke();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationHandler : MonoBehaviour
{
    public void AnimationActionEvent()
    {
        GetComponentInParent<EnemyBehavior>().Action?.Invoke();
    }
}

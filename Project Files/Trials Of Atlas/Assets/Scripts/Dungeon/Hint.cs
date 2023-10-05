using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hint : MonoBehaviour
{
    private Transform _follow;
    // Start is called before the first frame update
    public void InitializeHint(Transform follow, float lifetime)
    {
        _follow = follow;
        Invoke("EndHint", lifetime);
    }

    // Update is called once per frame
    void Update()
    {
        var dir = Quaternion.LookRotation(_follow.position - transform.parent.position);
        transform.rotation = dir;
    }

    public void EndHint()
    {
        Destroy(this.gameObject);
    }
}

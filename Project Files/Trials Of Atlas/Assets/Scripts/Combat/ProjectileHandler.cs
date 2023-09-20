using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileHandler : MonoBehaviour
{
    public float lifeTime, damage;
    public string targetTag;
    public bool isLaunched = false, destroyOnImpact = false;
    public Transform hand;

    private float _liveTime;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<HitBox>().SetupValues(targetTag, damage);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLaunched)
        {
            transform.position = hand.position;
        }

        if (isLaunched)
        {
            _liveTime += Time.deltaTime;
            if (_liveTime > lifeTime)
            {
                Destroy(this.gameObject);
            }
        }
    }

    public void SetupValues(float t, Transform h, string target, float dmg)
    {
        lifeTime = t;
        hand = h;
        targetTag = target;
        damage = dmg;
    }
}

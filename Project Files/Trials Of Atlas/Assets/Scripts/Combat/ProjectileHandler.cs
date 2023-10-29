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
    private Rigidbody _rb;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<HitBox>().SetupValues(targetTag, damage);
        GetComponent<SphereCollider>().enabled = false;

        _rb = GetComponent<Rigidbody>();
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
            GetComponent<SphereCollider>().enabled = true;

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

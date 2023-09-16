using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileHandler : MonoBehaviour
{
    public float lifeTime;
    public bool isLaunched = false;
    public Transform hand;

    private float _liveTime;
    // Start is called before the first frame update
    void Start()
    {
        
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
    
    
}

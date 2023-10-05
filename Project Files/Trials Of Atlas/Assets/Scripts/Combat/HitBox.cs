using System.Collections;
using System.Collections.Generic;
using AJK;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    private float _damage;
    private string _targetTag;

    public void SetupValues(string tag, float damageDealt)
    {
        _damage = damageDealt;
        _targetTag = tag;
    }

    public void UpdateValue(float amount)
    {
        _damage = amount;
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag(_targetTag))
        {
            if (_targetTag == "Player")
                other.GetComponent<PlayerManager>().TakeDamage(_damage);
            else if (_targetTag == "Enemy")
                other.GetComponent<Enemy>().TakeDamage(_damage);


            if (GetComponent<ProjectileHandler>() != null)
            {
                if (GetComponent<ProjectileHandler>().destroyOnImpact)
                {
                    Destroy(this.gameObject);
                }
            }
        }


    }
}

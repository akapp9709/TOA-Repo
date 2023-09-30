using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemyStats enemySO;

    private float enemyHealth;
    private Rigidbody _rb;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();

        enemyHealth = enemySO.health;
    }

    public void StartEnemyFSM()
    {
        Debug.Log("Enemy starting up");
        GetComponent<EnemyBehavior>().InitializeBehavior();
    }

    // Update is called once per frame
    void Update()
    {
        if (enemyHealth < 0)
        {
            return;
        }

        if (enemyHealth <= 0)
        {
            Destroy(this.gameObject);
        }


    }

    public void TakeDamage(float damage)
    {
        _rb.isKinematic = false;
        _rb.AddForce(transform.forward * -5f, ForceMode.Impulse);
        enemyHealth -= damage;
        StartCoroutine(KnockBackTimer());
    }

    private IEnumerator KnockBackTimer()
    {
        yield return new WaitForSeconds(0.2f);

        _rb.velocity = Vector3.zero;
        _rb.isKinematic = true;
    }

    public float EnemyHealth => enemySO.health;
    public float EnemyStrength => enemySO.strength;
    public float EnemyAggression => enemySO.aggression;
}

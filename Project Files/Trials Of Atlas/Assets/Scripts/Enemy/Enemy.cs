using System.Collections;
using System.Collections.Generic;
using AJK;
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

    private void OnDestroy()
    {

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
    }

    public void TakeDamage(float damage)
    {
        _rb.isKinematic = false;
        var dir = PlayerManager.Singleton.transform.position - transform.position;
        dir.Normalize();
        _rb.AddForce(dir * -5f, ForceMode.Impulse);
        enemyHealth -= damage;
        PlayerManager.Singleton.playerStats.GainXP(PlayerSO.Skill.Strength, damage);
        StartCoroutine(KnockBackTimer());
        if (enemyHealth <= 0)
        {
            KillEnemy();
        }
    }

    private void KillEnemy()
    {
        SendMessageUpwards("RemoveEnemy", this);
        GameObject.FindObjectOfType<BitCurrencyHandler>().PickupBits(enemySO.DropBits());
        Destroy(this.gameObject);
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

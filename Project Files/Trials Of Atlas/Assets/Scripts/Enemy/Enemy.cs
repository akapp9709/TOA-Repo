using System.Collections;
using System.Collections.Generic;
using AJK;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemyStats enemySO;

    private float enemyHealth, _maxHealth;
    private Rigidbody _rb;
    private bool _canTakeDamage = true;
    private Animator _anim;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();

        enemyHealth = enemySO.health;
        _maxHealth = enemyHealth;

        _anim = GetComponentInChildren<Animator>();
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

        _anim.SetFloat("HealthAvailable", enemyHealth / _maxHealth);
    }

    public void TakeDamage(float damage)
    {
        if (!_canTakeDamage)
            return;

        _canTakeDamage = false;
        StartCoroutine(ResetTriggerTimer());
        Debug.Log("Enemy taken damage");


        _rb.isKinematic = false;
        var dir = PlayerManager.Singleton.transform.position - transform.position;
        dir.Normalize();
        _rb.AddForce(dir * -5f, ForceMode.Impulse);
        enemyHealth -= damage;
        PlayerManager.Singleton.playerStats.GainXP(PlayerSO.Skill.Strength, damage);
        _anim.SetTrigger("Hit");
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

    private IEnumerator ResetTriggerTimer()
    {
        yield return new WaitForSeconds(0.1f);
        _canTakeDamage = true;
    }

    public float EnemyHealth => enemySO.health;
    public float EnemyStrength => enemySO.strength;
    public float EnemyAggression => enemySO.aggression;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemyStats enemySO;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TakeDamage(float damage)
    {
        Debug.Log($"{this.gameObject.name} took {damage} points of damage");
    }

    public float EnemyHealth => enemySO.health;
    public float EnemyStrength => enemySO.strength;
    public float EnemyAggression => enemySO.aggression;
}

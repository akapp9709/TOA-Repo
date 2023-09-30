using System.Collections;
using System.Collections.Generic;
using EnemyAI;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehavior : MonoBehaviour
{
    protected EnemyBrain _brain;
    public EnemyStats enemySO;
    public NavMeshAgent agent;
    public Animator anim;

    public delegate void Tick();
    public Tick OnTick;
    public float decisionTime = 2f;

    public delegate void AttackAction();
    public AttackAction Action;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        StartCoroutine(Clock());
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    protected virtual void Update()
    {

    }

    private IEnumerator Clock()
    {
        while (true)
        {
            OnTick?.Invoke();
            yield return new WaitForSeconds(decisionTime);
        }
    }

    public void InitializeBehavior()
    {
        _brain.isActive = true;
        var player = GameObject.FindGameObjectWithTag("Player");
        _brain.AddToDictionary("Player Target", player);
    }
}

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

    public delegate void Tick();
    public Tick OnTick;
    
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
            Debug.Log("Ticking");
            yield return new WaitForSeconds(1f);
        }
    }

    private void InitializeBehavior()
    {
        _brain.isActive = true;
        var player = GameObject.FindGameObjectWithTag("Player");
        _brain.AddToDictionary("Player Target", player);
    }
}

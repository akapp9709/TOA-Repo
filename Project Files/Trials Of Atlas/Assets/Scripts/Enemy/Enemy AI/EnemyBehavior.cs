using System.Collections;
using System.Collections.Generic;
using EnemyAI;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    protected EnemyFSM StateMachine;

    public delegate void Tick();
    public Tick OnTick;
    
    // Start is called before the first frame update
    protected virtual void Start()
    {
        StartCoroutine(Clock());
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
}

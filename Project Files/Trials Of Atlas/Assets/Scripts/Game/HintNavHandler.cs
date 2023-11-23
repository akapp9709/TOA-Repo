using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

public class HintNavHandler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        PF_Generator.Singleton.OnComplete += () =>
        {
            GetComponent<NavMeshSurface>().BuildNavMesh();
            // Invoke("InitializeAgent", 0.2f);
        };
    }

    // Update is called once per frame
    void Update()
    {

    }
}

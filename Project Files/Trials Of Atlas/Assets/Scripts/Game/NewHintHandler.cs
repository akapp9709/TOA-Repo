using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;
using AJK;

public class NewHintHandler : MonoBehaviour
{
    public float lifeTime;
    public GameObject hintObject;
    public NavMeshAgent agent;
    [HideInInspector] public Transform agentTransform;
    public Transform player;
    public float maxDistance = 20f;
    private Timer _lifeTimer;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        _lifeTimer = new Timer(lifeTime, () => Destroy(this.gameObject));
        player = FindObjectOfType<PlayerManager>().transform;
    }

    public void InitializeAgent(Vector3 position)
    {
        agent.SetDestination(position);
    }

    // Update is called once per frame
    void Update()
    {
        var dist = Vector3.Distance(transform.position, player.position);

        agent.speed = Mathf.Lerp(0, 12f, (maxDistance * maxDistance - dist * dist) / maxDistance * maxDistance);
        if (_lifeTimer != null)
        {
            _lifeTimer.Tick(Time.deltaTime);
        }
    }

    public void StartHint(Vector3 pos)
    {
        var obj = Instantiate(hintObject, player.position, Quaternion.identity, this.transform);
        agent = obj.GetComponent<NavMeshAgent>();
        agentTransform = agent.transform;
        agent.SetDestination(pos);

        _lifeTimer = new Timer(20f, () => { Destroy(obj); }, x => Debug.Log("Destroy"));
    }

    public void EndHint()
    {
        agent.ResetPath();
        agent.enabled = false;
    }
}

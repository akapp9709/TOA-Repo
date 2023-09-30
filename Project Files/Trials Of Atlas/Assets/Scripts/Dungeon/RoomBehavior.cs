using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomBehavior : MonoBehaviour
{
    [SerializeField] private List<Enemy> enemies;
    // Start is called before the first frame update
    public void InitializeRoom()
    {
        var arr = GetComponentsInChildren<Enemy>();
        foreach (var obj in arr)
        {
            enemies.Add(obj);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        foreach (var enemy in enemies)
        {
            enemy.StartEnemyFSM();
        }
    }
}

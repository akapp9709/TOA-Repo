using UnityEngine;

using System.Collections.Generic;
using System;

public class RoomBehavior : MonoBehaviour
{


    [SerializeField] private List<Enemy> enemies;
    [SerializeField] private Transform rewardSpawner;
    [SerializeField] private List<GameObject> rewards;

    public Action PlayerEntry;
    public Action RoomClear;
    // Start is called before the first frame update
    public void InitializeRoom()
    {
        var arr = GetComponentsInChildren<Enemy>();
        foreach (var obj in arr)
        {
            enemies.Add(obj);
        }
        PlayerEntry += RoomStart;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void RoomStart()
    {
        foreach (var enemy in enemies)
        {
            enemy.StartEnemyFSM();
        }
    }

    private void RemoveEnemy(Enemy obj)
    {
        enemies.Remove(obj);

        if (enemies.Count == 0)
        {
            Debug.Log("Room Cleared");
            GameManager.Singleton.UpdateProgress(this);
            var ind = UnityEngine.Random.Range(0, rewards.Count);
            Instantiate(rewards[ind], rewardSpawner);
            RoomClear?.Invoke();
        }
    }
}

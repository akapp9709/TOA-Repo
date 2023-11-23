using System.Collections;
using System.Collections.Generic;
using AJK;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.InputSystem;

public class HintHandler : MonoBehaviour
{
    private PlayerInputCLass _input;
    private GameManager _manager;
    public GameObject hintPrefab;
    public float hintCoolDown;
    private bool _coolDown;

    private Timer coolDown;

    void Start()
    {
        _manager = GameManager.Singleton;

        GetComponent<PlayerManager>().inputHandler.HintAction += ShowHint;
    }

    private void Update()
    {
        if (coolDown != null)
        {
            coolDown.Tick(Time.deltaTime);
        }
    }

    private void ShowHint()
    {
        if (_coolDown)
            return;

        Transform closestRoom = FindClosestRoom();
        var obj = Instantiate(hintPrefab, transform.position, Quaternion.identity);
        obj.GetComponent<NewHintHandler>().InitializeAgent(closestRoom.position);
        coolDown = new Timer(hintCoolDown, () => _coolDown = false);
        _coolDown = true;
    }

    private Transform FindClosestRoom()
    {
        var rooms = _manager.rooms;
        float closestDistance = Mathf.Infinity;
        Transform closestRoom = null;

        foreach (var room in rooms)
        {
            float distance = Vector3.Distance(transform.position, room.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestRoom = room.transform;
            }
        }

        return closestRoom;
    }
}

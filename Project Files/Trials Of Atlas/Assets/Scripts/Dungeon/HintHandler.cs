using System.Collections;
using System.Collections.Generic;
using AJK;
using UnityEngine;
using UnityEngine.InputSystem;

public class HintHandler : MonoBehaviour
{
    private InputHandler _input;
    private GameManager _manager;
    public GameObject hintPrefab;
    public float hintCoolDown;
    private bool _coolDown;

    void Start()
    {
        _manager = GameManager.Singleton;

        var controls = new PlayerControls();
        controls.PlayerActions.Enable();
        controls.PlayerActions.Hint.started += ShowHint;
    }

    private void ShowHint(InputAction.CallbackContext context)
    {
        if (_coolDown)
            return;

        Transform closestRoom = FindClosestRoom();
        var obj = Instantiate(hintPrefab, this.transform);
        obj.GetComponent<Hint>().InitializeHint(closestRoom, hintCoolDown / 2);

        StartCoroutine(CoolDown());
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

    private IEnumerator CoolDown()
    {
        _coolDown = true;
        yield return new WaitForSeconds(hintCoolDown);
        _coolDown = false;
    }
}

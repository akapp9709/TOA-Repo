using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DungeonUIManager : MonoBehaviour
{
    private GameManager _manager;
    public TextMeshProUGUI dungeonProgress;
    // Start is called before the first frame update
    void Start()
    {
        _manager = GameManager.Singleton;
        _manager.OnUpdate += UpdateValues;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void UpdateValues()
    {
        dungeonProgress.SetText($"Progress: \n\t {_manager.numRooms - _manager.roomsRemaining}/{_manager.numRooms}");
    }
}

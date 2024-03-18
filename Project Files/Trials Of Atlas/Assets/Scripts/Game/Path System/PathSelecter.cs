using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathSelecter : MonoBehaviour
{
    [SerializeField] private List<Path> availablePaths;

    private PathUI _pathUI;
    // Start is called before the first frame update
    void Start()
    {
        GetComponentInChildren<InteractibleDoor>().DoorOpen += SelectPath;
        _pathUI = FindObjectOfType<PathUI>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SelectPath()
    {
        Debug.Log("Selecting Path");
    }
}

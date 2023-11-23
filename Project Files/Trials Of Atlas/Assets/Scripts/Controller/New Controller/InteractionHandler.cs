using System.Collections;
using System.Collections.Generic;
using AJK;
using UnityEngine;
using UnityEngine.SceneManagement;


public class InteractionHandler : MonoBehaviour
{
    private void OnSceneUnload(Scene current)
    {
        GetComponent<PlayerManager>().inputHandler.InteractAction = null;
        SceneManager.sceneUnloaded -= OnSceneUnload;
    }

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<PlayerManager>().inputHandler.InteractAction = Interact;
        SceneManager.sceneUnloaded += OnSceneUnload;
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void Interact()
    {
        var arr = FindObjectsOfType<InteractibleBehaivor>();

        InteractibleBehaivor target = arr[0];

        if (arr[0] == null)
        {
            Debug.Log("This is what can be null");
            Debug.Log(arr.Length);
        }
        float minDist = Mathf.Infinity;
        foreach (var obj in arr)
        {
            var dist = Vector3.Distance(obj.transform.position, transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                target = obj;
            }
        }
        arr = null;

        target.Interact();
    }
}

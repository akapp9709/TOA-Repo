using System.Collections;
using System.Collections.Generic;
using AJK;
using UnityEngine;

public class InteractibleBehaivor : MonoBehaviour
{
    public static float ActivationDistance = 3;
    protected Transform playerTrans;
    public bool isActive = false;
    public List<RectTransform> inputImage;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        playerTrans = FindObjectOfType<PlayerManager>().transform;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        isActive = Vector3.Distance(playerTrans.position, transform.position) <= ActivationDistance;
        foreach (var image in inputImage)
        {
            image.gameObject.SetActive(isActive);
        }
    }

    public virtual void Interact()
    {
        if (!isActive)
            return;
    }
}

using System.Collections;
using System.Collections.Generic;
using AJK;
using UnityEngine;

public class PlayerStart : MonoBehaviour
{
    public Transform startPoint;
    // Start is called before the first frame update
    void Start()
    {
        PF_Generator.Singleton.OnComplete += MovePlayerToStart;
    }

    private void MovePlayerToStart()
    {
        var playTrans = PlayerManager.Singleton.transform;
        playTrans.position = startPoint.position;
    }
}

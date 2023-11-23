using AJK;
using UnityEngine;

public class PlayerStart : MonoBehaviour
{
    public Transform startPoint;
    // Start is called before the first frame update
    void Start()
    {
        PF_Generator.Singleton.OnComplete += MovePlayerToStart;

        var arr = GetComponentsInChildren<Canvas>();
        foreach (var obj in arr)
        {
            obj.worldCamera = Camera.current;
        }
    }

    private void MovePlayerToStart()
    {
        var playTrans = PlayerManager.Singleton.transform;
        playTrans.position = startPoint.position;
    }
}

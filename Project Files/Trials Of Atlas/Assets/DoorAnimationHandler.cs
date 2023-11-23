using UnityEditor;
using UnityEngine;
using DG.Tweening;

public class DoorAnimationHandler : MonoBehaviour
{
    private bool roomIsClear = false;

    public Transform openPosition, closedPosition, checkPosition;
    public Vector3 hitPos;
    private bool _canOpen = true;

    private void OnDestroy()
    {
        PF_Generator.Singleton.OnComplete -= CheckForHall;
    }
    private void OnDrawGizmos()
    {
        var pos = transform.position;
        Gizmos.DrawCube(openPosition.position, Vector3.one * 0.5f);
        Gizmos.color = Color.red;
        Gizmos.DrawCube(closedPosition.position, Vector3.one * 0.5f);
        Gizmos.DrawSphere(checkPosition.position, 0.3f);

        Gizmos.DrawSphere(hitPos, 1f);
    }
    private void Start()
    {
        GetComponentInParent<RoomBehavior>().PlayerEntry += CloseDoorAfterSeconds;
        GetComponentInParent<RoomBehavior>().RoomClear += OpenDoorDelay;

        PF_Generator.Singleton.OnComplete += CheckForHall;
        OpenDoor();
    }

    private void CheckForHall()
    {
        var hit = new RaycastHit();
        if (Physics.Raycast(checkPosition.position, Vector3.down, out hit))
        {
            OpenDoor();
            hitPos = hit.point;
        }
        else
        {
            CloseDoor();
            _canOpen = false;
        }

    }

    private void CloseDoorAfterSeconds()
    {
        Invoke("CloseDoor", 0.5f);
    }

    private void OpenDoorDelay()
    {
        Invoke("OpenDoor", 1f); roomIsClear = true;
    }

    private void OpenDoor()
    {
        if (_canOpen)
            transform.DOMove(openPosition.position, 1.5f);

    }

    private void CloseDoor()
    {
        if (roomIsClear)
        {
            return;
        }
        transform.DOMove(closedPosition.position, 0.4f);
    }
}

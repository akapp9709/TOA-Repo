using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class CameraSwitchHandler : MonoBehaviour
{
    public List<CinemachineVirtualCamera> cams;
    // Start is called before the first frame update
    void Start()
    {
        PF_Generator.Singleton.OnComplete += GetAllVCams;
    }

    private void GetAllVCams()
    {
        var arr = FindObjectsOfType<CinemachineVirtualCamera>();
        cams = new List<CinemachineVirtualCamera>(arr);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ChangeToCamera(ICinemachineCamera targetCamera)
    {
        var currentCam = GetComponent<CinemachineBrain>().ActiveVirtualCamera;
        currentCam.Priority = 0;
        targetCamera.Priority = 1;
    }
}

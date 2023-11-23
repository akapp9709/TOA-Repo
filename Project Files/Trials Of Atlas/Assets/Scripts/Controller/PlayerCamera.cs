using System;
using System.Collections;
using System.Collections.Generic;
using AJK;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.HighDefinition;

public class PlayerCamera : MonoBehaviour
{
    private CinemachineFreeLook _freeLook;
    private Timer _timer;
    public float recentreTime = 0.4f;

    private void Awake()
    {

    }

    private void Start()
    {
        _freeLook = GetComponent<CinemachineFreeLook>();
        FindObjectOfType<PlayerManager>().inputHandler.Recentre += RecentreCamera;
    }

    private void Update()
    {
        if (_timer == null)
            return;

        _timer.Tick(Time.deltaTime);
    }

    private void RecentreCamera()
    {
        _freeLook.m_RecenterToTargetHeading.m_enabled = true;
        _timer = new Timer(recentreTime, () => _freeLook.m_RecenterToTargetHeading.m_enabled = false);
    }
}

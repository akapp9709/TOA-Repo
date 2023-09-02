using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 targetOffset;
    [SerializeField] private Transform cameraTransform;
    public float cameraDistance = 3f, rotationSpeed, sensitivityX, sensitivityY;

    private PlayerControls _controls;
    private float _camX, _camY;
    private Transform _trans;

    private void OnDrawGizmos()
    {
        var pos = transform.position;
        
        Gizmos.color = Color.red;
        Gizmos.DrawLine(pos, pos + transform.forward*-2);
        Gizmos.DrawSphere(pos + transform.forward*-2, 0.3f);
    }

    // Start is called before the first frame update
    void Start()
    {
        _trans = transform;
        _controls = new PlayerControls();
        _controls.Main.Enable();
        _controls.Main.Look.performed += LookAround;
        
        _trans.rotation = Quaternion.identity;
    }

    // Update is called once per frame
    void Update()
    {
        var pos = _trans.position;
        var rot = _trans.eulerAngles;

        rot.x -= _camY * Time.deltaTime * sensitivityY * rotationSpeed;
        rot.x = Mathf.Clamp(rot.x, -90f, 90f);

        _trans.rotation = Quaternion.Euler(rot);
        
        cameraTransform.position = pos + pos + transform.forward * -1 * cameraDistance;
        cameraTransform.LookAt(pos);
    }

    private void LookAround(InputAction.CallbackContext context)
    {
        var inVec = context.ReadValue<Vector2>();

        _camX = inVec.x;
        _camY = inVec.y;
        
    }
}

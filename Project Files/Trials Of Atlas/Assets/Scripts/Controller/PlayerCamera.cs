using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.HighDefinition;

public class PlayerCamera : MonoBehaviour
{
    public Transform target;
    public Transform cameraTrans;
    public Transform cameraPivot;
    private Transform _myTransform;
    private Vector3 _cameraPos;
    private LayerMask _ignoreLayers;
    private Vector3 _cameraFollowVelocity = Vector3.zero;
    private Vector3 _cameraTransformPosition = Vector3.zero;

    public static PlayerCamera Singleton;
    
    public float lookSpeed = 0.1f;
    public float followSpeed = 0.1f;
    public float pivotSpeed = 0.03f;

    private float _targetPosition;
    private float _defaultPosition;
    private float _lookAngle;
    private float _pivotAngle;
    public float minPivot = -35;
    public float maxPivot = 35;
    
    private PlayerControls _controls;
    private Vector2 _mouseDelta;

    public float cameraSphereRadius = 0.2f;
    public float cameraCollisionOffset = 0.2f;
    public float minCollisionOffset = 0.2f;

    private void Awake()
    {
        Singleton = this;
        _myTransform = transform;
        _defaultPosition = cameraTrans.localPosition.z;
        _ignoreLayers = ~(1 << 8 | 1 << 9 | 1 << 10);

        _controls = new PlayerControls();
        _controls.Main.Enable();

        _controls.Main.Look.started += context => _mouseDelta = context.ReadValue<Vector2>();
        _controls.Main.Look.canceled += context => _mouseDelta = Vector2.zero;
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        var delta = Time.deltaTime;
        
        FollowTarget(delta);
        HandleRotation(delta, _mouseDelta.x, _mouseDelta.y);
    }

    public void FollowTarget(float delta)
    {
        var targetPosition = Vector3.SmoothDamp(_myTransform.position, target.position, ref _cameraFollowVelocity,
            delta / followSpeed);
        _myTransform.position = targetPosition;
        
        HandleCameraCollision(delta);
    }

    public void HandleRotation(float delta, float mouseX, float mouseY)
    {
        _lookAngle += (mouseX * lookSpeed) / delta;
        _pivotAngle -= (mouseY * pivotSpeed) / delta;
        _pivotAngle = Mathf.Clamp(_pivotAngle, minPivot, maxPivot);

        var rotation = Vector3.zero;
        rotation.y = _lookAngle;
        var targetRotation = Quaternion.Euler(rotation);
        _myTransform.rotation = targetRotation;

        rotation = Vector3.zero;
        rotation.x = _pivotAngle;
        targetRotation = Quaternion.Euler(rotation);
        cameraPivot.localRotation = targetRotation;
    }
    
    private void HandleCameraCollision(float delta)
    {
        _targetPosition = _defaultPosition;
        RaycastHit hit;
        var direction = cameraTrans.position - cameraPivot.position;
        direction.Normalize();

        if (Physics.SphereCast(cameraPivot.position, cameraSphereRadius, direction, out hit, 
            Mathf.Abs(_targetPosition),_ignoreLayers))
        {
            var dis = Vector3.Distance(cameraPivot.position, hit.point);
            _targetPosition = -(dis - cameraCollisionOffset);
        }

        if (Mathf.Abs(_targetPosition) < minCollisionOffset)
        {
            _targetPosition = -minCollisionOffset;
        }

        _cameraTransformPosition.z = Mathf.Lerp(cameraTrans.localPosition.z, _targetPosition, delta / 0.2f);
        cameraTrans.localPosition = _cameraTransformPosition;
    }
}

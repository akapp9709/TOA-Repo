using System.Collections;
using System.Collections.Generic;
using AJK;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class Dodge : MonoBehaviour
{
    private PlayerSO _stats;
    private Vector3 _direction;
    private Rigidbody _rb;

    private float _dodgeTime, _dodgeSpeed = 0, _maxDodgeSpeed;
    public Vector3 dodgeV;
    private Timer _timer;
    private Animator _anim;
    private float _startTime;
    // Start is called before the first frame update
    void Start()
    {
        _stats = GetComponent<PlayerManager>().playerStats;
        _dodgeTime = _stats.dodgeTime;
        _maxDodgeSpeed = _stats.dodgeSpeed;

        _rb = GetComponent<Rigidbody>();
        _anim = GetComponentInChildren<Animator>();

        var controls = new PlayerControls();
        controls.PlayerActions.Enable();
        controls.PlayerActions.Dodge.started += TriggerDodge;
    }

    // Update is called once per frame
    void Update()
    {
        if (_timer != null)
        {
            var currentSpeed = EaseFunctions.AccelerateValue(_startTime,
                                                            _dodgeTime,
                                                            Time.time,
                                                            0, _maxDodgeSpeed,
                                                            EaseFunctions.EaseType.CircEaseOut);
            dodgeV = _direction.normalized * currentSpeed;

            _timer.Tick(Time.deltaTime);
            _rb.velocity += dodgeV;
        }
    }

    private void TriggerDodge(InputAction.CallbackContext context)
    {
        _dodgeSpeed = 0;
        _startTime = Time.time;
        var inputDir = GetComponent<PlayerLocomotion>().inVec;
        var trans = GetComponent<PlayerLocomotion>().cameraTransform;
        _direction = inputDir.x * trans.right + inputDir.z * trans.forward;
        _direction.y = 0;

        dodgeV = _direction.normalized * _dodgeSpeed;
        _timer = new Timer(_dodgeTime, EndDodge);

        transform.DOLookAt(transform.position + _direction, 0.1f);


        _anim.SetTrigger("Dodge");
        _anim.SetBool("isDodging", true);
        _anim.SetBool("IsInteracting", true);
    }

    private void EndDodge()
    {
        _timer = null;
        dodgeV = Vector3.zero;
        _anim.SetBool("isDodging", false);
    }
}

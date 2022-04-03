/*
 * Authors: Ryan Coughlin
 * Class: CS-583 Price, Group 13
 * Desc: This class controls player input.
 * 
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float cameraHeadOffset;
    public static float headRaycastOffset;
    public float dragDistance;

    public static PlayerController instance;

    public float dragForce;

    public float walkSpeed;

    public float runSpeed;

    public float mouseSensitivity;

    public bool fly = false;

    public bool dragDebounce;

    public Transform groudCheck;

    public float groundDistance;

    public Sword sword;

    private float _braceLandDistance;

    public LayerMask groundMask;

    public float gravity;

    public float jumpHeight;

    private float _x, _z;

    private float _prevSpeed;

    private float _landAcceleration = .01f, _acceleration = 100;

    private float _timeOnGround = 0;

    private float _xRotation = 0f;

    private Vector3 _velocity;

    private bool _isGrounded;

    private Transform _playerTrans;

    private CharacterController _characterController;

    private Animator _animator;

    private float _savedStepOffset;

    private Vector3 _dragDestination;

    private GameObject _draggedObject, _highlightedObj;

    private Rigidbody _draggedObjectRB;

    private Transform _neckTrans, _headTrans;

    private Quaternion _lastUpdateRotate;

    private bool _previousGravity;

    private Image _draggableHighlight;

    // Start is called before the first frame update
    void Start()
    {
        InitReferences();

        Cursor.lockState = CursorLockMode.Locked;
        _characterController.enabled = true;

        _xRotation = Camera.main.transform.localRotation.x;

        _braceLandDistance = -gravity * Time.fixedDeltaTime * 10;

        _savedStepOffset = _characterController.stepOffset;

        if(PlayerController.instance == null)
        {
            PlayerController.instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void InitReferences()
    {
        _playerTrans = GetComponent<Transform>();
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _neckTrans = transform.Find("PlayerRig/Armature/Hips/Spine/Chest/UpperChest/Neck").transform;
        _headTrans = _neckTrans.Find("Head");

        _draggableHighlight = GameObject.Find("DraggableHighlight").GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        _isGrounded = Physics.CheckSphere(groudCheck.position, groundDistance, groundMask);

        bool braceLand = Physics.CheckSphere(groudCheck.position, _braceLandDistance, groundMask);

        if (_isGrounded && _velocity.y < 0 && !fly)
        {
            _velocity.y = -2f;
        }

        _x = Input.GetAxis("Horizontal");
        _z = Input.GetAxis("Vertical");

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        float moveSpeed = _prevSpeed;

        if (_isGrounded || fly)
        {
            float acceleration = _acceleration;

            if(_timeOnGround < .5f)
            {
                acceleration = _landAcceleration;
            }

            // only change walk speeds if on the ground
            if (Input.GetKey(KeyCode.LeftShift))
            {
                moveSpeed = Mathf.Min(_prevSpeed + acceleration, runSpeed);
            }
            else
            {
                moveSpeed = Mathf.Max(_prevSpeed - acceleration, walkSpeed);
            }

            _prevSpeed = moveSpeed;
            _timeOnGround += Time.deltaTime;
            _characterController.stepOffset = _savedStepOffset;
        }
        else
        {
            _timeOnGround = 0f;
            _characterController.stepOffset = 0f;
        }


        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

        Camera.main.transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
        _lastUpdateRotate = Quaternion.Euler(_xRotation, 0f, 0f);

        _animator.SetFloat("VerticalLook", -_xRotation);
        
        _playerTrans.Rotate(Vector3.up * mouseX);

        Vector3 move = _playerTrans.transform.forward * _z + _playerTrans.transform.right * _x;
        move = Vector3.ClampMagnitude(move, moveSpeed);

        _characterController.Move(moveSpeed * Time.deltaTime * move);



        if (fly)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                _velocity.y = 7;
            }
            else if (Input.GetKey(KeyCode.LeftControl))
            {
                _velocity.y = -7;
            } else
            {
                _velocity.y = 0;
            }
        }
        else
        {
            _velocity.y += gravity * Time.deltaTime;
            if (Input.GetButtonDown("Jump") && _isGrounded)
            {

                _velocity.y += Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }

        _characterController.Move(_velocity * Time.deltaTime);

        _animator.SetFloat("Speed", (moveSpeed * move).magnitude);
        _animator.SetFloat("Forward", _z);
        _animator.SetFloat("Right", _x);
        _animator.SetFloat("Vertical", Mathf.Abs(_velocity.y));
        _animator.SetBool("Grounded", _isGrounded);
        _animator.SetBool("BraceLand", braceLand);

        updateAttack();
    }

    private void FixedUpdate()
    {
        updateDrag();
    }

    private void updateAttack()
    {
        if(Input.GetMouseButton(0))
        {
            _animator.SetBool("Attack", true);
        }
        else
        {
            _animator.SetBool("Attack", false);
        }
    }

    private void updateDrag()
    {
        var raycastResult = PlayerController.getLookRay();
        bool hitSomething = raycastResult.Item1;

        RaycastHit hit = raycastResult.Item2;

        if (hit.transform != null && hit.transform.gameObject.tag == "Draggable")
        {
            _highlightedObj = hit.transform.gameObject;

            if (_draggedObject == null)
                _draggableHighlight.enabled = true;
            else
                _draggableHighlight.enabled = false;
        }
        else
        {
            _draggableHighlight.enabled = false;
        }
            
        // right click
        if (Input.GetMouseButton(1))
        {
            if (dragDebounce)
            {
                if (_draggedObject == null)
                {

                    if (!hitSomething)
                        return;

                    if (hit.transform.gameObject.tag != "Draggable")
                        return;

                    _draggedObject = hit.transform.gameObject;

                    _draggedObjectRB = _draggedObject.GetComponent<Rigidbody>();
                    _previousGravity = _draggedObjectRB.useGravity;
                    _draggedObjectRB.useGravity = false;
                    _draggedObjectRB.freezeRotation = true;
                }

                _dragDestination = Camera.main.transform.position + Camera.main.transform.forward * dragDistance;
                //_dragDestination += _dragDestination.normalized * _draggedObject.transform.lossyScale.magnitude/2;
                Vector3 objPos = _draggedObject.transform.position;
                Vector3 distVec = (_dragDestination - _draggedObject.transform.position);

                //float multipier = distVec.magnitude - _draggedObjectRB.velocity.magnitude;

                Vector3 objAcc = _draggedObjectRB.velocity;
                Vector3 forceVector = (distVec * dragForce - objAcc);
                //Vector3 forceVector = (distVec * distVec.magnitude  - objAcc.normalized * objAcc.magnitude * objAcc.magnitude) * dragForce;
                /*
                if (distVec.magnitude > .1f)
                {
                    forceVector =  - ;
                }
                else
                {
                    forceVector = - _draggedObjectRB.velocity;
                }*/

                _draggedObjectRB.AddForce(forceVector, ForceMode.VelocityChange);
                //_draggedObjectRB.AddTorque(_draggedObjectRB.rotation.eulerAngles * _draggedObjectRB.mass * Time.deltaTime);
            }
        }
        else
        {
            dragDebounce = true;
            if (_draggedObject != null)
            {
                DropDrag();
            }
        }
    }

    public void DropDrag()
    {
        dragDebounce = true;
        if (_draggedObjectRB != null)
        {
            _draggedObjectRB.useGravity = _previousGravity;
            _draggedObjectRB.freezeRotation = false;
        }
        _draggedObjectRB = null;
        _draggedObject = null;
    }

    private void LateUpdate()
    {
        _neckTrans.localRotation = _lastUpdateRotate;
        //Camera.main.transform.rotation = _lastUpdateRotate;
        Camera.main.transform.position = _headTrans.position + _headTrans.up * cameraHeadOffset;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Rigidbody rb;
        Debug.Log("collison:" + collision.gameObject.name);
        if ((rb = collision.gameObject.GetComponent<Rigidbody>()) != null)
        {
            Vector3 playerPush = _playerTrans.transform.forward * _prevSpeed;
            rb.AddForce(playerPush);
            Debug.Log("push");
        }
    }
    public static (bool, RaycastHit) getLookRay()
    {
        Vector3 start = Camera.main.transform.position + Camera.main.transform.forward * headRaycastOffset;
        Vector3 direction = Camera.main.transform.forward;
        RaycastHit hit;
        bool found = Physics.Raycast(start, direction, out hit, LayerMask.NameToLayer("Ground"));
        return (found, hit);
    }
}

/*
 * Authors: Ryan Coughlin
 * Class: CS-583 Price, Group 13
 * Desc: This class controls player input.
 * 
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    public float walkSpeed = 3f;

    public float runSpeed = 7f;

    public float mouseSensitivity = 10f;


    public Transform groudCheck;

    public float groundDistance;

    private float _braceLandDistance;

    public LayerMask groundMask;

    public float gravity;

    public float jumpHeight;

    private float _x, _z;


    private float _xRotation = 0f;

    private Vector3 _velocity;

    private bool _isGrounded;

    private Transform _playerTrans;

    private CharacterController _characterController;

    private Animator _animator;

    // Start is called before the first frame update
    void Start()
    {
        InitReferences();

        Cursor.lockState = CursorLockMode.Locked;
        _characterController.enabled = true;

        _xRotation = Camera.main.transform.localRotation.x;

        _braceLandDistance = -gravity * Time.fixedDeltaTime * 10;
    }

    private void InitReferences()
    {
        _playerTrans = GetComponent<Transform>();
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

        _isGrounded = Physics.CheckSphere(groudCheck.position, groundDistance, groundMask);

        bool braceLand = Physics.CheckSphere(groudCheck.position, _braceLandDistance, groundMask);

        if (_isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f;
        }

        _x = Input.GetAxis("Horizontal");
        _z = Input.GetAxis("Vertical");

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        float moveSpeed = 0;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            moveSpeed = runSpeed;
        } else
        {
            moveSpeed = walkSpeed;
        }

        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

        Camera.main.transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
        _playerTrans.Rotate(Vector3.up * mouseX);

        Vector3 move = _playerTrans.transform.forward * _z + _playerTrans.transform.right * _x;
        move = Vector3.ClampMagnitude(move, moveSpeed);

        _characterController.Move(moveSpeed * Time.deltaTime * move);

        _velocity.y += gravity * Time.deltaTime;

        if (Input.GetButtonDown("Jump") && _isGrounded)
        {
            _velocity.y += Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        _characterController.Move(_velocity * Time.deltaTime);

        _animator.SetFloat("Speed", (moveSpeed * move).magnitude);
        _animator.SetFloat("Forward", _z);
        _animator.SetFloat("Right", _x);
        _animator.SetFloat("Vertical", _velocity.y);
        _animator.SetBool("Grounded", _isGrounded);
        _animator.SetBool("BraceLand", braceLand);
    }
}

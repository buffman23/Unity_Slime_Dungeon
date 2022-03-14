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
    public float moveSpeed = 10f;

    public float mouseSensitivity = 10f;

    public Transform playerBody;

    public CharacterController playerController;

    public Transform groudCheck;

    public float groundDistance;

    public LayerMask groundMask;

    public float gravity;

    public float jumpHeight;

    private float _x, _z;

    private float _xRotation = 0f;

    private Vector3 _velocity;

    private bool _isGrounded;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        playerController.enabled = true;

        _xRotation = Camera.main.transform.localRotation.x;
    }

    // Update is called once per frame
    void Update()
    {

        _isGrounded = Physics.CheckSphere(groudCheck.position, groundDistance, groundMask);

        if(_isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f;
        }

        _x = Input.GetAxis("Horizontal");
        _z = Input.GetAxis("Vertical");

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime; ;

        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

        Camera.main.transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);

        Vector3 move = playerBody.transform.forward * _z + playerBody.transform.right * _x;

        playerController.Move(moveSpeed * Time.deltaTime * move);

        _velocity.y += gravity * Time.deltaTime;

        if (Input.GetButtonDown("Jump") && _isGrounded)
        {
            _velocity.y += Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        playerController.Move(_velocity * Time.deltaTime);
    }
}

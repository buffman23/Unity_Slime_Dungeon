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
using UnityEngine.SceneManagement;


public class PlayerController : MonoBehaviour
{
    public float cameraHeadOffset;
    public static float headRaycastOffset;
    public float dragDistance;

    public LivesCountController livesCountController;

    private Transform _respawnTrans;

    public static PlayerController instance;

    public MapController mapController;

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

    private Transform _neckTrans, _headTrans, _spineTrans, _chestTrans;

    private Quaternion _lastUpdateRotate;

    private bool _previousGravity;

    private Image _draggableHighlight, _keyImage;

    // updated by MapController which tracks players current room
    [HideInInspector]
    public Room currentRoom;

    private bool _hasKey;

    private Hashtable potentialHitSources = new Hashtable();

    public int playerHealth = 100;

    private int numOfLives = 3;


    public float mass = 3.0f; // defines the character mass
    private Vector3 _impact = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        InitReferences();
        potentialHitSources.Add("Slime", "10");
        potentialHitSources.Add("BombSlime", "50");
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
        _spineTrans = transform.Find("PlayerRig/Armature/Hips/Spine").transform;
        _neckTrans = transform.Find("PlayerRig/Armature/Hips/Spine/Chest/UpperChest/Neck").transform;
        _chestTrans = transform.Find("PlayerRig/Armature/Hips/Spine/Chest").transform;
        _headTrans = _neckTrans.Find("Head");
        _respawnTrans = transform.Find("Respawn");
        if (mapController == null)
            mapController = MapController.instance;
        if (livesCountController == null)
            livesCountController = LivesCountController.instance;
        _draggableHighlight = GameObject.Find("DraggableHighlight").GetComponent<Image>();
        _keyImage = GameObject.Find("KeyImage").GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        _isGrounded = Physics.CheckSphere(groudCheck.position, groundDistance, groundMask);

        bool braceLand = Physics.CheckSphere(groudCheck.position, _braceLandDistance, groundMask);

        if (_isGrounded && _velocity.y < -2f && !fly)
        {
            _velocity.y = -2f;
            _impact.y = 0;
            _impact.y = 0;
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
            if (!_isGrounded)
            {
                _velocity.y += gravity * Time.deltaTime;
            }

            if (Input.GetButtonDown("Jump") && _isGrounded)
            {

                _velocity.y += Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }

        _characterController.Move((_impact +_velocity) * Time.deltaTime);

        _animator.SetFloat("Speed", (moveSpeed * move).magnitude);
        _animator.SetFloat("Forward", _z);
        _animator.SetFloat("Right", _x);
        _animator.SetFloat("Vertical", Mathf.Abs(_velocity.y));
        _animator.SetBool("Grounded", _isGrounded);
        _animator.SetBool("BraceLand", braceLand);

        updateAttack();

        // apply the _impact force:
        //if (_impact.magnitude > 0.2) _characterController.Move(_impact * Time.deltaTime);
        // consumes the _impact energy each cycle:

        _impact = Vector3.Lerp(_impact, Vector3.zero, Time.deltaTime);
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
        _spineTrans.localRotation = _lastUpdateRotate;
        //Camera.main.transform.rotation = _lastUpdateRotate;
        Camera.main.transform.position = _headTrans.position + _headTrans.up * cameraHeadOffset;
    }

    private void OnTriggerEnter(Collider collider)
    {
        Rigidbody rb;
        Enemy enemy = collider.gameObject.GetComponent<Enemy>();
        if (enemy == null)
        {
            if (collider.transform.parent == null)
                return;
            enemy = collider.transform.parent.GetComponent<Enemy>();
            if (enemy == null)
                return;
        }
        //Debug.Log("collison:" + collider.gameObject.name);
        if ((rb = collider.gameObject.GetComponent<Rigidbody>()) != null)
        {
            Vector3 playerPush = _playerTrans.transform.forward * _prevSpeed;
            rb.AddForce(playerPush);
            Debug.Log("push");
        }
        // need to get the damage of the enemy that hits the player
        // check who made the collision
        

       
    }
    // checks if the players health goes below 0
    public void isOutOfHealth()
    {
        // two cases: player is out of lives and player must go back to beginning; player has more lives, decrease num of lives, go back to beginning state of room
            if (numOfLives - 1 == 0)
            {
                // back to main menu
                SceneManager.LoadScene("Main_menu_scene");
            }
            // send player back to previous room
            else
            {
                livesCountController.decrementNumOfLives(--numOfLives);
                playerHealth = 100;
                HealthBarController.instance.changeHealthBar(playerHealth);
            // in the first room
            if (currentRoom.previousRoom == null)
            {
                bool wasEnabled = _characterController.enabled;
                _characterController.enabled = false;
                transform.position = currentRoom.transform.position;
                _characterController.enabled = wasEnabled;
            }
            else
            {
                bool wasEnabled = _characterController.enabled;
                _characterController.enabled = false;
                transform.position = currentRoom.previousRoom.transform.position;
                _characterController.enabled = wasEnabled;
            }
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

    // call this function to add an _impact force:
    public void AddImpact(Vector3 dir, float force)
    {
        dir.Normalize();
        if (dir.y < 0) dir.y = -dir.y; // reflect down force on the ground
        _impact += dir.normalized * force / mass;
    }


    public void damage(int hitDamage)
    {
        playerHealth -= hitDamage;
        Debug.Log(playerHealth);
        if (playerHealth <= 0)
            isOutOfHealth();
        HealthBarController.instance.changeHealthBar(playerHealth);
    }

    public bool hasKey()
    {
        return _hasKey;
    }

    public void setHasKey(bool b)
    {
        _hasKey = b;
        _keyImage.enabled = _hasKey;

    }
}

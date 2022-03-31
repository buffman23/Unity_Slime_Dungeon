using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{

    public float tryForce;

    private bool _insertReady = false;

    private bool _rotationReady = false;

    private bool _orientationReady = false;

    private bool _inserting = false;

    private bool _rotate = false;

    private bool _openDoor = false;

    private GameObject _key;

    private Rigidbody _keyRB;

    private Transform _keyholeTrans;

    private Vector3 _destination;

    private Animator _animator;

    

    // Start is called before the first frame update
    void Start()
    {
        _keyholeTrans = transform.Find("KeyholePos");
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (_insertReady)
        {

            if (_rotationReady && _orientationReady)
            {
                _inserting = true;
                _destination = _keyholeTrans.position;
            }

            Vector3 deltaVec = _destination - _key.transform.position;

            

            if (deltaVec.magnitude > .1)
            {
                _keyRB.useGravity = false;
                PlayerController.instance.dragDebounce = false;
                Vector3 distVec = (_destination - _key.transform.position);

                Vector3 objAcc = _keyRB.velocity;
                Vector3 forceVector = (distVec * tryForce - objAcc);

                Debug.Log("Adding force" + forceVector.magnitude);


                _keyRB.AddForce(forceVector, ForceMode.VelocityChange);

                _orientationReady = false;
            } else
            {
                if (_inserting)
                {
                    _insertReady = false;
                    _rotate = true;
                    Destroy(_keyRB);
                    _key.transform.SetParent(transform.Find("Armature"));
                    _openDoor = true;
                    _animator.SetBool("OpenDoor", true);
                    //this.enabled = false;
                }
                _orientationReady = true;
            }

            /*
             * SOURCE : https://gamedev.stackexchange.com/questions/182850/rotate-rigidbody-to-face-away-from-camera-with-addtorque
             */
            // Compute the change in orientation we need to impart.
            Quaternion rotationChange = _keyholeTrans.rotation * Quaternion.Inverse(_keyRB.rotation);

            // Convert to an angle-axis representation, with angle in range -180...180
            rotationChange.ToAngleAxis(out float angle, out Vector3 axis);
            if (angle > 180f)
                angle -= 360f;

            // If we're already facing the right way, just stop.
            // This avoids problems with the infinite axes ToAngleAxis gives us in this case.
            if (Mathf.Approximately(angle, 0))
            {
                _rotationReady = true;
                _keyRB.angularVelocity = Vector3.zero;
                return;
            }
            else
            {
                _rotationReady = false;
            }

            // If you need to, you can enforce a cap here on the maximum rotation you'll
            // allow in a single step, to prevent overly jerky movement from upsetting your sim.
            // angle = Mathf.Clamp(angle, -90f, 90f);

            // Convert to radians.
            angle *= Mathf.Deg2Rad;

            // Compute an angular velocity that will bring us to the target orientation
            // in a single time step.
            var targetAngularVelocity = axis * angle / Time.deltaTime;

            // You can reduce this parameter to smooth the movement over multiple time steps,
            // to help reduce the effect of sudden jerks.
            float catchUp = .1f;
            targetAngularVelocity *= catchUp;

            // Apply a torque to finish the job.
            _keyRB.AddTorque(targetAngularVelocity - _keyRB.angularVelocity, ForceMode.VelocityChange);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Door triggered with " + other.gameObject.name);
        if (_key == null && other.gameObject.name.Equals("Key"))
        {
            Debug.Log("Key touched keyhole");
            _key = other.gameObject;
            _keyRB = _key.transform.GetComponent<Rigidbody>();
            _keyRB.useGravity = false;
            //_keyRB.freezeRotation = true;
            _key.transform.SetParent(null);
            _destination = _keyholeTrans.position - _keyholeTrans.forward * .7f;

            PlayerController.instance.DropDrag();

            _insertReady = true;

            Collider collider1 = transform.Find("Armature").GetComponent<Collider>();
            Collider collider2 = _key.transform.GetComponent<Collider>();

            Physics.IgnoreCollision(collider1, collider2);


        }
    }
}

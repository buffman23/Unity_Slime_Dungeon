using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Slime : Enemy
{
    private float _jumpCurrentTime = 0f;
    private static float _jumpWaitTime = 2f;
    private static float _distToGround = 1f;

    private Rigidbody _rigidBody;

    private bool _isGrounded;

    private float _halfHeight;

    private Vector3[] _waypoints;

    private ParticleSystem _particles;

    private GameObject _body;

    private LinkedList<GameObject> _floorTriggers = new LinkedList<GameObject>();

    // Start is called before the first frame update
    protected override void Start() 
    {
        base.Start();

        InitReferences();

        //_rigidBody.freezeRotation = true;
        _halfHeight = transform.lossyScale.y / 2f;

        // init random starting _jumpCurretnTime so not all simes jump at same time.
        _jumpCurrentTime = Random.Range(-_jumpWaitTime, 0);
    }

    private void InitReferences()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _particles = transform.Find("Particle System").GetComponent<ParticleSystem>();
        _body = transform.Find("Body").gameObject;
    }

    // Update is called once per frame
    //protected override void Update()
    //{
    //    base.Update();
    //}

    private void Update()
    {
        if (_dead)
            return;

        if (_isGrounded)
        {
            _jumpCurrentTime += Time.deltaTime;

            //if (_waypoints != null)
            //{
            //    for (int i = 0; i < _waypoints.Length - 1; i++)
            //        Debug.DrawLine(_waypoints[i], _waypoints[i + 1], Color.red);
            //}

            if (_jumpCurrentTime >= _jumpWaitTime)
            {
                // now jump since cooldown is reached
                _jumpCurrentTime = 0f;

                var path = new NavMeshPath();
                Vector3 destination = PlayerController.instance.transform.position;
                destination.y = transform.position.y;
                NavMesh.CalculatePath(transform.position, destination, NavMesh.AllAreas, path);

                

                _waypoints = path.corners;

                if (_waypoints.Length > 0)
                {
                    Vector3 lookVector = new Vector3(_waypoints[1].x, transform.position.y, _waypoints[1].z);
                    transform.LookAt(lookVector);

                    
                }
                else
                {
                    Vector3 playerPosition = _playerController.transform.position;
                    Vector3 lookVector = new Vector3(playerPosition.x, transform.position.y, playerPosition.z);
                    transform.LookAt(lookVector);
                }

                _rigidBody.AddForce(transform.up * 5f, ForceMode.VelocityChange);
                _rigidBody.AddForce(transform.forward * 5f, ForceMode.VelocityChange);

                _isGrounded = false;
            }
        }
        else
        {
            _isGrounded = IsGrounded();
        }
    }

    private bool IsGrounded()
    {
        //return Physics.Raycast(transform.position, -Vector3.up, _halfHeight + 0.1f);
        return _floorTriggers.Count > 0;
    }

    public override void Kill()
    {
        base.Kill(false);
        Destroy(_body);
        StartCoroutine(ParticleDeath());
    }

    private void OnTriggerEnter(Collider other)
    {
        _floorTriggers.AddFirst(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        _floorTriggers.Remove(other.gameObject);
    }

    IEnumerator ParticleDeath()
    {
        _particles.Play();

        yield return new WaitForSeconds(3);

        base.Kill(true);
    }
}

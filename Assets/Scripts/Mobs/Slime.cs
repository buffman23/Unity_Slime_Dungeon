using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Slime : Enemy
{
    protected float _jumpCooldownTime = 0f;
    private float _damageCoolDownTime = 0f;
    private float _stuckTime = 0f;
    protected float _jumpCooldownMaxTime = 2f;
    private static float _maxStuckTime = 3f;
    private int damage = 10;
    private int health = 100;
    private AudioClip[] _clips;
    private AudioSource _AS;

    private Rigidbody _rigidBody;

    private bool _isGrounded;

    private float _halfHeight;

    private Vector3[] _waypoints;

    private ParticleSystem _particles;

    private GameObject _body;

    private Animator _animator;

    private LinkedList<GameObject> _floorTriggers = new LinkedList<GameObject>();

    private bool _lastIsGrounded;

    private float _maxGroundedCooldown = .5f, _groundedCooldown = 0f;

    // Start is called before the first frame update
    protected override void Start() 
    {
        base.Start();

        InitReferences();

        //_rigidBody.freezeRotation = true;
        _halfHeight = transform.lossyScale.y / 2f;

        // init random starting _jumpCurretnTime so not all simes jump at same time.
        _jumpCooldownTime = Random.Range(-_jumpCooldownMaxTime, 0);
    }

    private void InitReferences()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _particles = transform.Find("Particle System").GetComponent<ParticleSystem>();
        _body = transform.Find("Body").gameObject;
        _animator = GetComponent<Animator>();
        _clips = Resources.LoadAll<AudioClip>("Sounds/Slime");
        _AS = gameObject.AddComponent<AudioSource>();
    }

    // Update is called once per frame
    //protected override void Update()
    //{
    //    base.Update();
    //}

    protected override void Update()
    {
        if (_dead)
            return;

        _stuckTime += Time.deltaTime;

        _damageCoolDownTime += Time.deltaTime;

        if (_isGrounded || _stuckTime >= _maxStuckTime)
        {
            _jumpCooldownTime += Time.deltaTime;

            //if (_waypoints != null)
            //{
            //    for (int i = 0; i < _waypoints.Length - 1; i++)
            //        Debug.DrawLine(_waypoints[i], _waypoints[i + 1], Color.red);
            //}

            if (_jumpCooldownTime >= _jumpCooldownMaxTime)
            {
                // now jump since cooldown is reached
                _jumpCooldownTime = 0f;
                _stuckTime = 0f;

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



        _isGrounded = IsGrounded();

        if(_isGrounded && !_lastIsGrounded && _groundedCooldown >= _maxGroundedCooldown)
        {
            _groundedCooldown = 0f;
            _AS.clip = GetMoveSound();
            _AS.Play();
            //Debug.Log("Landed");
        }

        if (!_isGrounded && _lastIsGrounded && _groundedCooldown >= _maxGroundedCooldown)
        {
            _groundedCooldown = 0f;
            _AS.clip = GetMoveSound();
            _AS.Play();
            //Debug.Log("Jumped");
        }

        _groundedCooldown += Time.deltaTime;

        _lastIsGrounded = _isGrounded;

        _animator.SetBool("OnGround", _isGrounded);
    }

    private AudioClip GetMoveSound()
    {
        int randIdx = Random.Range(0, _clips.Length / 2);
        return _clips[randIdx];
    }

    private AudioClip GetDamageSound()
    {
        int randIdx = Random.Range(_clips.Length / 2, _clips.Length);
        return _clips[randIdx];
    }

    public bool IsGrounded()
    {
        Vector3 corner = transform.position - transform.lossyScale / 2f;
        float length = transform.lossyScale.x;
        float width = transform.lossyScale.z;

        if (Physics.Raycast(corner, -Vector3.up, 0.1f))
            return true;

        if (Physics.Raycast(corner + new Vector3(length, 0f, width), -Vector3.up, 0.1f))
            return true;

        if (Physics.Raycast(corner + new Vector3(length, 0f, 0f), -Vector3.up, 0.1f))
            return true;

        if (Physics.Raycast(corner + new Vector3(0f, 0f, width), -Vector3.up, 0.1f))
            return true;

        return false;
    }

    public override void Kill(bool destroy)
    {
        base.Kill(false);
        Destroy(_body);
        _rigidBody.isKinematic = true;
        StartCoroutine(ParticleDeath());

    }

    public void damageSlime(int damage)
    {
        health -= damage;
        _AS.clip = GetDamageSound();
        _AS.Play();
        if (health <= 0)
        {
            Kill(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        _floorTriggers.AddFirst(other.gameObject);
        PlayerController player = other.gameObject.GetComponent<PlayerController>();
        
        if (player != null && _damageCoolDownTime >= 1)
        {
            player.damage((int)(damage * GameController.instance.difficulty));
            _damageCoolDownTime = 0;
            PlayerController.instance.AddImpact(other.gameObject.transform.up * 5f, 250f);
            PlayerController.instance.AddImpact(gameObject.transform.forward * 5f, 250f);

        }
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

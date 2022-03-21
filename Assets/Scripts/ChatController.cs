using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ChatController : MonoBehaviour
{


    private GameObject _chat;
    private InputField _inputField;
    private Text _outputText;
    private PlayerController _playerController;
    private CanvasGroup _chatCanvasGroup, _inputCanvasGroup;
    private Animator _animator;
    private LinkedList<GameObject> _chatTexts;
    private GameObject _chatTextPrefab;
    private GameObject _outputArea;
    private GameObject _player;
    private Vector2 _defaultPlayerSpeed;
    private float _defaultPlayerJump;

    private bool _chatOpen = false;

    private const int _MAX_LINES = 12;

    private int uid = 0;

    void Start()
    {
        _chatTexts = new LinkedList<GameObject>();
        InitReferences();

        _defaultPlayerSpeed = new Vector2(_playerController.walkSpeed, _playerController.runSpeed);
        _defaultPlayerJump = _playerController.jumpHeight;
    }

    private void InitReferences()
    {
        _player = GameObject.Find("Player");
        _chat = GameObject.Find("Chat");
        _outputArea = GameObject.Find("OutputArea");
        _chatCanvasGroup = _chat.GetComponent<CanvasGroup>();
        _inputCanvasGroup = GameObject.Find("InputField").GetComponent<CanvasGroup>();
        _inputField = _chat.GetComponentInChildren<InputField>();
        //_outputText = GameObject.Find("OutputText").GetComponent<Text>();

        _playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        _animator = _chat.GetComponent<Animator>();
        _chatTextPrefab = Resources.Load<GameObject>("Prefabs/ChatText");
    }

    // Update is called once per frame
    void Update()
    {
        if (!_chatOpen)
        {
            if (Input.GetKey(KeyCode.Slash))
            {

                _chatOpen = true;
                _inputField.enabled = true;
                _inputField.ActivateInputField();
                _playerController.enabled = false;
                _animator.SetBool("ChatOpen", true);
                _inputCanvasGroup.alpha = 1;
            }
            
        }
        else if (Input.GetKey(KeyCode.Return))
        {
            _chatOpen = false;
            _inputField.DeactivateInputField();
            _inputField.enabled = false;
            write('/' + _inputField.text);
            _inputField.text = "";
            _playerController.enabled = true;
            //_inputCanvasGroup.alpha = 0;
            _animator.SetBool("ChatOpen", false);
        }
    }

    public void write(string message)
    {
        if (message.Length == 0)
            return;

        if (message[0] == '/')
        {
            if(message.Length > 1)
                processCommand(message.Substring(1));
            return;
        }

        GameObject chatText = Instantiate<GameObject>(_chatTextPrefab);
        Text text = chatText.GetComponent<Text>();
        text.text = message;

        chatText.transform.SetParent(_outputArea.transform);

        _chatTexts.AddLast(chatText);

        if (_chatTexts.Count > _MAX_LINES)
        {
            GameObject oldestText = _chatTexts.First.Value;
            _chatTexts.RemoveFirst();
            Destroy(oldestText);
        }
    }

    void processCommand(string command)
    {
        string[] split = command.Split(' ');
        switch(split[0])
        {
            case "help":
                write("cube [size] - spawns cube in front of player");
                write("ball [size] - spawns ball in frotn of player");
                write("del - delete object being looked at");
                write("ramp [deg incline] [len] - on args will make ramp to cursor");
                write("speed [walkspeed]/reset - change player movement speed");
                write("jump [jump height]/reset - change player jump height");
                write("chat stay/fade - makes chat window stay or fade after closed");
                write("cam [offset] - offset camera to get 3rd person view. 0 to go back to 1st person.");
                break;

            case "cube":
            case "ball":
                PrimitiveType pt;
                string objectName;
                if (split[0].Equals("cube"))
                {
                    pt = PrimitiveType.Cube;
                    objectName = "Cube";
                }
                else
                { 
                    pt = PrimitiveType.Sphere;
                    objectName = "Ball";
                }

                float scale = 1f;
                if(split.Length > 1)
                    float.TryParse(split[1], out scale);

                GameObject cube = GameObject.CreatePrimitive(pt);
                cube.name = objectName + '_' + uid++;
                cube.transform.localScale = Vector3.one * scale;
                cube.layer = LayerMask.NameToLayer("Ground");
                cube.AddComponent<Rigidbody>();
                cube.AddComponent<BoxCollider>();                

                Vector3 spawn_position = _player.transform.position + _player.transform.forward * scale;
                spawn_position.y += scale;
                cube.transform.position = spawn_position;
                cube.transform.rotation = _player.transform.rotation;
                write("Spawned a " + split[0] + '.');
                break;
            case "ramp":
                float rotation = 30f;
                scale = 10f;

                if (split.Length == 1)
                {
                    var result1 = getLookRay();
                    bool found1 = result1.Item1;

                    if (found1)
                    {
                        RaycastHit hit = result1.Item2;
                        Vector3 hit_position = hit.point;
                        Vector3 player_position = _player.transform.position;
                        scale = (hit_position -player_position).magnitude;
                        float rise = hit_position.y - player_position.y;
                        rotation = Mathf.Asin(rise/scale) * Mathf.Rad2Deg;
                        write("hit pos: " + hit_position);
                        write("cross: " + (Vector3.Cross(hit_position, _player.transform.position).magnitude).ToString());
                    }
                }

                if (split.Length > 1)
                    float.TryParse(split[1], out rotation);

                if (split.Length > 2)
                    float.TryParse(split[2], out scale);

                cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.name = "Ramp_" + uid++;
                cube.transform.localScale = new Vector3(1f, .5f, scale);
                cube.transform.rotation = _player.transform.rotation;
                cube.transform.Rotate(new Vector3(-rotation, 0f, 0f), Space.Self);
                
                cube.layer = LayerMask.NameToLayer("Ground");
                cube.AddComponent<BoxCollider>();

                spawn_position = _player.transform.position + _player.transform.forward * scale/2 * Mathf.Cos(rotation * Mathf.Deg2Rad);
                spawn_position.y = _player.transform.position.y - _player.transform.localScale.y/2 + scale/2 * Mathf.Sin(rotation * Mathf.Deg2Rad);
                cube.transform.position = spawn_position;
                write("Spawned a ramp.");
                break;
            case "del":
                // source: https://answers.unity.com/questions/1119002/how-can-i-get-an-object-reference-from-a-raycast.html

                var result = getLookRay();
                bool found = result.Item1;

                if (found)
                {
                    RaycastHit hit = result.Item2;
                    string name = hit.collider.gameObject.name;
                    Destroy(hit.collider.gameObject);
                    write("Deleted " + name);
                }
                else
                {
                    write("Nothing to delete.");
                }
                break;

            case "speed":
                if (split.Length > 1)
                {
                    if (split[1].Equals("reset"))
                    {
                        _playerController.walkSpeed = _defaultPlayerSpeed.x;
                        _playerController.runSpeed = _defaultPlayerSpeed.y;
                        write("Reset player speed.");
                    }
                    else
                    {
                        try
                        {
                            float runWalkRatio = _playerController.runSpeed / _playerController.walkSpeed;
                            _playerController.walkSpeed = float.Parse(split[1]);
                            _playerController.runSpeed = _playerController.walkSpeed * runWalkRatio;
                            write(string.Format("Set player walk speed: {0}, run speed {1:0.00}", _playerController.walkSpeed, _playerController.runSpeed));
                        }
                        catch (System.FormatException e)
                        {
                            write("speed [walkspeed]/reset");
                        }
                    }
                }
                break;
            case "jump":
                if (split.Length > 1)
                {
                    if (split[1].Equals("reset"))
                    {
                        _playerController.jumpHeight = _defaultPlayerJump;
                        write("Reset player jump height.");
                    }
                    else
                    {
                        try
                        {
                            _playerController.jumpHeight = float.Parse(split[1]);
                            write(string.Format("Set player jump height: {0:0.00},", _playerController.jumpHeight));
                        }
                        catch (System.FormatException e)
                        {
                            write("jump [jump height]/reset");
                        }
                    }
                }
                break;
            case "chat":
                if(split.Length > 1)
                {
                    if (split[1].Equals("stay"))
                    {
                        _animator.SetBool("Stay", true);
                        write("chat set to stay");
                    }
                    else if(split[1].Equals("fade"))
                    {
                        _animator.SetBool("Stay", false);
                        write("chat set to fade");
                    }
                }
                break;

            case "cam":
                try
                {
                    Vector3 cam_position = Camera.main.transform.localPosition;
                    Camera.main.transform.localPosition = new Vector3(cam_position.x, cam_position.y, float.Parse(split[1]));
                    write("Set camera position to:" + Camera.main.transform.localPosition.ToString());
                }
                catch (System.FormatException e)
                {

                }
                break;

            default:
                write("Invalid command \"" + command + '\"');
                break;
        }
    }

    private (bool, RaycastHit) getLookRay()
    {
        Vector3 start = Camera.main.transform.position;
        Vector3 direction = Camera.main.transform.forward;
        RaycastHit hit;
        bool found = Physics.Raycast(start, direction, out hit);
        return (found, hit);
    }
}

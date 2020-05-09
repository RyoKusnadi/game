using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Animator _animator;
    private CharacterController _characterController;
    public float Speed = 5.0f;
    public float RotationSpeed = 240.0f;
    public float JumpForce = 8.0f;
    private float Gravity = 14.0f;
    private Vector3 _moveDir = Vector3.zero;

    void Start()
    {
        _animator = GetComponent<Animator>();
        _characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        Movement();
        GetInput();
    }

    void Movement() { 
        // Get Input for axis
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // Calculate the forward vector
        Vector3 camForward_Dir = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 move = v * camForward_Dir + h * Camera.main.transform.right;

            if (move.magnitude > 1f) move.Normalize();
            // Calculate the rotation for the player
            move = transform.InverseTransformDirection(move);
            // Get Euler angles
            float turnAmount = Mathf.Atan2(move.x, move.z);
            transform.Rotate(0, turnAmount* RotationSpeed * Time.deltaTime, 0);

        /*
            if (_characterController.isGrounded)
            {
                if (Input.GetKey(KeyCode.W)|| Input.GetKey(KeyCode.A)|| Input.GetKey(KeyCode.S)|| Input.GetKey(KeyCode.D))
                {
                    if(_animator.GetBool ("attacking")== true)
                    {
                    return;
                    }
                    else if (_animator.GetBool("attacking") == false) 
                    {
                    _animator.SetBool("running", true);
                    _animator.SetInteger("condition", 1);
                    }
                }
                if (Input.GetKeyUp(KeyCode.W)|| Input.GetKeyUp(KeyCode.A)|| Input.GetKeyUp(KeyCode.S)|| Input.GetKeyUp(KeyCode.D))
                {
                    _animator.SetBool("running", false);
                    _animator.SetInteger("condition", 0);
                }
                _moveDir = transform.forward* move.magnitude;
                _moveDir *= Speed;
            }
            */
        if (_characterController.isGrounded)
        {
            if (GetComponent<Rigidbody>().velocity == Vector3.zero)
            {
                _animator.SetBool("running", false);
                _animator.SetInteger("condition", 0);
            }
            else
            {
                _animator.SetBool("running", true);
                _animator.SetInteger("condition", 1);


            }
            _moveDir = transform.forward * move.magnitude;
            _moveDir *= Speed;
        }

        if (_characterController.isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            _moveDir.y = JumpForce;
            //_animator.SetBool("run", move.magnitude > 0);
        }
            _moveDir.y -= Gravity* Time.deltaTime;
        _characterController.Move(_moveDir* Time.deltaTime);
    }

    void GetInput()
    {
        if (_characterController.isGrounded)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (_animator.GetBool("running") == true)
                {
                    _animator.SetBool("running", false);
                    //_animator.SetInteger("condition", 0);
                    Attacking();

                }
                else if (_animator.GetBool("running") == false)
                {
                    Attacking();
                }
            }
        }
    }

    void Attacking()
    {
        StartCoroutine(AttackRoutine());
    }

    IEnumerator AttackRoutine()
    {
        _animator.SetBool("attacking", true);
        _animator.SetInteger("condition", 2);
        yield return new WaitForSeconds(1);
        _animator.SetInteger("condition", 0);
        _animator.SetBool("attacking", false);
    }
}

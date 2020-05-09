using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float movementSpeed;
    public float velocity;
    public Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
        Move();
    }

    void GetInput()
    {
        // Move Left
        if (Input.GetKey(KeyCode.A))
        {
            SetVelocity(-1);
        }
        else if (Input.GetKeyUp(KeyCode.A))
        {
            SetVelocity(0);
        }

        // Move Right
        if (Input.GetKey(KeyCode.D))
        {
            SetVelocity(1);
        }
        else if (Input.GetKeyUp(KeyCode.D))
        {
            SetVelocity(0);
        }
    }

    void Move()
    {
        rb.MovePosition(transform.position + (Vector3.right * velocity *movementSpeed* Time.deltaTime));
    }

    void SetVelocity(float dir)
    {
        velocity = dir;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamController : MonoBehaviour
{
    public Vector3 cameraStartPos;
    public float speed = 5;
    // Start is called before the first frame update
    void Start()
    {
        transform.position = cameraStartPos;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 velocity = Vector3.zero;
        if(Input.GetKey(KeyCode.W))
        {
            velocity += Vector3.forward;
        }
        if(Input.GetKey(KeyCode.A))
        {
            velocity += Vector3.left;
        }
        if(Input.GetKey(KeyCode.S))
        {
            velocity += Vector3.back;
        }
        if(Input.GetKey(KeyCode.D))
        {
            velocity += Vector3.right;
        }
        if(Input.GetKey(KeyCode.Q))
        {
            velocity += Vector3.up;
        }
        if(Input.GetKey(KeyCode.E))
        {
            velocity += Vector3.down;
        }
        velocity.Normalize();
        transform.position += velocity * Time.deltaTime * speed;
    }
}

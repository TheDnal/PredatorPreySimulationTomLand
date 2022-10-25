using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicWalk : MonoBehaviour
{
    private float speed = 1;
    private Vector3 velocity = Vector3.zero;
    private Rigidbody rb;
    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        StartCoroutine(i_Wander());
    }
    void Update()
    {
        rb.velocity = velocity * speed;
    }
    private IEnumerator i_Wander()
    {
        while(true)
        {  
            int x = Random.Range(-2,2);
            int y = Random.Range(-2,2);
            velocity = new Vector3(x,0,y);
            yield return new WaitForSeconds(1);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class controlCapsule : MonoBehaviour
{
    public float speed = 3;
    public float angularSpeed =5;
    public int number = 1;//胶囊的编号，用于控制不同的胶囊
    private KeyCode Space = KeyCode.Space;

    private Rigidbody capsule;
	// Use this for initialization
	void Start ()
	{
	    capsule = this.GetComponent<Rigidbody>();
	    
	}
	
	// Update is called once per frame
	void Update ()
	{
	    //float v = Input.GetAxis("VerticalCap"+number);
	    //capsule.velocity = transform.forward*v * speed;

	    //float h = Input.GetAxis("HorizontalCap"+number);
	    //capsule.angularVelocity = transform.up * h * angularSpeed;
	}
}

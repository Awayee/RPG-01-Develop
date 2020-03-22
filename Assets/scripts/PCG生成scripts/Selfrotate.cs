using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selfrotate : MonoBehaviour
{
    public float RotateSpeed=100;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		this.transform.Rotate(Vector3.up, RotateSpeed*Time.deltaTime);
	}
}

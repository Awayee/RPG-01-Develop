﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scriptforenemy : MonoBehaviour {

   

	// Use this for initialization
	void Start () {

   
		
	}
	
	// Update is called once per frame
	void Update () {
        

	}

    void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject,0);
        
    }

}
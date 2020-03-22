using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class viewlight : MonoBehaviour {

	// Use this for initialization
	void Start () {
        gameObject.GetComponent<Light>().color = Color.red;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}

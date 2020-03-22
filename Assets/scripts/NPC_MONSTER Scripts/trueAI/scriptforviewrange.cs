using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scriptforviewrange : MonoBehaviour {
    public GameObject selfmonster;
    private Vector3 dataposition;
    private float selfy;
	// Use this for initialization
	void Start () {
        dataposition = transform.position - selfmonster.transform.position;
        //selfy = transform.position.y;
    }
	
	// Update is called once per frame
	void Update () {
        transform.position = selfmonster.transform.position + dataposition;
        //transform.position = new Vector3(selfmonster.transform.position.x,selfy, selfmonster.transform.position.z);
		
	}
}

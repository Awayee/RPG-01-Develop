using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class ChangeLife : NetworkBehaviour {
    public GameObject Point;
    //public float MaxLife = 100;
    [Range(0, 100)]
    [SyncVar]
    public float Value;
    private Vector3 scale;
	// Use this for initialization
	void Start () {
        scale = Point.transform.localScale;
	}
	
	// Update is called once per frame
	void Update () {
        scale.x = Value / 100;
        Point.transform.localScale = scale;
	}
}

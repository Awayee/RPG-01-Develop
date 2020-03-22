using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mapcontrol : MonoBehaviour
{
    public GameObject player;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
	{
	    Vector3 i = player.transform.position;
	    i.y = this.transform.position.y;
	    this.transform.position = i;
	    this.transform.rotation = Quaternion.Euler(90,player.transform.eulerAngles.y,0);
	}
}

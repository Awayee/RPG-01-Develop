using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scriptforGuidepost : MonoBehaviour {
    public GameObject nextguidepost;//下一个路标
    private Vector3 startposition;
	// Use this for initialization
	void Start () {
        startposition = gameObject.transform.position;

    }
	
	// Update is called once per frame
	void Update () {
        gameObject.transform.position = startposition;
	}


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, nextguidepost.transform.position);
    }

}

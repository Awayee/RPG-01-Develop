using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//控制小地图摄像机移动
public class miniMapCamera : MonoBehaviour {
    Transform Player;
    Transform thisTrans;
	// Use this for initialization
	void Start () {
        Player = GameObject.FindGameObjectWithTag("Player").transform;
        thisTrans = this.transform;
        gameObject.transform.position = Player.transform.position + new Vector3(0, 100, 0);
	}
	
	// Update is called once per frame
	void LateUpdate () {
        Vector3 Trans = new Vector3(Player.position.x, thisTrans.position.y, Player.position.z);
        thisTrans.position = Trans;
	}
}

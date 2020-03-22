using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    public GameObject Player;//获得玩家
	// Use this for initialization
    void Start()
    {
        
    }
    // Update is called once per frame
	void Update () {
		transform.position= new Vector3(PlayerControl.PlayerPos.x, 10, PlayerControl.PlayerPos.z);

    }

}
	
	

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class normalattract : MonoBehaviour {
    float attract;//普攻的攻击力
	// Use this for initialization
	void Start () {
        attract = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayCharacter>().attackPower;

    }
	
	// Update is called once per frame
	void Update () {
		
	}
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "enemy")
        {
            if (other.gameObject.GetComponent<lnxAIlibrary>())
            {
                other.gameObject.GetComponent<lnxAIlibrary>().lifeValue = other.gameObject.GetComponent<lnxAIlibrary>().lifeValue - attract;
            }
        }
    }
}

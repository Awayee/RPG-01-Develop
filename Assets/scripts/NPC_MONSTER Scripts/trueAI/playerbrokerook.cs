using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerbrokerook : MonoBehaviour {
    public string TargetTag;//攻击目标的tag，用于识别攻击对象
                            // Use this for initialization
    void Start () {
		
	}

    // Update is called once per frame
    public void OnTriggerEnter(Collider Target)
    {

        //Vector2 AttackDir = new Vector2(Ve.x, Ve.z);
        if (Target.CompareTag(TargetTag))
        {
           
            //目标减血
            if (TargetTag == "enemyroot")
            {
                Destroy(Target.gameObject);
            }

         
        }
    }
    }

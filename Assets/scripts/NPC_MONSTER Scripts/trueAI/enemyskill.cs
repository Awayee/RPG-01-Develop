using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyskill : MonoBehaviour {
    public float Damage;//技能伤害
    //private GameObject player;//玩家
    public string TargetTag;//攻击目标的tag，用于识别攻击对象
	// Use this for initialization
    
    public void OnTriggerEnter(Collider Target)
    {
        Vector3 AttackDir = Target.transform.position - transform.position; //攻击方向
        //Vector2 AttackDir = new Vector2(Ve.x, Ve.z);
        if (Target.gameObject.CompareTag(TargetTag))
        {
            if (Target.GetComponent<PlayCharacter>())
            Target.GetComponent<PlayCharacter>().GetHurt(Damage, AttackDir);
            //Destroy(gameObject);
        }
        
    }
     
    /*
    public void OnCollisionEnter(Collision collision)
    {
        Vector3 Ve = collision.transform.position - transform.position;
        Vector2 AttackDir = new Vector2(Ve.x, Ve.z);
        if (collision.gameObject.tag == TargetTag)
        {
            collision.gameObject.GetComponent<PlayCharacter>().GetHurt(selfAttack, AttackDir);
        }
    }
     */
     
     

}

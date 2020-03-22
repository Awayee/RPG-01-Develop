using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GiveDamage : MonoBehaviour {
    public bool flying;//是否为飞行物体，是则会在命中后消失
    //private Vector3 OriginPosition;//原始位置
    public float damage = 0;//伤害

    public string TargetTag;//攻击目标的tag，用于识别攻击对象

    [SerializeField] private GameObject popDamage;//用于显示伤害值
    // Use this for initialization
    public void OnTriggerEnter(Collider Target)
    {

        //Vector2 AttackDir = new Vector2(Ve.x, Ve.z);
        if (Target.CompareTag(TargetTag))
        {
            bool targetIsPlayer;//目标为玩家
            //计算攻击方向
            Vector3 AttackDir = Target.transform.position - transform.position; 
            //目标减血
            if (TargetTag == "Player")
            {
                targetIsPlayer = true;
                Target.GetComponent<PlayCharacter>().GetHurt(damage, AttackDir);
            }

            else
            {
                targetIsPlayer = false;
                if (TargetTag == "enemy")
                Target.GetComponent<EnemyMotion>().GetHurt(damage);
            }

            if(flying)Destroy(gameObject);//销毁飞行物
            //print("Target:" + Target.name);
            //print("TargetPosition:" + Target.transform.position);
            //显示伤害数值
            DisplayDamage _popDamage = Instantiate(popDamage).GetComponent<DisplayDamage>();//生成显示伤害值的物体
            _popDamage.ShowDamage(Target.gameObject, damage, targetIsPlayer);//设置伤害值
            
            
            //print("AttackDir---1:" + AttackDir);
            AttackDir.y = 0;
            //print("AttackDir---2:" + AttackDir);

        }

    }
}

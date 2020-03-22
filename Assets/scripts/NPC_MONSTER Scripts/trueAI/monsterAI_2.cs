using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class monsterAI_2 : MonoBehaviour {
    private lnxAIlibrary ailibrary;
    private int state;//重要参数，现在的执行状态
    public bool patrolmonster;//是否巡逻NPC（会不会绕地图走）真的话时绕地图走，假的话时某处闲逛
    public bool AttackNPCmonster;//是否攻击NPC型怪物，是的话优先攻击NPC，关于优先与否考虑更新算法
    public bool ismoreaiattack;//是否是高级战斗方式
    // Use this for initialization
    void Start () {
        //开关脚本
        //GetComponent<monsterAI_2>().enabled = false;
        ailibrary = gameObject.GetComponent<lnxAIlibrary>();
        state = 0;//默认状态是0，既行走状态
    }

    //核心的行为控制函数。行为逻辑图的装换部分重现
    private void actionmachine()
    {
        if (ailibrary.selfvalueisok() && !ailibrary.playerisinstartview() && !ailibrary.findstrongplayer())//如果视野内(初始视野)无玩家，并且自身血量OK,周围没有危险的话，就执行默认的行走行为
        {
            state = 0;
        }
        //state == 4暂时
        else if ((state == 4 || state == 0) && ailibrary.playerisinview() && ailibrary.findstrongplayer())//如果是默认状态，并且发现玩家，如果玩家比较强力的话(暂时添加了战斗逃跑)
        {
            state = 1;

        }
        else if (state == 1 && ailibrary.playerisinview() == false)//如果是逃跑中，并且玩家不再视野内的话（安全了）那就先原地盘旋（目前是直接停住）
        {
            state = 2;

        }
        else if (state == 2 && ailibrary.playerisinview() && ailibrary.findstrongplayer())//如果是原地暂停状态，然后又发现玩家的话，继续回到逃跑
        {
            state = 1;

        }
        else if (ailibrary.playerisinview() && !ailibrary.findstrongplayer()&& !ailibrary.playerisinAttackview())//这个是转到追击玩家状态，判断因素应该更智能的进行修改，目前仅进行是否攻击NPC的属性判断，和简单的范围判断，与自身判断
        {
            state = 3;


        }
        else if ((state == 3|| state == 0) &&ailibrary.playerisinAttackview()&& !ailibrary.havehinder())//转到攻击玩家
        {
            state = 4;
        }
        

    }


	// Update is called once per frame
	void Update () {
       
        actionmachine();//调用核心的逻辑重现函数

        //ai核心的switch，实际作出行动的执行行为
        switch (state)
        {
            case 0:                                             //默认行为，默认的巡逻方式

                if (patrolmonster) {ailibrary.monsterpatrol();}
                else{ailibrary.monsterrandomwalk();  } break;

            case 1:   //逃跑行为，从默认行为转换为逃跑行为
                ailibrary.monsterescape();
                break;
            case 2:   //停止行为 仅从逃跑行为转成暂时停止
                ailibrary.playerstopmove();
                break;
            case 3:   //追击玩家
                ailibrary.playerstopmove();
                ailibrary.playerstartmovetohero();
                ailibrary.MovetoplayerwithmoerAI();
               // Debug.Log("isinview = " + ailibrary.playerisinAttackview());
                //Debug.Log("havehender = " + !ailibrary.havehinder());
                break;
            case 4:   //攻击玩家，初始版
                if (ismoreaiattack)
                {
                    ailibrary.moreAIattack();
                }
                else {
                    ailibrary.AIAttack();
                }
                
                //ailibrary.playerstopmovetohero();
                //ailibrary.playerstopmove();
                //ailibrary.Attackpalyer();//怪物攻击玩家
                //ailibrary.lookatpalyer();//怪物实时朝向玩家
                break;

        }

        //一直持续的函数

        ailibrary.selfbeAttack();     //一直检测自己被袭击
        ailibrary.showselflife();     //一直显示自己的血条
                                      //ailibrary.Movetoplayer();
        if (transform.position.y<-100)
        {
            Destroy(gameObject);
        }
        if (ailibrary.lifeValue<=0)
        {
            Invoke("destroyself", 3f);
        }
        //Debug.Log("state = "+state);
    }
    private void destroyself()
    {
        Destroy(gameObject);
    }
}

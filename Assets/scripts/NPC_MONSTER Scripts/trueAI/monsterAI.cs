using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class monsterAI : MonoBehaviour {
    private lnxAIlibrary ailibrary;

    public float Attackdistance;//怪物攻击的距离
    //利用布尔值对整体结构做判断，来确定调用的行为接口，
    private bool nowmovetoplayer = true;//怪物是否移动向玩家、




	// Use this for initialization
	void Start () {
        ailibrary = gameObject.GetComponent<lnxAIlibrary>();
        Attackdistance = 8f;//设定怪物的攻击距离

    }
	

	// Update is called once per frame
	void Update () {






        if (ailibrary.thedistancetohero() <= Attackdistance && !ailibrary.havehinder())//判断自身的和玩家的状态，如果进入攻击范围，或者某个距离，那就不追了，执行相应函数（攻击函数）
        {
            nowmovetoplayer = false;
            ailibrary.playerstopmovetohero();
            ailibrary.playerstopmove();
            ailibrary.Attackpalyerwithmainskill();//怪物攻击玩家
            ailibrary.lookatpalyer();//怪物实时朝向玩家
        }
        else if (ailibrary.thedistancetohero() > Attackdistance)//如果它大于某个临界值的话，那就智能的去追击玩家
        {
            nowmovetoplayer = true;
            ailibrary.playerstartmovetohero();
            ailibrary.MovetoplayerwithmoerAI();

        }
        else
        {
            ailibrary.nouseskill1();//不满足攻击的条件，那就强制攻击制否
        }

        //怪物一直追赶玩家
        if (nowmovetoplayer)
        {
           // ailibrary.MovetoplayerwithmoerAI();
        }



        //一直持续的函数
   
        ailibrary.selfbeAttack();     //一直检测自己被袭击
        ailibrary.showselflife();     //一直显示自己的血条
        //ailibrary.Movetoplayer();



    }
   
}

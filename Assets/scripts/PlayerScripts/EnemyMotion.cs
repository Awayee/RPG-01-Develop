using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//敌人行为，血量、血条相关
public class EnemyMotion : MonoBehaviour {
    lnxAIlibrary thisLnx;
    private OnEnemyLife onEnmyLife;//玩家血条预制体
    private OnEnemyLife enemylife;//预制体
    private ValueBar lifeBar;//血条脚本
    public float lifebar_Height = 6;
    //[SerializeField]
    //private float distance;//敌人与玩家间的距离
    private bool entered = false;//判断是否已进入区域

	// Use this for initialization
	void Start () {
        thisLnx = GetComponent<lnxAIlibrary>();
        onEnmyLife = Resources.Load<OnEnemyLife>("Prefabs/UIElement-EnemyLife");
    }

    	// Update is called once per frame
	void Update () {
		if(!IsLive()) return;//如果已死，直接跳过
		//进入感应范围
        if (thisLnx.playerisinstartview())
        {
            if (entered) return;
            else
            {
				if(null == lifeBar)
                {
                    InstantiateLIfeBar();//如果变量为空，重新生成预制体
                    lifeBar.Display(1, true);
                } 
                else lifeBar.Display(1,false);				
                entered = true;
            }
            
        }
        else
        {
            if (!entered) return;
            else
            {
                lifeBar.Hide(1,false);	
                entered = false;
            }
        }
	}

    public void GetHurt(float Damage)//受伤
    {
        if(!IsLive()) return;
        thisLnx.lifeValue -= Damage;
        //如果有血条，则血条显示血量
        if(lifeBar!=null) lifeBar.SetValue(thisLnx.lifeValue / thisLnx.maxLife);
        if (IsLive())
        {
            return;
        }
        else Die();
    }
    public void Die()//死亡
    {
        thisLnx.lifeValue = 0;
        thisLnx.GetComponent<Animator>().Play("Death");
        //onEnmyLf.Hide_FadeOut();//血条逐渐消失
        //Destroy(onEnmyLf.gameObject, 1f);
        if(lifeBar!=null)lifeBar.DestroyBar(2);//销毁血条
    }
    public bool IsLive()//是否存活
    {
        if (thisLnx.lifeValue <= 0) return false;
        else return true;
    }
    #region 血条
    void InstantiateLIfeBar()//实例化血条
	{
		//生成预制体
		enemylife = Instantiate(onEnmyLife);

		//将该物体赋予目标脚本
		enemylife.SetTarget(this.transform);
        enemylife.h_offset = lifebar_Height;
        //得到血条
        lifeBar = enemylife.GetComponent<ValueBar>();
        lifeBar.SetValue(thisLnx.lifeValue / thisLnx.maxLife);

    }
    #endregion


}

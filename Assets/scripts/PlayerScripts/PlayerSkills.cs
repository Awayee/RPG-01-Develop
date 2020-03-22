using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerSkills : MonoBehaviour {
    PlayCharacter PlayerC;
    Animator animator;//动画组件



    public int skillIndex;//现在可以使用的技能符号 1表示没技能，1以后是技能的编号
    public GameObject Skill2Area;//技能二弹道
    public GameObject newskill2point;
    public float speedofskilll2 = 150;
    private bool skill2flag = true;//技能2的标志，用于管理cd用（单发强力的掌攻）长cd，强攻击，前摇长
    private bool skill3flag = true;//技能3的标志，用于cd管理，四发小导弹，短cd，一般伤害，无前摇


    public LayerMask monster;//npc的层级
    private float skill3distance = 13;//技能三的攻击距离
    public GameObject skilll3original;//技能三原始预制体
    public GameObject skilll3show;//技能三原始预制体
    private GameObject newskill3;//生成的信技能3
    private Vector3 scaleofskill3;//技能3的原始大小信息
    public SkillCd skill3cd;//技能3的冷却时间

    //public GameObject skillbutto2;//技能2的按钮极其所用函数
    public ButtonSkillCD bsk2; 
	// Use this for initialization
	void Start () {
        PlayerC = GetComponent<PlayCharacter>();
        animator = PlayerC.animator;
        scaleofskill3 = skilll3show.transform.localScale;

    }

    void Update()
    {/*
        if (skilll3show.transform.localScale.x > scaleofskill3.x)
        {
            skilll3show.transform.localScale = Vector3.Slerp(skilll3show.transform.localScale, scaleofskill3, 0.2f * Time.deltaTime);
        }
        */
    }

    #region 技能
    //********技能
    public void CancleSkill()//取消释放
    {
        animator.SetBool("Runtorollforward_0", false);//禁止向当前方向翻滚；
        animator.SetBool("standtorollforeard", false);//j禁止向当前方向翻滚；
    }
    public void ReleasSkill()//释放技能
    {
        SetCD();
        if (skillIndex == 1)
        {
            //Attack = 0;
            //bsk2.CdTime = 100;
            return;
        }
        if (skillIndex == 2)
        {
            //Attack = Attack * 10;
            Skill2();
        }
        if (skillIndex == 3)
        {
            //Attack = Attack * 0.5f;
            playeruseskill3();
        }

    }
    public void SetCD()//技能CD
    {
        if (skillIndex == 1)
        {

            //bsk2.CdTime = 100;
            return;
        }
        if (skillIndex == 2)
        {
            bsk2.CdTime = 5;

        }
        if (skillIndex == 3)
        {
            //pop.PopUpTip("技能3");
            skilll3show.SetActive(true);
            bsk2.CdTime = 0.5f;

        }
        if (skillIndex != 3)
        {
            skilll3show.SetActive(false);
        }
    }


    ///技能二的函数组*******************************************************************************************
    public void Skill2()
    {


        if (skill2flag)//检查是否可以释放技能2，flag用于cd检测用的
        {
            animator.Play("Skill2");
            // animator.SetTrigger("heroforwardsmash");//放火花；
            //animator.SetBool("heroforwardsmash_0", true);//放火花；walkbacktosmash
            //animator.SetBool("walkbacktosmash", true);
            //animator.Play("Roll");
            PlayerC.canMove = false;//放技能时人不能动
            //PlayerC.LookAtMonster();//开始朝向怪物
            //animator.SetBool("Walk Backward", false);
            skill2flag = false;//技能进入不可释放状态,防止连续快速按键，既进入cd；
            Invoke("instantiatskill2", 0.25f);//这里会置换成不朝向怪物
            Invoke("awakeskill2", 1f);
            Invoke("playernouseskill2", 1f);

        }
    }
    void instantiatskill2()
    {
        var skill2 = Instantiate(Skill2Area);
        skill2.GetComponent<GiveDamage>().flying = true;//技能为飞行物体
        skill2.GetComponent<GiveDamage>().damage = 2 * PlayerC.attackPower; //造成2倍攻击力数值的伤害
        skill2.transform.position = newskill2point.transform.position;
        Rigidbody skill2rigid = skill2.GetComponent<Rigidbody>();
        skill2rigid.velocity = newskill2point.transform.forward * speedofskilll2;
        //Lookatmonster = false;
    }

    void awakeskill2()
    {
        print("awakeskill2");
        skill2flag = true;//此技能冷却完毕，可以释放下一次
        PlayerC.canMove = true;
    }
    public void playernouseskill2()
    {
        animator.SetBool("heroforwardsmash_0", false);//禁止放技能2；
        animator.SetBool("walkbacktosmash", false);
        
    }


    //新技能  技能三 追踪导弹
    public void playeruseskill3()
    {
        if (skill3flag)//检查是否可以释放技能2，flag用于cd检测用的
        {
            skilll3show.SetActive(false);
            Invoke("awakeskill3show",skill3cd.cdTime);
            Collider[] cols = Physics.OverlapSphere(transform.position, 30, monster);//寻找半径范围内的所有碰撞体
            Debug.Log("cols.Length " + cols.Length);

            if (cols.Length > 0)
            {
                //GameObject toal = nearestmonster(cols).gameObject;

                for (int i = 0; i < 4; i++)
                {
                    Invoke("instantiatskill3", (i) * 0.1f);
                    //newskill3.GetComponent<skill3self>().goal = toal;
                }


            }
        }
        else {
            skilll3show.transform.localScale = skilll3show.transform.localScale * 2;
        }

    }


    //实例化技能三
    void instantiatskill3()
    {
        newskill3 = Instantiate(skilll3original);
        newskill3.GetComponent<GiveDamage>().damage = PlayerC.attackPower * 0.3f;
        newskill3.transform.position = skilll3show.transform.position;
        //Rigidbody skill3rigid = newskill3.GetComponent<Rigidbody>();
        //skill3rigid.velocity = transform.forward * 5f;


    }
    //唤醒技能3
    void awakeskill3()
    {
        skill3flag = true;//此技能冷却完毕，可以释放下一次
    }
    private void awakeskill3show()//唤醒技能3显示
    {
        skilll3show.SetActive(true);
    }

    //技能显示判断
    private void showskill()
    {
        if (skillIndex == 3)
        {

        }
        else
        {
        }
    }

    #endregion
    public void GetEnergyAndLife()//恢复生命值
    {
        PlayerC.ChangeEnergyValue(30);
        PlayerC.ChangeLifeValue(30);
        GameManager.Instance.Tip("回春");
    }

    public UnityEvent getCharacterSkill(int id)//获取角色的技能
    {
        UnityEvent skillEvent = new UnityEvent();
        switch (id)
        {
            case 1:
                skillEvent.AddListener(PlayerC.ChangeMoveMode);
                break;
            case 2:
                skillEvent.AddListener(PlayerC.Rush);
                break;
            case 3:
                skillEvent.AddListener(PlayerC.Jump);
                break;
            case 4:
                skillEvent.AddListener(this.Skill2);
                break;
            case 5:
                skillEvent.AddListener(this.playeruseskill3);
                break;
            default:
                break;
        }
        return skillEvent;
    }
}

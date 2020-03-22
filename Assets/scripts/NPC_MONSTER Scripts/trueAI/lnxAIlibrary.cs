using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class lnxAIlibrary : MonoBehaviour {

    //自身重要属性；
    public float Attack;//攻击力
    public float lifeValue = 100;//血量
    public float maxLife = 100;//最大血量
    public bool isusuallyview;//是否是平常视野（不显示视野范围的那种）如果不是的话，下边的特殊视野属性判断才有用
    public bool iscircularview;//自身初始的特殊视野类型，真的话，是圆形视野
    public float slowspeed=4;//缓慢行走时的速度大小定义






    private GameObject self;
    private bool runflagtohero = true;
    CharacterController cc;//自身简易人物移动组件
    private GameObject maincamera;//场景的主相机
    public GameObject visionrange;//自身的视野范围
    private Collider m_collider;//自身的碰撞体组件



    public LayerMask NPC;//npc的层级
    public Animator animator;//获取自身的动画机
    public float turnspeed = 8f;//自身的转向速度
    public float distancetoplayer = 64f;//自身距离玩家的距离
    private bool isDilemma=true;//怪物自动追踪玩家防止卡死用的
    private GameObject barrier01;//怪物自动追踪玩家防止卡死用的
    private GameObject barrier02;//怪物自动追踪玩家防止卡死用的
    private GameObject barrier03;//怪物自动追踪玩家防止卡死用的
    private Transform barrier04;//怪物自动追踪玩家防止卡死用的, 
    public GameObject origenalbarrier;//防止报错用的空物体初值
    private float raydis=5f;//射线的长短
    private float lookReTime;//判断的标志
    private float margin=2f;//用于检测自己是否自己在地面上的标志
    public GameObject newskill1fire;//怪物放的技能1的原型
    public GameObject newskill1point;//怪物放的技能一的位置
    public float speedofskilll1 = 80;//发射技能1的速度
    private bool skill1flag = true;//判断技能一是否可以用的标志
    private bool beAttack = false;//自身被袭击的判断
    public float Attackdistance;//怪物攻击的距离
    private float saqAttackdistance;//怪物攻击的距离的平方
    public float viewdistance;//怪物视野的距离
    private float saqviewdistance;//怪物视野的距离的平方

    //自身血条显示用到的变量*********************************
    public GameObject lifeshow;//血条本条
    public GameObject Point;//血条原点
    //public float MaxLife = 100;
    private Vector3 scale;//大小
   //自身血条显示用到的变量******************************



    //随机行走使用到的变量************************************
    private int currentState;
    public float[] actionWeight = { 3000, 3000, 4000 };         //设置待机时各种动作的权重，顺序依次为呼吸、观察、移动
    public float actRestTme;            //更换待机指令的间隔时间
    private float lastActTime;          //最近一次指令时间
    private Quaternion targetRotation;        //怪物的目标朝向
    public float workrange;                //随机乱走的半径
    private Vector3 burnposition;//怪物出生的点位
    //随机行走使用到的变量************************************

    //巡逻行走使用到的变量************************************
    public GameObject nextGuidepost;
    public Transform nexttransform;
    //巡逻行走使用到的变量************************************


    //寻找锥形范围内敌人用到的变量（怪物激活）************************************
    private float SkillDistance = 5;//扇形距离
    private float SkillJiaodu = 60;//扇形的角度
    public GameObject pointlight;//圆形的视野光
    public GameObject spotlight;//锥形的视野光

    //寻找锥形范围内敌人用到的变量（怪物激活）************************************


    //怪物智能战斗用到的变量（怪物战斗）************************************
    private int state;//状态机战斗状态
    public GameObject AttackshortArea;//自身近战普攻1检测用的子物体
    public GameObject AttacklongArea;//自身近战普攻2检测用的子物体
    private bool normalattackshortflag = true;//短距离普攻的冷却标志
    public int shortattackpower = 5;//短距离普攻的攻击力
    private bool normalattacklongflag = true;//长距离普攻的冷却标志
    public int longattackpower = 4;//长距离普攻的攻击力
    public int shortattackdistance = 3;//短距离普攻的攻击距离
    private int saqshortattackdistance;//短距离普攻的距离平方（方便计算用）
    public int longattackdistance = 5;//长距离普攻的攻击距离
    private int saqlongattackdistance;//长距离普攻的攻击距离平方（方便计算用）
    private int attackstate = 0;//智能战斗状态机用初始状态
    public float mainskillcd = 3;//主技能的存档
    //怪物智能战斗用到的变量（怪物战斗）************************************


    GameObject player;//怪物用到的玩家对象
    // Use this for initialization
    void Start () {
        player = GameObject.FindGameObjectWithTag("Player");

        self = gameObject;
        cc = gameObject.GetComponent<CharacterController>();
        selfnoaction();//动画机布尔值回归
        maincamera = GameObject.FindGameObjectWithTag("MainCamera");//获取主相机
        saqAttackdistance = Attackdistance * Attackdistance;//为了减少计算量，在一开始计算一下攻击距离的平方
        saqviewdistance = viewdistance * viewdistance;//为了减少计算量，在一开始计算一下视野距离的平方
        m_collider = GetComponent<Collider>();//获取自身的碰撞体组件，这将用于周围物体的搜索
        //防止报错用；
        barrier02 = origenalbarrier;
        barrier03 = origenalbarrier;
        barrier04 = origenalbarrier.transform;

        //血条的显示初值用
        scale = Point.transform.localScale;
        
        //原地徘徊用的初值
        currentState = 1;
        burnposition = new Vector3(transform.position.x,0,transform.position.z);

        //场景巡逻赋初值
        if (nextGuidepost != null)
        {
            nexttransform = nextGuidepost.transform;
        }

        //战斗用数据赋初值
        saqshortattackdistance = shortattackdistance * shortattackdistance;
        saqlongattackdistance = longattackdistance * longattackdistance;
    }




    // Update is called once per frame
    //怪物ai接口

    //怪物朝着玩家移动（走到附近）
    public void Movetoplayer()//这个函数让怪物一直朝向玩家方向移动 此处处理人物转向，让怪物一直朝着玩家方向，然后播放动画，此接口包含防止贴合玩家功能
    {

        if (runflagtohero == false) return;//其它技能限制行走；


        Vector3 goalmovedir = player.transform.position-gameObject.transform.position;
        distancetoplayer = Vector3.Distance(transform.position,player.transform.position);//实时计算相距玩家的距离
        goalmovedir = new Vector3(goalmovedir.x,0,goalmovedir.z);
        //行走时判断，如果到达玩家附近了，停止移动，只是实时朝向玩家
        if (distancetoplayer < Attackdistance)//怪物或者生物离玩家的最短距离，防止贴合模型
        {
            RaycastHit hit;
            //如果和玩家中间有障碍的话，那不停，绕过继续自行追踪。
            if (Physics.Raycast(transform.position + Vector3.up * 1.5f, (player.transform.position + Vector3.up * 1.5f) - transform.position, out hit, Vector3.Distance(player.transform.position, transform.position)))
            {
                return;
            }
                playerstopmove();
            var stopturnto = Quaternion.LookRotation(goalmovedir);//转换成四元数
            Quaternion stopslerp = Quaternion.Slerp(transform.rotation, stopturnto, turnspeed * Time.deltaTime);
            transform.rotation = stopslerp;
            return;
        }
        //Vector3 newVec = Quaternion.AngleAxis(cameratarget.transform.eulerAngles.y, transform.up) * oldmovedir;
        //movedir = new Vector3();
        var turnto = Quaternion.LookRotation(goalmovedir);//转换成四元数
        Quaternion slerp = Quaternion.Slerp(transform.rotation, turnto, turnspeed * Time.deltaTime);
        transform.rotation = slerp;
        animator.SetBool("Run", true);
        cc.SimpleMove(transform.forward);
    }

    //怪物贴玩家脸
    public void Movetoplayernearly()//这个函数让怪物一直朝向玩家方向移动 此处处理人物转向，让怪物一直朝着玩家方向，然后播放动画，此接口包含防止贴合玩家功能
    {

        if (runflagtohero == false) return;//其它技能限制行走；


        Vector3 goalmovedir = player.transform.position - gameObject.transform.position;
        distancetoplayer = Vector3.Distance(transform.position, player.transform.position);//实时计算相距玩家的距离
        goalmovedir = new Vector3(goalmovedir.x, 0, goalmovedir.z);
        //行走时判断，如果到达玩家附近了，停止移动，只是实时朝向玩家
        if (distancetoplayer < longattackpower)//怪物或者生物离玩家的最短距离，防止贴合模型
        {
            RaycastHit hit;
            //如果和玩家中间有障碍的话，那不停，绕过继续自行追踪。
            if (Physics.Raycast(transform.position + Vector3.up * 1.5f, (player.transform.position + Vector3.up * 1.5f) - transform.position, out hit, Vector3.Distance(player.transform.position, transform.position)))
            {
                return;
            }
            playerstopmove();
            var stopturnto = Quaternion.LookRotation(goalmovedir);//转换成四元数
            Quaternion stopslerp = Quaternion.Slerp(transform.rotation, stopturnto, turnspeed * Time.deltaTime);
            transform.rotation = stopslerp;
            return;
        }
        //Vector3 newVec = Quaternion.AngleAxis(cameratarget.transform.eulerAngles.y, transform.up) * oldmovedir;
        //movedir = new Vector3();
        var turnto = Quaternion.LookRotation(goalmovedir);//转换成四元数
        Quaternion slerp = Quaternion.Slerp(transform.rotation, turnto, turnspeed * Time.deltaTime);
        transform.rotation = slerp;
        animator.SetBool("Run", true);
        cc.SimpleMove(transform.forward);
    }
    //怪物朝着指定方向移动
    public void Movetogoaldirection(Vector3 goaldirectiom)//这个函数让怪物朝着指定的方向移动并转向，里边包含静止时朝向目标位置的功能
    {

        if (runflagtohero == false) return;//其它技能限制行走；

        goaldirectiom = new Vector3(goaldirectiom.x,0, goaldirectiom.z);//去除y干扰
        Vector3 goalmovedir = player.transform.position - gameObject.transform.position;//指向玩家的向量
        distancetoplayer = goalmovedir.sqrMagnitude;//实时计算相距玩家的距离
        //行走时判断，如果到达玩家附近了，停止移动，只是实时朝向玩家
        if (distancetoplayer < saqAttackdistance)
        {
            RaycastHit hit;
            //如果和玩家中间有障碍的话，那不停，绕过继续自行追踪。
            if (!Physics.Raycast(transform.position + Vector3.up * 0.5f, player.transform.position - transform.position, out hit, Vector3.Distance(player.transform.position, transform.position)))
            {
                return;
            }
            playerstopmove();
            var stopturnto = Quaternion.LookRotation(goalmovedir);//转换成四元数
            Quaternion stopslerp = Quaternion.Slerp(transform.rotation, stopturnto, turnspeed * Time.deltaTime);
            transform.rotation = stopslerp;
            return;
        }
        //Vector3 newVec = Quaternion.AngleAxis(cameratarget.transform.eulerAngles.y, transform.up) * oldmovedir;
        //movedir = new Vector3();
        var turnto = Quaternion.LookRotation(goaldirectiom);//转换成四元数
        Quaternion slerp = Quaternion.Slerp(transform.rotation, turnto, turnspeed * Time.deltaTime);
        transform.rotation = slerp;
        animator.SetBool("Run", true);
        cc.SimpleMove(transform.forward);
    }
    //单纯的怪物向某个方向跑，不包含任何其它内容
    public void monstermove(Vector3 movedir)
    {
        movedir = new Vector3(movedir.x, 0, movedir.z);//去除y干扰
        var turnto = Quaternion.LookRotation(movedir);//转换成四元数
        Quaternion slerp = Quaternion.Slerp(transform.rotation, turnto, turnspeed * Time.deltaTime);
        transform.rotation = slerp;
        animator.SetBool("Run", true);
        cc.SimpleMove(transform.forward);
    }
    public void monstermove(Quaternion movedir)
    {
       // movedir = new Vector3(movedir.x, 0, movedir.z);//去除y干扰
      //  var turnto = Quaternion.LookRotation(movedir);//转换成四元数
        Quaternion slerp = Quaternion.Slerp(transform.rotation, movedir, turnspeed * Time.deltaTime);
        transform.rotation = slerp;
        animator.SetBool("Run", true);
        cc.SimpleMove(transform.forward);
    }
    //单纯的怪物向某个方向走，不包含任何其它内容
    public void monstermoveslow(Vector3 movedir)
    {
        movedir = new Vector3(movedir.x, 0, movedir.z);//去除y干扰
        var turnto = Quaternion.LookRotation(movedir);//转换成四元数
        Quaternion slerp = Quaternion.Slerp(transform.rotation, turnto, turnspeed * Time.deltaTime);
        transform.rotation = slerp;
        animator.SetBool("Run", false);
        animator.SetBool("Walk Forward", true);
        cc.SimpleMove(transform.forward*slowspeed);
    }
    public void monstermoveslow(Quaternion movedir)
    {
        //movedir = new Vector3(movedir.x, 0, movedir.z);//去除y干扰
       // var turnto = Quaternion.LookRotation(movedir);//转换成四元数
        Quaternion slerp = Quaternion.Slerp(transform.rotation, movedir, turnspeed * Time.deltaTime);
        transform.rotation = slerp;
        animator.SetBool("Run", false);
        animator.SetBool("Walk Forward", true);
        cc.SimpleMove(transform.forward* slowspeed);
    }
    //怪物停止移动行为
    public void playerstopmove()//停止人物行走
    {
        animator.SetBool("Walk Forward", false);
        animator.SetBool("WalkSlow", false);
        animator.SetBool("Run", false);
        animator.SetBool("Walk Backward", false);
    }
    //怪物停止追击玩家
    public void playerstopmovetohero()//停止人物行走向玩家
    {
        runflagtohero = false;
    }
    //怪物开始追击玩家
    public void playerstartmovetohero()//停止人物行走向玩家
    {
        runflagtohero = true;
    }
    //怪物带规避的实时追击玩家方法基础版(会被锁住的版本，行动力较低)
    public void MovetoplayerwithlittleAI()
    {
        RaycastHit hit;
        Vector3 goalmovedir = player.transform.position - gameObject.transform.position;//指向玩家的向量
        if (Physics.Raycast(transform.position + Vector3.up * 1f, transform.forward, out hit, 15f))//如果有障碍物，那么，就执行绕开障碍物
        { //往该角色提高半米的位置的前方发射一条20米的射线,如果有射中障碍物层级的物体
            
            float rotateDir = Vector3.Dot(goalmovedir, hit.normal); //获取角色右方向与击中位置的法线的点乘结果,主要用于判断障碍物位置,是在角色左边还是右边
            Debug.DrawRay(transform.position + Vector3.up * 1f, transform.forward * 7, Color.red);
            Debug.DrawRay(hit.point, hit.normal, Color.blue);
            if (rotateDir >= 0)
            {  //如果大于等于0
                transform.Rotate(transform.up * 90 * Time.deltaTime);  //则往顺时针方向以90度每秒的方向转动
                                                                       // lookReTime = 0; //避免在躲避障碍物的时候,还会朝向玩家,易发生相反转向
              //  animator.SetBool("Run", true);
               // cc.SimpleMove(transform.forward);
            }
            else
            {
                transform.Rotate(transform.up * -90 * Time.deltaTime);  //则往逆时针方向以90度每秒的方向转动
                                                                        //lookReTime = 0;
              //  animator.SetBool("Run", true);
              //  cc.SimpleMove(transform.forward);
            }
        }
        else
        {
            Movetoplayer();
        }
       
    }
    //智能移动接口；怪物带规避的实时追击玩家方法升级(防止被锁住),移动过程中实时判断是否需要跳跃，并进行实时追踪。自动获取玩家位置，
    public void MovetoplayerwithmoerAIway()
    {
        
        float rotateDir;//射线与法线点乘的结果，用来判断怪物在障碍前的转向问题
        RaycastHit hit;  //用来检测前方是否有障碍物

        if (!Physics.Raycast(transform.position + Vector3.up * 0.5f, (player.transform.position + Vector3.up * 0.5f) - transform.position, out hit, Vector3.Distance(player.transform.position, transform.position)))
        {//如果与目标之间发出的射线没有碰到障碍物的话
            //transform.LookAt(player.transform);
            isDilemma = false;  //死胡同模式关闭
            animator.SetBool("herorunjump_0", false);//跑跳禁止
            //Vector3 goalmovedir = player.transform.position - gameObject.transform.position;//指向玩家的向量        
            
            //lookReTime = 0;
            //Debug.Log("haha,看你往哪跑");
            
        }
        else
        {
            if (thistimeineedjump())//跳跃判断 如果这次的障碍物是可以跳跃通过的话，那就不执行下边的绕过了
            {
                if (icanjumpout())
                {
                    Debug.Log(gameObject.name + "jump!");
                    selfjump();
                    return;
                }
                else
                {
                    Movetoplayer();//这里可能需要再判断，是朝向玩家走还是朝向目标地点
                    animator.SetBool("herorunjump_0", false);//跑跳；
                    return;
                }
            }
            else
            {
            if (!isDilemma)  //如果不处于死胡同模式,则进入
            {
                if (Physics.Raycast(transform.position + Vector3.up * 1f, transform.forward, out hit, raydis))
                {

                
                  
                        animator.SetBool("herorunjump_0", false);//跑跳；

                        //往该角色提高半米的位置的前方发射一条10米的射线,如果有射中障碍物层级的物体
                        barrier03 = barrier02;  //barrier01-03这几个都是声明的GameObject类型
                    barrier02 = barrier01;
                    barrier01 = hit.transform.gameObject;

                    if (barrier01 == barrier02)
                    { //如果接着碰上的是同一个
                        Vector3 goalmovedir = player.transform.position - gameObject.transform.position;//指向玩家的向量
                        rotateDir = Vector3.Dot(goalmovedir, hit.normal);

                        if (rotateDir >= 0)
                        {
                            transform.Rotate(transform.up * 180 * Time.deltaTime);

                            lookReTime = 0; //避免在躲避障碍物的时候,还会朝向玩家,易发生相反转向
                        }
                        else
                        {
                            transform.Rotate(transform.up * -180 * Time.deltaTime);

                            lookReTime = 0; //避免在躲避障碍物的时候,还会朝向玩家,易发生相反转向
                        }
                        //Debug.Log("马上就会找到你的");
                    }
                    else if (barrier03 == barrier01)  //如果刚碰上的和之前碰上的是同一个,则触发死胡同模式
                    { //进入死胡同模式
                        barrier04 = barrier01.transform;
                        rotateDir = Vector3.Dot(transform.right, barrier04.position - transform.position);
                        if (rotateDir >= 0)
                        {  //如果障碍物在前进方向右侧
                            for (int i = 0; i < 72; i++) //快速找出障碍物的边缘,往其边缘行走
                            {
                                transform.Rotate(transform.up * 5);
                                if (!Physics.Raycast(transform.position + Vector3.up * 0.5f, transform.forward, out hit, raydis))
                                {
                                    //Debug.Log("4");
                                    lookReTime = 0;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            for (int i = 0; i < 72; i++)
                            {
                                transform.Rotate(transform.up * -5);
                                if (!Physics.Raycast(transform.position + Vector3.up * 0.5f, transform.forward, out hit, raydis))
                                {
                                    //Debug.Log("3");
                                    lookReTime = 0;
                                    break;
                                }
                            }
                        }
                        isDilemma = true;  //开启死胡同模式
                                           // Debug.Log("进入死胡同模式啦");
                    }
              
                }
            }
            else
            {
                rotateDir = Vector3.Dot(transform.forward, player.transform.position - transform.position);
                if (Physics.Raycast(transform.position + Vector3.up * 0.5f, player.transform.position - transform.position, out hit, raydis))
                {
                    lookReTime = 0;
                    Debug.DrawRay(transform.position + Vector3.up * 0.5f, player.transform.position - transform.position, Color.red, raydis);
                    if (hit.transform.gameObject != barrier04.transform.gameObject && rotateDir > 0)
                    {
                        isDilemma = false;
                        // Debug.Log("死胡同模式解除啦啦");
                        transform.LookAt(new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z));
                        
                        lookReTime = 0;
                    }
                    if (Physics.Raycast(transform.position + Vector3.up * 0.5f, transform.forward, out hit, raydis))
                    {
                        if (hit.transform.gameObject != barrier01 && hit.transform.gameObject != barrier02)
                        {
                            isDilemma = false;
                           // Debug.Log("临时解除死胡同模式");
                            lookReTime = 0;
                        }
                    }

                }
                else if (rotateDir > 0)
                {
                    isDilemma = false;
                    //Debug.Log("死胡同模式解除啦啦");
                    transform.LookAt(new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z));
                    
                    lookReTime = 0;
                }
                else
                {
                    rotateDir = Vector3.Dot(transform.right, barrier04.position - transform.position);
                    if (rotateDir >= 0)
                    {
                        for (int i = 0; i < 72; i++)
                        {
                            transform.Rotate(transform.up * 5f);
                            if (Physics.Raycast(transform.position + Vector3.up * 0.5f, transform.forward, out hit, raydis))
                            {
                                transform.Rotate(transform.up * -5f);
                               // Debug.Log("2");
                                lookReTime = 0;
                                break;
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < 72; i++)
                        {
                            transform.Rotate(transform.up * -5f);
                            if (Physics.Raycast(transform.position + Vector3.up * 0.5f, transform.forward, out hit, raydis))
                            {
                                transform.Rotate(transform.up * 5f);
                               // Debug.Log("1");
                                lookReTime = 0;
                                break;
                            }
                        }
                    }
                   // Debug.Log("什么时候能出死胡同啊");
                }


            }
        }
       }

    }
    public void MovetoplayerwithmoerAI()
    {
        RaycastHit hit;  //用来检测前方是否有障碍物
        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, (player.transform.position + Vector3.up * 0.5f) - transform.position, out hit, Vector3.Distance(player.transform.position, transform.position)))
        {
            lookReTime = 1;
            MovetoplayerwithmoerAIway();
            if (lookReTime != 0)
            {
                Movetoplayer();
                // Debug.Log("5");
            }
        }
        else
        {
            Movetoplayer();
        }
        
    }//这个函数才是真正的接口
    //怪物跑向指定方位，（带有复杂ai与跳跃判断）常用于有确定目标的位移

    //怪物跑向指定方位，（带有简单ai与跳跃判断）常用于没有确定目标的位移
    public void MovetogoalwithlittleAI(Vector3 goal)
    {
        RaycastHit hit;
        //Vector3 goalmovedir = player.transform.position - gameObject.transform.position;//指向玩家的向量
        if (Physics.Raycast(transform.position + Vector3.up * 1f, transform.forward, out hit, 15f))//如果有障碍物，那么，就执行绕开障碍物
        { //往该角色提高半米的位置的前方发射一条20米的射线,如果有射中障碍物层级的物体
            if (thistimeineedjump())//跳跃判断 如果这次的障碍物是可以跳跃通过的话，那就不执行下边的绕过了
            {
                if (icanjumpout())
                {
                    selfjump();
                    return;
                }
                else
                {
                    if (Physics.Raycast(transform.position + Vector3.up * 1f, transform.forward, out hit, 15f))//如果有障碍物，那么，就执行绕开障碍物
                    { //往该角色提高半米的位置的前方发射一条20米的射线,如果有射中障碍物层级的物体

                        float rotateDir = Vector3.Dot(transform.forward, hit.normal); //获取角色右方向与击中位置的法线的点乘结果,主要用于判断障碍物位置,是在角色左边还是右边
                        Debug.DrawRay(transform.position + Vector3.up * 1f, transform.forward * 7, Color.red);
                        Debug.DrawRay(hit.point, hit.normal, Color.blue);
                        if (rotateDir >= 0)
                        {  //如果大于等于0
                            transform.Rotate(transform.up * 90 * Time.deltaTime);  //则往顺时针方向以90度每秒的方向转动
                                                                                   // lookReTime = 0; //避免在躲避障碍物的时候,还会朝向玩家,易发生相反转向
                                                                                   //  animator.SetBool("Run", true);
                                                                                   // cc.SimpleMove(transform.forward);
                        }
                        else
                        {
                            transform.Rotate(transform.up * -90 * Time.deltaTime);  //则往逆时针方向以90度每秒的方向转动
                                                                                    //lookReTime = 0;
                                                                                    //  animator.SetBool("Run", true);
                                                                                    //  cc.SimpleMove(transform.forward);
                        }
                    }
                    else
                    {
                        monstermove(goal);
                    }

                }
            }

        }
        else
        {
            monstermove(goal);
        }
    }
    public void MovetogoalwithlittleAI(Quaternion goal)
    {
        RaycastHit hit;
        //Vector3 goalmovedir = player.transform.position - gameObject.transform.position;//指向玩家的向量
        if (Physics.Raycast(transform.position + Vector3.up * 1f, transform.forward, out hit, 15f))//如果有障碍物，那么，就执行绕开障碍物
        { //往该角色提高半米的位置的前方发射一条20米的射线,如果有射中障碍物层级的物体
            if (thistimeineedjump())//跳跃判断 如果这次的障碍物是可以跳跃通过的话，那就不执行下边的绕过了
            {
                if (icanjumpout())
                {
                    selfjump();
                    return;
                }
                else
                {
                    if (Physics.Raycast(transform.position + Vector3.up * 1f, transform.forward, out hit, 15f))//如果有障碍物，那么，就执行绕开障碍物
                    { //往该角色提高半米的位置的前方发射一条20米的射线,如果有射中障碍物层级的物体

                        float rotateDir = Vector3.Dot(transform.forward, hit.normal); //获取角色右方向与击中位置的法线的点乘结果,主要用于判断障碍物位置,是在角色左边还是右边
                        Debug.DrawRay(transform.position + Vector3.up * 1f, transform.forward * 7, Color.red);
                        Debug.DrawRay(hit.point, hit.normal, Color.blue);
                        if (rotateDir >= 0)
                        {  //如果大于等于0
                            transform.Rotate(transform.up * 90 * Time.deltaTime);  //则往顺时针方向以90度每秒的方向转动
                                                                                   // lookReTime = 0; //避免在躲避障碍物的时候,还会朝向玩家,易发生相反转向
                                                                                   //  animator.SetBool("Run", true);
                                                                                   // cc.SimpleMove(transform.forward);
                        }
                        else
                        {
                            transform.Rotate(transform.up * -90 * Time.deltaTime);  //则往逆时针方向以90度每秒的方向转动
                                                                                    //lookReTime = 0;
                                                                                    //  animator.SetBool("Run", true);
                                                                                    //  cc.SimpleMove(transform.forward);
                        }
                    }
                    else
                    {
                        monstermove(goal);
                    }

                }
            }

        }
        else
        {
            monstermove(goal);
        }
    }
    //怪物走向指定方位，（带有简单ai与跳跃判断）常用于没有确定目标的位移（四元数参数版）(因为走得比较慢，这个最好专门用于漫无目的的巡逻，避免停住)(障碍检测很短)
    public void slowMovetogoalwithlittleAI(Quaternion goal)
    {
        RaycastHit hit;
        //Vector3 goalmovedir = player.transform.position - gameObject.transform.position;//指向玩家的向量
        if (Physics.Raycast(transform.position + Vector3.up * 1f, transform.forward, out hit, 3f))//如果有障碍物，那么，就执行绕开障碍物
        { //往该角色提高半米的位置的前方发射一条20米的射线,如果有射中障碍物层级的物体
            if (thistimeineedjump())//跳跃判断 如果这次的障碍物是可以跳跃通过的话，那就不执行下边的绕过了
            {
                if (icanjumpout())
                {
                    selfjump();
                    return;
                }
                else
                {
                    if (Physics.Raycast(transform.position + Vector3.up * 1f, transform.forward, out hit, 3f))//如果有障碍物，那么，就执行绕开障碍物
                    { //往该角色提高半米的位置的前方发射一条20米的射线,如果有射中障碍物层级的物体

                        float rotateDir = Vector3.Dot(transform.forward, hit.normal); //获取角色右方向与击中位置的法线的点乘结果,主要用于判断障碍物位置,是在角色左边还是右边
                        Debug.DrawRay(transform.position + Vector3.up * 1f, transform.forward * 7, Color.red);
                        Debug.DrawRay(hit.point, hit.normal, Color.blue);
                        if (rotateDir >= 0)
                        {  //如果大于等于0
                            transform.Rotate(transform.up * 90 * Time.deltaTime);  //则往顺时针方向以90度每秒的方向转动
                                                                                   // lookReTime = 0; //避免在躲避障碍物的时候,还会朝向玩家,易发生相反转向
                                                                                   //  animator.SetBool("Run", true);
                                                                                   // cc.SimpleMove(transform.forward);
                        }
                        else
                        {
                            transform.Rotate(transform.up * -90 * Time.deltaTime);  //则往逆时针方向以90度每秒的方向转动
                                                                                    //lookReTime = 0;
                                                                                    //  animator.SetBool("Run", true);
                                                                                    //  cc.SimpleMove(transform.forward);
                        }
                    }
                    else
                    {
                        monstermoveslow(goal);
                    }

                }
            }

        }
        else
        {
            monstermoveslow(goal);
        }
    }
    //怪物走向指定方位，（带有简单ai与跳跃判断）常用于没有确定目标的位移(因为走得比较慢，这个最好专门用于漫无目的的巡逻，避免停住)(障碍检测很短)
    public void slowMovetogoalwithlittleAI(Vector3 goal)
    {
        RaycastHit hit;
        //Vector3 goalmovedir = player.transform.position - gameObject.transform.position;//指向玩家的向量
        if (Physics.Raycast(transform.position + Vector3.up * 1f, transform.forward, out hit, 3f))//如果有障碍物，那么，就执行绕开障碍物
        { //往该角色提高半米的位置的前方发射一条20米的射线,如果有射中障碍物层级的物体
            if (thistimeineedjump())//跳跃判断 如果这次的障碍物是可以跳跃通过的话，那就不执行下边的绕过了
            {
                //playerstopmove(); //防止无法转到jump，执行一次动画机回归
                if (icanjumpout())
                {
                   
                    selfjump();
                    return;
                }
                else
                {
                    if (Physics.Raycast(transform.position + Vector3.up * 1f, transform.forward, out hit, 3f))//如果有障碍物，那么，就执行绕开障碍物
                    { //往该角色提高半米的位置的前方发射一条2米的射线,如果有射中障碍物层级的物体

                        float rotateDir = Vector3.Dot(transform.forward, hit.normal); //获取角色右方向与击中位置的法线的点乘结果,主要用于判断障碍物位置,是在角色左边还是右边
                        Debug.DrawRay(transform.position + Vector3.up * 1f, transform.forward * 7, Color.red);
                        Debug.DrawRay(hit.point, hit.normal, Color.blue);
                        if (rotateDir >= 0)
                        {  //如果大于等于0
                            transform.Rotate(transform.up * 90 * Time.deltaTime);  //则往顺时针方向以90度每秒的方向转动
                                                                                   // lookReTime = 0; //避免在躲避障碍物的时候,还会朝向玩家,易发生相反转向
                                                                                   //  animator.SetBool("Run", true);
                                                                                   // cc.SimpleMove(transform.forward);
                        }
                        else
                        {
                            transform.Rotate(transform.up * -90 * Time.deltaTime);  //则往逆时针方向以90度每秒的方向转动
                                                                                    //lookReTime = 0;
                                                                                    //  animator.SetBool("Run", true);
                                                                                    //  cc.SimpleMove(transform.forward);
                        }
                    }
                    else
                    {
                        monstermoveslow(goal);
                    }

                }
            }

        }
        else
        {
            monstermoveslow(goal);
        }
    }
    //检测距离玩家的距离函数  返回一个浮点数值
    public float thedistancetohero()
    {
        Vector3 goalmovedir = player.transform.position - gameObject.transform.position;
        float thedistancetoplayer;
        thedistancetoplayer = goalmovedir.magnitude;//实时计算相距玩家的距离
        return thedistancetoplayer;
    }
    //检测玩家是否进入视野
    public bool playerisinview()
    {
        Vector3 selftoplayer = player.transform.position - gameObject.transform.position;
        distancetoplayer = selftoplayer.sqrMagnitude;//实时计算相距玩家的距离
        if (distancetoplayer <= saqviewdistance)//如果玩家距离怪物的距离在视野范围内
        {
           


            return true;
           

        }
        else {
          //  Debug.Log("istancetoplayer;" + distancetoplayer);
          //  Debug.Log("saqviewdistance;" + saqviewdistance);
            return false;
        }

    }
    //检测玩家是否进入初始视野
    public bool playerisinstartview()
    {if (isusuallyview)//如果是普通的视野的话，就直接判断就好了，不执行特殊行为，并且关闭两个光线显示
        {
            pointlight.SetActive(false);
            spotlight.SetActive(false);
            return playerisinview();
        }

    else {
            if (iscircularview)
            {
                pointlight.SetActive(true);
                spotlight.SetActive(false);

                Vector3 selftoplayer = player.transform.position - gameObject.transform.position;
                distancetoplayer = selftoplayer.sqrMagnitude;//实时计算相距玩家的距离
                if (distancetoplayer <= saqviewdistance)//如果玩家距离怪物的距离在视野范围内
                {
                    return true;
                }
                else
                {
                    //  Debug.Log("istancetoplayer;" + distancetoplayer);
                    //  Debug.Log("saqviewdistance;" + saqviewdistance);
                    return false;
                }

            }
            else
            {
                pointlight.SetActive(false);
                spotlight.SetActive(true);
                float distance = Vector3.Distance(transform.position, player.transform.position);//距离
                //Vector3 norVec = transform.rotation * Vector3.forward * 5;//此处*5只是为了画线更清楚,可以不要
                Vector3 norVec = transform.forward * 5;//此处*5只是为了画线更清楚,可以不要
                Vector3 temVec = player.transform.position - transform.position;
                //if (Physics.Raycast(transform.position + Vector3.up * 1f, transform.forward, out hit, raydis))
                //Debug.DrawLine(transform.position, transform.forward, Color.red);//画出技能释放者面对的方向向量
                Debug.DrawRay(transform.position + Vector3.up * 1f, transform.forward, Color.red, 10f);
                Debug.DrawLine(transform.position, player.transform.position, Color.green);//画出技能释放者与目标点的连线
                float jiajiao = Mathf.Acos(Vector3.Dot(norVec.normalized, temVec.normalized)) * Mathf.Rad2Deg;//计算两个向量间的夹角
                if (distance < viewdistance)
                {
                    if (jiajiao <= SkillJiaodu * 0.5f)
                    {
                        Debug.Log("在扇形范围内");
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                }
                else
                {
                    return false;
                }


            }

        }
       
    }
    //检测玩家是否进入攻击距离
    public bool playerisinAttackview()
    {
        if (playerisinview())
        {
            Vector3 selftoplayer = player.transform.position - gameObject.transform.position;
            distancetoplayer = selftoplayer.sqrMagnitude;//实时计算相距玩家的距离
            if (distancetoplayer <= saqAttackdistance)
            {
                //Debug.Log("true");
                return true;
            }
            else {
                //Debug.Log("false");
                return false;
                
            }

        }
        else
        {
            return false;
        }
  

    }
    //检测NPC是否进入视野
    public bool npcisinview()
    {
        //var cols = new List<Collider>(Physics.OverlapSphere(transform.position, viewdistance,NPC));//寻找半径范围内的所有碰撞体
        Collider[] cols = Physics.OverlapSphere(transform.position, viewdistance, NPC);//寻找半径范围内的所有碰撞体
        //cols.Remove(m_collider);//移除掉自身的内容
       
        //var firstnpc=cols.Sort(x=>Vector3.Distance(transform.position,x,transform.position)).
        if (cols.Length>1)
        {
            return true;
        }
        return false;
    }
    //实时返回最近的NPC的位置
    public Transform thenearestNPCtransform()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, viewdistance, NPC);//寻找半径范围内的所有碰撞体
        //cols.Remove(m_collider);//移除掉自身的内容
        float distancetoplayer;
        float mindistace;
        Transform nearstnpcposition;
        if (Vector3.Distance(transform.position, cols[0].transform.position) > 0.1f)
        {
            mindistace = Vector3.Distance(transform.position, cols[0].transform.position);
            nearstnpcposition = cols[0].transform;
        }
        else
        {
            mindistace = Vector3.Distance(transform.position, cols[1].transform.position);
            nearstnpcposition = cols[1].transform;
        }
        
        for (int i = 0; i < cols.Length; i++)
        {
            distancetoplayer = Vector3.Distance(transform.position, cols[i].transform.position);
            if (distancetoplayer >0.1f && distancetoplayer < mindistace)
            {
                mindistace = distancetoplayer;
                nearstnpcposition = cols[i].transform;
            }
            else { }
        }
        return nearstnpcposition;
    }
    //追击最近NPC
    public void movetonpc()
    {
        Transform nearestnpctransform = thenearestNPCtransform();
        Movetogoaldirection(nearestnpctransform.position);

    }
    //检测NPC是否进入攻击距离 这个首先判断视野内有玩家，并根据上边获取到的最近玩家的位置来判断其是否可以攻击，
    public bool NPCisinAttackview()
    {
        if (npcisinview())
        {
            Transform nearestnpc = thenearestNPCtransform();//获取最近的玩家的位置
            if (Vector3.Distance(transform.position, nearestnpc.position) <= Attackdistance)
            {
                return true;
            }
            else if(Vector3.Distance(transform.position, nearestnpc.position) > Attackdistance)
            {
                return false;
            }
        }

        return false;
    }
    //获得最近可攻击NPC的位置(这个接口先忽略，可用实时返回最近NPC代替)


    //攻击最近的NPC
    public void AttackNPC()//去攻击NPC，其内容和攻击玩家是一样的，向当前方向发射光波，但是为作区分，这里复用接口，（调用了攻击玩家的部分函数内容）
    {

        if (skill1flag)
        {
            // animator.SetTrigger("heroforwardsmash");//放火花；
            animator.SetBool("heroforwardsmash_0", true);//放火花；walkbacktosmash
            animator.SetBool("walkbacktosmash", true);
            //runflag = false;//放技能时人不能动
            //animator.SetBool("Walk Backward", false);
            skill1flag = false;//技能进入不可释放状态,防止连续快速按键，既进入cd；
            Invoke("instantiatskill1", 0.5f);
            Invoke("awakewalk", 1f);
            Invoke("awakeskill1", 1.5f);
            animator.SetBool("heroforwardsmash_0", false);//放火花；walkbacktosmash
            animator.SetBool("walkbacktosmash", false);
            //skill1flag = false;
        }


    }
    //检测自己与玩家间是否有物体
    public bool havehinder()
    {
        RaycastHit hit;
        
        bool havetings= Physics.Raycast(transform.position + Vector3.up * 1.5f, (player.transform.position + Vector3.up * 1.5f) - transform.position, out hit, Vector3.Distance(player.transform.position, transform.position));
        if (havetings)
        {
            if (hit.collider.gameObject.name == "Player")
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        else
        {
            return false;
        }
           
        
    }
    //智能判断是否需要跳跃
    public bool thistimeineedjump()
    {
        if (IsGrounded())//添加；是否在地面上的判断
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position + Vector3.up * 1f, transform.forward, out hit, raydis))
            {
                Debug.DrawRay(transform.position + Vector3.up * 3.8f, transform.forward, Color.red, 10f);
                if (!Physics.Raycast(transform.position + Vector3.up * 3.8f, transform.forward, out hit, 10f))
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }

    }
    public bool icanjumpout()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up * 1f, transform.forward, out hit, raydis))
        {
            Debug.DrawRay(transform.position + Vector3.up * 3.3f, transform.forward, Color.blue, 3f);
            if (!Physics.Raycast(transform.position + Vector3.up * 3.3f, transform.forward, out hit, 3f))
            {
               
                return true;
            }
            else
            {
                return false;
            }

        }
        else
        {
            return false;
        }
 
    }
    public void selfjump()
    {
        
        animator.SetTrigger("herorunjump");
            //animator.SetBool("herorunjump_0", true);//跑跳；  
         //Debug.Log("跳");
    }
    //动画机用归否函数
    public void selfnoaction()
    {
        animator.SetBool("herorunjump_0", false);//跑跳；
        animator.SetBool("Walk Forward", false);

    }
    // 通过射线检测主角是否落在地面或者物体上  
    bool IsGrounded()
    {
        //这里transform.position 一般在物体的中间位置，注意根据需要修改margin的值
        return Physics.Raycast(transform.position, -Vector3.up, margin);
    }
    //打击玩家 向玩家发射红色伤害光波 此为怪物的技能一 使用时调用Attackplayer即可
    public void Attackpalyerwithmainskill()
    {if(skill1flag)
            {
            // animator.SetTrigger("heroforwardsmash");//放火花；
            animator.SetBool("heroforwardsmash_0", true);//放火花；walkbacktosmash
            animator.SetBool("walkbacktosmash", true);
            animator.Play("ForwardSmash");
            //runflag = false;//放技能时人不能动
            //animator.SetBool("Walk Backward", false);
            skill1flag = false;//技能进入不可释放状态,防止连续快速按键，既进入cd；
            Invoke("instantiatskill1", 0.5f);
            Invoke("awakewalk", 1f);
            Invoke("awakeskill1", mainskillcd);
            animator.SetBool("heroforwardsmash_0", false);//放火花；walkbacktosmash
            animator.SetBool("walkbacktosmash", false);
            //skill1flag = false;
        }
        

    }
    void instantiatskill1()
    {
        var skill2 = Instantiate(newskill1fire);
        skill2.transform.position = newskill1point.transform.position;
        skill2.GetComponent<GiveDamage>().damage = Attack;//赋予技能攻击力
        Rigidbody skill2rigid = skill2.GetComponent<Rigidbody>();
        skill2rigid.velocity = newskill1point.transform.forward * speedofskilll1;


    }
    void awakewalk()
    {
        //runflag = true;//技能放完了，人可以动了(这里先不用它)
    }
    void awakeskill1()
    {
        skill1flag = true;//此技能冷却完毕，可以释放下一次
    }
    public void nouseskill1()
    {
        animator.SetBool("heroforwardsmash_0", false);//禁止放技能2；
        animator.SetBool("walkbacktosmash", false);
    }
    //自身实时朝向玩家//带有一定延迟
    public void lookatpalyer()
    {
        Vector3 goalmovedir = player.transform.position - gameObject.transform.position;//指向玩家的向量
        goalmovedir = new Vector3(goalmovedir.x, 0, goalmovedir.z);
        var stopturnto = Quaternion.LookRotation(goalmovedir);//转换成四元数
        Quaternion stopslerp = Quaternion.Slerp(transform.rotation, stopturnto, turnspeed * Time.deltaTime);
        transform.rotation = stopslerp;
        return;
    }
    //自身血条处理
    public void showselflife()
    {
        lifeshow.transform.rotation = Camera.main.transform.rotation;
        //lifeshow.transform.LookAt(Camera.main.transform);
        //lifeshow.transform.localRotation = maincamera.transform.localRotation;
        //Vector3 newVec = Quaternion.AngleAxis(90f, maincamera.transform.forward)*;
        //lifeshow.transform.position = gameObject.transform.position + Vector3.up * 2.5f;
        //Vector3 tocamera = maincamera.transform.position - lifeshow.transform.position;
        //Vector3 newVec = maincamera.transform.up;
        //var myrotation = Quaternion.LookRotation(tocamera);//转换成四元数
        //lifeshow.transform.localRotation = myrotation;
        //Debug.Log("i chan");
        //以上弃用
        scale.x = lifeValue / 100;
        Point.transform.localScale = scale;
    }
    //自身被攻击
    public void selfbeAttack()
    {
        if (beAttack)
        {
            beAttack = false;
            
            lifeValue = lifeValue - player.GetComponent<PlayCharacter>().attackPower;
            if (lifeValue<0)
            {
                 Destroy(gameObject);
            }
        }
    }
    //被调用的被攻击的接口（强行扣血）
 public void Attackenemyself()
    {
      
            beAttack = false;
            
            lifeValue = lifeValue - player.GetComponent<PlayCharacter>().attackPower;
            if (lifeValue<0)
            {
                 Destroy(gameObject);
            }
       
    }
    //判断自身血量函数(暂定健康血量为30)
    public bool selfvalueisok()
    {
        if (lifeValue<30f)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    //怪物自身范围随机行走,(这个可以先不用，所以先置空)
    public void monsterrandomwalk()
    {
       
        bool nowisbacktorange = true;//是否走出圈的标志
       
        if (Vector3.Distance(transform.position, burnposition) >= workrange)
        {
            nowisbacktorange = false;
        }
        if (Vector3.Distance(transform.position, burnposition) < workrange && nowisbacktorange)
        {
            //Debug.Log("currentState; " + currentState);
            switch (currentState)
            {
                //待机状态，等待actRestTme后重新随机指令
                case 1:

                    if (Time.time - lastActTime > actRestTme)
                    {
                        RandomAction();         //随机切换指令
                    }
                    //该状态下的检测指令
                    playerstopmove();
                    break;

                //待机状态，由于观察动画时间较长，并希望动画完整播放，故等待时间是根据一个完整动画的播放长度，而不是指令间隔时间
                case 2:
                    // if (Time.time - lastActTime > animator.GetCurrentAnimatorStateInfo(0).length)
                    if (Time.time - lastActTime > 1.5f)
                    {
                        RandomAction();         //随机切换指令
                    }

                    //该状态下的检测指令
                    // EnemyDistanceCheck();
                    break;

                //游走，根据状态随机时生成的目标位置修改朝向，并向前移动
                case 3:
                    //transform.Translate(Vector3.forward * Time.deltaTime * walkSpeed);
                    //transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed);
                    slowMovetogoalwithlittleAI(targetRotation);
                    if (Time.time - lastActTime > actRestTme)
                    {
                        RandomAction();         //随机切换指令
                    }
                    //该状态下的检测指令
                    //WanderRadiusCheck();
                    break;
            }
        }
        else if (nowisbacktorange==false)
            {
           // Debug.Log("出范围了！");
           
                //Vector3 goalposition = burnposition + Quaternion.AngleAxis(Random.Range(0, 360f), Vector3.up) * transform.forward * Random.Range(0, workrange / 3f);
           
            slowMovetogoalwithlittleAI(burnposition-new Vector3(transform.position.x,0,transform.position.z));
            if (Vector3.Distance(transform.position,burnposition)<=workrange/3f)
            {
                nowisbacktorange = true;
            }
        }
       
    }
    void RandomAction()
    {
        //更新行动时间
        lastActTime = Time.time;
        //根据权重随机
        float number = Random.Range(0, actionWeight[0] + actionWeight[1] + actionWeight[2]);
        if (number <= actionWeight[0])
        {
            currentState = 1;//站立
            playerstopmove();
            // animator.SetTrigger("Stand");
        }
        else if (actionWeight[0] < number && number <= actionWeight[0] + actionWeight[1])
        {
            currentState = 2;//待机
            playerstopmove();
            animator.SetTrigger("JabTrigger");
        }
        if (actionWeight[0] + actionWeight[1] < number && number <= actionWeight[0] + actionWeight[1] + actionWeight[2])
        {
            currentState = 3;//行走
            //随机一个朝向
            targetRotation = Quaternion.Euler(0, Random.Range(1, 5) * 90, 0);
            //animator.SetBool("Walk Forward",true);
        }
    }
    //怪物巡逻函数,(这个也先置空吧 )(怪物按事先指定好的路线行走，利用标志方块，总是寻找下一个方块 带寻路方式)
    public void monsterpatrol()
    {if (nextGuidepost!=null)
        {
            // nextGuidepost
            if (Vector3.Distance(transform.position, nextGuidepost.transform.position) > 3f)
            { slowMovetogoalwithlittleAI(nextGuidepost.transform.position - transform.position); }
            else if (Vector3.Distance(transform.position, nextGuidepost.transform.position) <= 3f)
            {
                nextGuidepost = nextGuidepost.GetComponent<scriptforGuidepost>().nextguidepost;
            }
        }
       
        
    }
    //发现强力玩家函数 如果是的话，返回真。（仅是战斗力检测）
    public bool findstrongplayer()
    {
        if (player.GetComponent<PlayCharacter>().attackPower/Attack>6||!selfvalueisok())//如果玩家的战斗力是自身的六倍以上的话，返回真
        {
            return true;
        }
        else
        {
            return false;
        }
       
    }
    //怪物逃跑escap（怪物向玩家相反方向逃跑，跑到玩家脱离视野为止，停下）
    public void monsterescape()
    {
        Vector3 goalmovedir =  gameObject.transform.position- player.transform.position;
        MovetogoalwithlittleAI(goalmovedir);
    }
    void Update () {
       
        // icanjumpout();
        // selfnoaction();//动画机布尔值回归

        //测试功能
        /*
        if (npcisinview())
        {
            Debug.Log("发现NPC！");
            Debug.Log("最近NPC位置；"+ thenearestNPCtransform());

        }
       */
        //测试用；
        //slowMovetogoalwithlittleAI(burnposition-transform.position);
        //测试用；

    }
    //智能战斗函数，综合战斗行为的ai逻辑
    public void AIAttack()
    {
        playerstopmovetohero();
        playerstopmove();
        lookatpalyer();//怪物实时朝向玩家
        Attackpalyerwithmainskill();//怪物攻击玩家
       
    }

    public void moreAIattack()
    {
        Vector3 goalmovedir = player.transform.position - gameObject.transform.position;//指向玩家的向量
        distancetoplayer = goalmovedir.sqrMagnitude;//实时计算相距玩家的距离
        if (skill1flag == true && distancetoplayer <= saqAttackdistance)
        {
            //playerstopmovetohero();
            playerstopmove();
            lookatpalyer();//怪物实时朝向玩家
            Attackpalyerwithmainskill();//怪物攻击玩家
            //Debug.Log("现在使用技能");

        }
        else if (skill1flag == false && distancetoplayer > saqlongattackdistance)
        {

            Movetoplayer();
            //playerstopmove();
            // playerstartmovetohero();
            lookatpalyer();//怪物实时朝向玩家
            runflagtohero = true;
        
            //MovetoplayerwithmoerAI();
           // Debug.Log("没招来 追玩家");
            Debug.Log(runflagtohero);
        }

        else if (skill1flag == false && distancetoplayer <= saqlongattackdistance && distancetoplayer > saqshortattackdistance)
        {
            //playerstopmovetohero();
            playerstopmove();
            lookatpalyer();//怪物实时朝向玩家
            monsterNormalAttacklong();//怪物攻击玩家
            //Debug.Log("长距离普攻");

        }
        else if (skill1flag == false && distancetoplayer <= saqshortattackdistance)
        {
            //playerstopmovetohero();
           playerstopmove();
            lookatpalyer();//怪物实时朝向玩家
            monsterNormalAttackshort();//怪物攻击玩家
            //Debug.Log("短距离普攻");

        }
       /* switch (attackstate)//因为可以使用状态判断来进行设定，先不适用有限状态机 使用传统的状态片段
        {
            case 0:     //短距离攻击

                break;

            case 1:    //长距离攻击

                break;
            case 2:   //技能攻击

                break;
            case 3:  //追击

                break;
    

        }*/
    }

    private void monsterNormalAttackshort()
    {if (normalattackshortflag)
        {
            //animator.SetBool("monsternormalattract_short", true);//普通攻击1
            animator.SetTrigger("monsternormalattract_short _trigger");
            //runflag = false;//放技能时人不能动
            //animator.SetBool("Walk Backward", false);
            normalattackshortflag = false;//技能进入不可释放状态，既进入cd；
            Invoke("startnormalshortattrack",0f);
            Invoke("EndAttackshort", 0.1f);
            Invoke("awakenormalshortattrack", 1.8f);//cd结束可以普攻
            //Invoke("awakewalk", 1f);
            //Invoke("awakeskill1", 1.5f);
            //animator.SetBool("monsternormalattract_short", false);//放火花；walkbacktosmash
                                                                  //animator.SetBool("walkbacktosmash", false);
                                                                  //skill1flag = false;
        }
    }
    private void startnormalshortattrack()
    {
        AttackshortArea.SetActive(true);
        AttackshortArea.GetComponent<GiveDamage>().flying = false;
        AttackshortArea.GetComponent<GiveDamage>().damage = shortattackpower;
    }
    private void EndAttackshort()
    {
        AttackshortArea.SetActive(false);
    }
    private void awakenormalshortattrack()
    {
        normalattackshortflag = true;
    }


    private void monsterNormalAttacklong()
    {
        if (normalattacklongflag)
        {
            //animator.SetBool("monsternormalattract_long", true);//普通攻击1
            animator.SetTrigger("monsternormalattract_long_trigger");
            //runflag = false;//放技能时人不能动
            //animator.SetBool("Walk Backward", false);
            normalattacklongflag = false;//技能进入不可释放状态，既进入cd；
            Invoke("startnormallongattrack", 0.1f);
            Invoke("EndAttacklong", 0.2f);
            Invoke("awakenormallongattrack", 1.8f);//cd结束可以普攻
            //Invoke("awakewalk", 1f);
            //Invoke("awakeskill1", 1.5f);
            //animator.SetBool("monsternormalattract_long", false);//放火花；walkbacktosmash
                                                                  //animator.SetBool("walkbacktosmash", false);
                                                                  //skill1flag = false;
        }
    }
    private void startnormallongattrack()
    {
        AttacklongArea.SetActive(true);
        AttacklongArea.GetComponent<GiveDamage>().flying = false;
        AttacklongArea.GetComponent<GiveDamage>().damage = longattackpower;
    }
    private void EndAttacklong()
    {
        AttacklongArea.SetActive(false);
    }
    private void awakenormallongattrack()
    {
        normalattacklongflag = true;
    }

    private void AIattackmachine()
    {

    }

}

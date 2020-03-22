using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayCharacter : MonoBehaviour
{
    //自身属性；
    [Header("Base Attributes")]
    public string Name;//角色名字
    public float life;//生命值
    public float maxLife;//最大生命值
    public float energy = 100;//能量值
    public float maxEnergy = 100;//最大能量值


    [Header("Move")]
    public bool canMove = true;//是否可以移动
    public float walkSpeed = 8;//行走速度
    public float runSpeed = 12;//奔跑速度
    public float moveSpeed = 10;//实时移动速度
    private float turnSpeed = 20;//转向速度
    //private bool aotuMoving = false;//是否正在自动行走
    [SerializeField] private bool moving = false;//是否正在移动


    [Header("Attack")]
    public bool BattleMode;//战斗模式判断
    public bool canAttack;//是否可以普攻
    public bool autoAttack;//是否自动攻击
    public float attackPower;//攻击力（攻击力折算成各个技能的伤害）
    public float attackSpeed;//攻击速度（1s内攻击次数）
    public float attackDistance;//攻击距离
    public float detectArea = 10;//攻击检测范围，可对此半径内的物体实行自动攻击
    public GameObject AttackArea;//自身普攻检测用的子物体
    private int attackIndex = 0;//攻击次数标记，用于连击动画
    private IEnumerator _autoAttack;//自动攻击的协程
    private IEnumerator _autoMove;//自动移动的协程

    [Header("Skill")]
    public bool canReleaseSkills = true;//是否可以释放技能
    public bool land;//是否着地
    public float jumpForce;//力度，决定跳跃高度
    public float rushDistance;//滚动距离
    [Header("2D Mode")]
    public bool is2dmode = false;//2d模式的判断
    public bool nowischangemode = false;//是否在变换模式的判断
    public GameControl gc;
    Rigidbody rigid;
    private Transform cameraTarget;//标志相机方向专用组件
    public LayerMask monster;//npc的层级

    //private bool crouchBool = false;
    //private bool blockBool = false;
    //private bool dead = false;
    //private bool InAir = false;
    public Animator animator;
    //private Vector3 resetPos;
    //private Quaternion resetRot;
    [SerializeField] private ValueBar lifeValueBar, EnergyValueBar;//血条和能量条

    public GameObject smoke;//烟雾原型
    private GameObject newsmoke;//新烟雾
    public GameObject smokebornpoint;//烟雾出生点

    public SkillDoubleCd jumpButton;//跳跃按钮
    public SkillCd rushButton;//翻滚按钮
    public SwitchSprite ChangeMoveButton;//切换移动方式的按钮
    // public bool heroisrun = false;//玩家是不z进状态的一个标志

    // Use this for initialization
    void Awake()
    {
        cameraTarget = GameObject.FindObjectOfType<CFollow>().transform;
    }
    void Start()
    {
        /*
        if (PlayerPrefs.GetInt("havefile") == 1 && SceneManager.GetActiveScene().name == PlayerPrefs.GetString("scenename"))//如果有存档，且在当前场景（当前场景判断是为了在测试时方便，只是暂时保留）
        {
            float x = PlayerPrefs.GetFloat("restartx");
            float y = PlayerPrefs.GetFloat("restarty");
            float z = PlayerPrefs.GetFloat("restartz");
            if (PlayerPrefs.HasKey("nowskill"))
            {
                skillIndex = PlayerPrefs.GetInt("nowskill");
            }
            transform.position = new Vector3(x, y, z);
        }
         */

        //cc = gameObject.GetComponent<CharacterController>();
        AttackArea.SetActive(false);
        rigid = gameObject.GetComponent<Rigidbody>();
        is2dmode = false;//初始时是3d模式
        attackDistance = AttackArea.GetComponent<BoxCollider>().size.z;//获取攻击距离

        //获得能量条和血条
        lifeValueBar = GameObject.Find("Canvas/Head/Life").GetComponent<ValueBar>();
        EnergyValueBar = GameObject.Find("Canvas/Head/Energy").GetComponent<ValueBar>();

        //设置生命值和能量值
        ChangeEnergyValue(maxEnergy);
        ChangeLifeValue(maxLife);
        //显示血条
        ShowStatus();
    }
    void Update()
    {
        if (life > 0)
        {

            if (!moving)//如果不动，则随时间恢复能量值
            {
                if (energy == maxEnergy) return;
                ChangeEnergyValue(3 * Time.deltaTime);//每秒恢复3点
            }
            else
            {
                if (IsRunning()) return;//如果正在奔跑则不回复

                if (energy == maxEnergy) return;
                ChangeEnergyValue(1 * Time.deltaTime);//每秒恢复1点
            }
        }

    }

    #region 移动
    //定义人物运动接口；
    public void Move(Vector2 mVector)
    {
        if (canMove == false) return;//如果无法移动
        if (BattleMode && IsRunning())//奔跑
        {
            if (energy < moveSpeed * 0.05f)
            {
                GameManager.Instance.Tip("能量不足");
                ChangeMoveMode();
                return;
            }
            ChangeEnergyValue(-moveSpeed * 0.01f);
        }
        moving = true;
        if (_autoMove != null) StopCoroutine(_autoMove);//停止自动行走
        if (_autoAttack != null) StopCoroutine(_autoAttack);//停止自动攻击
        if (is2dmode == false)
        {
            Vector3 tempV = new Vector3(mVector.x, 0, mVector.y);
            //mVector.y = 0;//不考虑竖直位移
            Vector3 _mVector = Quaternion.AngleAxis(cameraTarget.eulerAngles.y, Vector3.up) * tempV;//将向量旋转到Cameratargt的方向
            Move3d(_mVector);
        }
        if (is2dmode == true)
        {
            Move2d(mVector);
        }
        if (nowischangemode)
        {
            StopMove();
        }
    }

    public void Move3d(Vector3 moveVector)//此处处理人物转向，让人物一直转向当前走的方向，然后播放动画
    {
        if (canMove == false) return;//如果无法移动；
        
        //print("Velocity:" + rigid.velocity.magnitude);
        //Vector3 newMoveVector = new Vector3(moveVector.x, 0, moveVector.y);//多维向量转换，处理掉Y轴
        //旋转到当前视角的正方向
        var turnto = Quaternion.LookRotation(moveVector);//转换成四元数
        //Quaternion slerp = turnto;
        transform.rotation = Quaternion.Slerp(transform.rotation, turnto, turnSpeed * Time.deltaTime);//玩家旋转
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        if (!land) return;//如果未落地则不播放动画
        float midSpeed = (walkSpeed + runSpeed) / 2;//中间速度，决定播放走或跑的动作
        //根据移动速度设置动画播放速度
        if (moveSpeed <= midSpeed)
        {
            animator.SetInteger("Move", 1);
            animator.SetFloat("MoveSpeed", 0.25f + moveSpeed / midSpeed);
        }
        else if (moveSpeed > midSpeed)
        {
            animator.SetInteger("Move", 2);
            animator.SetFloat("MoveSpeed", moveSpeed / midSpeed);
        }
        //transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);

        /*
        if (oldmovedir.z > 0)
        {
            animator.SetBool("Run", true);
            cc.SimpleMove(transform.forward);
        }

      

        //var goalvector = transform.localRotation + movedir;
        // Vector3 nowdirectiom =  transform.forward;
        // movedir= nowdirectiom+ (movedir-Vector3.forward);
      
        if (movedir.x < 0)
        {
            movedir = -transform.right;
        }
        else if (movedir.x > 0)
        {
            movedir = transform.right;
        }
        else if (movedir.z < 0)
        {
            movedir = -2*transform.forward;
        }
        else
        {
            movedir = transform.forward;
            
        }
        movedir = new Vector3(movedir.x,0, movedir.z);

        var turnto = Quaternion.LookRotation(movedir + transform.forward);//转换成四元数
        if (oldmovedir.z >= 0)
        {
            Quaternion slerp = Quaternion.Slerp(transform.rotation, turnto, turnspeed * Time.deltaTime);
            transform.rotation = slerp;

        }
        else if (oldmovedir.z < 0)
        {
            animator.SetBool("Walk Backward", true);
           cc.SimpleMove(transform.forward);
        }
        
    */
        // rigid.velocity = movedir;


    }
    public void Move2d(Vector3 movedir)//人物移动的2d方式
    {
        if (canMove == false) return;//其它技能限制行走；
        /*
        if (jumpspeed > 0)
        {
            jumpspeed = jumpspeed - 1 * Time.deltaTime;
        }
        if (jumpspeed < 0)
        {
            jumpspeed = 0;
        }
        */
        movedir = new Vector3(0, 0, -movedir.x);
        if (movedir.magnitude > 0)
        {
            Vector3 oldmovedir = movedir;
            Vector3 newVec = Quaternion.AngleAxis(cameraTarget.eulerAngles.z, transform.up) * oldmovedir;
            //movedir = new Vector3();
            var turnto = Quaternion.LookRotation(newVec);//转换成四元数
            Quaternion slerp = Quaternion.Slerp(transform.rotation, turnto, 10000);
            transform.rotation = slerp;
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
            //rigid.AddForce(transform.up * jumpspeed);
            float midSpeed = (walkSpeed + runSpeed) / 2;//中间速度，决定播放走或跑的动作
            if (moveSpeed <= midSpeed)
            {
                animator.SetInteger("Move", 1);
                animator.SetFloat("MoveSpeed", 0.25f + moveSpeed / midSpeed);
            }
            else if (moveSpeed > midSpeed)
            {
                animator.SetInteger("Move", 2);
                animator.SetFloat("MoveSpeed", moveSpeed / midSpeed);
            }
            //animator.SetBool("stand", false);
        }
        //rigid.AddForce(transform.up * jumpspeed * 10000);
        //rigid.AddForce(transform.up * jumpspeed + transform.forward * jumpspeed);
    }


    public void StopMove()//停止人物行走
    {
        animator.SetInteger("Move", 0);
        moving = false;
        
        //animator.SetFloat("RunSpeed", 0);
    }
    public void AutoMoveTo(Transform obj, float outArea)//自动移动到指定对象身边，第二个参数为停止时的距离
    {
        if (_autoMove != null) StopCoroutine(_autoMove);//停止其他自动移动的协程
        if (_autoAttack != null) StopCoroutine(_autoAttack);//停止自动攻击
        _autoMove = MoveToObj(obj, outArea);
        StartCoroutine(_autoMove);
        moving = true;
    }

    IEnumerator MoveToObj(Transform gl, float minD)//提供自动移动调用的协程
    {
        float Distance = Vector3.Distance(transform.position, gl.position);//玩家当前位置和目标位置的距离

        while (Distance > minD)
        {
            Distance = Vector3.Distance(transform.position, gl.position);
            Vector3 moveD = gl.position - transform.position;
            moveD.y = 0;//不考虑垂直方向的位移
            //print(Distance);
            RaycastHit hit;
            if (Physics.Raycast(transform.position + Vector3.up * 1f, moveD, out hit, minD))//向正前方发射一条射线，如果有障碍物
            {
                StopMove();
            }
            else Move3d(moveD);
            yield return null;
        }
        //到达目标位置，停止移动
        StopMove();
        //如果目标有对话，则直接开始对话
        if (gl.GetComponent<ChatArea>()) gl.GetComponent<ChatArea>().StartChat();
        yield break;
    }



    //接口；产生烟雾////////////////////////////////////////////////////////////////// 

    public void playerrunsmoke()
    {
        //检测一下现在人物的状态，只有跑的时候才产生烟雾效果
        if (animator.GetBool("Run") == true)//如果它是正在跑的话
        {
            newsmoke = Instantiate(smoke);
            newsmoke.transform.position = smokebornpoint.transform.position;
        }

    }

    #endregion

    #region 翻滚
    //***************翻滚
    public void Rush()
    {
        if (!land) return;//如果处于悬空状态则不能翻滚
        if (energy < 10)
        {
            GameManager.Instance.Tip("能量不足");
            return;
        }
        ChangeEnergyValue(-10);

        rushButton.ResetCD();//技能进入冷却
        DisableMove();
        animator.SetFloat("RollSpeed", 0.5f * moveSpeed / 6 + 0.2f);//滚动速度转换式
        animator.Play("Roll");
        Invoke("EnableMove", 5 / (moveSpeed * 2));
        //检测前方是否有碰撞体
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, transform.forward, out hit, rushDistance))
        {
            //print("Rush Distance:" + rushDistance);
            //print("Hit Distance:" + hit.distance);
            StartCoroutine(RollForward(hit.distance, moveSpeed * 2));
        }
        else StartCoroutine(RollForward(rushDistance, moveSpeed * 2));

    }

    IEnumerator RollForward(float rollDistance, float rollSpeed)//协程，翻滚的同时向前移动
    {
        float rollingDis = 0;
        //Vector3 targetPos = transform.position + Vector3.forward * rollDistance;
        //print("Roll Target Position:" + targetPos);
        //Vector3 oldPos = this.transform.position;
        //print("弹出对话框");
        while (rollingDis < rollDistance)
        {
            rollingDis += rollSpeed * Time.deltaTime;
            transform.Translate(Vector3.forward * rollSpeed * Time.deltaTime);
            yield return null;
        }

    }
    #endregion

    #region 跳跃和悬空
    public void Jump()//跳跃
    {
        //float posY = 0;
        if (land)//若已经着地，则跳起
        {
            land = false;
            if (energy < 10)
            {
                GameManager.Instance.Tip("能量不足");
                return;
            }
            ChangeEnergyValue(-10);//消耗能量
            JumpUp(jumpForce);
            return;
        }
        else//若在空中，则翻滚
        {
            if (energy < 15)
            {
                GameManager.Instance.Tip("能量不足");
                return;
            }
            ChangeEnergyValue(-15);//消耗能量
            JumpFlip(1.2f * jumpForce);
            return;
        }
    }

    void JumpUp(float jForce)//跳起
    {
        if (jForce > 0)
        {
            rigid.AddForce(transform.up * jForce);
            animator.Play("Jump_Up");//处理按钮状态
            jumpButton.ResetCD();
        }
        animator.SetBool("Land", false);

        jumpButton.Enable2();
        //禁用滚动按钮
        if (null != rushButton.GetComponent<ButtonNormal>()) rushButton.GetComponent<ButtonNormal>().DisableButton();
    }
    void JumpFlip(float fForce)//空中翻滚
    {
        rigid.velocity = Vector3.zero;//施力前先让刚体速度为零，以保证高度一定
        rigid.AddForce(transform.up * 1.2f * jumpForce);
        /*
        Vector3 rigiV = rigid.velocity;
        print("V.y:"+rigiV.y);
        if (rigiV.y > 0) rigid.AddForce(transform.up * ( 100*rigiV.y));
        else rigid.AddForce(transform.up * (-200 * rigiV.y));
         */
        animator.Play("Jump_Flip");

        //处理技能状态
        jumpButton.ResetCD();
        jumpButton.Release2();
    }
    void OnCollisionEnter(Collision collision)//落地
    {

        animator.SetBool("Land", true);//回到初始状态；
        land = true;
        jumpButton.Baketo1();
        if (rushButton.Prepared() && null != rushButton.GetComponent<ButtonNormal>()) rushButton.GetComponent<ButtonNormal>().EnableButton();

    }
    void OnCollisionExit(Collision collision)//悬空
    {
        RaycastHit hit;
        //print(inAir);
        if (land)
        {
            if (!Physics.Raycast(transform.position, Vector3.down, out hit, 2f))
            {
                JumpUp(0);
                land = false;
            }
        }

    }
    #endregion

    #region 攻击
    //********************普通攻击
    public void NormalAttack()
    {
        Transform mstr = nearestmonster(detectArea);//检测距离最近的敌人
        if (mstr == null)
        {
            Attack(null);
            return;
        }
        //判断与怪物的距离
        float d = Vector3.Distance(transform.position, mstr.position);//与怪物的距离
        if (d > detectArea) Attack(null);
        else if (d <= detectArea && d > attackDistance)
        {
            if (_autoMove != null) StopCoroutine(_autoMove);//停止其他的协程
            if (_autoAttack != null) StopCoroutine(_autoAttack);
            _autoAttack = AutoAttack(mstr);
            StartCoroutine(_autoAttack);
        }
        else
        {
            if (autoAttack)//如果开启了自动攻击
            {
                if (_autoMove != null) StopCoroutine(_autoMove);//停止其他的协程
                if (_autoAttack != null) StopCoroutine(_autoAttack);
                _autoAttack = AutoAttack(mstr);
                StartCoroutine(_autoAttack);
            }
            else Attack(mstr);
        }
    }

    private void Attack(Transform mstr)    //供普通攻击调用
    {
        //如果能量不足
        if (energy < 7)
        {
            GameManager.Instance.Tip("能量不足");
            return;
        }


        if (!canAttack) return;//如果不能攻击
        if (!BattleMode) Normal2Battle();///进入战斗模式
                                         ///
        attackIndex++;
        if (attackIndex > 5) attackIndex = 1;
        //暂时禁用移动、攻击和释放技能
        canAttack = false;
        DisableMove();
        canReleaseSkills = false;
        if (mstr != null) TurnAtRightNow(mstr);//转向敌人
        animator.SetFloat("AttackSpeed", attackSpeed * 1);
        animator.SetInteger("AttackIndex", attackIndex);

        //animator.Play("Attack");

        //StartCoroutine(RollForward(5));
        //animator.SetBool("normalAttack", false);//普通攻击动画通路关闭；
        //Invoke("startAttackpart", 0.1f);//0.1秒后开启自身普攻检测组件

        Invoke("StartAttack", 0.5f / attackSpeed);
        Invoke("EndAttack", 1 / attackSpeed);
        //Invoke("EnableNormalAttack", 1 / attackSpeed);//重新激活普攻（cd）
        //Invoke("EnableMove", 0.5f);
        //Invoke("noAttackanimplay", 0.1f);//禁止普攻动作的动画机流通
    }
    private void StartAttack()//开始本次攻击
    {
        ChangeEnergyValue(-7);//消耗7能量值
        AttackArea.SetActive(true);
        //AttackArea.transform.localScale = Vector3.one;
        AttackArea.GetComponent<GiveDamage>().flying = false;
        AttackArea.GetComponent<GiveDamage>().damage = attackPower;//造成攻击力数值的伤害
    }
    private void EndAttack()//结束本次攻击，激活下一次
    {
        AttackArea.SetActive(false);
        EnableMove();
        canReleaseSkills = true;
        canAttack = true;
    }
    public void SetAttackIndexZero()//设置动画机中的变量
    {
        animator.SetInteger("AttackIndex", 0);
    }

    IEnumerator AutoAttack(Transform enemy)//自动走向敌人并攻击
    {
        float Distance = Vector3.Distance(transform.position, enemy.position);//玩家当前位置和目标位置的距离
        //print("Distance:"+Distance);
        while (Distance > attackDistance)
        {
            Distance = Vector3.Distance(transform.position, enemy.position);
            Vector3 moveD = enemy.position - transform.position;
            moveD.y = 0;//不考虑垂直方向的位移
            Move3d(moveD);
            yield return null;
        }
        if (Distance <= attackDistance)
        {
            StopMove();
            EnemyMotion em = enemy.GetComponent<EnemyMotion>();
            if (autoAttack)//如果开启了自动攻击
            {
                while (em.IsLive())
                {
                    Attack(enemy);
                    yield return null;
                }
                yield break;
            }
            else
            {
                Attack(enemy);
                yield break;
            }


        }
    }
    /*
    private void EnableNormalAttack()//启用普通攻击
    {
        canMove = true;
        canAttack = true;
    }
     */
    //private void noAttackanimplay()
    //{
    //    animator.SetBool("Attack", false);//普通攻击动画通路关闭；
    //}

    #endregion

    #region 转向
    //接口，实时朝向最近怪物
    public void TurnAtRightNow(Transform goal)
    {
        if (goal == null) return;
        Vector3 goalmovedir = goal.position - gameObject.transform.position;//指最近怪物
        goalmovedir = new Vector3(goalmovedir.x, 0, goalmovedir.z);
        var stopturnto = Quaternion.LookRotation(goalmovedir);//转换成四元数
        Quaternion stopslerp = Quaternion.Slerp(transform.rotation, stopturnto, 1);
        transform.rotation = stopslerp;
        return;
    }
    private Transform nearestmonster(float radiu)//在一定范围内距离最近的敌人
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, radiu, monster);//寻找半径范围内的所有碰撞体
        if (cols.Length == 0) return null;

        //Debug.Log("cols.Length " + cols.Length);
        float distancetoplayer;
        float mindistace;
        Transform nearstnpcposition;
        nearstnpcposition = cols[0].transform;
        mindistace = Vector3.Distance(transform.position, cols[0].transform.position);
        for (int i = 0; i < cols.Length; i++)
        {
            distancetoplayer = Vector3.Distance(transform.position, cols[i].transform.position);
            if (distancetoplayer > 0.1f && distancetoplayer < mindistace)
            {
                mindistace = distancetoplayer;
                nearstnpcposition = cols[i].transform;
            }
            else { }
        }
        return nearstnpcposition;
    }
    public void TurnAt(Transform target)//转向目标
    {
        Vector3 _tPos = target.position - transform.position;
        _tPos.y = 0;
        //print("target position:" + target.position);
        _tPos = Quaternion.AngleAxis(-transform.eulerAngles.y, Vector3.up) * _tPos;//旋转到自身方向的坐标系
        Vector2 tPos = new Vector2(_tPos.x, _tPos.z);
        //print("transformed target Position:" + tPos);
        tPos.Normalize();
        //计算旋转角
        float angle = VectorAngle(Vector2.up, tPos);
        //print("turn angle:" + angle); 
        animator.SetFloat("TurnAngle", angle);
        animator.SetFloat("TurnSpeed", 0.05f * turnSpeed);
        animator.SetBool("Turn", true);
        //animator.Play("Turn");
        //animator.SetFloat("TurnSpeed", 0);
        //transform.LookAt(target);
    }
    float VectorAngle(Vector2 from, Vector2 to)//计算向量夹角(得到-180度到180度的角）
    {
        Vector3 cross = Vector3.Cross(from, to);
        float angle = Vector2.Angle(from, to);
        return cross.z > 0 ? -angle : angle;
    }
    public void StopTurn()//停止转向动画
    {
        animator.SetBool("Turn", false);
    }
    #endregion

    #region 受伤
    //受到伤害，扣除血量
    public void GetHurt(float Damage, Vector3 HurtDirection)
    {
        if (life == 0) return;
        ChangeLifeValue(-Damage);
        if (life > 0)
        {
            //受到攻击的方向，由碰撞时飞行物和玩家的距离向量决定，用于角色动作
            //将参考系旋转到相对玩家的正方向
            //print("transform.eulerAngles.y:" + transform.eulerAngles.y);
            //print("HurtDirection--1:" + HurtDirection);
            HurtDirection = Quaternion.AngleAxis(-transform.eulerAngles.y, Vector3.up) * HurtDirection;
            //print("HurtDirection--2:" + HurtDirection);
            Vector2 RelVec = new Vector2(HurtDirection.x, HurtDirection.z);
            RelVec.Normalize();
            //print("Relative Vector" + RelVec);
            //print("New HurtDirection:" + HurtDirection);

            animator.SetFloat("GetHurtX", RelVec.x);
            animator.SetFloat("GetHurtY", RelVec.y);
            animator.Play("GetHurt");
            if (!BattleMode) Normal2Battle();//强制战斗
        }
        else Die();




    }
    #endregion

    #region 生命值和能量值变化   

    // public void LossLife(float loseValue)//减血
    // {
    //     life -= loseValue;
    //     if (life < 0) life = 0;
    //     lifeValueBar.SetValue(life / maxLife);
    // }
    public void ChangeLifeValue(float getValue)//回血，减血
    {
        life += getValue;
        if (life > maxLife) life = maxLife;
        else if (life < 0) life = 0;
        lifeValueBar.SetValue(life / maxLife);
    }
    // public void LossEnegy(float lossValue)//减能量
    // {
    //     energy -= lossValue;
    //     if (energy < 0) energy = 0;
    //     EnergyValueBar.SetValue(energy / maxEnergy);
    // }
    public void ChangeEnergyValue(float getValue)//回能量，减能量
    {
        energy += getValue;
        if (energy > maxEnergy) energy = maxEnergy;
        else if (energy < 0) energy = 0;
        EnergyValueBar.SetValue(energy / maxEnergy);

    }

    public void HideStatus()//隐藏血条和能量条
    {
        EnergyValueBar.Hide(1, false);
        lifeValueBar.Hide(1, false);
    }
    public void ShowStatus()//隐藏血条和能量条
    {
        EnergyValueBar.Display(1, true);
        lifeValueBar.Display(1, true);
    }
    #endregion 
    public void EnableMove()//启用移动
    {
        canMove = true;
    }

    public void DisableMove()//禁用移动
    {
        canMove = false;
    }
    public void ChangeMoveMode()//切换行走或奔跑模式
    {
        float midSpeed = (runSpeed + walkSpeed) / 2;//平均速度
        if (moveSpeed < midSpeed)
        {
            moveSpeed = runSpeed;
        }
        else moveSpeed = walkSpeed;
        ChangeMoveButton.Switch();//切换技能图标
    }
    public bool IsRunning()//判断是否正在奔跑
    {
        if (moveSpeed < (runSpeed + walkSpeed) / 2)
            return false;
        else return true;
    }
    public void ChangeBattleMode()//切换战斗模式
    {
        if (!BattleMode) Normal2Battle();
        else Battle2Normal();
    }
    public void Normal2Battle()//进入战斗模式
    {
        GameManager.Instance.Tip("进入战斗模式");
        if(IsRunning())ChangeMoveMode();
        BattleMode = true;
        animator.SetBool("Battle", true);
    }
    public void Battle2Normal()//退出战斗模式
    {
        BattleMode = false;
        animator.SetBool("Battle", false);
    }
    public void Die()//死亡
    {
        //if (Life == 0) return;
        life = 0;
        //隐藏血条和能量条
        EnergyValueBar.Hide(1, true);
        lifeValueBar.Hide(1, true);

        animator.SetBool("Live", false);
        //animator.Play("Die");
        canAttack = false;
        DisableMove();
        canReleaseSkills = false;
        gc.GameOver();
    }


    public void Relive(float ratio)//复活，按比例恢复生命值
    {
        //gc.onRelive();
        //gameObject.SetActive(true);
        ChangeLifeValue(ratio * maxLife);
        animator.SetBool("Live", true);
        Invoke("EndAttack", 1f);//启用移动和攻击
        //Button_Relive.transform.localScale = Vector3.one;

    }
}

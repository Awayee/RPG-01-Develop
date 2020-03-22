using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class skill3self : MonoBehaviour {

    public GameObject goal;//自身的目标
    Rigidbody skill3rigid;//自身物理组件
    public LayerMask monster;//npc的层级
    private float m_uptimer;//时间计数器
    private float m_speed;//初始速度
    private float m_RotSpeed;//旋转速度
    private float m_currLife;//生成后的时间
    private float m_MaxLife;//最长存在时间
    private bool startrun;//正式开始追踪的标志
    Vector3 bornposition;
    private Transform playertransform;
    // Use this for initialization
    void Start () {
        startrun = false;
        playertransform = GameObject.FindGameObjectWithTag("Player").transform;
        bornposition = transform.position;
        m_uptimer = 0;
        m_speed = 20f;
        m_RotSpeed = 10;
        m_currLife = 1;
        m_MaxLife = 10f;
        skill3rigid = gameObject.GetComponent<Rigidbody>();
         Collider[] cols = Physics.OverlapSphere(transform.position, 20, monster);//寻找半径范围内的所有碰撞体
        goal = nearestmonster(cols).gameObject;
        if (goal==gameObject.transform)
        {
            Destroy(gameObject);
        }
        //skill3rigid.velocity = -(goal.transform.position + Vector3.up * 1f - transform.position).normalized * 40;
        if (goal == null)
        {
            Destroy(gameObject);

        }
        if (goal != null)
        {
            var vector3 = transform.position- goal.transform.position;

            var result = Quaternion.AngleAxis(90, Vector3.up) * vector3;
            var turnto = Quaternion.LookRotation(result);//转换成四元数
            var backturnto = Quaternion.LookRotation(-result);//转换成四元数
            Debug.DrawRay(transform.position , result, Color.red);
            if (Random.Range(0f, 1f) < 0.5f)
            {

                transform.rotation = turnto;
                //transform.forward = result;
                //transform.forward = playertransform.right;
                //skill3rigid.velocity = (transform.right.normalized * 40);
                //transform.Translate(transform.right, Space.Self);
            }
            else
            {
                transform.rotation = backturnto;
                //transform.forward = -result;
                // transform.forward = -playertransform.right;
                //skill3rigid.velocity = (-transform.right.normalized * 40);
                //transform.Translate(-transform.right, Space.Self);
            }
        }
           


    }
    public void Move(Vector3 movedir)//此处处理人物转向，让人物一直转向当前走的方向，然后播放动画
    {
        var turnto = Quaternion.LookRotation(movedir);//转换成四元数
        Quaternion slerp = Quaternion.Slerp(transform.rotation, turnto, 5f*Time.deltaTime);
        transform.rotation = slerp;
    }
    private Transform nearestmonster(Collider[] cols)
    {
        float distancetoplayer;
        float mindistace;
        Transform nearstnpcposition;

        if (cols.Length > 0)
        {
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
        else return gameObject.transform;
       
    }
    // Update is called once per frame
    void Update () {
      
		if(goal!=null)
        {
            if (m_uptimer < 0.1f)

            {
                Move(goal.transform.position + Vector3.up * 1f - transform.position);
                transform.Translate(transform.forward, Space.Self);
                m_uptimer += Time.deltaTime;
                 //m_speed -= 2 * Time.deltaTime;
                // transform.position += (goal.transform.position - transform.position) * m_speed * Time.deltaTime;
            }
            else
            {
                Vector3 goalvector = goal.transform.position + Vector3.up * 1f - transform.position;
                if ((transform.forward - goalvector).sqrMagnitude > 0.01f&& startrun==false)
                {
                    Debug.Log("转向!");
                    transform.forward = Vector3.Slerp(transform.forward, goalvector, 10 * Time.deltaTime).normalized * 8;
                    skill3rigid.velocity = transform.forward;
                    startrun = true;
                }
                else {
                    Debug.Log("追踪！");
                    skill3rigid.velocity = ((goal.transform.position + Vector3.up * 1f )- transform.position).normalized*60;
                }



                /*
                // 开始追踪敌人
                Vector3 target = (goal.transform.position - transform.position).normalized;
                float a = Vector3.Angle(transform.forward, target) / m_RotSpeed;//旋转
                if (a > 0.1f || a < -0.1f)
                    transform.forward = Vector3.Slerp(transform.forward, target, Time.deltaTime / a).normalized;
                else
                {
                    m_speed += 2 * Time.deltaTime;
                    transform.forward = Vector3.Slerp(transform.forward, target, 1).normalized;
                }

                transform.position += transform.forward * m_speed * Time.deltaTime;
                */
            }
            m_currLife += Time.deltaTime;
            if (m_currLife > m_MaxLife)
            {
                // 超过生命周期爆炸（不同与击中敌人）
                Destroy(gameObject);
               // Destroy(Instantiate(m_Explosion, transform.position, Quaternion.identity), 1.2f);
            }

         
        
            //Move(goal.transform.position +Vector3.up * 1f - transform.position);
            //transform.Translate(transform.forward*10, Space.Self);
            //skill3rigid.velocity = (goal.transform.position + Vector3.up * 1f - transform.position).normalized*40;
        }
        if (goal == null)
        {
            Destroy(gameObject);

        }


        // Vector3 now = goal.transform.position - transform.position;
        //skill3rigid.AddForce(now.normalized*(1000/ now.magnitude));
        //skill3rigid.velocity = now.normalized * (100 / now.magnitude);
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag=="enemy")
        {
            Destroy(gameObject);
        }
       

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "enemy")
        {
            /*if (other.gameObject.GetComponent<lnxAIlibrary>())
            {
                other.gameObject.GetComponent<lnxAIlibrary>().Attackenemyself();
            }
            else if (other.gameObject.GetComponent<brokenbyplayer>())
            {
                other.gameObject.GetComponent<brokenbyplayer>().beAttack(true);//摧毁物体，并以小技能的方式摧毁
            }
           */
            Destroy(gameObject);
        }

    }

}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//一个引用气泡对话的例子，可以根据NPC与玩家的距离变换对话内容
public class NPCSay : MonoBehaviour
{
    Transform Player;
    [SerializeField] GameObject TextObj; //预制体
    private PopText popText; //弹出气泡的脚本
    public float MaxDis, //最大感应距离，进入这个距离出现弹框
    MinDis; //最今距离，最佳距离，这个区域内弹框最清晰，且区域内弹框不会再变色
    //float k, b = 0; //用于气泡颜色变化
    Transform ThisTransform;
    //说话内容
    public string Max_Text_In = "你好";//进入外感应范围的说话为本，如果为""，则不说话
    public string Min_Text = "";//进入内感应范围的说话为本，如为""，则与Max_Text相同；若为"null"，则不说话
    //public string Min_Text_Out = "";//走出内感应范围
    public string Max_Text_Out = "";//走出外感应范围
    private int state; //状态判断
    private bool saying = false;//正在说话
    [HideInInspector] public bool inview;//在视野内
    [SerializeField] private float height;//气泡高度
    // Use this for initialization
    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player").transform;
        ThisTransform = this.transform;
        state = 0;
        //k = -0.8f / (MaxDis - MinDis); //颜色透明度变化范围为0.8
        //b = 1 - MinDis * k;
        //print("k: " + k + ", b: " + b);
    }

    // Update is called once per frame
    void Update()
    {

        float distance = Vector3.Distance(Player.position, ThisTransform.position); //与玩家的距离

        if (distance <= MaxDis && distance > MinDis)
        {
            //如果在这个范围内气泡已经弹出，执行以下操作，气泡文字透明度随物体与玩家的距离而改变
            if (popText != null)
            {
                if (!popText.displaying) return;
                float alp = Mathf.Lerp(0.05f, 1, (MaxDis - distance) / (MaxDis - MinDis));
                var txt = popText.talkTxt;
                txt.color = new Color(txt.color.r, txt.color.g, txt.color.b, alp);

            }
            if (state == 1) return;
            else if (state == 0)//从外走近
            {
                if (saying && Min_Text == "") return;
                if (Max_Text_In != "") Say(Max_Text_In);
                state = 1;
            }
            else if(state == 2)//从内圈走出
            {
                if (saying && Min_Text == "") return;
                if (Max_Text_Out != "")
                {
                    if(Max_Text_Out == "null")NeverSay();
                    else Say(Max_Text_Out);
                } 
                
                state = 1;
            }
        }

        else if (distance <= MinDis)
        {
            if (state == 2) return;
            else
            {
                state = 2;
                if (Min_Text == "") return;
                else if (Min_Text == "null") NeverSay();
                else Say(Min_Text);
                // if(!inview) popText.follow = false;
                // else popText.follow = true;

            }
        }
        else
        {
            if (state == 0) return;
            else
            {
                NeverSay();
                state = 0;
            }
        }

    }

    public void Say(string Say_Text) //说话
    {
        //print("我要说话");
        //实例化对话气泡
        if (popText == null)
        {
            Transform _TextObj = Instantiate(TextObj).transform;
            popText = _TextObj.GetComponent<PopText>();
            popText.SetPlayer(Player);
        }
        popText.objInView = inview;//告知自己是否在视野内
        popText.h_offset = height;
        popText.PopupText(gameObject, Say_Text);
        saying = true;
    }
    public void NeverSay() //不再说话
    {
        if (popText != null)
        {
            popText.DestroyPopText();
            //Destroy (popText.gameObject, popText.downDuration);
        }
        saying = false;

    }

    //物体出现在屏幕  
    void OnBecameVisible()
    {
        inview = true;
        if (popText != null) popText.objInView = true;
        //Debug.Log (this.name.ToString () + "这个物体出现在屏幕里面了");
    }

    //物体离开屏幕  
    void OnBecameInvisible()
    {
        inview = false;
        if (popText != null) popText.objInView = false;
        //Debug.Log (this.name.ToString () + "这个物体离开屏幕里面了");
    }
}
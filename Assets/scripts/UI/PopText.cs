using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//实时定位对话框，包括分辨率适配
public class PopText : MonoBehaviour
{
    Transform sayingObj; //说话的物体
    [HideInInspector] public Text talkTxt; //文字控件
    private float talkTxt_a; //记录文字透明度
    [HideInInspector] public Image thisImg; //对话框IMAGE属性
    [HideInInspector] public float h_offset = 5; //气泡弹出后高度
    private float h_crnt = 0; //气泡此时的高度
    private float thisImg_a; //记录透明度
    private RectTransform thisRect; //对话框位置大小
    private PlayCharacter playC; //玩家属性
    private Camera mainCamera;//主相机

    public bool onEdge;//是否贴边

    private float wR, hR;//坐标转换
    private float x_max, y_max, x_min, y_min; //屏幕宽高

    [HideInInspector] public bool displaying = false; //是否已显示并弹出
    //public bool follow = true; //是否跟随
    [HideInInspector]public bool objInView = false;//NPC是否在视野内
    [Range(0, 2)] public float upDuration, downDuration; //动画延时
    void Awake()
    {

        //调整UI层级
        Transform cvs = GameManager.Instance.UICanvas;
        //print(cvs.name + ": " + cvs.transform.position);
        if (null == cvs) cvs = GameObject.Find("Canvas").transform; //如果没有获取到，则自行寻找

        transform.SetParent(cvs.GetChild(0)); //设置父物体
        // int tcount = cvs.childCount; //此时画布的子物体个数
        // transform.SetSiblingIndex(tcount - GameManager.Instance.uiCount - 1); //每次调用时都会将新的气泡置于所有对话气泡的顶层

        //获取自身组件
        talkTxt = GetComponentInChildren<Text>(); //文字
        talkTxt_a = talkTxt.color.a;
        thisRect = GetComponent<RectTransform>(); //位置
        thisImg = GetComponent<Image>(); //图片
        thisImg_a = thisImg.color.a;
        //获取坐标转换比例
        wR = GameManager.Instance.wRatio;
        hR = GameManager.Instance.hRatio;
        //获取屏幕大小，确定气泡显示范围
        x_min = (thisRect.sizeDelta.x) / wR;
        y_min = (thisRect.sizeDelta.y / 2) / hR;
        x_max = GameManager.Instance.screenSize.x - x_min;
        y_max = GameManager.Instance.screenSize.y - y_min;
        //获取相机
        mainCamera = Camera.main;

        // x_min = 0;
        // x_max = 1;
        // y_min = 0;
        // y_max = 1;
        //print("X_Min: " + x_min + ", X_Max: " + x_max + ", Y_Min: " + y_min + ", Y_Max: " + y_max);
        //缩放
        // normalScale = new Vector2(thisRect.localScale.x / w, thisRect.localScale.y / h);
        // print("this localScale:" + thisRect.localScale);
        // print("W:" + w + ", H:" + h);
        // print(normalScale);
        // thisRect.localScale = normalScale;

        //PopRect.localPosition = new Vector3(CanvSize.x, CanvSize.y, 0);//游戏刚开始时隐藏对话框
        //获取玩家属性
        //playC = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayCharacter>();
    }

    // Update is called once per frame
    void LateUpdate()
    {

        if (!displaying) return; //没有弹出就跳过
        if (h_crnt == h_offset) //使弹出的对话框实时定位
        {
            //跟随
            FollowNpc(h_offset, objInView);
        }
    }
    public void FollowNpc(float off_set, bool inView) //气泡跟随说话的目标
    {
        Vector3 ObjPos = sayingObj.position;
        Vector2 pos;
        if (onEdge)//如果需要贴边显示
        {
            if (inView)//如果目标位于视野内，则直接转换
            {
                pos = mainCamera.WorldToScreenPoint(ObjPos + off_set * Vector3.up);
                //print(sayingObj.name + ", Position:" + pos);

                //限制在屏幕内
                if (pos.x < x_min) pos.x = x_min;
                else if (pos.x > x_max) pos.x = x_max;
                   //纵坐标特殊处理
                if (pos.y < y_min) pos = StayOnEdge(ObjPos);
                else if (pos.y > y_max) pos = StayOnEdge(ObjPos);
            }
            else //如果没有位于视野内，则根据目标方向停靠在屏幕边缘，
            {
                pos = StayOnEdge(ObjPos);
            }
        }
        else //无需贴边显示
        {
            if (!inView)
            {
                thisRect.anchoredPosition=new Vector2(0,-100) ;//移到界面外
                return;
            }
            pos = mainCamera.WorldToScreenPoint(ObjPos + off_set * Vector3.up);
        }

        thisRect.anchoredPosition = new Vector2(wR * pos.x, hR * pos.y);
    }

    //根据目标方向停靠在屏幕边缘
    Vector2 StayOnEdge(Vector3 ObjPos)//
    {
        Vector3 playerPos = playC.transform.position;//玩家位置
        //得到方向上距离为1（尽量小）的点，将位置投影到屏幕上
        Vector2 transedPos = mainCamera.WorldToScreenPoint(playerPos + (ObjPos - playerPos).normalized);

        Vector2 pPos = mainCamera.WorldToScreenPoint(playerPos);//转换玩家坐标
        transedPos = transedPos - pPos;//得到屏幕上的坐标差
        float tVec = transedPos.y / transedPos.x;//坐标差值的纵横比
        float tanP = 0;//玩家屏幕位置的横纵坐标比
        Vector2 goalPos;

        //以玩家的屏幕位置为坐标原点，检测屏幕边缘，使气泡停靠
        if (transedPos.x > 0)
        {
            if (transedPos.y > 0)//第一象限
            {
                //正
                tanP = (y_max - pPos.y) / (x_max - pPos.x);

                if (tVec > tanP)
                    //停靠在上边界的右半部分，同时转换坐标
                    goalPos = new Vector2(pPos.x + (y_max - pPos.y) / tVec, y_max);

                else
                    //停靠在右边界的上半部分
                    goalPos = new Vector2(x_max, pPos.y + tVec * (x_max - pPos.x));
            }
            else//第四象限
            {
                //横坐标为正，纵坐标为负
                tanP = (y_min - pPos.y) / (x_max - pPos.x);
                //tanP < 0
                if (tVec < -tanP)
                    goalPos = new Vector2(pPos.x + (y_min - pPos.y) / tVec, y_min);//下右
                else
                    goalPos = new Vector2(x_max, pPos.y + tVec * (x_max - pPos.x));//右下

            }
        }
        else
        {
            if (transedPos.y > 0)//第二象限
            {
                //横负，纵正
                tanP = (y_max - pPos.y) / (x_min - pPos.x);
                //tanP < 0
                if (tVec < tanP)
                    goalPos = new Vector2(pPos.x + (y_max - pPos.y) / tVec, y_max);//上左
                else
                    goalPos = new Vector2(x_min, pPos.y + tVec * (x_min - pPos.x));//左上
            }
            else//第三象限
            {
                //皆负
                tanP = (y_min - pPos.y) / (x_min - pPos.x);
                if (tVec > tanP)
                    goalPos = new Vector2(pPos.x + (y_min - pPos.y) / tVec, y_min); //下左
                else
                    goalPos = new Vector2(x_min, pPos.y + tVec * (x_min - pPos.x));//左下
            }

        }
        return goalPos;
    }
    public void PopupText(GameObject _NPC, string Txt) //弹出气泡
    {
        sayingObj = _NPC.transform;
        talkTxt.text = Txt;
        //播放动画
        StopAllCoroutines();
        StartCoroutine(PopUp());

    }

    public void PopdownText() //缩回气泡
    {
        talkTxt.text = "";
        //PopAnimator.SetBool("Show", false);
        //播放动画
        StopAllCoroutines();
        StartCoroutine(PopDown());

    }

    IEnumerator PopUp() //弹出动画
    {
        displaying = true; //标志已弹出
        //displaying = false; //标志未弹出
        float t = Time.time;
        float dtime = 0;
        while (dtime <= 1)
        {
            dtime = (Time.time - t) / upDuration;
            //动画事件
            //缩放
            thisRect.localScale = Vector2.Lerp(Vector2.zero, Vector2.one, dtime);
            //上移
            h_crnt = Mathf.Lerp(h_offset-3, h_offset, dtime);
            FollowNpc(h_crnt, objInView);

            yield return null;
        }

        yield break;

    }

    IEnumerator PopDown() //收回
    {
        float t = Time.time;
        float dtime = 0;
        while (dtime <= downDuration)
        {
            dtime = (Time.time - t) / downDuration;
            thisRect.localScale = Vector2.Lerp(Vector2.one, Vector2.zero, dtime); //缩放
            h_crnt = Mathf.Lerp(h_offset, h_offset-3, dtime);
            FollowNpc(h_crnt, objInView);
            yield return null;
        }
        displaying = false; //标志未弹出
        yield break;

    }

    public void HidePop() //隐藏气泡
    {
        //播放动画
        StopAllCoroutines();
        StartCoroutine(FadeOut());
        //禁用按钮
        ButtonNormal btn = GetComponent<ButtonNormal>();
        if (btn != null) btn.interactable = false;
    }
    public void ShowPop() //显示气泡
    {
        //播放动画
        StopAllCoroutines();
        StartCoroutine(FadeIn());
        //启用按钮
        ButtonNormal btn = GetComponent<ButtonNormal>();
        if (btn != null) btn.interactable = true;
    }
    public void DestroyPopText()//销毁该对象
    {
        StopAllCoroutines();
        StartCoroutine(FadeOut());
        StartCoroutine(PopDown());
        Destroy(gameObject, downDuration);//销毁该物体
    }

    IEnumerator FadeOut() //渐隐
    {
        //print("h_crnt: "+h_crnt);
        displaying = false; //标志未弹出
        //记录透明度
        thisImg_a = thisImg.color.a;
        talkTxt_a = talkTxt.color.a;

        float t = Time.time;
        float dtime = 0;
        while (dtime <= 1)
        {
            dtime = (Time.time - t) / downDuration;
            //事件
            // thisImg.color = Color.Lerp(thisImg.color,
            //     thisImg.color - new Color(0, 0, 0, thisImg.color.a),
            //     dtime);
            // talkTxt.color = Color.Lerp(talkTxt.color,
            //     talkTxt.color - new Color(0, 0, 0, talkTxt.color.a),
            //     dtime);
            float a0 = Mathf.Lerp(thisImg.color.a, 0, dtime);
            thisImg.color = new Color(thisImg.color.r, thisImg.color.g, thisImg.color.b, a0);

            float a1 = Mathf.Lerp(talkTxt.color.a, 0, dtime);
            talkTxt.color = new Color(talkTxt.color.r, talkTxt.color.g, talkTxt.color.b, a1);

            yield return null;
        }
        yield break;
    }

    IEnumerator FadeIn() //渐显
    {
        //print("h_crnt: "+h_crnt);
        if (h_crnt < h_offset)//如果未完成弹出动画，强制弹出
        {
            h_crnt = h_offset;
            FollowNpc(h_crnt, objInView);//强制弹出
            //thisRect.localScale = Vector2.one;//强制设置缩放量
        }
        float t = Time.time;
        float dtime = 0;
        while (dtime <= 1)
        {
            dtime = (Time.time - t) / upDuration;
            //事件
            //事件
            float a_I = Mathf.Lerp(thisImg.color.a, thisImg_a, dtime);
            thisImg.color =
                new Color(thisImg.color.r, thisImg.color.g, thisImg.color.b, a_I);
            float a_T = Mathf.Lerp(talkTxt.color.a, talkTxt_a, dtime);
            talkTxt.color =
                new Color(talkTxt.color.r, talkTxt.color.g, talkTxt.color.b, a_T);
            // float a = Mathf.Lerp(thisImg.color.a, 0, dtime / downDuration);
            // thisImg.color = new Color(thisImg.color.r, thisImg.color.g, thisImg.color.b, a);
            // talkTxt.color = new Color(talkTxt.color.r, talkTxt.color.g, talkTxt.color.b, a);

            yield return null;
        }
        thisRect.localScale = Vector2.one;
        displaying = true; //标志已弹出
        yield break;
    }

    public void SetPlayer(Transform player) //设置玩家
    {
        playC = player.GetComponent<PlayCharacter>();
    }
    public void PlayerComeHere() //点击动作，玩家走过来
    {
        float sensDis = sayingObj.GetComponent<ChatArea>().SenseDis;
        playC.AutoMoveTo(sayingObj, sensDis);
    }

}
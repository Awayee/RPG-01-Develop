using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
//对话控制
//绑定在对话框上，附带点击事件，无需其他交互组件
public struct RoleChat
{
    public string roleName;//角色名字
    public string headPicture;//角色头像，如果未指定，脚本设定为和名字相同
    public bool dRight;//头像朝向，true为居右朝左，false为居右朝左。
    public string expMethod;// 对话框呈现方式,true为说话，false为思考
    public string TextContent;//说话内容

}
public class ChatControl : MonoBehaviour
{

    private PlayCharacter player;//玩家属性
    [HideInInspector] public GameObject role;//进行对话的角色
    //UI组件
    private ButtonNormal Chat_Button;//对话按钮
    private Image chatRect;//对话横幅
    private Image Chat_Name;//人物名字
    private Text Chat_Text;//说话内容
    private RectTransform Chat_Head;//人物头像
    private TouchControl touch_Control;//触摸
    private GameControl gCtrl;//游戏控制的脚本
    //点击事件
    //private bool interactable = false;//能否点击
    //对话事件
    public RoleChat[] chatItems;//对话内容
    int index;//指向对话内容的索引
    //Vector2 CBx_origionPos;//对话框初始位置 
    //Text Chat_Text;//文本内容

    private ButtonAttack attackButton;//攻击按钮
    public ButtonNormal[] hideWhenEnter;//进入对话区域需要隐藏的按钮
    public RectTransform[] HideWhenChat;//对话时需要隐藏的组件
    private PopText[] popTexts;
    private float a_rect, a_name;//横幅、名字的透明度
    IEnumerator _showTxt;//逐字显示对话内容的协程
    //private ButtonTouch attackButton;//攻击按钮

    //float spd = 5;//弹出速度
    //bool txtEnd;//判断当前文字是否显示完毕

    //Gradient GCColor;//渐变颜色控制
    private void Initialize()//初始化
    {
        player = GameObject.FindObjectOfType<PlayCharacter>();
        if (null == chatRect)
        {
            GameObject _chatRect = Resources.Load<GameObject>("Prefabs/GameUI-ChatRect");//读取
            chatRect = Instantiate(_chatRect).GetComponent<Image>();//实例化，赋值
            chatRect.transform.SetParent(GameManager.Instance.UICanvas, false);//设置为画布的子物体
            chatRect.GetComponent<ButtonNormal>().onClick.AddListener(clickChatRect);//添加横幅的点击事件
            a_rect = chatRect.color.a;
            chatRect.color -= new Color(0, 0, 0, a_rect);//全透明化
        }

        //获取对象
        //Chat_Button = GameObject.Find("Canvas/Buttons/Button_Chat").GetComponent<ButtonNormal>();
        Chat_Name = chatRect.transform.GetChild(0).GetComponent<Image>();
        a_name = Chat_Name.color.a;

        Chat_Text = chatRect.transform.GetChild(1).GetComponent<Text>();
        Chat_Head = chatRect.transform.GetChild(2).GetComponent<RectTransform>();

        touch_Control = GameObject.FindObjectOfType<TouchControl>();
        gCtrl = GameObject.FindObjectOfType<GameControl>();
        attackButton = GameObject.FindObjectOfType<ButtonAttack>();
        Chat_Button = attackButton.transform.parent.Find("ButtonChat").GetComponent<ButtonNormal>();
        Chat_Button.onClick.AddListener(EnterChat);//添加点击事件
        //attackButton = GameObject.FindObjectOfType<ButtonTouch>();
    }

    public void clickChatRect()//点击横幅事件
    {
        //if (!interactable) return;
        //点击事件
        if (Chat_Text.text == chatItems[index - 1].TextContent)
        {//若文字已逐个呈现完毕，则演进对话
            //print("已经显示完毕。");
            PushConversation();
            //chatItems[index - 1].TextContent = null;
        }
        else
        {
            //强制显示剩余文字
            if (_showTxt != null) StopCoroutine(_showTxt);
            Chat_Text.text = chatItems[index - 1].TextContent;
            //chatItems[index - 1].TextContent = "";
        }
    }
    //进入对话
    public void EnterChat()
    {
        //禁用触摸和移动
        //player.canMove = false;
        touch_Control.DisableTouchMove();
        player.Battle2Normal();
        player.TurnAt(role.transform);//玩家面向人物

        //如果此时有窗口弹出，则先关闭窗口
        if (gCtrl.windowed) gCtrl.ClickorEsc();
        //  Player.transform.LookAt(transform);
        //touch_Control.ctrlView = false;

        //属性框和小地图
        //Ins = GameObject.Find("Canvas/Dialog_InsPector").GetComponent<RectTransform>();
        //mMap = GameObject.Find("Canvas/miniMap").GetComponent<RectTransform>();
        //Ins.SetActive(false);
        //mMap.SetActive(false);
        //初始化索引
        index = 0;
        //获取按钮和对话框的原始位置
        //CBx_origionPos = chatRect.anchoredPosition;
        // CBt_origionPos = Chat_Button.anchoredPosition; 

        //隐藏所有对话气泡
        popTexts = GameObject.FindObjectsOfType<PopText>();
        //print("length of popTexts:" + popTexts.Length);
        for (int i = 0; i < popTexts.Length; i++)
        {
            popTexts[i].HidePop();
        }
        //移动横幅到界面内
        chatRect.rectTransform.anchoredPosition = Vector2.zero;
        //Chat_Button.gameObject.SetActive(true);//调整对话按钮的层级
        //隐藏按钮，滑出对话框，同时进行一次对话演进
        StopAllCoroutines();
        StartCoroutine(HideObj());
        PushConversation();

    }

    public void PushConversation()//对话演进
    {

        if (index < chatItems.Length)
        {

            Character_Say(chatItems[index].roleName,
                          chatItems[index].headPicture,
                          chatItems[index].expMethod,
                          chatItems[index].dRight,
                          chatItems[index].TextContent);
            index++;
        }
        else ExitChat();//文本结束    
    }
    public void ExitChat()
    {
        player.canMove = true;
        //touch_Control.EnableTouchControlView();
        touch_Control.EnableTouchMove();
        Chat_Name.GetComponentInChildren<Text>().text = "";
        Chat_Text.text = "";
        StartCoroutine(HideTxtBx(chatItems[chatItems.Length - 1].dRight));//收回横幅对话框
        //StartCoroutine(HideChtHd(chatItems[chatItems.Length - 1].dRight));
        StartCoroutine(ShowObj());//显示对话按钮
        //显示所有对话气泡
        for (int i = 0; i < popTexts.Length; i++)
        {
            if (null == popTexts[i]) continue;
            popTexts[i].GetComponent<PopText>().ShowPop();
        }
        index = 0;
    }
    //人物说话
    //包括角色姓名，角色头像，角色头像的位置朝向，说话内容
    //角色信息根据游戏剧情决定
    #region 说话


    public void Character_Say(string name, string headPic, string method, bool HeadLoc, string sayTxt)
    {
        if (headPic == null) headPic = name;//如果头像图片的路径未指定，则设置为角色名
        //切换头像
        switch (headPic)
        {
            case "不愿透露姓名的主角":
                Chat_Head.GetComponent<Image>().sprite = Resources.Load<Sprite>("HeadPics/c1a_head");
                break;
            case "陆千回":
                Chat_Head.GetComponent<Image>().sprite = Resources.Load<Sprite>("HeadPics/c1a_head");
                break;
            case "雪见":
                Chat_Head.GetComponent<Image>().sprite = Resources.Load<Sprite>("HeadPics/xuejian");
                break;
            case "景天":
                Chat_Head.GetComponent<Image>().sprite = Resources.Load<Sprite>("HeadPics/jingtian");
                break;
            case "龙葵":
                Chat_Head.GetComponent<Image>().sprite = Resources.Load<Sprite>("HeadPics/longkui");
                break;
            case "柳梦璃":
                Chat_Head.GetComponent<Image>().sprite = Resources.Load<Sprite>("HeadPics/mengli");
                break;
            case "韩菱纱":
                Chat_Head.GetComponent<Image>().sprite = Resources.Load<Sprite>("HeadPics/lingsha");
                break;
            case "云天河":
                Chat_Head.GetComponent<Image>().sprite = Resources.Load<Sprite>("HeadPics/tianhe");
                break;
            case "慕容紫英":
                Chat_Head.GetComponent<Image>().sprite = Resources.Load<Sprite>("HeadPics/ziying");
                break;
            case "？":
                Chat_Head.GetComponent<Image>().sprite = Resources.Load<Sprite>("HeadPics/unknown");
                break;
            case "none":
                Chat_Head.GetComponent<Image>().sprite = Resources.Load<Sprite>("HeadPics/none");
                break;
            default:
                Sprite spr = Resources.Load<Sprite>("HeadPics/" + headPic);//按名称读取头像
                if (null == spr)
                {
                    Chat_Head.GetComponent<Image>().sprite = Resources.Load<Sprite>("HeadPics/none");
                }
                else Chat_Head.GetComponent<Image>().sprite = spr;
                break;
        }

        //说或想
        switch (method)
        {
            case "say":
                //Chat_Box.GetComponent<Image>().sprite = 
                break;
            case "think":
                //Chat_Box.GetComponent<Image>().sprite = 
                break;
            default:

                break;
        }
        //StartCoroutine(ShowChtHd(Direction));
        //Chat_Text.text = sayTxt;
        //chatRect.anchoredPosition = CBx_origionPos;
        //StopAllCoroutines();
        _showTxt = ShowTxt(name, sayTxt, 0.1f, HeadLoc);
        StartCoroutine(_showTxt);//弹出对话框


    }
    IEnumerator ShowTxtBx(bool drct)//弹出对话横幅
    {
        float time = Time.time;
        float delTime = 0;
        Chat_Text.text = "";
        Vector2 HdEdPs;
        //头像框起始位置
        if (drct)
        {
            //居右朝左
            Chat_Head.localScale = new Vector2(-1, 1);
            //头像锚点和位置
            Chat_Head.anchoredPosition = (GameManager.Instance.canvasSize.x + Chat_Head.sizeDelta.x) * Vector2.right;
            HdEdPs = Chat_Head.anchoredPosition - Chat_Head.sizeDelta.x * Vector2.right;
            //print("StartPosition: "+Chat_Head.anchoredPosition);
            //Chat_Name.rectTransform.anchoredPosition = new Vector2(50, 50);
            //print("BkgColor Left:" + GCColor.colorLeft + "Right" + GCColor.colorRight);
            //GCColor.colorLeft = new Color(0, 0, 0, 0.392f);
            //GCColor.colorRight = new Color(0, 0, 0, 1);
        }
        else
        {
            //居左朝右
            Chat_Head.localScale = new Vector2(1, 1);
            Chat_Head.anchoredPosition = Chat_Head.sizeDelta.x * Vector2.left;
            //Chat_Name.rectTransform.anchoredPosition = new Vector2(260, 50);
            HdEdPs = Chat_Head.anchoredPosition + Chat_Head.sizeDelta.x * Vector2.right;
            //print("BkgColor Left:" + GetComponent<GradualChangeColor>().colorLeft + "Right" + GetComponent<GradualChangeColor>().colorRight);
            //GCColor.colorRight = new Color(0, 0, 0, 0.392f);
            //GCColor.colorLeft = new Color(0, 0, 0, 1);
        }
        Text Chat_Name_txt = Chat_Name.GetComponentInChildren<Text>();
        Chat_Name_txt.color = new Color(Chat_Name_txt.color.r, Chat_Name_txt.color.g, Chat_Name_txt.color.b, 0);//全透明化
        //float alp = 0;
        //print("Head End Position: " + HdEdPs);
        //print("弹出对话框");
        while (delTime <= 1)
        {
            delTime = (Time.time - time) * 5;
            Chat_Head.anchoredPosition = Vector2.Lerp(Chat_Head.anchoredPosition, HdEdPs, delTime);//显示角色头像
            //显示角色姓名
            Chat_Name.rectTransform.localScale = Vector2.Lerp(Vector2.up, Vector2.one, delTime);
            Chat_Name.color = new Color(Chat_Name.color.r, Chat_Name.color.g, Chat_Name.color.b,
                                        Mathf.Lerp(0, a_name, delTime));//改变透明度
            Chat_Name_txt.color = new Color(Chat_Name_txt.color.r, Chat_Name_txt.color.g, Chat_Name_txt.color.b,
                                        Mathf.Lerp(0, 0.8f, delTime));//改变文字透明度

            //显示对话框
            if (chatRect.color.a != a_rect)
            {
                //chatRect.rectTransform.anchoredPosition = Vector2.Lerp(new Vector2(0, -300), Vector2.zero, delTime);
                chatRect.color = new Color(chatRect.color.r, chatRect.color.g, chatRect.color.b,
                                            Mathf.Lerp(0, a_rect, delTime));
            }

            yield return new WaitForEndOfFrame();
        }
        //StartCoroutine(ShowTxt(txt, 0.1f));//显示文字
        chatRect.GetComponent<ButtonNormal>().interactable = true;//启用点击
        yield break;
    }
    IEnumerator ShowTxt(string Name, string Txt, float duration, bool right)//逐字显示对话内容
    {
        Chat_Text.text = "";
        //说话者姓名
        string beforeName = Chat_Name.GetComponentInChildren<Text>().text;
        //如果说话者与之前不同，则使用切换动画
        if (Name != beforeName)
        {
            Chat_Name.GetComponentInChildren<Text>().text = Name;
            yield return StartCoroutine(ShowTxtBx(right));//等待对话框动画
        }
        string char_text = Txt;
        char_text.ToCharArray();
        foreach (char letter in char_text)
        {
            //if (Chat_Text.text == Txt)break;
            Chat_Text.text += letter;
            yield return new WaitForSeconds(duration);
        }
        yield break;
    }
    IEnumerator HideTxtBx(bool drct)//收回对话横幅
    {

        chatRect.GetComponent<ButtonNormal>().interactable = false;//禁用点击
        float time = Time.time;
        float delTime = 0;
        //头像框初始位置
        Vector2 HdStrtPs;
        if (drct) HdStrtPs = Chat_Head.anchoredPosition + Chat_Head.sizeDelta.x * Vector2.right;
        else HdStrtPs = Chat_Head.anchoredPosition - Chat_Head.sizeDelta.x * Vector2.right;

        Text Chat_Name_txt = Chat_Name.GetComponentInChildren<Text>();
        //float alp = 0;
        //print("收回对话框");
        while (delTime <= 1)
        {
            //print("隐藏文本框");
            delTime = (Time.time - time) * 5;
            Chat_Head.anchoredPosition = Vector2.Lerp(Chat_Head.anchoredPosition, HdStrtPs, delTime);
            //隐藏角色姓名
            //Chat_Name.rectTransform.localScale = Vector2.Lerp(Vector2.one, Vector2.up, delTime);
            Chat_Name.color = new Color(Chat_Name.color.r, Chat_Name.color.g, Chat_Name.color.b,
                                        Mathf.Lerp(Chat_Name.color.a, 0, delTime));//改变透明度
            Chat_Name_txt.color = new Color(Chat_Name_txt.color.r, Chat_Name_txt.color.g, Chat_Name_txt.color.b,
                                        Mathf.Lerp(0.8f, 0, delTime));//改变文字透明度
            //隐藏对话框
            //chatRect.rectTransform.anchoredPosition = Vector2.Lerp(Vector2.zero,new Vector2(0, -300), delTime);
            chatRect.color = new Color(chatRect.color.r, chatRect.color.g, chatRect.color.b,
                                        Mathf.Lerp(a_rect, 0, delTime));
            yield return new WaitForEndOfFrame();
        }

        chatRect.rectTransform.anchoredPosition = new Vector2(0, -300);//移动横幅到界面外
        yield break;

    }
    #endregion

    //按钮转换，显示对话按钮，进入对话区域时调用
    public void ShowChatButton()
    {
        if (null == player) Initialize();//如果没有变量，初始化
        attackButton.HideButton();
        for (int i = 0; i < hideWhenEnter.Length; i++) hideWhenEnter[i].HideButton();
        Chat_Button.ShowButton();

    }

    //隐藏对话按钮
    public void HideChatButton()
    {
        attackButton.ShowButton();
        for (int i = 0; i < hideWhenEnter.Length; i++) hideWhenEnter[i].ShowButton();
        Chat_Button.HideButton(false);
    }
    IEnumerator HideObj()//隐藏按钮组件，动画
    {
        Chat_Button.HideButton();//隐藏对话按钮
        float time = Time.time;
        float delTime = 0;
        while (delTime <= 1)
        {
            //print("正在缩放");
            delTime = (Time.time - time) * 5;
            for (int i = 0; i < HideWhenChat.Length; i++)
            {
                HideWhenChat[i].localScale = Vector2.Lerp(HideWhenChat[i].localScale, Vector2.zero, delTime);
                //print(HideWhenChat[i].localScale);
                //yield return null;
            }
            // print("Deltime: " + delTime);
            //改变透明度
            //float alpha = Mathf.Lerp(1, 0, delTime);
            //ChatButton.GetComponent<Image>().color = new Color(1, 1, 1, alpha);
            yield return null;
        }
        yield break;
    }
    IEnumerator ShowObj()//显示按钮组件
    {
        Chat_Button.ShowButton();//显示对话按钮
        float time = Time.time;
        float delTime = 0;
        while (delTime <= 1)
        {

            delTime = 5 * (Time.time - time);
            for (int i = 0; i < HideWhenChat.Length; i++)
            {
                HideWhenChat[i].localScale = Vector2.Lerp(HideWhenChat[i].localScale, Vector2.one, delTime);
                //yield return null;
            }
            //float alpha = Mathf.Lerp(0, 1, delTime);
            //ChatButton.GetComponent<Image>().color = new Color(1, 1, 1, alpha);
            yield return new WaitForEndOfFrame();
        }
        yield break;
    }
}


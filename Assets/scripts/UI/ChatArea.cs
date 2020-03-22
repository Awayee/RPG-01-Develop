using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//玩家接近角色，进入对话模式，绑定在需要对话的角色上
public class ChatArea : MonoBehaviour {

    //public RectTransform[] SkillButtons;//技能按键
    //private Vector2[] originPos;//技能键初始位置
    Transform Player;//玩家位置
    Transform thisTrans;//该角色位置
    public float SenseDis;//感应距离
    bool entered =false ;//是否已进入区域
    LoadConversation Load_chatTxt;//读取对话文本

    ChatControl chtBtn;//对话按钮
    
	// Use this for initialization
	void Start () {
        //初始化
        Player = GameObject.FindGameObjectWithTag("Player").transform;
        thisTrans = this.transform;
        //Chat_Rec = Button_Chat.GetComponent<RectTransform>();
        Load_chatTxt = GetComponent<LoadConversation>();
        chtBtn = GameObject.FindObjectOfType<ChatControl>();
        //print("ChatButton:" + chtBtn.gameObject.name);
        /*
        //获取技能键的初始位置
        int len = SkillButtons.Length;
        originPos = new Vector2[len];
        for (int i = 0; i < len; i++)  
        {
            originPos[i] = SkillButtons[i].anchoredPosition;
        }
         */
	}
	
	// Update is called once per frame
	void Update () {
		float distance =Vector3.Distance(Player.position,thisTrans.position);

        if (distance <= SenseDis)//进入对话区域，按钮隐藏，同时读取对话
        {
            //print("可以对话了。");
            if (entered) return;
            else
            {
                if (!Load_chatTxt.GotDialogList())
                {
                    Load_chatTxt.GetConversation();
                }
                 
                Load_chatTxt.TransDialog();//将对话内容赋值给对话框
                chtBtn.ShowChatButton();//显示对话按钮
                entered = true;
            }
            
        }
        else
        {
            //print("不再说话了");
            if (!entered) return;
            else
            {
                chtBtn.HideChatButton();//隐藏对话按钮
                entered = false;
            }
            
        } 
	}
    public void StartChat()//直接进入对话
    {
        entered = true;
        chtBtn.EnterChat();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

//读取xml文件，加载对话内容
public class LoadConversation : MonoBehaviour
{

    public string[] conversationText;//对话内容
    public bool useXml = false;//是否使用XML文件，若否，则需要手动输入对话内容，若是，则对话内容无效

    public string dialogDoc;//文件路径

    private XmlDocument dialogue;//xml文件
    public List<RoleChat> dialogues_list;//存放对话列表
    ChatControl chatCtrl;
    //public string _Xml;
    void Start()
    {
        chatCtrl = GameObject.FindObjectOfType<ChatControl>();//获取控制对话的对象
        //if(useXml);
    }

    public void LoadXML()//读取XML
    {
        dialogue = new XmlDocument();
        string _Xml = Resources.Load("Dialogues/" + dialogDoc).ToString();//读取resources下的xml文件
        //print("XML:" + _Xml);
        dialogue.LoadXml(_Xml);//解析xml

    }
    public void GetConversation()//解析XML
    {
        dialogues_list = new List<RoleChat>();//实例化对话列表
        if (useXml)//如果使用XML
        {
            if (dialogue == null) LoadXML();
            //string _xmlPath = Application.dataPath + "/" + role;//文件途径
            //dialogues_list = new List<RoleChat>();
            //实例化
            //_Xml = Resources.Load("Dialogues/" + dialogDoc).ToString();//读取resources下的xml文件
            ///print("path:"+_Xml); 
            XmlNodeList node = dialogue.SelectSingleNode("dialogues").ChildNodes;//遍历节点
            //给列表中的每个元素赋值
            foreach (XmlElement ele in node)
            {
                RoleChat temp = new RoleChat();
                //print("elements:" + ele.Name);
                //赋值
                temp.roleName = ele.ChildNodes[0].InnerText;//姓名
                temp.expMethod = ele.Name;//说话方式
                temp.dRight = XmlConvert.ToBoolean(ele.ChildNodes[1].InnerText);//头像位置，string转boolean
                temp.TextContent = ele.ChildNodes[2].InnerText;
                //头像
                if (ele.ChildNodes.Count >= 4)
                    temp.headPicture = ele.ChildNodes[3].InnerText;
                //print("(Right: " + temp.dRight + ")" + " (" + temp.Method + ")" + temp.roleName + "：" + temp.TextContent);
                dialogues_list.Add(temp);//添加元素
                //TransDialog();
            }
        }
        else //若不使用xml，则获取数组
        {
            //依次读取每一条对话
            for (int i = 0; i < conversationText.Length; i++)
            {
                RoleChat temp = new RoleChat();
                //temp.expMethod = ele.Name;
                temp.roleName = gameObject.name;
                //temp.headPicture = "女村民1";
                temp.TextContent = conversationText[i];
                dialogues_list.Add(temp);
            }
        }


    }
    public bool GotDialogList()//判断DialogList是否为空
    {
        if (dialogues_list == null) return false;
        else return true;
    }
    public void TransDialog()//复制对话列表到UI组件中，顺便赋予玩家变量
    {
        //设置头像位置
        if (!useXml)
        {
            dialogues_list.Clear();
            //获取头像位置
            Vector2 pos = Camera.main.WorldToViewportPoint(this.transform.position);//世界坐标转换为视口坐标
            print("Position: " + pos);
            bool dright;//头像是否靠右
            dright = pos.x >= .5f ? true : false;
            for (int i = 0; i < conversationText.Length; i++)
            {
                RoleChat temp = new RoleChat();
                //temp.expMethod = ele.Name;
                temp.roleName = gameObject.name;
                temp.dRight = dright;
                //temp.headPicture = "女村民1";
                temp.TextContent = conversationText[i];
                dialogues_list.Add(temp);
            }
        }

        chatCtrl.chatItems = dialogues_list.ToArray();//复制列表到对话内容中
        chatCtrl.role = gameObject;
    }
}

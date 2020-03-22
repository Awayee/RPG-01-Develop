using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Xml;
using System.IO;

//配置技能的脚本，用于整理技能

public struct skill
{
    public int ID;//技能唯一索引
    public string name;//技能名称
    public string detail;//技能描述
    public string spritePath;//技能图标
    public float Cd;//技能冷却时间
    public float MpCost;//技能消耗能量值
    public float HpCost;//技能消耗生命值
}
public class SkillSettings : MonoBehaviour
{
    //public SkillDrag selectedSkill;//已选中图标 
    //public bool selected;//已选中

    [SerializeField] private Image img_Icon;
    [SerializeField] private Text txt_Name;
    [SerializeField] private Text txt_Detail;
    [SerializeField] SkillDrag[] allSkillDrags;//所有技能，前六个在界面启用
    // [SerializeField] SkillDrag[] inactiveSkills;
    private SkillCd[] curruntSkills;
    private string path;//用于存储XML文件的路径
    private XmlDocument activeSkillXml;//用于存储激活的技能的xml文件
    private XmlElement activeSkillElement;
    private XmlDocument inactiveSkillXml;//用于存储未激活的技能的xml文件
    private XmlElement inActiveSkillElement;
    List<skill> activeSkillList = new List<skill>();//存储技能数据
    List<skill> inActiveSkillList = new List<skill>();//存储技能数据
    SkillLoad skillload;
    skill[] skills_OnUI;//已启用的技能
    int[] skills_OnUI_Pos;//技能位置信息

    // Use this for initialization
    void Start()
    {
        //初始化XML
        activeSkillXml = new XmlDocument();
        activeSkillElement = activeSkillXml.CreateElement("Active");
        inactiveSkillXml = new XmlDocument();
        inActiveSkillElement = inactiveSkillXml.CreateElement("Inactive");
        //文件存储途径
        path = Application.temporaryCachePath;
        skillload = GameManager.Instance.UICanvas.Find("Buttons/Attack").GetComponent<SkillLoad>();
        skills_OnUI = skillload.skillsonUI;
        skills_OnUI_Pos = skillload.positionIdxs;
        loadUISkills();

        //curruntSkills = GameObject.Find("Canvas/Buttons/Attack").GetComponent<SkillLoad>().skillsOnUI;
        //print("skills: "+curruntSkills.Length);
        
        //LoadSkills();
        ClearDiscribe();
        //selected = false;
        //activeSkills = GetComponentsInChildren<SkillDrag>();
    }
    void loadUISkills()
    {
        for (int i = 0; i < skills_OnUI.Length; i++)//将UI的技能转换到窗口的图标上
        {
            //print(i);
            allSkillDrags[skills_OnUI_Pos[i]].SetThisSkill(skills_OnUI[i]);
        }


        if (File.Exists(path + "/SkillsInavtive.xml"))//如果文件存在
        {
            inactiveSkillXml.Load(path + "/SkillsInavtive.xml");
            //获取未启用的技能
            XmlNodeList node_inactive = inactiveSkillXml.SelectSingleNode("Inactive").ChildNodes;//遍历节点
            if (node_inactive.Count > 0)
            {
                //给列表中的每个元素赋值
                foreach (XmlElement ele in node_inactive)
                {
                    int ip;
                    skill temp = new skill();
                    //print("elements:" + ele.Name);
                    //赋值
                    temp.ID = int.Parse(ele.GetAttribute("ID"));
                    //print(temp.ID);
                    //temp.=int.Parse(ele.ChildNodes[0].InnerText);
                    temp.name = ele.ChildNodes[0].InnerText;//名称
                    temp.Cd = float.Parse(ele.ChildNodes[1].InnerText);//冷却
                    temp.detail = ele.ChildNodes[2].InnerText;//详情
                    temp.spritePath = ele.ChildNodes[3].InnerText;//图标
                    temp.MpCost = float.Parse(ele.ChildNodes[4].InnerText);//能量消耗
                    temp.MpCost = float.Parse(ele.ChildNodes[5].InnerText);//生命消耗

                    ip = int.Parse(ele.ChildNodes[6].InnerText);//位置
                    //print(temp.name)
                    //print("this position: " + ip);
                    allSkillDrags[ip].SetThisSkill(temp);
                }
            }
        }

    }

    public void ApplySkillsSetting()//应用技能配置
    {
        saveSkills();
        skillload.SetSkillIni();
        GameManager.Instance.Tip("应用配置成功！");
    }
    public void ResetSkillSetting()//重置技能配置
    {
         List <skill> allskills = new List<skill>();
        for (int i = 0; i < allSkillDrags.Length; i++)//获取所有已存在的技能
        {
            if (allSkillDrags[i].existSkill)
            {
                allskills.Add(allSkillDrags[i].thisSkill);
                allSkillDrags[i].SetEmpty();
            }

        }
        //重新分配
        for (int i = 0; i < allskills.Count;i++)
        {
            allSkillDrags[allskills[i].ID].SetThisSkill(allskills[i]);
        }
        saveSkills();
        skillload.SetSkillIni();
        GameManager.Instance.Tip("已重置");

    }
    private void saveSkills()//保存技能数据伟XML文档
    {
         inactiveSkillXml.RemoveAll();
        inActiveSkillElement.RemoveAll();
        for (int i = 0; i < allSkillDrags.Length; i++)
        {
            if (i < 6)//前六个应用到界面
            {
                skills_OnUI_Pos[i] = i;
                skills_OnUI[i] = allSkillDrags[i].thisSkill;
            }
            else//后六个存储为XML
            {
                if (allSkillDrags[i].existSkill)//如果有技能
                {
                    skill sk = allSkillDrags[i].thisSkill;
                    SaveInactiveSkillXML(sk.ID, sk.name, sk.Cd, sk.detail, sk.spritePath, sk.MpCost,sk.MpCost, i);
                }
                else SaveInactiveSkillXML(-1, "", 0, "", "", 0, 0, i);//无技能
            }
        }
        inactiveSkillXml.Save(path + "/SkillsInavtive.xml");//保存为启用的技能信息
        skillload.positionIdxs = skills_OnUI_Pos;
        skillload.skillsonUI = skills_OnUI;
    }
    /* 
    public void LoadSkills()//读取技能
    {

        if (!File.Exists(path + "/SkillsActive.xml")) //如果文件不存在，则新建
        {
            //XmlElement skills_active = activeSkillXml.CreateElement("Active");
            print("没有数据");
            for (int i = 0; i < curruntSkills.Length; i++)
            {
                int skillidx = curruntSkills[i].skillIndex;
                print("Skill Index: "+skillidx);
                if (skillidx > 0)
                {
                    string _name, _detail, _sprite;
                    switch (skillidx)
                    {
                        case 1:
                            _name = "移动";
                            //skill_CD.InnerText = "1";
                            _detail = "切换行走或者奔跑。";
                            _sprite = "Move";
                            break;
                        case 2:
                            _name = "翻滚";
                            //skill_CD.InnerText = "4";
                            _detail = "向前方翻滚一段距离";
                            _sprite = "Rush";
                            break;
                        case 3:
                            _name = "跳跃";
                            //skill_CD.InnerText = "4";
                            _detail = "跳跃，状态良好时可以空翻。";
                            _sprite = "Jump";
                            break;
                        case 4:
                            _name = "气功波";
                            //skill_CD.InnerText = "5";
                            _detail = "向前放发射一束波，对命中的第一个敌人造成伤害。";
                            _sprite = "skill4";
                            break;
                        case 5:
                            _name = "如影随形";
                            //skill_CD.InnerText = "6";
                            _detail = "发出几束小型光波，自动追随并打击附近的敌人。";
                            _sprite = "skill5";
                            break;
                        case 6:
                            _name = "妙手";
                            //skill_CD.InnerText = "6";
                            _detail = "治疗自身。";
                            _sprite = "skill6";
                            break;
                        default:
                            _name = "";
                            _detail = "";
                            _sprite = "";
                            break;
                    }
                    SaveActiveSkillXML(skillidx, _name, curruntSkills[i].cdTime, _detail, _sprite, i);


                }
            }
            activeSkillXml.AppendChild(activeSkillElement);
            activeSkillXml.Save(path + "/SkillsActive.xml");
        }
        else activeSkillXml.Load(path + "/SkillsActive.xml");

        if (!File.Exists(path + "/SkillsInavtive.xml"))
        {
            XmlElement skills_inactive = inactiveSkillXml.CreateElement("Inactive");
            inactiveSkillXml.AppendChild(skills_inactive);
            inactiveSkillXml.Save(path + "/SkillsInavtive.xml");
        }
        else inactiveSkillXml.Load(path + "/SkillsInavtive.xml");

        //读取已启用的技能
        XmlNodeList node_active = activeSkillXml.SelectSingleNode("Active").ChildNodes;//遍历节点
        if (node_active.Count > 0)
        {
            //给列表中的每个元素赋值
            foreach (XmlElement ele in node_active)
            {
                int ip;
                skill temp = new skill();
                //print("elements:" + ele.Name);
                //赋值
                temp.ID = int.Parse(ele.GetAttribute("ID"));
                //print(temp.ID);
                //temp.=int.Parse(ele.ChildNodes[0].InnerText);
                temp.name = ele.ChildNodes[0].InnerText;//名称
                temp.skillCd = float.Parse(ele.ChildNodes[1].InnerText);//冷却
                temp.detail = ele.ChildNodes[2].InnerText;//string转boolean
                temp.spritePath = ele.ChildNodes[3].InnerText;

                ip = int.Parse(ele.ChildNodes[4].InnerText);
                //print(temp.name);
                activeSkills[ip].SetThisSkill(temp);
                //print("(Right: " + temp.dRight + ")" + " (" + temp.Method + ")" + temp.roleName + "：" + temp.TextContent);
                activeSkillList.Add(temp);//添加元素
            }
        }

        //获取未启用的技能
        XmlNodeList node_inactive = inactiveSkillXml.SelectSingleNode("Inactive").ChildNodes;//遍历节点
        if (node_inactive.Count > 0)
        {
            //给列表中的每个元素赋值
            foreach (XmlElement ele in node_inactive)
            {
                int ip;
                skill temp = new skill();
                //print("elements:" + ele.Name);
                //赋值
                temp.ID = int.Parse(ele.GetAttribute("ID"));
                //print(temp.ID);
                //temp.=int.Parse(ele.ChildNodes[0].InnerText);
                temp.name = ele.ChildNodes[0].InnerText;//名称
                temp.skillCd = float.Parse(ele.ChildNodes[1].InnerText);//冷却
                temp.detail = ele.ChildNodes[2].InnerText;//string转boolean
                temp.spritePath = ele.ChildNodes[3].InnerText;

                ip = int.Parse(ele.ChildNodes[4].InnerText);
                //print(temp.name)
                inactiveSkills[ip].SetThisSkill(temp);
                //print("(Right: " + temp.dRight + ")" + " (" + temp.Method + ")" + temp.roleName + "：" + temp.TextContent);
                inActiveSkillList.Add(temp);//添加元素
            }
        }

    }
    */
    /* 
    public void ApllySkillSetting()//应用技能配置
    {
        activeSkillElement.RemoveAll();
        inActiveSkillElement.RemoveAll();
        activeSkillXml.RemoveAll();
        inactiveSkillXml.RemoveAll();

        //curruntSkills[i].setSkill(s);//应用到游戏界面
        //存储为XML
        //存储启用的技能信息
        for (int i = 0; i < activeSkills.Length; i++)
        {

            if (activeSkills[i].existSkill)
            {
                skill s = activeSkills[i].thisSkill;
                SaveActiveSkillXML(s.ID, s.name, s.skillCd, s.detail, s.spritePath, i);//保存配置信息
            }
            //curruntSkills[i].setSkill(s);//应用布局到界面

        }
        activeSkillXml.Save(path + "/SkillsActive.xml");


        //处理未启用的技能
        for (int i = 0; i < inactiveSkills.Length; i++)
        {
            if (inactiveSkills[i].existSkill)
            {
                skill s = inactiveSkills[i].thisSkill;
                SaveInactiveSkillXML(s.ID, s.name, s.skillCd, s.detail, s.spritePath, i);
            }

        }
        inactiveSkillXml.Save(path + "/SkillsInavtive.xml");
        GameManager.Instance.Tip("技能配置成功！");
    }
    //存储已启用的技能数据
    private void SaveActiveSkillXML(int ID, string name, float Cd, string detail, string spritePath, int pos)
    {
        XmlElement skill = activeSkillXml.CreateElement("Skill");
        //创建节点
        XmlElement skill_Position = activeSkillXml.CreateElement("Position");
        //XmlElement skill_ID = skillSetXml.CreateElement("ID");
        XmlElement skill_Name = activeSkillXml.CreateElement("Name");
        XmlElement skill_CD = activeSkillXml.CreateElement("CD");
        XmlElement skill_Detial = activeSkillXml.CreateElement("Detail");
        XmlElement skill_Sprite = activeSkillXml.CreateElement("Sprite");
        //处理已启用的技能
        //XmlElement skills = activeSkillXml.CreateElement("Acitve");
        skill.SetAttribute("ID", ID.ToString());//技能ID
        skill_Name.InnerText = name;              //技能名称
        skill_CD.InnerText = Cd.ToString();//技能
        skill_Detial.InnerText = detail;   //描述
        skill_Sprite.InnerText = spritePath;//图标
        skill_Position.InnerText = pos.ToString();//位置
        skill.AppendChild(skill_Name);
        skill.AppendChild(skill_CD);
        skill.AppendChild(skill_Detial);
        skill.AppendChild(skill_Sprite);
        skill.AppendChild(skill_Position);

        activeSkillElement.AppendChild(skill);
        activeSkillXml.AppendChild(activeSkillElement);
    }*/

    //存储未启用的技能
    private void SaveInactiveSkillXML(int ID, string name, float Cd, string detail, string spritePath, float mpcost, float hpcost, int pos)
    {
        XmlElement skill = inactiveSkillXml.CreateElement("Skill");
        //创建节点
        XmlElement skill_Position = inactiveSkillXml.CreateElement("Position");
        //XmlElement skill_ID = skillSetXml.CreateElement("ID");
        XmlElement skill_Name = inactiveSkillXml.CreateElement("Name");
        XmlElement skill_CD = inactiveSkillXml.CreateElement("CD");
        XmlElement skill_Detial = inactiveSkillXml.CreateElement("Detail");
        XmlElement skill_Sprite = inactiveSkillXml.CreateElement("Sprite");
        XmlElement skill_mp = inactiveSkillXml.CreateElement("MpCost");
        XmlElement skill_hp = inactiveSkillXml.CreateElement("HpCost");
        //处理已启用的技能
        //XmlElement skills = activeSkillXml.CreateElement("Acitve");
        skill.SetAttribute("ID", ID.ToString());//技能索引
        skill_Name.InnerText = name;              //技能名称
        skill_CD.InnerText = Cd.ToString();//技能
        skill_Detial.InnerText = detail;   //描述
        skill_Sprite.InnerText = spritePath;//图标
        skill_mp.InnerText = mpcost.ToString();//能量消耗
        skill_hp.InnerText = hpcost.ToString();//生命消耗

        skill_Position.InnerText = pos.ToString();//位置

        skill.AppendChild(skill_Name);
        skill.AppendChild(skill_CD);
        skill.AppendChild(skill_Detial);
        skill.AppendChild(skill_Sprite);
        skill.AppendChild(skill_mp);
        skill.AppendChild(skill_hp);
        skill.AppendChild(skill_Position);

        inActiveSkillElement.AppendChild(skill);
        inactiveSkillXml.AppendChild(inActiveSkillElement);
    }

    public void DescribeSkill(skill s)//将技能信息显示在右边的方框内
    {
        if (!img_Icon.gameObject.activeInHierarchy) img_Icon.gameObject.SetActive(true);
        img_Icon.sprite = Resources.Load<Sprite>("Skills/" + s.spritePath);
        txt_Name.text = s.name;
        //技能消耗
        string skillcost = "";
        if(s.MpCost<=0&&s.HpCost<=0)skillcost = "无消耗";
        else if(s.MpCost>0&& s.HpCost>0)skillcost = "消耗 " + s.MpCost + "能量, " + s.HpCost + "生命";
        else{
            if(s.MpCost>0)skillcost = "消耗 " + s.MpCost + "能量";
            if(s.HpCost>0)skillcost = "消耗 " + s.HpCost + "生命";
        }

        txt_Detail.text = "" + s.detail + "\n\n" +skillcost+ "\n冷却时间: " + s.Cd +" 秒";
}
    public void ClearDiscribe()//清除描述信息
    {
        img_Icon.sprite = null;
        txt_Name.text = "";
        txt_Detail.text = "";
        img_Icon.gameObject.SetActive(false);
    }
    public void SortInactiveSkills()//重新整理
    {
        List<skill> childSkill_id = new List<skill>();
        //List<Sprite> childSkill_sp = new List<Sprite>();
        for (int i = 6; i < allSkillDrags.Length; i++)
        {
            if (allSkillDrags[i].existSkill)//记录非空
            {
                childSkill_id.Add(allSkillDrags[i].thisSkill);
                //inactiveSkills[i].skillID = -1;
                //childSkill_sp.Add(allSkills[i].skillIcon.sprite);
                allSkillDrags[i].SetEmpty();

            }
        }
        for (int i = 0; i < childSkill_id.Count; i++)
        {
            allSkillDrags[i + 6].SetThisSkill(childSkill_id[i]);
            //inactiveSkills[i].thisSkill = childSkill_id[i];
            //inactiveSkills[i].skillIcon.sprite = childSkill_sp[i];
        }


    }

}

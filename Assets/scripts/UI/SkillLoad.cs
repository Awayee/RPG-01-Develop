using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.IO;

public class SkillLoad : MonoBehaviour
{

    [HideInInspector]public SkillCd[] SkillCDs;
    [HideInInspector]public skill[] skillsonUI;
    [HideInInspector]public int[] positionIdxs;
    // Use this for initialization
    void Start()
    {
		SkillCd[] _skillcds = GameObject.FindObjectsOfType<SkillCd>();//获取技能对象
		int len = _skillcds.Length;
        SkillCDs = new SkillCd[len];
		//重新排序后赋值，确保数组索引即为位置索引
        for (int i = 0; i < _skillcds.Length;i++)
		{
            SkillCDs[_skillcds[i].positionIndex] = _skillcds[i];
        }
        // print("skillcd[5].positionindex: " + SkillCDs[5].positionIndex);
        skillsonUI = new skill[len];
        positionIdxs = new int[len];

        GetSkillsInfo();

    }
    void OnEnable()//物体启用时读取技能信息
    {
        //SetSkillIni();
    }
    public void GetSkillsInfo()//获取游戏界面下的技能信息
    {
        //sortSkillCDs();
        for (int i = 0; i < SkillCDs.Length; i++)
        {
            
            int sid = SkillCDs[i].skillIndex;
            skillsonUI[i].ID = sid;//技能ID
			//设置技能参数
            switch (sid)
            {
                case 0:
                    skillsonUI[i].name = "奔走";
                    //skill_CD.InnerText = "1";
                    skillsonUI[i].detail = "切换行走或者奔跑。";
                    skillsonUI[i].spritePath = "Move";
                    break;
                case 1:
                    skillsonUI[i].name = "伏行";
                    //skill_CD.InnerText = "4";
                    skillsonUI[i].detail = "向前方翻滚一段距离。";
                    skillsonUI[i].spritePath = "Rush";
                    break;
                case 2:
                    skillsonUI[i].name = "纵身";
                    //skill_CD.InnerText = "4";
                    skillsonUI[i].detail = "跳跃到空中，状态良好可以空翻。";
                    skillsonUI[i].spritePath = "Jump";
                    break;
                case 3:
                    skillsonUI[i].name = "截流";
                    //skill_CD.InnerText = "5";
                    skillsonUI[i].detail = "向前放发射一道强力气功波，对命中的第一个敌人造成伤害。";
                    skillsonUI[i].spritePath = "skill4";
                    break;
                case 4:
                    skillsonUI[i].name = "逐影";
                    //skill_CD.InnerText = "6";
                    skillsonUI[i].detail = "散发出几束小型气功波，自动追随并打击附近的敌人。";
                    skillsonUI[i].spritePath = "skill5";
                    break;
                case 5:
                    skillsonUI[i].name = "回春";
                    //skill_CD.InnerText = "6";
                    skillsonUI[i].detail = "治疗自身，获得一定量生命值和能量值。";
                    skillsonUI[i].spritePath = "skill6";
                    break;
                default:
                    skillsonUI[i].name = "";
                    skillsonUI[i].detail = "";
                    skillsonUI[i].spritePath = "";
                    break;
            }
            skillsonUI[i].MpCost = SkillCDs[i].mpCost;//技能能量消耗
            skillsonUI[i].HpCost = SkillCDs[i].hpCost;//技能生命消耗
            skillsonUI[i].Cd = SkillCDs[i].cdTime;//技能冷却时间
            positionIdxs[i] = SkillCDs[i].positionIndex;//技能位置
        }
    }
	//重新排序
	private void sortSkillCDs()
	{
        for (int i = 0; i < SkillCDs.Length;i++)
		{
            for (int j = 0; j < SkillCDs.Length;j++)
			{
                SkillCd temp;
                if(SkillCDs[j].positionIndex > SkillCDs[i].positionIndex)
				{
                    temp = SkillCDs[i];
                    SkillCDs[i] = SkillCDs[j];
                    SkillCDs[j] = temp;
                }
            }
        }

    }
	public void SetSkillIni()//配置技能信息
	{
        for (int i = 0; i < skillsonUI.Length;i++)
		{
            SkillCDs[positionIdxs[i]].setSkill(skillsonUI[i]);
            //print("position[" + i + "]" + ": " + positionIdxs[i]+", skill; "+skillsonUI[i].ID);
        }

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class SkillCd : MonoBehaviour
{
    private Vector2 defaultPos;//默认位置
    public int positionIndex;//位置索引
    public int skillIndex;//技能索引
    public float cdTime = 1;//冷却时间
    public float mpCost = 0;//技能消耗能量
    public float hpCost = 0;//技能消耗生命

    private Text timeTip;//时间提示
    private string ButtonText;//初始按钮文字
    //private Image CdImg;//用于可视化CD时间的图片
    private Image CdProgress;//用于显示进度
    private Color normalColor;//正常颜色
    [SerializeField] private Color DisabledColor = Color.gray;//禁用时的颜色
    //private Color colorStep;//颜色变化单位
    public bool enable;//是否启用
    private float CurrentTime;//当前时刻
    public UnityEvent onSkill = new UnityEvent();//触发的技能
    [SerializeField] private Image skillIcon;//技能图标
    private ButtonNormal btn;//按钮组件
    private PlayerSkills playerSkill;
    private PlayCharacter playC;
    // Use this for initialization
    void Awake()
    {
        defaultPos = transform.localPosition;//记录初始位置
        btn = GetComponent<ButtonNormal>();
        //如果没有按钮组件，则图片为自身，反之则为子物体
        //if (null == btn) skillIcon = GetComponent<Image>();
        if (null == skillIcon) skillIcon = transform.GetChild(0).GetComponent<Image>();
        normalColor = skillIcon.color;//记录颜色
        //CdImg = transform.Find("CdImg").GetComponent<Image>();
        CdProgress = transform.Find("CdProgress").GetComponent<Image>();
        if (null == CdProgress) CdProgress = GetComponent<Image>();//如果没有子物体，选择自身
        //normalColor = CdImg.color;
        timeTip = this.GetComponentInChildren<Text>();
        //if(!enable)ResetCD();
        //else EnableSkill();
        //colorStep = (normalColor - DisabledColor) * Time.deltaTime / cdTime;

    }
    void Start()
    {
        playC = GameObject.FindObjectOfType<PlayCharacter>();
        playerSkill = playC.GetComponent<PlayerSkills>();
        setSkillwithIndex(skillIndex);
        //print("Player: " + playerSkill.name);
        //onSkill = new UnityEvent();
        if (!enable) ResetCD();
        else EnableSkill();
    }

    // Update is called once per frame
    void Update()
    {
        if (enable) return;//如果冷却完毕，则不执行
        CurrentTime += Time.deltaTime;
        timeTip.text = (cdTime - (int)CurrentTime).ToString();//显示技能剩余时间
        //thisImage.fillAmount = CurrentTime / cdTime;//图标填充效果
        //CdImg.color += colorStep;//颜色变化效果
        CdProgress.fillAmount = CurrentTime / cdTime;
        if (CurrentTime >= cdTime)
        {
            EnableSkill();
            CurrentTime = 0;
            timeTip.text = ButtonText;
            return;
        }
    }

    public void EnableSkill()//激活技能
    {
        enable = true;
        skillIcon.color = normalColor;
        if (btn) btn.EnableButton();//如果有按钮组件，启用
    }
    public void RealeaseSkill()//释放技能
    {
        if (playC)
            if (enable)
                onSkill.Invoke();
    }
    public void ResetCD()//释放技能，重新进入冷却
    {
        enable = false;
        skillIcon.color = DisabledColor;
        if (btn) btn.DisableButton();//如果有按钮组件，禁用
    }

    public bool Prepared()//冷却完毕
    {
        return enable;
    }
    public void HideSkill()//暂时隐藏技能图标（轻触攻击键）
    {
        transform.localScale = Vector3.zero;
    }
    public void ShowSkill()//显示技能图标
    {
        transform.localScale = Vector3.one;
    }
    public void setSkill(skill sk)//配置技能
    {
        if (skillIndex == 0)//如果原先为移动模式切换按钮，则移除组件
        {
            Destroy(skillIcon.GetComponent<SwitchSprite>());
        }
        else if (skillIndex == 2)//如果原先为跳跃按钮，则移除双重CD的组件
        {
            Destroy(this.GetComponent<SkillDoubleCd>());

        }
        skillIndex = sk.ID;
        //print("Skill Index: " + skillIndex);
        cdTime = sk.Cd;
        mpCost = sk.MpCost;
        hpCost = sk.HpCost;
        skillIcon.sprite = Resources.Load<Sprite>("Skills/" + sk.spritePath);
        setSkillwithIndex(skillIndex);
    }
    public void setSkillwithIndex(int idx)//按索引配置技能
    {
        //gameObject.SetActive(true);
        onSkill.RemoveAllListeners();
        if (idx >= 0)
        {
            transform.localPosition = defaultPos;//移动到界面内
            switch (idx)
            {
                case 0://切换行走模式
                    SwitchSprite ss = skillIcon.gameObject.AddComponent<SwitchSprite>();
                    ss.EnabledSprite = Resources.Load<Sprite>("Skills/Run");
                    playC.ChangeMoveButton = ss;
                    onSkill.AddListener(playC.ChangeMoveMode);

                    break;
                case 1:
                    playC.rushButton = this;
                    onSkill.AddListener(playC.Rush);
                    break;
                case 2://跳跃
                    SkillDoubleCd sdcd = this.gameObject.AddComponent<SkillDoubleCd>();
                    sdcd.skillIcon = skillIcon;
                    //设置图标
                    sdcd.buttonSprite1 = skillIcon.sprite;
                    sdcd.buttonSprite2 = Resources.Load<Sprite>("Skills/Jump2");
                    sdcd.CdTime1 = cdTime;
                    sdcd.CdTime2 = cdTime - 1;
                    sdcd.Color2 = normalColor;
                    playC.jumpButton = sdcd;
                    onSkill.AddListener(playC.Jump);
                    break;
                case 3:
                    onSkill.AddListener(playerSkill.Skill2);
                    break;
                case 4:
                    playerSkill.skill3cd = this;
                    onSkill.AddListener(playerSkill.playeruseskill3);
                    break;
                case 5:
                    onSkill.AddListener(playerSkill.GetEnergyAndLife);
                    break;
                default:
                    break;
            }
        }
        else //移动到界面外
            transform.localPosition = new Vector2(200, 0);
    }

}

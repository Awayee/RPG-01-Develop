using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

//可拖拽的技能图标
public class SkillDrag : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IDragHandler
{
    public bool isActive = true;//技能是否已配置
    //public int skillID;//技能唯一索引
    public skill thisSkill;//当前技能
    private float longPressTime = 0.5f;//长按时间
    private float pressedStartTime = 0;//按下开始的时刻
    private bool pressed;//是否已按下
    //private bool entered;//是否在图标内
    private bool longPressed;//是否长按
    private Image thisImg;
    private RectTransform thisRect;
    public Image skillIcon;
    [HideInInspector] public bool existSkill;//是否已安排了技能 
    private Color normalColor;
    public Vector2 normalPos;//初始位置
    private float wr, hr;//坐标缩放
    private SkillSettings skillsSetting;
    GraphicRaycaster gr;
    // Use this for initialization
    void Awake()
    {
        thisImg = GetComponent<Image>();
        normalColor = thisImg.color;//记录此时的颜色
        skillIcon = transform.Find("Icon").GetComponent<Image>();
        
        // print("activeSelf: "+skillIcon.gameObject.activeSelf);
        // print("activeInHierarchy: "+skillIcon.gameObject.activeInHierarchy);
        //existSkill = skillIcon.gameObject.activeInHierarchy;
        if (!isActive) thisImg.color -= new Color(0, 0, 0, thisImg.color.a);//将未启用且空的技能图标透明化 

        thisRect = thisImg.rectTransform;
        normalPos = thisRect.anchoredPosition;
        Transform canvas = GameManager.Instance.UICanvas;
        wr = GameManager.Instance.wRatio;
        hr = GameManager.Instance.hRatio;
        gr = canvas.GetComponent<GraphicRaycaster>();

        skillsSetting = GameObject.FindObjectOfType<SkillSettings>();
    }

    // Update is called once per frame
    void Update()
    {
        if (pressed && !longPressed)
        {
            if (!existSkill) return;
            if (Time.unscaledTime - pressedStartTime >= longPressTime)
            {
                //长按事件
                longPressed = true;
                thisImg.raycastTarget = false;//禁用射线检测
                transform.SetAsLastSibling();//调整层级
                StartCoroutine(LongPress());
                return;
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(!existSkill)return;
        pressed = true;
        //entered = true;
        thisImg.color = Color.gray;//图标变灰
        pressedStartTime = Time.unscaledTime;//记录时刻
        skillsSetting.DescribeSkill(thisSkill);//显示此技能详情
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (longPressed) return;
        if (pressed) pressed = false;
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (longPressed)
        {
            Vector2 inputPos = Input.mousePosition;
            Vector2 transedPos = new Vector2(inputPos.x * wr, inputPos.y * hr);
            SkillDrag sdrag = CheckUIRaycastObjects(eventData).GetComponent<SkillDrag>();//获取射线得到的目标
            //SkillSetting sSetting = rayed.GetComponent<SkillSetting>();
            //if(rayed.GetComponent<SkillSetting>()) sSetting = rayed.GetComponent<SkillSetting>();
            if (null != sdrag)
            {
                thisRect.anchoredPosition = sdrag.normalPos;
            }
            else thisRect.anchoredPosition = transedPos;
        }

    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if(!existSkill)return;
        pressed = false;
        thisImg.color = normalColor;
        if (longPressed)
        {
            longPressed = false;
            SkillDrag sdrag = CheckUIRaycastObjects(eventData).GetComponent<SkillDrag>();//获取射线得到的目标
            if (null != sdrag)
            {
                sdrag.SetSkill(this);
            }
            thisRect.anchoredPosition = normalPos;
        }
        thisImg.raycastTarget = true;//启用射线检测
        //设定父物体
        StopAllCoroutines();
        StartCoroutine(Release());
    }
    //从触摸点发射一条射线检测物体
    private GameObject CheckUIRaycastObjects(PointerEventData eventData)
    {
        List<RaycastResult> RaycastList = new List<RaycastResult>();
        gr.Raycast(eventData, RaycastList);
        //print(RaycastList.Count);
        //SkillDrag target = RaycastList[1].gameObject.GetComponent<SkillDrag>();//得到第二个物体
        //print(target.name);
        if (RaycastList.Count >= 1) return RaycastList[0].gameObject;//得到第二个物体
        else return null;
    }

    //设置技能
    public void SetSkill(SkillDrag sd)
    {
        if (existSkill)
        {
            //交换图标
            Sprite tempSprite = skillIcon.sprite;
            skillIcon.sprite = sd.skillIcon.sprite;
            sd.skillIcon.sprite = tempSprite;
            //交换技能
            skill tempSkill = this.thisSkill;
            this.thisSkill = sd.thisSkill;
            sd.thisSkill = tempSkill;


        }
        else//如果该图标为空 
        {
            SetNotEmpty(sd.skillIcon.sprite,sd.thisSkill);//设为非空
            thisSkill = sd.thisSkill;
            skillIcon.sprite = sd.skillIcon.sprite;//设置图标
            //sd.skillIcon.color = new Color(sd.skillIcon.color.r, sd.skillIcon.color.g, sd.skillIcon.color.b, 0);
            //sd.thisSkill = null;
            sd.SetEmpty();//将对方置空
        }

        //如果拖到下方区域，重新整理
        if ((!isActive && !sd.existSkill) || (!sd.isActive && !sd.existSkill) ) skillsSetting.SortInactiveSkills();
    }

    public void SetEmpty()//技能置空
    {
        existSkill = false;
        skillIcon.sprite = null;
        thisSkill.ID =-1;
        skillIcon.gameObject.SetActive(false);
        if (!isActive) thisImg.color = new Color(thisImg.color.r, thisImg.color.g, thisImg.color.b, 0);//透明化
    }
    public void SetNotEmpty(Sprite sp, skill sk)//设为非空
    {
        existSkill = true;
        if (!isActive) thisImg.color = new Color(thisImg.color.r, thisImg.color.g, thisImg.color.b, 1);
        skillIcon.gameObject.SetActive(true);//激活子物体
        thisSkill = sk;
        skillIcon.sprite = sp;//设置图标

    }
    public void SetThisSkill(skill s)//设置当前技能图标
    {
        //print(s.spritePath);
        if (!isActive) thisImg.color = new Color(thisImg.color.r, thisImg.color.g, thisImg.color.b, 1);
        if (!existSkill) existSkill = true;
        print("SkillIcon: " + skillIcon.gameObject.activeSelf);
        if (!skillIcon.gameObject.activeSelf) skillIcon.gameObject.SetActive(true);//激活子物体
        thisSkill = s;
        if(s.ID<0)//如果技能索引小于零，置空
        {
            SetEmpty();
            return;
        }
        Sprite sp = Resources.Load<Sprite>("sSkills/" + s.spritePath);
        skillIcon.gameObject.SetActive(true);
        skillIcon.sprite = Resources.Load<Sprite>("Skills/" + s.spritePath);
    }
    IEnumerator LongPress()//动画，长按
    {
        //skillIcon.color = Color.gray;//图标变灰
        //thisImg.color = Color.gray;//图标变灰
        float t = Time.unscaledTime;
        float dt = 0;
        while (dt <= 1)
        {
            dt = (Time.unscaledTime - t) * 2f;
            //图标放大
            thisRect.localScale = Vector2.Lerp(thisRect.localScale, new Vector2(1.2f, 1.2f), dt);
            yield return null;
        }
    }
    IEnumerator Release()//松开
    {
        float t = Time.unscaledTime;
        float dt = 0;
        while (dt <= 1)
        {
            dt = (Time.unscaledTime - t) * 2f;
            //图标还原
            thisRect.localScale = Vector2.Lerp(thisRect.localScale, Vector2.one, dt);
            //thisImg.color = Color.Lerp(thisImg.color, normalColor, dt);
            yield return null;
        }
    }
}

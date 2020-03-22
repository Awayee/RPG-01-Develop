using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ButtonAttack : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, IDragHandler
{
    private bool hided;//是否已隐藏
    private bool pressed = false; //按下判断
    private bool entered = false;//进入按钮区域判断
    private Image thisImg;
    private Color normalColor;//正常颜色
    public Color pressedColor = Color.gray;//按下颜色
    private Vector2 normalSize;//正常尺寸
    private Vector2 pressedSize = new Vector2(160, 160);//按下尺寸
    //public Color disabledColor;//按钮禁用时的颜色
    private Vector2 beginPos;//拖拽开始点坐标
    private CanvasGroup thisCG;//按钮的Canvas Group组件
    private int dragDir = 0;//拖拽方向判别，1-3
    //public float minD = 10;//触发拖拽效果最小值
    private float btnRadiu, maxD = 30;//按钮半径、拖动最大距离
    private float wr, hr;//坐标转换

    //private ButtonNormal thisButton;//按钮组件，用于判断是否可交互
    //private bool up, down, left, right = false;//各向技能是否能释放

    public Image attack, skill3Icon, skill4Icon, skill5Icon;//攻击和三个技能的图标，由下到上

    public SkillCd skill3, skill4, skill5;//技能提示
    public Image cd;//跟随触摸点的图标
    private Vector2 pos1, pos2, pos3;//拖动技能图标的最终位置
    public UnityEvent onAttack;//点击事件，攻击

    void Start()
    {
        thisImg = GetComponent<Image>();
        //attack = GetComponent<Image>();
        thisCG = GetComponent<CanvasGroup>();
        //获取初始的按钮状态
        normalColor = thisImg.color;
        normalSize = thisImg.rectTransform.sizeDelta;
        wr = GameManager.Instance.wRatio;
        hr = GameManager.Instance.hRatio;
        //获取位置
        pos1 = new Vector2(-200, -25);
        maxD = pos1.magnitude;
        pos2 = maxD * (new Vector2(-1, 1).normalized);
        pos3 = new Vector2(25, 200);

        //btnRadiu = 0.5f * thisImg.rectTransform.sizeDelta.x;
        btnRadiu = 0.5f * pressedSize.x;
        beginPos = new Vector2(1152, 128);
    }
    public void OnPointerDown(PointerEventData eventData)//按下按钮
    {
        if (hided) return;
        pressed = true;
        entered = true;

        StopAllCoroutines();
        dragDir = 0;
        thisImg.color = pressedColor;
        thisImg.rectTransform.sizeDelta = pressedSize;
        attack.color = new Color(attack.color.r, attack.color.g, attack.color.b, 1);
        attack.rectTransform.localScale = Vector2.one;
        //beginPos = Input.mousePosition;
    }
    public void OnPointerEnter(PointerEventData eventData) //进入按钮区域
    {
        if (hided) return;
        if (pressed)
        {
            entered = true;
        }
    }
    public void OnPointerExit(PointerEventData eventData) //退出按钮区域
    {
        if (hided) return;
        if (pressed)
        {
            entered = false;
        }
    }
    public void OnDrag(PointerEventData eventData)//拖拽按钮
    {
        //if (hided) return;
        Vector2 _nowpos = Input.mousePosition;
        Vector2 dvec = Vector2.Scale(_nowpos, new Vector2(wr, hr));//坐标转换
        dvec -= beginPos;
        //限制在第三象限
        if (dvec.x > 0) dvec.x = 0;
        else if (dvec.y < 0) dvec.y = 0;
        //print("Dvec: " + dvec);
        //dvec -= new Vector2(512, -232);

        float a = dvec.magnitude / (0.25f * pressedSize.magnitude);
        //print("a: " + a);
        HideSkills();//隐藏技能图标
        attack.color = new Color(attack.color.r, attack.color.g, attack.color.b, Mathf.Lerp(1, 0, a));//攻击图标透明度
        thisImg.rectTransform.sizeDelta = Vector2.Lerp(pressedSize, 1.2f * pressedSize, a);//按钮尺寸
        if (entered)//在按钮区域内
        {
            cd.color = new Color(cd.color.r, cd.color.g, cd.color.b, 0);//跟随点透明化
            if (dragDir == 1) skill3Icon.color = new Color(skill3Icon.color.r, skill3Icon.color.g, skill3Icon.color.b, 0);//技能图标透明化
            else if (dragDir == 2) skill4Icon.color = new Color(skill4Icon.color.r, skill4Icon.color.g, skill4Icon.color.b, 0);//技能图标透明化
            else if (dragDir == 3) skill5Icon.color = new Color(skill5Icon.color.r, skill5Icon.color.g, skill5Icon.color.b, 0);//技能图标透明化
            dragDir = 0;
        }
        else//否则
        {
            //print("dX:" + deltaX + ", dY:" + deltaY);
            dragDir = DrawSkills(dvec);
        }
    }

    public void OnPointerUp(PointerEventData eventData)//松开按钮
    {
        //if (hided) return;
        pressed = false;


        thisImg.color = normalColor;
        //beginPos = Vector2.zero;
        //print("DragDirection: " + dragDir);

        Release();

    }
    float VectorAngle(Vector2 from, Vector2 to)//计算向量夹角(得到-180度到180度的角）
    {
        Vector3 cross = Vector3.Cross(from, to);
        float angle = Vector2.Angle(from, to);
        return cross.z > 0 ? -angle : angle;
    }
    public int DrawSkills(Vector2 dVector) //拖拽时滑出技能图标
    {
        //print("Draged");
        float dis = dVector.magnitude;
        //print("di: " + dis);
        //范围调整
        if (dis > maxD)
        {
            dis = maxD;
            dVector = maxD * dVector.normalized;
        }
        float angle = VectorAngle(Vector2.left, dVector);
        float d = (dis - btnRadiu) / (maxD - btnRadiu);
        
        //print(d);
        //技能1
        if (angle >= -30 && angle < 30)
        {
            if(skill3.skillIndex<0) return 0;
            if (!skill3.Prepared())
            {
                skill3.ShowSkill();
                cd.color = new Color(0, 0, 0, 0);
                skill3Icon.color = new Color(skill3Icon.color.r, skill3Icon.color.g, skill3Icon.color.b, 0);//技能图标透明度
                skill4Icon.color = new Color(skill4Icon.color.r, skill4Icon.color.g, skill4Icon.color.b, 0);//技能图标透明度
                skill5Icon.color = new Color(skill5Icon.color.r, skill5Icon.color.g, skill5Icon.color.b, 0);//技能图标透明度
                return 0;
            }
            //改变技能图标透明度
            skill3Icon.color = new Color(skill3Icon.color.r, skill3Icon.color.g, skill3Icon.color.b, Mathf.Lerp(0, 1, d));//技能图标透明度
            skill4Icon.color = new Color(skill4Icon.color.r, skill4Icon.color.g, skill4Icon.color.b, 0);//技能图标透明度
            skill5Icon.color = new Color(skill5Icon.color.r, skill5Icon.color.g, skill5Icon.color.b, 0);//技能图标透明度
            //cd标志
            cd.color = new Color(0, 0, 0, Mathf.Lerp(0.5f, 1, d));
            cd.rectTransform.localPosition = Vector2.Lerp(Vector2.zero, pos1, dis / 200);
            cd.rectTransform.localScale = Vector2.Lerp(Vector2.one, 2.5f * Vector2.one, d);
            thisImg.color = Color.Lerp(pressedColor, normalColor, d);//按钮透透明度
            return 1;
        }
        //技能2
        else if (angle >= 30 && angle < 60)
        {
            if(skill4.skillIndex<0) return 0;
            if (!skill4.Prepared())
            {
                skill4.ShowSkill();
                cd.color = new Color(0, 0, 0, 0);
                skill3Icon.color = new Color(skill3Icon.color.r, skill3Icon.color.g, skill3Icon.color.b, 0);//技能图标透明度
                skill4Icon.color = new Color(skill4Icon.color.r, skill4Icon.color.g, skill4Icon.color.b, 0);//技能图标透明度
                skill5Icon.color = new Color(skill5Icon.color.r, skill5Icon.color.g, skill5Icon.color.b, 0);//技能图标透明度
                return 0;
            }
            //改变技能图标透明度
            skill4Icon.color = new Color(skill4Icon.color.r, skill4Icon.color.g, skill4Icon.color.b, Mathf.Lerp(0, 1, d));//技能图标透明度
            skill3Icon.color = new Color(skill3Icon.color.r, skill3Icon.color.g, skill3Icon.color.b, 0);//技能图标透明度
            skill5Icon.color = new Color(skill5Icon.color.r, skill5Icon.color.g, skill5Icon.color.b, 0);//技能图标透明度
            //按钮cd
            cd.color = new Color(0, 0, 0, Mathf.Lerp(0.5f, 1, d));
            cd.rectTransform.localPosition = Vector2.Lerp(Vector2.zero, pos2, dis / 200);
            cd.rectTransform.localScale = Vector2.Lerp(Vector2.one, 2.5f * Vector2.one, d);
            thisImg.color = Color.Lerp(pressedColor, normalColor, d);//按钮透透明度
            return 2;
        }
        //技能3
        else if (angle >= 60 && angle < 120)
        {
            if(skill5.skillIndex<0) return 0;
            if (!skill5.Prepared())
            {
                skill5.ShowSkill();
                cd.color = new Color(0, 0, 0, 0);
                skill3Icon.color = new Color(skill3Icon.color.r, skill3Icon.color.g, skill3Icon.color.b, 0);//技能图标透明度
                skill4Icon.color = new Color(skill4Icon.color.r, skill4Icon.color.g, skill4Icon.color.b, 0);//技能图标透明度
                skill5Icon.color = new Color(skill5Icon.color.r, skill5Icon.color.g, skill5Icon.color.b, 0);//技能图标透明度
                return 0;
            }
            //改变技能图标透明度
            skill5Icon.color = new Color(skill5Icon.color.r, skill5Icon.color.g, skill5Icon.color.b, Mathf.Lerp(0, 1, d));//技能图标透明度
            skill3Icon.color = new Color(skill3Icon.color.r, skill3Icon.color.g, skill3Icon.color.b, 0);//技能图标透明度
            skill4Icon.color = new Color(skill4Icon.color.r, skill4Icon.color.g, skill4Icon.color.b, 0);//技能图标透明度
            //按钮cd
            cd.color = new Color(0, 0, 0, Mathf.Lerp(0.5f, 1, d));
            cd.rectTransform.localPosition = Vector2.Lerp(Vector2.zero, pos3, dis / 200);
            cd.rectTransform.localScale = Vector2.Lerp(Vector2.one, 2.5f * Vector2.one, d);
            thisImg.color = Color.Lerp(pressedColor, normalColor, d);//按钮透透明度

            return 3;
        }
        else return 0;
    }
    //释放技能
    public void Release()
    {

        ShowSkills();
        StartCoroutine(EndRelease(dragDir));
        if (entered)
        {
            onAttack.Invoke();
            entered = false;
        }
        else
        {
            switch (dragDir)
            {
                case 1:
                    if (skill3.skillIndex > 0)
                    {
                        skill3.RealeaseSkill();
                        skill3.ResetCD();
                    }
                    //StartCoroutine(EndRelease(upButton));
                    break;
                case 2:
                    if (skill4.skillIndex > 0)
                    {
                        skill4.RealeaseSkill();
                        skill4.ResetCD();
                    }
                    //StartCoroutine(EndRelease(downButton));
                    break;
                case 3:
                    if (skill5.skillIndex > 0)
                    {
                        skill5.RealeaseSkill();
                        skill5.ResetCD();
                    }
                    //StartCoroutine(EndRelease(leftButton));
                    break;
            }
        }


    }
    void HideSkills()//隐藏技能图标
    {
        skill3.HideSkill();
        skill4.HideSkill();
        skill5.HideSkill();
    }
    void ShowSkills()//显示技能图标
    {
        skill3.ShowSkill();
        skill4.ShowSkill();
        skill5.ShowSkill();
    }
    //松开按钮后的动态反馈
    IEnumerator EndRelease(int skill)
    {
        float time = Time.time;
        float dTime = 0;
        //float s = rctT.localScale.x;
        // attack.localScale = Vector2.zero;
        while (dTime < 1)
        {
            dTime = (Time.time - time) * 4f;

            if (skill == 1)
            {
                skill3Icon.color = new Color(skill3Icon.color.r, skill3Icon.color.g, skill3Icon.color.b,
                                    Mathf.Lerp(skill3Icon.color.a, 0, dTime));
                //skill1.rectTransform.localScale = Vector2.Lerp(skill1.rectTransform.localScale, Vector2.one * 1.5f, dTime);
                //按钮图标
                cd.color = new Color(cd.color.r, cd.color.g, cd.color.b, Mathf.Lerp(1, 0, dTime));
                cd.rectTransform.localScale = Vector2.Lerp(2.5f * Vector2.one, 3 * Vector2.one, dTime);
            }
            else if (skill == 2)
            {
                skill4Icon.color = new Color(skill4Icon.color.r, skill4Icon.color.g, skill4Icon.color.b,
                                                    Mathf.Lerp(skill4Icon.color.a, 0, dTime));
                //skill2.rectTransform.localScale = Vector2.Lerp(skill2.rectTransform.localScale, Vector2.one * 1.5f, dTime);
                //按钮图标
                cd.color = new Color(cd.color.r, cd.color.g, cd.color.b, Mathf.Lerp(1, 0, dTime));
                cd.rectTransform.localScale = Vector2.Lerp(2.5f * Vector2.one, 3 * Vector2.one, dTime);
            }
            else if (skill == 3)
            {
                skill5Icon.color = new Color(skill5Icon.color.r, skill5Icon.color.g, skill5Icon.color.b,
                                    Mathf.Lerp(skill5Icon.color.a, 0, dTime));
                //skill3.rectTransform.localScale = Vector2.Lerp(skill3.rectTransform.localScale, Vector2.one * 1.5f, dTime);
                //按钮图标
                cd.color = new Color(cd.color.r, cd.color.g, cd.color.b, Mathf.Lerp(1, 0, dTime));
                cd.rectTransform.localScale = Vector2.Lerp(2.5f * Vector2.one, 3 * Vector2.one, dTime);
            }

            //还原按钮背景
            thisImg.rectTransform.sizeDelta = Vector2.Lerp(pressedSize, normalSize, dTime);
            thisImg.color = Color.Lerp(pressedColor, normalColor, dTime);
            yield return new WaitForEndOfFrame();
        }
        cd.rectTransform.localPosition = Vector2.zero;
        cd.rectTransform.localScale = Vector2.one;
        ShowSkills();//显示技能图标
        attack.color = new Color(attack.color.r, attack.color.g, attack.color.b, 1);
        attack.rectTransform.localPosition = Vector2.zero;
        attack.rectTransform.localScale = Vector2.one;
    }
    #region 按钮隐藏和显示
    public void HideButton()//隐藏按钮
    {
        hided = true;
        StartCoroutine(Button_Hide());
    }
    public void ShowButton()//显示按钮 
    {
        StartCoroutine(Button_Show());
    }
    IEnumerator Button_Hide()
    {
        float time = Time.unscaledTime;
        float dTime = 0;
        while (dTime <= 1)
        {
            dTime = (Time.unscaledTime - time) * 4;
            thisCG.alpha = Mathf.Lerp(1, 0, dTime);//按钮渐隐，匀速
            //thisImg.rectTransform.localScale = Vector2.Lerp(thisImg.rectTransform.localScale, Vector2.zero, dTime); //（改变缩放量）
            yield return new WaitForEndOfFrame();
        }
        yield break;
    }
    IEnumerator Button_Show()
    {
        float time = Time.unscaledTime;
        float dTime = 0;
        //Color _color = buttonImg.color;
        while (dTime <= 1)
        {
            dTime = (Time.unscaledTime - time) * 4;
            thisCG.alpha = Mathf.Lerp(0, 1, dTime);//按钮渐隐，匀速
            //thisImg.rectTransform.localScale = Vector2.Lerp(thisImg.rectTransform.localScale, Vector2.one, dTime); //（改变缩放量）
            yield return new WaitForEndOfFrame();
        }
        hided = false;
        yield break;
    }
    #endregion

}

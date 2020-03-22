using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
//普通按钮脚本
//可定义点击动画反馈，动画延时，交互动画颜色、大小的值等，需注意不能改变按钮的缩放量
//包含接口：按钮启用、禁用，按钮显示与消失
[AddComponentMenu("UI/Button Normal")]
public class ButtonNormal : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    //按钮属性
    public bool interactable = true; //可交互
    [HideInInspector]public bool hided = false;//是否已隐藏
    [SerializeField] private Color disabledColor = Color.gray; //禁用时的颜色
    private Color normalColor; //正常颜色
    private Vector2 normalSize; //正常尺寸
    //private Vector2 normalScale; //正常缩放量

    [SerializeField] private bool colorTint = true; //按下是否有颜色变化
    [SerializeField] private Color pressedColor = Color.gray; //按下的颜色
    [SerializeField] private bool zoom = false; //按下是否缩放
    [SerializeField] private bool useLocalScale = false; //按下时是否改变缩放量，若否则改变尺寸
    [SerializeField] private Vector2 pressedScale = Vector2.one; //按下缩放，默认不缩放

    [SerializeField] bool clickAudio; //点击声音
    //AudioSource buttonAudio; //声音组件
    private Image buttonImg; //按钮IMAGE组件
    private RectTransform buttonRect; //按钮的Transform组件
    [Range(0, 2)] [SerializeField] private float pressDuration; //按下动画延时
    [Range(0, 2)] [SerializeField] private float releaseDuration; //松开动画延时
    public UnityEvent onClick; //点击事件

    private bool pressed; //按下判断
    private bool pointEnter; //进入区域判断
    private CanvasGroup thisCG;//按钮的CanvasGroup组件
    // Use this for initialization
    void Awake()
    {
        buttonImg = GetComponent<Image>();
        buttonRect = GetComponent<RectTransform>();
        //获取初始变量
        normalColor = buttonImg.color;
        normalSize = buttonRect.sizeDelta;
        //normalScale = buttonRect.localScale;
        //print ("normalscale: " + normalScale);
        //获取CanvasGroup组件
        if(null == GetComponent<CanvasGroup>()) gameObject.AddComponent<CanvasGroup>();
        thisCG = GetComponent<CanvasGroup>();
        thisCG.alpha = 1;
        //获取声音组件
        //buttonAudio = GetComponent<AudioSource>();
        //if (null == buttonAudio) clickAudio = false; //如果没有声音组件，则默认不播放点击声音
        //如果不可交互，禁用
        if (!interactable) DisableButton();

        hided = false;
    }
    #region 事件响应
    public void OnPointerDown(PointerEventData eventData) //按下按钮
    {
        if (!interactable || hided) return;
        //开启协程
        StopAllCoroutines(); //停止所有动画
        //开始动画
        if (colorTint) StartCoroutine(colorTint_Down());
        if (zoom) StartCoroutine(Zoom_Down());
        pressed = true;
        pointEnter = true;
    }

    public void OnPointerEnter(PointerEventData eventData) //进入按钮区域
    {
        if (!interactable || hided) return;

        if (pressed)
        {
            pointEnter = true;
            //开启协程
            StopAllCoroutines(); //停止所有动画
            //开始动画
            if (colorTint) StartCoroutine(colorTint_Down());
            if (zoom) StartCoroutine(Zoom_Down());
        }

    }
    public void OnPointerExit(PointerEventData eventData) //退出按钮区域
    {
        //if (!interactable || hided) return;
        if (pressed)
        {
            pointEnter = false;
            //开启协程
            StopAllCoroutines(); //停止所有动画
            //开始动画
            if (colorTint) StartCoroutine(colorTint_Up());
            if (zoom) StartCoroutine(Zoom_Up());
        }

    }

    public void OnPointerUp(PointerEventData eventData) //松开按钮
    {
        //if (!interactable || hided) return;

        pressed = false;
        if (pointEnter)
        {
            //print("ReleaseDuration: " + releaseDuration);
            // if (colorTint) buttonImg.color = pressedColor;
            // if (zoom) buttonRect.sizeDelta = new Vector2(pressedScale.x * normalSize.x,
            //                                      pressedScale.y * normalSize.y);
            //开启协程
            StopAllCoroutines(); //停止所有动画
            //开始动画
            if (colorTint) StartCoroutine(colorTint_Up());
            if (zoom) StartCoroutine(Zoom_Up());
            if (clickAudio) GameManager.Instance.PlayAudio(); //播放音效
            pointEnter = false;

            //执行点击事件
            onClick.Invoke();

        }
    }
    #endregion

    public void DisableButton() //禁用按钮
    {
        if(!interactable)return;
        //StopAllCoroutines();
        interactable = false;
        pressed = false;
        pointEnter = false;
        buttonImg.color = disabledColor; //按钮变色
    }
    public void EnableButton() //启用按钮
    {
        if(interactable)return;
        interactable = true;
        buttonImg.color = normalColor;

    }

    IEnumerator colorTint_Down() //按下时的颜色变换
    {
        //Color goalColor = normalColor + (pressedColor - new Color (0.5f, 0.5f, 0.5f, 0.5f)); //计算颜色
        //如果动画速度为零，则直接转换，不进行动画
        if (pressDuration == 0)
        {
            buttonImg.color = pressedColor;
            yield break;
        }

        float time = Time.unscaledTime;
        float dTime = 0;
        while (dTime <= pressDuration)
        {
            dTime = Time.unscaledTime - time;
            //按钮变色动画，加速变换
            buttonImg.color = Color.Lerp(buttonImg.color, pressedColor, dTime / pressDuration);
            yield return new WaitForEndOfFrame();
        }
        yield break;
    }
    IEnumerator colorTint_Up() //松开时的颜色变换
    {
        //如果动画延时为零，则直接转换，不进行动画
        if (releaseDuration == 0)
        {
            buttonImg.color = normalColor;
            yield break;
        }
        else if (releaseDuration >= 2)//如果动画延时大于2，则不恢复普通状态
        {
            yield break;
        }
        Color crtColor = buttonImg.color;
        float time = Time.unscaledTime;
        float dTime = 0;
        while (dTime <= releaseDuration)
        {
            dTime = Time.unscaledTime - time;
            //按钮恢复原色，匀速动画
            if(interactable)buttonImg.color = Color.Lerp(crtColor, normalColor, dTime / releaseDuration);
            else buttonImg.color = Color.Lerp(crtColor, disabledColor, dTime / releaseDuration);
            yield return new WaitForEndOfFrame();
        }
        yield break;
    }
    IEnumerator Zoom_Down() //按下时尺寸变换
    {
        Vector2 goalSize = new Vector2(normalSize.x * pressedScale.x, normalSize.y * pressedScale.y);
        //如果动画速度为零，则直接转换，不进行动画
        if (pressDuration == 0)
        {
            //print("goalSize: " + goalSize);
            if (useLocalScale) buttonRect.localScale = pressedScale;
            else buttonRect.sizeDelta = goalSize;
            yield break;
        }

        float time = Time.unscaledTime;
        float dTime = 0;
        while (dTime <= pressDuration)
        {
            dTime = Time.unscaledTime - time;
            //加速动画
            if (useLocalScale)
                buttonRect.localScale = Vector2.Lerp(buttonRect.localScale, pressedScale, dTime / pressDuration); //（改变缩放量）
            else
                buttonRect.sizeDelta = Vector2.Lerp(buttonRect.sizeDelta, goalSize, dTime / pressDuration); //按钮缩放（改变尺寸）
            yield return new WaitForEndOfFrame();
        }
        yield break;
    }
    IEnumerator Zoom_Up() //松开时尺寸变换
    {
        //如果动画速度为零，则直接转换，不进行动画
        if (releaseDuration == 0)
        {
            if (useLocalScale) buttonRect.localScale = Vector2.one;
            else buttonRect.sizeDelta = normalSize;
            yield break;
        }
        else if (releaseDuration >= 2)//如果动画延时大于2，则不恢复普通状态
        {
            yield break;
        }
        //记录此时的数值
        Vector2 crntScale = buttonRect.localScale;
        Vector2 crntSize = buttonRect.sizeDelta;

        float time = Time.unscaledTime;
        float dTime = 0;
        while (dTime <= releaseDuration)
        {
            dTime = Time.unscaledTime - time;
            //动画，匀速变换
            if (useLocalScale)
                buttonRect.localScale = Vector2.Lerp(crntScale, Vector2.one, dTime / releaseDuration); //（改变缩放量）,默认原始缩放量为1
            else
                buttonRect.sizeDelta = Vector2.Lerp(crntSize, normalSize, dTime / releaseDuration); //按钮缩放（改变尺寸）
            yield return new WaitForEndOfFrame();
        }
        yield break;
    }
    
    public void HideButton()//禁用按钮不带参数，默认在隐藏时启用物体
    {
        //showed = interactable;
        //StopAllCoroutines();
        hided = true;
        if(!gameObject.activeInHierarchy)return;
        StartCoroutine(Button_Hide(true));
    }
    public void HideButton(bool setActive)//隐藏按钮，参数为按钮物体的启用状态、是否缩放
    {
        //showed = interactable;
        //StopAllCoroutines();
        hided = true;
        if(!gameObject.activeInHierarchy)return;
        StartCoroutine(Button_Hide(setActive));
    }
    public void ShowButton()//显示按钮
    {
        if(!gameObject.activeInHierarchy)gameObject.SetActive(true);//启用按钮
        //StopAllCoroutines();
        StartCoroutine(Button_Show());
    }
    IEnumerator Button_Hide(bool setActive)
    {
        float time = Time.unscaledTime;
        float dTime = 0;
        while (dTime <= 1)
        {
            dTime = (Time.unscaledTime - time) * 4;

            //按钮渐隐，匀速
            thisCG.alpha = Mathf.Lerp(1, 0, dTime);
            //buttonRect.localScale = Vector2.Lerp(buttonRect.localScale, Vector2.zero, dTime); //（改变缩放量）
            yield return new WaitForEndOfFrame();
        }
        if (!setActive)//需要禁用物体
        {
            buttonImg.color = normalColor;
            if(useLocalScale)buttonRect.localScale = Vector2.one;
            else buttonRect.sizeDelta = normalSize;
            gameObject.SetActive(setActive);//禁用按钮
        }
        yield break;
    }
    IEnumerator Button_Show()
    {
        //恢复按钮颜色
        if(interactable)buttonImg.color = normalColor;
        else buttonImg.color = disabledColor;
        float time = Time.unscaledTime;
        float dTime = 0;
        while (dTime <= 1)
        {
            dTime = (Time.unscaledTime - time) * 4;
            //按钮渐隐，匀速
            thisCG.alpha = Mathf.Lerp(0, 1, dTime);
            //buttonRect.localScale = Vector2.Lerp(buttonRect.localScale, Vector2.one, dTime); //（改变缩放量）
            yield return new WaitForEndOfFrame();
        }
        hided = false;//改变标志
        yield break;
    }
}
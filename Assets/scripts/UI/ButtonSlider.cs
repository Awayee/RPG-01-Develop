using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;


public class FloatEvent : UnityEvent<float> { }
//自制滑动条，绑在handle上

public class ButtonSlider : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler{
    //按钮属性
    public bool interactable = true;//可交互
    public float value;
    private Color normalColor;//正常颜色
    [SerializeField] private Color pressedColor;//按下颜色
    private Vector2 normalSize;//正常尺寸
    
    [SerializeField] private Vector2 pressedScale;//按下缩放
    //private Vector2 pressedSize;//按下尺寸
    private Image thisImg;//按钮IMAGE组件
    private RectTransform thisRect;//按钮的Transform组件
    //UI
    private float startPointX;//拖拽初始点
    private float handleStartPosX;//拖拽开始时控件位置
    private float xR;//坐标转换比例
    private Image fillArea;
    private float maxX;//滑动的范围

    [SerializeField]public FloatEvent onValueChanged = new FloatEvent();//拖拽事件
    // Use this for initialization
    void Awake () {

        //获取必要组件
        thisImg = GetComponent<Image>();
        normalColor = thisImg.color;
        thisRect = GetComponent<RectTransform>();
        normalSize = thisRect.sizeDelta;
        //gM = GameObject.Find("Canvas/GameMenu").GetComponent<MainMenu>();

        fillArea = transform.parent.GetChild(0).GetComponent<Image>();
        //获取滑动范围
        //minX = fthrRect.position.x - fthrRect.sizeDelta.x / 2 + 15;
        maxX = fillArea.rectTransform.sizeDelta.x - 30;
        float fillAmnt = thisRect.anchoredPosition.x / maxX;
        fillArea.fillAmount = fillAmnt;
        
        xR = GameManager.Instance.wRatio;
        //onValueChanged = new UnityEvent<float>();
        //print(xR);


    }
    public void OnPointerDown(PointerEventData eventData)//按下按钮
    {
        if (!interactable) return;
        StopAllCoroutines();//停止该脚本所有动画
        thisImg.color = pressedColor;
        //fillArea.color = pressedColor;
        thisRect.sizeDelta = Vector2.Scale(normalSize,pressedScale);//获取按下时按钮的尺寸;//按钮缩放
        startPointX = Input.mousePosition.x;//获取此时触摸点的X坐标
        handleStartPosX = thisRect.anchoredPosition.x;//此时控件的X坐标
    }
    public void OnDrag(PointerEventData eventData)//拖拽按钮
    {
        //print("Mouse Position: " + Input.mousePosition);
        //print("Position: " + transform.position);
        //跟随触摸点
        float pointX = Input.mousePosition.x;//此时的触摸点
        float dPos = pointX - startPointX;//得到差值
        dPos *= xR;//坐标转换
        //print("delta Position:" + dPos);
        thisRect.anchoredPosition = new Vector2(handleStartPosX + dPos, 0);
        //调整到滑动范围内
        if (thisRect.anchoredPosition.x >= maxX)
            thisRect.anchoredPosition = new Vector2(maxX, 0);
        else if(thisRect.anchoredPosition.x <= 0)
            thisRect.anchoredPosition = new Vector2(0, 0);
        //计算，绘制填充区域
        value = thisRect.anchoredPosition.x / maxX;
        fillArea.fillAmount = value;

        //拖拽按按钮事件
        //gM.SetVolume(fillAmnt);//设置音量
        onValueChanged.Invoke(value);
    }

    public void OnPointerUp(PointerEventData eventData)//松开按钮
    {
        if (!interactable) return;
        StartCoroutine(PressUp());//松开按钮的动画
        //松开按钮事件：
        
        

    }
    public void SetValue(float _value)//设置值
    {
        //限制范围为0-1
        if(_value>1)_value = 1;
        else if(_value<0)_value = 0;
        thisRect.anchoredPosition = new Vector2(maxX * _value, 0);
        fillArea.fillAmount = _value;
        value = _value;

    }

    /*
    private Vector3 ScreenToUI(Vector3 ScreenPos)    //坐标转换（屏幕坐标转换为UI坐标）
    {
        Vector3 TransPos = ScreenPos;
        TransPos -= ScreenSize;
        return new Vector3(w * TransPos.x, h * TransPos.y, 0);
    }
     */

    IEnumerator PressUp()//协程，松开动画
    {
        float time = Time.unscaledTime;
        float dtime = 0;
        Vector2 pressedSize = Vector2.Scale(normalSize,pressedScale);//获取按下时按钮的尺寸
        //Vector2 goalSize = new Vector2(normalSize.x * pressedScale.x, normalSize.y * pressedScale.y);
        while (dtime < 1)
        {
            dtime = (Time.unscaledTime - time) * 2;
            thisImg.color = Color.Lerp(pressedColor, normalColor, dtime);//按钮变色
            //fillArea.color = Color.Lerp(pressedColor, normalColor, dtime);//填充区域变色
            thisRect.sizeDelta = Vector2.Lerp(pressedSize, normalSize, dtime);//按钮缩放
            yield return null;
        }
        yield break;
    }

}

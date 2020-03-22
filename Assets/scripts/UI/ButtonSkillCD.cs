using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
//按钮CD。无需绑定Button组件
public class ButtonSkillCD : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    //按钮组件
    private Color normalColor;//正常颜色
    public Color pressedColor;//按下颜色
    public Color disabledColor;//按钮禁用时的颜色

    //private RectTransform buttonRect;//RECTTRANSFORM属性
    //private Vector3 normalScale;//正常缩放
    //public Vector3 pressedScale;//按下缩放
    
    public bool interactable;//按钮能否点击
    private bool pressed;//按下判断
    private bool pointEter;//进入区域判断

    private Text TimeTip;//时间提示
    private string ButtonText;//初始按钮文字
    private Image buttonImg;//按钮IMAGE

    private float CurrentTime = 0f;//当前时间
    private bool canSkill;//能否释放技能
    public float CdTime = 5f;//技能冷却时间
    public UnityEvent onClick;//点击事件
	// Use this for initialization
	void Start () {
        //获取所需对象
        //player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayCharacter>();
		//thisButton = this.GetComponent<Button>();
        buttonImg = this.GetComponent<Image>();
        normalColor = buttonImg.color;
        TimeTip = this.GetComponentInChildren<Text>();
        //获取初始缩放量
        //buttonRect = GetComponent<RectTransform>();
        //normalScale = buttonRect.localScale;
        //ButtonText = TimeTip.text;
        //print("TimeTip:" + TimeTip.text);
        canSkill = true;
	}
	
	// Update is called once per frame
    void Update()
    {
        //技能CD控制
        if (canSkill) return;
        else
        {
            CurrentTime += Time.deltaTime;
            buttonImg.fillAmount = CurrentTime / CdTime;
            TimeTip.text = (CdTime - (int)CurrentTime).ToString();//显示技能剩余时间
            if (CurrentTime >= CdTime)
            {
                canSkill = true;
                CurrentTime = 0; 
                TimeTip.text = ButtonText;
                EnableButton();
                return;
            }
        }

    }
    public void OnPointerDown(PointerEventData eventData)//按下按钮
    {
        if (!interactable) return;//如果按钮不可点击
        buttonImg.color = pressedColor;
        //buttonRect.localScale = pressedScale;
        pressed = true;
        pointEter = true;
    }
    public void OnPointerEnter(PointerEventData eventData)//进入按钮区域
    {
        if (!interactable) return;

        if (pressed)
        {
            pointEter = true;
            buttonImg.color = pressedColor;
            //buttonRect.localScale = pressedScale;
        }

    }
    public void OnPointerExit(PointerEventData eventData)//退出按钮区域
    {
        if (!interactable) return;
        if (pressed)
        {
            pointEter = false;
            buttonImg.color = normalColor;
            //buttonRect.localScale = normalScale;
        }

    }
    public void OnPointerUp(PointerEventData eventData)//松开按钮
    {

        if (!interactable) return;//如果按钮不可点击
        buttonImg.color = normalColor;
        //buttonRect.localScale = normalScale;
        pressed = false;
        if (pointEter)
        {
            //Skilled = true;
            canSkill = false;
            onClick.Invoke();//触发事件
            DisableButton();
        }

    }
    public bool CanSkill()
    {
        if (canSkill) return true;
        else return false;
    }
    public void EnableButton()
    {
        interactable = true;
        buttonImg.color = normalColor;
    }

    public void DisableButton()
    {
        interactable = false;
        buttonImg.color = disabledColor;
    }
}

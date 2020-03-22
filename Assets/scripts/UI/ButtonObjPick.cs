using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonObjPick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler {

	//按钮属性
    private Image buttonImage;//按钮IMAGE用于调整按钮颜色
    private Color normalColor;//正常颜色
    [SerializeField]
    private Color pressedColor;//按下颜色
    private Text buttonTxt;//按钮文字
    private float a_txt;//记录文字透明度

    public bool interactable = true;//能否交互
    private bool pressed;//按下判断
    private bool pointEter;//进入区域判断

    //物品属性
    Transform pkTrans;//要捡起的物品的位置
    PickableObj pkObj;//要捡起的物品的脚本
    [HideInInspector]public Transform canvasTrans;//画布物体
    private float w, h;//坐标转换因子
    //private Vector2 halfScrnSize;//屏幕分辨率的一半，用于坐标转换

    Camera cmr;//相机
    // Use this for initialization
    void Awake() {
        //获取屏幕和画布
        canvasTrans = GameManager.Instance.UICanvas;        
        //获取图片信息
        buttonImage = GetComponent<Image>();
        normalColor = buttonImage.color;
        buttonTxt = GetComponentInChildren<Text>();
        a_txt = buttonTxt.color.a;
        buttonImage.color -= new Color(0, 0, 0, normalColor.a);
        buttonTxt.color -= new Color(0, 0, 0, a_txt);
        
        //获取坐标转换因子
        w = GameManager.Instance.wRatio;
        h = GameManager.Instance.hRatio;
        //根据屏幕重新调整尺寸
        transform.localScale = new Vector2(transform.localScale.x / w, transform.localScale.y / h);
        //设置父物体
        transform.SetParent(canvasTrans.GetChild(0));
        // int tcount = canvasTrans.childCount;
        // transform.SetSiblingIndex(tcount - GameManager.Instance.uiCount -1);//得到此时的层级，每次调用时都会将新的按钮置于所有按钮的顶层
        //调整位置
        transform.localPosition = Vector3.zero;
        //valueImage.GetComponent<RectTransform>().sizeDelta = scaledSize;//调整子物体尺寸
        cmr = Camera.main;
    }
	
	// Update is called once per frame
	void LateUpdate () {
        
        if(pkTrans != null) {
            Vector2 Pos = cmr.WorldToScreenPoint(pkTrans.position);//世界坐标转换为屏幕坐标
            buttonImage.rectTransform.anchoredPosition = new Vector2(w * Pos.x + 40, h * Pos.y - 30);//位置偏右下
        }

	}

	#region 按钮属性
    public void OnPointerDown(PointerEventData eventData)//按下按钮
    {
        if (!interactable) return;
        buttonImage.color = pressedColor;//按钮反馈
        pressed = true;
        pointEter = true;
    }

    public void OnPointerEnter(PointerEventData eventData)//进入按钮区域
    {
        if (!interactable) return;

        if (pressed)
        {
            pointEter = true;
            buttonImage.color = pressedColor;//按钮反馈
            //buttonRect.localScale = pressedScale;
        }

    }
    public void OnPointerExit(PointerEventData eventData)//退出按钮区域
    {
        if (!interactable) return;
        if (pressed)
        {
            pointEter = false;
            buttonImage.color = normalColor;//按钮反馈
            //buttonRect.localScale = normalScale;
        }

    }

    public void OnPointerUp(PointerEventData eventData)//松开按钮
    {
         if (!interactable) return;
        pressed = false;
        if (pointEter)
        {
            pointEter = false;
            buttonImage.color = normalColor;
            //执行点击事件

            pkObj.PickUpThis();
            
        }


    }
    #endregion
    
    //得到对应的物体
    public void GetObj(Transform objTrans)
    {
        pkTrans = objTrans;
        pkObj = pkTrans.GetComponent<PickableObj>();
    }
    //销毁按钮
    public void DestroyButton(float t)
    {
        Fade_Disappear(t);
        Destroy(gameObject, t);
    }
    
    //按钮动画
    public void Fade_Appear(float t)//逐渐显示
    {
        StopAllCoroutines();
        StartCoroutine(FadeIn(t));
    }
    public void Fade_Disappear(float t)//逐渐消失
    {
        StopAllCoroutines();
        StartCoroutine(FadeOut(t));   
    }

    IEnumerator FadeIn(float offTime)//渐显
    {
        Text btnTxt = GetComponentInChildren<Text>();
        float t = Time.time;
		float delT = 0;
		while (delT < 1) {
			delT = (Time.time - t) / offTime;
            //改变透明度
			float alp_I = Mathf.Lerp(buttonImage.color.a, normalColor.a, delT);
			
			buttonImage.color = new Color(buttonImage.color.r, buttonImage.color.g, buttonImage.color.b, alp_I);

            float alp_T = Mathf.Lerp(buttonTxt.color.a, a_txt, delT);
            buttonTxt.color = new Color (buttonTxt.color.r, buttonTxt.color.g, buttonTxt.color.b, alp_T);
			yield return null;
		}
        interactable = true;//不可交互
		yield break;
    }

    IEnumerator FadeOut(float offTime)//渐隐
    {
        interactable = false;//不可交互
        Text btnTxt = GetComponentInChildren<Text>();
        float t = Time.time;
		float delT = 0;
		while (delT < 1) {
			delT = (Time.time - t)/offTime;
            //改变透明度
			float alp_I = Mathf.Lerp(buttonImage.color.a, 0, delT);
			
			buttonImage.color = new Color(buttonImage.color.r, buttonImage.color.g, buttonImage.color.b, alp_I);

            float alp_T = Mathf.Lerp(buttonTxt.color.a, 0, delT);
            buttonTxt.color = new Color (buttonTxt.color.r, buttonTxt.color.g, buttonTxt.color.b, alp_T);
			yield return null;
		}
		yield break;
    }
}

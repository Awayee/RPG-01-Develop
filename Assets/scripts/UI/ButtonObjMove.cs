using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
//“捡起物品”按钮上的脚本，实现点击交互和物品信息的传递
public class ButtonObjMove : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
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

    //用于坐标转换
    private float w, h;//坐标转换因子
    //private Vector2 halfScrnSize;//屏幕分辨率的一半，用于坐标转换
    [HideInInspector]public Transform canvasTrans;//画布
    //物品属性
    Transform mvObj;//要捡起的物品
    //玩家属性
    PlayerObj playO;
    private bool Picked=false;
	// Use this for initialization
	void Awake() {
        //获取屏幕和画布
        canvasTrans = GameManager.Instance.UICanvas;       
        //获取图片信息
        buttonImage = GetComponent<Image>();
        normalColor = buttonImage.color;
        buttonTxt = GetComponentInChildren<Text>();
        a_txt = buttonTxt.color.a;
        //透明化
        buttonImage.color -= new Color(0, 0, 0, buttonImage.color.a);
        buttonTxt.color -= new Color(0, 0, 0, buttonTxt.color.a);

        //根据屏幕重新调整尺寸 
        h = GameManager.Instance.wRatio;//获取坐标转换因子
        w = GameManager.Instance.hRatio;
        transform.localScale = new Vector2(transform.localScale.x / w, transform.localScale.y / h);
        //valueImage.GetComponent<RectTransform>().sizeDelta = scaledSize;//调整子物体尺寸
        //调整层级
        transform.SetParent(canvasTrans.GetChild(0));//设置父物体
        // int tcount = canvasTrans.childCount;
        // transform.SetSiblingIndex(tcount - GameManager.Instance.uiCount-1);//得到此时的层级，每次调用时都会将新的按钮置于所有按钮的顶层
	}
	
	// Update is called once per frame
	void LateUpdate () {
        //如果物品没有被捡起，则使按钮实时跟踪物品
        if (!Picked)
        {
            if(mvObj == null) return;
            Vector2 Pos = Camera.main.WorldToScreenPoint(mvObj.position);//世界坐标转换为屏幕坐标
            //print("Pos: "+Pos);
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
            if(!Picked){
                mvObj.GetComponent<MoveableObj>().PickUpThisObj();//捡起物品
                Fade_Disappear(0.5f);//按钮消失
                Picked = true;//标记
                Invoke("ChangeButton", 0.5f);//更换按钮
            }
            else
            {
                playO.TakeOffObs();//放下物品
                DestroyObjButton(2);//销毁按钮
            }
        }


    }
    #endregion
    public void SetPlayerAndObj(PlayerObj plyo, Transform obj)//设置玩家
    {
        playO = plyo;
        mvObj = obj;
    }
    void ChangeButton()//更换按钮状态
    {
        Fade_Appear(0.5f);
        buttonTxt.text = "放下";
    }

    public void DestroyObjButton(float t)//销毁该按钮
    {
        interactable = false;//不可交互
        StopAllCoroutines();
        StartCoroutine(FadeOut(t));
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
        StartCoroutine(FadeOut(t));   
    }
    IEnumerator FadeIn(float offtime)//渐显
    {
        float t = Time.time;
		float delT = 0;
		while (delT < 1) {
			delT = (Time.time - t)/offtime;
            //改变透明度
			float alp_I = Mathf.Lerp(buttonImage.color.a, normalColor.a, delT);
			
			buttonImage.color = new Color(buttonImage.color.r, buttonImage.color.g, buttonImage.color.b, alp_I);

            float alp_T = Mathf.Lerp(buttonTxt.color.a, a_txt, delT);
            buttonTxt.color = new Color (buttonTxt.color.r, buttonTxt.color.g, buttonTxt.color.b, alp_T);
			yield return null;
		}
        interactable = true;//可交互
		yield break;
    }

    IEnumerator FadeOut(float offtime)//渐隐
    {
        interactable = false;//不可交互
        float t = Time.time;
		float delT = 0;
		while (delT < 1) {
			delT = (Time.time - t)/offtime;
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

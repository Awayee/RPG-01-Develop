using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TipMessage : MonoBehaviour {
    Image tipImg;
    Text tipMsg;
    private float alpha_Img, alpha_Txt;//记录透明度
    private Vector2 tipPosition;//提示框位置
    [Range(0, 2)][SerializeField] float appearDuration=.1f, disappearDuration=.5f;//动画延时
	// Use this for initialization
	void Awake () {
        //获取对象和初始值
        tipImg = GetComponent<Image>();
        alpha_Img = tipImg.color.a;
        tipPosition = tipImg.rectTransform.anchoredPosition;
        tipMsg = GetComponentInChildren<Text>();
        alpha_Txt = tipMsg.color.a;

        tipImg.color -= new Color(0,0,0,alpha_Img);//外框颜色
        tipMsg.color -= new Color(0,0,0,alpha_Txt);//文字颜色
        
	}
    public void TipWords(string msg,float offTime)//弹出提示消息，停留时间
    {
        transform.SetAsLastSibling();
        //print(TipperOriginColor);
        tipMsg.text = msg;//消息内容
        //开始东动画
        StopAllCoroutines();
        StartCoroutine(FadeOut(offTime));//一段时间后，提示框消失
        
    }
    IEnumerator FadeIn()//动画，渐显
    {
        tipImg.rectTransform.anchoredPosition = tipPosition;//移动到界面内
        if(appearDuration <= 0)//如果延时为零则无动画
        {
            tipImg.color = new Color(tipImg.color.r, tipImg.color.g, tipImg.color.b, alpha_Img);
            tipMsg.color = new Color(tipMsg.color.r, tipMsg.color.g, tipMsg.color.b, alpha_Txt);
        }
        else
        {
            float time = Time.unscaledTime;//此刻的时间
            float dtime = 0;//经过的时间
            while(dtime < appearDuration){
                dtime = Time.unscaledTime - time;
                //变换背景框透明度
                float a_img = Mathf.Lerp(0, alpha_Img, dtime / appearDuration);
                tipImg.color = new Color(tipImg.color.r, tipImg.color.g, tipImg.color.b, a_img);
                //变换文本透明度
                float a_txt = Mathf.Lerp(0, alpha_Txt, dtime / appearDuration);
                tipMsg.color = new Color(tipMsg.color.r, tipMsg.color.g, tipMsg.color.b, a_txt);
                yield return null;
            }
        }
        yield break;


    }
    IEnumerator FadeOut(float t)//动画，渐隐
    {
        yield return StartCoroutine(FadeIn());//等待显示完毕
        yield return new WaitForSecondsRealtime(t);//t秒后消失

        if(disappearDuration <= 0)//如果延时为零则无动画
        {
            tipImg.color = new Color(tipImg.color.r, tipImg.color.g, tipImg.color.b, 0);
            tipMsg.color = new Color(tipMsg.color.r, tipMsg.color.g, tipMsg.color.b, 0);
        }
        else
        {
            float time = Time.unscaledTime;//此刻的时间
            float dtime = 0;//经过的时间
            while(dtime < disappearDuration){
                dtime = Time.unscaledTime - time;
                //变换背景框透明度
                float a_img = Mathf.Lerp(alpha_Img, 0, dtime / disappearDuration);
                tipImg.color = new Color(tipImg.color.r, tipImg.color.g, tipImg.color.b, a_img);
                //变换文本透明度
                float a_txt = Mathf.Lerp(alpha_Txt, 0, dtime / disappearDuration);
                tipMsg.color = new Color(tipMsg.color.r, tipMsg.color.g, tipMsg.color.b, a_txt);
                yield return null;
            }

        }
        tipImg.rectTransform.anchoredPosition = -tipPosition;//移除到界面外
        yield break;
    }
}

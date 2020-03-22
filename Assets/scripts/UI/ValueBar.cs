using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//血条、能量条等
public class ValueBar : MonoBehaviour {

    private Image thisImg; //该物体
    private Image valueBkg; //血条背景，实现残留效果
    private Image valueBar; //血条
    private float a_this, a_bkg, a_value; //记录各组件的透明度
    private float value; //当前值
    private bool hided; //是否隐藏
    void Awake () {
        // thisImg = GetComponent<Image>();
        valueBkg = transform.GetChild(0).GetComponent<Image> ();
        valueBar = transform.GetChild(1).GetComponent<Image> ();

        thisImg = GetComponent<Image> ();
        valueBar.fillAmount = value;
        //开始时颜色透明透明化
        a_this = thisImg.color.a;
        a_value = valueBar.color.a;
        a_bkg = valueBkg.color.a;
        thisImg.color -= new Color (0, 0, 0, a_this);
        valueBar.color -= new Color (0, 0, 0, a_value);

        valueBkg.color -= new Color (0, 0, 0, a_bkg);

    }
    // Use this for initialization
    //设定值
    public void SetValue (float fillamt) {
        if (hided) //如果已隐藏，无需进行以下操作
        {
            value = fillamt;
            return;
        }
        value = valueBar.fillAmount; //记录变化前的值

        valueBar.fillAmount = fillamt;
        if (value - fillamt >= 0.04f) {//如果变化的值过小，则跳过动画
            StopAllCoroutines();
            StartCoroutine (barGradual (fillamt));
        }
        value = fillamt;
    }
    private IEnumerator barGradual (float fill) //血量渐变的效果
    {
        valueBkg.fillAmount = value;
        valueBkg.color = 0.8f * valueBar.color;
        //根据插值设置颜色变化量
        //float colorStep = valueBkg.color.a / Mathf.Abs(fill - currentValue);
        float fillstep = 2 * Time.deltaTime * Mathf.Abs (value - fill);//背景停留时间为半秒
        Color colorStep = new Color (0, 0, 0, valueBkg.color.a * fillstep / Mathf.Abs (value - fill));
        while (valueBkg.fillAmount != fill) {
            //print("渐变中……"+valueBkg.fillAmount);
            valueBkg.fillAmount = Mathf.MoveTowards (valueBkg.fillAmount, fill, fillstep); //填充范围
            valueBkg.color -= colorStep; //背景渐变
            yield return null;
        }
        valueBkg.fillAmount = 0;
        yield break;
    }

    public void Display (float offTime, bool d) //血条显现，参数为：动画时间、是否改变FillAmount
    {
        StopAllCoroutines ();
        //SetValue(value);
        StartCoroutine (FadeIn (offTime, d)); //一秒的渐显动画

    }

    public void Hide (float offTime, bool d) //血条渐隐
    {
        StopAllCoroutines ();
        StartCoroutine (FadeOut (offTime, d)); //一秒的渐隐动画
    }

    IEnumerator FadeIn (float completTime, bool draw) //逐渐显现，变量为动画时间、是否使用笔划效果
    {

        //valueBar.fillAmount = 0;
        if (draw) {
            //填充暂时设为零
            thisImg.fillAmount = 0;
            valueBar.fillAmount = 0;
        } else {
            thisImg.fillAmount = 1;
            valueBar.fillAmount = value;
        }
        //Image bkgImage = GetComponent<Image>();//该物体的IMAGE组件
        //float crntFill = 
        float t = Time.unscaledTime;
        float delT = 0;
        while (delT <= 1) {
            delT = (Time.unscaledTime - t) / completTime;
            //改变透明度
            //加速动画
            float alp_t = Mathf.Lerp (thisImg.color.a, a_this, delT);
            thisImg.color = new Color (thisImg.color.r, thisImg.color.g, thisImg.color.b, alp_t);

            float alp_v = Mathf.Lerp (valueBar.color.a, a_value, delT);
            valueBar.color = new Color (valueBar.color.r, valueBar.color.g, valueBar.color.b, alp_v);
            if (draw) {
                valueBar.fillAmount = Mathf.Lerp (valueBar.fillAmount, value, delT);
                thisImg.fillAmount = Mathf.Lerp (thisImg.fillAmount, 1, delT);
            }

            yield return new WaitForEndOfFrame();
        }
        hided = false;
        yield break;
    }

    IEnumerator FadeOut (float completTime, bool draw) //逐渐消失，变量为动画时间
    {
        //背景立即回到填充点
        valueBkg.fillAmount = valueBar.fillAmount;
        valueBkg.color = new Color (valueBkg.color.r, valueBkg.color.g, valueBkg.color.g, 0);

        thisImg.fillAmount = 1;
        valueBkg.fillAmount = value;
        //Image bkgImage = GetComponent<Image>();//该物体的IMAGE组件
        float t = Time.unscaledTime;
        float delT = 0;
        while (delT < 1) {
            delT = (Time.unscaledTime - t) / completTime;
            //改变透明度
            float alp_t = Mathf.Lerp (thisImg.color.a, 0, delT);
            thisImg.color = new Color (thisImg.color.r, thisImg.color.g, thisImg.color.b, alp_t);

            float alp_v = Mathf.Lerp (valueBar.color.a, 0, delT);
            valueBar.color = new Color (valueBar.color.r, valueBar.color.g, valueBar.color.b, alp_v);

            if (draw) {
                valueBar.fillAmount = Mathf.Lerp (valueBar.fillAmount, 0, delT);
                thisImg.fillAmount = Mathf.Lerp (thisImg.fillAmount, 0, delT);
            }

            yield return new WaitForEndOfFrame();
        }
        hided = true;
        yield break;
    }
    public void DestroyBar (float offTime) //销毁该物体
    {
        hided = true;
        Hide (offTime, true);
        Destroy (gameObject, offTime);
    }

}
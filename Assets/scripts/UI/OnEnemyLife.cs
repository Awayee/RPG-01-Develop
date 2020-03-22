using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//控制敌人血条
public class OnEnemyLife : MonoBehaviour
{
    [HideInInspector]public float h_offset = 6f;//血条高度
    Transform targetTrans;//要跟随的怪物
    bool hided = false;//是否已经隐藏
    Camera cmr;
    //坐标转换
    RectTransform thisRect;
    private float w, h;//坐标转换因子
    void Awake()
    {
        thisRect = GetComponent<RectTransform>();
        //获取画布
        Transform canvasTrans = GameManager.Instance.UICanvas;
        //获取坐标转换因子
        w = GameManager.Instance.wRatio;
        h = GameManager.Instance.hRatio;
        //hafScrnSize = GameManager.Instance.screenSize * 0.5f;
        //调整层级
        transform.SetParent(canvasTrans.GetChild(0));
        // int tcount = canvasTrans.childCount;
        // transform.SetSiblingIndex(tcount - GameManager.Instance.uiCount-1);//调整层级，每次调用时都会将新的按钮置于所有按钮的顶层

        //重新调整尺寸
        transform.localScale = new Vector2(transform.localScale.x / w, transform.localScale.y / h);
        cmr = Camera.main;//获取主相机
        //调节颜色为透明
        // Image thisImg = GetComponent<Image>();
        // thisImg = GetComponent<Image>();
        // thisImg.color = new Color(thisImg.color.r, thisImg.color.g, thisImg.color.b, 0);
        // valueImage.color = new Color (valueImage.color.r, valueImage.color.g, valueImage.color.b, 0);
    }
    void LateUpdate()
    {
        if (hided) return;
        //跟随
        Vector2 Pos = cmr.WorldToScreenPoint(targetTrans.position + Vector3.up * h_offset);//世界坐标转换为屏幕坐标
        //Pos -= hafScrnSize;//屏幕坐标转换为GUI坐标
        thisRect.anchoredPosition = new Vector3(w * Pos.x, h * Pos.y, 0);

    }

    //赋予目标物体
    public void SetTarget(Transform objTrans)
    {
        targetTrans = objTrans;
    }

    // public void Show_FadeIn(float offTime)//血条显现
    // {
    // 	StopAllCoroutines();
    // 	StartCoroutine(FadeIn(offTime));//一秒的渐显动画
    // }

    // public void Hide_FadeOut(float offTime)//血条渐隐
    // {
    // 	StopAllCoroutines();
    // 	StartCoroutine(FadeOut(offTime));//一秒的渐隐动画
    // }

    // IEnumerator FadeIn(float completTime)//逐渐显现，变量为动画时间
    // {
    // 	hided = false;
    // 	Image bkgImage = GetComponent<Image>();//该物体的IMAGE组件
    // 	float t = Time.time;
    // 	float delT = 0;
    // 	while (delT < completTime) {
    // 		delT = Time.time - t;
    // 		float alp = Mathf.Lerp(bkgImage.color.a, 1, delT / completTime);
    // 		//改变透明度
    // 		bkgImage.color = new Color(bkgImage.color.r, bkgImage.color.g, bkgImage.color.b, alp);
    // 		valueImage.color = new Color (valueImage.color.r, valueImage.color.g, valueImage.color.b, alp);
    // 		yield return null;
    // 	}
    // 	yield break;
    // }

    // IEnumerator FadeOut(float completTime)//逐渐消失，变量为动画时间
    // {
    // 	Image bkgImage = GetComponent<Image>();//该物体的IMAGE组件
    // 	float t = Time.time;
    // 	float delT = 0;
    // 	while (delT < completTime) {
    // 		delT = Time.time - t;
    // 		float alp = Mathf.Lerp(bkgImage.color.a, 0, delT / completTime);
    // 		//改变透明度
    // 		bkgImage.color = new Color(bkgImage.color.r, bkgImage.color.g, bkgImage.color.b, alp);
    // 		valueImage.color = new Color (valueImage.color.r, valueImage.color.g, valueImage.color.b, alp);
    // 		yield return null;
    // 	}
    // 	hided = true;
    // 	yield break;
    // }
    // public void DestroyBar(float offTime)//销毁该物体
    // {
    // 	Hide_FadeOut(offTime);
    // 	Destroy(gameObject, offTime);
    // }
}

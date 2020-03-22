using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayDamage : MonoBehaviour {
    public Color colorOnPlayer;//玩家受伤
    public Color colorOnEnemy;//敌人受伤
    private float w, h;
    private Text thisText;
    private RectTransform thisRect;
    // Use this for initialization
    void Awake () {

        thisText = GetComponent<Text>();
        thisRect = GetComponent<RectTransform>();
        
        //一秒后销毁
        Destroy(gameObject, 1f);
        w = GameManager.Instance.wRatio;
        h = GameManager.Instance.hRatio;
        //根据屏幕重新调整尺寸
        transform.localScale = new Vector2(transform.localScale.x / w, transform.localScale.y / h);

        //调整层级
        Transform canvasTrans = GameManager.Instance.UICanvas;
        transform.SetParent(canvasTrans);
        int tcount = canvasTrans.childCount;
        transform.SetSiblingIndex(tcount - GameManager.Instance.uiCount-1);//得到此时的层级，每次调用时都会将新的按钮置于所有按钮的顶层

	}
    public void ShowDamage(GameObject gameObj, float DamageAmount, bool isPlayer)//显示受到的伤害
    {
        Vector2 Pos = Camera.main.WorldToScreenPoint(gameObj.transform.position + 4 * Vector3.up);//世界坐标转换为屏幕坐标
        thisRect.anchoredPosition = new Vector2(w * Pos.x, h * Pos.y);
        if (isPlayer)
            thisText.color = colorOnPlayer;
        else thisText.color = colorOnEnemy;
        thisText.text = DamageAmount.ToString();
        //Destroy(gameObject, 1f);//1秒后销毁

    }
}

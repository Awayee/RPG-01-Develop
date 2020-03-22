using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//UI上的开关按钮
public class SwitchSprite : MonoBehaviour {
    public bool enable; //状态
    private Sprite DisabledSprite; //禁用
    private Color DisabledColor;
    public Sprite EnabledSprite; //启用
    //private Color EnabledColor;
    //public UnityEvent onClick;//点击事件
    private Image thisImage;
    //private Image ftrImage;//父物体
    // Use this for initialization
    void Awake () {
        //初始化
        thisImage = gameObject.GetComponent<Image> ();
        DisabledSprite = thisImage.sprite;//记录禁用时的图标
        DisabledColor = thisImage.color;
        //ftrImage = transform.parent.GetComponent<Image>();
        thisImage.sprite = enable ? EnabledSprite : DisabledSprite;
        //thisImage.color = enable ? EnabledColor : DisabledColor;
    }

    public void Switch () //切换图标
    {
        if (enable) {
            SetFalse ();
        } else {
            SetTrue ();
        }
        //print("onClick.");
        //onClick.Invoke();
    }
    public void SetFalse () {
        thisImage.sprite = DisabledSprite;
        //ftrImage.sprite = DisabledSprite;
        thisImage.color = DisabledColor;
        enable = false;
    }
    public void SetTrue () {
        thisImage.sprite = EnabledSprite;
        //ftrImage.sprite = EnabledSprite;
        //thisImage.color = EnabledColor;
        enable = true;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class TabItem : MonoBehaviour, IPointerClickHandler
{
    bool enable;//是否启用的标记
    Image thisimg;
    Text thisTxt;
    TabSelect ts;//父物体的选项卡脚本
    [SerializeField] string Page;//对应的预制体的名称
    private GameObject thisPage;//
    //public GameObject thisPage;//对应页面
    // public Color disabledTxtColor=new Color(0,0,0,0);//未选择时的文字颜色
    // public Color disabledBkgColor = Color.black;//未选择时的背景颜色
    // public Color enabledTxtColor=Color.black;//选择时的文字颜色
    // public Color enabledBkgColor = Color.white;//选择时的背景颜色


    //初始化
    public void InitializeThis()
    {
        //获取对象
        thisimg = transform.GetChild(0).GetComponent<Image>();
        if(null == thisimg)thisimg = GetComponent<Image>();
        thisTxt = GetComponentInChildren<Text>();
        //InSelectThis();//开始时未选择
        ts = transform.parent.GetComponent<TabSelect>();
    }
    public void InSelectThis()//未选中该选项卡
    {
        //调整颜色
        // thisimg.color = disabledBkgColor;
        // thisTxt.color = disabledTxtColor;
        enable = false;
        StopAllCoroutines();
        thisimg.fillAmount = 0;
        if (null == thisPage) return;
        Destroy(thisPage);
    }

    public void SelectedThis()//选中该选项卡
    {
        //调整颜色
        // thisimg.color = enabledBkgColor;
        // thisTxt.color = enabledTxtColor;
        //显示页面
        enable = true;
        if (null == thisPage)
        {
            GameObject _thisPage = Resources.Load<GameObject>("Prefabs/" + Page);
            thisPage = Instantiate(_thisPage);
            thisPage.transform.SetParent(GameManager.Instance.UICanvas, false);
            //thisPage.transform.localPosition = Vector2.zero;
        }
        StopAllCoroutines();
        StartCoroutine(FillImg());
        //thisimg.fillAmount = 1;
        //页面置空


        thisPage.SetActive(true);
    }
    public void OnPointerClick(PointerEventData eventData)//按下按钮
    {
        if(enable)return;
        ts.DisableAll();
        SelectedThis();
    }
    private IEnumerator FillImg()//逐渐填充
    {
        while (thisimg.fillAmount < 1)
        {
            thisimg.fillAmount += 4 * Time.unscaledDeltaTime;
            yield return new WaitForEndOfFrame();
        }
        yield break;

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//选项卡
public class TabSelect : MonoBehaviour
{
    private TabItem[] tabs;//所有子物体
    private CanvasGroup cG;//画布组
    private RectTransform inspedtorWnd;//该窗口
    private Vector2 inspctrOrgnPos;//窗口默认位置
    private float hide_height = 50;//隐藏窗口的位置的高度

    //private bool popped;//本窗口是否已弹出
    //private float popSpeed = 3;//弹出速度
    //private PlayCharacter player;//获取玩家
    Vector2[] tabsPos;//记录每个选项按钮的初始位置
    // Use this for initialization
    void Awake()
    {
        cG = GetComponent<CanvasGroup>();
        inspedtorWnd = GetComponent<RectTransform>();
        //inspedtorWnd.localScale = Vector2.zero;
        inspctrOrgnPos = inspedtorWnd.anchoredPosition;
        //headPos = new Vector2(-560, 50);
        tabs = GetComponentsInChildren<TabItem>();
        tabsPos = new Vector2[tabs.Length];
        //选项卡初始化
        for (int i = 0; i < tabs.Length; i++)
        {
            //暂时让子选项卡回到零
            tabsPos[i] = tabs[i].transform.localPosition;
            tabs[i].transform.localPosition = new Vector2(0, 150);
            //print(tabs[i].name);
            //print("tabsPostion[" + i + "]" + tabsPos[i]);
            tabs[i].InitializeThis();
        }
        //得到玩家
        //player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayCharacter>();
    }

    public void SelectFirst()//选中第一个选项卡
    {
        tabs[0].SelectedThis();
    }
    public void DisableAll()//取消选择所有选项卡
    {
        for (int i = 0; i < tabs.Length; i++)
        {
            tabs[i].InSelectThis();
        }
    }

    public void ShowInspector()//弹出属性框
    {
        // if(gmCtrl.windowed)return;//如果已有窗口弹出
        // popped = true;
        // gmCtrl.PauseGame();//暂停游戏
        // gmCtrl.DisplayWindow(true);//告知窗口管理员

        //启用窗口
        inspedtorWnd.gameObject.SetActive(true);

        //全部禁用
        DisableAll();
        //print("弹出属性框");
        //player.HideStatus();//隐藏玩家的能量条和血条
        //开始动画
        StartCoroutine(Show());
    }
    public void HideInspector()//收回属性框
    {
        StopAllCoroutines();
        DisableAll();
        StartCoroutine(Hide());//收回窗口，继续游戏
        //player.ShowStatus();

    }

    IEnumerator Show()//弹出属性框
    {

        //移动背景到中央
        // panelImgBkg.gameObject.SetActive(true);
        // panelImgBkg.rectTransform.anchoredPosition = Vector2.zero;//背景
        //panelImgBkg.color = new Color(BkgColor.r, BkgColor.g, BkgColor.b, 0);//背景颜色暂时设为透明
        //调整层级
        // panelImgBkg.transform.SetAsLastSibling();
        //inspedtorWnd.transform.SetAsLastSibling();
        //inspedtorWnd.localScale = Vector2.right;

        //int i = 0;
        //int len = tabs.Length;
        inspedtorWnd.anchoredPosition = new Vector2(inspedtorWnd.anchoredPosition.x, hide_height);
        float time = Time.unscaledTime;
        float dtime = 0;

        float vcStep = 4 * 300 * Time.unscaledDeltaTime;//单步
        float insStep = 4 * (hide_height - inspctrOrgnPos.y) * Time.unscaledDeltaTime;
        //print("Vstep:" + vcStep);

        while (dtime <= 1)
        {
            dtime = (Time.unscaledTime - time) * 4;
            //移动属性框
            inspedtorWnd.anchoredPosition = Vector2.MoveTowards(inspedtorWnd.anchoredPosition, inspctrOrgnPos, insStep);
            //移动子选项卡
            for (int i = 0; i < tabs.Length; i++)
            {
                tabs[i].transform.localPosition =
                    Vector2.MoveTowards(tabs[i].transform.localPosition, tabsPos[i], vcStep);

            }
            cG.alpha = Mathf.Lerp(0, 1, dtime);//匀速动画
            yield return new WaitForEndOfFrame();

        }
        cG.blocksRaycasts = true;//子选项可点击
        SelectFirst();//选中第一个
        yield break;
    }

    IEnumerator Hide()//隐藏属性框
    {
        //移动背景到界面外
        // panelImgBkg.rectTransform.anchoredPosition = new Vector2(0, -720);//背景
        // panelImgBkg.gameObject.SetActive(false);
        cG.blocksRaycasts = false;//子选项不可点击
        float vcStep = 4 * 300 * Time.unscaledDeltaTime;//单步
        float insStep = 4 * (hide_height - inspctrOrgnPos.y) * Time.unscaledDeltaTime;

        //print("Vstep:" + vcStep);
        float time = Time.unscaledTime;
        float dtime = 0;
        while (dtime <= 1)
        {
            dtime = (Time.unscaledTime - time) * 4;
            //移动属性框
            inspedtorWnd.anchoredPosition = new Vector2(inspedtorWnd.anchoredPosition.x,
                                                        Mathf.MoveTowards(inspedtorWnd.anchoredPosition.y, hide_height, insStep));
            //选项卡动画
            for (int i = 0; i < tabs.Length; i++)
            {
                tabs[i].transform.localPosition =
                    Vector2.MoveTowards(tabs[i].transform.localPosition, new Vector2(0, 150), vcStep);

            }
            cG.alpha = Mathf.Lerp(cG.alpha, 0, dtime);
            yield return new WaitForEndOfFrame();
        }
        //禁用窗口
        inspedtorWnd.gameObject.SetActive(false);
        //gmCtrl.ContinueGame();//恢复游戏
        yield break;
    }
}

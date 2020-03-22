using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
//游戏控制，窗口管理，绑定在Panel上

public class GameControl : MonoBehaviour, IPointerClickHandler
{

    [SerializeField] private bool interactable = true;//可点击
    public UnityEvent onClickOrEsc;//点击事件
    GameObject[] Buttons; //暂停时禁用的组件，Tag为Button的物体
    TouchControl tchScrn; //触摸组件
    Image bkgImg; //背景
    private RectTransform gameOver; //游戏结束“是否复活”的窗口
    private RectTransform option; //游戏暂停时的操作窗口
    private TabSelect inspector;//玩家属性面板
    //ClickPanel clckPanel; //点击背景的脚本
    float animSpeed = 4; //动画速度
    Transform cvs;//画布
    //int wndwIndex;//窗口序号，用于辨别窗口，1代表操作窗口，2代表游戏结束窗口
    //enum GameState { gaming, paused, ended } //游戏状态判断
    [HideInInspector] public bool windowed = false; //是否已弹出窗口
    //GameState gameState;
    PlayCharacter player;//玩家
    private int indx;//层级管理
    ButtonNormal HeadIcon;//头像图标

    // Use this for initialization
    void Start()
    {
        tchScrn = GameObject.Find("Canvas/TouchControl").GetComponent<TouchControl>();
        bkgImg = GetComponent<Image>();
        //clckPanel = GetComponent<ClickPanel>();
        //添加事件
        onClickOrEsc.AddListener(DisplayWindow_Option);
        Time.timeScale = 1;
        //gameState = GameState.gaming;

        //获取玩家
        player = GameObject.FindObjectOfType<PlayCharacter>();
        HeadIcon = GameObject.Find("Canvas/Head").GetComponent<ButtonNormal>();
                //重新读取界面信息
        GameManager.Instance.loadCanvas();
        //获取画布
        cvs = GameManager.Instance.UICanvas;
    }
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))//Esc，安卓返回
        {
            ClickorEsc();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!interactable) return;
        //点击事件
        if (onClickOrEsc != null)
        {
            ClickorEsc();
        }

    }
    public void ClickorEsc()//执行事件
    {
        onClickOrEsc.Invoke();
    }
    public void DisplayWindow(RectTransform rctTrans, bool disableButtons, Color bkgColor) //窗口管理，弹出一个窗口，所有弹出窗口相关的方法都要先调用此方法
    {
        if (windowed) return;
        windowed = true;

        StopAllCoroutines();
        if (disableButtons)
        {
            //禁用触摸
            tchScrn.DisableTouchMove();
            tchScrn.EnableTouchControlView();
            //禁用所有按钮
            Buttons = GameObject.FindGameObjectsWithTag("Button"); //获取暂时禁用的组件
            StartCoroutine(DisableButtons());
        }
        //transform.SetAsLastSibling();//调整层级

        StartCoroutine(EnablePanel(bkgColor));
        StartCoroutine(DisplayWnd(rctTrans));
    }
    public void DisplayWindow(bool disableButtons, Color bkgColor) //窗口管理，未指定窗口，用于弹出本脚本以外的窗口，
    {
        if (windowed) return;
        windowed = true;

        StopAllCoroutines();
        if (disableButtons)
        {
            //禁用触摸
            tchScrn.DisableTouchMove();
            tchScrn.DisableTouchControlView();
            //禁用所有按钮
            Buttons = GameObject.FindGameObjectsWithTag("Button"); //获取暂时禁用的组件
            StartCoroutine(DisableButtons());
        }
        //ransform.SetAsLastSibling();//调整层级
        StartCoroutine(EnablePanel(bkgColor));
    }
    public void CloseWindow(RectTransform rctTrans, bool enableButtons) //关闭窗口，同上
    {
        windowed = false;
        //修改背景点击事件
        onClickOrEsc.RemoveAllListeners();
        onClickOrEsc.AddListener(DisplayWindow_Option);
        StopAllCoroutines();
        if (enableButtons)
        {
            //启用触摸
            tchScrn.EnableTouchMove();
            tchScrn.EnableTouchControlView();
            //启用所有按钮
            StartCoroutine(EnableButtons());
        }
        StartCoroutine(DisablePanel());
        StartCoroutine(CloseWnd(rctTrans));
    }
    public void CloseWindow(bool enableButtons) //关闭窗口，同上
    {
        windowed = false;
        //修改背景点击事件
        onClickOrEsc.RemoveAllListeners();
        onClickOrEsc.AddListener(DisplayWindow_Option);
        StopAllCoroutines();
        if (enableButtons)
        {
            //启用触摸
            tchScrn.EnableTouchMove();
            tchScrn.EnableTouchControlView();
            //启用所有按钮
            StartCoroutine(EnableButtons());
        }
        StartCoroutine(DisablePanel());

    }


    public void DisplayWindow_Option() //暂停游戏，弹出操作框
    {
        if (windowed) return;
        //游戏暂停
        PauseGame();
        //记录此时的层级，并调整层级
        indx = transform.GetSiblingIndex();
        transform.SetAsLastSibling();//显示在最上层

        if (null == option)
        {
            GameObject _option = Resources.Load<GameObject>("Prefabs/GameUI-Option");//读取
            //实例化
            option = Instantiate(_option).GetComponent<RectTransform>();

            option.transform.SetParent(cvs, false);
            //调整层级
            //option.SetAsLastSibling();
            //添加按钮事件
            //option.Find("Button_Back").GetComponent<ButtonNormal>().onClick.AddListener(BackToMenu);
            option.Find("Button_Cancel").GetComponent<ButtonNormal>().onClick.AddListener(CloseWindow_Option);
            option.Find("Button_Exit").GetComponent<ButtonNormal>().onClick.AddListener(QuitGame);
        }
        //调整层级
        option.SetAsLastSibling();
        //弹出窗口 
        DisplayWindow(option, true, new Color(1, 1, 1, 0.5f));//窗口颜色为半透白色
        HeadIcon.gameObject.SetActive(false);
        //修改背景点击事件
        onClickOrEsc.RemoveAllListeners();
        onClickOrEsc.AddListener(CloseWindow_Option);
    }

    public void DisplayWindow_Inspector()//显示玩家属性窗口
    {
        if (windowed) return;

        PauseGame();//暂停游戏

        if (null == inspector)//如果不存在，则读取
        {
            GameObject _inspector = Resources.Load<GameObject>("Prefabs/GameUI-Inspector");
            inspector = Instantiate(_inspector).GetComponent<TabSelect>();//实例化

            inspector.transform.SetParent(cvs, false);
            //调整层级
            inspector.transform.SetAsLastSibling();
        }
        //告知管理，禁用按钮
        DisplayWindow(true, new Color(1, 1, 1, 0.9f));//窗口颜色为半透明白色
        //单独调用属性窗口的弹出事件
        inspector.ShowInspector();
        player.HideStatus();//隐藏血条和能量条
        HeadIcon.HideButton(false);
        //修改背景点击事件
        onClickOrEsc.RemoveAllListeners();
        onClickOrEsc.AddListener(CloseWindow_Inspector);
    }
    public void DisplayWindow_GameOver() //弹出“确认复活”对话框
    {
        if (windowed) onClickOrEsc.Invoke();//如果有窗口弹出，则先关闭窗口
        //记录此时的层级，并调整层级
        indx = transform.GetSiblingIndex();
        transform.SetAsLastSibling();//显示在最上层

        if (null == gameOver)
        {
            GameObject _gameOver = Resources.Load<GameObject>("Prefabs/GameUI-GameOver");//读取
            //实例化
            gameOver = Instantiate(_gameOver).GetComponent<RectTransform>();

            gameOver.transform.SetParent(cvs, false);

            //添加按钮事件
            gameOver.Find("Button_Relive").GetComponent<ButtonNormal>().onClick.AddListener(onRelive);
            gameOver.Find("Button_GiveUp").GetComponent<ButtonNormal>().onClick.AddListener(BackToMenu);
            gameOver.Find("Button_Replay").GetComponent<ButtonNormal>().onClick.AddListener(ReplayGame);
        }
        //调整层级
        gameOver.SetAsLastSibling();
        DisplayWindow(gameOver, false, new Color(1, 1, 1, 0.8f));//窗口背景颜色为半透明黑色
        HeadIcon.HideButton();
        //StopAllCoroutines();
        //StartCoroutine(DisplayWnd(gameOver));
    }

    public void CloseWindow_Inspector()//关闭玩家属性窗口
    {
        CloseWindow(true);
        inspector.HideInspector();
        HeadIcon.ShowButton();//显示头像
        player.ShowStatus();//显示血条和能量条
        //ContinueGame();
        //修改背景点击事件
        // onClickOrEsc.RemoveAllListeners();
        // onClickOrEsc.AddListener(DisplayWindow_Option);
    }
    public void CloseWindow_Option() //关闭操作窗口
    {
        //关闭窗口
        CloseWindow(option, true);
        HeadIcon.gameObject.SetActive(true);
        //StartCoroutine(CloseWnd(option)); //包含恢复游戏相关方法

    }

    public void GameOver() //游戏结束，弹出复活角色的对话框
    {
        Buttons = GameObject.FindGameObjectsWithTag("Button"); //获取暂时禁用的组件
        //禁用按钮
        StopAllCoroutines();
        StartCoroutine(DisableButtons());
        //gameState = GameState.ended; //游戏结束
        tchScrn.DisableTouchMove(); //
        tchScrn.DisableTouchControlView(); //禁用触摸视野控制
        Invoke("DisplayWindow_GameOver", 1f); //1s后弹出对话框
        //清除背景点击事件
        //clckPanel.onClickorEsc.RemoveAllListeners ();
    }
    public void onRelive() //选择“复活”
    {
        //bkgImg.gameObject.SetActive(false);
        StopAllCoroutines();
        StartCoroutine(EnableButtons());
        StartCoroutine(CloseWnd(gameOver));
        tchScrn.EnableTouchControlView();
        tchScrn.EnableTouchMove();
        //修改背景点击事件
        onClickOrEsc.RemoveAllListeners();
        onClickOrEsc.AddListener(CloseWindow_Option);
    }
    public void PauseGame() //暂停游戏
    {
        player.StopMove();//玩家停止移动
        //暂停游戏
        Time.timeScale = 0;
        //gameState = GameState.paused;


    }
    public void ContinueGame() //继续游戏
    {
        //继续游戏
        Time.timeScale = 1;
        //gameState = GameState.gaming;
        player.StopMove();//玩家停止移动
    }
    public void ReplayGame() //重新开始
    {
        int index = SceneManager.GetActiveScene().buildIndex;
        GameManager.Instance.LoadSceneIndex(index);
    }
    public void BackToMenu() //返回主选单
    {
        Time.timeScale = 1;
        SceneManager.LoadSceneAsync(0);
    }
    public void QuitGame() //退出游戏
    {
        Application.Quit();
    }
    IEnumerator DisableButtons() //禁用按钮
    {

        for (int i = 0; i < Buttons.Length; i++)
        {
            Buttons[i].SetActive(false);
            yield return new WaitForEndOfFrame();
        }
        yield break;

    }
    IEnumerator EnableButtons() //启用按钮，
    {
        for (int i = 0; i < Buttons.Length; i++)
        {
            Buttons[i].SetActive(true);
            yield return new WaitForEndOfFrame();
        }
        yield break;
    }

    IEnumerator DisplayWnd(RectTransform rct) //动画效果，弹出对话框
    {
        //暂时禁用窗口上的内容
        foreach (Transform child in rct.transform)
        {
            child.localScale = Vector2.zero;
        }
        rct.anchoredPosition = Vector2.zero; //窗口归位
        // bkgRect.anchoredPosition = Vector2.zero; //背景归位
        //暂时隐藏窗口上的组件
        //float slideSpeed = 5;//滑动速度
        Image rctImg = rct.GetComponent<Image>(); //获取该窗口的IMAGE组件
        float time = Time.unscaledTime;
        float DelTime = 0;
        while (DelTime <= 1)
        {
            DelTime = (Time.unscaledTime - time) * animSpeed;
            // //改变背景透明度
            // float bkgA = Mathf.Lerp(0, bkgAlpha, DelTime);
            // bkgImg.color = new Color(bkgImg.color.r, bkgImg.color.g, bkgImg.color.b, bkgA);
            //窗口缩放
            float S = Mathf.Lerp(0.5f, 1, DelTime);
            rct.localScale = Vector2.one * S;
            //窗口旋转
            float R = Mathf.Lerp(90, 0, DelTime);
            rct.eulerAngles = new Vector3(0, 0, R);
            //边框渐变
            float F = Mathf.Lerp(0, 1, DelTime);
            rctImg.fillAmount = F;
            //窗口渐显
            float A = Mathf.Lerp(0, 1, DelTime);
            rctImg.color = new Color(rctImg.color.r, rctImg.color.g, rctImg.color.b, A);

            yield return new WaitForEndOfFrame();
        }
        //启用窗口上的内容
        foreach (Transform child in rct.transform)
        {
            child.localScale = Vector2.one;
        }

        yield break;
    }
    IEnumerator CloseWnd(RectTransform rct) //动画效果，缩回对话框
    {
        //暂时禁用窗口上的内容
        foreach (Transform child in rct.transform)
        {
            child.localScale = Vector2.zero;
        }
        interactable = false; //禁用背景点击
        Image rctImg = rct.GetComponent<Image>(); //获取该窗口的IMAGE组件
        float time = Time.unscaledTime;
        float DelTime = 0;
        while (DelTime <= 1)
        {
            DelTime = (Time.unscaledTime - time) * animSpeed;
            // //改变背景透明度
            // float bkgA = Mathf.Lerp(bkgAlpha, 0, DelTime);
            // bkgImg.color = new Color(bkgImg.color.r, bkgImg.color.g, bkgImg.color.b, bkgA);
            //窗口缩小
            float S = Mathf.Lerp(1, 0.5f, DelTime);
            rct.localScale = Vector2.one * S;
            //窗口旋转
            float R = Mathf.Lerp(0, -180, DelTime);
            rct.eulerAngles = new Vector3(0, 0, R);
            //边框渐变
            //float F = Mathf.Lerp(1, 0, DelTime);
            //rctImg.fillAmount = F;
            //窗口渐隐
            float A = Mathf.Lerp(1, 0, DelTime);
            rctImg.color = new Color(rctImg.color.r, rctImg.color.g, rctImg.color.b, A);
            yield return new WaitForEndOfFrame();
        }

        //将窗口移到界面外
        rct.anchoredPosition = new Vector2(0, 720);
        //恢复层级
        transform.SetSiblingIndex(indx);

        yield break;
    }
    IEnumerator EnablePanel(Color paneColor)//启用背景
    {
        bkgImg.rectTransform.anchoredPosition = Vector2.zero; //背景归位
        float a = paneColor.a;//得到透明度
        bkgImg.color = paneColor - new Color(0, 0, 0, a);//暂时先变为透明
        float time = Time.unscaledTime;
        float DelTime = 0;
        while (DelTime <= 1)
        {
            DelTime = (Time.unscaledTime - time) * animSpeed;
            //改变背景透明度
            float bkgA = Mathf.Lerp(0, a, DelTime);
            bkgImg.color = new Color(bkgImg.color.r, bkgImg.color.g, bkgImg.color.b, bkgA);
            yield return new WaitForEndOfFrame();
        }
        interactable = true; //启用点击
        yield break;
    }
    IEnumerator DisablePanel()//禁用背景
    {
        interactable = false; //禁用点击
        float a = bkgImg.color.a;
        float time = Time.unscaledTime;
        float DelTime = 0;
        while (DelTime <= 1)
        {
            DelTime = (Time.unscaledTime - time) * animSpeed;
            //改变背景透明度
            float bkgA = Mathf.Lerp(a, 0, DelTime);
            bkgImg.color = new Color(bkgImg.color.r, bkgImg.color.g, bkgImg.color.b, bkgA);
            yield return new WaitForEndOfFrame();
        }
        bkgImg.rectTransform.anchoredPosition = 1920 * Vector2.down;

        HeadIcon.EnableButton();//强制启用按钮
        ContinueGame();
        yield break;
    }
}
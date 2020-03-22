using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
//游戏主菜单
//主菜单的四个按钮作为该空物体的子物体
//按钮整体布局边界矩形长宽比严格约束为16:10
//
public class MainMenu : MonoBehaviour {
    [SerializeField]
    private int FirstScene = 1;
    private int sign = 0;//判断当前已弹出的窗口的标志，1-设置，2-回忆，3-关于，4-确认退出
    Image panel;
    Transform cvs;//画布
    public Color bkgColor;//背景颜色

    GameObject drawer;//画板

    RectTransform btnRect;//按钮

    RectTransform settings;//设置窗口
    //Vector2 stngsOrgnPos;//设置窗口初始位置
    RectTransform loadGame;//读取游戏
    RectTransform aboutGame;//关于
    RectTransform cnfrmExit;//确认退出

    Transform ink;//墨滴图片
    //Image 
    CanvasGroup BtnCg;//渐隐掩盖
    AudioSource audioSrc;//音频组件
    //Slider sld;

    [SerializeField]float animSpeed = 5;//动画速度
    void Start()
    {
        //本身组件
        btnRect = GetComponent<RectTransform>();
        BtnCg = btnRect.GetComponent<CanvasGroup>();
        //画布
        cvs = GameManager.Instance.UICanvas;
        if(null == cvs)cvs = GameObject.Find("Canvas").transform;
        panel = GameObject.Find("Canvas/Panel").GetComponent<Image>();
        //窗口
        //settings = GameObject.Find("Canvas/Window-Settings").GetComponent<RectTransform>();
        //cnfrmExit = GameObject.Find("Canvas/Window-CnfrmExit").GetComponent<RectTransform>();
        //loadGame = GameObject.Find("Canvas/Window-LoadGame").GetComponent<RectTransform>();
        //stngsOrgnPos = settings.anchoredPosition;
        //音量
        audioSrc = GetComponent<AudioSource>();
        //sld = GameObject.Find("Canvas/Window-Settings/Slider").GetComponent<Slider>();
        //同步页面
        //dsplyldscn = GameObject.Find("Canvas/LoadScene").GetComponent<LoadScenePanel>();
        //dsplyldscn.gameObject.SetActive(false);
        //子物体
        ink = transform.Find("Ink");
        panel.gameObject.SetActive(false);//游戏开始禁用Panel

        drawer = GameObject.Find("Canvas/Drawer");
        GameManager.Instance.loadCanvas();
        GameManager.Instance.PlayBGM();
        drawer.SetActive(GameManager.Instance.paintOnMainMenu);

    }
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))//ESE，安卓返回
        {
            if (sign > 0) WithdrawWds();
            else ConfirmExit();
        }
    }
    public void SetPaint(bool paintEnable)//设置画板是否启用
    {
        GameManager.Instance.paintOnMainMenu = paintEnable;
        drawer.SetActive(paintEnable);
    }
    public void StartGame()//开始游戏
    {
        if(sign>0) return;
        drawer.SetActive(false);
        GameManager.Instance.LoadSceneIndex(FirstScene);
    }
    public void Settings()//设置
    {
        if(sign>0) return;
        if(null == settings)//如果没有，则读取resource
        {
            GameObject stsTemp = Resources.Load<GameObject>("Prefabs/MainMenu-Settings");
            settings = Instantiate(stsTemp).GetComponent<RectTransform>();
            settings.transform.SetParent(cvs,false);//设定为画布的子物体

            //添加事件
            Toggle tg = settings.GetComponentInChildren<Toggle>();
            tg.isOn = GameManager.Instance.paintOnMainMenu;
            tg.onValueChanged.AddListener(SetPaint);
            ButtonSlider bs = settings.GetComponentInChildren<ButtonSlider>();
            //print(bs.name);
            bs.SetValue(GameManager.Instance.volume_BGM);
            bs.onValueChanged.AddListener(GameManager.Instance.SetBGMVolume);
            //bs.onValueChanged.AddListener(GameManager.Instance.SetAudioVolume);
        }
        StartCoroutine(Appear(settings));
        sign = 1;
    }
    public void LoadGame()//读取游戏存档
    {
        if(sign>0) return;
        if(null == loadGame)//如果没有，则读取resource
        {
            GameObject ldgTemp = Resources.Load<GameObject>("Prefabs/MainMenu-LoadGame");
            loadGame = Instantiate(ldgTemp).GetComponent<RectTransform>();
            loadGame.transform.SetParent(cvs,false);//设定为画布的子物体
        }
        StartCoroutine(Appear(loadGame));
        sign = 2;
    }
    public void ConfirmExit()//确认退出
    {
        if(sign>0) return;
        if(null == cnfrmExit)//如果没有，则读取resource
        {
            GameObject cfeTemp = Resources.Load<GameObject>("Prefabs/MainMenu-CnfrmExit");
            cnfrmExit = Instantiate(cfeTemp).GetComponent<RectTransform>();
            cnfrmExit.transform.SetParent(cvs,false);//设定为画布的子物体
            //设置按钮事件
            cnfrmExit.transform.Find("Yes").GetComponent<ButtonNormal>().onClick.AddListener(ExitGame);
            cnfrmExit.transform.Find("No").GetComponent<ButtonNormal>().onClick.AddListener(WithdrawWds);
        }
        StartCoroutine(Appear(cnfrmExit));
        sign = 4;
    }
    public void ExitGame()//退出游戏
    {
        //StopAllCoroutines();
        Application.Quit();
    }
    public void WithdrawWds()//撤回窗口
    {
        StopAllCoroutines();
        switch (sign){
            case 1:
                StartCoroutine(Disappear(settings));
                break;
            case 2:
                 StartCoroutine(Disappear(loadGame));;
                break;
            case 3:
                 ;
                break;
            case 4:
                StartCoroutine(Disappear(cnfrmExit));
                break;
            default:
                break;

        }
        sign = 0;
        
    }

    IEnumerator HideButons()//动画效果，隐藏按钮
    {
        ink.gameObject.SetActive(true);
        Image inkImg = ink.GetComponent<Image>();//获取IMG以改变颜色
        RectTransform inkRect = ink.GetComponent<RectTransform>();// 按钮的矩形组件以改变大小
        
        float time = Time.unscaledTime;
        float dtime = 0;
        while (dtime < 1)
        {
            dtime = (Time.unscaledTime - time) * animSpeed;
            //按钮缩小
            float S = Mathf.Lerp(btnRect.localScale.x, 0.2f, dtime);
            btnRect.localScale = Vector2.one * S;
            //按钮旋转
            //float R = Mathf.Lerp(0, -90, dtime);
            //buttonsTrans.eulerAngles = new Vector3(0, 0, R);
            //墨滴渐显
            float A = Mathf.Lerp(0, 1, dtime);
            inkImg.color = new Color(inkImg.color.r, inkImg.color.g, inkImg.color.b, A);

            yield return new WaitForEndOfFrame();
        }
        btnRect.anchoredPosition = new Vector2(0, 500);//移出到界面外
        yield break;
    }
    IEnumerator ShowButtons()//动画效果，显示按钮
    {

        btnRect.anchoredPosition = new Vector2(0, 0);//移动到界面内

        Image inkImg = ink.GetComponent<Image>();//获取IMG以改变颜色
        //RectTransform inkRect = ink.GetComponent<RectTransform>();//矩形组件以改变大小

        float time = Time.unscaledTime;
        float dtime = 0;
        while (dtime < 1)
        {
            dtime = (Time.unscaledTime - time) * animSpeed;
            //按钮缩小
            float S = Mathf.Lerp(btnRect.localScale.x, 1, dtime);
            btnRect.localScale = Vector2.one * S;
            //按钮旋转
            //float R = Mathf.Lerp(-90, 0, dtime);
            //buttonsTrans.eulerAngles = new Vector3(0, 0, R);
            //墨滴渐显
            float A = Mathf.Lerp(1, 0, dtime);
            inkImg.color = new Color(inkImg.color.r, inkImg.color.g, inkImg.color.b, A);

            yield return new WaitForEndOfFrame();
        }
        ink.gameObject.SetActive(false);
        yield break;
    }
    IEnumerator Appear(RectTransform rct)//动画效果，弹出对话框
    {
        //将按钮移除到界面外
        btnRect.localScale = Vector2.zero;
        btnRect.anchoredPosition = new Vector2(0, 540);
        rct.anchoredPosition = Vector2.zero;//窗口归位
        //暂时隐藏窗口上的组件
        foreach (Transform child in rct.transform)
        {
            child.localScale = Vector2.zero;
        }
        //float slideSpeed = 5;//滑动速度
        Image rctImg = rct.GetComponent<Image>();//获取该窗口的IMAGE组件
        rctImg.fillClockwise = true;//调整为顺时针
        float time = Time.unscaledTime;
        float DelTime = 0;
        while (DelTime <= 1)
        {
            DelTime = (Time.unscaledTime - time) * animSpeed;
            //改变背景透明度
            //float A = Mathf.Lerp(0, bkgColor.a, DelTime);
            //panel.color = new Color(panel.color.r, panel.color.g, panel.color.b, A);
            //滑出窗口
            //float Y = Mathf.Lerp(stngsOrgnPos.y, 0, DelTime);
            //rct.anchoredPosition = new Vector2(0, Y);
            //墨滴渐隐

            //窗口移动
            //float X = Mathf.Lerp(200,0,DelTime);
            //rct.anchoredPosition = new Vector2(X, 0);

            //按钮渐隐
            //float cA = Mathf.Lerp(1, 0, DelTime);
            //BtnCg.alpha = cA;
            //按钮旋转
            //float btnR = Mathf.Lerp(0, -180, DelTime);
            //btnRect.eulerAngles = new Vector3(0, 0, btnR);//按钮旋转
            //按钮缩放
            //float btnS = Mathf.Lerp(1, 0, DelTime);
            //btnRect.localScale = new Vector2(btnS, btnS);
            //窗口放大
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
        
        //启用窗口上的组件
        foreach (Transform child in rct.transform)
        {
            child.localScale = Vector2.one;
        }
        //启用背景
        panel.gameObject.SetActive(true);
        //panel.raycastTarget = true;
        yield break;
    }
    IEnumerator Disappear(RectTransform rct)//动画效果，缩回对话框
    {
        panel.gameObject.SetActive(false);//禁用背景
        //暂时禁用窗口上的组件
        foreach (Transform child in rct.transform)
        {
            child.localScale = Vector2.zero;
        }

        Image rctImg = rct.GetComponent<Image>();//获取该窗口的IMAGE组件
        rctImg.fillClockwise = true;//调整为逆时针
        float time = Time.unscaledTime;
        float DelTime = 0;
        while (DelTime <= 1)
        {
            DelTime = (Time.unscaledTime - time) * animSpeed;
            //改变背景透明度
            //float A = Mathf.Lerp(bkgColor.a, 0, DelTime);
            //panel.color = new Color(panel.color.r, panel.color.g, panel.color.b, A);
            //收回设置窗口
            //float Y = Mathf.Lerp(0, stngsOrgnPos.y, DelTime);
            //rct.anchoredPosition = new Vector2(0, Y);

            //按钮渐隐
            //float cA = Mathf.Lerp(0, 1, DelTime);
            //BtnCg.alpha = cA;
            //按钮旋转
            //float btnR = Mathf.Lerp(-180, 0, DelTime);
            //btnRect.eulerAngles = new Vector3(0, 0, btnR);//按钮旋转
            //按钮缩放
            //float btnS = Mathf.Lerp(0, 1, DelTime);
            //btnRect.localScale = Vector2.one * btnS;
            //窗口缩小
            float S = Mathf.Lerp(1, 0.5f, DelTime);
            rct.localScale = Vector2.one * S;
            //窗口旋转
            float R = Mathf.Lerp(0, -135, DelTime);
            rct.eulerAngles = new Vector3(0, 0, R);
            //边框渐变
            //float F = Mathf.Lerp(1, 0, DelTime);
            //rctImg.fillAmount = F;
            //窗口渐隐
            float A = Mathf.Lerp(1, 0, DelTime);
            rctImg.color = new Color(rctImg.color.r, rctImg.color.g, rctImg.color.b, A);
            yield return new WaitForEndOfFrame();
        }
        //按钮归位
        btnRect.localScale = Vector2.one;
        btnRect.anchoredPosition = Vector2.zero;
        //将窗口移到界面外
        rct.anchoredPosition = new Vector2(0, 640);
        
        yield break;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//游戏管理器，存储设备信息、场景信息、游戏内容等
public class GameManager : MonoBehaviour
{
    //UI管理
    [SerializeField] private GameObject panel;//遮住UI
    [SerializeField] private LoadingScene loadS; //读取场景的页面
    public Transform UICanvas; //画布物体
    public Vector2 canvasSize, screenSize; //画布大小，屏幕分辨率
    public float wRatio, hRatio; //画布与屏幕的坐标比例
    public int uiCount; //UI元素的个数
    public bool paintOnMainMenu, paintOnLoading = false;//界面涂鸦
    //声音管理
    public float volume_BGM = 1, volume_Audio = 1;//背景音乐、音效的音量
    private AudioSource audioSource_BGM, audioSource_Clip;//声音管理
    private AudioClip audio_Bgm, audio_Click;//背景音乐，点击音效
    //消息提示
    private TipMessage tipMessage;


    //单例
    private static GameManager _Instance;
    public static GameManager Instance
    {
        get
        {
            if (null == _Instance)
            {
                //寻找场景中的物体，若没有物体，则创建
                GameObject gameManage = GameObject.Find("gameManager");
                if (null == gameManage)
                {
                    gameManage = new GameObject("gameManager");
                    //添加画布组件
                    Canvas c = gameManage.AddComponent<Canvas>();
                    //设置参数
                    c.renderMode = RenderMode.ScreenSpaceOverlay;
                    c.sortingOrder = 1;
                    gameManage.AddComponent<AudioSource>();
                }
                DontDestroyOnLoad(gameManage); //不随场景切换而消失
                //实例化本类
                _Instance = gameManage.GetComponent<GameManager>();
                if (null == _Instance)
                {
                    _Instance = gameManage.AddComponent<GameManager>();
                }
                //_Instance = FindObjectOfType(typeof(LoadingScene)) as LoadingScene;

            }
            return _Instance;
        }

    }
    void Awake()
    {
        loadCanvas();
    }
    // Use this for initialization
    // void Start()
    // {

    //     loadCanvas();
    // }

    //获取游戏场景信息
    public void loadCanvas()
    {
        UICanvas = GameObject.Find("Canvas").transform; //主画布
        //print ("UI Canvas: " + UICanvas.name);
        canvasSize = UICanvas.GetComponent<RectTransform>().sizeDelta; //获取画布尺寸
        screenSize = new Vector2(Screen.width, Screen.height); //获取屏幕分辨率
        //获取坐标转换比例
        wRatio = canvasSize.x / screenSize.x;
        hRatio = canvasSize.y / screenSize.y;
        //获取UI的子物体数量
        uiCount = UICanvas.childCount;

        // print ("UI Size: " + canvasSize);
        // print ("Screen: " + screenSize);
        // print ("wRatio: " + wRatio + ", hRatio: " + hRatio);
    }
    public void setPaintOnMainMenu(bool paint)
    {
        paintOnMainMenu = paint;
    }
    public void setPaintOnLoading(bool paint)
    {
        paintOnLoading = paint;
        loadS.SetPaint(paint);
    }
    public void SetAudioVolume(float v)//设置音效的音量
    {
        audioSource_Clip.volume = v;
        volume_Audio = v;
    }
    public void SetBGMVolume(float v)//设置背景音乐的音量
    {
        audioSource_BGM.volume = v;
        volume_BGM = v;
    }
    public void PlayAudio()//播放音效
    {
        if (null == audio_Click) audio_Click = Resources.Load<AudioClip>("Audios/Audio-ButtonClick");
        if (null == audioSource_Clip)
        {
            audioSource_Clip = gameObject.AddComponent<AudioSource>();
        }

        audioSource_Clip.clip = audio_Click;//赋值
        audioSource_Clip.volume = volume_Audio;//设置音量
        audioSource_Clip.Play();
    }
    public void PlayBGM()//播放背景音乐
    {
        if (null == audio_Bgm) audio_Bgm = Resources.Load<AudioClip>("Audios/Bgm-MainMenu");

        if (null == audioSource_BGM)
        {
            audioSource_BGM = gameObject.AddComponent<AudioSource>();
        }

        audioSource_BGM.clip = audio_Bgm;//赋值
        audioSource_BGM.volume = volume_BGM;
        audioSource_BGM.Play();
    }
    public void StopBGM(float duration)//停止播放背景音乐
    {
        StartCoroutine(GradualStopAudio(duration));
    }
    IEnumerator GradualStopAudio(float duration)//声音淡出
    {
        volume_BGM = audioSource_BGM.volume;
        float time = Time.unscaledTime;
        float dtime = 0;
        while (dtime <= 1)
        {
            dtime = (Time.unscaledTime - time) / duration;
            audioSource_BGM.volume = Mathf.Lerp(volume_BGM, 0, dtime);//声音逐渐减小
            yield return null;
        }
        audioSource_BGM.Stop();//停止播放声音
        audioSource_BGM.volume = volume_BGM;//恢复音量
        yield break;
    }

    public void Tip(string tipMsg)//提示消息
    {
        if (null == tipMessage)//如果未实例化
        {
            Transform _tm = Resources.Load<Transform>("Prefabs/UIElement-TipMessage");
            Transform tm = Instantiate(_tm);
            tm.SetParent(UICanvas, false);
            tipMessage = tm.GetComponent<TipMessage>();
        }
        tipMessage.TipWords(tipMsg, 1f);
    }
    public void Tip(string tipMsg, float offtime)//提示消息，指定时间
    {
        if (null == tipMessage)//如果未实例化
        {
            Transform _tm = Resources.Load<Transform>("Prefabs/UIElement-TipMessage");
            Transform tm = Instantiate(_tm);
            tm.SetParent(UICanvas, false);
            tipMessage = tm.GetComponent<TipMessage>();
        }
        tipMessage.TipWords(tipMsg, offtime);
    }

    //读取场景
    public void LoadSceneIndex(int i)
    {

        //如果没有遮罩
        if (null == panel)
        {
            panel = Resources.Load<GameObject>("Prefabs/GameUI-Panel");
        }
        Transform _panel = Instantiate(panel).transform;
        _panel.transform.SetParent(UICanvas, false);
        _panel.transform.SetAsLastSibling();
        if (null == loadS) //如果场景中没有该页面，则重新生成
        {
            //string path = Application.dataPath + "Prefabs/UI/LoadScenePanel";
            GameObject _LoadS = Resources.Load<GameObject>("Prefabs/GameUI-LoadingScene");
            //print("Loaded name of game object: " + _LoadS.name);
            loadS = Instantiate(_LoadS).GetComponent<LoadingScene>();
            loadS.transform.SetParent(this.transform, false);
            loadS.SetPaint(paintOnLoading);
            //loadS.transform.localPosition = Vector2.zero;
        }
        loadS.gameObject.SetActive(true);
        //LoadingScene loadS = Instantiate(loadScenePanel);//实例化预制体
        loadS.LoadSceneIdx(i);
        //loadCanvas ();
    }
}
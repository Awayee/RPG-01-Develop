using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//使用协程实现加载进度条
public class LoadingScene : MonoBehaviour {

    private CanvasGroup progressBkg; //背景条
    private Image thisImg; //本对象
    private Image progressImg; //进度条
    private Text progressTxt; // 文字显示加载进度   
    //private float a_img, a_txt, a_bkg;//记录透明度
    [Range (0, 1)] private float opratingPrgrs = 0; // 加载进度(由于加载进度不能为 1，所以需要此变量在加载进度大于某一个值时让加载进度变为1)
    [Range (0, 100)] private int intCurrentPrgrs; //当前进度
    [Range (0, 5)] public float inDuration = 0.5f, outDuration = 1; //过场延时

    private GameObject painting;//画板
    void Awake () {
        //获取组件
        progressBkg = transform.Find ("ProgressBkg").GetComponent<CanvasGroup> ();
        progressImg = progressBkg.transform.Find("ProgressImg").GetComponent<Image>();
        progressTxt = progressBkg.transform.Find("ProgressTxt").GetComponent<Text>();
        painting = transform.Find("Paint").gameObject;
        //得到透明度
        //a_img = progressImg.color.a;
        //a_txt = progressTxt.color.a;
        //a_bkg = progressBkg.color.a;
        thisImg = GetComponent<Image> ();
        //transform.localPosition = Vector2.zero;
        //调整层级
        // transform.SetParent(GameObject.Find("Canvas").transform);//设置为画布的子物体
        // transform.localPosition = Vector2.zero;//定位
        // transform.SetAsLastSibling();//设置为最末的层级已确保显示在UI最上层

        //缩放
        float wR = GameManager.Instance.wRatio;
        float hR = GameManager.Instance.hRatio;
        //print (wR + "," + hR);
        //RectTransform thisRect = thisImg.rectTransform;

        transform.localScale = new Vector2 (transform.localScale.x / wR,
            transform.localScale.y / hR);

    }
    public void LoadSceneIdx (int indx) {
        intCurrentPrgrs = 0;
        progressImg.fillAmount = 0;
        StopAllCoroutines ();
        StartCoroutine (LoadSceneTask (indx)); //开启协程
    }

    // 异步加载场景
    private IEnumerator LoadSceneTask (int idx) {
        yield return StartCoroutine (PanelFadeIn (inDuration)); //等待页面渐显
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync (idx); //加载场景操作
        asyncOperation.allowSceneActivation = false; //不允许场景立即激活
        opratingPrgrs = 0;
        while (opratingPrgrs < 1) {

            opratingPrgrs = asyncOperation.progress + 0.1f; // progress的值最大为0.9
            yield return LoadingProgress ();
        }
        //print("loading progerss:" + asyncOperation.progress);
        //opratingPrgrs = 1; //进度设为1，继续推动进度条
        //yield return LoadingProgress();
        asyncOperation.allowSceneActivation = true; //激活场景 —— 跳转场景成功
        
        GameManager.Instance.StopBGM(2*outDuration);//停止背景音乐
        yield return StartCoroutine (PanelFadeOut (outDuration)); //等待该页面逐渐消失
       
        //场景读取完成后执行
        //GameManager.Instance.loadCanvas ();//重新读取场景信息
        gameObject.SetActive (false); //禁用面板
        yield break;
    }

    //进度条变换
    private IEnumerator LoadingProgress () {
        int x = (int) (1 + opratingPrgrs * 1.5f);
        while (intCurrentPrgrs < opratingPrgrs * 100) //当前进度 < 目标进度时
        {
            //if(opratingPrgrs >= 0.9f)x++;
            //print("int X:" + x);
            intCurrentPrgrs += x; //当前进度不断累加 （如果场景很小，可以调整这里的值 例如：+=10 +=20，来调节加载速度）
            if (intCurrentPrgrs > 100) intCurrentPrgrs = 100;
            //显示在UI上   
            progressTxt.text = intCurrentPrgrs.ToString () + " %";
            progressImg.fillAmount = (float) intCurrentPrgrs / 100; //显示进度
            yield return new WaitForEndOfFrame(); //等一帧
        }

    }
    //设置画板是否启用
    public void SetPaint(bool paintEnable)
    {
        painting.SetActive(paintEnable);
    }

    //该页面渐显示
    private IEnumerator PanelFadeIn (float duration) {
        RawImage msk = transform.GetChild(0).GetComponent<RawImage>();
        ResetProgress();//重置进度条
        float t = Time.unscaledTime;
        float dtime = 0;
        while (dtime <= 1) {
            dtime = (Time.unscaledTime - t)/duration;
            float a = Mathf.Lerp (0, 1, dtime);
            thisImg.color = new Color (thisImg.color.r, thisImg.color.g, thisImg.color.b, a);
            msk.color = new Color(msk.color.r, msk.color.g, msk.color.b, 0.5f * a);
            progressBkg.alpha = a;
            yield return null;
        }
        yield break;
    }

    //该页面渐渐消失
    private IEnumerator PanelFadeOut (float duration) {
        RawImage msk = transform.GetChild(0).GetComponent<RawImage>();
        float t = Time.unscaledTime;
        float dtime = 0;
        while (dtime <= 1) {
            dtime = (Time.unscaledTime - t)/duration;
            float a = Mathf.Lerp (1, 0, dtime);
            thisImg.color = new Color (thisImg.color.r, thisImg.color.g, thisImg.color.b, a);
            progressBkg.alpha = Mathf.Lerp(progressBkg.alpha, 0, dtime);
            msk.color = new Color(msk.color.r, msk.color.g, msk.color.b, 0.5f * a);
            yield return null;
        }
        yield break;
    }
    void ResetProgress()//初始化进度条
    {
        progressImg.fillAmount = 0;
        progressTxt.text = "0 %";

    }

}
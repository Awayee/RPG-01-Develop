using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
//拥有残留效果的画笔，效果不太理想
public class Painting_Line : MonoBehaviour {

    private RenderTexture texRender; //画布
    public Material mat; //给定的shader新建材质
    public Material mat_eraser;//橡皮擦
    public Texture brushTypeTexture; //画笔纹理，半透明
    public Texture clearBrush; //橡皮擦
    //private Camera mainCamera;
    private float brushScale = 0.5f; //当前
    public Color brushColor = Color.black;
    private RawImage rawImg; //使用UGUI的RawImage显示，方便进行添加UI,将pivot设为(0.5,0.5)
    private float lastDistance;
    //private Vector2[] PositionArray = new Vector2[3];

    List<Vector2>[] linePoints = new List<Vector2>[10]; //每一条线上的点
    private int[] a = new int[10];
    private Vector2[, ] PositionArray1 = new Vector2[4, 10];
    private int[] b = new int[10];
    private float[, ] speedArray = new float[4, 10];
    private int[] s = new int[10];
    public int num = 50;
    //Vector2 rawImgSize; //该图片尺寸

    //Vector2 rawMousePosition; //raw图片的左下角对应鼠标位置
    float rawWidth; //raw图片宽度
    float rawHeight; //raw图片长度
    void Start () {

        //raw图片鼠标位置，宽度计算
        rawImg = GetComponent<RawImage> (); //获取本图片
        rawWidth = rawImg.rectTransform.sizeDelta.x;
        rawHeight = rawImg.rectTransform.sizeDelta.y;
        Vector2 rawanchorPositon = new Vector2 (rawImg.rectTransform.anchoredPosition.x - rawImg.rectTransform.sizeDelta.x / 2.0f, rawImg.rectTransform.anchoredPosition.y - rawImg.rectTransform.sizeDelta.y / 2.0f);
        //rawMousePosition = rawanchorPositon + new Vector2 (Screen.width / 2.0f, Screen.height / 2.0f);
        //rawImgSize = rawImg.rectTransform.sizeDelta;//得到图片尺寸
        texRender = new RenderTexture (Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);
        Clear (texRender);
        Input.multiTouchEnabled = true; //开启多点触控

        for (int i = 0; i < 10; i++) {
            linePoints[i] = new List<Vector2> ();
        }
    }

    //最多支持10点触控
    Vector2[] startPosition = new Vector2[10];
    Vector2[] endPosition = new Vector2[10];
    void Update () {
        if (Input.touchCount == 0) return;
        //记录触控点
        int tCount = Input.touchCount;
        Touch[] inputTouches = Input.touches;

        //遍历每个点
        for (int i = 0; i <= tCount - 1; i++) {
            //-------------------开始触控---------------------------------
            if (inputTouches[i].phase == TouchPhase.Began) {

                //记录起始点
                int fID = inputTouches[i].fingerId; //利用FingerID来辨识触控点
                a[fID] = 0;
                b[fID] = 0;
                s[fID] = 0;
                startPosition[fID] = inputTouches[i].position;
                endPosition[fID] = inputTouches[i].position;
                //记录经过的点
                //linePoints.Add(endPosition[fID]);
            }
            //-------------------悬停---------------------------------
            else if (inputTouches[i].phase == TouchPhase.Stationary) {
                int fID = inputTouches[i].fingerId; //利用FingerID来辨识触控点
                startPosition[fID] = inputTouches[i].position;
                endPosition[fID] = inputTouches[i].position;
            }
            //-------------------移动---------------------------------
            else if (inputTouches[i].phase == TouchPhase.Moved) {
                int fID = inputTouches[i].fingerId; //利用FingerID来辨识触控点
                endPosition[fID] = inputTouches[i].position;

                float distance = Vector3.Distance (endPosition[fID], startPosition[fID]);
                brushScale = SetScale (distance);
                //画出贝第i条塞尔曲线
                ThreeOrderBézierCurse (endPosition[fID], distance, 4.5f, fID);
                //startPosition = endPosition;
                lastDistance = distance;
                DrawImage ();

            }

            //-------------------离开---------------------------------
            else if (inputTouches[i].phase == TouchPhase.Ended) {
                //当前手指下的线条消失
                //ThreeOrderBézierCurse(endPosition[i], distance, 4.5f, i)
                // OnMouseUp();

                //清除这些点//DrawBrush(texRender,new Rect(linePoints[l].x, linePoints[l].y, brushTypeTexture.width, brushTypeTexture.height),Texture.,new Color(0,0,0,0),5);

                int fID = inputTouches[i].fingerId; //利用FingerID来辨识触控点

                StartCoroutine (ClearPoints (fID));

                //重置点
                startPosition[fID] = Vector2.zero;
                endPosition[fID] = Vector2.zero;
                //brushScale = 0.5f;
                a[fID] = 0;
                b[fID] = 0;
                s[fID] = 0;
                //speedArray[i] = 0;

                //linePoints[i].Clear ();
            }

        }

    }
    IEnumerator ClearPoints (int i) //清除一条线上的点
    {
        List<Vector2> pointList = linePoints[i]; //先赋值
        //linePoints[i].Clear ();

        int count = pointList.Count; //记录长度
        int idx = 0;
        int step = Mathf.CeilToInt (Time.deltaTime * count / 2); //2s后消失
        print ("Count" + count + ", Step:" + step);

        float time = Time.time;
        float dt = 0;
        while (idx < count) //逐帧清除
        {

            //dt = Time.time-time;
            if (idx < count) {
                print ("clearing");
                ClearPaint (pointList[idx], texRender, Color.white);
                //pointList.RemoveAt(idx);
                //count--;
            }

            idx += step;
            //_color -= new Color (0, 0, 0, alp_step);
            yield return null;
        }
        //剩下的逐渐消失
        Color _color = Color.white;
        time = Time.time;
        dt = 0;
        while (dt <= 1) {
            dt = (Time.time - time);
            for (idx = 0; idx < pointList.Count; idx++) {
                ClearPaint (pointList[idx], texRender, new Color (1, 1, 1, dt));
            }
            yield return null;
        }
        yield break;

    }
    void DrawImage () {
        rawImg.texture = texRender;
    }
    public void OnClickClear () {
        Clear (texRender);
    }

    // void OnMouseUp()
    // {
    //     startPosition = Vector3.zero;
    //     //brushScale = 0.5f;
    //     a = 0;
    //     b = 0;
    //     s = 0;
    // }
    //设置画笔宽度
    float SetScale (float distance) {
        float Scale = 0;
        if (distance < 100) {
            Scale = 0.8f - 0.005f * distance;
        } else {
            Scale = 0.425f - 0.00125f * distance;
        }
        if (Scale <= 0.05f) {
            Scale = 0.05f;
        }
        return Scale;
    }

    // void DrawLine (Vector2 pos) {

    //     float distance = Vector3.Distance (pos, pos);
    //     brushScale = SetScale (distance);
    //     ThreeOrderBézierCurse (pos, distance, 4.5f);

    //     //startPosition = endPosition;
    //     lastDistance = distance;
    // }

    void Clear (RenderTexture destTexture) //画布清零
    {
        Graphics.SetRenderTarget (destTexture);
        GL.PushMatrix ();
        GL.Clear (true, true, Color.white);
        GL.PopMatrix ();
    }

    void DrawBrush (RenderTexture destTexture, Vector2 drawPoint, Texture sourceTexture, Color color, float scale) {
        DrawBrush (destTexture, new Rect (drawPoint.x, drawPoint.y, sourceTexture.width, sourceTexture.height),
            sourceTexture, color, scale);
    }
    void DrawBrush (RenderTexture destTexture, Rect destRect, Texture sourceTexture, Color color, float scale) {

        float left = destRect.xMin - destRect.width * scale / 2.0f;
        float right = destRect.xMin + destRect.width * scale / 2.0f;
        float top = destRect.yMin - destRect.height * scale / 2.0f;
        float bottom = destRect.yMin + destRect.height * scale / 2.0f;

        Graphics.SetRenderTarget (destTexture);

        GL.PushMatrix ();
        GL.LoadOrtho ();

        mat.SetTexture ("_MainTex", sourceTexture);
        mat.SetColor ("_Color", color);
        mat.SetPass (0);

        GL.Begin (GL.QUADS);

        GL.TexCoord2 (0.0f, 0.0f);
        GL.Vertex3 (left / Screen.width, top / Screen.height, 0);
        GL.TexCoord2 (1.0f, 0.0f);
        GL.Vertex3 (right / Screen.width, top / Screen.height, 0);
        GL.TexCoord2 (1.0f, 1.0f);
        GL.Vertex3 (right / Screen.width, bottom / Screen.height, 0);
        GL.TexCoord2 (0.0f, 1.0f);
        GL.Vertex3 (left / Screen.width, bottom / Screen.height, 0);

        GL.End ();
        GL.PopMatrix ();
    }
    //bool bshow = true;

    void ClearPaint (Vector2 point, RenderTexture destTexture, Color color) //清除第i条线
    {
        float left = point.x - 32;
        float right = point.x + 32;
        float top = point.y - 32;
        float bottom = point.y + 32;

        Graphics.SetRenderTarget (destTexture);

        GL.PushMatrix ();
        GL.LoadOrtho ();

        mat_eraser.SetTexture ("_MainTex", clearBrush);
        mat_eraser.SetColor ("_Color", color);
        mat_eraser.SetPass (0);

        GL.Begin (GL.QUADS);

        GL.TexCoord2 (0.0f, 0.0f);
        GL.Vertex3 (left / Screen.width, top / Screen.height, 0);
        GL.TexCoord2 (1.0f, 0.0f);
        GL.Vertex3 (right / Screen.width, top / Screen.height, 0);
        GL.TexCoord2 (1.0f, 1.0f);
        GL.Vertex3 (right / Screen.width, bottom / Screen.height, 0);
        GL.TexCoord2 (0.0f, 1.0f);
        GL.Vertex3 (left / Screen.width, bottom / Screen.height, 0);

        GL.End ();
        GL.PopMatrix ();
    }

    //二阶贝塞尔曲线
    public void TwoOrderBézierCurse (Vector2 pos, float distance, int i) {
        PositionArray1[a[i], i] = pos;
        a[i]++;
        //暂存点
        Vector2 _point;
        if (a[i] == 3) {
            for (int index = 0; index < num; index++) {
                Vector2 middle = (PositionArray1[0, i] + PositionArray1[2, i]) / 2;
                PositionArray1[1, i] = (PositionArray1[1, i] - middle) / 2 + middle;

                float t = (1.0f / num) * index / 2;
                Vector2 target = Mathf.Pow (1 - t, 2) * PositionArray1[0, i] + 2 * (1 - t) * t * PositionArray1[1, i] +
                    Mathf.Pow (t, 2) * PositionArray1[2, i];
                float deltaSpeed = (float) (distance - lastDistance) / num;

                _point = new Vector2 ((int) target.x, (int) target.y);
                DrawBrush (texRender, _point, brushTypeTexture, brushColor, SetScale (lastDistance + (deltaSpeed * index)));
                //记录这些点
                linePoints[i].Add (_point);
            }
            PositionArray1[0, i] = PositionArray1[1, i];
            PositionArray1[1, i] = PositionArray1[2, i];
            a[i] = 2;

        } else {
            _point = new Vector2 ((int) pos.x, (int) pos.y);
            DrawBrush (texRender, _point, brushTypeTexture,
                brushColor, brushScale);
            //记录这些点
            linePoints[i].Add (_point);
        }
    }

    //三阶贝塞尔曲线，获取连续4个点坐标，通过调整中间2点坐标，画出部分（我使用了num/1.5实现画出部分曲线）来使曲线平滑;通过速度控制曲线宽度。
    private void ThreeOrderBézierCurse (Vector3 pos, float distance, float targetPosOffset, int i) {
        //记录坐标
        PositionArray1[b[i], i] = pos;
        b[i]++;
        //记录速度
        speedArray[s[i], i] = distance;
        s[i]++;
        //暂存点
        Vector2 _point;

        if (b[i] == 4) {
            Vector2 temp1 = PositionArray1[1, i];
            Vector2 temp2 = PositionArray1[2, i];

            //修改中间两点坐标
            Vector2 middle = (PositionArray1[0, i] + PositionArray1[2, i]) / 2;
            PositionArray1[1, i] = (PositionArray1[1, i] - middle) * 1.5f + middle;
            middle = (temp1 + PositionArray1[3, i]) / 2;
            PositionArray1[2, i] = (PositionArray1[2, i] - middle) * 2.1f + middle;

            for (int index1 = 0; index1 < num / 1.5f; index1++) {
                float t1 = (1.0f / num) * index1;
                Vector3 target = Mathf.Pow (1 - t1, 3) * PositionArray1[0, i] +
                    3 * PositionArray1[1, i] * t1 * Mathf.Pow (1 - t1, 2) +
                    3 * PositionArray1[2, i] * t1 * t1 * (1 - t1) + PositionArray1[3, i] * Mathf.Pow (t1, 3);
                //float deltaspeed = (float)(distance - lastDistance) / num;
                //获取速度差值（存在问题，参考）
                float deltaspeed = (float) (speedArray[3, i] - speedArray[0, i]) / num;
                //float randomOffset = Random.Range(-1/(speedArray[0] + (deltaspeed * index1)), 1 / (speedArray[0] + (deltaspeed * index1)));
                //模拟毛刺效果
                float randomOffset = Random.Range (-targetPosOffset, targetPosOffset);
                _point = new Vector2 ((int) (target.x + randomOffset), (int) (target.y + randomOffset));
                DrawBrush (texRender, _point,
                    brushTypeTexture, brushColor, SetScale (speedArray[0, i] + (deltaspeed * index1)));

                //记录这些点
                linePoints[i].Add (_point);
            }

            PositionArray1[0, i] = temp1;
            PositionArray1[1, i] = temp2;
            PositionArray1[2, i] = PositionArray1[3, i];

            speedArray[0, i] = speedArray[1, i];
            speedArray[1, i] = speedArray[2, i];
            speedArray[2, i] = speedArray[3, i];
            b[i] = 3;
            s[i] = 3;
        } else {
            _point = new Vector2 ((int) pos.x, (int) pos.y);

            DrawBrush (texRender, _point, brushTypeTexture,
                brushColor, brushScale);
            //记录这些点
            linePoints[i].Add (_point);
        }

    }
}